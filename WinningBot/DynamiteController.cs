using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static WinningBot.Constants;
using static WinningBot.Predictors.PredictingMethods;

namespace WinningBot
{
    class DynamiteController
    {
        public ScoreKeeper ScoreKeeper = new ScoreKeeper();
        int DynamitesUsed = 0;

        // separated from DynamitesUsed to allow always leaving 1 spare
        int MyDynamitesLeft = MAX_DYNAMITE - 1;
        int YourDynamitesLeft = MAX_DYNAMITE;

        public void Update(char MyMove, char YourMove)
        {
            ScoreKeeper.Update(MyMove, YourMove);
            if (MyMove == D)
            {
                DynamitesUsed++;
                MyDynamitesLeft--;
            }
            if (YourMove == D)
            {
                YourDynamitesLeft--;
            }
        }

        public char ApplyDynamiteCheck(char move)
        {
            if (move == D && MyDynamitesLeft == 0 || move == W && YourDynamitesLeft == 0)
            {
                return RandomRPS();
            } else
            {
                return move;
            }
        }

        
        // TODO: Worth doing as doubles or just leave as ints?
        // at the current rate, how many dynamites left at end

        // TODO: Getting divide by 0 error
        public double EstimateDynamitesRemaining()
        {
            double proportionDone = (double)ScoreKeeper.GetHighScore() / WINNING_SCORE;
            if (proportionDone == 0)
            {
                return MAX_DYNAMITE;
            }
            return MAX_DYNAMITE - (DynamitesUsed / proportionDone);
        }

        // estimates the number of draws to go, related to how many of them to play dynamite on.
        public int EstimateDrawsRemaining()
        {
            if (ScoreKeeper.GetHighScore() == 0)
            {
                return WINNING_SCORE / 3;
            }
            return (ScoreKeeper.GetTotalScore() * ((WINNING_SCORE / ScoreKeeper.GetHighScore()) - 1)) / 3;
        }
    }
}
