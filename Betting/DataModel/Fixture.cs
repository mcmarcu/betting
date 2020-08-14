using Betting.Config;
using System;
using System.Collections.Generic;

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
        public double homeTeam;
        public double awayTeam;
    }

    public class Fixture
    {
        public string homeTeamName;
        public string awayTeamName;
        public DateTime date;
        public Score finalScore;
        public Score halfScore;
        public Dictionary<string, double> odds = new Dictionary<string, double>();
        public Dictionary<string, double> fairOdds = new Dictionary<string, double>();
        public Points points;
        public GamesPlayed gamesPlayed;
        public Coeficient coeficient;
        public string result;

        public void Init(ConfigManagerInterface configManager)
        {
            SetResult();
            SetCoeficients(configManager);
            AddDoubleOdds();
        }

        private void SetResult()
        {
            if (finalScore.homeTeamGoals == finalScore.awayTeamGoals)
                result = "X";
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
                coeficient.homeTeam = (double)points.homeTeamPoints / (weight * gamesPlayed.homeTeamGamesPlayed);
                coeficient.awayTeam = (double)points.awayTeamPoints / (weight * gamesPlayed.awayTeamGamesPlayed);
            }
            else
            {
                coeficient.homeTeam = 1;
                coeficient.awayTeam = 1;
            }
        }

        private void AddDoubleOdds()
        {
            // https://www.reddit.com/r/SoccerBetting/comments/90fd4d/how_to_calculate_double_chance/ 

            if (odds["1"] == 1 && odds["X"] == 1 && odds["2"] == 1)
            {
                odds["1X"] = 1;
                odds["X2"] = 1;
                odds["12"] = 1;
            }
            else
            {
                odds.Add("1X", (odds["1"] * odds["X"]) / (odds["1"] + odds["X"]));
                odds.Add("X2", (odds["X"] * odds["2"]) / (odds["X"] + odds["2"]));
                odds.Add("12", (odds["1"] * odds["2"]) / (odds["1"] + odds["2"]));

                // normalization of commision
                if (odds["1X"] < 1 && odds["1X"] > 0.90)
                    odds["1X"] = 1;
                if (odds["X2"] < 1 && odds["X2"] > 0.90)
                    odds["X2"] = 1;
                if (odds["12"] < 1 && odds["12"] > 0.90)
                    odds["12"] = 1;

                // checking odds
                if (odds["1X"] < 1)
                    throw new ArgumentOutOfRangeException("Failed to get odds 1X >= 1");
                if (odds["X2"] < 1)
                    throw new ArgumentOutOfRangeException("Failed to get odds X2 >= 1");
                if (odds["12"] < 1)
                    throw new ArgumentOutOfRangeException("Failed to get odds 12 >= 1");
            }

            odds.Add("1X2", 0);
            odds.Add("", 0);
        }
    }
}
