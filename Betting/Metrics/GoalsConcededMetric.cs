﻿using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public class GoalsConcededMetric : MetricInterface
    {
        public GoalsConcededMetric(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever) : base(config, year, configManager, fixtureRetriever)
        {
        }

        private void GetTeamPoints(out int pTeam, int teamId, Fixture fixture)
        {
            double pctTeam = 0;

            List<Fixture> allT1 = fixtureRetriever_.GetAllFixtures(year, teamId);
            List<Fixture> fixturesTeam1 = FindFixtures(allT1, fixture, config.depth);
            foreach (Fixture fix in fixturesTeam1)
            {
                pctTeam += GetGoals(fix, teamId) * GetCoeficient(fix, teamId);
            }
           
            pTeam = (int)pctTeam;
        }

        public override void GetPoints(out int pTeam1, out int pTeam2, int teamId1, int teamId2, Fixture fixture)
        {
            GetTeamPoints(out pTeam2, teamId1, fixture);//reverse
            GetTeamPoints(out pTeam1, teamId2, fixture);//reverse
        }

        public override void GetPercentage(out int pTeam1, out int pTeam2, int teamId1, int teamId2, Fixture fixture)
        {
            GetPoints(out int pctTeam1, out int pctTeam2, teamId1, teamId2, fixture);

            if (pctTeam1 == 0 && pctTeam2 == 0)
                pTeam1 = 50;
            else
                pTeam1 = (int)((double)pctTeam1 / ((double)pctTeam1 + (double)pctTeam2) * 100);
            pTeam2 = 100 - pTeam1;
        }

        public int GetGoals(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.finalScore.awayTeamGoals;
            else
                return fixture.finalScore.homeTeamGoals;
        }

        public double GetCoeficient(Fixture fixture, int teamId)
        {
            if (!configManager_.GetUseExpanded())
                return 1;

            if (teamId == fixture.homeTeamId)
                return 1 - fixture.coeficient.awayTeam;
            else
                return 1 - fixture.coeficient.homeTeam;
        }
    }
}
