using Betting.Config;
using Betting.DataModel;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
            if (numberOfTeamsCache.TryGetValue(year, out int value))
            {
                return value;
            }

            HashSet<string> teams = new HashSet<string>();

            List<Fixture> allFixtures = GetAllFixtures(year);
            foreach (Fixture fixture in allFixtures)
            {
                teams.Add(fixture.homeTeamName);
                teams.Add(fixture.awayTeamName);
            }
                
            return numberOfTeamsCache.GetOrAdd(year, teams.Count);
        }

        public override int GetGamesPerMatchDay(int year)
        {
            return GetNumberOfTeams(year) / 2;
        }

        public override int FindFixtureIndex(int year, int teamId, int fixtureId)
        {
            int key = (year * 1000000000) + (teamId * 1000000) + fixtureId;

            if (fixturesIndexCache.TryGetValue(key, out int value))
            {
                return value;
            }

            List<Fixture> teamFixtures = GetAllFixtures(year, teamId);
            int result = teamFixtures.FindIndex(thisFix => thisFix.fixtureId == fixtureId);


            return fixturesIndexCache.GetOrAdd(key, result);
        }

        public override List<Fixture> GetAllFixtures(int year, int teamId)
        {
            int key = year * 1000 + teamId;
            
            if (fixturesTeamCache.TryGetValue(key, out List<Fixture> value))
            {
                return value;
            }

            List<Fixture> allFixtures = GetAllFixtures(year);

            List<Fixture> result = new List<Fixture>(allFixtures.Count);
            for (int i = 0; i < allFixtures.Count; ++i)
            {
                if (allFixtures[i].homeTeamId == teamId ||
                    allFixtures[i].awayTeamId == teamId)
                {
                    result.Add(allFixtures[i]);
                }
            }

            return fixturesTeamCache.GetOrAdd(key, result);
        }

        public override List<Fixture> GetAllFixtures(int year)
        {
            
            if (fixturesCache.TryGetValue(year, out List<Fixture> value))
            {
                return value;
            }

            string leagueName = configManager_.GetLeagueName();
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

                string[] oddProviders = { "BbAv", "Avg", "B365", "GB", "P", "BW", "PS", "VC" };
                Dictionary<string, int> oddIdx = new Dictionary<string, int>();

                foreach (string oddProvider in oddProviders)
                {
                    int idxH = Array.FindIndex(fields, item => item == (oddProvider + "H"));
                    oddIdx.Add((oddProvider + "H"), idxH);
                    int idxD = Array.FindIndex(fields, item => item == (oddProvider + "D"));
                    oddIdx.Add((oddProvider + "D"), idxD);
                    int idxA = Array.FindIndex(fields, item => item == (oddProvider + "A"));
                    oddIdx.Add((oddProvider + "A"), idxA);
                    int idxM25 = Array.FindIndex(fields, item => item == (oddProvider + ">2.5"));
                    oddIdx.Add((oddProvider + ">2.5"), idxM25);
                    int idxL25 = Array.FindIndex(fields, item => item == (oddProvider + "<2.5"));
                    oddIdx.Add((oddProvider + "<2.5"), idxL25);
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

                    if (newFixture.odds.Count != 5)
                    {
                        newFixture.odds.Add("1", 1);
                        newFixture.odds.Add("X", 1);
                        newFixture.odds.Add("2", 1);
                        newFixture.odds.Add(">2.5", 1);
                        newFixture.odds.Add("<2.5", 1);
                    }

                    if (configManager_.GetUseExpanded())
                    {
                        newFixture.points.homeTeamPoints = int.Parse(fields[idxHPTS]);
                        newFixture.points.awayTeamPoints = int.Parse(fields[idxAPTS]);
                        newFixture.gamesPlayed.homeTeamGamesPlayed = int.Parse(fields[idxHPL]);
                        newFixture.gamesPlayed.awayTeamGamesPlayed = int.Parse(fields[idxAPL]);
                        newFixture.fairOdds.Add("1", double.Parse(fields[idxFOH]));
                        newFixture.fairOdds.Add("X", double.Parse(fields[idxFOD]));
                        newFixture.fairOdds.Add("2", double.Parse(fields[idxFOA]));
                    }
                    newFixture.Init(configManager_);

                    result.Add(newFixture);
                }
            }

            return fixturesCache.GetOrAdd(year, result);
        }

        private bool TryGetOddData(string oddProvider, string[] fields, Dictionary<string, int> oddIdx, ref Fixture fixture)
        {
            string idH = oddProvider + "H";
            string idD = oddProvider + "D";
            string idA = oddProvider + "A";
            string idM25 = oddProvider + ">2.5";
            string idL25 = oddProvider + "<2.5";

            try
            {
                double odd1 = double.Parse(fields[oddIdx[idH]]);
                if (odd1 < 1) return false;
                double oddD = double.Parse(fields[oddIdx[idD]]);
                if (oddD < 1) return false;
                double oddA = double.Parse(fields[oddIdx[idA]]);
                if (oddA < 1) return false;
                double oddM25 = double.Parse(fields[oddIdx[idM25]]);
                if (oddA < 1) return false;
                double oddL25 = double.Parse(fields[oddIdx[idL25]]);
                if (oddA < 1) return false;

                fixture.odds.Add("1", odd1);
                fixture.odds.Add("X", oddD);
                fixture.odds.Add("2", oddA);
                fixture.odds.Add(">2.5", oddM25);
                fixture.odds.Add("<2.5", oddL25);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public override List<Fixture> GetRound(int year, int matchDay)
        {
            int key = year * 100 + matchDay;

            if (matchdayFixtureCache.TryGetValue(key, out List<Fixture> value))
            {
                return value;
            }

            List<Fixture> all = GetAllFixtures(year);

            int gamesPerMatchDay = GetGamesPerMatchDay(year);
            int startRow = (matchDay - 1) * gamesPerMatchDay;

            List<Fixture> result = new List<Fixture>(gamesPerMatchDay);
            for (int i = 0; i < gamesPerMatchDay; ++i)
            {
                result.Add(all[startRow + i]);
            }

            return matchdayFixtureCache.GetOrAdd(key, result);
        }

        private readonly ConcurrentDictionary<int, List<Fixture>> fixturesCache = new ConcurrentDictionary<int, List<Fixture>>(10, 31);
        private readonly ConcurrentDictionary<int, List<Fixture>> matchdayFixtureCache = new ConcurrentDictionary<int, List<Fixture>>(10, 809);
        private readonly ConcurrentDictionary<int, List<Fixture>> fixturesTeamCache = new ConcurrentDictionary<int, List<Fixture>>(10, 809);

        private readonly ConcurrentDictionary<int, int> fixturesIndexCache = new ConcurrentDictionary<int, int>(10, 9679);
        private readonly ConcurrentDictionary<int, int> numberOfTeamsCache = new ConcurrentDictionary<int, int>(10, 31);

        private readonly ConfigManagerInterface configManager_;
    }
}
