using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.DataModel
{
    struct Score
    {
        public int homeTeamGoals;
        public int awayTeamGoals;
    }
    struct Fixture
    {
        public string homeTeamName;
        public string awayTeamName;
        public int homeTeamId;
        public int awayTeamId;
        public DateTime date;
        public bool finished;
        public Score finalScore;
        public Score halfScore;
    }
}
