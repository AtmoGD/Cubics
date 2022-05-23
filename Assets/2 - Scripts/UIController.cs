using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemyCountText;
    // [SerializeField] private TMP_Text manaSectionText;
    // [SerializeField] private Image manaSectionImage;
    [SerializeField] private Image dashCooldownImage;
    [SerializeField] private Image shieldCooldownImage;
    [SerializeField] private Slider worldEnemyCountSlider;
    [SerializeField] private List<Image> manaImages = new List<Image>();
    [SerializeField] private float shieldActiveTime = 0f;
    [SerializeField] private float worldMaxEnemyLerpSpeed = 1f;

    public void Start()
    {
        GameController.instance.OnScoreChanged += OnScoreChanged;
        SpawnController.instance.OnWaveChanged += OnWaveChanged;

        scoreText.text = GameController.instance.Score.ToString();
        waveText.text = SpawnController.instance.WaveCount.ToString();
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

    public void OnScoreChanged(int score)
    {
        scoreText.text = score.ToString();
    }

    public void OnWaveChanged(int wave)
    {
        waveText.text = wave.ToString();
    }
}
