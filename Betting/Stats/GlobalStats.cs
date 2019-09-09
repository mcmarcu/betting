using Betting.Config;
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
                float yearRate = ((float)yearCorrect / (float)yearTotal) * 100;
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
            int reverseYears = ConfigManager.Instance.GetReverseYears();
            int totalFixtures = 0;
            int correctFixtures = 0;
            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            currentProfit = 0;
            float matchdayProfit = 0;

            for (int i = 0; i < reverseDays; ++i)
            {
                GetMatchdayData(out correctFixtures, out totalFixtures, out matchdayProfit, matchDay, year);

                Logger.LogDebug("Matchday : {0} - year {1}, correct {2}\t total {3}, rate {4}, profit {5:0.00} \n", matchDay, year, correctFixtures, totalFixtures, ((float)correctFixtures / (float)totalFixtures) * 100, matchdayProfit);

                correctFixturesWithData += correctFixtures;
                totalFixturesWithData += totalFixtures;
                currentProfit += matchdayProfit;
                FixtureRetriever.GetPrevRound(out year, out matchDay, year, matchDay);
            }
        }


        public float GetMatchdayProfit(List<float> matchdayOdds)
        {
            float profit = 0;

            string betStyle = ConfigManager.Instance.GetBetStyle();

            //one by one
            if(betStyle.Contains('1') && matchdayOdds.Count >= 1)
            { 
                foreach (float odd in matchdayOdds)
                {
                    if (odd > 0)
                        profit += odd - 1;
                    else
                        profit -= 1;
                }
            }

            //two by twos
            if (betStyle.Contains('2') && matchdayOdds.Count >=2)
            {
                for(int i=0;i<matchdayOdds.Count-1;++i)
                    for(int j=i+1;j<matchdayOdds.Count;++j)
                    {
                        float newOdd = matchdayOdds[i] * matchdayOdds[j];
                        if (newOdd > 0)
                            profit += newOdd - 1;
                        else
                            profit -= 1;
                    }
            }

            //three by threes
            if (betStyle.Contains('3') && matchdayOdds.Count >= 3)
            {
                for (int i = 0; i < matchdayOdds.Count - 2; ++i)
                    for (int j = i + 1; j < matchdayOdds.Count - 1; ++j)
                        for (int k = j + 1; k < matchdayOdds.Count; ++k)
                        {
                            float newOdd = matchdayOdds[i] * matchdayOdds[j] * matchdayOdds[k];
                            if (newOdd > 0)
                                profit += newOdd- 1;
                            else
                                profit -= 1;
                        }
            }

            //four by fours
            if (betStyle.Contains('4') && matchdayOdds.Count >= 4)
            {
                for (int i = 0; i < matchdayOdds.Count - 3; ++i)
                    for (int j = i + 1; j < matchdayOdds.Count - 2; ++j)
                        for (int k = j + 1; k < matchdayOdds.Count -1; ++k)
                            for (int l = k + 1; l < matchdayOdds.Count; ++l)
                            {
                                float newOdd = matchdayOdds[i] * matchdayOdds[j] * matchdayOdds[k] * matchdayOdds[l];
                                if (newOdd > 0)
                                    profit += newOdd - 1;
                                else
                                    profit -= 1;
                            }
            }

            //five by fives
            if (betStyle.Contains('5') && matchdayOdds.Count >= 5)
            {
                for (int i = 0; i < matchdayOdds.Count - 4; ++i)
                    for (int j = i + 1; j < matchdayOdds.Count - 3; ++j)
                        for (int k = j + 1; k < matchdayOdds.Count - 2; ++k)
                            for (int l = k + 1; l < matchdayOdds.Count -1; ++l)
                                for (int m = l + 1; m < matchdayOdds.Count ; ++m)
                                {
                                    float newOdd = matchdayOdds[i] * matchdayOdds[j] * matchdayOdds[k] * matchdayOdds[l] * matchdayOdds[m];
                                    if (newOdd > 0)
                                        profit += newOdd - 1;
                                    else
                                        profit -= 1;
                                }
            }
            return profit;
        }

        public void GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out float currentProfit, int matchDay, int year)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(metricConfigs, matchDay, year);

            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;
            List<float> matchdayOdds = new List<float>();

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

                //TODO: what does GetMinMetricCorrect do?
                int goodMetricsCount = (int)(ConfigManager.Instance.GetMinMetricCorrect() * (float)totalMetricsWithData); 
                string possibleResults = "1X2";
                foreach(char result in possibleResults)
                {
                    if (aggregateResult.Split(result).Length - 1 >= goodMetricsCount)
                    {
                        computedResult += result;
                    }
                }

                //TODO: put under config
                if(computedResult == "1" && aggregateResult.Split('X').Length - 1 > 0)
                {
                    computedResult = "1X";
                }
                if (computedResult == "2" && aggregateResult.Split('X').Length - 1 > 0)
                {
                    computedResult = "X2";
                }

                if (computedResult == "X")
                {
                    computedResult = "1X2";
                    totalMetricsWithData = 0;
                }
                //END X related hacks

                //bad expected 1X2
                if (computedResult.Length == 3 || computedResult.Length == 0)
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

            currentProfit = GetMatchdayProfit(matchdayOdds);
        }

        private List<MetricConfig> metricConfigs;
    }
}