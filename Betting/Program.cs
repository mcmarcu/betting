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
        private static void getStatData(out int correctFixturesWithData, out int totalFixturesWithData, int matchDay, int year)
        {
            List<MetricInterface> metrics = MetricFactory.GetMetrics(matchDay,year);

            List<Fixture> thisRoundFixtures = FixtureRetriever.GetRound(year, matchDay);

            /*foreach (Fixture fixture in thisRoundFixtures)
            {
                foreach (MetricInterface metric in metrics)
                {
                    Console.Write("\n------ Metric: {0}, depth {1} -------\n", metric.config.name, metric.config.depth);
                    ResultChecker checker = new ResultChecker(metric, fixture);
                    checker.PrintResult();
                }
            }*/

            correctFixturesWithData = 0;
            totalFixturesWithData = 0;


            foreach (Fixture fixture in thisRoundFixtures)
            {
                int correctMetricsWithData = 0;
                int totalMetricsWithData = 0;
                foreach (MetricInterface metric in metrics)
                {
                    ResultChecker checker = new ResultChecker(metric, fixture);
                    ResultChecker.InterpretResultStatus status = checker.InterpretResult();
                    if (status != ResultChecker.InterpretResultStatus.NODATA)
                    {
                        totalMetricsWithData++;
                        if (status == ResultChecker.InterpretResultStatus.CORRECT)
                            correctMetricsWithData++;
                    }
                }
                if (totalMetricsWithData == metrics.Count)
                {
                    bool metricSuccess = ((float)correctMetricsWithData / (float)totalMetricsWithData) >= 0.66;

                    totalFixturesWithData++;
                    if (metricSuccess)
                        correctFixturesWithData++;

                    //Console.Write("Fixture : {0} - {1}, success {2} correctMetrics {3}\t -------\n", fixture.homeTeamName, fixture.awayTeamName, metricSuccess, correctMetricsWithData);
                }
                else
                {
                    //Console.Write("Fixture : {0} - {1}, no data \t -------\n");
                }
            }
            
        }

        static void Main(string[] args)
        {

            int year = ConfigManager.Instance.GetYear();
            int matchDay = ConfigManager.Instance.GetMatchDay();

            int totalFixtures = 0;
            int correctFixtures = 0;
            int globalTotal = 0;
            int globalCorrect = 0;
            float rate = 0;

            for(int i=0;i<38;++i)
            {
                getStatData(out correctFixtures, out totalFixtures, matchDay, year);
                Console.Write("Matchday : {0} - year {1}, correct {2}\t total {3} \n", matchDay, year, correctFixtures, totalFixtures);
                globalTotal += totalFixtures;
                globalCorrect += correctFixtures;
                FixtureRetriever.GetPrevRound(out year, out matchDay, year, matchDay);
            }

            rate = ((float)globalCorrect / (float)globalTotal)*100;

            Console.Write("Global success rate : {0}  -------\n", rate);

            Console.ReadLine();
        }
    }
}
