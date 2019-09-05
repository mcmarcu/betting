using Betting.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Metrics
{
    

    abstract class MetricInterface
    {
        abstract public void GetPercentage(out int pTeam1, out int pTeam2, string teamName1, string teamName2, Fixture fixture);

        public MetricInterface(MetricConfig config, int matchDay, int year)
        {
            this.config = config;
            this.matchDay = matchDay;
            this.year = year;
        }

        // Find <depth> fixtures with <thisTeam> starting from <fixture>
        public List<Fixture> FindFixtures(List<Fixture> allFixtures, string thisTeam, Fixture fixture, int depth)
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
                if (allFixtures[i].homeTeamName == thisTeam || allFixtures[i].awayTeamName == thisTeam)
                {
                    result.Add(allFixtures[i]);
                }
                if (result.Count == depth)
                {
                    break;
                }
            }

            return result;
        }

        // Find all fixtures with these 2 teams
        public List<Fixture> FindFixtures(List<Fixture> allFixtures, Fixture fixture, bool getIncludingFixture)
        {
            List<Fixture> result = new List<Fixture>();

            int startIdx = allFixtures.Count - 1;
            if (!getIncludingFixture)
            {
                for (int i = allFixtures.Count - 1; i >= 0; --i)
                {
                    if (allFixtures[i].homeTeamName == fixture.homeTeamName &&
                        allFixtures[i].awayTeamName == fixture.awayTeamName)
                    {
                        startIdx = i - 1;
                        break;
                    }
                }
            }

            for (int i = startIdx; i >= 0; --i)
            {
                if (allFixtures[i].homeTeamName == fixture.homeTeamName &&
                    allFixtures[i].awayTeamName == fixture.awayTeamName)
                {
                    result.Add(allFixtures[i]);
                }
                if (allFixtures[i].awayTeamName == fixture.homeTeamName &&
                    allFixtures[i].homeTeamName == fixture.awayTeamName)
                {
                    result.Add(allFixtures[i]);
                }
            }

            return result;
        }

        // Find all fixtures with this team
        public List<Fixture> FindFixtures(List<Fixture> allFixtures, string thisTeam)
        {
            List<Fixture> result = new List<Fixture>();

            for (int i = allFixtures.Count - 1; i >= 0; --i)
            {
                if (allFixtures[i].homeTeamName == thisTeam &&
                    allFixtures[i].awayTeamName == thisTeam)
                {
                    result.Add(allFixtures[i]);
                }
            }

            return result;
        }

        public MetricConfig config;
        public int matchDay;
        public int year;
    }
}

// constructor with string teamName1 string teamName2
// a global data of results 

//DefenceMetric
//OffenceMetric
//DirectGamesMetric