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
        public List<Fixture> FindFixtures(List<Fixture> allFixtures, Fixture fixture, int depth)
        {
            List<Fixture> result = new List<Fixture>(depth);
            int startIdx = allFixtures.Count - 1;
            for (; startIdx >= 0  && (allFixtures[startIdx].fixtureId != fixture.fixtureId); --startIdx)
            {}

            for (int i = startIdx - 1; (i >= 0) && (result.Count < depth); --i)
            {
                result.Add(allFixtures[i]);
            }

            if (result.Count != depth)
            {
                throw new ArgumentException("Not enough fixtures to satisfy metric depth");
            }

            return result;
        }

        public MetricConfig config;
        public int year;
        public ConfigManagerInterface configManager_;
        public FixtureRetrieverInterface fixtureRetriever_;
    }
}