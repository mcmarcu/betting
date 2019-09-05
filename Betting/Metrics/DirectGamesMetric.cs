﻿using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Metrics
{
    class DirectGamesMetric : MetricInterface
    {

        public DirectGamesMetric(MetricConfig config, int matchDay, int year) : base(config, matchDay, year)
        {
        }

        public override void GetPercentage(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture)
        {
            int thisYear = year;
            int thisMatchDay = matchDay;
            int pctTeam1 = 0;
            int pctTeam2 = 0;
            int matchesProcessed = 0;
            while (matchesProcessed < config.depth)
            {
                List<Fixture> all = FixtureRetriever.GetAllFixtures(thisYear);

                List<Fixture> fixtures = FindFixtures(all, fixture, thisYear != year);

                //some team not in league
                if (fixtures.Count == 0)
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        matchesProcessed++;
                        if (FindFixtures(all, teamName1).Count > 0)
                            pctTeam1 += 3;//teamName1 was in league
                        else if (FindFixtures(all, teamName2).Count > 0)
                            pctTeam2 += 3;//teamName2 was in league
                        if (matchesProcessed == config.depth)
                            break;
                    }
                }

                //both teams in league
                foreach (Fixture fix in fixtures)
                {
                    matchesProcessed++;
                    pctTeam1 += GetGoals(fix, teamName1);
                    pctTeam2 += GetGoals(fix, teamName2);
                    if (matchesProcessed == config.depth)
                        break;
                }

                thisYear--;
            }

            pTeam1 = (int)((float)pctTeam1/((float)pctTeam1 + (float)pctTeam2)*100);
            pTeam2 = 100 - pTeam1;
        }

        public int GetGoals(Fixture fixture, string teamName)
        {
            if (teamName == fixture.homeTeamName)
                return fixture.finalScore.homeTeamGoals;
            else 
                return fixture.finalScore.awayTeamGoals;
        }
    }
}
