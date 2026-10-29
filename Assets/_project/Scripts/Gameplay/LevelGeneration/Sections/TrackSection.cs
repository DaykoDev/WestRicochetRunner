using UnityEngine;

[CreateAssetMenu(fileName = "TrackSection", menuName = "Game/Level Generation/Track Section")]
public class TrackSection : ScriptableObject
{
    [Header("Section")]
    [SerializeField] private TrackSectionType _type;
    [SerializeField] private GameObject _prefab;

    [Header("Availability")]
    [SerializeField, Min(1)] private int _minLevel = 1;
    [SerializeField, Min(0)] private int _maxLevel;

    [Header("Selection")]
    [SerializeField, Min(1)] private int _selectionWeight = 1;

    public TrackSectionType Type => _type;
    public GameObject Prefab => _prefab;
    public int SelectionWeight => _selectionWeight;

    public bool IsAvailableForLevel(int level)
    {
        if (level < _minLevel)
        {
            return false;
        }

        return _maxLevel <= 0 || level <= _maxLevel;
    }
}