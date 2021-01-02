using Betting.Config;
using Betting.DataModel;
using System;

namespace Betting.Metrics
{
    public class ResultChecker
    {
        public ResultChecker(MetricInterface metric, Fixture fixture, ConfigManagerInterface configManager)
        {
            this.configManager = configManager;
            dataAvailable = true;
            try
            {
                metric.GetPercentage(out this.pct1, out this.pct2, fixture.homeTeamId, fixture.awayTeamId, fixture);
            }
            catch (Exception)
            {
                dataAvailable = false;
            }
        }

        public string GetExpectedResult()
        {
            int drawMargin = configManager.GetDrawMargin();
            int drawMixedMargin = configManager.GetDrawMixedMargin();

            int pctDiff = Math.Abs(pct1 - pct2);
            if (pctDiff < drawMargin)
            {
                return "X";
            }
            if (pct1 > pct2)
            {
                return pctDiff < drawMixedMargin ? "1X" : "1"; 
            }
            if (pct1 < pct2)
            {
                return pctDiff < drawMixedMargin ? "X2" : "2";
            }
            if(pctDiff < drawMixedMargin)
            {
                return "X";
            }
            throw new ArgumentException("Failed to get expected result. Check drawMargin and drawMixedMrgin");
        }

        private readonly ConfigManagerInterface configManager;
        public int pct1;
        public int pct2;
        public bool dataAvailable;
    }
}
