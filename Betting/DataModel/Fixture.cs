using Betting.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Betting.DataModel
{
    public class Score
    {
        public int homeTeamGoals;
        public int awayTeamGoals;
    }

    public class Points
    {
        public int homeTeamPoints;
        public int awayTeamPoints;
    }

    public class GamesPlayed
    {
        public int homeTeamGamesPlayed;
        public int awayTeamGamesPlayed;
    }

    public class Coeficient
    {
        public double homeTeam;
        public double awayTeam;
    }

    public class Fixture
    {
        public int fixtureId;
        public string homeTeamName;
        public string awayTeamName;
        public int homeTeamId;
        public int awayTeamId;
        public DateTime date;
        public Score finalScore = new Score();
        public Score halfScore = new Score();
        public Dictionary<string, double> odds = new Dictionary<string, double>();
        public Dictionary<string, double> fairOdds = new Dictionary<string, double>();
        public Points points = new Points();
        public GamesPlayed gamesPlayed = new GamesPlayed();
        public Coeficient coeficient = new Coeficient();
        public string result;

        /// start static generation of team Ids
        private static readonly ConcurrentDictionary<string, int> teamIdMap = new ConcurrentDictionary<string, int>(10, 31);
        private static int lastTeamId = 0;
        private static int lastFixtureId = 0;

        private static int GetTeamId(string teamName)
        {
            if (teamIdMap.TryGetValue(teamName, out int value))
            {
                return value;
            }

            return teamIdMap.GetOrAdd(teamName, ++lastTeamId);
        }
        /// end static generation of team Ids

        public void Init(ConfigManagerInterface configManager)
        {
            SetResult();
            SetCoeficients(configManager);
            AddDoubleOdds();
            fixtureId = ++lastFixtureId;

            homeTeamId = GetTeamId(homeTeamName);
            awayTeamId = GetTeamId(awayTeamName);
        }

        private void SetResult()
        {
            if (finalScore.homeTeamGoals == finalScore.awayTeamGoals)
            {
                result = "X";
            }
            else if (finalScore.homeTeamGoals > finalScore.awayTeamGoals)
            {
                result = "1";
            }
            else
            {
                result = "2";
            }
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
                {
                    odds["1X"] = 1;
                }

                if (odds["X2"] < 1 && odds["X2"] > 0.90)
                {
                    odds["X2"] = 1;
                }

                if (odds["12"] < 1 && odds["12"] > 0.90)
                {
                    odds["12"] = 1;
                }

                // checking odds
                if (odds["1X"] < 1)
                {
                    throw new ArgumentOutOfRangeException("Failed to get odds 1X >= 1");
                }

                if (odds["X2"] < 1)
                {
                    throw new ArgumentOutOfRangeException("Failed to get odds X2 >= 1");
                }

                if (odds["12"] < 1)
                {
                    throw new ArgumentOutOfRangeException("Failed to get odds 12 >= 1");
                }
            }

            odds.Add("1X2", 0);
            odds.Add("", 0);
        }
    }
}
