using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public class MetricFactory
    {
        public const int differentMetricCount = 5;
        private static void ExtractMetricCoefficient(int i, out int LastGamesMetricD, out int GoalsConcededMetricD, out int GoalsDifferenceMetricD, out int GoalsScoredMetricD, out int HomeAdvantageMetricD)
        {
            LastGamesMetricD = i % 10;
            GoalsConcededMetricD = (i / 10) % 10;
            GoalsDifferenceMetricD = (i / 100) % 10;
            GoalsScoredMetricD = (i / 1000) % 10;
            HomeAdvantageMetricD = (i / 10000) % 10;
        }

        public static void PrintMetricList(Logger logger, int i)
        {
            ExtractMetricCoefficient(i, 
                out int LastGamesMetricD, 
                out int GoalsConcededMetricD, 
                out int GoalsDifferenceMetricD, 
                out int GoalsScoredMetricD, 
                out int HomeAdvantageMetricD);

            if (LastGamesMetricD != 0)
            {
                logger.LogResult(" LastGames ({0}); ", LastGamesMetricD);
            }

            if (GoalsDifferenceMetricD != 0)
            {
                logger.LogResult(" GoalsDifferenceMetric ({0}); ", GoalsDifferenceMetricD);
            }

            if (GoalsScoredMetricD != 0)
            {
                logger.LogResult(" GoalsScored ({0}); ", GoalsScoredMetricD);
            }

            if (GoalsConcededMetricD != 0)
            {
                logger.LogResult(" GoalsConceded ({0}); ", GoalsConcededMetricD);
            }

            if (HomeAdvantageMetricD != 0)
            {
                logger.LogResult(" HomeAdvantageMetric ({0}); ", HomeAdvantageMetricD);
            }

        }

        public static List<MetricConfig> GetMetricList(int i)
        {
            ExtractMetricCoefficient(i,
                out int LastGamesMetricD,
                out int GoalsConcededMetricD,
                out int GoalsDifferenceMetricD,
                out int GoalsScoredMetricD,
                out int HomeAdvantageMetricD);

            List<MetricConfig> metricConfigs = new List<MetricConfig>(MetricFactory.differentMetricCount);
            if (LastGamesMetricD != 0)
            {
                MetricConfig lastGamesMetric = new MetricConfig
                {
                    name = "LastGamesMetric",
                    depth = LastGamesMetricD
                };
                metricConfigs.Add(lastGamesMetric);
            }

            if (GoalsDifferenceMetricD != 0)
            {
                MetricConfig goalsDifferenceMetric = new MetricConfig
                {
                    name = "GoalsDifferenceMetric",
                    depth = GoalsDifferenceMetricD
                };
                metricConfigs.Add(goalsDifferenceMetric);
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

            return metricConfigs;
        }

        public static List<MetricInterface> GetMetrics(List<MetricConfig> configs, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever)
        {
            List<MetricInterface> result = new List<MetricInterface>(configs.Count);
            foreach (MetricConfig config in configs)
            {
                if (config.name == "LastGamesMetric")
                {
                    result.Add(new LastGamesMetric(config, year, configManager, fixtureRetriever));
                }

                if (config.name == "GoalsScoredMetric")
                {
                    result.Add(new GoalsScoredMetric(config, year, configManager, fixtureRetriever));
                }

                if (config.name == "GoalsConcededMetric")
                {
                    result.Add(new GoalsConcededMetric(config, year, configManager, fixtureRetriever));
                }

                if (config.name == "HomeAdvantageMetric")
                {
                    result.Add(new HomeAdvantageMetric(config, year, configManager, fixtureRetriever));
                }

                if (config.name == "GoalsDifferenceMetric")
                {
                    result.Add(new GoalsDifferenceMetric(config, year, configManager, fixtureRetriever));
                }
            }

            if (result.Count != configs.Count)
            {
                throw new ArgumentException("Could not create all configs. Check input to MetricFactory.GetMetrics");
            }

            return result;
        }
    }
}
