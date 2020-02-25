﻿using Betting.Config;
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

namespace Betting
{
    class Program
    {
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

            var leagueOption = app.Option("-l|--league <optionvalue>", "League name (PremierLeague/Championship)", CommandOptionType.SingleValue);
            var yearOption = app.Option("-y|--year <optionvalue>", "Yeasr to start compute (2018)", CommandOptionType.SingleValue);
            var yReverseOption = app.Option("-r|--yreverse <optionvalue>", "Year to go behind(7)", CommandOptionType.SingleValue);
            var matchdayOption = app.Option("-m|--matchday <optionvalue>", "Matchday to start compute (max Championship = 46 max Premier League = 38)", CommandOptionType.SingleValue);
            var mReverseOption = app.Option("-v|--mreverse <optionvalue>", "Matchdays to go behind(Matchday-10)", CommandOptionType.SingleValue);
            var drawMarginOption = app.Option("-d|--drawmargin <optionvalue>", "Percent safety for draw(2)", CommandOptionType.SingleValue);
            var drawMixedMarginOption = app.Option("-i|--drawmixedmargin <optionvalue>", "Percent safety for draw mixed(20)", CommandOptionType.SingleValue);
            var maxOddsOption = app.Option("-o|--maxodds <optionvalue>", "Max odds (2.0)", CommandOptionType.SingleValue);
            var minMetricCorrectOption = app.Option("-c|--minmetriccorrect <optionvalue>", "?? 1", CommandOptionType.SingleValue);
            var minYearProfitOption = app.Option("-p|--minyearprofit <optionvalue>", "Min profit per year (-5)", CommandOptionType.SingleValue);
            var logLevelOption = app.Option("-g|--loglevel <optionvalue>", "LOG_DEBUG, LOG_INFO, LOG_RESULT", CommandOptionType.SingleValue);
            var successRateOption = app.Option("-s|--successrate <optionvalue>", "how much should be correct(75)", CommandOptionType.SingleValue);
            var filterTopRate = app.Option("-f|--filtertoprate <optionvalue>", "how many should we keep in the output sorted by rate(10)", CommandOptionType.SingleValue);
            var filterTopProfit = app.Option("-f|--filtertopprofit <optionvalue>", "how many should we keep in the output sorted by profit(10)", CommandOptionType.SingleValue);
            var betStyleOption = app.Option("-t|--betstyle <optionvalue>", "ticket options (12345)", CommandOptionType.SingleValue);
            


            app.OnExecute(() =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                if (leagueOption.HasValue())
                    ConfigManager.Instance.SetLeagueName(leagueOption.Value());
                if (yearOption.HasValue())
                    ConfigManager.Instance.SetYear(yearOption.Value());
                if (yReverseOption.HasValue())
                    ConfigManager.Instance.SetReverseYears(yReverseOption.Value());
                if (matchdayOption.HasValue())
                    ConfigManager.Instance.SetMatchDay(matchdayOption.Value());
                if (mReverseOption.HasValue())
                    ConfigManager.Instance.SetReverseDays(mReverseOption.Value());
                if (drawMarginOption.HasValue())
                    ConfigManager.Instance.SetDrawMargin(drawMarginOption.Value());
                if (drawMixedMarginOption.HasValue())
                    ConfigManager.Instance.SetDrawMixedMargin(drawMixedMarginOption.Value());
                if (maxOddsOption.HasValue())
                    ConfigManager.Instance.SetMaxOdds(maxOddsOption.Value());
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

                if (predictResults.HasValue())
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
                    SortedDictionary<float, Tuple<bool ,float, float, int>> topByProfit =
                        new SortedDictionary<float, Tuple<bool, float, float, int>>();
                    SortedDictionary<float, Tuple<bool, float, float, int>> topByRate =
                        new SortedDictionary<float, Tuple<bool, float, float, int>>();
                    SortedDictionary<float, Tuple<bool, float, float, int>> successRuns =
                        new SortedDictionary<float, Tuple<bool, float, float, int>>();

                    Parallel.For(0, 10000, (i, state) =>
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
                                successRuns.Add(averageProfit, Tuple.Create(success, rate, averageProfit, i));
                            }

                            Logger.LogResultSuccess("Result {0}, Rate {1}, avgProfit {2}, cfg {3} \n", success, rate, averageProfit, i);
                        }
                        else
                            Logger.LogResultFail("Result {0}, Rate {1}, avgProfit {2}, cfg {3} \n", success, rate, averageProfit, i);

                                                
                        lock (topByProfit)
                        {
                            if (!topByProfit.ContainsKey(averageProfit))
                                topByProfit.Add(averageProfit, Tuple.Create(success, rate, averageProfit, i));
                            if (topByProfit.Count > ConfigManager.Instance.GetFilterTopProfit())
                                topByProfit.Remove(topByProfit.Keys.First());
                        }

                        lock (topByRate)
                        {
                            if (!topByRate.ContainsKey(rate))
                                topByRate.Add(rate, Tuple.Create(success, rate, averageProfit, i));
                            if (topByRate.Count > ConfigManager.Instance.GetFilterTopRate())
                                topByRate.Remove(topByRate.Keys.First());
                        }
                    });

                    if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_RESULT)
                    {
                        Logger.LogResult("TopByProfit {0}: \n\n", ConfigManager.Instance.GetFilterTopProfit());

                        foreach (Tuple<bool, float, float, int> t in topByProfit.Values)
                        {
                            if (t.Item1)
                                Logger.LogResultSuccess("Rate {0}, avgProfit {1}, id {2}: ", t.Item2, t.Item3, t.Item4);
                            else
                                Logger.LogResultFail("Rate {0}, avgProfit {1}, id {2}: ", t.Item2, t.Item3, t.Item4);
                            PrintMetricList(t.Item4);
                            Logger.LogResult("\n ---------------- \n");
                        }

                        Logger.LogResult("TopByRate {0}: \n\n", ConfigManager.Instance.GetFilterTopRate());

                        foreach (Tuple<bool, float, float, int> t in topByRate.Values)
                        {
                            if(t.Item1)
                                Logger.LogResultSuccess("Rate {0}, avgProfit {1}, id {2}: ", t.Item2, t.Item3, t.Item4);
                            else
                                Logger.LogResultFail("Rate {0}, avgProfit {1}, id {2}: ", t.Item2, t.Item3, t.Item4);
                            PrintMetricList(t.Item4);
                            Logger.LogResult("\n ---------------- \n");
                        }

                        Logger.LogResult("SuccessRuns {0}: \n\n", successRuns.Count);

                        foreach (Tuple<bool, float, float, int> t in successRuns.Values)
                        {
                            if (t.Item1)
                                Logger.LogResultSuccess("Rate {0}, avgProfit {1}, id {2}: ", t.Item2, t.Item3, t.Item4);
                            else
                                Logger.LogResultFail("Rate {0}, avgProfit {1}, id {2}: ", t.Item2, t.Item3, t.Item4);
                            PrintMetricList(t.Item4);
                            Logger.LogResult("\n ---------------- \n");
                        }
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
