using UnityEngine;

public class UpgradeGateSpawnPoints : MonoBehaviour
{
    [SerializeField] private Transform[] _points;

    public Transform[] Points => _points;
}