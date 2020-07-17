using System;
using System.Collections.Generic;
using BotInterface.Bot;
using BotInterface.Game;

namespace Dud {
    public class Class1 : IBot {
        public static List<Move> MovesRPS = new List<Move> {Move.R, Move.P, Move.S};
        
        public static Random random = new Random();

        public static Move Choose(List<Move> choices) {
            return choices[random.Next(choices.Count)];
        }
        
        public Move MakeMove(Gamestate gamestate) {
            return Choose(MovesRPS);
        }
    }
}