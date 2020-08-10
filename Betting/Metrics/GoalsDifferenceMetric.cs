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
    public class GoalsDifferenceMetric : MetricInterface
    {
        public GoalsDifferenceMetric(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever) : base(config, year, configManager, fixtureRetriever)
        {
        }

        public override void GetPercentage(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture)
        {
            GetPoints(out int pctTeam1, out int pctTeam2, teamName1, teamName2, fixture);

            if (pctTeam1 == 0 && pctTeam2 == 0)
                pTeam1 = 50;
            else
                pTeam1 = (int)((double)pctTeam1/((double)pctTeam1 + (double)pctTeam2)*100);
            pTeam2 = 100 - pTeam1;
        }

        public override void GetPoints(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture)
        {
            double pctTeam1 = 0;
            double pctTeam2 = 0;

            List<Fixture> allT1 = fixtureRetriever_.GetAllFixtures(year, teamName1);
            List<Fixture> fixturesTeam1 = FindFixtures(allT1, fixture, config.depth);
            foreach (Fixture fix in fixturesTeam1)
            {
                pctTeam1 += GetScoredGoals(fix, teamName1) * GetCoeficient(fix, teamName2);
                pctTeam1 -= GetConcededGoals(fix, teamName1) * GetCoeficient(fix, teamName2);
            }
            List<Fixture> allT2 = fixtureRetriever_.GetAllFixtures(year, teamName2);
            List<Fixture> fixturesTeam2 = FindFixtures(allT2, fixture, config.depth);
            foreach (Fixture fix in fixturesTeam2)
            {
                pctTeam2 += GetScoredGoals(fix, teamName2) * GetCoeficient(fix, teamName1);
                pctTeam2 -= GetConcededGoals(fix, teamName2) * GetCoeficient(fix, teamName1);
            }

            pTeam1 = (int)pctTeam1;
            pTeam2 = (int)pctTeam2;
        }

        public double GetCoeficient(Fixture fixture, string teamName)
        {
            if (teamName == fixture.homeTeamName)
                return fixture.coeficient.homeTeam;
            else
                return fixture.coeficient.awayTeam;
        }

        public int GetScoredGoals(Fixture fixture, string teamName)
        {
            if (teamName == fixture.homeTeamName)
                return fixture.finalScore.homeTeamGoals;
            else 
                return fixture.finalScore.awayTeamGoals;
        }

        public int GetConcededGoals(Fixture fixture, string teamName)
        {
            if (teamName == fixture.homeTeamName)
                return fixture.finalScore.awayTeamGoals;
            else
                return fixture.finalScore.homeTeamGoals;
        }

    }
}
