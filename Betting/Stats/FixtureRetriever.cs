using Betting.Config;
using Betting.DataModel;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Betting.Stats
{

    public class FixtureRetriever : FixtureRetrieverInterface
    {
        public FixtureRetriever(ConfigManagerInterface configManager)
        {
            configManager_ = configManager;
        }

        public override int GetNumberOfMatchDays(int year)
        {
            int teams = GetNumberOfTeams(year);
            return (teams - 1) * 2;
        }

        private int GetNumberOfTeams(int year)
        {
            lock (numberOfTeamsCache)
            {
                if (numberOfTeamsCache.ContainsKey(year))
                {
                    return numberOfTeamsCache[year];
                }
                string leagueName = configManager_.GetLeagueName();
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

        public override int GetGamesPerMatchDay(int year)
        {
            return GetNumberOfTeams(year) / 2;
        }

        public override List<Fixture> GetAllFixtures(int year, string team)
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

        public override List<Fixture> GetAllFixtures(int year)
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
                string leagueName = configManager_.GetLeagueName();
                int gamesPerMatchDay = GetGamesPerMatchDay(year);
                List<Fixture> result = new List<Fixture>();
                string fileName;
                if (configManager_.GetUseExpanded())
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

                    string[] oddProviders = { "B365", "BW", "PS", "VC", "GB" };
                    Dictionary<string, int> oddIdx = new Dictionary<string, int>();

                    foreach (string oddProvider in oddProviders)
                    {
                        int idxH = Array.FindIndex(fields, item => item == (oddProvider + "H"));
                        oddIdx.Add((oddProvider + "H"), idxH);
                        int idxD = Array.FindIndex(fields, item => item == (oddProvider + "D"));
                        oddIdx.Add((oddProvider + "D"), idxD);
                        int idxA = Array.FindIndex(fields, item => item == (oddProvider + "A"));
                        oddIdx.Add((oddProvider + "A"), idxA);
                    }


                    int idxHPTS = -1;
                    int idxAPTS = -1;
                    int idxHPL = -1;
                    int idxAPL = -1;
                    int idxFOH = -1;
                    int idxFOD = -1;
                    int idxFOA = -1;


                    if (configManager_.GetUseExpanded())
                    {
                        idxHPTS = Array.FindIndex(fields, item => item == "HPTS");
                        idxAPTS = Array.FindIndex(fields, item => item == "APTS");
                        idxHPL = Array.FindIndex(fields, item => item == "HPL");
                        idxAPL = Array.FindIndex(fields, item => item == "APL");
                        idxFOH = Array.FindIndex(fields, item => item == "FOH");
                        idxFOD = Array.FindIndex(fields, item => item == "FOD");
                        idxFOA = Array.FindIndex(fields, item => item == "FOA");
                    }


                    while (!parser.EndOfData)
                    {
                        fields = parser.ReadFields();
                        if (fields[2] == "")
                            break;
                        Fixture newFixture = new Fixture
                        {
                            homeTeamName = fields[idxHomeTeam],
                            awayTeamName = fields[idxAwayTeam]
                        };
                        int.TryParse(fields[idxFTHG], out newFixture.finalScore.homeTeamGoals);
                        int.TryParse(fields[idxFTAG], out newFixture.finalScore.awayTeamGoals);
                        int.TryParse(fields[idxHTHG], out newFixture.halfScore.homeTeamGoals);
                        int.TryParse(fields[idxHTAG], out newFixture.halfScore.awayTeamGoals);
                        newFixture.date = DateTime.Parse(fields[idxDate]);

                        foreach (string oddProvider in oddProviders)
                        {
                            if (TryGetOddData(oddProvider, fields, oddIdx, ref newFixture))
                                break;
                        }

                        if (newFixture.odds.Count != 3)
                        {
                            //newFixture.odds.Add("1", 1);
                            //newFixture.odds.Add("X", 1);
                            //newFixture.odds.Add("2", 1);
                            throw new Exception("could not get odds for fixture");
                        }

                        if (configManager_.GetUseExpanded())
                        {
                            newFixture.points.homeTeamPoints = int.Parse(fields[idxHPTS]);
                            newFixture.points.awayTeamPoints = int.Parse(fields[idxAPTS]);
                            newFixture.gamesPlayed.homeTeamGamesPlayed = int.Parse(fields[idxHPL]);
                            newFixture.gamesPlayed.awayTeamGamesPlayed = int.Parse(fields[idxAPL]);
                            newFixture.fairOdds.Add("1", float.Parse(fields[idxFOH]));
                            newFixture.fairOdds.Add("X", float.Parse(fields[idxFOD]));
                            newFixture.fairOdds.Add("2", float.Parse(fields[idxFOA]));
                        }
                        newFixture.Init(configManager_);

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

        private bool TryGetOddData(string oddProvider, string[] fields, Dictionary<string, int> oddIdx, ref Fixture fixture)
        {
            string idH = oddProvider + "H";
            string idD = oddProvider + "D";
            string idA = oddProvider + "A";

            try
            {
                double odd1 = double.Parse(fields[oddIdx[idH]]);
                if (odd1 < 1) return false;
                double oddD = double.Parse(fields[oddIdx[idD]]);
                if (oddD < 1) return false;
                double oddA = double.Parse(fields[oddIdx[idA]]);
                if (oddA < 1) return false;

                fixture.odds.Add("1", odd1);
                fixture.odds.Add("X", oddD);
                fixture.odds.Add("2", oddA);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public override List<Fixture> GetRound(int year, int matchDay)
        {
            Tuple<int, int> t = Tuple.Create(year, matchDay);

            matchdayFixtureCacheLock.EnterReadLock();
            try
            {
                if (matchdayFixtureCache.ContainsKey(t))
                {
                    return matchdayFixtureCache[t];
                }
            }
            finally
            {
                matchdayFixtureCacheLock.ExitReadLock();
            }

            matchdayFixtureCacheLock.EnterWriteLock();
            try
            {

                List<Fixture> all = GetAllFixtures(year);

                int gamesPerMatchDay = GetGamesPerMatchDay(year);
                int startRow = (matchDay - 1) * gamesPerMatchDay;

                List<Fixture> result = new List<Fixture>();
                for (int i = 0; i < gamesPerMatchDay; ++i)
                {
                    result.Add(all[startRow + i]);
                }
                matchdayFixtureCache[t] = result;
                return result;
            }
            finally
            {
                matchdayFixtureCacheLock.ExitWriteLock();
            }
        }

        private readonly Dictionary<int, List<Fixture>> fixturesCache = new Dictionary<int, List<Fixture>>();
        private readonly ReaderWriterLockSlim fixturesCacheLock = new ReaderWriterLockSlim();

        private readonly Dictionary<Tuple<int, int>, List<Fixture>> matchdayFixtureCache = new Dictionary<Tuple<int, int>, List<Fixture>>();
        private readonly ReaderWriterLockSlim matchdayFixtureCacheLock = new ReaderWriterLockSlim();

        private readonly Dictionary<Tuple<int, string>, List<Fixture>> fixturesTeamCache = new Dictionary<Tuple<int, string>, List<Fixture>>();
        private readonly ReaderWriterLockSlim fixturesTeamCacheLock = new ReaderWriterLockSlim();

        private readonly Dictionary<int, int> numberOfTeamsCache = new Dictionary<int, int>();

        private readonly ConfigManagerInterface configManager_;
    }
}
