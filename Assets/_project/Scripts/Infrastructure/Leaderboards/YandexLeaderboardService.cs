using YG;

public sealed class YandexLeaderboardService : ILeaderboardService
{
    public void SubmitScore(int score)
    {
        if (YG2.isSDKEnabled == false)
        {
            return;
        }

        YG2.SetLeaderboard(LeaderboardNames.BestLevel, score);
    }
}