using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public abstract class MetricInterface
    {
        abstract public void GetPoints(out int pTeam1, out int pTeam2, int teamId1, int teamId2, Fixture fixture);
        abstract public void GetPercentage(out int pTeam1, out int pTeam2, int teamId1, int teamId2, Fixture fixture);

        //For mocking/testing
        public MetricInterface()
        { }

        public MetricInterface(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever)
        {
            this.config = config;
            this.year = year;
            fixtureRetriever_ = fixtureRetriever;
            configManager_ = configManager;
        }

        // Find <depth> fixtures with starting from <fixture>
        public int FindFixtures(List<Fixture> allFixtures, int fixtureId, int depth)
        {
            int startIdx = allFixtures.FindLastIndex(thisFix => thisFix.fixtureId == fixtureId);

            --startIdx;

            if ((startIdx+1) < depth)//+1 as startIdx is 0-indexed
            {
                throw new ArgumentException("Not enough fixtures to satisfy metric depth");
            }
            return startIdx;
        }

        public MetricConfig config;
        public int year;
        public ConfigManagerInterface configManager_;
        public FixtureRetrieverInterface fixtureRetriever_;
    }
}