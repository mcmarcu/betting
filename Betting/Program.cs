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

namespace Betting
{
    class Program
    {

        public static List<MetricConfig> GetMetricList(int i)
        {
            List<MetricConfig> metricConfigs = new List<MetricConfig>();

            int LastGamesMetricD = i % 10;
            int GoalsScoredMetricD = (i / 10) % 10;
            int GoalsConcededMetricD = (i / 100) % 10;

            if (LastGamesMetricD != 0)
            { 
                MetricConfig lastGamesMetric = new MetricConfig
                {
                    name = "LastGamesMetric",
                    depth = LastGamesMetricD
                };
                Console.Write("Name {0}, Depth {1} \n", lastGamesMetric.name, lastGamesMetric.depth);
                metricConfigs.Add(lastGamesMetric);
            }

            if (GoalsScoredMetricD != 0)
            {
                MetricConfig goalsScoredMetric = new MetricConfig
                {
                    name = "GoalsScoredMetric",
                    depth = GoalsScoredMetricD
                };
                Console.Write("Name {0}, Depth {1} \n", goalsScoredMetric.name, goalsScoredMetric.depth);
                metricConfigs.Add(goalsScoredMetric);
            }

            if (GoalsConcededMetricD != 0)
            {
                MetricConfig goalsConcededMetric = new MetricConfig
                {
                    name = "GoalsConcededMetric",
                    depth = GoalsConcededMetricD
                };
                Console.Write("Name {0}, Depth {1} \n", goalsConcededMetric.name, goalsConcededMetric.depth);
                metricConfigs.Add(goalsConcededMetric);
            }

            if(i / 1000 > 0)
            {
                metricConfigs.AddRange(GetMetricList(i / 1000));
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
            var logLevelOption = app.Option("-g|--loglevel <optionvalue>", "LOG_ALL, LOG_EXTRA, LOG_RESULT", CommandOptionType.SingleValue);
            var successRateOption = app.Option("-s|--successrate <optionvalue>", "how much should be correct(75)", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
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

                if (executeMetrics.HasValue())
                {
                    for (int i = 0; i <= 1000000; ++i)
                    {
                        List<MetricConfig> metricConfigs = GetMetricList(i);

                        if (metricConfigs.Count == 0)
                            continue;

                        ConfigManager.Instance.SetMetricConfigs(metricConfigs);

                        GlobalStats.GetAllYearsData(out bool success, out float rate, out float averageProfit);

                        if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_RESULT)
                        {
                            Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
                                
                            Console.Write("Result {0}, Rate {1}, avgProfit {2} \n", success, rate, averageProfit);
                            if (success || rate > ConfigManager.Instance.GetSuccessRate())
                                Console.ReadLine();

                            Console.ResetColor();
                        }
                    }
                }

                Console.Write("DONE");
                Console.ReadLine();
                return 0;
            });

            app.Execute(args);
        }
    }
}
