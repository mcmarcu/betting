using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Metrics
{
    class LastGamesMetric : MetricInterface
    {

        public LastGamesMetric(MetricConfig config, int matchDay, int year) : base(config, matchDay, year)
        {
        }

        public override void GetPercentage(out int pTeam1, out int pTeam2, string teamName1, string teamName2)
        {
            int thisYear = year;
            int thisMatchDay = matchDay;
            int pctTeam1 = 0;
            int pctTeam2 = 0;
            for (int i = 0; i < config.depth; ++i)
            {
                FixtureRetriever.GetPrevRound(out thisYear, out thisMatchDay, thisYear, thisMatchDay);
                List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(thisYear, thisMatchDay);
                pctTeam1 += GetPoints(FindFixture(thisRoundFixtures, teamName1,FixtureMode.All), teamName1);
                pctTeam2 += GetPoints(FindFixture(thisRoundFixtures, teamName2,FixtureMode.All), teamName2);
            }

            pTeam1 = (int)((float)pctTeam1/((float)pctTeam1 + (float)pctTeam2)*100);
            pTeam2 = 100 - pTeam1;
        }

        public int GetPoints(Fixture fixture, string teamName)
        {
            if (fixture.finalScore.homeTeamGoals == fixture.finalScore.awayTeamGoals)
                return 1;
            else if (teamName == fixture.homeTeamName && fixture.finalScore.homeTeamGoals > fixture.finalScore.awayTeamGoals)
                return 3;
            else if (teamName == fixture.awayTeamName && fixture.finalScore.homeTeamGoals < fixture.finalScore.awayTeamGoals)
                return 3;
            else
                return 0;
        }
    }
}
