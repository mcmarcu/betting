using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Betting.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using System.Diagnostics;
using Accord.MachineLearning;

namespace Betting
{
    class Program
    {
        public class RunOutput
        {
            public bool  success;
            public float rate;
            public float averageProfit;
            public int metricId;
            public List<double> metricDepths;
            public int cluster;

            public RunOutput(bool success_, float rate_, float averageProfit_, int i, int maxI)
            {
                success = success_;
                rate = rate_;
                averageProfit = averageProfit_;
                metricDepths = new List<double>();
                metricId = i;
                maxI /= 10;
                while (maxI>0)
                {
                    metricDepths.Add(i % 10);
                    maxI /= 10;
                    i /= 10;
                }
                cluster = 0;
            }
        }

        public static void AddClusterInfo(ref SortedDictionary<float, RunOutput> dict)
        {
            if (dict.Count <= 1)
                return;

            Accord.Math.Random.Generator.Seed = 0;

            double[][] metrics = new double[dict.Count][];
            int i = 0;
            foreach (RunOutput t in dict.Values)
            {
                metrics[i++] = t.metricDepths.ToArray();
            }

            // Create a new K-Means algorithm
            KMeans kmeans = new KMeans(k: dict.Count/3);

            // Compute and retrieve the data centroids
            var clusters = kmeans.Learn(metrics);

            // Use the centroids to parition all the data
            int[] labels = clusters.Decide(metrics);

            int j = 0;
            foreach (RunOutput v in dict.Values)
            {
                v.cluster = labels[j++];
            }
        }

        public static void PrintClusterInfo(SortedDictionary<float, RunOutput> dict)
        {
            if (ConfigManager.Instance.GetLogLevel() > ConfigManager.LogLevel.LOG_RESULT)
                return;

            SortedDictionary<int, SortedSet<int>> clusteredOutput = new SortedDictionary<int, SortedSet<int>>();

            foreach(RunOutput t in dict.Values)
            {
                if (!clusteredOutput.ContainsKey(t.cluster))
                    clusteredOutput.Add(t.cluster, new SortedSet<int>());
           
                clusteredOutput[t.cluster].Add(t.metricId);
            }

            foreach(int k in clusteredOutput.Keys)
            {
                Logger.LogResult("config {0}", k);
                foreach(int t in clusteredOutput[k])
                    Logger.LogResult(", {0}", t);
                Logger.LogResult("\n");
            }

        }

        public static void PrintMetricList(int i)
        {
            int LastGamesMetricD = i % 10;
            int GoalsScoredMetricD = (i / 10) % 10;
            int GoalsConcededMetricD = (i / 100) % 10;
            int HomeAdvantageMetricD = (i / 1000) % 10;

            if (LastGamesMetricD != 0)
            {
                Logger.LogResult(" LastGames ({0}); ", LastGamesMetricD);
            }

            if (GoalsScoredMetricD != 0)
            {
                Logger.LogResult(" GoalsScored ({0}); ", GoalsScoredMetricD);
            }

            if (GoalsConcededMetricD != 0)
            {
                Logger.LogResult(" GoalsConceded ({0}); ", GoalsConcededMetricD);
            }
            if (HomeAdvantageMetricD != 0)
            {
                Logger.LogResult(" HomeAdvantageMetric ({0}); ", HomeAdvantageMetricD);
            }

            if (i / 10000 > 0)
            {
                PrintMetricList(i / 10000);
            }

        }

        public static List<MetricConfig> GetMetricList(int i)
        {
            List<MetricConfig> metricConfigs = new List<MetricConfig>();

            int LastGamesMetricD = i % 10;
            int GoalsScoredMetricD = (i / 10) % 10;
            int GoalsConcededMetricD = (i / 100) % 10;
            int HomeAdvantageMetricD = (i / 1000) % 10;

            if (LastGamesMetricD != 0)
            { 
                MetricConfig lastGamesMetric = new MetricConfig
                {
                    name = "LastGamesMetric",
                    depth = LastGamesMetricD
                };
                metricConfigs.Add(lastGamesMetric);
            }

            if (GoalsScoredMetricD != 0)
            {
                MetricConfig goalsScoredMetric = new MetricConfig
                {
                    name = "GoalsScoredMetric",
                    depth = GoalsScoredMetricD
                };
                metricConfigs.Add(goalsScoredMetric);
            }

            if (GoalsConcededMetricD != 0)
            {
                MetricConfig goalsConcededMetric = new MetricConfig
                {
                    name = "GoalsConcededMetric",
                    depth = GoalsConcededMetricD
                };
                metricConfigs.Add(goalsConcededMetric);
            }

            if (HomeAdvantageMetricD != 0)
            {
                MetricConfig homeAdvantageMetric = new MetricConfig
                {
                    name = "HomeAdvantageMetric",
                    depth = HomeAdvantageMetricD
                };
                metricConfigs.Add(homeAdvantageMetric);
            }

            if (i / 10000 > 0)
            {
                metricConfigs.AddRange(GetMetricList(i / 10000));
            }

            return metricConfigs;
        }

        static void Main(string[] args)
        {

            var app = new CommandLineApplication();
            app.Name = "Betting";
            app.Description = "Betting console app with argument parsing.";

            app.HelpOption("-?|-h|--help");

            var executeMetrics = app.Option("-e|--evaluateMetrics", "Evaluate all metrics", CommandOptionType.NoValue);
            var inspectMetric = app.Option("-x|--inspectMeric <optionvalue>", "get data about a metric", CommandOptionType.SingleValue);
            var predictResults = app.Option("-w|--predictResults", "predict results", CommandOptionType.NoValue);
            var valueStats = app.Option("-z|--valueStats <optionvalue>", "get value stats for metric", CommandOptionType.SingleValue);
            var dbUpdate = app.Option("-u|--dbUpdate", "Create enhanced csv files", CommandOptionType.NoValue);

            var leagueOption = app.Option("-l|--league <optionvalue>", "League name (PremierLeague/Championship)", CommandOptionType.SingleValue);
            var yearOption = app.Option("-y|--year <optionvalue>", "Yeasr to start compute (2018)", CommandOptionType.SingleValue);
            var yReverseOption = app.Option("-r|--yreverse <optionvalue>", "Year to go behind(7)", CommandOptionType.SingleValue);
            var matchdayOption = app.Option("-m|--matchday <optionvalue>", "Matchday to start compute (max Championship = 46 max Premier League = 38)", CommandOptionType.SingleValue);
            var mReverseOption = app.Option("-v|--mreverse <optionvalue>", "Matchdays to go behind(Matchday-10)", CommandOptionType.SingleValue);
            var drawMarginOption = app.Option("-d|--drawmargin <optionvalue>", "Percent safety for draw(2)", CommandOptionType.SingleValue);
            var drawMixedMarginOption = app.Option("-i|--drawmixedmargin <optionvalue>", "Percent safety for draw mixed(20)", CommandOptionType.SingleValue);
            var maxOddsOption = app.Option("-o|--maxodds <optionvalue>", "Max odds (2.0)", CommandOptionType.SingleValue);
            var minOddsOption = app.Option("-O|--minodds <optionvalue>", "Min odds (2.0)", CommandOptionType.SingleValue);
            var minMetricCorrectOption = app.Option("-c|--minmetriccorrect <optionvalue>", "?? 1", CommandOptionType.SingleValue);
            var minYearProfitOption = app.Option("-p|--minyearprofit <optionvalue>", "Min profit per year (-5)", CommandOptionType.SingleValue);
            var logLevelOption = app.Option("-g|--loglevel <optionvalue>", "LOG_DEBUG, LOG_INFO, LOG_RESULT", CommandOptionType.SingleValue);
            var successRateOption = app.Option("-s|--successrate <optionvalue>", "how much should be correct(75)", CommandOptionType.SingleValue);
            var filterTopRate = app.Option("-f|--filtertoprate <optionvalue>", "how many should we keep in the output sorted by rate(10)", CommandOptionType.SingleValue);
            var filterTopProfit = app.Option("-f|--filtertopprofit <optionvalue>", "how many should we keep in the output sorted by profit(10)", CommandOptionType.SingleValue);
            var betStyleOption = app.Option("-t|--betstyle <optionvalue>", "ticket options (12345)", CommandOptionType.SingleValue);
            var useExpanded = app.Option("-X|--useExpanded", "Use expanded csv data", CommandOptionType.NoValue);


            app.OnExecute(() =>
            {
                //ClusterPrint();

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                if (leagueOption.HasValue())
                    ConfigManager.Instance.SetLeagueName(leagueOption.Value());
                if (yearOption.HasValue())
                    ConfigManager.Instance.SetYear(yearOption.Value());
                if (yReverseOption.HasValue())
                    ConfigManager.Instance.SetReverseYears(yReverseOption.Value());
                if (matchdayOption.HasValue())
                {
                    if(matchdayOption.Value() == "max")
                        ConfigManager.Instance.SetMatchDay(FixtureRetriever.GetNumberOfMatchDays(ConfigManager.Instance.GetYear()).ToString());
                    else
                        ConfigManager.Instance.SetMatchDay(matchdayOption.Value());
                }
                if (mReverseOption.HasValue())
                    ConfigManager.Instance.SetReverseDays(mReverseOption.Value());
                if (drawMarginOption.HasValue())
                    ConfigManager.Instance.SetDrawMargin(drawMarginOption.Value());
                if (drawMixedMarginOption.HasValue())
                    ConfigManager.Instance.SetDrawMixedMargin(drawMixedMarginOption.Value());
                if (maxOddsOption.HasValue())
                    ConfigManager.Instance.SetMaxOdds(maxOddsOption.Value());
                if (minOddsOption.HasValue())
                    ConfigManager.Instance.SetMinOdds(minOddsOption.Value());
                if (minMetricCorrectOption.HasValue())
                    ConfigManager.Instance.SetMinMetricCorrect(minMetricCorrectOption.Value());
                if (minYearProfitOption.HasValue())
                    ConfigManager.Instance.SetMinYearProfit(minYearProfitOption.Value());
                if (logLevelOption.HasValue())
                    ConfigManager.Instance.SetLogLevel(logLevelOption.Value());
                if (successRateOption.HasValue())
                    ConfigManager.Instance.SetSuccessRate(successRateOption.Value());
                if (filterTopRate.HasValue())
                    ConfigManager.Instance.SetSuccessRate(filterTopRate.Value());
                if (filterTopProfit.HasValue())
                    ConfigManager.Instance.SetSuccessRate(filterTopProfit.Value());
                if (betStyleOption.HasValue())
                    ConfigManager.Instance.SetBetStyle(betStyleOption.Value());
                if (useExpanded.HasValue())
                    ConfigManager.Instance.SetUseExpanded(true);


                if (dbUpdate.HasValue())
                {
                    DBUpdater.AddPoints();
                }
                else if (valueStats.HasValue())
                {
                    int metricConfigId = Int32.Parse(valueStats.Value());
                    List<MetricConfig> metricConfigs = GetMetricList(metricConfigId);
                    PrintMetricList(metricConfigId);
                    Logger.LogResultSuccess("\n Value Stats: \n");
                    ValueStats vs = new ValueStats(metricConfigs);
                    vs.GetAllYearsData();
                }
                else if (predictResults.HasValue())
                {
                    int metricConfigId = Int32.Parse(inspectMetric.Value());
                    List<MetricConfig> metricConfigs = GetMetricList(metricConfigId);
                    PrintMetricList(metricConfigId);
                    Logger.LogResultSuccess("\n Results: \n");
                    GlobalStats gs = new GlobalStats(metricConfigs);
                    gs.ProcessUpcomingFixtures();
                }
                else if (inspectMetric.HasValue())
                {
                    int metricConfigId = Int32.Parse(inspectMetric.Value());
                    List<MetricConfig> metricConfigs = GetMetricList(metricConfigId);
                    PrintMetricList(metricConfigId);

                    GlobalStats gs = new GlobalStats(metricConfigs);
                    gs.GetAllYearsData(out bool success, out float rate, out float averageProfit);

                    if(success)
                        Logger.LogResultSuccess("Result {0}, Rate {1}, avgProfit {2} \n", success, rate, averageProfit);
                    else
                        Logger.LogResultFail("Result {0}, Rate {1}, avgProfit {2} \n", success, rate, averageProfit);
                }
                else if (executeMetrics.HasValue())
                {   
                    SortedDictionary<float, RunOutput> topByProfit =
                        new SortedDictionary<float, RunOutput>();
                    SortedDictionary<float, RunOutput> topByRate =
                        new SortedDictionary<float, RunOutput>();
                    SortedDictionary<float, RunOutput> successRuns =
                        new SortedDictionary<float, RunOutput>();

                    int numMetrics = 4;
                    int maxI = Convert.ToInt32(Math.Pow(10, numMetrics));
                    Parallel.For(0, maxI, (i, state) =>
                    {
                        List<MetricConfig> metricConfigs = GetMetricList(i);

                        if (metricConfigs.Count == 0)
                            return;

                        if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_INFO)
                            PrintMetricList(i);

                        GlobalStats gs = new GlobalStats(metricConfigs);
                        gs.GetAllYearsData(out bool success, out float rate, out float averageProfit);

                        if (success)
                        {
                            lock(successRuns)
                            {
                                try
                                {
                                    successRuns.Add(averageProfit, new RunOutput(success, rate, averageProfit, i, maxI));
                                }
                                catch(Exception)
                                {

                                }
                            }

                            Logger.LogResultSuccess("Result {0}, Rate {1}, avgProfit {2}, cfg {3} \n", success, rate, averageProfit, i);
                        }
                        else
                            Logger.LogResultFail("Result {0}, Rate {1}, avgProfit {2}, cfg {3} \n", success, rate, averageProfit, i);

                                                
                        lock (topByProfit)
                        {
                            if (!topByProfit.ContainsKey(averageProfit))
                                topByProfit.Add(averageProfit, new RunOutput(success, rate, averageProfit, i, maxI));
                            if (topByProfit.Count > ConfigManager.Instance.GetFilterTopProfit())
                                topByProfit.Remove(topByProfit.Keys.First());
                        }

                        lock (topByRate)
                        {
                            if (!topByRate.ContainsKey(rate))
                                topByRate.Add(rate, new RunOutput(success, rate, averageProfit, i, maxI));
                            if (topByRate.Count > ConfigManager.Instance.GetFilterTopRate())
                                topByRate.Remove(topByRate.Keys.First());
                        }
                    });

                    if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_RESULT)
                    {
                        Logger.LogResult("TopByProfit {0}: \n\n", ConfigManager.Instance.GetFilterTopProfit());

                        AddClusterInfo(ref topByProfit);
                        foreach (RunOutput t in topByProfit.Values)
                        {
                            if (t.success)
                                Logger.LogResultSuccess("Rate {0}, avgProfit {1}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            else
                                Logger.LogResultFail("Rate {0}, avgProfit {1}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            PrintMetricList(t.metricId);
                            Logger.LogResult("\n ---------------- \n");
                        }

                        Logger.LogResult("TopByRate {0}: \n\n", ConfigManager.Instance.GetFilterTopRate());

                        AddClusterInfo(ref topByRate);
                        foreach (RunOutput t in topByRate.Values)
                        {
                            if(t.success)
                                Logger.LogResultSuccess("Rate {0}, avgProfit {1}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            else
                                Logger.LogResultFail("Rate {0}, avgProfit {1}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            PrintMetricList(t.metricId);
                            Logger.LogResult("\n ---------------- \n");
                        }

                        Logger.LogResult("SuccessRuns {0}: \n\n", successRuns.Count);

                        AddClusterInfo(ref successRuns);
                        foreach (RunOutput t in successRuns.Values)
                        {
                            if (t.success)
                                Logger.LogResultSuccess("Rate {0}, avgProfit {1}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            else
                                Logger.LogResultFail("Rate {0}, avgProfit {1}, id {2}, cl {3}:", t.rate, t.averageProfit, t.metricId, t.cluster);
                            PrintMetricList(t.metricId);
                            Logger.LogResult("\n ---------------- \n");
                        }
                        PrintClusterInfo(successRuns);
                    }
                }

                stopWatch.Stop();
                Logger.LogResult("\nTime spent: {0}", stopWatch.ElapsedMilliseconds);
                Console.ReadLine();

                return 0;
            });

            app.Execute(args);
            
        }

        
    }
}
