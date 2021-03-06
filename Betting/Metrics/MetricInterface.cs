﻿using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System;

namespace Betting.Metrics
{
    public abstract class MetricInterface
    {
        public abstract void GetPoints(out double pTeam1, out double pTeam2, int teamId1, int teamId2, Fixture fixture);
        public abstract void GetPercentage(out int pTeam1, out int pTeam2, int teamId1, int teamId2, Fixture fixture);

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
        public int FindFixtures(int year, int teamId, int fixtureId, int depth)
        {
            int startIdx = fixtureRetriever_.FindFixtureIndex(year, teamId, fixtureId);

            --startIdx;

            if ((startIdx + 1) < depth)//+1 as startIdx is 0-indexed
            {
                //comment out for update
                //throw new ArgumentException("Not enough fixtures to satisfy metric depth");
            }
            return startIdx;
        }

        public double GetCoeficient(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
            {
                return fixture.coeficient.awayTeam;
            }
            else
            {
                return fixture.coeficient.homeTeam;
            }
        }

        public MetricConfig config;
        public int year;
        public ConfigManagerInterface configManager_;
        public FixtureRetrieverInterface fixtureRetriever_;
    }
}