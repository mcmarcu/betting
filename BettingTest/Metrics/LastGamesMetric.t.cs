﻿using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Betting.Stats;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace BettingTest
{
    public class LastGamesMetricTest
    {
        private Mock<ConfigManagerInterface> configManagerMock;
        private Mock<FixtureRetrieverInterface> fixtureRetrieverMock;
        private string team1;
        private string team2;
        private int year;
        private Fixture actualFixture;

        [SetUp]
        public void Setup()
        {
            year = 0;
            team1 = "team1";
            team2 = "team2";

            List<Fixture> fixturesTeam1 = new List<Fixture>();
            List<Fixture> fixturesTeam2 = new List<Fixture>();

            Fixture fixTeam11 = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = ""
            };
            fixTeam11.finalScore.homeTeamGoals = 1;
            fixTeam11.finalScore.awayTeamGoals = 2;
            fixTeam11.coeficient.homeTeam = 1;
            fixTeam11.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixTeam11);

            Fixture fixTeam12 = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = ""
            };
            fixTeam12.finalScore.homeTeamGoals = 0;
            fixTeam12.finalScore.awayTeamGoals = 0;
            fixTeam12.coeficient.homeTeam = 1;
            fixTeam12.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixTeam12);

            Fixture fixTeam21 = new Fixture
            {
                homeTeamName = "",
                awayTeamName = team2
            };
            fixTeam21.finalScore.homeTeamGoals = 0;
            fixTeam21.finalScore.awayTeamGoals = 2;
            fixTeam21.coeficient.homeTeam = 1;
            fixTeam21.coeficient.awayTeam = 1;

            fixturesTeam2.Add(fixTeam21);

            Fixture fixTeam22 = new Fixture
            {
                homeTeamName = "",
                awayTeamName = team2
            };
            fixTeam22.finalScore.homeTeamGoals = 0;
            fixTeam22.finalScore.awayTeamGoals = 0;
            fixTeam22.coeficient.homeTeam = 1;
            fixTeam22.coeficient.awayTeam = 1;

            fixturesTeam2.Add(fixTeam22);

            actualFixture = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = team2
            };
            actualFixture.finalScore.homeTeamGoals = 1;
            actualFixture.finalScore.awayTeamGoals = 2;
            actualFixture.coeficient.homeTeam = 1;
            actualFixture.coeficient.awayTeam = 1;

            fixturesTeam1.Add(actualFixture);
            fixturesTeam2.Add(actualFixture);

            fixtureRetrieverMock = new Mock<FixtureRetrieverInterface>();
            fixtureRetrieverMock.Setup(p => p.GetAllFixtures(year, team1)).Returns(fixturesTeam1);
            fixtureRetrieverMock.Setup(p => p.GetAllFixtures(year, team2)).Returns(fixturesTeam2);

            configManagerMock = new Mock<ConfigManagerInterface>();
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
        }

        [Test]
        public void GetPointsDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 1
            };
            LastGamesMetric metric = new LastGamesMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 1);
            Assert.AreEqual(pTeam2, 1);
        }

        [Test]
        public void GetPointsDepth2()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 2
            };
            LastGamesMetric metric = new LastGamesMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 1);
            Assert.AreEqual(pTeam2, 4);
        }

        [Test]
        public void GetPercentageDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 1
            };
            LastGamesMetric metric = new LastGamesMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPercentage(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 50);
            Assert.AreEqual(pTeam2, 50);
        }

        [Test]
        public void GetPercentageDepth2()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 2
            };
            LastGamesMetric metric = new LastGamesMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPercentage(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 20);
            Assert.AreEqual(pTeam2, 80);
        }

        [Test]
        public void GetPoints()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 2
            };
            LastGamesMetric metric = new LastGamesMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            int pointsTeam1 = metric.GetPoints(actualFixture, team1);
            int pointsTeam2 = metric.GetPoints(actualFixture, team2);

            // Assert
            Assert.AreEqual(pointsTeam1, 0);
            Assert.AreEqual(pointsTeam2, 3);
        }
    }
}