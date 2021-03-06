﻿using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Bot;
using BotInterface.Game;

namespace DynamiteBot {
    public class Program : IBot, SmartBot {
        private List<SmartBot> Bots;
        private List<double> Scores; // can have falling off weighting
        private List<Dictionary<Move,double>> Predictions;
        private double Decay;

        private bool DrawVotes;
        private bool Exp;

        private int dLeft = 100;
        
        public int CurrentValue = 1;
        
        public Program() {
            Bots = new List<SmartBot> {
          //      new ProgramGeneral(3), new ProgramGeneral(2), new ProgramGeneral(1),
                new AntiBot(3), new AntiBot(2), new AntiBot(1),
                new AntiCustom(), new Mk2General(1), new Mk2General(2), new Mk2General(3)
            };
            Scores = new List<double> {
              //  5, 0, 0,
                2, 0, 0,
                30, 0, 0, 5
            };
            Decay = 0.99;
            DrawVotes = true;
            Exp = false;
        }

        public bool isEm = false;

        public void Update(Gamestate g) {
            var rounds = g.GetRounds();
            if (rounds.Length == 0) {
                return;
            }
            // Reset EMbot to play rest of game hopefully, should start playing more D
            if (rounds.Length == 1100) {
                Scores = new List<double> {
                    2, 0, 0,
                    15, 0, 0, 0
                };
            }
            var round = rounds[rounds.Length - 1];
            var move1 = round.GetP1();
            var move2 = round.GetP2();
            
            int result;
            
            for (var i = 0; i < Bots.Count; i++) {
                var moves = Predictions[i];
                foreach (var m in moves) {
                    if (Exp) {
                        result = Utils.GetScore(m.Key, move2);
                    }
                    else {
                        result = Math.Max(Utils.GetScore(m.Key, move2), 0);
                    }

                    Scores[i] = Scores[i] * Decay + result * CurrentValue * m.Value;
                }
            }
            result = Utils.GetScore(move1, move2);
            if (result == 0) {
                CurrentValue++;
            }
            else {
                CurrentValue = 1;
            }
        }
        
        public Dictionary<Move, double> GetProbs(Gamestate g) {
            Update(g);
            Dictionary<Move, double> votes = new Dictionary<Move, double>();
            foreach (var move in Utils.Moves) {
                votes[move] = 0.0;
            }
            Predictions = new List<Dictionary<Move, double>>();
            for (int i = 0; i < Bots.Count; i++) {
                // Could add each bot's distribution instead and do full Baysian
                var moves = Utils.Norm(Bots[i].GetProbs(g));
                Predictions.Add(moves);
                foreach (var m in moves) {
                    if (Exp) {
                        votes[m.Key] += m.Value * Math.Exp(Scores[i] / 8.0);
                    }
                    else {
                        votes[m.Key] += m.Value * Scores[i];
                    }
                }
            }
            return votes;
        }

        public Move MakeMove(Gamestate g) {
            Update(g);
            
            
            Dictionary<Move, double> votes = new Dictionary<Move, double>();
            foreach (var move in Utils.Moves) {
                votes[move] = 0.0;
            }
            Predictions = new List<Dictionary<Move, double>>();
            for (int i = 0; i < Bots.Count; i++) {
                // Could add each bot's distribution instead and do full Baysian
                var moves = Bots[i].GetProbs(g);
                Predictions.Add(moves);
                foreach (var m in moves) {
                    if (Exp) {
                        votes[m.Key] += m.Value * Math.Exp(Scores[i] / 8.0);
                    }
                    else {
                        votes[m.Key] += m.Value * Scores[i];
                    }
                }
            }
            
            
            
            var rounds = g.GetRounds();
            if (rounds.Length > 0 && rounds[rounds.Length - 1].GetP1() == Move.D) {
                dLeft--;
            }

            if (rounds.Length == 8 && rounds[4].GetP2() == Move.D &&
                rounds[5].GetP2() == Move.D && rounds[6].GetP2() == Move.D && rounds[7].GetP2() == Move.D ) {
                isEm = true;
                return Move.W;
            }
            if (rounds.Length > 7 && rounds[rounds.Length - 2].GetP2() == Move.D
                                  && rounds[rounds.Length - 1].GetP2() == Move.D &&
                                  rounds[rounds.Length - 2].GetP1() != Move.W &&
                                  rounds[rounds.Length - 1].GetP1() != Move.W) {
                return Move.W;
            }
            
            if (rounds.Length > 1300 && isEm && dLeft > 0) {
                if (rounds.Length % 3 == 0) {
                    return Utils.Choose(Utils.MovesRPS);
                }
                else {
                    return Move.D;
                }
            }
            
            

            votes[Move.W] = 0.0;

            if (g.GetRounds().Length < 1100) {
                votes[Move.D] *= 0.1; // delay dynamite till Em stops playing W
            }
            else {
                votes[Move.D] *= 1.5;
            }

            
            
            // could select from distribution at this point
            if (DrawVotes) {
                return Utils.ChooseWeighted(votes);
            }
            var best = double.MinValue;
            var bestMove = Move.R;
            foreach (var vote in votes) {
                if (vote.Value > best) {
                    best = vote.Value;
                    bestMove = vote.Key;
                }
            }
            return bestMove;
        }
    }
}
    