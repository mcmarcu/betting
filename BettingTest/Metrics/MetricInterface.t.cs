﻿using Betting.Config;
using Betting.DataModel;
using Betting.Metrics;
using Betting.Stats;
using Moq;
using NUnit.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BettingTest
{
    public class MetricInterfaceTest
    {
        private Mock<ConfigManagerInterface> configManagerMock;
        private Mock<FixtureRetrieverInterface> fixtureRetrieverMock;
        private string team;
        private int year;
        private Fixture actualFixture;
        private List<Fixture> fixturesTeam;


        [SetUp]
        public void Setup()
        {
            year = 0;
            team = "team";

            fixturesTeam = new List<Fixture>();

            Fixture firstGame = new Fixture
            {
                homeTeamName = team,
                awayTeamName = "firstGame"
            };
            firstGame.finalScore.homeTeamGoals = 1;
            firstGame.finalScore.awayTeamGoals = 0;
            firstGame.coeficient.homeTeam = 1;
            firstGame.coeficient.awayTeam = 1;

            fixturesTeam.Add(firstGame);

            Fixture secondGame = new Fixture
            {
                homeTeamName = team,
                awayTeamName = "secondGame"
            };
            secondGame.finalScore.homeTeamGoals = 0;
            secondGame.finalScore.awayTeamGoals = 0;
            secondGame.coeficient.homeTeam = 1;
            secondGame.coeficient.awayTeam = 1;

            fixturesTeam.Add(secondGame);

            actualFixture = new Fixture
            {
                homeTeamName = team,
                awayTeamName = "thirdGame"
            };
            actualFixture.finalScore.homeTeamGoals = 1;
            actualFixture.finalScore.awayTeamGoals = 2;
            actualFixture.coeficient.homeTeam = 1;
            actualFixture.coeficient.awayTeam = 1;

            fixturesTeam.Add(actualFixture);

            fixtureRetrieverMock = new Mock<FixtureRetrieverInterface>();
            configManagerMock = new Mock<ConfigManagerInterface>();
        }

        [Test]
        public void GetFixturesDepth1()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 1
            };
            MetricInterface metric = new LastGamesMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            int fixtureIdx = metric.FindFixtures(fixturesTeam, actualFixture.fixtureId, 1);

            // Assert
            Assert.AreEqual(fixtureIdx, 1);
            Assert.AreEqual(fixturesTeam[fixtureIdx].homeTeamName, team);
            Assert.AreEqual(fixturesTeam[fixtureIdx].awayTeamName, "secondGame");
        }

        [Test]
        public void GetFixturesDepth2()
        {
            // Arrange
            MetricConfig metricConfig = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 1
            };
            MetricInterface metric = new LastGamesMetric(metricConfig, year, configManagerMock.Object, fixtureRetrieverMock.Object);

            // Act
            int fixtureIdx = metric.FindFixtures(fixturesTeam, actualFixture.fixtureId, 2);

            // Assert
            Assert.AreEqual(fixtureIdx, 1);
            Assert.AreEqual(fixturesTeam[fixtureIdx].homeTeamName, team);
            Assert.AreEqual(fixturesTeam[fixtureIdx].awayTeamName, "secondGame");
            Assert.AreEqual(fixturesTeam[fixtureIdx-1].homeTeamName, team);
            Assert.AreEqual(fixturesTeam[fixtureIdx-1].awayTeamName, "firstGame");
        }
    }
}
