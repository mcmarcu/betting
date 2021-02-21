using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Models.Regression.Linear;
using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Betting.Stats
{
    internal class DBUpdater
    {
        public DBUpdater(List<MetricConfig> metricConfigs, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever)
        {
            fixtureRetriever_ = fixtureRetriever;
            configManager_ = configManager;
            metricConfigs_ = metricConfigs;
            metricD_ = 0;
            foreach (MetricConfig m in metricConfigs_)
            {
                if (m.depth > metricD_)
                {
                    metricD_ = m.depth;
                }
            }
            r2Values_ = new Dictionary<char, double>();
        }

        private PolynomialRegression GenerateRegressionFitting(SortedDictionary<double, double> values, char result)
        {
            // Extract inputs and outputs
            double[] inputs = values.Keys.ToArray();
            double[] outputs = values.Values.ToArray();

            // We can create a learning algorithm
            PolynomialLeastSquares ls = new PolynomialLeastSquares()
            {
                Degree = 2
            };

            // Now, we can use the algorithm to learn a polynomial
            PolynomialRegression poly = ls.Learn(inputs, outputs);

            // The learned polynomial will be given by
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            string str = poly.ToString("N1"); // "y(x) = 1.0x^2 + 0.0x^1 + 0.0"

            // Where its weights can be accessed using
            double[] weights = poly.Weights;   // { 1.0000000000000024, -1.2407665029287351E-13 }
            double intercept = poly.Intercept; // 1.5652369518855253E-12
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            // Finally, we can use this polynomial
            // to predict values for the input data
            double[] prediction = poly.Transform(inputs);

            double r2 = new RSquaredLoss(outputs.Length, outputs).Loss(prediction); // should be > 0.85 (close to 1 is ok)
            //LastGamesMetric:   0.77 0.81 0.08
            //GoalsScoredMetric: 0.75 0.85 0.02
            if (r2 == 1.0)
            {
                r2 = 0.0;
            }

            r2Values_.Add(result, r2);

            return poly;
        }


        private void AddToDict(ref Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] += value;
            }
        }

        private void AddToDict(ref SortedDictionary<double, double> dict, int key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] += value;
            }
        }


        public void AddPoints(bool writeToFile)
        {
            int year = configManager_.GetYear();
            int reverseYears = configManager_.GetReverseYears();
            MemoryStream statsMemoryStream = new MemoryStream(1024);
            MemoryStream ratingsMemoryStream = new MemoryStream(1024);


            int width = 12;
            for (int i = 0; i < reverseYears; ++i)
            {
                AddPointsForYear(year - i);
                GenerateStatsForYear(year - i, i == 0, ref statsMemoryStream);
                GenerateRatingsForYear(year - i, width, i == 0, ref ratingsMemoryStream);
            }

            AggregateTotalRatings(ref ratingsMemoryStream);
            PolynomialRegression r1 = GenerateEquation(ratingsMemoryStream, width, '1');
            PolynomialRegression rx = GenerateEquation(ratingsMemoryStream, width, 'X');
            PolynomialRegression r2 = GenerateEquation(ratingsMemoryStream, width, '2');

            if (writeToFile)
            {
                for (int i = 0; i < reverseYears; ++i)
                {
                    AddFairOddsForYear(year - i, r1, rx, r2);
                }
            }
        }

        public PolynomialRegression GenerateEquation(MemoryStream ratingsSteam, int width, char result)
        {
            string prefix = "GDM6";
            SortedDictionary<double, double> points = new SortedDictionary<double, double>();
            {
                ratingsSteam.Seek(0, SeekOrigin.Begin);
                TextFieldParser parser = new TextFieldParser(ratingsSteam)
                {
                    TextFieldType = FieldType.Delimited
                };
                parser.SetDelimiters(",");

                string[] fields = parser.ReadFields();

                while (!parser.EndOfData)
                {
                    string[] stringFields = parser.ReadFields();
                    if (stringFields[0] != "PCT")
                    {
                        continue;
                    }

                    stringFields[0] = "0";
                    double[] doubleFields = Array.ConvertAll(stringFields, s => double.Parse(s));

                    for (int i = -width; i <= width; ++i)
                    {
                        int idx = Array.FindIndex(fields, item => item == prefix + result + i.ToString());
                        points.Add(i, doubleFields[idx]);
                    }
                }
            }

            return GenerateRegressionFitting(points, result);
        }

        public void AggregateTotalRatings(ref MemoryStream ratingsMemoryStream)
        {
            double[] result = null;
            {
                ratingsMemoryStream.Seek(0, SeekOrigin.Begin);
                TextFieldParser parser = new TextFieldParser(ratingsMemoryStream)
                {
                    TextFieldType = FieldType.Delimited
                };
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
            {
                ratingsMemoryStream.Seek(0, SeekOrigin.End);
                StreamWriter outputFile = new StreamWriter(ratingsMemoryStream);
                string outputLine = "SUM" + ',';
                for (int i = 1; i < result.Length; ++i)
                {
                    outputLine += result[i].ToString();
                    if (i < result.Length - 1)
                    {
                        outputLine += ',';
                    }
                }
                outputFile.WriteLine(outputLine);

                outputLine = "PCT" + ',';
                for (int i = 1; i < result.Length; i += 4)
                {
                    outputLine += (result[i] / result[i + 3] * 100).ToString("0.00") + ',';
                    outputLine += (result[i + 1] / result[i + 3] * 100).ToString("0.00") + ',';
                    outputLine += (result[i + 2] / result[i + 3] * 100).ToString("0.00") + ',';
                    outputLine += "0";
                    if (i < result.Length - 4)
                    {
                        outputLine += ',';
                    }
                }
                outputFile.WriteLine(outputLine);
                outputFile.Flush();
            }
        }

        //as described in https://www.football-data.co.uk/ratings.pdf
        public void GenerateRatingsForYear(int year, int width, bool writeHeader, ref MemoryStream ratingsMemorySteam)
        {
            int matchdays = fixtureRetriever_.GetNumberOfMatchDays(year);
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

            List<MetricInterface> metrics = MetricFactory.GetMetrics(metricConfigs_, year, configManager_, fixtureRetriever_);
            for (int i = matchdays; i > metricD_; --i)
            {
                List<Fixture> thisRoundFixtures = fixtureRetriever_.GetRound(year, i);

                foreach (Fixture fixture in thisRoundFixtures)
                {
                    int diff = 0;
                    foreach (MetricInterface m in metrics)
                    {
                        m.GetPoints(out double pctTeam1, out double pctTeam2, fixture.homeTeamId, fixture.awayTeamId, fixture);
                        diff += (int)pctTeam1;
                        diff -= (int)pctTeam2;
                    }

                    if (diff < -width || diff > width)
                    {
                        continue;
                    }

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
            string outputLine = "YEAR" + ',';
            for (int i = -width; i <= width; ++i)
            {
                outputLine += keyPrefix + "1" + i.ToString() + ',';
                outputLine += keyPrefix + "X" + i.ToString() + ',';
                outputLine += keyPrefix + "2" + i.ToString() + ',';
                outputLine += keyPrefix + "T" + i.ToString();

                if (i < width)
                {
                    outputLine += ',';
                }
            }

            {
                StreamWriter outputFile = new StreamWriter(ratingsMemorySteam);
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
                    if (i < width)
                    {
                        outputLine += ',';
                    }
                }
                outputFile.WriteLine(outputLine);
                outputFile.Flush();
            }
        }

        public void GenerateStatsForYear(int year, bool writeHeader, ref MemoryStream memoryStream)
        {
            {
                StreamWriter outputFile = new StreamWriter(memoryStream);
                string outputLine;
                if (writeHeader)
                {
                    outputLine = "YEAR" + ',' + "HWPCT" + ',' + "AWPCT" + ',' + "DPCT";
                    outputFile.WriteLine(outputLine);
                }

                List<Fixture> fixtures = fixtureRetriever_.GetAllFixtures(year);
                float HWIN = 0;
                float AWIN = 0;
                float DRAWS = 0;
                foreach (Fixture fixture in fixtures)
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
                outputFile.Flush();
            }
        }

        public void AddPointsForYear(int year)
        {
            Dictionary<string, int> currentPoints = new Dictionary<string, int>();
            Dictionary<string, int> currentPlayed = new Dictionary<string, int>();
            string leagueName = configManager_.GetLeagueName();
            string inputFilePath = "..\\..\\DB\\" + leagueName + year + ".csv";
            string outputFilePath = "..\\..\\DBEX\\" + leagueName + year + ".csv";

            if (File.Exists(outputFilePath))
            {
                return;
            }

            using TextFieldParser parser = new TextFieldParser(inputFilePath);
            using FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
            using StreamWriter outputFile = new StreamWriter(fileStream);
            string outputLine = parser.ReadLine() + ',' + "HPTS" + ',' + "APTS" + ',' + "HPL" + ',' + "APL";
            outputFile.WriteLine(outputLine);

            List<Fixture> fixtures = fixtureRetriever_.GetAllFixtures(year);
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


        public void AddFairOddsForYear(int year, PolynomialRegression r1, PolynomialRegression rx, PolynomialRegression r2)
        {
            string leagueName = configManager_.GetLeagueName();
            string inputFilePath = "..\\..\\DBEX\\" + leagueName + year + ".csv";
            string outputFilePath = "..\\..\\DBEX\\" + leagueName + year + "_ex.csv";

            List<MetricInterface> metrics = MetricFactory.GetMetrics(metricConfigs_, year, configManager_, fixtureRetriever_);

            using (TextFieldParser parser = new TextFieldParser(inputFilePath))
            {
                using FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
                using StreamWriter outputFile = new StreamWriter(fileStream);
                string outputLine = parser.ReadLine() + ',' + "FOH" + ',' + "FOD" + ',' + "FOA";
                outputFile.WriteLine(outputLine);

                List<Fixture> fixtures = fixtureRetriever_.GetAllFixtures(year);
                int index = 0;
                while (!parser.EndOfData)
                {
                    Fixture fixture = fixtures[index];
                    int diff = 0;
                    foreach (MetricInterface m in metrics)
                    {
                        m.GetPoints(out double pctTeam1, out double pctTeam2, fixture.homeTeamId, fixture.awayTeamId, fixture);
                        diff += (int)pctTeam1;
                        diff -= (int)pctTeam2;
                    }

                    outputLine = parser.ReadLine() + ',' + (100 / r1.Transform(diff)).ToString("0.00") + ',' + (100 / rx.Transform(diff)).ToString("0.00") + ',' + (100 / r2.Transform(diff)).ToString("0.00");
                    outputFile.WriteLine(outputLine);
                    index++;
                }
            }

            File.Delete(inputFilePath);
            File.Move(outputFilePath, inputFilePath);
        }

        private readonly List<MetricConfig> metricConfigs_;
        private readonly int metricD_;
        public Dictionary<char, double> r2Values_;
        private readonly ConfigManagerInterface configManager_;
        private readonly FixtureRetrieverInterface fixtureRetriever_;

    }
}
