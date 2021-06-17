using System;
using System.Collections.Generic;
using System.Linq;

using BotInterface.Bot;
using BotInterface.Game;

using static WinningBot.Constants;
using WinningBot.Predictors;
using WinningBot.Sequences;
using static WinningBot.Predictors.PredictingMethods;

namespace WinningBot
{
    public class WinProgram : IBot
    {
        GamePredictor GamePredictor = new GamePredictor(Scorers.AllScorers, 2);

        // Mapping between characters and the interface defined for IBot
        public static Dictionary<char, Move> CharToMove = new Dictionary<char, Move>
        {
            {R, Move.R }, {P, Move.P }, {S, Move.S }, {D, Move.D }, {W, Move.W },
        };
        public static Dictionary<Move, char> MoveToChar = new Dictionary<Move, char>
        {
            { Move.R, R }, { Move.P, P }, { Move.S, S }, { Move.D, D }, { Move.W, W },
        };

        public WinProgram()
        {
            // TODO: Is this actually useful?
            AllMovePredictors.Shuffle();
            AllClassPredictors.Shuffle();
            AllDrawPredictors.Shuffle();
        }

        public Move MakeMove(Gamestate gamestate)
        {
            int numRounds = gamestate.GetRounds().Length;
            if (numRounds > 0)
            {
                Round lastRound = gamestate.GetRounds()[numRounds - 1];
                char p1 = MoveToChar[lastRound.GetP1()];
                char p2 = MoveToChar[lastRound.GetP2()];
                GamePredictor.Update(p1, p2);
            }
            return CharToMove[GamePredictor.DynamiteController.ApplyDynamiteCheck(GamePredictor.GetNextMove())];
        }
    }
}
