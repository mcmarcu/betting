using System.Collections.Generic;
using Betting.DataModel;

namespace Betting.Stats
{
    public abstract class FixtureRetrieverInterface
    {
        public abstract int GetNumberOfMatchDays(int year);
        public abstract int GetGamesPerMatchDay(int year);
        public abstract List<Fixture> GetAllFixtures(int year, string team);
        public abstract List<Fixture> GetAllFixtures(int year);
        public abstract List<Fixture> GetRound(int year, int matchDay);
        public abstract void GetPrevRound(out int outYear, out int outDay, int currentYear, int currentDay);
    }
}
