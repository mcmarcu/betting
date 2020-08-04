using Betting.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.DataModel
{
    public struct Score
    {
        public int homeTeamGoals;
        public int awayTeamGoals;
    }

    public struct Points
    {
        public int homeTeamPoints;
        public int awayTeamPoints;
    }

    public struct GamesPlayed
    {
        public int homeTeamGamesPlayed;
        public int awayTeamGamesPlayed;
    }

    public struct Coeficient
    {
        public float homeTeam;
        public float awayTeam;
    }

    public class Fixture
    {
        public string homeTeamName;
        public string awayTeamName;
        public DateTime date;
        public Score finalScore;
        public Score halfScore;
        public Dictionary<string, float> odds;
        public Dictionary<string, float> fairOdds;
        public Points points;
        public GamesPlayed gamesPlayed;
        public Coeficient coeficient;
        public string result;

        public void init(ConfigManagerInterface configManager)
        {
            SetResult();
            SetCoeficients(configManager);
        }

        private void SetResult()
        {
            if (finalScore.homeTeamGoals == finalScore.awayTeamGoals)
                result =  "X";
            else if (finalScore.homeTeamGoals > finalScore.awayTeamGoals)
                result = "1";
            else
                result = "2";
        }

        private void SetCoeficients(ConfigManagerInterface configManager)
        {
            int weight = configManager.GetCoeficientWeight();
            if (configManager.GetUseExpanded() && weight != 0)
            {
                coeficient.homeTeam = (float)points.homeTeamPoints / (weight * gamesPlayed.homeTeamGamesPlayed);
                coeficient.awayTeam = (float)points.awayTeamPoints / (weight * gamesPlayed.awayTeamGamesPlayed);
            }
            else
            {
                coeficient.homeTeam = 1;
                coeficient.awayTeam = 1;
            }
        }
    }

    
}
