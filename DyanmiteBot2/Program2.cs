﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BotInterface.Bot;
using BotInterface.Game;

namespace DynamiteBot {
    public class Program2 : IBot {
        
        
        public static Dictionary<Move, double> WinWeights(Dictionary<Move, double> moves) {
            var R = moves[Move.S] + moves[Move.W] / 3;
            var P = moves[Move.R] + moves[Move.W] / 3;
            var S = moves[Move.P] + moves[Move.W] / 3;
            var D = moves[Move.R] + moves[Move.P] + moves[Move.S]; // scale this by usage
            var W = moves[Move.D]; // scale this by usage

            var entropyScale = Utils.RPSEntropy(moves);
            D *= 0.5 + 1.5 * entropyScale; // half chance when predictable, double when no clue
            
            return new Dictionary<Move, double> {
                {Move.R,R}, {Move.P, P}, {Move.S, S}, {Move.D, D/3}, {Move.W, W}
            };
        }

        private double InstantMix = 0.5;
        private int cutOff = 1;

        private double AdjustTheirDynamite(double rate) {
            var instantRate = dModel.GetEstimate();
            var theirFactor = (TheirDynamite * 10.0) / (1000.0 - TheirScore);

            rate = rate * (1 - InstantMix) + instantRate * InstantMix;
            if (TheirDynamite <= cutOff) {
                rate *= theirFactor;
            }
            return rate;
        }
        
        public Dictionary<Move, double> DynamiteAdjust(Dictionary<Move, double> moves) {
            var myFactor = MyDynamite * 10.0 / (1000.0 - MyScore) * Math.Sqrt(CurrentValue) * 0.7;
            moves[Move.D] *= myFactor;
            return moves;
        }
        
        int MyDynamite = 99; // opponent will always have chance of me playing dynamite
        int TheirDynamite = 100;
        int MyScore = 0;
        int TheirScore = 0;
        int CurrentValue = 1;
        private int GameLength = 0;

        public void Update(Gamestate g) {
            var rounds = g.GetRounds();
            GameLength++;
            if (rounds.Length == 0) {
                return;
            }
            var round = rounds[rounds.Length - 1];
            var move1 = round.GetP1();
            var move2 = round.GetP2();
            if (move1 == Move.D) {MyDynamite--;}
            if (move2 == Move.D) {TheirDynamite--;}
            var result = Utils.GetScore(move1, move2);
            if (result == 0) {
                CurrentValue++;
            }
            else {
                if (result > 0) {
                    MyScore += result*CurrentValue;
                } else {
                    TheirScore -= result*CurrentValue;
                }
                CurrentValue = 1;
            }
            
            ////////////////////////////////
            // UPDATE ANY MATRICES ETC. HERE
            model.UpdateModel(g);
            dModel.Update(g);
        }
        
        public IMarkov model = new GeneralMarkov(1, 0.01);
        public DynamitePredictor dModel = new DynamitePredictor(0.1, 0.99);
        
        public Move MakeMove(Gamestate gamestate)
        {
            Update(gamestate);
            var weights = model.GetProbs(gamestate);
            var tot = weights.Values.Sum();
            var dRate = AdjustTheirDynamite(weights[Move.D] / tot);
            weights[Move.D] = tot * dRate;
            var selectionWeights = DynamiteAdjust(WinWeights(weights));
            var choice = GameLength < 10 ? Utils.Choose(Utils.MovesRPS) : Utils.ChooseWeighted(selectionWeights);
     /*       if (GameLength % 10 == 0) {
                Console.WriteLine($"Round {GameLength}");
                Console.WriteLine($"Dynamite left {MyDynamite}");
                PrintPredictions(selectionWeights);
                Console.WriteLine(choice);
            }*/
            return choice;
        }
    }
}