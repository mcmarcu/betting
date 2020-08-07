using System;
using System.Collections.Generic;
using System.Xml;

namespace Betting.Config
{
    public class ConfigManager : ConfigManagerInterface
    {
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

        public override string GetLeagueName()
        {
            return GetData("league");
        }

        public void SetYear(string value)
        {
            SetData("year", value);
        }

        public override int GetYear()
        {
            return int.Parse(GetData("year"));
        }

        public void SetReverseYears(string value)
        {
            SetData("yreverse", value);
        }

        public override int GetReverseYears()
        {
            return int.Parse(GetData("yreverse"));
        }

        public void SetLogLevel(string value)
        {
            SetData("loglevel", value);
        }

        public override LogLevel GetLogLevel()
        {
            return (LogLevel)Enum.Parse(typeof(LogLevel), GetData("loglevel"));
        }

        public void SetDrawMargin(string value)
        {
            SetData("drawmargin", value);
        }

        public override int GetDrawMargin()
        {
            return int.Parse(GetData("drawmargin"));
        }

        public void SetDrawMixedMargin(string value)
        {
            SetData("drawmixedmargin", value);
        }

        public override int GetDrawMixedMargin()
        {
            return int.Parse(GetData("drawmixedmargin"));
        }

        public void SetMatchDay(string value)
        {
            SetData("matchday", value);
        }

        public override int GetMatchDay()
        {
            return int.Parse(GetData("matchday"));
        }

        public void SetReverseDays(string value)
        {
            SetData("mreverse", value);
        }

        public override int GetReverseDays()
        {
            return int.Parse(GetData("mreverse"));
        }

        public void SetMaxOdds(string value)
        {
            SetData("maxodds", value);
        }

        public override double GetMinOdds()
        {
            return double.Parse(GetData("minodds"));
        }

        public void SetMinOdds(string value)
        {
            SetData("minodds", value);
        }

        public override double GetMaxOdds()
        {
            return double.Parse(GetData("maxodds"));
        }

        public void SetMinMetricCorrect(string value)
        {
            SetData("minmetriccorrect", value);
        }

        public override double GetMinMetricCorrect()
        {
            return double.Parse(GetData("minmetriccorrect"));
        }

        public void SetMinYearProfit(string value)
        {
            SetData("minyearprofit", value);
        }

        public override double GetMinYearProfit()
        {
            return double.Parse(GetData("minyearprofit"));
        }

        public void SetSuccessRate(string value)
        {
            SetData("successrate", value);
        }

        public override double GetSuccessRate()
        {
            return double.Parse(GetData("successrate"));
        }

        public void SetFilterTopRate(string value)
        {
            SetData("filtertoprate", value);
        }

        public override int GetFilterTopRate()
        {
            return int.Parse(GetData("filtertoprate"));
        }

        public void SetFilterTopProfit(string value)
        {
            SetData("filtertopprofit", value);
        }

        public override int GetFilterTopProfit()
        {
            return int.Parse(GetData("filtertopprofit"));
        }


        public void SetBetStyle(string value)
        {
            SetData("betstyle", value);
        }

        public override string GetBetStyle()
        {
            return GetData("betstyle");
        }

        public void SetUseExpanded(bool value)
        {
            SetData("useexpanded", value ? "1" : "0");
        }

        public override bool GetUseExpanded()
        {
            return int.Parse(GetData("useexpanded")) == 1;
        }

        public void SetCoeficientWeight(int value)
        {
            SetData("coeficientweight", value.ToString());
        }

        public override int GetCoeficientWeight()
        {
            return int.Parse(GetData("coeficientweight"));
        }

        public ConfigManager()
        {
            configDocument_ = new XmlDocument();
            configDocument_.Load("..\\..\\Config\\globalconfig.xml");
        }
        private readonly XmlDocument configDocument_;
        private readonly Dictionary<string, string> cache = new Dictionary<string, string>();
    }
}
