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

        public static void GetData(out int correctFixturesWithData, out int totalFixturesWithData, out float currentProfit, int matchDay, int year)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(matchDay, year);

            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            currentProfit = 0;


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

                int goodMetricsCount = (int)(0.67 * (float)totalMetricsWithData); 
                string possibleResults = "1X2";
                foreach(char result in possibleResults)
                {
                    if (aggregateResult.Split(result).Length - 1 >= goodMetricsCount)
                    {
                        computedResult += result;
                    }
                }

                //bad expected 1X2
                if(computedResult.Length == 3)
                {
                    totalMetricsWithData = 0;
                }

                if (fixture.odds[computedResult] > 2.5)
                {
                    totalMetricsWithData = 0;
                }
                

                string padding = new string(' ', 50 - fixture.homeTeamName.Length - fixture.awayTeamName.Length);
                if (totalMetricsWithData == metrics.Count && computedResult != String.Empty && actualResult != String.Empty)
                {
                    bool metricSuccess = computedResult.Contains(actualResult);

                    totalFixturesWithData++;
                    if (metricSuccess)
                        correctFixturesWithData++;

                    if (metricSuccess)
                    {
                        currentProfit += (fixture.odds[computedResult] - 1);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        currentProfit -= 1;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.Write("{0} - {1},{2} result {3}({4}), \t odds {5:0.00} \t aggregate {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, fixture.odds[computedResult], aggregateResult);

                Console.ResetColor();
            }

        }
    }
}