using UnityEngine;
using WestRicochet.Gameplay.Balls.Shooting;

namespace WestRicochet.Gameplay.Player.Visuals
{
    public sealed class RevolverMuzzleVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BallLauncher _ballLauncher;
        [SerializeField] private ParticleSystem _muzzleFlash;

        [Header("Revolver")]
        [SerializeField, Range(0, 1)] private int _revolverIndex;

        private void OnEnable()
        {
            if (_ballLauncher != null)
            {
                _ballLauncher.Fired += OnFired;
            }

            StopEffect(_muzzleFlash);
        }

        private void OnDisable()
        {
            if (_ballLauncher != null)
            {
                _ballLauncher.Fired -= OnFired;
            }

            StopEffect(_muzzleFlash);
        }

        private void OnFired(int revolverIndex)
        {
            if (revolverIndex != _revolverIndex)
            {
                return;
            }

            PlayFlash();
        }

        private void PlayFlash()
        {
            if (_muzzleFlash == null)
            {
                return;
            }

            _muzzleFlash.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            _muzzleFlash.Play(true);
        }

        private static void StopEffect(ParticleSystem effect)
        {
            if (effect == null)
            {
                return;
            }

            effect.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}