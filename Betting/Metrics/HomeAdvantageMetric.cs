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
            GetPoints(out double pctTeam1, out double pctTeam2, teamId1, teamId2, fixture);

            if (pctTeam1 == 0 && pctTeam2 == 0)
            {
                pTeam1 = 50;
            }
            else
            {
                pTeam1 = (int)(pctTeam1 / (pctTeam1 + pctTeam2) * 100d);
            }

            pTeam2 = 100 - pTeam1;
        }

        private void GetTeamPoints(out double pTeam, int teamId, Fixture fixture, bool checkHomeTeam)
        {
            pTeam = 0d;
            List<Fixture> allT = fixtureRetriever_.GetAllFixtures(year, teamId);
            int startIdx = FindFixtures(year, teamId, fixture.fixtureId, config.depth * 2);
            int toProcess = config.depth * 2;
            int foundFixtures = 0;
            for (int i = startIdx; toProcess > 0 && i >= 0; --i, --toProcess)
            {
                int idToCheck = checkHomeTeam ? allT[i].homeTeamId : allT[i].awayTeamId;
                if (idToCheck == teamId)
                {
                    pTeam += GetPoints(allT[i], teamId) * GetCoeficient(allT[i], teamId);
                    if (++foundFixtures == config.depth)
                    {
                        break;
                    }
                }
            }
        }

        public override void GetPoints(out double pTeam1, out double pTeam2, int teamId1, int teamId2, Fixture fixture)
        {
            GetTeamPoints(out pTeam1, teamId1, fixture, true);
            GetTeamPoints(out pTeam2, teamId2, fixture, false);
        }

        public double GetPoints(Fixture fixture, int teamId)
        {
            if (fixture.finalScore.homeTeamGoals == fixture.finalScore.awayTeamGoals)
            {
                return 1d;
            }
            else if (teamId == fixture.homeTeamId && fixture.finalScore.homeTeamGoals > fixture.finalScore.awayTeamGoals)
            {
                return 3d;
            }
            else if (teamId == fixture.awayTeamId && fixture.finalScore.homeTeamGoals < fixture.finalScore.awayTeamGoals)
            {
                return 3d;
            }
            else
            {
                return 0d;
            }
        }
    }
}
