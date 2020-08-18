using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Betting.Stats;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace BettingTest
{
    public class GoalsDifferenceMetricTest
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
                awayTeamId = -1
            };
            fixTeam11.finalScore.homeTeamGoals = 4;
            fixTeam11.finalScore.awayTeamGoals = 2;
            fixTeam11.coeficient.homeTeam = 1;
            fixTeam11.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixTeam11);

            Fixture fixTeam12 = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = "",
                homeTeamId = teamId1,
                awayTeamId = -1
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
                awayTeamId = teamId2
            };
            fixTeam21.finalScore.homeTeamGoals = 2;
            fixTeam21.finalScore.awayTeamGoals = 1;
            fixTeam21.coeficient.homeTeam = 1;
            fixTeam21.coeficient.awayTeam = 1;

            fixturesTeam2.Add(fixTeam21);

            Fixture fixTeam22 = new Fixture
            {
                homeTeamName = "",
                awayTeamName = team2,
                homeTeamId = -1,
                awayTeamId = teamId2
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
                awayTeamId = teamId2
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

            configManagerMock = new Mock<ConfigManagerInterface>();
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
        }

        [Test]
        public void GetPointsDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsDifferenceMetric",
                depth = 1
            };
            GoalsDifferenceMetric metric = new GoalsDifferenceMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out int pTeam1, out int pTeam2, teamId1, teamId2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 0);
            Assert.AreEqual(pTeam2, 0);
        }

        [Test]
        public void GetPointsDepth2()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsDifferenceMetric",
                depth = 2
            };
            GoalsDifferenceMetric metric = new GoalsDifferenceMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPoints(out int pTeam1, out int pTeam2, teamId1, teamId2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 2);
            Assert.AreEqual(pTeam2, -1);
        }

        [Test]
        public void GetPercentageDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsDifferenceMetric",
                depth = 1
            };
            GoalsDifferenceMetric metric = new GoalsDifferenceMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

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
                name = "GoalsDifferenceMetric",
                depth = 2
            };
            GoalsDifferenceMetric metric = new GoalsDifferenceMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            metric.GetPercentage(out int pTeam1, out int pTeam2, teamId1, teamId2, actualFixture);

            // Assert
            Assert.AreEqual(pTeam1, 200);
            Assert.AreEqual(pTeam2, -100);
        }

        [Test]
        public void GetScoredGoals()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsDifferenceMetric",
                depth = 2
            };
            GoalsDifferenceMetric metric = new GoalsDifferenceMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            int goalsScoredTeam1 = metric.GetScoredGoals(actualFixture, teamId1);
            int goalsScoredTeam2 = metric.GetScoredGoals(actualFixture, teamId2);

            // Assert
            Assert.AreEqual(goalsScoredTeam1, actualFixture.finalScore.homeTeamGoals);
            Assert.AreEqual(goalsScoredTeam2, actualFixture.finalScore.awayTeamGoals);
        }

        [Test]
        public void GetConcededGoals()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "GoalsDifferenceMetric",
                depth = 2
            };
            GoalsDifferenceMetric metric = new GoalsDifferenceMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            int goalsConcededTeam1 = metric.GetConcededGoals(actualFixture, teamId1);
            int goalsConcededTeam2 = metric.GetConcededGoals(actualFixture, teamId2);

            // Assert
            Assert.AreEqual(goalsConcededTeam1, actualFixture.finalScore.awayTeamGoals);
            Assert.AreEqual(goalsConcededTeam2, actualFixture.finalScore.homeTeamGoals);
        }

    }
}
