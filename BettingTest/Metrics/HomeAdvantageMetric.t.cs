﻿using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Betting.Stats;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace BettingTest
{
    public class HomeAdvantageMetricTest
    {
        private Mock<ConfigManagerInterface> configManagerMock;
        private Mock<FixtureRetrieverInterface> fixtureRetrieverMock;
        private string team1;
        private string team2;
        private int teamId1;
        private int teamId2;
        private int year;
        private Fixture actualFixture;

        [SetUp]
        public void Setup()
        {
            year = 0;
            team1 = "team1";
            team2 = "team2";
            teamId1 = 1;
            teamId2 = 2;

            List<Fixture> fixturesTeam1 = new List<Fixture>();
            List<Fixture> fixturesTeam2 = new List<Fixture>();

            Fixture fixTeam11 = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = "",
                homeTeamId = teamId1,
                awayTeamId = -1,
                fixtureId = 1,
            };
            fixTeam11.finalScore.homeTeamGoals = 1;
            fixTeam11.finalScore.awayTeamGoals = 2;
            fixTeam11.coeficient.homeTeam = 1;
            fixTeam11.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixTeam11);

            Fixture fixTeam1Ignored = new Fixture
            {
                homeTeamName = "",
                awayTeamName = team1,
                homeTeamId = -1,
                awayTeamId = teamId1,
                fixtureId = 2,
            };
            fixTeam1Ignored.finalScore.homeTeamGoals = 0;
            fixTeam1Ignored.finalScore.awayTeamGoals = 2;
            fixTeam1Ignored.coeficient.homeTeam = 1;
            fixTeam1Ignored.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixTeam1Ignored);//one
            fixturesTeam1.Add(fixTeam1Ignored);//two

            Fixture fixTeam12 = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = "",
                homeTeamId = teamId1,
                awayTeamId = -1,
                fixtureId = 3,
            };
            fixTeam12.finalScore.homeTeamGoals = 0;
            fixTeam12.finalScore.awayTeamGoals = 0;
            fixTeam12.coeficient.homeTeam = 1;
            fixTeam12.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixTeam12);

            Fixture fixTeam21 = new Fixture
            {
                homeTeamName = "",
                awayTeamName = team2,
                homeTeamId = -1,
                awayTeamId = teamId2,
                fixtureId = 4,
            };
            fixTeam21.finalScore.homeTeamGoals = 0;
            fixTeam21.finalScore.awayTeamGoals = 2;
            fixTeam21.coeficient.homeTeam = 1;
            fixTeam21.coeficient.awayTeam = 1;

            fixturesTeam2.Add(fixTeam21);

            Fixture fixTeam2Ignored = new Fixture
            {
                homeTeamName = team2,
                awayTeamName = "",
                homeTeamId = teamId2,
                awayTeamId = -1,
                fixtureId = 5,
            };
            fixTeam2Ignored.finalScore.homeTeamGoals = 0;
            fixTeam2Ignored.finalScore.awayTeamGoals = 2;
            fixTeam2Ignored.coeficient.homeTeam = 1;
            fixTeam2Ignored.coeficient.awayTeam = 1;

            fixturesTeam2.Add(fixTeam2Ignored);//one
            fixturesTeam2.Add(fixTeam2Ignored);//two

            Fixture fixTeam22 = new Fixture
            {
                homeTeamName = "",
                awayTeamName = team2,
                homeTeamId = -1,
                awayTeamId = teamId2,
                fixtureId = 6,
            };
            fixTeam22.finalScore.homeTeamGoals = 0;
            fixTeam22.finalScore.awayTeamGoals = 0;
            fixTeam22.coeficient.homeTeam = 1;
            fixTeam22.coeficient.awayTeam = 1;

            fixturesTeam2.Add(fixTeam22);

            actualFixture = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = team2,
                homeTeamId = teamId1,
                awayTeamId = teamId2,
                fixtureId = 7,
            };
            actualFixture.finalScore.homeTeamGoals = 1;
            actualFixture.finalScore.awayTeamGoals = 2;
            actualFixture.coeficient.homeTeam = 1;
            actualFixture.coeficient.awayTeam = 1;

            fixturesTeam1.Add(actualFixture);
            fixturesTeam2.Add(actualFixture);

            fixtureRetrieverMock = new Mock<FixtureRetrieverInterface>();
            fixtureRetrieverMock.Setup(p => p.GetAllFixtures(year, teamId1)).Returns(fixturesTeam1);
            fixtureRetrieverMock.Setup(p => p.GetAllFixtures(year, teamId2)).Returns(fixturesTeam2);

            fixtureRetrieverMock.Setup(p => p.FindFixtureIndex(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int year, int teamId, int fixtureId) =>
                {
                    List<Fixture> teamFixtures = fixtureRetrieverMock.Object.GetAllFixtures(year, teamId);
                    return teamFixtures.FindLastIndex(thisFix => thisFix.fixtureId == fixtureId);
                });

            configManagerMock = new Mock<ConfigManagerInterface>();
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
        }

        [Test]
        public void GetPointsDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "HomeAdvantageMetric",
                depth = 1
            };
            HomeAdvantageMetric metric = new HomeAdvantageMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out double pTeam1, out double pTeam2, teamId1, teamId2, actualFixture);

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
                name = "HomeAdvantageMetric",
                depth = 2
            };
            HomeAdvantageMetric metric = new HomeAdvantageMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out double pTeam1, out double pTeam2, teamId1, teamId2, actualFixture);

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
                name = "HomeAdvantageMetric",
                depth = 1
            };
            HomeAdvantageMetric metric = new HomeAdvantageMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPercentage(out int pTeam1, out int pTeam2, teamId1, teamId2, actualFixture);

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
                name = "HomeAdvantageMetric",
                depth = 2
            };
            HomeAdvantageMetric metric = new HomeAdvantageMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPercentage(out int pTeam1, out int pTeam2, teamId1, teamId2, actualFixture);

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
                name = "HomeAdvantageMetric",
                depth = 2
            };
            HomeAdvantageMetric metric = new HomeAdvantageMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            double pointsTeam1 = metric.GetPoints(actualFixture, teamId1);
            double pointsTeam2 = metric.GetPoints(actualFixture, teamId2);

            // Assert
            Assert.AreEqual(pointsTeam1, 0);
            Assert.AreEqual(pointsTeam2, 3);
        }
    }
}
