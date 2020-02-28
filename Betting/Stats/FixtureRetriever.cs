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
        public static int GetNumberOfMatchDays(int year)
        {
            int teams = GetNumberOfTeams(year);
            return (teams - 1) * 2;
        }

        private static int GetNumberOfTeams(int year)
        {
            lock (numberOfTeamsCache)
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

                    string[] fields = parser.ReadFields();
                    int idxHomeTeam = Array.FindIndex(fields, item => item == "HomeTeam");
                    while (!parser.EndOfData)
                    {
                        fields = parser.ReadFields();
                        if (fields[idxHomeTeam] != "" && fields[2] != "HomeTeam")
                            teams.Add(fields[idxHomeTeam]);
                    }
                }

                numberOfTeamsCache.Add(year, teams.Count);
                return teams.Count;
            }

        }
        private static int GetNumberOfMatchdays(int year)
        {
            return (GetNumberOfTeams(year) - 1) * 2;
        }

        public static int GetGamesPerMatchDay(int year)
        {
            return GetNumberOfTeams(year) / 2;
        }

        public static List<Fixture> GetAllFixtures(int year, string team)
        {
            Tuple<int, string> t = Tuple.Create(year, team);

            fixturesTeamCacheLock.EnterReadLock();
            try
            {
                if (fixturesTeamCache.ContainsKey(t))
                {
                    return fixturesTeamCache[t];
                }
            }
            finally
            {
                fixturesTeamCacheLock.ExitReadLock();
            }

            fixturesTeamCacheLock.EnterWriteLock();
            try
            {
                if (fixturesTeamCache.ContainsKey(t))
                {
                    return fixturesTeamCache[t];
                }

                List<Fixture> allFixtures = GetAllFixtures(year);

                List<Fixture> result = new List<Fixture>();
                for (int i = 0; i < allFixtures.Count; ++i)
                {
                    if (allFixtures[i].homeTeamName == team ||
                        allFixtures[i].awayTeamName == team)
                    {
                        result.Add(allFixtures[i]);
                    }
                }

                fixturesTeamCache.Add(t, result);
                return result;
            }
            finally
            {
                fixturesTeamCacheLock.ExitWriteLock();
            }
        }

        public static List<Fixture> GetAllFixtures(int year)
        {
            fixturesCacheLock.EnterReadLock();
            try
            {
                if (fixturesCache.ContainsKey(year))
                {
                    return fixturesCache[year];
                }
            }
            finally
            {
                fixturesCacheLock.ExitReadLock();
            }

            fixturesCacheLock.EnterWriteLock();
            try
            {
                if (fixturesCache.ContainsKey(year))
                {
                    return fixturesCache[year];
                }
                string leagueName = ConfigManager.Instance.GetLeagueName();
                int gamesPerMatchDay = GetGamesPerMatchDay(year);
                List<Fixture> result = new List<Fixture>();
                string fileName;
                if (ConfigManager.Instance.GetUseExpanded())
                {
                    fileName = "..\\..\\DBEX\\" + leagueName + year + ".csv";
                }
                else
                {
                    fileName = "..\\..\\DB\\" + leagueName + year + ".csv";
                }

                using (TextFieldParser parser = new TextFieldParser(fileName))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    string[] fields = parser.ReadFields();
                    int idxDate = Array.FindIndex(fields, item => item == "Date");
                    int idxHomeTeam = Array.FindIndex(fields, item => item == "HomeTeam");
                    int idxAwayTeam = Array.FindIndex(fields, item => item == "AwayTeam");
                    int idxFTHG = Array.FindIndex(fields, item => item == "FTHG");
                    int idxFTAG = Array.FindIndex(fields, item => item == "FTAG");
                    int idxHTHG = Array.FindIndex(fields, item => item == "HTHG");
                    int idxHTAG = Array.FindIndex(fields, item => item == "HTAG");
                    int idxB365H = Array.FindIndex(fields, item => item == "B365H");
                    int idxB365D = Array.FindIndex(fields, item => item == "B365D");
                    int idxB365A = Array.FindIndex(fields, item => item == "B365A");
                    int idxBWH = Array.FindIndex(fields, item => item == "BWH");
                    int idxBWD = Array.FindIndex(fields, item => item == "BWD");
                    int idxBWA = Array.FindIndex(fields, item => item == "BWA");

                    int idxHPTS = -1;
                    int idxAPTS = -1;
                    int idxHPL = -1;
                    int idxAPL = -1;


                    if (ConfigManager.Instance.GetUseExpanded())
                    {
                        idxHPTS = Array.FindIndex(fields, item => item == "HPTS");
                        idxAPTS = Array.FindIndex(fields, item => item == "APTS");
                        idxHPL = Array.FindIndex(fields, item => item == "HPL");
                        idxAPL = Array.FindIndex(fields, item => item == "APL");
                    }

                    

                    while (!parser.EndOfData)
                    {
                        fields = parser.ReadFields();
                        if (fields[2] == "")
                            break;
                        Fixture newFixture = new Fixture();
                        newFixture.homeTeamName = fields[idxHomeTeam];
                        newFixture.awayTeamName = fields[idxAwayTeam];
                        Int32.TryParse(fields[idxFTHG], out newFixture.finalScore.homeTeamGoals);
                        Int32.TryParse(fields[idxFTAG], out newFixture.finalScore.awayTeamGoals);
                        Int32.TryParse(fields[idxHTHG], out newFixture.halfScore.homeTeamGoals);
                        Int32.TryParse(fields[idxHTAG], out newFixture.halfScore.awayTeamGoals);
                        newFixture.date = DateTime.Parse(fields[idxDate]);

                        newFixture.odds = new Dictionary<string, float>();
                        try
                        { newFixture.odds.Add("1", float.Parse(fields[idxB365H])); }
                        catch(Exception)
                        { newFixture.odds.Add("1", float.Parse(fields[idxBWH])); }
                        try
                        { newFixture.odds.Add("X", float.Parse(fields[idxB365D])); }
                        catch (Exception)
                        { newFixture.odds.Add("X", float.Parse(fields[idxBWD])); }
                        try
                        { newFixture.odds.Add("2", float.Parse(fields[idxB365A])); }
                        catch (Exception)
                        { newFixture.odds.Add("2", float.Parse(fields[idxBWA])); }

                        newFixture.odds.Add("1X", (newFixture.odds["1"] * newFixture.odds["X"]) / (newFixture.odds["1"] + newFixture.odds["X"]));
                        newFixture.odds.Add("X2", (newFixture.odds["X"] * newFixture.odds["2"]) / (newFixture.odds["X"] + newFixture.odds["2"]));
                        newFixture.odds.Add("12", (newFixture.odds["1"] * newFixture.odds["2"]) / (newFixture.odds["1"] + newFixture.odds["2"]));
                        newFixture.odds.Add("1X2", 0);


                        if (ConfigManager.Instance.GetUseExpanded())
                        {
                            newFixture.points.homeTeamPoints = Int32.Parse(fields[idxHPTS]);
                            newFixture.points.awayTeamPoints = Int32.Parse(fields[idxAPTS]);
                            newFixture.gamesPlayed.homeTeamGamesPlayed = Int32.Parse(fields[idxHPL]);
                            newFixture.gamesPlayed.awayTeamGamesPlayed = Int32.Parse(fields[idxAPL]);

                            newFixture.SetCoeficients();
                        }

                        result.Add(newFixture);
                    }
                }

                fixturesCache.Add(year, result);
                return result;
            }
            finally
            {
                fixturesCacheLock.ExitWriteLock();
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
        private static ReaderWriterLockSlim fixturesCacheLock = new ReaderWriterLockSlim();
        public static Dictionary<Tuple<int,string>, List<Fixture>> fixturesTeamCache = new Dictionary<Tuple<int, string>, List<Fixture>>();
        private static ReaderWriterLockSlim fixturesTeamCacheLock = new ReaderWriterLockSlim();
        public static Dictionary<int, int> numberOfTeamsCache = new Dictionary<int, int>();

    }
}
