using System;
using System.Collections.Generic;
using BotInterface.Game;
using DynamiteBot;

namespace DyanmiteBot2 {
    public class ExpertBot :  SmartBot {
        private List<SmartBot> Bots;
        private List<double> Scores; // can have falling off weighting
        private List<Move> Predictions;
        private double Decay;

        private int BestBot;
        
        public int CurrentValue = 1;
        
        public ExpertBot(double decay = 0.99) {
            Bots = new List<SmartBot> {
                new ProgramGeneral(3), new ProgramGeneral(2), new ProgramGeneral(1),
                new AntiBot(3), new AntiBot(2), new AntiBot(1),
                new BrokenBot(3), new BrokenBot(2), new BrokenBot(1)
            };
            BestBot = 0;
            Scores = new List<double>();
            Bots.ForEach(bot => Scores.Add(0.0));
            Decay = decay;
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
                 result = Utils.GetScore(move1, move2);
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
            return new Dictionary<Move, double> {{MakeMove(g), 1.0}};
        }
        
        public Move MakeMove(Gamestate g) {
            Update(g);
            Predictions = new List<Move>();
            for (int i = 0; i < Bots.Count; i++) {
                Predictions.Add(Bots[i].MakeMove(g));
            }

            return Predictions[BestBot];
        }
    }
}