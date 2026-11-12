using System;
using UnityEngine;

public sealed class EditorLeaderboardService : ILeaderboardService
{
    public event Action AuthorizationChanged;

    public bool IsAuthorized => false;

    public void SubmitScore(int score)
    {
        Debug.Log($"[Leaderboard] New best level submitted: {score}");
    }

    public void RequestAuthorization()
    {
        Debug.Log("[Leaderboard] Authorization requested.");
        AuthorizationChanged?.Invoke();
    }
}