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

        private string GetData(string id)
        {
            if (!cache.ContainsKey(id))
            {
                XmlNodeList elemList = configDocument_.GetElementsByTagName(id);
                cache[id] = elemList[0].InnerText;
            }
            return cache[id];
        }

        public string GetCompetitionsUrl()
        {
            return GetData("competitions");
        }

        public string GetFixturesForLeagueUrl()
        {
            return GetData("fixturesforleague");
        }

        public string GetLeagueTableUrl()
        {
            return GetData("leaguetable");
        }

        public string GetLeagueInfoUrl()
        {
            return GetData("leagueinfo");
        }

        public string GetLeagueName()
        {
            return GetData("league");
        }

        public int GetYear()
        {
            return Int32.Parse(GetData("year"));
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
            int matchDay =  Int32.Parse(GetData("matchday"));
            if(matchDay == -1)
            {
                matchDay = FixtureRetriever.GetCurrentMatchDay(GetYear());
            }
            return matchDay;
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
