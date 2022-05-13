using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class GameController : MonoBehaviour
{
    public Action<int> OnScoreChanged;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] float gain = 0.5f;
    public static GameController instance;
    public int Score { get; private set; }
    public CubeController Player { get; private set; }
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake() {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        
    }

    private void Start() {
        Player = CubeController.instance;
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }
    // Add Score
    public void AddScore(int amount, bool addMana = false, float multiAmplitude = 0.52f, float multiFrequency = 0.56f, float duration = 0.28f) {
        Score += amount;
        OnScoreChanged?.Invoke(Score);
        Player.AddMana(addMana ? amount : 0); 
        StartCoroutine(CameraShake(multiAmplitude, multiFrequency, duration, amount));
    }

    public IEnumerator CameraShake(float multiplierAmplitude, float multiplierFrequency,  float duration, float multiplier = 1f) {
        noise.m_AmplitudeGain = multiplierAmplitude * gain * multiplier;
        noise.m_FrequencyGain = multiplierFrequency * gain * multiplier;
        yield return new WaitForSeconds(duration * gain);
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;

    }
}
