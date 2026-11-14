using UnityEngine;

namespace WestRicochet.Gameplay.Level.Sections
{
    public class TrackSectionAnchors : MonoBehaviour
    {
        [SerializeField] private Transform _startAnchor;
        [SerializeField] private Transform _endAnchor;

        public Transform StartAnchor => _startAnchor;
        public Transform EndAnchor => _endAnchor;
    }
}