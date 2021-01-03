using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace BettingTest
{
    public class GlobalStatsBasicTest
    {
        private Mock<ConfigManagerInterface> configManagerMock;
        private Mock<FixtureRetrieverInterface> fixtureRetrieverMock;
        private Logger logger;

        private string team1;
        private string team2;
        private Fixture actualFixture;

        [SetUp]
        public void Setup()
        {
            team1 = "team1";
            team2 = "team2";

            actualFixture = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = team2
            };
            actualFixture.finalScore.homeTeamGoals = 1;
            actualFixture.finalScore.awayTeamGoals = 2;
            actualFixture.coeficient.homeTeam = 1;
            actualFixture.coeficient.awayTeam = 1;

            configManagerMock = new Mock<ConfigManagerInterface>();
            fixtureRetrieverMock = new Mock<FixtureRetrieverInterface>();
            logger = new Logger(ConfigManagerInterface.LogLevel.LOG_DEBUG);
        }

        [Test]
        public void ComputeExpectedResultMin1Results1X()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);
            List<MetricConfig> configs = new List<MetricConfig>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            string aggregateResult = "1 X1";
            int totalMetricsWithData = 2;

            // Act
            string result = globalStats.ComputeExpectedResult(aggregateResult, totalMetricsWithData);

            // Assert
            Assert.AreEqual(result, "1X");
        }

        [Test]
        public void ComputeExpectedResultMin1ResultsNoneMeeting()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);
            List<MetricConfig> configs = new List<MetricConfig>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            string aggregateResult = "1 2";
            int totalMetricsWithData = 2;

            // Act
            string result = globalStats.ComputeExpectedResult(aggregateResult, totalMetricsWithData);

            // Assert
            Assert.AreEqual(result, "");
        }

        [Test]
        public void ComputeExpectedResultMin05ResultsSplit()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(0.5);
            List<MetricConfig> configs = new List<MetricConfig>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            string aggregateResult = "1 X";
            int totalMetricsWithData = 2;

            // Act
            string result = globalStats.ComputeExpectedResult(aggregateResult, totalMetricsWithData);

            // Assert
            Assert.AreEqual(result, "1X");
        }

        [Test]
        public void ComputeExpectedResultMin1ResultsXButAdds1()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);
            List<MetricConfig> configs = new List<MetricConfig>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            string aggregateResult = "X X1";
            int totalMetricsWithData = 2;

            // Act
            string result = globalStats.ComputeExpectedResult(aggregateResult, totalMetricsWithData);

            // Assert
            Assert.AreEqual(result, "1X");
        }

        [Test]
        public void ComputeExpectedResultMin1Results1()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);
            List<MetricConfig> configs = new List<MetricConfig>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            string aggregateResult = "1 1";
            int totalMetricsWithData = 2;

            // Act
            string result = globalStats.ComputeExpectedResult(aggregateResult, totalMetricsWithData);

            // Assert
            Assert.AreEqual(result, "1");
        }

        [Test]
        public void ComputeExpectedResultMin1Results12()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);
            List<MetricConfig> configs = new List<MetricConfig>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            string aggregateResult = "1 2";
            int totalMetricsWithData = 2;

            // Act
            string result = globalStats.ComputeExpectedResult(aggregateResult, totalMetricsWithData);

            // Assert
            Assert.AreEqual(result, "");
        }

        [Test]
        public void GetMatchdayProfitAll()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("all");
            List<MetricConfig> configs = new List<MetricConfig>();
            List<double> matchdayOdds = new List<double>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            double oddGame1 = 2;
            double oddGame2 = 3;
            double oddGame3 = 4;
            double oddGame4 = 5;
            matchdayOdds.Add(oddGame1);
            matchdayOdds.Add(oddGame2);
            matchdayOdds.Add(oddGame3);
            matchdayOdds.Add(oddGame4);


            // Act
            double result = globalStats.GetMatchdayProfit(matchdayOdds);

            // Assert
            Assert.AreEqual(result, (oddGame1 - 1) +
                                    (oddGame2 - 1) +
                                    (oddGame3 - 1) +
                                    (oddGame4 - 1) +
                                    ((oddGame1 * oddGame2) - 1) +
                                    ((oddGame1 * oddGame3) - 1) +
                                    ((oddGame1 * oddGame4) - 1) +
                                    ((oddGame2 * oddGame3) - 1) +
                                    ((oddGame2 * oddGame4) - 1) +
                                    ((oddGame3 * oddGame4) - 1) +
                                    ((oddGame1 * oddGame2 * oddGame3) - 1) +
                                    ((oddGame1 * oddGame2 * oddGame4) - 1) +
                                    ((oddGame1 * oddGame3 * oddGame4) - 1) +
                                    ((oddGame2 * oddGame3 * oddGame4) - 1) +
                                    ((oddGame1 * oddGame2 * oddGame3 * oddGame4) - 1));
        }

        [Test]
        public void GetMatchdayProfitMax()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("max");
            List<MetricConfig> configs = new List<MetricConfig>();
            List<double> matchdayOdds = new List<double>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            double oddGame1 = 2;
            double oddGame2 = 3;
            double oddGame3 = 4;
            matchdayOdds.Add(oddGame1);
            matchdayOdds.Add(oddGame2);
            matchdayOdds.Add(oddGame3);

            // Act
            double result = globalStats.GetMatchdayProfit(matchdayOdds);

            // Assert
            Assert.AreEqual(result, ((oddGame1 * oddGame2 * oddGame3) - 1));
        }

        [Test]
        public void GetMatchdayProfit1()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            List<MetricConfig> configs = new List<MetricConfig>();
            List<double> matchdayOdds = new List<double>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            double oddGame1 = 2;
            double oddGame2 = 3;
            double oddGame3 = 4;
            matchdayOdds.Add(oddGame1);
            matchdayOdds.Add(oddGame2);
            matchdayOdds.Add(oddGame3);

            // Act
            double result = globalStats.GetMatchdayProfit(matchdayOdds);

            // Assert
            Assert.AreEqual(result, (oddGame1 - 1) + (oddGame2 - 1) + (oddGame3 - 1));
        }

        [Test]
        public void GetMatchdayProfit12()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("12");
            List<MetricConfig> configs = new List<MetricConfig>();
            List<double> matchdayOdds = new List<double>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            double oddGame1 = 2;
            double oddGame2 = 3;
            double oddGame3 = 4;
            matchdayOdds.Add(oddGame1);
            matchdayOdds.Add(oddGame2);
            matchdayOdds.Add(oddGame3);

            // Act
            double result = globalStats.GetMatchdayProfit(matchdayOdds);

            // Assert
            Assert.AreEqual(result, (oddGame1 - 1) + 
                                    (oddGame2 - 1) + 
                                    (oddGame3 - 1) +
                                    (oddGame1 * oddGame2 - 1) + 
                                    (oddGame2 * oddGame3 - 1) + 
                                    (oddGame3 * oddGame1 - 1));
        }

        [Test]
        public void GetMatchdayProfit123()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("123");
            List<MetricConfig> configs = new List<MetricConfig>();
            List<double> matchdayOdds = new List<double>();
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            double oddGame1 = 2;
            double oddGame2 = 3;
            double oddGame3 = 4;
            matchdayOdds.Add(oddGame1);
            matchdayOdds.Add(oddGame2);
            matchdayOdds.Add(oddGame3);

            // Act
            double result = globalStats.GetMatchdayProfit(matchdayOdds);

            // Assert
            Assert.AreEqual(result, (oddGame1 - 1) + 
                                    (oddGame2 - 1) + 
                                    (oddGame3 - 1) +
                                    (oddGame1 * oddGame2 - 1) +
                                    (oddGame2 * oddGame3 - 1) + 
                                    (oddGame3 * oddGame1 - 1) +
                                    (oddGame1 * oddGame2 * oddGame3 - 1));
        }
    }
}
