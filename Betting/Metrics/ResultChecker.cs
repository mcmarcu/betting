﻿using Betting.Config;
using Betting.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Metrics
{
    class ResultChecker
    {
        public ResultChecker(MetricInterface metric, Fixture fixture)
        {
            this.metric = metric;
            this.fixture = fixture;
            metric.GetPercentage(out this.pct1, out this.pct2, fixture.homeTeamName, fixture.awayTeamName);
        }

        private string GetActualResult()
        {
            if (fixture.finalScore.homeTeamGoals == fixture.finalScore.awayTeamGoals)
                return "X";
            else if (fixture.finalScore.homeTeamGoals > fixture.finalScore.awayTeamGoals)
                return "1";
            else
                return "2";
        }

        private string GetExpectedResult()
        {
            int drawMargin = ConfigManager.Instance.GetDrawMargin();
            int drawMixedMargin = ConfigManager.Instance.GetDrawMixedMargin();
            if (Math.Abs(pct1 - pct2) < drawMargin*2)
                return "X";

            string result = string.Empty;
            if (pct1 > pct2)
                result += "1";
            if (Math.Abs(pct1 - pct2) < drawMixedMargin * 2)
                result += "X";
            if (pct1 < pct2)
                result += "2";
            return result;
        }

        public void PrintResult()
        {
            InterpretResultStatus status = InterpretResult();
            if (status == InterpretResultStatus.CORRECT)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (status == InterpretResultStatus.WRONG)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ResetColor();
            try
            {
                Console.Write("{0} vs {1}\t\tchances -- {2} : {3} ({4})", fixture.homeTeamName, fixture.awayTeamName, pct1, pct2, GetExpectedResult());
                if (fixture.finished)
                {
                    Console.Write(" ---- Final score {0} vs {1} ({2})", fixture.finalScore.homeTeamGoals, fixture.finalScore.awayTeamGoals,GetActualResult());
                }
                Console.WriteLine();
            }
            catch (Exception)
            {
                Console.Write("{0} vs {1} \t not enough data\n", fixture.homeTeamName, fixture.awayTeamName);
            }
            Console.ResetColor();
        }

        public enum InterpretResultStatus
        {
            CORRECT,
            WRONG,
            NODATA
        }

        public InterpretResultStatus InterpretResult()
        {
            if (!fixture.finished)
                return InterpretResultStatus.NODATA;
            
            try
            {
                string expectedResult = GetExpectedResult();
                string actualResult = GetActualResult();

                if (expectedResult.Contains(actualResult))
                    return InterpretResultStatus.CORRECT;
                else
                    return InterpretResultStatus.WRONG;
            }
            catch (Exception)
            {
                return InterpretResultStatus.NODATA;
            }
        }

        private MetricInterface metric;
        private Fixture fixture;
        private int pct1;
        private int pct2;
    }
}
