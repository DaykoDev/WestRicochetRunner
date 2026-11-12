using System;

public interface ILeaderboardService
{
    bool IsAuthorized { get; }

    event Action AuthorizationChanged;

    void SubmitScore(int score);
    void RequestAuthorization();
}