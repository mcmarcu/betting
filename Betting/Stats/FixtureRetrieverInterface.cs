using Betting.DataModel;
using System.Collections.Generic;

namespace Betting.Stats
{
    public abstract class FixtureRetrieverInterface
    {
        public abstract int GetNumberOfMatchDays(int year);
        public abstract int GetGamesPerMatchDay(int year);
        public abstract List<Fixture> GetAllFixtures(int year, int teamId);
        public abstract List<Fixture> GetAllFixtures(int year);
        public abstract List<Fixture> GetRound(int year, int matchDay);
        public void GetPrevRound(ref int year, ref int day)
        {
            if (day == 1)
            {
                day = GetNumberOfMatchDays(year - 1);
                year--;
            }
            else
            {
                day--;
            }
        }

        public abstract int FindFixtureIndex(int year, int teamId, int fixtureId);
    }
}
