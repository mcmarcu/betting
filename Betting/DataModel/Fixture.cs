using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.DataModel
{
    struct Score
    {
        public int homeTeamGoals;
        public int awayTeamGoals;
    }

    struct Points
    {
        public int homeTeamPoints;
        public int awayTeamPoints;
    }

    struct GamesPlayed
    {
        public int homeTeamGamesPlayed;
        public int awayTeamGamesPlayed;
    }

    struct Coeficient
    {
        public float homeTeam;
        public float awayTeam;
    }

    struct Fixture
    {
        public string homeTeamName;
        public string awayTeamName;
        public DateTime date;
        public Score finalScore;
        public Score halfScore;
        public Dictionary<string, float> odds;
        public Points points;
        public GamesPlayed gamesPlayed;
        public Coeficient coeficient;

        public string GetResult()
        {
            if (finalScore.homeTeamGoals == finalScore.awayTeamGoals)
                return "X";
            else if (finalScore.homeTeamGoals > finalScore.awayTeamGoals)
                return "1";
            else
                return "2";
        }

        public void SetCoeficients()
        {
            coeficient.homeTeam = (float)points.homeTeamPoints / (3 * gamesPlayed.homeTeamGamesPlayed);
            coeficient.awayTeam = (float)points.awayTeamPoints / (3 * gamesPlayed.awayTeamGamesPlayed);
        }
    }

    
}
