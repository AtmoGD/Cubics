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
    [SerializeField] private List<Image> manaImages = new List<Image>();
    [SerializeField] private float shieldActiveTime = 0f;

    public void Start()
    {
        GameController.instance.OnScoreChanged += OnScoreChanged;
        SpawnController.instance.OnWaveChanged += OnWaveChanged;

        scoreText.text = GameController.instance.Score.ToString();
        waveText.text = SpawnController.instance.WaveCount.ToString();
    }

    private void Update() {
        shieldActiveTime -= Time.deltaTime;
        // manaSectionText.text = "Mana: " + CubeController.instance.ManaSections;
        // manaSectionImage.fillAmount = CubeController.instance.Mana / CubeController.instance.GetMaxMana();

        dashCooldownImage.fillAmount = CubeController.instance.DashCooldown / CubeController.instance.DashDuration;
        // if(dashCooldownImage.fillAmount > 0f)
        //     dashCooldownImage.gameObject.SetActive(true);
        // else
        //     dashCooldownImage.gameObject.SetActive(false);

        shieldCooldownImage.fillAmount = CubeController.instance.ShieldTimeLeft > 0f ? CubeController.instance.ShieldTimeLeft / CubeController.instance.ShieldDuration : 0f;

        enemyCountText.text = SpawnController.instance.WallCount.ToString() + " / " + GameController.instance.maxEnemys.ToString();
        // if(shieldCooldownImage.fillAmount > 0f)
        //     shieldCooldownImage.gameObject.SetActive(true);
        // else
        //     shieldCooldownImage.gameObject.SetActive(false);

        for(int i = 0; i < manaImages.Count; i++) {
            if(i < CubeController.instance.ManaSections)
                manaImages[i].gameObject.SetActive(true);
            else
                manaImages[i].gameObject.SetActive(false);
        }

        // for (int i = 1; i < manaImages.Count + 1; i++)
        //     manaImages[i - 1].fillAmount = CubeController.instance.ManaSections >= i ? 1 : 0;
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
