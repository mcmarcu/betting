﻿using Betting.Config;
using Betting.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Metrics
{
    public class ResultChecker
    {
        public ResultChecker(MetricInterface metric, Fixture fixture, ConfigManagerInterface configManager)
        {
            this.configManager = configManager;
            this.dataAvailable = true;
            try
            {
                metric.GetPercentage(out this.pct1, out this.pct2, fixture.homeTeamName, fixture.awayTeamName, fixture);
            }
            catch(Exception)
            {
                this.dataAvailable = false;
            }
        }

        public string GetExpectedResult()
        {
            int drawMargin = configManager.GetDrawMargin();
            int drawMixedMargin = configManager.GetDrawMixedMargin();
            if (Math.Abs(pct1 - pct2) < drawMargin)
                return "X";

            string result = string.Empty;
            if (pct1 > pct2)
                result += "1";
            if (Math.Abs(pct1 - pct2) < drawMixedMargin)
                result += "X";
            if (pct1 < pct2)
                result += "2";
            return result;
        }

        private readonly ConfigManagerInterface configManager;
        public int pct1;
        public int pct2;
        public bool dataAvailable;
    }
}
