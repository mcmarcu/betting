using Accord.MachineLearning;
using System.Collections.Generic;

namespace Betting.Config
{
    public class RunOutput
    {
        public bool success;
        public double rate;
        public double averageProfit;
        public int metricId;
        public List<double> metricDepths;
        public int cluster;

        public RunOutput(bool success_, double rate_, double averageProfit_, int i, int maxI)
        {
            success = success_;
            rate = rate_;
            averageProfit = averageProfit_;
            metricDepths = new List<double>();
            metricId = i;
            maxI /= 10;
            while (maxI > 0)
            {
                metricDepths.Add(i % 10);
                maxI /= 10;
                i /= 10;
            }
            cluster = 0;
        }
    }

    public class OutputFormatter
    {
        public static void AddClusterInfo(ref SortedDictionary<double, RunOutput> dict)
        {
            if (dict.Count <= 3)
            {
                return;
            }

            Accord.Math.Random.Generator.Seed = 0;

            double[][] metrics = new double[dict.Count][];
            int i = 0;
            foreach (RunOutput t in dict.Values)
            {
                metrics[i++] = t.metricDepths.ToArray();
            }

            // Create a new K-Means algorithm
            KMeans kmeans = new KMeans(k: dict.Count / 3);

            // Compute and retrieve the data centroids
            KMeansClusterCollection clusters = kmeans.Learn(metrics);

            // Use the centroids to parition all the data
            int[] labels = clusters.Decide(metrics);

            int j = 0;
            foreach (RunOutput v in dict.Values)
            {
                v.cluster = labels[j++];
            }
        }

        public static void PrintClusterInfo(Logger logger, SortedDictionary<double, RunOutput> dict)
        {
            SortedDictionary<int, SortedSet<int>> clusteredOutput = new SortedDictionary<int, SortedSet<int>>();

            foreach (RunOutput t in dict.Values)
            {
                if (!clusteredOutput.ContainsKey(t.cluster))
                {
                    clusteredOutput.Add(t.cluster, new SortedSet<int>());
                }

                clusteredOutput[t.cluster].Add(t.metricId);
            }

            foreach (int k in clusteredOutput.Keys)
            {
                logger.LogResult("config {0}", k);
                foreach (int t in clusteredOutput[k])
                {
                    logger.LogResult(", {0}", t);
                }

                logger.LogResult("\n");
            }

        }
    }
}
