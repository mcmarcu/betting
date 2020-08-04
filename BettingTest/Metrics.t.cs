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
    public class MetricTest
    {
        private Mock<ConfigManagerInterface> configManagerMock;
        private Mock<FixtureRetrieverInterface> fixtureRetrieverMock;
        private String team1;
        private String team2;
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

            Fixture fixTeam11 = new Fixture();
            fixTeam11.homeTeamName = team1;
            fixTeam11.awayTeamName = "";
            fixTeam11.finalScore.homeTeamGoals = 1;
            fixTeam11.finalScore.awayTeamGoals = 0;
            fixTeam11.coeficient.homeTeam = 1;
            fixTeam11.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixTeam11);

            Fixture fixTeam12 = new Fixture();
            fixTeam12.homeTeamName = team1;
            fixTeam12.awayTeamName = "";
            fixTeam12.finalScore.homeTeamGoals = 0;
            fixTeam12.finalScore.awayTeamGoals = 0;
            fixTeam12.coeficient.homeTeam = 1;
            fixTeam12.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixTeam12);

            Fixture fixTeam21 = new Fixture();
            fixTeam21.homeTeamName = "";
            fixTeam21.awayTeamName = team2;
            fixTeam21.finalScore.homeTeamGoals = 0;
            fixTeam21.finalScore.awayTeamGoals = 2;
            fixTeam21.coeficient.homeTeam = 1;
            fixTeam21.coeficient.awayTeam = 1;

            fixturesTeam2.Add(fixTeam21);

            Fixture fixTeam22 = new Fixture();
            fixTeam22.homeTeamName = "";
            fixTeam22.awayTeamName = team2;
            fixTeam22.finalScore.homeTeamGoals = 0;
            fixTeam22.finalScore.awayTeamGoals = 0;
            fixTeam22.coeficient.homeTeam = 1;
            fixTeam22.coeficient.awayTeam = 1;

            fixturesTeam2.Add(fixTeam22);

            actualFixture = new Fixture();
            actualFixture.homeTeamName = team1;
            actualFixture.awayTeamName = team2;
            actualFixture.finalScore.homeTeamGoals = 0;
            actualFixture.finalScore.awayTeamGoals = 0;
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
        public void GoalsScoredMetric_GetPointsDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsScoredMetric",
                depth = 1
            };
            GoalsScoredMetric metric = new GoalsScoredMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 0);
            Assert.AreEqual(pTeam2, 0);
        }

        [Test]
        public void GoalsScoredMetric_GetPointsDepth2()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsScoredMetric",
                depth = 2
            };
            GoalsScoredMetric metric = new GoalsScoredMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 1);
            Assert.AreEqual(pTeam2, 2);
        }

        [Test]
        public void GoalsScoredMetric_GetPercentageDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsScoredMetric",
                depth = 1
            };
            GoalsScoredMetric metric = new GoalsScoredMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPercentage(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 50);
            Assert.AreEqual(pTeam2, 50);
        }

        [Test]
        public void GoalsScoredMetric_GetPercentageDepth2()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsScoredMetric",
                depth = 2
            };
            GoalsScoredMetric metric = new GoalsScoredMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPercentage(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 33);
            Assert.AreEqual(pTeam2, 67);
        }

        [Test]
        public void GoalsConcededMetric_GetPointsDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsConcededMetric",
                depth = 1
            };
            GoalsConcededMetric metric = new GoalsConcededMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out int pTeam1, out int pTeam2, team1, team2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 0);
            Assert.AreEqual(pTeam2, 0);
        }
    }
}
