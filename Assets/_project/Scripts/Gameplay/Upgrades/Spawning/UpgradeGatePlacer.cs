using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeGatePlacer : MonoBehaviour
{
    [Header("Gate Prefabs")]
    [SerializeField] private GameObject[] _gatePrefabs;

    [Header("Gate Count")]
    [SerializeField, Min(1)] private int _minGates = 1;
    [SerializeField, Min(1)] private int _maxGates = 2;

    public void PlaceGates(Transform sectionRoot, int seed, int sectionIndex, RunModifiers modifiers)
    {
        if (sectionRoot == null || _gatePrefabs == null || _gatePrefabs.Length == 0)
        {
            Debug.LogError("UpgradeGatePlacer references are missing.", this);
            return;
        }

        UpgradeGateSpawnPoints spawnPoints = sectionRoot.GetComponent<UpgradeGateSpawnPoints>();

        if (spawnPoints == null || spawnPoints.Points == null)
        {
            Debug.LogWarning($"No gate spawn points in section '{sectionRoot.name}'.", sectionRoot);
            return;
        }

        var points = new List<Transform>();

        foreach (Transform point in spawnPoints.Points)
        {
            if (point != null)
            {
                points.Add(point);
            }
        }

        var prefabs = new List<GameObject>();

        foreach (GameObject prefab in _gatePrefabs)
        {
            if (prefab != null)
            {
                prefabs.Add(prefab);
            }
        }

        if (points.Count == 0 || prefabs.Count == 0)
        {
            return;
        }

        var random = new System.Random(CreateSeed(seed, sectionIndex));

        Shuffle(points, random);
        Shuffle(prefabs, random);

        int maxPossible = Mathf.Min(points.Count, prefabs.Count);

        if (maxPossible == 0)
        {
            return;
        }

        int minCount = Mathf.Clamp(_minGates, 1, maxPossible);
        int maxCount = Mathf.Clamp(_maxGates, minCount, maxPossible);
        int gateCount = random.Next(minCount, maxCount + 1);

        var spawnedGates = new List<UpgradeGateApplier>();

        for (int i = 0; i < gateCount; i++)
        {
            Transform point = points[i];
            GameObject prefab = prefabs[i];

            GameObject gate = Instantiate(prefab, point.position, point.rotation, sectionRoot);

            UpgradeGateApplier applier = gate.GetComponentInChildren<UpgradeGateApplier>(true);

            if (applier == null)
            {
                Debug.LogError($"Prefab '{prefab.name}' has no UpgradeGateApplier.", gate);
                Destroy(gate);
                continue;
            }

            applier.SetRunModifiers(modifiers);
            spawnedGates.Add(applier);
        }

        if (spawnedGates.Count > 1)
        {
            UpgradeGateGroup group = sectionRoot.gameObject.AddComponent<UpgradeGateGroup>();

            foreach (UpgradeGateApplier gate in spawnedGates)
            {
                group.Register(gate);
            }
        }
    }

    private static void Shuffle<T>(List<T> items, System.Random random)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }

    private static int CreateSeed(int seed, int sectionIndex)
    {
        return unchecked(seed * 397 ^ (sectionIndex + 1) * 7919 ^ 15485863);
    }
}