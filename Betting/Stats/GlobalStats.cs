﻿using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Stats
{
    class GlobalStats
    {
        public GlobalStats(List<MetricConfig> metricConfigs)
        {
            this.metricConfigs = metricConfigs;
        }

        public void GetAllYearsData(out bool success, out float rate, out float averageProfit)
        {
            int year = ConfigManager.Instance.GetYear();
            int reverseYears = ConfigManager.Instance.GetReverseYears();
     
            float laverageProfit = 0;
            bool lsuccess = true;
            float lrate = 0;

            int maxThreads = ConfigManager.Instance.GetLogLevel() == ConfigManager.LogLevel.LOG_DEBUG ? 1 : 8;

            Parallel.For(0, reverseYears, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, (i, state) =>
            {
                int yearTotal = 0;
                int yearCorrect = 0;
                float yearProfit = 0;
                int computeYear = year - i;
                GetYearData(out yearCorrect, out yearTotal, out yearProfit, computeYear);
                float yearRate = (yearTotal != 0) ? ((float)yearCorrect / (float)yearTotal) * 100 : 100;
                lrate += yearRate;

                Logger.LogInfo("\nGlobal success rate : {0:0.00}, profit {1:0.00} on year {2}  -------\n\n", yearRate, yearProfit, computeYear);

                if (yearProfit < ConfigManager.Instance.GetMinYearProfit())
                    lsuccess = false;

                laverageProfit += yearProfit;
            });

            success = lsuccess;
            rate = lrate/reverseYears;
            averageProfit = laverageProfit/reverseYears;
        }

        public void GetYearData(out int correctFixturesWithData, out int totalFixturesWithData, out float currentProfit, int year)
        {
            int matchDay = ConfigManager.Instance.GetMatchDay();
            int reverseDays = ConfigManager.Instance.GetReverseDays();
            int totalFixtures = 0;
            int correctFixtures = 0;
            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            currentProfit = 0;
            float matchdayProfit = 0;

            for (int i = 0; i < reverseDays; ++i)
            {
                //GetMatchdayDataFairOdds(out correctFixtures, out totalFixtures, out matchdayProfit, matchDay, year);
                GetMatchdayData(out correctFixtures, out totalFixtures, out matchdayProfit, matchDay, year);

                Logger.LogDebug("Matchday : {0} - year {1}, correct {2}\t total {3}, rate {4}, profit {5:0.00} \n", matchDay, year, correctFixtures, totalFixtures, ((float)correctFixtures / (float)totalFixtures) * 100, matchdayProfit);

                correctFixturesWithData += correctFixtures;
                totalFixturesWithData += totalFixtures;
                currentProfit += matchdayProfit;
                FixtureRetriever.GetPrevRound(out year, out matchDay, year, matchDay);
            }
        }

        private int GetNumOnesInInteger(int n)
        {
            int count = 0;
            while (n > 0)
            {
                count += n & 1;
                n >>= 1;
            }
            return count;
        }

        private float GetCombinationProfit(List<float> matchdayOdds, int betStyleMask)
        {
            double count = Math.Pow(2, matchdayOdds.Count);
            float profit = 0;
            for (int i = 1; i <= count - 1; i++)
            {
                if ((betStyleMask & (1 << GetNumOnesInInteger(i))) == 0)
                    continue;

                float cProfit = 1;
                for (int j = 0; j < matchdayOdds.Count; j++)
                {
                    if ( ((i >> j) & 1) == 1)
                    {
                        cProfit *= matchdayOdds[j];
                    }
                }
                if (cProfit > 0)
                    profit += cProfit - 1;
                else
                    profit -= 1;
            }
            return profit;
        }

        private string ComputeExpectedResult(string aggregateResult, int totalMetricsWithData)
        {
            string computedResult = String.Empty;
            int goodMetricsCount = (int)(ConfigManager.Instance.GetMinMetricCorrect() * (float)totalMetricsWithData);
            //int goodMetricsCount = 1;//(totalMetricsWithData+1) / 2;//ceil
            string possibleResults = "1X2";
            foreach (char result in possibleResults)
            {
                int count = aggregateResult.Count(f => f == result);
                if (count >= goodMetricsCount)
                {
                    computedResult += result;
                }
            }


            if (computedResult == "1" && aggregateResult.Contains('X'))
            {
                computedResult = "1X";
            }
            else if (computedResult == "2" && aggregateResult.Contains('X'))
            {
                computedResult = "X2";
            }
            else if (computedResult == "X" /*|| computedResult == ""*/)
            {
                int count1 = aggregateResult.Count(f => f == '1');
                int count2 = aggregateResult.Count(f => f == '2');
                if (count1 >= (goodMetricsCount+1)/2 && !aggregateResult.Contains('2'))
                    computedResult = "1X";
                else if (count2 >= (goodMetricsCount+1) / 2 && !aggregateResult.Contains('1'))
                    computedResult = "X2";
                else
                    computedResult = "";
            }

            //bad expected 1X2
            if (computedResult.Length == 3 || computedResult.Length == 0)
            {
                computedResult = "";
            }
            return computedResult;
        }

        public float GetMatchdayProfit(List<float> matchdayOdds, int year)
        {
            int maxGames = matchdayOdds.Count;
            string betStyle = ConfigManager.Instance.GetBetStyle();

            int betStyleMask = 0;

            //max
            if (betStyle.Contains("max") && !betStyle.Contains(maxGames.ToString()))
            {
                betStyleMask |= 1 << maxGames;
                betStyle = betStyle.Replace("max", "");
            }

            //all
            if (betStyle.Contains("all"))
            {
                for (int i = 1; i <= maxGames; ++i)
                    betStyleMask |= 1 << i;
                betStyle = betStyle.Replace("all", "");
            }

            
            for(int i=0;i<betStyle.Count();++i)
            {
                betStyleMask |= 1 << (betStyle[i] -'0');
            }

            return GetCombinationProfit(matchdayOdds, betStyleMask);
        }

        public void GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out float currentProfit, int matchDay, int year)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(metricConfigs, year);

            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            List<float> matchdayOdds = new List<float>();

            foreach (Fixture fixture in thisRoundFixtures)
            {
                int totalMetricsWithData = 0;
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
                    actualResult = fixture.result;
                }

                float oddDiff1 = fixture.odds["1"] - fixture.fairOdds["1"];
                if (oddDiff1 > 0 && oddDiff1 < 1 
                    && fixture.odds["1"] <= ConfigManager.Instance.GetMaxOdds()
                    && fixture.odds["1"] >= ConfigManager.Instance.GetMinOdds())
                {
                    aggregateResult += "1 ";
                    totalMetricsWithData++;
                }

                /*float oddDiffX = fixture.odds["X"] - fixture.fairOdds["X"];
                if (oddDiffX > 0 && oddDiffX < 1
                    && fixture.odds["X"] <= ConfigManager.Instance.GetMaxOdds()
                    && fixture.odds["X"] >= ConfigManager.Instance.GetMinOdds())
                {
                    aggregateResult += "X ";
                    totalMetricsWithData++;
                }*/

                string computedResult = ComputeExpectedResult(aggregateResult, totalMetricsWithData);
                
                if (computedResult.Length == 0)
                {
                    totalMetricsWithData = 0;
                }
                else if (fixture.odds[computedResult] > ConfigManager.Instance.GetMaxOdds() ||
                    fixture.odds[computedResult] < ConfigManager.Instance.GetMinOdds())
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
                        Logger.LogDebugSuccess("{0} - {1},{2} result {3}({4}), \t odds {5:0.00} \t aggregate {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, fixture.odds[computedResult], aggregateResult);
                        matchdayOdds.Add(fixture.odds[computedResult]);
                    }
                    else
                    {
                        Logger.LogDebugFail("{0} - {1},{2} result {3}({4}), \t odds {5:0.00} \t aggregate {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, fixture.odds[computedResult], aggregateResult);
                        matchdayOdds.Add(0);
                    }
                }
                else
                {
                    Logger.LogDebug("{0} - {1},{2} result {3}({4}), \t odds {5:0.00} \t aggregate {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, fixture.odds[computedResult], aggregateResult);
                }

            }

            currentProfit = GetMatchdayProfit(matchdayOdds, year);
        }

        public void GetMatchdayDataFairOdds(out int correctFixturesWithData, out int totalFixturesWithData, out float currentProfit, int matchDay, int year)
        {
            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            List<float> matchdayOdds = new List<float>();

            foreach (Fixture fixture in thisRoundFixtures)
            {
                string actualResult = fixture.result;

                string padding = new string(' ', 50 - fixture.homeTeamName.Length - fixture.awayTeamName.Length);

                bool success = false;
                string[] resultsToAccount = { "1", "X" };
                foreach (string result in resultsToAccount)
                {
                    if (fixture.fairOdds[result] <= ConfigManager.Instance.GetMaxOdds()
                    && fixture.fairOdds[result] >= ConfigManager.Instance.GetMinOdds())
                    {
                        bool metricSuccess = actualResult == result;

                        totalFixturesWithData++;
                        if (metricSuccess)
                            correctFixturesWithData++;

                        if (metricSuccess)
                        {
                            Logger.LogDebugSuccess("{0} - {1},{2} result {3} \n", fixture.homeTeamName, fixture.awayTeamName, padding, actualResult);
                            matchdayOdds.Add(fixture.odds[result]);
                        }
                        else
                        {
                            Logger.LogDebugFail("{0} - {1},{2} result {3} \n", fixture.homeTeamName, fixture.awayTeamName, padding, actualResult);
                            matchdayOdds.Add(0);
                        }
                        success = true;
                        break;
                    }

                }
                if(!success)
                {
                    Logger.LogDebug("{0} - {1},{2} result {3}\n", fixture.homeTeamName, fixture.awayTeamName, padding, actualResult);
                }
                

            }

            currentProfit = GetMatchdayProfit(matchdayOdds, year);
        }

        public void ProcessUpcomingFixtures()
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(metricConfigs, ConfigManager.Instance.GetYear());

            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(ConfigManager.Instance.GetYear(), ConfigManager.Instance.GetMatchDay());

            List<float> matchdayOdds = new List<float>();

            foreach (Fixture fixture in thisRoundFixtures)
            {
                int totalMetricsWithData = 0;
                string aggregateResult = String.Empty;
                foreach (MetricInterface metric in metrics)
                {
                    ResultChecker checker = new ResultChecker(metric, fixture);
                    if (checker.dataAvailable)
                    {
                        aggregateResult += checker.GetExpectedResult() + " ";
                        totalMetricsWithData++;
                    }

                }

                string computedResult = ComputeExpectedResult(aggregateResult, totalMetricsWithData);

                if (computedResult.Length == 0)
                {
                    totalMetricsWithData = 0;
                }
                else if (fixture.odds[computedResult] > ConfigManager.Instance.GetMaxOdds() &&
                    fixture.odds[computedResult] < ConfigManager.Instance.GetMinOdds())
                {
                    totalMetricsWithData = 0;
                }


                string padding = new string(' ', 50 - fixture.homeTeamName.Length - fixture.awayTeamName.Length);
                if (totalMetricsWithData == metrics.Count && computedResult != String.Empty)
                {
                    Logger.LogInfo("{0} - {1},{2} result {3}, \t odds {4:0.00} \t aggregate {5} \t date {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, fixture.odds[computedResult], aggregateResult, fixture.date);
                    matchdayOdds.Add(fixture.odds[computedResult]);
                }
                else
                {
                    Logger.LogInfoFail("{0} - {1},{2} result {3}, \t odds {4:0.00} \t aggregate {5} \t date {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, fixture.odds[computedResult], aggregateResult, fixture.date);
                }

            }

            Logger.LogInfo("Profit: {0}", GetMatchdayProfit(matchdayOdds, ConfigManager.Instance.GetYear()));
        }

        private List<MetricConfig> metricConfigs;
    }
}