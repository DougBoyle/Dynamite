using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotInterface.Bot;
using BotInterface.Game;
using DynamiteBot;

using WinningBot;


namespace DynamiteTest {
    internal class Program {
        
        public static void Main(string[] args) {
           // int a = WinningBot.WinningBot.x;

           // foreach (var balance in new double[] {0.5, 0.7, 0.85, 1.0, 1.2, 1.4}) {
                // Set bots here
                int score1 = 0;
                int score2 = 0;
                int remaining = 0;
                int remainingMain = 0;
                for (int i = 0; i < 1000; i++) {
                    SmartBot bot1 = new DynamiteBot.Program();
                //    IBot bot2 = new DynamiteBot.Program();
                    IBot bot2 = new WinProgram();
                //      bot2.balance = balance;
                var result = TestBots(bot1, bot2, false);
                    if (result > 0) {
                        score1++;
                    }
                    else if (result < 0) {
                        score2++;
                    }

                    // TODO: Test runs 1000 games (each with both bots playing first to 1000),
                    //       Can't print every 50th game for WinningBot, as games take too long.
                    //       Change % 50 to % 1 for that
                   if (i % 1 == 0) {
                       Console.WriteLine($"{i}: {score1} - {score2}");
                    } 
                    //    }
                }

              Console.WriteLine($"Final outcome balance: {score1} - {score2}");
            
            }
            
           /* IBot bot1 = new DynamiteBot.Program();
            IBot bot2 = new ProgramGeneral(3);
            TestBots(bot1, bot2);*/
        

        public static int TestBots(SmartBot bot1, IBot bot2, bool output = true){
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
                    if (output) {
                        Console.WriteLine($"Bot1 error, Bot2 wins, score was {bot1Wins} - {bot2Wins}");
                    }
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return -1;
                }

                try {
                    move2 = bot2.MakeMove(game2);
                } catch (Exception e) {
                    if (output) {
                        Console.WriteLine($"Bot2 error, Bot1 wins, score was {bot1Wins} - {bot2Wins}");
                    }
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return 1;
                }

                if (move1 == Move.D && bot1D-- == 0) {
                    if (output) {
                        Console.WriteLine($"Bot1 out of dynamite, Bot2 wins, score was {bot1Wins} - {bot2Wins}");
                    }

                    return -1;
                }
                if (move2 == Move.D && bot2D-- == 0) {
                    if (output) {
                        Console.WriteLine($"Bot2 out of dynamite, Bot1 wins, score was {bot1Wins} - {bot2Wins}");
                    }

                    return 1;
                }

                var round1 = new Round();
                var round2 = new Round();
                round1.SetP1(move1);
                round1.SetP2(move2);
                round2.SetP1(move2);
                round2.SetP2(move1);
                rounds1.Add(round1);
                rounds2.Add(round2);
                var result = Utils.GetScore(move1, move2);
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
                    if (output) {
                        Console.WriteLine($"Bot1 wins, score was {bot1Wins} - {bot2Wins}");
                    }

                    return 1;
                } else if (bot2Wins >= 1000) {
                    if (output) {
                        Console.WriteLine($"Bot2 wins, score was {bot1Wins} - {bot2Wins}");
                    }

                    return -1;
                }
            }

            if (output) {
                Console.WriteLine($"Draw, score was {bot1Wins} - {bot2Wins}");
            }

            return 0;
        }
    }
}