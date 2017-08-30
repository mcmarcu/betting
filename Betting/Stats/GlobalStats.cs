using Betting.DataModel;
using Betting.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Betting.Stats
{
    public class GlobalStats
    {

        public static void GetData(out int correctFixturesWithData, out int totalFixturesWithData, int matchDay, int year)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(matchDay, year);

            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;


            foreach (Fixture fixture in thisRoundFixtures)
            {
                int correctMetricsWithData = 0;
                int totalMetricsWithData = 0;
                string computedResult = String.Empty;
                string actualResult = String.Empty;
                foreach (MetricInterface metric in metrics)
                {
                    ResultChecker checker = new ResultChecker(metric, fixture);
                    ResultChecker.InterpretResultStatus status = checker.InterpretResult();
                    if (status != ResultChecker.InterpretResultStatus.NODATA)
                    {
                        totalMetricsWithData++;
                        if (status == ResultChecker.InterpretResultStatus.CORRECT)
                        {
                            correctMetricsWithData++;
                            actualResult = checker.GetActualResult();
                            if (computedResult == String.Empty)
                                computedResult = checker.GetExpectedResult();
                            else
                                computedResult = new String(computedResult.Intersect(checker.GetExpectedResult()).ToArray());
                        }
                    }
                }
                string padding = new string(' ', 50 - fixture.homeTeamName.Length - fixture.awayTeamName.Length);
                if (totalMetricsWithData == metrics.Count)
                {
                    bool metricSuccess = ((float)correctMetricsWithData / (float)totalMetricsWithData) >= 0.66;

                    totalFixturesWithData++;
                    if (metricSuccess)
                        correctFixturesWithData++;

                    
                    if (metricSuccess)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("{0} - {1},{2} correctMetrics {3} result {4}({5}) \n", fixture.homeTeamName, fixture.awayTeamName, padding, correctMetricsWithData, computedResult, actualResult);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("{0} - {1},{2} correctMetrics {3} \n", fixture.homeTeamName, fixture.awayTeamName, padding, correctMetricsWithData);
                    }

                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("{0} - {1},{2} no data \n", fixture.homeTeamName, fixture.awayTeamName, padding);
                    Console.ResetColor();
                }
            }

        }

        public static void GetDataEx(out int correctFixturesWithData, out int totalFixturesWithData, int matchDay, int year)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(matchDay, year);

            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;


            foreach (Fixture fixture in thisRoundFixtures)
            {
                int totalMetricsWithData = 0;
                string computedResult = String.Empty;
                string aggregateResult = String.Empty;
                string actualResult = String.Empty;
                foreach (MetricInterface metric in metrics)
                {
                    ResultChecker checker = new ResultChecker(metric, fixture);
                    if (checker.dataAvailable)
                    {
                        aggregateResult += checker.GetExpectedResult() + " ";
                        totalMetricsWithData++;
                    }
                    actualResult = checker.GetActualResult();

                }

                string possibleResults = "1X2";
                foreach(char result in possibleResults)
                {
                    if (aggregateResult.Split(result).Length - 1 == metrics.Count)
                        computedResult += result;
                    if (aggregateResult.Split(result).Length - 1 == metrics.Count -1)
                        computedResult += result;
                }

                string padding = new string(' ', 50 - fixture.homeTeamName.Length - fixture.awayTeamName.Length);
                if (totalMetricsWithData == metrics.Count && computedResult != String.Empty && actualResult != String.Empty)
                {
                    bool metricSuccess = computedResult.Contains(actualResult);

                    totalFixturesWithData++;
                    if (metricSuccess)
                        correctFixturesWithData++;

                    if (metricSuccess)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.Write("{0} - {1},{2} result {3}({4}), \t aggregate {5} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, aggregateResult);

                Console.ResetColor();
            }

        }
    }
}