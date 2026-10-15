using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform[] _points;

    public IReadOnlyList<Transform> Points => _points;
}