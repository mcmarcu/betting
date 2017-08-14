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

            List<MetricInterface> metrics = MetricFactory.GetMetrics();
            
            int year = ConfigManager.Instance.GetYear();
            int matchDay = ConfigManager.Instance.GetMatchDay();
            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);
            foreach(MetricInterface metric in metrics)
            {
                Console.Write("\n------ Metric: {0}, depth {1} -------\n", metric.config.name, metric.config.depth);
                foreach (Fixture fixture in thisRoundFixtures)
                {
                    int pct1 = 0;
                    int pct2 = 0;
                    try
                    {
                        metric.GetPercentage(out pct1, out pct2, fixture.homeTeamName, fixture.awayTeamName);
                        Console.Write("{0} vs {1} \t chances -- {2} : {3}", fixture.homeTeamName, fixture.awayTeamName, pct1, pct2);
                        if(fixture.finished)
                            Console.Write("\t---- Final score {0} vs {1}", fixture.finalScore.homeTeamGoals, fixture.finalScore.awayTeamGoals);
                        Console.WriteLine();
                    }
                    catch (Exception)
                    {
                        Console.Write("{0} vs {1} \t not enough data\n", fixture.homeTeamName, fixture.awayTeamName);
                    }
                }

            }

            Console.ReadLine();
        }
    }
}
