namespace ET.Monetization
{
    public enum AdCheckReason
    {
        None = 0,
        DailyMissionAdWatch = 100,
        CollectCrate = 200,
        CollectCrateClaimed = 201,
        Hint = 300,
        DailyRewardDouble = 400,
        DailyRewardDoubleClaimed = 401,
        DailyMissionDouble = 500,
        DailyMissionDoubleClaimed = 501,
        AdBreak = 600,
        AdBreakClaimed = 601,
        D7Incentive = 700,
        D7IncentiveClaimed = 701,
        CardMilestoneReward = 800,
        CardMilestoneRewardClaimed = 801,
    }
}