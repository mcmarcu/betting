using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Betting.Stats
{
    public class GlobalStats
    {
        public GlobalStats(List<MetricConfig> metricConfigs, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever, Logger logger)
        {
            metricConfigs_ = metricConfigs;
            configManager_ = configManager;
            fixtureRetriever_ = fixtureRetriever;
            logger_ = logger;
        }

        public void GetAllYearsData(out bool success, out double rate, out double averageProfit)
        {
            int year = configManager_.GetYear();
            int reverseYears = configManager_.GetReverseYears();

            int successYears = 0;
            double profitSum = 0;
            int totalGames = 0;
            int correctGames = 0;

            int maxThreads = configManager_.GetLogLevel() == ConfigManager.LogLevel.LOG_DEBUG ? 1 : 8;

            Parallel.For(0, reverseYears, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, (i, state) =>
            {
                int computeYear = year - i;
                GetYearData(out int yearCorrect, out int yearTotal, out double yearProfit, computeYear);
                
                totalGames += yearTotal;
                correctGames += yearCorrect;

                double yrate = (yearTotal != 0) ? ((double)yearCorrect / (double)yearTotal) * 100 : 0;
                logger_.LogInfo("\nGlobal success rate : {0:0.00}, profit {1:0.00} on year {2}  -------\n\n", yrate, yearProfit, computeYear);

                if (yearProfit >= configManager_.GetMinYearProfit())
                    ++successYears;

                profitSum += yearProfit;
            });

            success = successYears == reverseYears;
            rate = (totalGames != 0) ? ((double)correctGames / (double)totalGames) * 100 : 0;
            averageProfit = profitSum / reverseYears;
        }

        public void GetYearData(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, int year)
        {
            int matchDay = configManager_.GetMatchDay();
            int reverseDays = configManager_.GetReverseDays();
            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            currentProfit = 0;
            for (int i = 0; i < reverseDays; ++i)
            {
                //GetMatchdayDataFairOdds(out correctFixtures, out totalFixtures, out matchdayProfit, matchDay, year);
                GetMatchdayData(out int correctFixtures, out int totalFixtures, out double matchdayProfit, matchDay, year);

                logger_.LogDebug("Matchday : {0} - year {1}, correct {2}\t total {3}, rate {4}, profit {5:0.00} \n", matchDay, year, correctFixtures, totalFixtures, ((double)correctFixtures / (double)totalFixtures) * 100, matchdayProfit);

                correctFixturesWithData += correctFixtures;
                totalFixturesWithData += totalFixtures;
                currentProfit += matchdayProfit;
                fixtureRetriever_.GetPrevRound(ref year, ref matchDay);
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

        public double GetCombinationProfit(List<double> matchdayOdds, int betStyleMask)
        {
            double count = Math.Pow(2, matchdayOdds.Count);
            double profit = 0;
            for (int i = 1; i <= count - 1; i++)
            {
                if ((betStyleMask & (1 << GetNumOnesInInteger(i))) == 0)
                    continue;

                double cProfit = 1;
                for (int j = 0; j < matchdayOdds.Count; j++)
                {
                    if (((i >> j) & 1) == 1)
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

        public string ComputeExpectedResult(string aggregateResult, int totalMetricsWithData)
        {
            string computedResult = string.Empty;
            int goodMetricsCount = (int)(configManager_.GetMinMetricCorrect() * (double)totalMetricsWithData);
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
                if (count1 >= (goodMetricsCount + 1) / 2 && !aggregateResult.Contains('2'))
                    computedResult = "1X";
                else if (count2 >= (goodMetricsCount + 1) / 2 && !aggregateResult.Contains('1'))
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

        public double GetMatchdayProfit(List<double> matchdayOdds)
        {
            int maxGames = matchdayOdds.Count;
            string betStyle = configManager_.GetBetStyle();

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


            for (int i = 0; i < betStyle.Count(); ++i)
            {
                betStyleMask |= 1 << (betStyle[i] - '0');
            }

            return GetCombinationProfit(matchdayOdds, betStyleMask);
        }

        public void GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, int matchDay, int year)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(metricConfigs_, year, configManager_, fixtureRetriever_);

            List<Fixture> thisRoundFixtures = fixtureRetriever_.GetRound(year, matchDay);

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            List<double> matchdayOdds = new List<double>(thisRoundFixtures.Count);

            foreach (Fixture fixture in thisRoundFixtures)
            {
                int totalMetricsWithData = 0;
                string aggregateResult = string.Empty;
                string actualResult = string.Empty;
                foreach (MetricInterface metric in metrics)
                {
                    ResultChecker checker = new ResultChecker(metric, fixture, configManager_);
                    if (checker.dataAvailable)
                    {
                        aggregateResult += checker.GetExpectedResult() + " ";
                        totalMetricsWithData++;
                    }
                    actualResult = fixture.result;
                }

                /*double oddDiff1 = fixture.odds["1"] - fixture.fairOdds["1"];
                if (oddDiff1 > 0 && oddDiff1 < 1 
                    && fixture.odds["1"] <= configManager_.GetMaxOdds()
                    && fixture.odds["1"] >= configManager_.GetMinOdds())
                {
                    aggregateResult += "1 ";
                    totalMetricsWithData++;
                }

                double oddDiffX = fixture.odds["X"] - fixture.fairOdds["X"];
                if (oddDiffX > 0 && oddDiffX < 1
                    && fixture.odds["X"] <= configManager_.GetMaxOdds()
                    && fixture.odds["X"] >= configManager_.GetMinOdds())
                {
                    aggregateResult += "X ";
                    totalMetricsWithData++;
                }*/

                string computedResult = ComputeExpectedResult(aggregateResult, totalMetricsWithData);

                if (computedResult.Length == 0)
                {
                    totalMetricsWithData = 0;
                }
                else if (fixture.odds[computedResult] > configManager_.GetMaxOdds() ||
                    fixture.odds[computedResult] < configManager_.GetMinOdds())
                {
                    totalMetricsWithData = 0;
                }

                string padding = new string(' ', 50 - fixture.homeTeamName.Length - fixture.awayTeamName.Length);
                if (totalMetricsWithData == metrics.Count && computedResult.Length != 0 && actualResult.Length != 0)
                {
                    bool metricSuccess = computedResult.Contains(actualResult);

                    totalFixturesWithData++;
                    if (metricSuccess)
                        correctFixturesWithData++;

                    if (metricSuccess)
                    {
                        logger_.LogDebugSuccess("{0} - {1},{2} result {3}({4}), \t odds {5:0.00} \t aggregate {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, fixture.odds[computedResult], aggregateResult);
                        matchdayOdds.Add(fixture.odds[computedResult]);
                    }
                    else
                    {
                        logger_.LogDebugFail("{0} - {1},{2} result {3}({4}), \t odds {5:0.00} \t aggregate {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, fixture.odds[computedResult], aggregateResult);
                        matchdayOdds.Add(0);
                    }
                }
                else
                {
                    logger_.LogDebug("{0} - {1},{2} result {3}({4}), \t odds {5:0.00} \t aggregate {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, actualResult, fixture.odds[computedResult], aggregateResult);
                }

            }

            currentProfit = GetMatchdayProfit(matchdayOdds);
        }

        public void GetMatchdayDataFairOdds(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, int matchDay, int year)
        {
            List<Fixture> thisRoundFixtures = fixtureRetriever_.GetRound(year, matchDay);

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            List<double> matchdayOdds = new List<double>(thisRoundFixtures.Count);

            foreach (Fixture fixture in thisRoundFixtures)
            {
                string actualResult = fixture.result;

                string padding = new string(' ', 50 - fixture.homeTeamName.Length - fixture.awayTeamName.Length);

                bool success = false;
                string[] resultsToAccount = { "1", "X" };
                foreach (string result in resultsToAccount)
                {
                    if (fixture.fairOdds[result] <= configManager_.GetMaxOdds()
                    && fixture.fairOdds[result] >= configManager_.GetMinOdds())
                    {
                        bool metricSuccess = actualResult == result;

                        totalFixturesWithData++;
                        if (metricSuccess)
                            correctFixturesWithData++;

                        if (metricSuccess)
                        {
                            logger_.LogDebugSuccess("{0} - {1},{2} result {3} \n", fixture.homeTeamName, fixture.awayTeamName, padding, actualResult);
                            matchdayOdds.Add(fixture.odds[result]);
                        }
                        else
                        {
                            logger_.LogDebugFail("{0} - {1},{2} result {3} \n", fixture.homeTeamName, fixture.awayTeamName, padding, actualResult);
                            matchdayOdds.Add(0);
                        }
                        success = true;
                        break;
                    }

                }
                if (!success)
                {
                    logger_.LogDebug("{0} - {1},{2} result {3}\n", fixture.homeTeamName, fixture.awayTeamName, padding, actualResult);
                }
            }

            currentProfit = GetMatchdayProfit(matchdayOdds);
        }

        public void ProcessUpcomingFixtures(out double expectedProfit)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(metricConfigs_, configManager_.GetYear(), configManager_, fixtureRetriever_);
            List<Fixture> thisRoundFixtures = fixtureRetriever_.GetRound(configManager_.GetYear(), configManager_.GetMatchDay());

            List<double> matchdayOdds = new List<double>(thisRoundFixtures.Count);
            foreach (Fixture fixture in thisRoundFixtures)
            {
                int totalMetricsWithData = 0;
                string aggregateResult = string.Empty;
                foreach (MetricInterface metric in metrics)
                {
                    ResultChecker checker = new ResultChecker(metric, fixture, configManager_);
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
                else if (fixture.odds[computedResult] > configManager_.GetMaxOdds() &&
                    fixture.odds[computedResult] < configManager_.GetMinOdds())
                {
                    totalMetricsWithData = 0;
                }

                string padding = new string(' ', 50 - fixture.homeTeamName.Length - fixture.awayTeamName.Length);
                if (totalMetricsWithData == metrics.Count && computedResult != string.Empty)
                {
                    logger_.LogInfo("{0} - {1},{2} result {3}, \t odds {4:0.00} \t aggregate {5} \t date {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, fixture.odds[computedResult], aggregateResult, fixture.date);
                    matchdayOdds.Add(fixture.odds[computedResult]);
                }
                else
                {
                    logger_.LogInfoFail("{0} - {1},{2} result {3}, \t odds {4:0.00} \t aggregate {5} \t date {6} \n", fixture.homeTeamName, fixture.awayTeamName, padding, computedResult, fixture.odds[computedResult], aggregateResult, fixture.date);
                }

            }

            expectedProfit = GetMatchdayProfit(matchdayOdds);
            logger_.LogInfo("Profit: {0}", expectedProfit);
        }

        private readonly List<MetricConfig> metricConfigs_;
        private readonly ConfigManagerInterface configManager_;
        private readonly FixtureRetrieverInterface fixtureRetriever_;
        private readonly Logger logger_;
    }
}