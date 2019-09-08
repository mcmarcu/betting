using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betting.DataModel;
using System.Net;
using System.IO;
using Betting.Config;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using System.Threading;

namespace Betting.Stats
{
    class FixtureRetriever
    {
        private static int GetNumberOfTeams(int year)
        {
            if (numberOfTeamsCache.ContainsKey(year))
            {
                return numberOfTeamsCache[year];
            }
            string leagueName = ConfigManager.Instance.GetLeagueName();
            HashSet<string> teams = new HashSet<string>();
            using (TextFieldParser parser = new TextFieldParser("..\\..\\DB\\" + leagueName + year + ".csv"))
            {

                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if(fields[2]!="" && fields[2]!="HomeTeam")
                        teams.Add(fields[2]);
                }
            }

            numberOfTeamsCache.Add(year, teams.Count);
            return teams.Count;

        }
        private static int GetNumberOfMatchdays(int year)
        {
            return (GetNumberOfTeams(year) - 1) * 2;
        }

        private static int GetGamesPerMatchDay(int year)
        {
            return GetNumberOfTeams(year) / 2;
        }

        public static List<Fixture> GetAllFixtures(int year)
        {
            lock (fixturesCache)
            {
                if (fixturesCache.ContainsKey(year))
                {
                    return fixturesCache[year];
                }
                string leagueName = ConfigManager.Instance.GetLeagueName();
                int gamesPerMatchDay = GetGamesPerMatchDay(year);
                List<Fixture> result = new List<Fixture>();
                using (TextFieldParser parser = new TextFieldParser("..\\..\\DB\\" + leagueName + year + ".csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.ReadFields();
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (fields[2] == "")
                            break;
                        Fixture newFixture = new Fixture();
                        newFixture.homeTeamName = fields[2];
                        newFixture.awayTeamName = fields[3];
                        newFixture.finalScore.homeTeamGoals = Int32.Parse(fields[4]);
                        newFixture.finalScore.awayTeamGoals = Int32.Parse(fields[5]);
                        newFixture.halfScore.homeTeamGoals = Int32.Parse(fields[7]);
                        newFixture.halfScore.awayTeamGoals = Int32.Parse(fields[8]);
                        newFixture.date = DateTime.Parse(fields[1]);

                        newFixture.odds = new Dictionary<string, float>();
                        newFixture.odds.Add("1", float.Parse(fields[23]));
                        newFixture.odds.Add("X", float.Parse(fields[24]));
                        newFixture.odds.Add("2", float.Parse(fields[25]));
                        newFixture.odds.Add("1X", (newFixture.odds["1"] * newFixture.odds["X"]) / (newFixture.odds["1"] + newFixture.odds["X"]));
                        newFixture.odds.Add("X2", (newFixture.odds["X"] * newFixture.odds["2"]) / (newFixture.odds["X"] + newFixture.odds["2"]));
                        newFixture.odds.Add("12", (newFixture.odds["1"] * newFixture.odds["2"]) / (newFixture.odds["1"] + newFixture.odds["2"]));
                        newFixture.odds.Add("1X2", 0);

                        result.Add(newFixture);
                    }
                }

                fixturesCache.Add(year, result);
                return result;
            }
        }


        public static List<Fixture> GetRound(int year, int matchDay)
        {
            List<Fixture> all = GetAllFixtures(year);

            int gamesPerMatchDay = GetGamesPerMatchDay(year);
            int startRow = (matchDay - 1) * gamesPerMatchDay;
            
            List<Fixture> result = new List<Fixture>();
            for(int i = 0;i<gamesPerMatchDay;++i)
            {
                result.Add(all[startRow + i]);
            }
            return result;
        }

        public static void GetPrevRound(out int outYear, out int outDay, int currentYear, int currentDay)
        {
            if (currentDay == 1)
            {
                outDay = GetNumberOfMatchdays(currentYear - 1);
                outYear = currentYear - 1;
            }
            else
            {
                outDay = currentDay - 1;
                outYear = currentYear;
            }  
        }

        public static Dictionary<int, List<Fixture>> fixturesCache = new Dictionary<int, List<Fixture>>();
        public static Dictionary<int, int> numberOfTeamsCache = new Dictionary<int, int>();
    }
}
