﻿using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public class HomeAdvantageMetric : MetricInterface
    {
        public HomeAdvantageMetric(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever) : base(config, year, configManager, fixtureRetriever)
        {
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

        public override void GetPoints(out int pTeam1, out int pTeam2, int teamId1, int teamId2, Fixture fixture)
        {
            double pctTeam1 = 0;
            double pctTeam2 = 0;

            List<Fixture> allT1 = fixtureRetriever_.GetAllFixtures(year, teamId1);
            List<Fixture> fixturesTeam1 = FindFixtures(allT1, fixture, config.depth * 2);
            int homeFoundFixtures = 0;
            foreach (Fixture fix in fixturesTeam1)
            {
                if (fix.homeTeamId == teamId1)
                {
                    pctTeam1 += GetPoints(fix, teamId1);// no Coeficient
                    if (++homeFoundFixtures == config.depth)
                        break;
                }
            }

            List<Fixture> allT2 = fixtureRetriever_.GetAllFixtures(year, teamId2);
            List<Fixture> fixturesTeam2 = FindFixtures(allT2, fixture, config.depth * 2);
            int awayFoundFixtures = 0;
            foreach (Fixture fix in fixturesTeam2)
            {
                if (fix.awayTeamId == teamId2)
                {
                    pctTeam2 += GetPoints(fix, teamId2);// no Coeficient
                    if (++awayFoundFixtures == config.depth)
                        break;
                }
            }

            pTeam1 = (int)pctTeam1;
            pTeam2 = (int)pctTeam2;
        }

        public int GetPoints(Fixture fixture, int teamId)
        {
            if (fixture.finalScore.homeTeamGoals == fixture.finalScore.awayTeamGoals)
                return 1;
            else if (teamId == fixture.homeTeamId && fixture.finalScore.homeTeamGoals > fixture.finalScore.awayTeamGoals)
                return 3;
            else if (teamId == fixture.awayTeamId && fixture.finalScore.homeTeamGoals < fixture.finalScore.awayTeamGoals)
                return 3;
            else
                return 0;
        }
    }
}
