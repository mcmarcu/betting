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
        static public List<MetricInterface> GetMetrics()
        {
            List<MetricInterface> result = new List<MetricInterface>();
            List<MetricConfig> configs = ConfigManager.Instance.GetMetricConfigs();
            foreach(MetricConfig config in configs)
            {
                if (config.name == "LastGamesMetric")
                    result.Add(new LastGamesMetric(config));
                if (config.name == "LastHomeAwayGamesMetric")
                    result.Add(new LastHomeAwayGamesMetric(config));
            }
            return result;
        }
    }
}
