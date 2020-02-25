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
            LOG_DEBUG,
            LOG_INFO,
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

        private void SetData(string id, string value)
        {
            cache[id] = value;
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

        public void SetLeagueName(string value)
        {
            SetData("league", value);
        }

        public string GetLeagueName()
        {
            return GetData("league");
        }

        public void SetYear(string value)
        {
            SetData("year", value);
        }

        public int GetYear()
        {
            return Int32.Parse(GetData("year"));
        }

        public void SetReverseYears(string value)
        {
            SetData("yreverse", value);
        }

        public int GetReverseYears()
        {
            return Int32.Parse(GetData("yreverse"));
        }

        public void SetLogLevel(string value)
        {
            SetData("loglevel", value);
        }

        public LogLevel GetLogLevel()
        {
            return (LogLevel)Enum.Parse(typeof(LogLevel), GetData("loglevel"));
        }

        public void SetDrawMargin(string value)
        {
            SetData("drawmargin", value);
        }

        public int GetDrawMargin()
        {
            return Int32.Parse(GetData("drawmargin"));
        }

        public void SetDrawMixedMargin(string value)
        {
            SetData("drawmixedmargin", value);
        }

        public int GetDrawMixedMargin()
        {
            return Int32.Parse(GetData("drawmixedmargin"));
        }

        public void SetMatchDay(string value)
        {
            SetData("matchday", value);
        }

        public int GetMatchDay()
        {
            return Int32.Parse(GetData("matchday"));
        }

        public void SetReverseDays(string value)
        {
            SetData("mreverse", value);
        }

        public int GetReverseDays()
        {
            return Int32.Parse(GetData("mreverse"));
        }

        public void SetMaxOdds(string value)
        {
            SetData("maxodds", value);
        }

        public float GetMaxOdds()
        {
            return float.Parse(GetData("maxodds"));
        }

        public void SetMinMetricCorrect(string value)
        {
            SetData("minmetriccorrect", value);
        }

        public float GetMinMetricCorrect()
        {
            return float.Parse(GetData("minmetriccorrect"));
        }

        public void SetMinYearProfit(string value)
        {
            SetData("minyearprofit", value);
        }

        public float GetMinYearProfit()
        {
            return float.Parse(GetData("minyearprofit"));
        }

        public void SetSuccessRate(string value)
        {
            SetData("successrate", value);
        }

        public float GetSuccessRate()
        {
            return float.Parse(GetData("successrate"));
        }

        public void SetFilterTopRate(string value)
        {
            SetData("filtertoprate", value);
        }

        public int GetFilterTopRate()
        {
            return Int32.Parse(GetData("filtertoprate"));
        }

        public void SetFilterTopProfit(string value)
        {
            SetData("filtertopprofit", value);
        }

        public int GetFilterTopProfit()
        {
            return Int32.Parse(GetData("filtertopprofit"));
        }


        public void SetBetStyle(string value)
        {
            SetData("betstyle", value);
        }

        public string GetBetStyle()
        {
            return GetData("betstyle");
        }


        private ConfigManager()
        {
            configDocument_ = new XmlDocument();
            configDocument_.Load("..\\..\\Config\\globalconfig.xml");
        }
        private static ConfigManager instance;
        private XmlDocument configDocument_;
        private Dictionary<string, string> cache = new Dictionary<string, string>();
    }
}
