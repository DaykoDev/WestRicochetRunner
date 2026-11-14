namespace WestRicochet.Gameplay.Level.Generation
{
    public static class SectionSeedUtility
    {
        private const int LevelSeedMultiplier = 397;
        private const int SectionIndexMultiplier = 7919;

        public static int Create(
            int levelSeed,
            int sectionIndex,
            int salt = 0)
        {
            return unchecked(
                levelSeed * LevelSeedMultiplier ^
                (sectionIndex + 1) * SectionIndexMultiplier ^
                salt);
        }
    }
}