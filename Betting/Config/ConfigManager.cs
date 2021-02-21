using System;
using System.Xml;

namespace Betting.Config
{
    public class ConfigManager : ConfigManagerInterface
    {

        private string GetData(string id)
        {
            XmlNodeList elemList = configDocument_.GetElementsByTagName(id);
            return elemList[0].InnerText;
        }

        public void SetLeagueName(string value)
        {
            LeagueName = value;
        }

        public override string GetLeagueName()
        {
            return LeagueName;
        }


        public void SetBetStyle(string value)
        {
            BetStyle = value;
        }

        public override string GetBetStyle()
        {
            return BetStyle;
        }

        public void SetYear(int value)
        {
            Year = value;
        }

        public override int GetYear()
        {
            return Year.Value;
        }

        public void SetReverseYears(int value)
        {
            ReverseYears = value;
        }

        public override int GetReverseYears()
        {
            return ReverseYears.Value;
        }

        public void SetLogLevel(string value)
        {
            TheLogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), value);
        }

        public override LogLevel GetLogLevel()
        {
            return TheLogLevel.Value;
        }

        public void SetDrawMargin(int value)
        {
            DrawMargin = value;
        }

        public override int GetDrawMargin()
        {
            return DrawMargin.Value;
        }

        public void SetDrawMixedMargin(int value)
        {
            DrawMixedMargin = value;
        }

        public override int GetDrawMixedMargin()
        {
            return DrawMixedMargin.Value;
        }

        public void SetMatchDay(int value)
        {
            Matchday = value;
        }

        public override int GetMatchDay()
        {
            return Matchday.Value;
        }

        public void SetReverseDays(int value)
        {
            ReverseDays = value;
        }

        public override int GetReverseDays()
        {
            return ReverseDays.Value;
        }

        public void SetMaxOdds(double value)
        {
            MaxOdds = value;
        }

        public override double GetMaxOdds()
        {
            return MaxOdds.Value;
        }

        public void SetMinOdds(double value)
        {
            MinOdds = value;
        }

        public override double GetMinOdds()
        {
            return MinOdds.Value;
        }

        public void SetMinMetricCorrect(double value)
        {
            MinMetricCorrect = value;
        }

        public override double GetMinMetricCorrect()
        {
            return MinMetricCorrect.Value;
        }

        public void SetMinYearProfit(double value)
        {
            MinYearProfit = value;
        }

        public override double GetMinYearProfit()
        {
            return MinYearProfit.Value;
        }

        public void SetMinAverageProfit(double value)
        {
            MinAverageProfit = value;
        }

        public override double GetMinAverageProfit()
        {
            return MinAverageProfit.Value;
        }

        public void SetFilterTopRate(int value)
        {
            FilterTopRate = value;
        }

        public override int GetFilterTopRate()
        {
            return FilterTopRate.Value;
        }

        public void SetFilterTopProfit(int value)
        {
            FilterTopProfit = value;
        }

        public override int GetFilterTopProfit()
        {
            return FilterTopProfit.Value;
        }

        public void SetUseExpanded(bool value)
        {
            UseExpanded = value;
        }

        public override bool GetUseExpanded()
        {
            return UseExpanded.Value;
        }

        public void SetCoeficientWeight(int value)
        {
            CoeficientWeight = value;
        }

        public override int GetCoeficientWeight()
        {
            return CoeficientWeight.Value;
        }

        public ConfigManager()
        {
            configDocument_ = new XmlDocument();
            configDocument_.Load("..\\..\\Config\\globalconfig.xml");


            LeagueName = GetData("leaguename");
            BetStyle = GetData("betstyle");

            Year = int.Parse(GetData("year"));
            ReverseYears = int.Parse(GetData("yreverse"));
            TheLogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), GetData("loglevel"));
            DrawMargin = int.Parse(GetData("drawmargin"));
            DrawMixedMargin = int.Parse(GetData("drawmixedmargin"));
            Matchday = int.Parse(GetData("matchday"));
            ReverseDays = int.Parse(GetData("mreverse"));
            MaxOdds = double.Parse(GetData("maxodds"));
            MinOdds = double.Parse(GetData("minodds"));
            MinMetricCorrect = double.Parse(GetData("minmetriccorrect"));
            MinYearProfit = double.Parse(GetData("minyearprofit"));
            MinAverageProfit = double.Parse(GetData("minaverageprofit"));
            FilterTopRate = int.Parse(GetData("filtertoprate"));
            FilterTopProfit = int.Parse(GetData("filtertopprofit"));
            UseExpanded = GetData("useexpanded") == "1";
            CoeficientWeight = int.Parse(GetData("coeficientweight"));
        }

        private readonly XmlDocument configDocument_;

        private string LeagueName;
        private string BetStyle;

        private int? Year;
        private int? ReverseYears;
        private LogLevel? TheLogLevel;
        private int? DrawMargin;
        private int? DrawMixedMargin;
        private int? Matchday;
        private int? ReverseDays;
        private double? MaxOdds;
        private double? MinOdds;
        private double? MinMetricCorrect;
        private double? MinYearProfit;
        private double? MinAverageProfit;
        private int? FilterTopRate;
        private int? FilterTopProfit;
        private bool? UseExpanded;
        private int? CoeficientWeight;

    }
}
