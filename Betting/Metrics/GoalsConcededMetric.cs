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
    public class GoalsConcededMetric : MetricInterface
    {
        public GoalsConcededMetric(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever) : base(config, year, configManager, fixtureRetriever)
        {
        }

        public override void GetPoints(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture)
        {
            float pctTeam1 = 0;
            float pctTeam2 = 0;

            List<Fixture> allT1 = fixtureRetriever_.GetAllFixtures(year, teamName1);
            List<Fixture> fixturesTeam1 = FindFixtures(allT1, fixture, config.depth);
            foreach (Fixture fix in fixturesTeam1)
            {
                pctTeam1 += GetGoals(fix, teamName1) * GetCoeficient(fix, teamName1);
            }
            List<Fixture> allT2 = fixtureRetriever_.GetAllFixtures(year, teamName2);
            List<Fixture> fixturesTeam2 = FindFixtures(allT2, fixture, config.depth);
            foreach (Fixture fix in fixturesTeam2)
            {
                pctTeam2 += GetGoals(fix, teamName2) * GetCoeficient(fix, teamName2);
            }

            //reverse
            pTeam1 = (int)pctTeam2;
            pTeam2 = (int)pctTeam1;
        }


        public override void GetPercentage(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture)
        {
            int pctTeam1;
            int pctTeam2;

            GetPoints(out pctTeam1, out pctTeam2, teamName1, teamName2, fixture);

            pTeam1 = (int)((float)pctTeam1 / ((float)pctTeam1 + (float)pctTeam2) * 100);
            pTeam2 = 100 - pTeam1;
        }

        

        public int GetGoals(Fixture fixture, string teamName)
        {
            if (teamName == fixture.homeTeamName)
                return fixture.finalScore.awayTeamGoals;
            else 
                return fixture.finalScore.homeTeamGoals;
        }

        public float GetCoeficient(Fixture fixture, string teamName)
        {
            if (!configManager_.GetUseExpanded())
                return 1;

            if (teamName == fixture.homeTeamName)
                return 1-fixture.coeficient.awayTeam;
            else
                return 1-fixture.coeficient.homeTeam;
        }
    }
}
