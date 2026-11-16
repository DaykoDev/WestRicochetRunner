using UnityEngine;
using WestRicochet.Gameplay.Obstacles.Core;

namespace WestRicochet.Gameplay.Obstacles.Spawning
{
    [DisallowMultipleComponent]
    public sealed class ObstacleAreaReleaser : MonoBehaviour
    {
        [SerializeField] private LayerMask _obstacleMask;

        [SerializeField, Min(4)] private int _bufferSize = 32;

        private Collider[] _hits;

        private void Awake()
        {
            _hits = new Collider[_bufferSize];
        }

        public int Release(Vector3 center, float radius)
        {
            if (_hits == null || radius <= 0f)
            {
                return 0;
            }

            int hitCount = Physics.OverlapSphereNonAlloc(
                center,
                radius,
                _hits,
                _obstacleMask,
                QueryTriggerInteraction.Ignore);

            int releasedCount = 0;

            for (int i = 0; i < hitCount; i++)
            {
                Obstacle obstacle =
                    _hits[i].GetComponentInParent<Obstacle>();

                if (obstacle == null || obstacle.IsDestroyed)
                {
                    continue;
                }

                obstacle.ReleaseWithoutReward();
                releasedCount++;
            }

            return releasedCount;
        }
    }
}