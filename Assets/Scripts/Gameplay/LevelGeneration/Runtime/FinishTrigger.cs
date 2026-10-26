using System;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private bool _hasTriggered;

    public event Action Reached;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered)
        {
            return;
        }

        if (other.GetComponent<Player>() == null)
        {
            return;
        }

        _hasTriggered = true;
        Reached?.Invoke();
    }
}