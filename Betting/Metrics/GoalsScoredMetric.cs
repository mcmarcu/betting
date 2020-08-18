using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public class GoalsScoredMetric : MetricInterface
    {
        public GoalsScoredMetric(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever) : base(config, year, configManager, fixtureRetriever)
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
            List<Fixture> fixturesTeam1 = FindFixtures(allT1, fixture, config.depth);
            foreach (Fixture fix in fixturesTeam1)
            {
                pctTeam1 += GetGoals(fix, teamId1) * GetCoeficient(fix, teamId1);
            }
            List<Fixture> allT2 = fixtureRetriever_.GetAllFixtures(year, teamId2);
            List<Fixture> fixturesTeam2 = FindFixtures(allT2, fixture, config.depth);
            foreach (Fixture fix in fixturesTeam2)
            {
                pctTeam2 += GetGoals(fix, teamId2) * GetCoeficient(fix, teamId2);
            }

            pTeam1 = (int)pctTeam1;
            pTeam2 = (int)pctTeam2;
        }

        public double GetCoeficient(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.coeficient.awayTeam;
            else
                return fixture.coeficient.homeTeam;
        }

        public int GetGoals(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.finalScore.homeTeamGoals;
            else
                return fixture.finalScore.awayTeamGoals;
        }

    }
}
