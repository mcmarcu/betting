using Betting.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Metrics
{

    enum FixtureMode
    {
        All,
        Home,
        Away
    }

    abstract class MetricInterface
    {
        abstract public void GetPercentage(out int pTeam1, out int pTeam2, string teamName1, string teamName2);

        public MetricInterface(MetricConfig config, int matchDay, int year)
        {
            this.config = config;
            this.matchDay = matchDay;
            this.year = year;
        }
        
        public Fixture FindFixture(List<Fixture> fixtures, string teamName, FixtureMode fixtureMode)
        {
            foreach(Fixture fixture in fixtures)
            {
                if(fixture.homeTeamName == teamName && (fixtureMode==FixtureMode.Home || fixtureMode == FixtureMode.All))
                {
                    return fixture;
                }
                if (fixture.awayTeamName == teamName && (fixtureMode == FixtureMode.Away || fixtureMode == FixtureMode.All))
                {
                    return fixture;
                }
            }
            throw new KeyNotFoundException();
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