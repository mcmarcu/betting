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
        static public List<MetricInterface> GetMetrics(int matchDay, int year)
        {
            List<MetricInterface> result = new List<MetricInterface>();
            List<MetricConfig> configs = ConfigManager.Instance.GetMetricConfigs();
            foreach(MetricConfig config in configs)
            {
                if (config.name == "LastGamesMetric")
                    result.Add(new LastGamesMetric(config, matchDay, year));
                if (config.name == "GoalsScoredMetric")
                    result.Add(new GoalsScoredMetric(config, matchDay, year));
                if (config.name == "GoalsConcededMetric")
                    result.Add(new GoalsConcededMetric(config, matchDay, year));
                if (config.name == "DirectGamesMetric")
                    result.Add(new DirectGamesMetric(config, matchDay, year));
            }
            return result;
        }
    }
}
