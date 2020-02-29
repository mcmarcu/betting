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
    class ValueStats
    {
        public ValueStats(List<MetricConfig> metricConfigs)
        {
            this.metricConfigs = metricConfigs;
        }

        private void printMaps(SortedDictionary<int, int> diffHome, SortedDictionary<int, int> diffDraw, SortedDictionary<int, int> diffAway, int computeYear)
        {
            Logger.LogInfo("\n Value map on year {0}  -------\n", computeYear);

            SortedSet<int> keys = new SortedSet<int>();
            keys.UnionWith(diffHome.Keys);
            keys.UnionWith(diffDraw.Keys);
            keys.UnionWith(diffAway.Keys);

            foreach (int key in keys)
            {
                int homeCount = diffHome.ContainsKey(key) ? diffHome[key] : 0;
                int drawCount = diffDraw.ContainsKey(key) ? diffDraw[key] : 0;
                int awayCount = diffAway.ContainsKey(key) ? diffAway[key] : 0;

                Logger.LogInfo("v {0} \t 1 {1} \t X {2} \t 2 {3} \n", key, homeCount, drawCount, awayCount);
            }

            Logger.LogInfo("-------------------\n\n", computeYear);
        }

        private void AddToDict(ref SortedDictionary<int, int> dict, int key, int value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key]+= value;
        }

        public void GetAllYearsData()
        {
            int year = ConfigManager.Instance.GetYear();
            int reverseYears = ConfigManager.Instance.GetReverseYears();

            SortedDictionary<int, int> totalDiffHome = new SortedDictionary<int, int>();
            SortedDictionary<int, int> totalDiffDraw = new SortedDictionary<int, int>();
            SortedDictionary<int, int> totalDiffAway = new SortedDictionary<int, int>();


            for (int i = 0; i < reverseYears; ++i)
            {
                SortedDictionary<int, int> diffHome = new SortedDictionary<int, int>();
                SortedDictionary<int, int> diffDraw = new SortedDictionary<int, int>();
                SortedDictionary<int, int> diffAway = new SortedDictionary<int, int>();
                int computeYear = year - i;
                GetYearData(ref diffHome, ref diffDraw, ref diffAway, computeYear);

                foreach(KeyValuePair<int, int> item in diffHome)
                    AddToDict(ref totalDiffHome, item.Key, item.Value);
                foreach (KeyValuePair<int, int> item in diffDraw)
                    AddToDict(ref totalDiffDraw, item.Key, item.Value);
                foreach (KeyValuePair<int, int> item in diffAway)
                    AddToDict(ref totalDiffAway, item.Key, item.Value);

                printMaps(diffHome, diffDraw, diffAway, computeYear);
            };

            printMaps(totalDiffHome, totalDiffDraw, totalDiffAway, 0);
        }

        public void GetYearData(ref SortedDictionary<int, int>  diffHome, ref SortedDictionary<int, int> diffDraw, ref SortedDictionary<int, int> diffAway, int year)
        {
            int matchDay = ConfigManager.Instance.GetMatchDay();
            int reverseDays = ConfigManager.Instance.GetReverseDays();

            for (int i = 0; i < reverseDays; ++i)
            {
                GetMatchdayData(ref diffHome, ref diffDraw, ref diffAway, matchDay, year);
                FixtureRetriever.GetPrevRound(out year, out matchDay, year, matchDay);
            }
        }


        public void GetMatchdayData(ref SortedDictionary<int, int> diffHome, ref SortedDictionary<int, int> diffDraw, ref SortedDictionary<int, int> diffAway, int matchDay, int year)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(metricConfigs, matchDay, year);
            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);

            float ptsHome = 0;
            float ptsAway = 0;

            foreach (Fixture fixture in thisRoundFixtures)
            {
                foreach (MetricInterface metric in metrics)
                {
                    ResultChecker checker = new ResultChecker(metric, fixture);
                    if (checker.dataAvailable)
                    {
                        ptsHome += checker.pct1;
                        ptsAway += checker.pct2;
                    }
                }


                int key = Convert.ToInt32(ptsHome*100/(ptsHome + ptsAway));
                if (fixture.result == "1")
                {
                    AddToDict(ref diffHome, key, 1);
                }
                else if (fixture.result == "X")
                {
                    AddToDict(ref diffDraw, key, 1);
                }
                else
                {
                    AddToDict(ref diffAway, key, 1);
                }
            }
        }


        private List<MetricConfig> metricConfigs;
        

    }
}