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

            int totalFixtures = 0;
            int correctFixtures = 0;
            int globalTotal = 0;
            int globalCorrect = 0;
            float rate = 0;

            for(int i=0;i<37;++i)
            {
                GlobalStats.GetDataEx(out correctFixtures, out totalFixtures, matchDay, year);
                Console.Write("Matchday : {0} - year {1}, correct {2}\t total {3}, rate {4} \n", matchDay, year, correctFixtures, totalFixtures, ((float)correctFixtures / (float)totalFixtures) * 100);
                globalTotal += totalFixtures;
                globalCorrect += correctFixtures;
                FixtureRetriever.GetPrevRound(out year, out matchDay, year, matchDay);
            }

            rate = ((float)globalCorrect / (float)globalTotal)*100;

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("\nGlobal success rate : {0}  -------\n", rate);
            Console.ResetColor();

            Console.ReadLine();
        }
    }
}
