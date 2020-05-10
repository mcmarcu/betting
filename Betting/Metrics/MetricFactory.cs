using Betting.Config;
using Betting.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Metrics
{
    class MetricFactory
    {
        static public List<MetricInterface> GetMetrics(List<MetricConfig> configs, int year)
        {
            List<MetricInterface> result = new List<MetricInterface>();
            foreach(MetricConfig config in configs)
            {
                if (config.name == "LastGamesMetric")
                    result.Add(new LastGamesMetric(config, year));
                if (config.name == "GoalsScoredMetric")
                    result.Add(new GoalsScoredMetric(config, year));
                if (config.name == "GoalsConcededMetric")
                    result.Add(new GoalsConcededMetric(config, year));
                if (config.name == "HomeAdvantageMetric")
                    result.Add(new HomeAdvantageMetric(config, year));
                if (config.name == "GoalsDifferenceMetric")
                    result.Add(new GoalsDiferenceMetric(config, year));
            }
            return result;
        }
    }
}
