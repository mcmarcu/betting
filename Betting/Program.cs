using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Betting.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting
{
    class Program
    {
        
        public static List<MetricConfig> GetMetricList(int i)
        {
            List<MetricConfig> metricConfigs = new List<MetricConfig>();

            MetricConfig lastGamesMetric1 = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = i % 10
            };
            metricConfigs.Add(lastGamesMetric1);

            MetricConfig lastGamesMetric2 = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = (i / 10) % 10
            };
            metricConfigs.Add(lastGamesMetric2);

            MetricConfig goalsScoredMetric1 = new MetricConfig
            {
                name = "GoalsScoredMetric",
                depth = (i / 100) % 10
            };
            metricConfigs.Add(goalsScoredMetric1);

            MetricConfig goalsScoredMetric2 = new MetricConfig
            {
                name = "GoalsConcededMetric",
                depth = (i / 1000) % 10
            };
            metricConfigs.Add(goalsScoredMetric2);

            return metricConfigs;
        }

        static void Main(string[] args)
        {
            int[] configs = { 1158 };
            //for (int i = 1111; i<=9999; ++i)
            foreach(int i in configs)
            {
                if (i.ToString().Contains("0"))
                    continue;

                ConfigManager.Instance.SetMetricConfigs(GetMetricList(i));

                GlobalStats.GetAllYearsData(out bool success, out float averageProfit);

                if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_RESULT)
                {
                    Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
                    if (success)
                    {
                        Console.Write("Result {0}, avgProfit {1} idx {2} \n", success, averageProfit, i);
                        Console.ReadLine();
                    }
                    Console.ResetColor();
                    //Console.ReadLine();
                }

            }

            Console.Write("DONE");
            Console.ReadLine();
        }
    }
}
