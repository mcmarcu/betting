using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System;
using System.Collections.Generic;

namespace Betting.Metrics
{
    public class MetricFactory
    {
        static public List<MetricInterface> GetMetrics(List<MetricConfig> configs, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriever)
        {
            List<MetricInterface> result = new List<MetricInterface>();
            foreach(MetricConfig config in configs)
            {
                if (config.name == "LastGamesMetric")
                    result.Add(new LastGamesMetric(config, year, configManager, fixtureRetriever));
                if (config.name == "GoalsScoredMetric")
                    result.Add(new GoalsScoredMetric(config, year, configManager, fixtureRetriever));
                if (config.name == "GoalsConcededMetric")
                    result.Add(new GoalsConcededMetric(config, year, configManager, fixtureRetriever));
                if (config.name == "HomeAdvantageMetric")
                    result.Add(new HomeAdvantageMetric(config, year, configManager, fixtureRetriever));
                if (config.name == "GoalsDifferenceMetric")
                    result.Add(new GoalsDifferenceMetric(config, year, configManager, fixtureRetriever));
            }

            if(result.Count != configs.Count)
            {
                throw new ArgumentException("Could not create all configs. Check input to MetricFactory.GetMetrics");
            }

            return result;
        }
    }
}
