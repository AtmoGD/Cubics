using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnController : MonoBehaviour
{
    public static SpawnController instance;
    public Action<int> OnWaveChanged;
    [SerializeField] private bool autoStart = false;
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] private int objectsCountMin;
    [SerializeField] private int objectsCountMax;
    [SerializeField] private int spawnSize;
    [SerializeField] private float waveTime;
    [SerializeField] private float waveTimeMin;
    [SerializeField] private float waveTimeSpeedIncrease;
    [SerializeField] private Transform spawnPoint;

    public int WallCount { get; private set; }
    public int WaveCount { get; private set; }

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Start()
    {
        WaveCount = 0;
        if(autoStart)
            GenerateRoom();
    }

    public void GenerateRoom()
    {
        int wallCount = UnityEngine.Random.Range(objectsCountMin, objectsCountMax);
        for (int i = 0; i < wallCount; i++)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-spawnSize, spawnSize), UnityEngine.Random.Range(-spawnSize, spawnSize), 0);
            GameObject cube = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];
            Instantiate(cube, spawnPoint.position + pos, Quaternion.identity, transform);
        }

        WallCount += wallCount;
        waveTime = waveTime < waveTimeMin ? waveTimeMin : waveTime - waveTimeSpeedIncrease;
        WaveCount++;
        OnWaveChanged?.Invoke(WaveCount);


        StartCoroutine(Spawn());
    }

    public void WallDie(int amount = 1)
    {
        WallCount -= amount;
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(waveTime);
        GenerateRoom();
    }
}
