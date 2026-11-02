using System;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] private FinishCelebration _celebration;

    private bool _hasTriggered;

    public event Action Reached;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered)
        {
            return;
        }

        if (other.GetComponentInParent<Player>() == null)
        {
            return;
        }

        _hasTriggered = true;

        _celebration?.Play();

        Reached?.Invoke();
    }
}