using System;
using System.IO;
using BotInterface.Game;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GameAnalysis {
    internal class Program {
        public static void Main(string[] args) {
            string gameFile = "C:\\Users/douboy/Downloads/download (1).json";
            TopJson obj = JsonConvert.DeserializeObject<TopJson>( new StreamReader(gameFile).ReadToEnd());
            int Dcount = 100;
            int i = 0;
            foreach (var round in obj.moves) {
                i++;
                if (round.p1 == Move.D) {
                    Console.WriteLine($"Round {i}");
                    Dcount--;
                }
            }
            Console.WriteLine(Dcount);
            int Wcount = 0;
            i = 0;
            foreach (var round in obj.moves) {
                i++;
                if (round.p1 == Move.W) {
                    Console.WriteLine($"Round {i}");
                    Wcount++;
                }
            }
            Console.WriteLine(Wcount);
        }
    }
}