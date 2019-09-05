using Betting.DataModel;
using Betting.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Betting.Config
{
    class ConfigManager
    {
        public enum LogLevel
        {
            LOG_ALL,
            LOG_EXTRA,
            LOG_RESULT
        }

        public static ConfigManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigManager();
                }
                return instance;
            }
        }

        public List<MetricConfig> GetMetricConfigs()
        {
            List<MetricConfig> result = new List<MetricConfig>();
            XmlNodeList elemList = configDocument_.GetElementsByTagName("metrics");
            foreach (XmlNode metric in elemList[0].ChildNodes)
            {
                MetricConfig config = new MetricConfig();
                config.name = metric.Name;
                foreach (XmlNode attr in metric.ChildNodes)
                {
                    if (attr.Name == "weight")
                        config.weight = Int32.Parse(attr.InnerText);

                    if (attr.Name == "depth")
                        config.depth = Int32.Parse(attr.InnerText);

                }
                result.Add(config);
            }
            return result;
        }

        public void SetMetricConfigs(List<MetricConfig> metrics)
        {
            XmlNodeList elemList = configDocument_.GetElementsByTagName("metrics");
            elemList[0].RemoveAll();
            foreach(MetricConfig metric in metrics)
            {
                XmlElement rootElem = configDocument_.CreateElement(metric.name);
                XmlElement weightElem = configDocument_.CreateElement("weight");
                weightElem.InnerText = metric.weight.ToString();
                XmlElement depthElem = configDocument_.CreateElement("depth");
                depthElem.InnerText = metric.depth.ToString();
                rootElem.AppendChild(weightElem);
                rootElem.AppendChild(depthElem);
                elemList[0].AppendChild(rootElem);
            }
        }

        private string GetData(string id)
        {
            if (!cache.ContainsKey(id))
            {
                XmlNodeList elemList = configDocument_.GetElementsByTagName(id);
                cache[id] = elemList[0].InnerText;
            }
            return cache[id];
        }

        public string GetLeagueName()
        {
            return GetData("league");
        }

        public int GetYear()
        {
            return Int32.Parse(GetData("year"));
        }

        public int GetReverseYears()
        {
            return Int32.Parse(GetData("yreverse"));
        }

        public LogLevel GetLogLevel()
        {
            return (LogLevel)Enum.Parse(typeof(LogLevel), GetData("loglevel"));
        }

        public int GetDrawMargin()
        {
            return Int32.Parse(GetData("drawmargin"));
        }

        public int GetDrawMixedMargin()
        {
            return Int32.Parse(GetData("drawmixedmargin"));
        }

        public int GetMatchDay()
        {
            return Int32.Parse(GetData("matchday"));
        }

        public int GetReverseDays()
        {
            return Int32.Parse(GetData("mreverse"));
        }

        public float GetMaxOdds()
        {
            return float.Parse(GetData("maxodds"));
        }

        public float GetMinMetricCorrect()
        {
            return float.Parse(GetData("minmetriccorrect"));
        }

        public float GetMinYearProfit()
        {
            return float.Parse(GetData("minyearprofit"));
        }
        

        private ConfigManager()
        {
            configDocument_ = new XmlDocument();
            configDocument_.Load(@"c:\users\mcmar\documents\visual studio 2017\Projects\Betting\Betting\Config\globalconfig.xml");
        }
        private static ConfigManager instance;
        private XmlDocument configDocument_;
        private Dictionary<string, string> cache = new Dictionary<string, string>();
    }
}
