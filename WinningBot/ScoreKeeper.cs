using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static WinningBot.Constants;

namespace WinningBot
{
    class ScoreKeeper
    {
        public int MyScore = 0;
        public int YourScore = 0;
        public int PointsForRound = 0;

        public void Update(char MyMove, char YourMove)
        {
            PointsForRound++;
            if (RESULT[MyMove][YourMove] == 1)
            {
                MyScore += PointsForRound;
                PointsForRound = 0;
            } else if (RESULT[YourMove][MyMove] == 1)
            {
                YourScore += PointsForRound;
                PointsForRound = 0;
            }
        }

        public int GetTotalScore()
        {
            return MyScore + YourScore;
        }

        public int GetHighScore()
        {
            return MyScore > YourScore ? MyScore : YourScore;
        }
    }
}
