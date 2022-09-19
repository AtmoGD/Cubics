using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public Action<int> OnScoreChanged;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] float gain = 0.5f;
    [SerializeField] float amplitudeMin = 1f;
    [SerializeField] float amplitudeMax = 5f;
    [SerializeField] float frequencyMin = 10.1f;
    [SerializeField] float frequencyMax = 1f;
    [SerializeField] float durationMin = 1f;
    [SerializeField] float durationMax = 2f;
    [SerializeField] public int maxEnemys = 100;
    public static GameController instance;
    public int Score { get; private set; }

    public CubeController Player { get; private set; }
    private CinemachineBasicMultiChannelPerlin noise;

    private float cameraAmplitude;
    private float cameraFrequency;
    private float cameraShakeLeft;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        IsGameOver = false;
        Player = CubeController.instance;
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    private void Update()
    {

        if (SpawnController.instance.EnemyCount > maxEnemys)
        {
            GameOver();
            return;
        }

        if (cameraShakeLeft > 0f)
        {
            cameraShakeLeft -= Time.deltaTime;
            noise.m_AmplitudeGain = cameraAmplitude;
            noise.m_FrequencyGain = cameraFrequency;
        }
        else
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
            cameraAmplitude = 0f;
            cameraFrequency = 0f;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        IsGameOver = true;

        UIController.instance.GameOver();

        StartCoroutine(SpawnController.instance.RemoveAll());
    }

    public void AddScore(int amount, bool addMana = false, float multiAmplitude = 1f, float multiFrequency = 1f, float duration = 0.1f)
    {
        Score += amount;
        OnScoreChanged?.Invoke(Score);
        Player.AddMana(addMana ? amount : 0);

        CameraShake(amount, multiAmplitude, multiFrequency, duration);
    }

    public void CameraShake(int amount, float multiAmplitude = 1f, float multiFrequency = 1f, float duration = 0.1f)
    {
        if (amount == 0) amount = 10;

        // cameraAmplitude += gain * multiAmplitude * amount;
        cameraAmplitude = gain * multiAmplitude * (amount > 0 ? amount : 1);
        cameraAmplitude = Mathf.Clamp(cameraAmplitude, amplitudeMin, amplitudeMax);
        // cameraFrequency += gain * multiFrequency * amount;
        cameraFrequency = gain * multiFrequency * amount;
        cameraFrequency = Mathf.Clamp(cameraFrequency, frequencyMin, frequencyMax);
        // cameraShakeLeft += duration * amount;
        cameraShakeLeft = duration * amount;
        cameraShakeLeft = Mathf.Clamp(cameraShakeLeft, durationMin, durationMax);
    }
}
