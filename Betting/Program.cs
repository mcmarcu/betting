using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Betting.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting
{
    class Program
    {
        

        static void Main(string[] args)
        {
            int year = ConfigManager.Instance.GetYear();
            int matchDay = ConfigManager.Instance.GetMatchDay();
            int reverseDays = ConfigManager.Instance.GetReverseDays();

            int totalFixtures = 0;
            int correctFixtures = 0;
            int globalTotal = 0;
            int globalCorrect = 0;
            float rate = 0;
            float currentProfit = 0;
            float globalProfit = 0;

            for(int i=0;i<reverseDays; ++i)
            {
                GlobalStats.GetData(out correctFixtures, out totalFixtures, out currentProfit, matchDay, year);
                Console.Write("Matchday : {0} - year {1}, correct {2}\t total {3}, rate {4}, profit {5:0.00} \n", matchDay, year, correctFixtures, totalFixtures, ((float)correctFixtures / (float)totalFixtures) * 100, currentProfit);
                globalTotal += totalFixtures;
                globalCorrect += correctFixtures;
                globalProfit += currentProfit;
                FixtureRetriever.GetPrevRound(out year, out matchDay, year, matchDay);
            }

            rate = ((float)globalCorrect / (float)globalTotal)*100;

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\nGlobal success rate : {0}, profit {1}  -------\n", rate, globalProfit);
            Console.ResetColor();

            Console.ReadLine();
        }
    }
}
