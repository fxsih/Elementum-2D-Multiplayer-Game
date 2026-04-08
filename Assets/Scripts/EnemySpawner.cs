using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    // 🔥 LEVEL-BASED ENEMY DATA (FINAL SYSTEM)
    [System.Serializable]
    public class LevelEnemyData
    {
        public GameObject prefab;
        public int baseCount;

        [Header("Weight")]
        public int weight = 1;
    }

    [System.Serializable]
    public class LevelSpawnData
    {
        public int level;

        [Header("Base Enemies")]
        public List<LevelEnemyData> enemies;

        [Header("Max Enemies")]
        public int maxEnemies = 30;
    }

    // 🔥 INSPECTOR
    public List<LevelSpawnData> levelConfigs;

    public float spawnRate = 0.5f;

    [Header("Spawn Distance")]
    public float spawnDistanceFromCamera = 2f;

    [Header("Blocked Layers")]
    public LayerMask blockedLayers;

    [Header("Difficulty Scaling")]
    public float increaseInterval = 10f;
    public int increaseAmount = 5;

    Camera cam;
    Transform player;

    float timer;
    float difficultyTimer;

    int targetEnemyCount;
    int lastLevel = -1;

    LevelSpawnData currentLevelData;

    // ---------------- START ----------------
    void Start()
    {
        cam = Camera.main;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        SetupLevel();
        SpawnLevelBaseEnemies();
    }

    // ---------------- LEVEL SETUP ----------------
    void SetupLevel()
    {
        int level = GameManager.Instance.currentLevel;

        currentLevelData = levelConfigs.Find(l => l.level == level);

        // 🔥 fallback if level not defined
        if (currentLevelData == null && levelConfigs.Count > 0)
        {
            currentLevelData = levelConfigs[levelConfigs.Count - 1];
        }

        targetEnemyCount = GetBaseEnemyCount();
        difficultyTimer = 0f;

        Debug.Log("Spawner updated to level: " + level);
    }

    int GetBaseEnemyCount()
    {
        if (currentLevelData == null) return 5;

        int total = 0;

        foreach (var e in currentLevelData.enemies)
            total += e.baseCount;

        return total;
    }

    // ---------------- UPDATE ----------------
    void Update()
    {
        if (player == null) return;

        int currentLevel = GameManager.Instance.currentLevel;

        // 🔥 LEVEL CHANGE DETECTION
        if (currentLevel != lastLevel)
        {
            lastLevel = currentLevel;
            SetupLevel();
        }

        int currentEnemies = EnemyController.ActiveEnemies;
        int maxEnemies = currentLevelData != null ? currentLevelData.maxEnemies : 50;

        // 🔥 INCREASE TARGET OVER TIME
        difficultyTimer += Time.deltaTime;

        if (difficultyTimer >= increaseInterval)
        {
            difficultyTimer = 0f;

            targetEnemyCount += increaseAmount;
            targetEnemyCount = Mathf.Min(targetEnemyCount, maxEnemies);
        }

        // 🔥 STOP IF REACHED TARGET
        if (currentEnemies >= targetEnemyCount)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            timer = 0f;

            int remaining = targetEnemyCount - currentEnemies;

            // 🔥 SMALL CONTROLLED SPAWN
            int spawnPerTick = Mathf.Min(2, remaining);

            SpawnEnemies(spawnPerTick);
        }
    }

    // ---------------- BASE SPAWN ----------------
    void SpawnLevelBaseEnemies()
    {
        if (currentLevelData == null) return;

        foreach (var enemy in currentLevelData.enemies)
        {
            for (int i = 0; i < enemy.baseCount; i++)
            {
                SpawnSingle(enemy.prefab);
            }
        }
    }

    // ---------------- SPAWN SYSTEM ----------------
    void SpawnEnemies(int spawnAmount)
    {
        if (currentLevelData == null) return;

        List<LevelEnemyData> available = currentLevelData.enemies;

        if (available == null || available.Count == 0) return;

        int maxEnemies = currentLevelData.maxEnemies;

        int spawned = 0;

        while (spawned < spawnAmount)
        {
            // 🔥 HARD LIMIT CHECK
            if (EnemyController.ActiveEnemies >= maxEnemies)
                return;

            LevelEnemyData chosen = GetWeightedEnemy(available);

            SpawnSingle(chosen.prefab);

            spawned++;
        }
    }

    // 🔥 WEIGHT SYSTEM
    LevelEnemyData GetWeightedEnemy(List<LevelEnemyData> list)
    {
        int totalWeight = 0;

        foreach (var e in list)
            totalWeight += e.weight;

        int rand = Random.Range(0, totalWeight);

        foreach (var e in list)
        {
            if (rand < e.weight)
                return e;

            rand -= e.weight;
        }

        return list[0];
    }

    // ---------------- SPAWN POSITION ----------------
    void SpawnSingle(GameObject prefab)
    {
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        float radius = Mathf.Max(width, height) + spawnDistanceFromCamera;

        for (int i = 0; i < 15; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;

            if (dir == Vector2.zero)
                dir = Vector2.right;

            Vector2 spawnPos = (Vector2)player.position + dir * radius;

            // outside camera
            Vector3 view = cam.WorldToViewportPoint(spawnPos);
            if (view.x > 0 && view.x < 1 && view.y > 0 && view.y < 1)
                continue;

            // blocked
            if (Physics2D.OverlapCircle(spawnPos, 0.3f, blockedLayers))
                continue;

            // A* walkable
            var nn = AstarPath.active.GetNearest(spawnPos);

            if (nn.node != null && nn.node.Walkable)
            {
                Instantiate(prefab, (Vector3)nn.position, Quaternion.identity);
                return;
            }
        }
    }
}