
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

        abstract public string GetLeagueName();
        abstract public int GetYear();
        abstract public int GetReverseYears();
        abstract public LogLevel GetLogLevel();
        abstract public int GetDrawMargin();
        abstract public int GetDrawMixedMargin();
        abstract public int GetMatchDay();
        abstract public int GetReverseDays();
        abstract public float GetMinOdds();
        abstract public float GetMaxOdds();
        abstract public float GetMinMetricCorrect();
        abstract public float GetMinYearProfit();
        abstract public float GetSuccessRate();
        abstract public int GetFilterTopRate();
        abstract public int GetFilterTopProfit();
        abstract public string GetBetStyle();
        abstract public bool GetUseExpanded();
        abstract public int GetCoeficientWeight();
    }
}
