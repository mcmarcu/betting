using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public class GoalsConcededMetric : MetricInterface
    {
        public GoalsConcededMetric(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever) : base(config, year, configManager, fixtureRetriever)
        {
        }

        private void GetTeamPoints(out double pTeam, int teamId, Fixture fixture)
        {
            pTeam = 0d;
            List<Fixture> allT = fixtureRetriever_.GetAllFixtures(year, teamId);
            int startIdx = FindFixtures(year, teamId, fixture.fixtureId, config.depth);
            int toProcess = config.depth;
            for (int i = startIdx; toProcess > 0; --i, --toProcess)
            {
                pTeam += GetGoals(allT[i], teamId) * GetCoeficient(allT[i], teamId);
            }
        }

        public override void GetPoints(out double pTeam1, out double pTeam2, int teamId1, int teamId2, Fixture fixture)
        {
            GetTeamPoints(out pTeam2, teamId1, fixture);//reverse
            GetTeamPoints(out pTeam1, teamId2, fixture);//reverse
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

        public double GetGoals(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.finalScore.awayTeamGoals;
            else
                return fixture.finalScore.homeTeamGoals;
        }

        public double GetCoeficient(Fixture fixture, int teamId)
        {
            if (!configManager_.GetUseExpanded())
                return 1d;

            if (teamId == fixture.homeTeamId)
                return 1d - fixture.coeficient.awayTeam;
            else
                return 1d - fixture.coeficient.homeTeam;
        }
    }
}
