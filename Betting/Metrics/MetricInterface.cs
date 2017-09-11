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