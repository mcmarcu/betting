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
    public abstract class MetricInterface
    {
        abstract public void GetPoints(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture);
        abstract public void GetPercentage(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture);

        public MetricInterface(MetricConfig config, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever)
        {
            this.config = config;
            this.year = year;
            fixtureRetriever_ = fixtureRetriever;
            configManager_ = configManager;
        }

        public float GetTeamCoeficient(string teamName, Fixture fixture)
        {
            if (configManager_.GetUseExpanded())
            {
                float homeTeamCoeficient = fixture.gamesPlayed.homeTeamGamesPlayed > 0 ? (float)fixture.points.homeTeamPoints / (float)fixture.gamesPlayed.homeTeamGamesPlayed : 0;
                float awayTeamCoeficient = fixture.gamesPlayed.awayTeamGamesPlayed > 0 ? (float)fixture.points.awayTeamPoints / (float)fixture.gamesPlayed.awayTeamGamesPlayed : 0;

                if (teamName == fixture.homeTeamName)
                    return awayTeamCoeficient;
                else
                    return homeTeamCoeficient;
            }
            else
            {
                return 1;
            }
        }

        // Find <depth> fixtures with <thisTeam> starting from <fixture>
        public List<Fixture> FindFixtures(List<Fixture> allFixtures, Fixture fixture, int depth)
        {
            List<Fixture> result = new List<Fixture>();
            int startIdx = 0;
            for (int i=allFixtures.Count -1; i>=0; --i)
            {
                if( allFixtures[i].homeTeamName == fixture.homeTeamName &&
                    allFixtures[i].awayTeamName == fixture.awayTeamName)
                {
                    startIdx = i;
                    break;
                }
            }

            for (int i = startIdx - 1; i >= 0; --i)
            {
                result.Add(allFixtures[i]);
                if (result.Count == depth)
                {
                    break;
                }
            }

            return result;
        }

        public MetricConfig config;
        public int year;
        public ConfigManagerInterface configManager_;
        public FixtureRetrieverInterface fixtureRetriever_;
    }
}

// constructor with string teamName1 string teamName2
// a global data of results 

//DefenceMetric
//OffenceMetric
//DirectGamesMetric