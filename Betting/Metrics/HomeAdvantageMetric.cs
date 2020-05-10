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
    class HomeAdvantageMetric : MetricInterface
    {

        public HomeAdvantageMetric(MetricConfig config, int year) : base(config, year)
        {
        }

        public override void GetPercentage(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture)
        {
            int pctTeam1;
            int pctTeam2;

            GetPoints(out pctTeam1, out pctTeam2, teamName1, teamName2, fixture);

            pTeam1 = (int)((float)pctTeam1 / ((float)pctTeam1 + (float)pctTeam2) * 100);
            pTeam2 = 100 - pTeam1;
        }

        public override void GetPoints(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture)
        {
            float pctTeam1 = 0;
            float pctTeam2 = 0;

            List<Fixture> allT1 = FixtureRetriever.GetAllFixtures(year, teamName1);
            List<Fixture> fixturesTeam1 = FindFixtures(allT1, fixture, config.depth * 2);
            foreach (Fixture fix in fixturesTeam1)
            {
                if (fix.homeTeamName == teamName1)
                    pctTeam1 += GetPoints(fix, teamName1) * GetCoeficient(fix, teamName1);
            }

            List<Fixture> allT2 = FixtureRetriever.GetAllFixtures(year, teamName2);
            List<Fixture> fixturesTeam2 = FindFixtures(allT2, fixture, config.depth * 2);
            foreach (Fixture fix in fixturesTeam2)
            {
                if (fix.awayTeamName == teamName2)
                    pctTeam2 += GetPoints(fix, teamName2) * GetCoeficient(fix, teamName2);
            }

            pTeam1 = (int)pctTeam1;
            pTeam2 = (int)pctTeam2;
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

        public float GetCoeficient(Fixture fixture, string teamName)
        {
            return 1;

            if (teamName == fixture.homeTeamName)
                return fixture.coeficient.awayTeam;
            else
                return fixture.coeficient.homeTeam;
        }
    }
}
