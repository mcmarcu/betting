
namespace Betting.Config
{
    public abstract class ConfigManagerInterface
    {
        public enum LogLevel
        {
            LOG_DEBUG,
            LOG_INFO,
            LOG_RESULT
        }

        public abstract string GetLeagueName();
        public abstract string GetBetStyle();
        public abstract int GetYear();
        public abstract int GetReverseYears();
        public abstract LogLevel GetLogLevel();
        public abstract int GetDrawMargin();
        public abstract int GetDrawMixedMargin();
        public abstract int GetMatchDay();
        public abstract int GetReverseDays();
        public abstract double GetMinOdds();
        public abstract double GetMaxOdds();
        public abstract double GetMinMetricCorrect();
        public abstract double GetMinYearProfit();
        public abstract double GetMinAverageProfit();
        public abstract int GetFilterTopRate();
        public abstract int GetFilterTopProfit();
        public abstract bool GetUseExpanded();
        public abstract int GetCoeficientWeight();
    }
}
