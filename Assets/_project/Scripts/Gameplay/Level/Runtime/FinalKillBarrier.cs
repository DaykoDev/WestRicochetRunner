using UnityEngine;
using WestRicochet.Gameplay.Player.Damage;

namespace WestRicochet.Gameplay.Level.Runtime
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class FinalKillBarrier : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Kill();
            }
        }
    }
}