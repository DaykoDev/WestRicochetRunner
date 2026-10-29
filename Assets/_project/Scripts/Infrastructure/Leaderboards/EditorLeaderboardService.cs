using UnityEngine;

public sealed class EditorLeaderboardService : ILeaderboardService
{
    public void SubmitScore(int score)
    {
        Debug.Log($"[Leaderboard] New best level submitted: {score}");
    }
}