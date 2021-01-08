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
            GetPoints(out double pctTeam1, out double pctTeam2, teamId1, teamId2, fixture);

            if (pctTeam1 == 0 && pctTeam2 == 0)
                pTeam1 = 50;
            else
                pTeam1 = (int)(pctTeam1 / (pctTeam1 + pctTeam2) * 100d);
            pTeam2 = 100 - pTeam1;
        }

        private void GetTeamPoints(out double pTeam, int teamId, int advTeamId, Fixture fixture)
        {
            pTeam = 0d;
            List<Fixture> allT = fixtureRetriever_.GetAllFixtures(year, teamId);
            int startIdx = FindFixtures(year, teamId, fixture.fixtureId, config.depth);
            int toProcess = config.depth;
            for (int i = startIdx; toProcess > 0; --i, --toProcess) {
                pTeam += GetScoredGoals(allT[i], teamId) * GetCoeficient(allT[i], advTeamId);
                pTeam -= GetConcededGoals(allT[i], teamId) * GetCoeficient(allT[i], advTeamId);
            }
        }

        public override void GetPoints(out double pTeam1, out double pTeam2, int teamId1, int teamId2, Fixture fixture)
        {
            GetTeamPoints(out pTeam1, teamId1, teamId2, fixture);
            GetTeamPoints(out pTeam2, teamId2, teamId1, fixture);
        }

        public double GetScoredGoals(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.finalScore.homeTeamGoals;
            else
                return fixture.finalScore.awayTeamGoals;
        }

        public double GetConcededGoals(Fixture fixture, int teamId)
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
