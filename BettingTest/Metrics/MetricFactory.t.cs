using System;
using Betting.Metrics;
using Betting.DataModel;
using Betting.Stats;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Betting.Config;

namespace BettingTest
{
    public class MetricFactoryTest
    {
        private Mock<ConfigManagerInterface> configManagerMock;
        private Mock<FixtureRetrieverInterface> fixtureRetrieverMock;
        private int year;

        [SetUp]
        public void Setup()
        {
            year = 1987;
            fixtureRetrieverMock = new Mock<FixtureRetrieverInterface>();
            configManagerMock = new Mock<ConfigManagerInterface>();
        }

        [Test]
        public void GetMetricsCheckSize()
        {
            // Arrange
            MetricConfig metricConfigLastGames = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 1
            };
            MetricConfig metricConfigGoalsScored = new MetricConfig
            {
                name = "GoalsScoredMetric",
                depth = 2
            };

            List<MetricConfig> configs = new List<MetricConfig>
            {
                metricConfigLastGames,
                metricConfigGoalsScored
            };

            // Act
            List<MetricInterface> metrics = MetricFactory.GetMetrics(configs, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Assert
            Assert.AreEqual(metrics.Count, 2);
        }

        [Test]
        public void GetMetricsCheckType()
        {
            // Arrange
            MetricConfig metricConfigLastGames = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 1
            };

            List<MetricConfig> configs = new List<MetricConfig>
            {
                metricConfigLastGames
            };

            // Act
            List<MetricInterface> metrics = MetricFactory.GetMetrics(configs, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Assert
            Assert.AreEqual(metrics.Count, 1);
            Assert.AreEqual(metrics[0].GetType(), typeof(LastGamesMetric));
            Assert.AreEqual(metrics[0].year, year);
        }


        [Test]
        public void GetMetricsCheckWrongType()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "LastGamesMetricMisspelled",
                depth = 1
            };

            List<MetricConfig> configs = new List<MetricConfig>
            {
                metricConfig
            };

            // Act
            // Assert
            Assert.Throws(typeof(ArgumentException), delegate { MetricFactory.GetMetrics(configs, year, configManagerMock.Object, fixtureRetrieverMock.Object); });
        }
    }
}
