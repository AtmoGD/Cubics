using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HighscorePlugin;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    [SerializeField] private Highscores highscores;
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text endScoreText;
    [SerializeField] private List<TMP_Text> highscoreNames;
    [SerializeField] private List<TMP_Text> highscoreScores;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject submitScoreField;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemyCountText;
    [SerializeField] private Image dashCooldownImage;
    [SerializeField] private Image shieldCooldownImage;
    [SerializeField] private Slider worldEnemyCountSlider;
    [SerializeField] private List<Image> manaImages = new List<Image>();
    [SerializeField] private float shieldActiveTime = 0f;
    [SerializeField] private float worldMaxEnemyLerpSpeed = 1f;

    private void Awake() {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Start()
    {
        Time.timeScale = 1f;

        startUI.SetActive(true);
        gameUI.SetActive(false);
        pauseUI.SetActive(false);
        endUI.SetActive(false);

        GameController.instance.OnScoreChanged += OnScoreChanged;
        SpawnController.instance.OnWaveChanged += OnWaveChanged;

        scoreText.text = GameController.instance.Score.ToString();
        waveText.text = SpawnController.instance.WaveCount.ToString();
    }

    public void StartGame()
    {
        startUI.SetActive(false);
        gameUI.SetActive(true);
        SpawnController.instance.GenerateRoom();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseUI.SetActive(false);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        GameController.instance.ReloadScene();
    }

    public void GameOver()
    {
        // Time.timeScale = 0;

        // gameUI.SetActive(false);

        highscores.GetHighscores(3);
        highscores.OnHighscoresReceived += UpdateHighscores;

        // if(GameController.instance.Score > GameController.instance.HighScore)
        //     GameController.instance.HighScore = GameController.instance.Score;
            
        endUI.SetActive(true);
        gameUI.SetActive(false);

        endScoreText.text = GameController.instance.Score.ToString();
        // highScoreText.text = GameController.instance.HighScore.ToString();
    }

    private void Update() {
        float newSliderValue = (float)SpawnController.instance.WallCount / (float)GameController.instance.maxEnemys;
        worldEnemyCountSlider.value = Mathf.Lerp(worldEnemyCountSlider.value, newSliderValue, Time.deltaTime * worldMaxEnemyLerpSpeed);

        shieldActiveTime -= Time.deltaTime;

        dashCooldownImage.fillAmount = CubeController.instance.DashCooldown / CubeController.instance.DashDuration;
        shieldCooldownImage.fillAmount = CubeController.instance.ShieldTimeLeft > 0f ? CubeController.instance.ShieldTimeLeft / CubeController.instance.ShieldDuration : 0f;

        enemyCountText.text = SpawnController.instance.WallCount.ToString() + " / " + GameController.instance.maxEnemys.ToString();

        for(int i = 0; i < manaImages.Count; i++) {
            if(i < CubeController.instance.ManaSections)
                manaImages[i].gameObject.SetActive(true);
            else
                manaImages[i].gameObject.SetActive(false);
        }
    }

    public void UpdateHighscores(List<SingleNameScore> scores) {
        foreach(SingleNameScore score in scores) {
            highscoreNames[scores.IndexOf(score)].text = score.name;
            highscoreScores[scores.IndexOf(score)].text = score.score.ToString();
        }
    }

    public void SubmitScore() {
        highscores.CreateHighscore(playerName.text, GameController.instance.Score);
        highscores.GetHighscores(3);

        submitScoreField.SetActive(false);
    }

    public void OnScoreChanged(int score)
    {
        scoreText.text = score.ToString();
    }

    public void OnWaveChanged(int wave)
    {
        waveText.text = wave.ToString();
    }
}
