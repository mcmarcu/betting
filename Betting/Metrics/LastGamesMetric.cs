using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public class LastGamesMetric : MetricInterface
    {
        public LastGamesMetric(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever) : base(config, year, configManager, fixtureRetriever)
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

        private void GetTeamPoints(out int pTeam, int teamId, Fixture fixture)
        {
            double pctTeam = 0;

            List<Fixture> allT = fixtureRetriever_.GetAllFixtures(year, teamId);
            int startIdx = FindFixtures(year, teamId, fixture.fixtureId, config.depth);
            int toProcess = config.depth;
            for (int i = startIdx; toProcess > 0; --i, --toProcess)
            {
                pctTeam += GetPoints(allT[i], teamId) * GetCoeficient(allT[i], teamId);
            }

            pTeam = (int)pctTeam;
        }

        public override void GetPoints(out int pTeam1, out int pTeam2, int teamId1, int teamId2, Fixture fixture)
        {
            GetTeamPoints(out pTeam1, teamId1, fixture);
            GetTeamPoints(out pTeam2, teamId2, fixture);
        }

        public double GetPoints(Fixture fixture, int teamId)
        {
            if (fixture.finalScore.homeTeamGoals == fixture.finalScore.awayTeamGoals)
                return 1d;
            else if (teamId == fixture.homeTeamId && fixture.finalScore.homeTeamGoals > fixture.finalScore.awayTeamGoals)
                return 3d;
            else if (teamId == fixture.awayTeamId && fixture.finalScore.homeTeamGoals < fixture.finalScore.awayTeamGoals)
                return 3d;
            else
                return 0d;
        }

        public double GetCoeficient(Fixture fixture, int teamId)
        {
            if (teamId == fixture.homeTeamId)
                return fixture.coeficient.awayTeam;
            else
                return fixture.coeficient.homeTeam;
        }
    }
}
