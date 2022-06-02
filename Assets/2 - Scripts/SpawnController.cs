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
    [SerializeField] private float dieAllDelayMin = 0.2f;
    [SerializeField] private float dieAllDelayMax = 0.8f;
    [SerializeField] private int spawnSize;
    [SerializeField] private float waveTime;
    [SerializeField] private float waveTimeMin;
    [SerializeField] private float waveTimeSpeedIncrease;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<WallController> walls = new List<WallController>();

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
        if (autoStart)
            GenerateRoom();
    }

    public void GenerateRoom()
    {
        int wallCount = UnityEngine.Random.Range(objectsCountMin, objectsCountMax);
        for (int i = 0; i < wallCount; i++)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-spawnSize, spawnSize), UnityEngine.Random.Range(-spawnSize, spawnSize), 0);
            GameObject cube = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];
            WallController newWall = Instantiate(cube, spawnPoint.position + pos, Quaternion.identity, transform).GetComponent<WallController>();
            walls.Add(newWall);
        }

        WallCount += wallCount;
        waveTime = waveTime < waveTimeMin ? waveTimeMin : waveTime - waveTimeSpeedIncrease;
        WaveCount++;
        OnWaveChanged?.Invoke(WaveCount);


        StartCoroutine(Spawn());
    }

    public IEnumerator RemoveAll()
    {
        WallCount = 0;
        try
        {
            foreach (WallController wall in walls)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(dieAllDelayMin, dieAllDelayMax));
                wall.Die(Vector2.zero, 0, false, false, false);
            }
        }
        finally
        {
            walls.Clear();
        }
    }

    // public void RemoveAllWalls()
    // {
    //     foreach (WallController wall in walls)
    //     {
    //         float randomDelay = UnityEngine.Random.Range(dieAllDelayMin, dieAllDelayMax);
    //         wall.Die(Vector2.zero, randomDelay, false, false, false);
    //     }

    //     walls.Clear();
    //     WallCount = 0;
    // }

    public void WallDie(WallController wall, int amount = 1)
    {
        WallCount -= amount;
        walls.Remove(wall);
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(waveTime);
        GenerateRoom();
    }
}
