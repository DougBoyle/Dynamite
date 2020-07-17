using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Bot;
using BotInterface.Game;
using DynamiteBot;

namespace DyanmiteBot2 {
    public class MetaBot : IBot {
       
        public static Dictionary<Move, double> LoseWeights(Dictionary<Move, double> moves) {
            var R = moves[Move.P] + moves[Move.D] / 5 + moves[Move.W] / 10;
            var P = moves[Move.S] + moves[Move.D] / 5 + moves[Move.W] / 10;
            var S = moves[Move.R] + moves[Move.D] / 5 + moves[Move.W] / 10;
            var D = moves[Move.W] + (moves[Move.P] + moves[Move.S] + moves[Move.R])/10;
            var W = (moves[Move.P] + moves[Move.S] + moves[Move.R])/10;

            return new Dictionary<Move, double> {
                {Move.R,R}, {Move.P, P}, {Move.S, S}, {Move.D, D}, {Move.W, W}
            };
        }
        
        public static Dictionary<Move, double> WinWeights(Dictionary<Move, double> moves) {
            var R = moves[Move.S] + moves[Move.W];
            var P = moves[Move.R] + moves[Move.W];
            var S = moves[Move.P] + moves[Move.W];
            var D = moves[Move.R] + moves[Move.P] + moves[Move.S]; // scale this by usage
            var W = moves[Move.D]; // scale this by usage

            var entropyScale = Utils.RPSEntropy(moves);
            D *= 0.5 + 1.5 * entropyScale; // half chance when predictable, double when no clue
            
            return new Dictionary<Move, double> {
                {Move.R,R}, {Move.P, P}, {Move.S, S}, {Move.D, D}, {Move.W, W/3}
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

        public double balance = 1.0;
        
        public Dictionary<Move, double> DynamiteAdjust(Dictionary<Move, double> moves) {
            var myFactor = MyDynamite / (1000.0 - MyScore) * Math.Sqrt(CurrentValue) * balance;
            moves[Move.D] *= myFactor;
            return moves;
        }
        
        public int MyDynamite = 99; // opponent will always have chance of me playing dynamite
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
            
            model.UpdateModel(g);
            selfDoubt.UpdateModel(g);
            dModel.Update(g);
        }
        
        public IMarkov model;
        public GeneralReverseMarkov selfDoubt;
        public DynamitePredictor dModel = new DynamitePredictor(0.1, 0.99);
        public int order;
        
        public MetaBot() {
            order = 3;
            model = new GeneralMarkov(1, 0.01);
            selfDoubt = new GeneralReverseMarkov(1,0.0);
        }

        public static Dictionary<Move, double> Norm(Dictionary<Move, double> moves) {
            var total = moves.Values.Sum();
            var result = new Dictionary<Move, double>();
            foreach (var x in moves) {
                result[x.Key] = x.Value / total;
            }
            return result;
        }
        
        public Move MakeMove(Gamestate gamestate)
        {
            Update(gamestate);
            var weights = model.GetProbs(gamestate);
            var tot = weights.Values.Sum();
            var dRate = AdjustTheirDynamite(weights[Move.D] / tot);
            weights[Move.D] = tot * dRate;
            var selectionWeights = DynamiteAdjust(WinWeights(weights));
            //var choice = GameLength < order+10 ? Choose(MovesRPS) : ChooseWeighted(selectionWeights);
            var predicted = selfDoubt.GetProbs(gamestate);
            Console.WriteLine("Metaturn");

            var choice = GameLength < order + 5
                ? Utils.ChooseWeighted(model.GetInitial())
                : Utils.ChooseWeighted(DynamiteAdjust(LoseWeights(predicted)));
            return choice;
        }
    }
}