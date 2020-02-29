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
    class DBUpdater
    {
        private static void AddToDict(ref Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] += value;
        }

        public static void AddPoints()
        {
            int year = ConfigManager.Instance.GetYear();
            int reverseYears = ConfigManager.Instance.GetReverseYears();

            for(int i = 0; i< reverseYears;++i)
            {
                AddPointsForYear(year - i);
            }
        }

        public static void AddPointsForYear(int year)
        {
            Dictionary<string, int> currentPoints = new Dictionary<string, int>();
            Dictionary<string, int> currentPlayed = new Dictionary<string, int>();
            string leagueName = ConfigManager.Instance.GetLeagueName();
            string inputFilePath = "..\\..\\DB\\" + leagueName + year + ".csv";
            string outputFilePath = "..\\..\\DBEX\\" + leagueName + year + ".csv";
            File.Delete(outputFilePath);
            using (TextFieldParser parser = new TextFieldParser(inputFilePath))
            {
                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Append, FileAccess.Write))
                using (var outputFile = new StreamWriter(fileStream))
                {
                    string outputLine = parser.ReadLine() + ',' + "HPTS" + ',' + "APTS"+ ',' + "HPL" + ',' + "APL";
                    outputFile.WriteLine(outputLine);

                    List<Fixture> fixtures = FixtureRetriever.GetAllFixtures(year);
                    int index = 0;

                    while (!parser.EndOfData)
                    {
                        Fixture fixture = fixtures[index];

                        int homePointCnt = currentPoints.ContainsKey(fixture.homeTeamName) ? currentPoints[fixture.homeTeamName] : 0;
                        int awayPointCnt = currentPoints.ContainsKey(fixture.awayTeamName) ? currentPoints[fixture.awayTeamName] : 0;
                        int homePlayCnt = currentPlayed.ContainsKey(fixture.homeTeamName) ? currentPlayed[fixture.homeTeamName] : 0;
                        int awayPlayCnt = currentPlayed.ContainsKey(fixture.awayTeamName) ? currentPlayed[fixture.awayTeamName] : 0;

                        outputLine = parser.ReadLine() + ',' + homePointCnt.ToString() + ',' + awayPointCnt.ToString() + ',' + homePlayCnt + ',' + awayPlayCnt;
                        outputFile.WriteLine(outputLine);

                        if (fixture.result == "1")
                        {
                            AddToDict(ref currentPoints, fixture.homeTeamName, 3);

                        }
                        else if (fixture.result == "X")
                        {
                            AddToDict(ref currentPoints, fixture.homeTeamName, 1);
                            AddToDict(ref currentPoints, fixture.awayTeamName, 1);
                        }
                        else
                        {
                            AddToDict(ref currentPoints, fixture.awayTeamName, 3);
                        }

                        AddToDict(ref currentPlayed, fixture.homeTeamName, 1);
                        AddToDict(ref currentPlayed, fixture.awayTeamName, 1);

                        index++;
                    }
                }

                
            }
        }
    }
}
