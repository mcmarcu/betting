﻿using Betting.Config;
using Betting.DataModel;
using Betting.Stats;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace BettingTest
{
    public class GlobalStatsDataTest
    {
        private Mock<ConfigManagerInterface> configManagerMock;
        private Mock<FixtureRetrieverInterface> fixtureRetrieverMock;
        private Logger logger;
        private Dictionary<string, double> commonOdds;


        [SetUp]
        public void Setup()
        {
            configManagerMock = new Mock<ConfigManagerInterface>();
            fixtureRetrieverMock = new Mock<FixtureRetrieverInterface>();
            logger = new Logger(ConfigManagerInterface.LogLevel.LOG_DEBUG);

            string team1 = "team1";
            string team2 = "team2";
            string team3 = "team3";
            string team4 = "team4";

            int teamId1 = 1;
            int teamId2 = 2;
            int teamId3 = 3;
            int teamId4 = 4;

            commonOdds = new Dictionary<string, double>
            {
                { "1", 1.5 },
                { "X", 4 },
                { "2", 6.7 }
            };


            List<Fixture> fixturesTeam1 = new List<Fixture>();
            List<Fixture> fixturesTeam2 = new List<Fixture>();
            List<Fixture> fixturesTeam3 = new List<Fixture>();
            List<Fixture> fixturesTeam4 = new List<Fixture>();

            List<Fixture> allFixtures = new List<Fixture>();

            List<Fixture> fixturesRound1 = new List<Fixture>();
            List<Fixture> fixturesRound2 = new List<Fixture>();
            List<Fixture> fixturesRound3 = new List<Fixture>();
            List<Fixture> fixturesRound4 = new List<Fixture>();

            Fixture fixture;

            // ROUND 1
            fixture = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = team2,
                homeTeamId = teamId1,
                awayTeamId = teamId2,
                fixtureId = 1,
            };
            fixture.finalScore.homeTeamGoals = 0;
            fixture.finalScore.awayTeamGoals = 1;
            fixture.coeficient.homeTeam = 1;
            fixture.coeficient.awayTeam = 1;

            fixturesTeam1.Add(fixture);
            fixturesTeam2.Add(fixture);
            fixturesRound1.Add(fixture);
            allFixtures.Add(fixture);
            fixture.result = "2";
            fixture.odds = new Dictionary<string, double>(commonOdds);
            fixture.Init(configManagerMock.Object);

            fixture = new Fixture
            {
                homeTeamName = team3,
                awayTeamName = team4,
                homeTeamId = teamId3,
                awayTeamId = teamId4,
                fixtureId = 2,
            };
            fixture.finalScore.homeTeamGoals = 1;
            fixture.finalScore.awayTeamGoals = 1;
            fixture.coeficient.homeTeam = 1;
            fixture.coeficient.awayTeam = 1;
            fixture.result = "X";
            fixture.odds = new Dictionary<string, double>(commonOdds);
            fixture.Init(configManagerMock.Object);

            fixturesTeam3.Add(fixture);
            fixturesTeam4.Add(fixture);
            fixturesRound1.Add(fixture);
            allFixtures.Add(fixture);

            // ROUND 2
            fixture = new Fixture
            {
                homeTeamName = team1,
                awayTeamName = team3,
                homeTeamId = teamId1,
                awayTeamId = teamId3,
                fixtureId = 3,
            };
            fixture.finalScore.homeTeamGoals = 1;
            fixture.finalScore.awayTeamGoals = 0;
            fixture.coeficient.homeTeam = 1;
            fixture.coeficient.awayTeam = 1;
            fixture.result = "1";
            fixture.odds = new Dictionary<string, double>(commonOdds);
            fixture.Init(configManagerMock.Object);

            fixturesTeam1.Add(fixture);
            fixturesTeam3.Add(fixture);
            fixturesRound2.Add(fixture);
            allFixtures.Add(fixture);

            fixture = new Fixture
            {
                homeTeamName = team2,
                awayTeamName = team4,
                homeTeamId = teamId2,
                awayTeamId = teamId4,
                fixtureId = 4,
            };
            fixture.finalScore.homeTeamGoals = 1;
            fixture.finalScore.awayTeamGoals = 1;
            fixture.coeficient.homeTeam = 1;
            fixture.coeficient.awayTeam = 1;
            fixture.result = "X";
            fixture.odds = new Dictionary<string, double>(commonOdds);
            fixture.Init(configManagerMock.Object);

            fixturesTeam2.Add(fixture);
            fixturesTeam4.Add(fixture);
            fixturesRound2.Add(fixture);
            allFixtures.Add(fixture);

            // ROUND 3
            fixture = new Fixture
            {
                homeTeamName = team2,
                awayTeamName = team1,
                homeTeamId = teamId2,
                awayTeamId = teamId1,
                fixtureId = 5,
            };
            fixture.finalScore.homeTeamGoals = 0;
            fixture.finalScore.awayTeamGoals = 1;
            fixture.coeficient.homeTeam = 1;
            fixture.coeficient.awayTeam = 1;
            fixture.result = "2";
            fixture.odds = new Dictionary<string, double>(commonOdds);
            fixture.Init(configManagerMock.Object);

            fixturesTeam1.Add(fixture);
            fixturesTeam2.Add(fixture);
            fixturesRound3.Add(fixture);
            allFixtures.Add(fixture);

            fixture = new Fixture
            {
                homeTeamName = team4,
                awayTeamName = team3,
                homeTeamId = teamId4,
                awayTeamId = teamId3,
                fixtureId = 6,
            };
            fixture.finalScore.homeTeamGoals = 3;
            fixture.finalScore.awayTeamGoals = 2;
            fixture.coeficient.homeTeam = 1;
            fixture.coeficient.awayTeam = 1;
            fixture.result = "1";
            fixture.odds = new Dictionary<string, double>(commonOdds);
            fixture.Init(configManagerMock.Object);

            fixturesTeam3.Add(fixture);
            fixturesTeam4.Add(fixture);
            fixturesRound3.Add(fixture);
            allFixtures.Add(fixture);

            // ROUND 4
            fixture = new Fixture
            {
                homeTeamName = team3,
                awayTeamName = team1,
                homeTeamId = teamId3,
                awayTeamId = teamId1,
                fixtureId = 7,
            };
            fixture.finalScore.homeTeamGoals = 0;
            fixture.finalScore.awayTeamGoals = 1;
            fixture.coeficient.homeTeam = 1;
            fixture.coeficient.awayTeam = 1;
            fixture.result = "2";
            fixture.odds = new Dictionary<string, double>(commonOdds);
            fixture.Init(configManagerMock.Object);

            fixturesTeam1.Add(fixture);
            fixturesTeam3.Add(fixture);
            fixturesRound4.Add(fixture);
            allFixtures.Add(fixture);

            fixture = new Fixture
            {
                homeTeamName = team4,
                awayTeamName = team2,
                homeTeamId = teamId4,
                awayTeamId = teamId2,
                fixtureId = 8,
            };
            fixture.finalScore.homeTeamGoals = 0;
            fixture.finalScore.awayTeamGoals = 1;
            fixture.coeficient.homeTeam = 1;
            fixture.coeficient.awayTeam = 1;
            fixture.result = "2";
            fixture.odds = new Dictionary<string, double>(commonOdds);
            fixture.Init(configManagerMock.Object);

            fixturesTeam2.Add(fixture);
            fixturesTeam4.Add(fixture);
            fixturesRound4.Add(fixture);
            allFixtures.Add(fixture);

            int yearsToSetup = 4;
            for (int i = 0; i > -yearsToSetup; --i)
            {
                fixtureRetrieverMock.Setup(p => p.GetRound(i, 1)).Returns(fixturesRound1);
                fixtureRetrieverMock.Setup(p => p.GetRound(i, 2)).Returns(fixturesRound2);
                fixtureRetrieverMock.Setup(p => p.GetRound(i, 3)).Returns(fixturesRound3);
                fixtureRetrieverMock.Setup(p => p.GetRound(i, 4)).Returns(fixturesRound4);

                fixtureRetrieverMock.Setup(p => p.GetAllFixtures(i, teamId1)).Returns(fixturesTeam1);
                fixtureRetrieverMock.Setup(p => p.GetAllFixtures(i, teamId2)).Returns(fixturesTeam2);
                fixtureRetrieverMock.Setup(p => p.GetAllFixtures(i, teamId3)).Returns(fixturesTeam3);
                fixtureRetrieverMock.Setup(p => p.GetAllFixtures(i, teamId4)).Returns(fixturesTeam4);




                fixtureRetrieverMock.Setup(p => p.GetNumberOfMatchDays(i)).Returns(4);
                fixtureRetrieverMock.Setup(p => p.GetGamesPerMatchDay(i)).Returns(2);
            }

            fixtureRetrieverMock.Setup(p => p.FindFixtureIndex(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int year, int teamId, int fixtureId) =>
                {
                    List<Fixture> teamFixtures = fixtureRetrieverMock.Object.GetAllFixtures(year, teamId);
                    return teamFixtures.FindLastIndex(thisFix => thisFix.fixtureId == fixtureId);
                });

        }

        [Test]
        public void GetMatchdayDataNotEnoughData()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
            configManagerMock.Setup(p => p.GetMaxOdds()).Returns(10);
            configManagerMock.Setup(p => p.GetMinOdds()).Returns(1);
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(10);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);

            int matchDay = 1;
            int year = 0;

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
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            globalStats.GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, matchDay, year);

            // Assert
            Assert.AreEqual(correctFixturesWithData, 0);
            Assert.AreEqual(totalFixturesWithData, 0);
        }

        [Test]
        public void GetMatchdayDataBadResults()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
            configManagerMock.Setup(p => p.GetMaxOdds()).Returns(10);
            configManagerMock.Setup(p => p.GetMinOdds()).Returns(1);
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(10);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);

            int matchDay = 2;
            int year = 0;

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
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            globalStats.GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, matchDay, year);

            // Assert
            Assert.AreEqual(correctFixturesWithData, 0);
            Assert.AreEqual(totalFixturesWithData, 2);
            Assert.AreEqual(currentProfit, 0 - 1 - 1);
        }

        [Test]
        public void GetMatchdayDataCorrectResults()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
            configManagerMock.Setup(p => p.GetMaxOdds()).Returns(10);
            configManagerMock.Setup(p => p.GetMinOdds()).Returns(1);
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(10);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);

            int matchDay = 3;
            int year = 0;

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
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            globalStats.GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, matchDay, year);

            // Assert
            Assert.AreEqual(correctFixturesWithData, 2);
            Assert.AreEqual(totalFixturesWithData, 2);
            Assert.AreEqual(currentProfit, commonOdds["1"] - 1 + commonOdds["2"] - 1);
        }


        [Test]
        public void GetMatchdayDataMixedResults()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
            configManagerMock.Setup(p => p.GetMaxOdds()).Returns(10);
            configManagerMock.Setup(p => p.GetMinOdds()).Returns(1);
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(10);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);

            int matchDay = 4;
            int year = 0;

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
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            globalStats.GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, matchDay, year);

            // Assert
            Assert.AreEqual(correctFixturesWithData, 1);//just one is correct
            Assert.AreEqual(totalFixturesWithData, 2);
            Assert.AreEqual(currentProfit, commonOdds["2"] - 1 - 1);
        }

        [Test]
        public void GetMatchdayDataDepth()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
            configManagerMock.Setup(p => p.GetMaxOdds()).Returns(10);
            configManagerMock.Setup(p => p.GetMinOdds()).Returns(1);
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(10);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);

            int matchDay = 4;
            int year = 0;

            MetricConfig metricConfigLastGames = new MetricConfig
            {
                name = "LastGamesMetric",
                depth = 3
            };

            List<MetricConfig> configs = new List<MetricConfig>
            {
                metricConfigLastGames
            };

            // Act
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            globalStats.GetMatchdayData(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, matchDay, year);

            // Assert
            Assert.AreEqual(correctFixturesWithData, 1);//just one is correct
            Assert.AreEqual(totalFixturesWithData, 2);
            Assert.AreEqual(currentProfit, commonOdds["2"] - 1 - 1);
        }

        [Test]
        public void GetYearData()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
            configManagerMock.Setup(p => p.GetMaxOdds()).Returns(10);
            configManagerMock.Setup(p => p.GetMinOdds()).Returns(1);
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(10);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);
            configManagerMock.Setup(p => p.GetMatchDay()).Returns(4);
            configManagerMock.Setup(p => p.GetReverseDays()).Returns(4);

            int year = 0;

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
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            globalStats.GetYearData(out int correctFixturesWithData, out int totalFixturesWithData, out double currentProfit, year);

            // Assert
            Assert.AreEqual(correctFixturesWithData, 0 + 2 + 1);
            Assert.AreEqual(totalFixturesWithData, 2 + 2 + 2);
            Assert.AreEqual(currentProfit, (0 - 1 - 1)
                                           + (commonOdds["1"] - 1 + commonOdds["2"] - 1)
                                           + (commonOdds["2"] - 1 - 1));
        }

        [Test]
        public void GetAllYearsData()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
            configManagerMock.Setup(p => p.GetMaxOdds()).Returns(10);
            configManagerMock.Setup(p => p.GetMinOdds()).Returns(1);
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(10);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);
            configManagerMock.Setup(p => p.GetMatchDay()).Returns(4);
            configManagerMock.Setup(p => p.GetReverseDays()).Returns(4);
            configManagerMock.Setup(p => p.GetYear()).Returns(0);
            configManagerMock.Setup(p => p.GetReverseYears()).Returns(4);
            configManagerMock.Setup(p => p.GetLogLevel()).Returns(ConfigManager.LogLevel.LOG_DEBUG);
            configManagerMock.Setup(p => p.GetMinYearProfit()).Returns(70);



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
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            globalStats.GetAllYearsData(out bool success, out double rate, out double averageProfit);

            // Assert
            double correctFixturesWithDataPerYear = 0 + 2 + 1;
            double totalFixturesWithDataPerYear = 2 + 2 + 2;
            Assert.AreEqual(rate, correctFixturesWithDataPerYear / totalFixturesWithDataPerYear * 100);
            Assert.AreEqual(success, false);
            Assert.AreEqual(averageProfit, (0 - 1 - 1)
                                           + (commonOdds["1"] - 1 + commonOdds["2"] - 1)
                                           + (commonOdds["2"] - 1 - 1));
        }

        [Test]
        public void ProcessUpcomingFixtures()
        {
            // Arrange
            configManagerMock.Setup(p => p.GetBetStyle()).Returns("1");
            configManagerMock.Setup(p => p.GetUseExpanded()).Returns(false);
            configManagerMock.Setup(p => p.GetMaxOdds()).Returns(10);
            configManagerMock.Setup(p => p.GetMinOdds()).Returns(1);
            configManagerMock.Setup(p => p.GetDrawMargin()).Returns(10);
            configManagerMock.Setup(p => p.GetDrawMixedMargin()).Returns(20);
            configManagerMock.Setup(p => p.GetMinMetricCorrect()).Returns(1);
            configManagerMock.Setup(p => p.GetMatchDay()).Returns(3);
            configManagerMock.Setup(p => p.GetReverseDays()).Returns(4);

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
            GlobalStats globalStats = new GlobalStats(configs, configManagerMock.Object, fixtureRetrieverMock.Object, logger);
            globalStats.ProcessUpcomingFixtures(out double expectedProfit);

            // Assert
            Assert.AreEqual(expectedProfit, (commonOdds["1"] - 1)
                                          + (commonOdds["2"] - 1));
        }
    }
}
