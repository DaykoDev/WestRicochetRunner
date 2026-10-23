using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform[] _points;
    [SerializeField] private Transform[] _widePoints;

    public IReadOnlyList<Transform> Points => _points;
    public IReadOnlyList<Transform> WidePoints => _widePoints;
}