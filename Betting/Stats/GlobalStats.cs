using Betting.Config;
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
        public static void GetAllYearsData(out bool success, out float averageProfit)
        {
            int year = ConfigManager.Instance.GetYear();
            int reverseYears = ConfigManager.Instance.GetReverseYears();

            int yearTotal = 0;
            int yearCorrect = 0;
            float yearProfit = 0;
            int correctYears = 0;

            averageProfit = 0;
            success = true;

            for(int i=0;i<reverseYears; ++i)
            {
                int computeYear = year - i;
                GlobalStats.GetYearData(out yearCorrect, out yearTotal, out yearProfit, computeYear);
                float rate = ((float)yearCorrect / (float)yearTotal) * 100;


                if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_EXTRA)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\nGlobal success rate : {0:0.00}, profit {1:0.00} on year {2}  -------\n\n", rate, yearProfit, computeYear);
                    Console.ResetColor();
                }

                if (yearProfit < ConfigManager.Instance.GetMinYearProfit())
                    success = false;

                if (yearProfit >= 0)
                    correctYears++;
                averageProfit += yearProfit;

            }

            if (reverseYears - correctYears > 1)
                success = false;

            if (!success)
                averageProfit = 0;
            else
                averageProfit = averageProfit/reverseYears;
        }

        public static void GetYearData(out int correctFixturesWithData, out int totalFixturesWithData, out float currentProfit, int year)
        {
            int matchDay = ConfigManager.Instance.GetMatchDay();
            int reverseDays = ConfigManager.Instance.GetReverseDays();
            int reverseYears = ConfigManager.Instance.GetReverseYears();
            int totalFixtures = 0;
            int correctFixtures = 0;
            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            currentProfit = 0;
            float matchdayProfit = 0;

            for (int i = 0; i < reverseDays; ++i)
            {
                GlobalStats.GetMatchdayData(out correctFixtures, out totalFixtures, out matchdayProfit, matchDay, year);

                if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_ALL)
                    Console.Write("Matchday : {0} - year {1}, correct {2}\t total {3}, rate {4}, profit {5:0.00} \n", matchDay, year, correctFixtures, totalFixtures, ((float)correctFixtures / (float)totalFixtures) * 100, matchdayProfit);
                correctFixturesWithData += correctFixtures;
                totalFixturesWithData += totalFixtures;
                currentProfit += matchdayProfit;
                FixtureRetriever.GetPrevRound(out year, out matchDay, year, matchDay);
            }
        }

        public static void GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out float currentProfit, int matchDay, int year)
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

                int goodMetricsCount = (int)(ConfigManager.Instance.GetMinMetricCorrect() * (float)totalMetricsWithData); 
                string possibleResults = "1X2";
                foreach(char result in possibleResults)
                {
                    if (aggregateResult.Split(result).Length - 1 >= goodMetricsCount)
                    {
                        computedResult += result;
                    }
                }

                //bad expected 1X2
                if(computedResult.Length == 3 || computedResult.Length == 0)
                {
                    computedResult = "1X2";
                    totalMetricsWithData = 0;
                }

                if (fixture.odds[computedResult] > ConfigManager.Instance.GetMaxOdds())
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

                if(ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_ALL)
                    Console.Write("{0} - {1},{2} result {3}({4}), \t odds {5:0.00} \t aggregate {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, fixture.odds[computedResult], aggregateResult);

                Console.ResetColor();
            }

        }
    }
}