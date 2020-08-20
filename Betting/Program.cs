
using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Betting.Stats;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Betting
{
    class Program
    {
        static void Main(string[] args)
        {

            var app = new CommandLineApplication
            {
                Name = "Betting",
                Description = "Betting console app with argument parsing."
            };

            app.HelpOption("-?|-h|--help");

            var executeMetrics = app.Option("-e|--evaluateMetrics", "Evaluate all metrics", CommandOptionType.NoValue);
            var inspectMetric = app.Option("-x|--inspectMeric <optionvalue>", "get data about a metric", CommandOptionType.SingleValue);
            var predictResults = app.Option("-w|--predictResults", "predict results", CommandOptionType.NoValue);
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
            var minYearProfitOption = app.Option("-p|--minyearprofit <optionvalue>", "Min profit per year (0)", CommandOptionType.SingleValue);
            var minAverageProfitOption = app.Option("-P|--minaverageprofit <optionvalue>", "Min average for all years (0)", CommandOptionType.SingleValue);
            var logLevelOption = app.Option("-g|--loglevel <optionvalue>", "LOG_DEBUG, LOG_INFO, LOG_RESULT", CommandOptionType.SingleValue);
            var filterTopRate = app.Option("-f|--filtertoprate <optionvalue>", "how many should we keep in the output sorted by rate(10)", CommandOptionType.SingleValue);
            var filterTopProfit = app.Option("-F|--filtertopprofit <optionvalue>", "how many should we keep in the output sorted by profit(10)", CommandOptionType.SingleValue);
            var betStyleOption = app.Option("-t|--betstyle <optionvalue>", "ticket options (12345)", CommandOptionType.SingleValue);
            var useExpanded = app.Option("-X|--useExpanded", "Use expanded csv data", CommandOptionType.SingleValue);


            app.OnExecute(() =>
            {
                ConfigManager configManager = new ConfigManager();
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                if (leagueOption.HasValue())
                    configManager.SetLeagueName(leagueOption.Value());
                if (yearOption.HasValue())
                    configManager.SetYear(int.Parse(yearOption.Value()));
                if (yReverseOption.HasValue())
                    configManager.SetReverseYears(int.Parse(yReverseOption.Value()));
                if (mReverseOption.HasValue())
                    configManager.SetReverseDays(int.Parse(mReverseOption.Value()));
                if (drawMarginOption.HasValue())
                    configManager.SetDrawMargin(int.Parse(drawMarginOption.Value()));
                if (drawMixedMarginOption.HasValue())
                    configManager.SetDrawMixedMargin(int.Parse(drawMixedMarginOption.Value()));
                if (maxOddsOption.HasValue())
                    configManager.SetMaxOdds(double.Parse(maxOddsOption.Value()));
                if (minOddsOption.HasValue())
                    configManager.SetMinOdds(double.Parse(minOddsOption.Value()));
                if (minMetricCorrectOption.HasValue())
                    configManager.SetMinMetricCorrect(double.Parse(minMetricCorrectOption.Value()));
                if (minYearProfitOption.HasValue())
                    configManager.SetMinYearProfit(double.Parse(minYearProfitOption.Value()));
                if (minAverageProfitOption.HasValue())
                    configManager.SetMinAverageProfit(double.Parse(minAverageProfitOption.Value()));
                if (logLevelOption.HasValue())
                    configManager.SetLogLevel(logLevelOption.Value());
                if (filterTopRate.HasValue())
                    configManager.SetFilterTopRate(int.Parse(filterTopRate.Value()));
                if (filterTopProfit.HasValue())
                    configManager.SetFilterTopProfit(int.Parse(filterTopProfit.Value()));
                if (betStyleOption.HasValue())
                    configManager.SetBetStyle(betStyleOption.Value());
                if (useExpanded.HasValue())
                {
                    configManager.SetUseExpanded(true);
                    configManager.SetCoeficientWeight(int.Parse(useExpanded.Value()));
                }

                FixtureRetriever fixtureRetriever = new FixtureRetriever(configManager);
                Logger logger = new Logger(configManager.GetLogLevel());

                if (matchdayOption.HasValue())
                {
                    if (matchdayOption.Value().Contains("max"))
                    {
                        string expression = matchdayOption.Value().Replace("max", fixtureRetriever.GetNumberOfMatchDays(configManager.GetYear()).ToString());
                        DataTable dt = new DataTable();
                        configManager.SetMatchDay(int.Parse(dt.Compute(expression, "").ToString()));
                    }
                    else
                        configManager.SetMatchDay(int.Parse(matchdayOption.Value()));
                }


                if (dbUpdate.HasValue())
                {
                    if (executeMetrics.HasValue())
                    {
                        SortedDictionary<double, KeyValuePair<char, int>> sortedAvg =
                        new SortedDictionary<double, KeyValuePair<char, int>>();

                        int numMetrics = 4;
                        int maxI = Convert.ToInt32(Math.Pow(10, numMetrics));
                        for (int i = 0; i < maxI; ++i)
                        {
                            List<MetricConfig> metricConfigs = MetricFactory.GetMetricList(i);

                            if (metricConfigs.Count == 0)
                                continue;

                            if (configManager.GetLogLevel() <= ConfigManager.LogLevel.LOG_INFO)
                                MetricFactory.PrintMetricList(logger, i);

                            DBUpdater db = new DBUpdater(metricConfigs, configManager, fixtureRetriever);
                            db.AddPoints(false);
                            logger.LogResult("\n R2 values  1 {0:0.00}, X {1:0.00}, 2 {2:0.00} metric {3} \n", db.r2Values_['1'], db.r2Values_['X'], db.r2Values_['2'], i);

                            foreach (KeyValuePair<char, double> kv in db.r2Values_)
                            {
                                if (kv.Key != '1')
                                    continue;
                                if (!sortedAvg.ContainsKey(kv.Value))
                                    sortedAvg.Add(kv.Value, new KeyValuePair<char, int>(kv.Key, i));
                                if (sortedAvg.Count > configManager.GetFilterTopProfit())
                                    sortedAvg.Remove(sortedAvg.Keys.First());
                            }
                        }

                        foreach (var x in sortedAvg)
                        {
                            logger.LogResult("\nR2 metric {0:0.00}, value {1:0.00}\n", x.Value.Value, x.Key);
                        }

                    }
                    else
                    {
                        int metricConfigId = int.Parse(inspectMetric.Value());
                        List<MetricConfig> metricConfigs = MetricFactory.GetMetricList(metricConfigId);
                        MetricFactory.PrintMetricList(logger, metricConfigId);
                        DBUpdater db = new DBUpdater(metricConfigs, configManager, fixtureRetriever);
                        db.AddPoints(true);
                        logger.LogResult("\n R2 values  1 {0:0.00}, X {1:0.00}, 2 {2:0.00} \n", db.r2Values_['1'], db.r2Values_['X'], db.r2Values_['2']);
                    }
                }
                else if (predictResults.HasValue())
                {
                    int metricConfigId = int.Parse(inspectMetric.Value());
                    List<MetricConfig> metricConfigs = MetricFactory.GetMetricList(metricConfigId);
                    MetricFactory.PrintMetricList(logger, metricConfigId);
                    logger.LogResultSuccess("\n Results: \n");
                    GlobalStats gs = new GlobalStats(metricConfigs, configManager, fixtureRetriever, logger);
                    gs.ProcessUpcomingFixtures(out double profit);
                }
                else if (inspectMetric.HasValue())
                {
                    int metricConfigId = int.Parse(inspectMetric.Value());
                    List<MetricConfig> metricConfigs = MetricFactory.GetMetricList(metricConfigId);
                    MetricFactory.PrintMetricList(logger, metricConfigId);

                    GlobalStats gs = new GlobalStats(metricConfigs, configManager, fixtureRetriever, logger);
                    gs.GetAllYearsData(out bool success, out double rate, out double averageProfit);

                    if (success)
                        logger.LogResultSuccess("Result {0}, Rate {1:0.00}, avgProfit {2:0.00} \n", success, rate, averageProfit);
                    else
                        logger.LogResultFail("Result {0}, Rate {1:0.00}, avgProfit {2:0.00} \n", success, rate, averageProfit);
                }
                else if (executeMetrics.HasValue())
                {
                    SortedDictionary<double, RunOutput> topByProfit =
                        new SortedDictionary<double, RunOutput>();
                    SortedDictionary<double, RunOutput> topByRate =
                        new SortedDictionary<double, RunOutput>();
                    SortedDictionary<double, RunOutput> successRuns =
                        new SortedDictionary<double, RunOutput>();

                    int numMetrics = 5;
                    int maxI = Convert.ToInt32(Math.Pow(10, numMetrics));
                    Parallel.For(0, maxI, (i, state) =>
                    {
                        List<MetricConfig> metricConfigs = MetricFactory.GetMetricList(i);

                        if (metricConfigs.Count == 0)
                            return;

                        if (configManager.GetLogLevel() <= ConfigManager.LogLevel.LOG_INFO)
                            MetricFactory.PrintMetricList(logger, i);

                        GlobalStats gs = new GlobalStats(metricConfigs, configManager, fixtureRetriever, logger);
                        gs.GetAllYearsData(out bool success, out double rate, out double averageProfit);

                        if (success)
                        {
                            lock (successRuns)
                            {
                                try
                                {
                                    successRuns.Add(averageProfit, new RunOutput(success, rate, averageProfit, i, maxI));
                                }
                                catch (Exception)
                                {

                                }
                            }

                            logger.LogResultSuccess("Result {0}, Rate {1:0.00}, avgProfit {2:0.00}, cfg {3} \n", success, rate, averageProfit, i);
                        }
                        else
                            logger.LogResultFail("Result {0}, Rate {1:0.00}, avgProfit {2:0.00}, cfg {3} \n", success, rate, averageProfit, i);


                        lock (topByProfit)
                        {
                            if (!topByProfit.ContainsKey(averageProfit))
                                topByProfit.Add(averageProfit, new RunOutput(success, rate, averageProfit, i, maxI));
                            if (topByProfit.Count > configManager.GetFilterTopProfit())
                                topByProfit.Remove(topByProfit.Keys.First());
                        }

                        lock (topByRate)
                        {
                            if (!topByRate.ContainsKey(rate))
                                topByRate.Add(rate, new RunOutput(success, rate, averageProfit, i, maxI));
                            if (topByRate.Count > configManager.GetFilterTopRate())
                                topByRate.Remove(topByRate.Keys.First());
                        }
                    });

                    if (configManager.GetLogLevel() <= ConfigManager.LogLevel.LOG_RESULT)
                    {
                        logger.LogResult("TopByProfit {0}: \n\n", configManager.GetFilterTopProfit());

                        OutputFormatter.AddClusterInfo(ref topByProfit);
                        foreach (RunOutput t in topByProfit.Values)
                        {
                            if (t.success)
                                logger.LogResultSuccess("Rate {0:0.00}, avgProfit {1:0.00}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            else
                                logger.LogResultFail("Rate {0:0.00}, avgProfit {1:0.00}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            MetricFactory.PrintMetricList(logger, t.metricId);
                            logger.LogResult("\n ---------------- \n");
                        }

                        logger.LogResult("TopByRate {0}: \n\n", configManager.GetFilterTopRate());

                        OutputFormatter.AddClusterInfo(ref topByRate);
                        foreach (RunOutput t in topByRate.Values)
                        {
                            if (t.success)
                                logger.LogResultSuccess("Rate {0:0.00}, avgProfit {1:0.00}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            else
                                logger.LogResultFail("Rate {0:0.00}, avgProfit {1:0.00}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            MetricFactory.PrintMetricList(logger, t.metricId);
                            logger.LogResult("\n ---------------- \n");
                        }

                        logger.LogResult("SuccessRuns {0}: \n\n", successRuns.Count);

                        OutputFormatter.AddClusterInfo(ref successRuns);
                        foreach (RunOutput t in successRuns.Values)
                        {
                            if (t.success)
                                logger.LogResultSuccess("Rate {0:0.00}, avgProfit {1:0.00}, id {2}, cl {3}: ", t.rate, t.averageProfit, t.metricId, t.cluster);
                            else
                                logger.LogResultFail("Rate {0:0.00}, avgProfit {1:0.00}, id {2}, cl {3}:", t.rate, t.averageProfit, t.metricId, t.cluster);
                            MetricFactory.PrintMetricList(logger, t.metricId);
                            logger.LogResult("\n ---------------- \n");
                        }
                        if (configManager.GetLogLevel() > ConfigManager.LogLevel.LOG_RESULT)
                            OutputFormatter.PrintClusterInfo(logger, successRuns);
                    }
                }

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                Console.WriteLine("\nRunTime " + elapsedTime);
                Console.ReadLine();

                return 0;
            });

            app.Execute(args);

        }


    }
}
