using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static WinningBot.Scorers;

namespace WinningBot.Predictors
{
    public class MetaPredictor<T>
    {
        List<Predictor<T>> Predictors;
        public char NextPrediction;
        public double Score = 0;

        public MetaPredictor(List<BasePredictor<T>> bps, scorer s, pickSide wrapper) {
            Predictors = bps.Select(p => new Predictor<T>(p, s, wrapper)).ToList();
        }

        public void UpdateScores(determineResult determiner)
        {
            Predictors.ForEach(p => p.UpdateScore(determiner));
            Score = MetaScorer(Score, determiner(NextPrediction));
        }

        public void MakePredictions()
        {
            Predictors.ForEach(p => p.MakeNextPrediction());
            NextPrediction = MaxBy(Predictors, p => p.Score).NextPrediction;
        }
    }
}
