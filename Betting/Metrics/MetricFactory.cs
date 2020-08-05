using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using System.Collections.Generic;

namespace Betting.Metrics
{
    class MetricFactory
    {
        static public List<MetricInterface> GetMetrics(List<MetricConfig> configs, int year, ConfigManagerInterface configManager, FixtureRetrieverInterface fixtureRetriver )
        {
            List<MetricInterface> result = new List<MetricInterface>();
            foreach(MetricConfig config in configs)
            {
                if (config.name == "LastGamesMetric")
                    result.Add(new LastGamesMetric(config, year, configManager, fixtureRetriver));
                if (config.name == "GoalsScoredMetric")
                    result.Add(new GoalsScoredMetric(config, year, configManager, fixtureRetriver));
                if (config.name == "GoalsConcededMetric")
                    result.Add(new GoalsConcededMetric(config, year, configManager, fixtureRetriver));
                if (config.name == "HomeAdvantageMetric")
                    result.Add(new HomeAdvantageMetric(config, year, configManager, fixtureRetriver));
                if (config.name == "GoalsDifferenceMetric")
                    result.Add(new GoalsDifferenceMetric(config, year, configManager, fixtureRetriver));
            }
            return result;
        }
    }
}
