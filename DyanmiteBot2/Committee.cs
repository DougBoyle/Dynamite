using System;
using System.Collections.Generic;
using BotInterface.Game;
using DynamiteBot;

namespace DyanmiteBot2 {
    public class Committee : SmartBot {
        private List<SmartBot> Bots;
        private List<double> Scores; // can have falling off weighting
        private List<Move> Predictions;
        private double Decay;

        private bool DrawVotes;
        private bool Exp;
        
        private int BestBot;
        
        public int CurrentValue = 1;
        
        public Committee(double decay = 0.99, bool drawVotes = false, bool exp = true) {
            Bots = new List<SmartBot> {
                new ProgramGeneral(3), new ProgramGeneral(2), new ProgramGeneral(1),
                new AntiBot(3), new AntiBot(2), new AntiBot(1),
                new AntiEm(), new Mk2General(1), new Mk2General(2), new Mk2General(3)
            };
            BestBot = 0;
            Scores = new List<double>();
            Bots.ForEach(bot => Scores.Add(1.0));
            Decay = decay;
            DrawVotes = drawVotes;
            Exp = exp;
        }

        public void Update(Gamestate g) {
            var rounds = g.GetRounds();
            if (rounds.Length == 0) {
                return;
            }
            var round = rounds[rounds.Length - 1];
            var move1 = round.GetP1();
            var move2 = round.GetP2();
            int result;
            
            for (var i = 0; i < Bots.Count; i++) {
                move1 = Predictions[i];
                if (Exp) {
                    result = Utils.GetScore(move1, move2);
                }
                else {
                    result = Math.Max(Utils.GetScore(move1, move2), 0);
                }
                Scores[i] = Scores[i]*Decay + result * CurrentValue;
                if (Scores[i] > Scores[BestBot]) {
                    BestBot = i;
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
            Predictions = new List<Move>();
            for (int i = 0; i < Bots.Count; i++) {
                // Could add each bot's distribution instead and do full Baysian
                var move = Bots[i].MakeMove(g);
                Predictions.Add(move);
                if (Exp) {
                    votes[Predictions[i]] += Math.Exp(Scores[i]);
                }
                else {
                    votes[Predictions[i]] += Scores[i];
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
            Predictions = new List<Move>();
            for (int i = 0; i < Bots.Count; i++) {
                // Could add each bot's distribution instead and do full Baysian
                var move = Bots[i].MakeMove(g);
                Predictions.Add(move);
                if (Exp) {
                    votes[Predictions[i]] += Math.Exp(Scores[i]);
                }
                else {
                    votes[Predictions[i]] += Scores[i];
                }
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
    
    