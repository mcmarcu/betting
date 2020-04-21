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
using Betting.Metrics;
using Accord.Math;
using Accord.Statistics.Models.Regression.Fitting;
using Accord.Math.Optimization;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Math.Optimization.Losses;

namespace Betting.Stats
{
    class DBUpdater
    {

        private static PolynomialRegression GenerateRegressionFitting(SortedDictionary<double, double> values)
        {
            // Extract inputs and outputs
            double[] inputs = values.Keys.ToArray();
            double[] outputs = values.Values.ToArray();

            // We can create a learning algorithm
            var ls = new PolynomialLeastSquares()
            {
                Degree = 2
            };

            // Now, we can use the algorithm to learn a polynomial
            PolynomialRegression poly = ls.Learn(inputs, outputs);

            // The learned polynomial will be given by
            string str = poly.ToString("N1"); // "y(x) = 1.0x^2 + 0.0x^1 + 0.0"

            // Where its weights can be accessed using
            double[] weights = poly.Weights;   // { 1.0000000000000024, -1.2407665029287351E-13 }
            double intercept = poly.Intercept; // 1.5652369518855253E-12

            // Finally, we can use this polynomial
            // to predict values for the input data
            double[] prediction = poly.Transform(inputs);

            double r2 = new RSquaredLoss(outputs.Length, outputs).Loss(prediction); // should be > 0.85 (close to 1 is ok)

            return poly;
        }


        private static void AddToDict(ref Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] += value;
        }

        private static void AddToDict(ref SortedDictionary<double, double> dict, int key, int value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] += value;
        }

        public static void AddPoints()
        {
            string leagueName = ConfigManager.Instance.GetLeagueName();
            int year = ConfigManager.Instance.GetYear();
            int reverseYears = ConfigManager.Instance.GetReverseYears();
            string outputStatsFilePath = "..\\..\\DBEX\\" + leagueName + "_stats.csv";
            string outputRatingsFilePath = "..\\..\\DBEX\\" + leagueName + "_ratings.csv";
            File.Delete(outputStatsFilePath);
            File.Delete(outputRatingsFilePath);


            int width = 12;
            for (int i = 0; i< reverseYears;++i)
            {
                AddPointsForYear(year - i);
                GenerateStatsForYear(year - i, i==0, outputStatsFilePath);
                GenerateRatingsForYear(year - i, width, i == 0, outputRatingsFilePath);
            }

            
            AggregateTotalRatings(outputRatingsFilePath);
            PolynomialRegression r1 = GenerateEquation(outputRatingsFilePath, width, '1');
            PolynomialRegression rx = GenerateEquation(outputRatingsFilePath, width, 'X');
            PolynomialRegression r2 = GenerateEquation(outputRatingsFilePath, width, '2');

            for (int i = 0; i < reverseYears; ++i)
            {
                AddFairOddsForYear(year - i, r1, rx, r2);
            }
        }

        public static PolynomialRegression GenerateEquation(string ratingsFilePath, int width, char result)
        {
            string prefix = "GDM6";
            SortedDictionary<double, double> points = new SortedDictionary<double, double>();
            using (TextFieldParser parser = new TextFieldParser(ratingsFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string[] fields = parser.ReadFields();
                
                while (!parser.EndOfData)
                {
                    string[] stringFields = parser.ReadFields();
                    if (stringFields[0] != "PCT")
                        continue;

                    stringFields[0] = "0";
                    double[] doubleFields = Array.ConvertAll(stringFields, s => double.Parse(s));

                    for (int i = -width; i <= width; ++i)
                    {
                        int idx = Array.FindIndex(fields, item => item == prefix + result + i.ToString());
                        points.Add(i, doubleFields[idx]);
                    }
                }
            }

            return GenerateRegressionFitting(points);
        }

        public static void AggregateTotalRatings(string outputRatingsFilePath)
        {
            double[] result = null;
            using (TextFieldParser parser = new TextFieldParser(outputRatingsFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                parser.ReadFields();
                while (!parser.EndOfData)
                {
                    string[] stringFields = parser.ReadFields();
                    double[] fields = Array.ConvertAll(stringFields, s => double.Parse(s));
                    if (result == null)
                    {
                        result = fields;
                    }
                    else
                    {
                        for (int i = 0; i < fields.Length; ++i)
                        {
                            result[i] += fields[i];
                        }
                    }
                }
            }

            result[0] = 0;
            using (FileStream fileStream = new FileStream(outputRatingsFilePath, FileMode.Append, FileAccess.Write))
            using (var outputFile = new StreamWriter(fileStream))
            {
                string outputLine = "SUM"+',';
                for(int i=1; i<result.Length;++i)
                {
                    outputLine += result[i].ToString();
                    if (i < result.Length - 1)
                        outputLine += ',';
                }
                outputFile.WriteLine(outputLine);

                outputLine = "PCT" + ',';
                for (int i = 1; i < result.Length; i+=4)
                {
                    outputLine += (result[i] / result[i + 3]*100).ToString("0.00") + ',';
                    outputLine += (result[i + 1] / result[i + 3]*100).ToString("0.00") + ',';
                    outputLine += (result[i + 2] / result[i + 3]*100).ToString("0.00") + ',';
                    outputLine += "0";
                    if (i < result.Length - 4)
                        outputLine += ',';
                }
                outputFile.WriteLine(outputLine);
            }
        }

        public static void GenerateRatingsForYear(int year, int width, bool writeHeader, string outputRatingsFilePath)
        {
            int GoalsDiferenceMetricD = 6;
            MetricConfig goalsDiferenceMetric = new MetricConfig
            {
                name = "GoalsDiferenceMetric",
                depth = GoalsDiferenceMetricD
            };
            GoalsDiferenceMetric m = new GoalsDiferenceMetric(goalsDiferenceMetric, year);

            int matchdays = FixtureRetriever.GetNumberOfMatchDays(year);
            SortedDictionary<double, double> homeWinDiff = new SortedDictionary<double, double>();
            SortedDictionary<double, double> awayWinDiff = new SortedDictionary<double, double>();
            SortedDictionary<double, double> drawDiff = new SortedDictionary<double, double>();
            SortedDictionary<double, double> totalOnDiff = new SortedDictionary<double, double>();

            for (int i = -width; i <= width; ++i)
            {
                AddToDict(ref homeWinDiff, i, 0);
                AddToDict(ref awayWinDiff, i, 0);
                AddToDict(ref drawDiff, i, 0);
                AddToDict(ref totalOnDiff, i, 0);
            }

            for (int i= matchdays;i>GoalsDiferenceMetricD;--i)
            {
                
                List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, i);

                foreach (Fixture fixture in thisRoundFixtures)
                {
                    m.GetPoints(out int pctTeam1, out int pctTeam2, fixture.homeTeamName, fixture.awayTeamName, fixture);
                    int diff = pctTeam1 - pctTeam2;

                    if (diff < -width || diff > width)
                        continue;
 
                    if (fixture.result == "1")
                    {
                        AddToDict(ref homeWinDiff, diff, 1);
                    }
                    else if (fixture.result == "2")
                    {
                        AddToDict(ref awayWinDiff, diff, 1);
                    }
                    else
                    {
                        AddToDict(ref drawDiff, diff, 1);
                    }
                    AddToDict(ref totalOnDiff, diff, 1);
                }
            }

            string keyPrefix = "GDM6";
            string outputLine= "YEAR" + ',';
            for (int i = -width; i<= width; ++i)
            {
                outputLine += keyPrefix + "1" + i.ToString() + ',';
                outputLine += keyPrefix + "X" + i.ToString() + ',';
                outputLine += keyPrefix + "2" + i.ToString() + ',';
                outputLine += keyPrefix + "T" + i.ToString();

                if (i < width)
                    outputLine += ',';
            }

            using (FileStream fileStream = new FileStream(outputRatingsFilePath, FileMode.Append, FileAccess.Write))
            using (var outputFile = new StreamWriter(fileStream))
            {
                if (writeHeader)
                {
                    outputFile.WriteLine(outputLine);
                }

                outputLine = year.ToString() + ',';
                for (int i = -width; i <= width; ++i)
                {
                    outputLine += homeWinDiff[i].ToString() + ',';
                    outputLine += awayWinDiff[i].ToString() + ',';
                    outputLine += drawDiff[i].ToString() + ',';
                    outputLine += totalOnDiff[i].ToString();
                    if(i< width)
                        outputLine += ',';
                }
                outputFile.WriteLine(outputLine);
            }
        }

        public static void GenerateStatsForYear(int year, bool writeHeader, string outputStatsFilePath)
        {
            using (FileStream fileStream = new FileStream(outputStatsFilePath, FileMode.Append, FileAccess.Write))
            using (var outputFile = new StreamWriter(fileStream))
            {
                string outputLine;
                if (writeHeader)
                {
                    outputLine = "YEAR" + ',' + "HWPCT" + ',' + "AWPCT" + ',' + "DPCT";
                    outputFile.WriteLine(outputLine);
                }

                List<Fixture> fixtures = FixtureRetriever.GetAllFixtures(year);
                float HWIN = 0;
                float AWIN = 0;
                float DRAWS = 0;
                foreach(Fixture fixture in fixtures)
                {
                    if (fixture.result == "1")
                    {
                        HWIN++;
                    }
                    else if (fixture.result == "X")
                    {
                        AWIN++;
                    }
                    else
                    {
                        DRAWS++;
                    }
                }
                int totalFixtures = fixtures.Count;

                outputLine = year.ToString() + ',' + (HWIN / totalFixtures).ToString("0.00") + ',' + (AWIN / totalFixtures).ToString("0.00") + ',' + (DRAWS / totalFixtures).ToString("0.00");
                outputFile.WriteLine(outputLine);
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
                    string outputLine = parser.ReadLine() + ',' + "HPTS" + ',' + "APTS" + ',' + "HPL" + ',' + "APL";
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


        public static void AddFairOddsForYear(int year, PolynomialRegression r1, PolynomialRegression rx, PolynomialRegression r2)
        {
            string leagueName = ConfigManager.Instance.GetLeagueName();
            string inputFilePath = "..\\..\\DBEX\\" + leagueName + year + ".csv";
            string outputFilePath = "..\\..\\DBEX\\" + leagueName + year + "_ex.csv";
            File.Delete(outputFilePath);

            int GoalsDiferenceMetricD = 6;
            MetricConfig goalsDiferenceMetric = new MetricConfig
            {
                name = "GoalsDiferenceMetric",
                depth = GoalsDiferenceMetricD
            };
            GoalsDiferenceMetric m = new GoalsDiferenceMetric(goalsDiferenceMetric, year);

            using (TextFieldParser parser = new TextFieldParser(inputFilePath))
            {
                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Append, FileAccess.Write))
                using (var outputFile = new StreamWriter(fileStream))
                {
                    string outputLine = parser.ReadLine() + ',' + "FOH" + ',' + "FOD" + ',' + "FOA";
                    outputFile.WriteLine(outputLine);

                    List<Fixture> fixtures = FixtureRetriever.GetAllFixtures(year);
                    int index = 0;

                    while (!parser.EndOfData)
                    {
                        Fixture fixture = fixtures[index];
                        m.GetPoints(out int pctTeam1, out int pctTeam2, fixture.homeTeamName, fixture.awayTeamName, fixture);
                        double diff = pctTeam1 - pctTeam2;

                        outputLine = parser.ReadLine() + ',' + (100/r1.Transform(diff)).ToString("0.00") + ',' + (100/rx.Transform(diff)).ToString("0.00") + ',' + (100/r2.Transform(diff)).ToString("0.00");
                        outputFile.WriteLine(outputLine);
                        index++;
                    }
                
                }
            }

            File.Delete(inputFilePath);
            File.Copy(outputFilePath, inputFilePath);
            File.Delete(outputFilePath);
        }
    }
}
