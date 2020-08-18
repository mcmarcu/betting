using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Moq;
using NUnit.Framework;

namespace BettingTest
{
    public class ResultCheckerTest
    {
        private Mock<ConfigManagerInterface> configManagerMock;
        private Mock<MetricInterface> metricInterfaceMock;
        private string team1;
        private string team2;
        private int teamId1;
        private int teamId2;
        private Fixture actualFixture;

        [SetUp]
        public void Setup()
        {
            team1 = "team1";
            team2 = "team2";
            teamId1 = 1;
            teamId2 = 2;

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

            configManagerMock = new Mock<ConfigManagerInterface>();
            metricInterfaceMock = new Mock<MetricInterface>();
        }

        [Test]
        public void GetExpectedResultX()
        {
            // Arrange
            int pctTeam1 = 55;
            int pctTeam2 = 45;
            metricInterfaceMock.Setup(p => p.GetPercentage(out pctTeam1, out pctTeam2, teamId1, teamId2, actualFixture));
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(40);
            ResultChecker resultChecker = new ResultChecker(metricInterfaceMock.Object, actualFixture, configManagerMock.Object);

            // Act
            string expectedResult = resultChecker.GetExpectedResult();

            // Assert
            Assert.AreEqual(expectedResult, "X");
            Assert.IsTrue(resultChecker.dataAvailable);
        }

        [Test]
        public void GetExpectedResult1()
        {
            // Arrange
            int pctTeam1 = 90;
            int pctTeam2 = 10;
            metricInterfaceMock.Setup(p => p.GetPercentage(out pctTeam1, out pctTeam2, teamId1, teamId2, actualFixture));
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(40);
            ResultChecker resultChecker = new ResultChecker(metricInterfaceMock.Object, actualFixture, configManagerMock.Object);

            // Act
            string expectedResult = resultChecker.GetExpectedResult();

            // Assert
            Assert.AreEqual(expectedResult, "1");
            Assert.IsTrue(resultChecker.dataAvailable);
        }

        [Test]
        public void GetExpectedResult2()
        {
            // Arrange
            int pctTeam1 = 10;
            int pctTeam2 = 90;
            metricInterfaceMock.Setup(p => p.GetPercentage(out pctTeam1, out pctTeam2, teamId1, teamId2, actualFixture));
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(40);
            ResultChecker resultChecker = new ResultChecker(metricInterfaceMock.Object, actualFixture, configManagerMock.Object);

            // Act
            string expectedResult = resultChecker.GetExpectedResult();

            // Assert
            Assert.AreEqual(expectedResult, "2");
            Assert.IsTrue(resultChecker.dataAvailable);
        }

        [Test]
        public void GetExpectedResult1X()
        {
            // Arrange
            int pctTeam1 = 65;
            int pctTeam2 = 35;
            metricInterfaceMock.Setup(p => p.GetPercentage(out pctTeam1, out pctTeam2, teamId1, teamId2, actualFixture));
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(40);
            ResultChecker resultChecker = new ResultChecker(metricInterfaceMock.Object, actualFixture, configManagerMock.Object);

            // Act
            string expectedResult = resultChecker.GetExpectedResult();

            // Assert
            Assert.AreEqual(expectedResult, "1X");
            Assert.IsTrue(resultChecker.dataAvailable);
        }

        [Test]
        public void GetExpectedResultX2()
        {
            // Arrange
            int pctTeam1 = 35;
            int pctTeam2 = 65;
            metricInterfaceMock.Setup(p => p.GetPercentage(out pctTeam1, out pctTeam2, teamId1, teamId2, actualFixture));
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(40);
            ResultChecker resultChecker = new ResultChecker(metricInterfaceMock.Object, actualFixture, configManagerMock.Object);

            // Act
            string expectedResult = resultChecker.GetExpectedResult();

            // Assert
            Assert.AreEqual(expectedResult, "X2");
            Assert.IsTrue(resultChecker.dataAvailable);
        }

    }
}
