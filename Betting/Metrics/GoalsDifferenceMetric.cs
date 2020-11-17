using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public class GoalsDifferenceMetric : MetricInterface
    {
        public GoalsDifferenceMetric(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever) : base(config, year, configManager, fixtureRetriever)
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

        private void GetTeamPoints(out int pTeam, int teamId, int advTeamId, Fixture fixture)
        {
            double pctTeam = 0;

            List<Fixture> allT = fixtureRetriever_.GetAllFixtures(year, teamId);
            List<Fixture> fixturesTeam1 = FindFixtures(allT, fixture, config.depth);
            foreach (Fixture fix in fixturesTeam1)
            {
                pctTeam += GetScoredGoals(fix, teamId) * GetCoeficient(fix, advTeamId);
                pctTeam -= GetConcededGoals(fix, teamId) * GetCoeficient(fix, advTeamId);
            }
            pTeam = (int)pctTeam;
        }

        public override void GetPoints(out int pTeam1, out int pTeam2, int teamId1, int teamId2, Fixture fixture)
        {
            GetTeamPoints(out pTeam1, teamId1, teamId2, fixture);
            GetTeamPoints(out pTeam2, teamId2, teamId1, fixture);
        }

        public int GetScoredGoals(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.finalScore.homeTeamGoals;
            else
                return fixture.finalScore.awayTeamGoals;
        }

        public int GetConcededGoals(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.finalScore.awayTeamGoals;
            else
                return fixture.finalScore.homeTeamGoals;
        }
        public double GetCoeficient(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.coeficient.homeTeam;
            else
                return fixture.coeficient.awayTeam;
        }

    }
}
