using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Bot;
using BotInterface.Game;

namespace DynamiteTest {
    internal class Program {
        
        private static Dictionary<Move, Dictionary<Move, int>> outcomes = new Dictionary<Move, Dictionary<Move, int>> {
            {Move.R, new Dictionary<Move, int> {{Move.R, 0}, {Move.P, -1}, {Move.S, 1}, {Move.D, -1}, {Move.W, 1}}},
            {Move.P, new Dictionary<Move, int> {{Move.R, 1}, {Move.P, 0}, {Move.S, -1}, {Move.D, -1}, {Move.W, 1}}},
            {Move.S, new Dictionary<Move, int> {{Move.R, -1}, {Move.P, 1}, {Move.S, 0}, {Move.D, -1}, {Move.W, 1}}},
            {Move.D, new Dictionary<Move, int> {{Move.R, 1}, {Move.P, 1}, {Move.S, 1}, {Move.D, 0}, {Move.W, -1}}},
            {Move.W, new Dictionary<Move, int> {{Move.R, -1}, {Move.P, -1}, {Move.S, -1}, {Move.D, 1}, {Move.W, 0}}}
        };

        public static int GetScore(Move m1, Move m2) {
            return outcomes[m1][m2];
        }

        public static void Main(string[] args) {
			
			// Set bots here
            IBot bot1 = new DynamiteBot.Program();
            IBot bot2 = new DyanmiteBot2.Class1();
			
			
            
            Gamestate game1 = new Gamestate();
            Gamestate game2 = new Gamestate();
            List<Round> rounds1 = new List<Round>();
            List<Round> rounds2 = new List<Round>();
            int bot1D = 100;
            int bot2D = 100;
            int bot1Wins = 0;
            int bot2Wins = 0;
            int value = 1;
            for (int i = 0; i < 2500; i++) {
                game1.SetRounds(rounds1.ToArray());
                game2.SetRounds(rounds2.ToArray());
                Move move1, move2;
                try {
                    move1 = bot1.MakeMove(game1);
                } catch (Exception e) {
                    Console.WriteLine($"Bot1 error, Bot2 wins, score was {bot1Wins} - {bot2Wins}");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return;
                }

                try {
                    move2 = bot2.MakeMove(game2);
                } catch (Exception e) {
                    Console.WriteLine($"Bot2 error, Bot1 wins, score was {bot1Wins} - {bot2Wins}");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return;
                }

                if (move1 == Move.D && bot1D-- == 0) {
                    Console.WriteLine($"Bot1 out of dynamite, Bot2 wins, score was {bot1Wins} - {bot2Wins}");
                    return;
                }
                if (move2 == Move.D && bot2D-- == 0) {
                    Console.WriteLine($"Bot2 out of dynamite, Bot1 wins, score was {bot1Wins} - {bot2Wins}");
                    return;
                }
                var round1 = new Round();
                var round2 = new Round();
                round1.SetP1(move1);
                round1.SetP2(move2);
                round2.SetP1(move2);
                round2.SetP2(move1);
                rounds1.Add(round1);
                rounds2.Add(round2);
                var result = GetScore(move1, move2);
                if (result == 0) {
                    value++;
                }
                else {
                    if (result > 0) {
                        bot1Wins += result*value;
                    } else {
                        bot2Wins -= result*value;
                    }
                    value = 1;
                }

                if (bot1Wins >= 1000) {
                    Console.WriteLine($"Bot1 wins, score was {bot1Wins} - {bot2Wins}");
                    return;
                } else if (bot2Wins >= 1000) {
                    Console.WriteLine($"Bot2 wins, score was {bot1Wins} - {bot2Wins}");
                    return;
                }
            }
            Console.WriteLine($"Draw, score was {bot1Wins} - {bot2Wins}");
        }
    }
}