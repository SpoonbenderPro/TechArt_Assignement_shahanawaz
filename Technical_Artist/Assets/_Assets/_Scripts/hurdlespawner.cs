using System.Collections.Generic;
using UnityEngine;

public class hurdlespawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private trafficobstacle[] obstaclePrefabs;

    [SerializeField] private int poolSizePerPrefab = 5;

    [SerializeField] private float[] laneXPositions = new float[] { -2.5f, 0f, 2.5f };
    [SerializeField] private float spawnStartZ = 80f;
    [SerializeField] private float despawnZ = -20f;

    [SerializeField] private float minSpawnGap = 18f;
    [SerializeField] private float maxSpawnGap = 32f;

    [SerializeField] private int initialSpawnCount = 8;

    private readonly List<trafficobstacle> pooledObstacles = new List<trafficobstacle>();
    private float nextSpawnZ;

    private void Start()
    {
        BuildPool();

        if (player == null)
        {
            UnityEngine.Debug.LogError("hurdlespawner: Player reference is missing.");
            enabled = false;
            return;
        }

        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            UnityEngine.Debug.LogError("hurdlespawner: No obstacle prefabs assigned.");
            enabled = false;
            return;
        }

        if (laneXPositions == null || laneXPositions.Length == 0)
        {
            UnityEngine.Debug.LogError("hurdlespawner: No lane positions assigned.");
            enabled = false;
            return;
        }

        nextSpawnZ = player.position.z + spawnStartZ;

        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnNextObstacle();
        }
    }

    private void Update()
    {
        RecyclePassedObstacles();

        while (nextSpawnZ < player.position.z + spawnStartZ)
        {
            SpawnNextObstacle();
        }
    }

    private void BuildPool()
    {
        pooledObstacles.Clear();

        for (int p = 0; p < obstaclePrefabs.Length; p++)
        {
            if (obstaclePrefabs[p] == null)
            {
                continue;
            }

            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                trafficobstacle obj = Instantiate(obstaclePrefabs[p], transform);
                obj.SetActiveState(false);
                pooledObstacles.Add(obj);
            }
        }
    }

    private void SpawnNextObstacle()
    {
        trafficobstacle obstacle = GetFreeObstacle();

        if (obstacle == null)
        {
            return;
        }

        int laneIndex = UnityEngine.Random.Range(0, laneXPositions.Length);
        float laneX = laneXPositions[laneIndex];

        obstacle.transform.position = new Vector3(
            laneX,
            obstacle.transform.position.y,
            nextSpawnZ
        );

        obstacle.SetActiveState(true);

        float nextGap = UnityEngine.Random.Range(minSpawnGap, maxSpawnGap);
        nextSpawnZ += nextGap;
    }

    private void RecyclePassedObstacles()
    {
        float recycleLine = player.position.z + despawnZ;

        for (int i = 0; i < pooledObstacles.Count; i++)
        {
            if (!pooledObstacles[i].gameObject.activeSelf)
            {
                continue;
            }

            if (pooledObstacles[i].transform.position.z < recycleLine)
            {
                pooledObstacles[i].SetActiveState(false);
            }
        }
    }

    private trafficobstacle GetFreeObstacle()
    {
        for (int i = 0; i < pooledObstacles.Count; i++)
        {
            if (!pooledObstacles[i].gameObject.activeSelf)
            {
                return pooledObstacles[i];
            }
        }

        return null;
    }
}