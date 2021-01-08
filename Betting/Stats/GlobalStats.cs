using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


            rate = totalGames == 0 ? 0 : ((double)correctGames / (double)totalGames) * 100;
            averageProfit = profitSum / reverseYears;
            success = (successYears == reverseYears) && (averageProfit >= configManager_.GetMinAverageProfit());
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

                double rate = totalFixtures == 0 ? 0 : ((double)correctFixtures / (double)totalFixtures) * 100;
                logger_.LogDebug("Matchday : {0} - year {1}, correct {2}\t total {3}, rate {4}, profit {5:0.00} \n", matchDay, year, correctFixtures, totalFixtures, rate, matchdayProfit);

                correctFixturesWithData += correctFixtures;
                totalFixturesWithData += totalFixtures;
                currentProfit += matchdayProfit;
                fixtureRetriever_.GetPrevRound(ref year, ref matchDay);
            }
        }

        private int GetNumOnesInInteger(int v)
        {
            // https://stackoverflow.com/questions/14555607/number-of-bits-set-in-a-number
            v -= ((v >> 1) & 0x55555555);
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);
            return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
        }

        public double GetCombinationProfit(List<double> matchdayOdds, int betStyleMask)
        {
            double count = 1 << matchdayOdds.Count;
            double profit = 0;
            for (int i = 1; i <= count; ++i)
            {
                //if betStyleMask=2 (we count double odds only) and we matchdayOdds.Count=3
                //we only take the masks which have 2 ones set
                if ((betStyleMask & (1 << GetNumOnesInInteger(i))) == 0)
                {
                    continue;
                }

                double cProfit = 1;
                for (int j = 0; cProfit != 0 && j < matchdayOdds.Count; ++j)
                {
                    if (((i >> j) & 1) == 1)
                    {
                        cProfit *= matchdayOdds[j];
                    }
                }

                if (cProfit != 0)
                {
                    profit += cProfit - 1;
                }
                else
                {
                    profit -= 1;
                }
            }
            return profit;
        }

        private int CountCharsInString(string str, char c)
        {
            int count = 0;
            int length = str.Length;
            for (int i = length - 1; i >= 0; --i)
            {
                if (str[i] == c)
                    count++;
            }
            return count;
        }

        public string ComputeExpectedResult(string aggregateResult, int totalMetricsWithData)
        {
            string computedResult = string.Empty;
            int goodMetricsCount = (int)(configManager_.GetMinMetricCorrect() * (double)totalMetricsWithData);
            int count1 = CountCharsInString(aggregateResult, '1');
            int count2 = CountCharsInString(aggregateResult, '2');
            int countX = CountCharsInString(aggregateResult, 'X');

            if (count1 >= goodMetricsCount)
            {
                computedResult += "1";
            }
            if (countX >= goodMetricsCount)
            {
                computedResult += "X";
            }
            if (count2 >= goodMetricsCount)
            {
                computedResult += "2";
            }
            

            if (computedResult == "1" && countX > 0)
            {
                computedResult = "1X";
            }
            else if (computedResult == "2" && countX > 0)
            {
                computedResult = "X2";
            }
            /*else if (computedResult == "X")
            {
                if (count1 >= (goodMetricsCount + 1) / 2 && count2 == 0)
                    computedResult = "1X";
                else if (count2 >= (goodMetricsCount + 1) / 2 && count1 == 0)
                    computedResult = "X2";
                else
                    computedResult = "";
            }*/
            else if (computedResult == "X" || computedResult == "")
            {
                if (count1 > 0 && count2 == 0)
                    computedResult = "1X";
                else if (count2 >= 0 && count1 == 0)
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

            if (betStyle=="all")
            {
                betStyleMask = ~0;
            }
            else if (betStyle=="max")
            {
                betStyleMask |= 1 << maxGames;
            }
            else
            {
                for (int i = 0; i < betStyle.Count(); ++i)
                {
                    betStyleMask |= 1 << (betStyle[i] - '0');
                }
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
                StringBuilder aggregatExpectedResultsBuilder = new StringBuilder();
                string actualResult = string.Empty;
                foreach (MetricInterface metric in metrics)
                {
                    ResultChecker checker = new ResultChecker(metric, fixture, configManager_);
                    if (checker.dataAvailable)
                    {
                        aggregatExpectedResultsBuilder.Append(checker.GetExpectedResult());
                        aggregatExpectedResultsBuilder.Append(" ");

                        totalMetricsWithData++;
                    }
                    actualResult = fixture.result;
                }
                string aggregateResult = aggregatExpectedResultsBuilder.ToString();

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