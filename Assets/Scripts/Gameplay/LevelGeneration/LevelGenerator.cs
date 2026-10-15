using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private TrackSection[] _sections;
    [SerializeField] private Transform _generatedRoot;
    [SerializeField] private ObstaclePlacer _obstaclePlacer;
    [SerializeField] private UpgradeGatePlacer _upgradeGatePlacer;

    [Header("Level Length")]
    [SerializeField, Min(1)] private int _earlyChallengeBlocks = 1;
    [SerializeField, Min(1)] private int _midChallengeBlocks = 2;
    [SerializeField, Min(1)] private int _highChallengeBlocks = 3;

    private SectionSequenceBuilder _sequenceBuilder;
    private readonly List<GameObject> _spawnedSections = new();
    private readonly List<FinishTrigger> _finishTriggers = new();

    public event Action Finished;

    public void BuildLevel(int level, int seed, RunModifiers modifiers)
    {
        if (_generatedRoot == null || _obstaclePlacer == null || _upgradeGatePlacer == null)
        {
            Debug.LogError("LevelGenerator references are missing.", this);
            return;
        }

        if (_sections == null || _sections.Length == 0)
        {
            Debug.LogError("Track sections are not assigned.", this);
            return;
        }

        _sequenceBuilder ??= new SectionSequenceBuilder(_sections);

        List<TrackSection> sequence;

        try
        {
            sequence = _sequenceBuilder.Build(level, seed, GetChallengeBlockCount(level));
        }
        catch (ArgumentOutOfRangeException exception)
        {
            Debug.LogError($"Invalid level generation arguments: {exception.Message}", this);
            return;
        }
        catch (InvalidOperationException exception)
        {
            Debug.LogError($"Failed to build level sequence: {exception.Message}", this);
            return;
        }

        ClearLevel();
        _obstaclePlacer.BeginRun();

        Vector3 nextPosition = transform.position;
        Quaternion nextRotation = transform.rotation;

        for (int i = 0; i < sequence.Count; i++)
        {
            TrackSection section = sequence[i];

            GameObject instance = Instantiate(section.Prefab, _generatedRoot);
            TrackSectionAnchors anchors = instance.GetComponent<TrackSectionAnchors>();

            if (anchors == null || anchors.StartAnchor == null || anchors.EndAnchor == null)
            {
                Debug.LogError($"TrackSection '{section.name}' has missing anchors.", instance);
                Destroy(instance);
                ClearLevel();
                return;
            }

            AlignSection(instance.transform, anchors.StartAnchor, nextPosition, nextRotation);

            _spawnedSections.Add(instance);

            if (section.Type == TrackSectionType.Finish)
            {
                FinishTrigger finishTrigger = instance.GetComponentInChildren<FinishTrigger>(true);

                if (finishTrigger == null)
                {
                    Debug.LogError($"FinishTrigger is missing in section '{section.name}'.", instance);
                }
                else
                {
                    finishTrigger.Reached += OnFinishReached;
                    _finishTriggers.Add(finishTrigger);
                }
            }

            if (section.Type == TrackSectionType.Bonus)
            {
                _upgradeGatePlacer.PlaceGates(instance.transform, seed, i, modifiers);
            }

            _obstaclePlacer.PlaceObstacles(instance.transform, level, section.Type, modifiers != null ? modifiers.IncomeMultiplier : 1f, seed, i);

            nextPosition = anchors.EndAnchor.position;
            nextRotation = anchors.EndAnchor.rotation;
        }
    }

    public void ClearLevel()
    {
        foreach (FinishTrigger trigger in _finishTriggers)
        {
            if (trigger != null)
            {
                trigger.Reached -= OnFinishReached;
            }
        }

        _finishTriggers.Clear();

        foreach (GameObject section in _spawnedSections)
        {
            if (section != null)
            {
                Destroy(section);
            }
        }

        _spawnedSections.Clear();
    }

    private int GetChallengeBlockCount(int level)
    {
        if (level <= 3)
        {
            return _earlyChallengeBlocks;
        }

        if (level <= 15)
        {
            return _midChallengeBlocks;
        }

        return _highChallengeBlocks;
    }

    private static void AlignSection(Transform sectionRoot, Transform startAnchor, Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 localPosition = sectionRoot.InverseTransformPoint(startAnchor.position);
        Quaternion localRotation = Quaternion.Inverse(sectionRoot.rotation) * startAnchor.rotation;

        Quaternion rootRotation = targetRotation * Quaternion.Inverse(localRotation);
        sectionRoot.rotation = rootRotation;
        sectionRoot.position = targetPosition - rootRotation * localPosition;
    }

    private void OnDestroy()
    {
        ClearLevel();
    }

    private void OnFinishReached()
    {
        Finished?.Invoke();
    }
}