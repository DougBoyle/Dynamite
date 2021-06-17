using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinningBot.Predictors
{
    public delegate double scorer(double current, int result);
    public delegate char pickSide(Prediction p); // selects yours/mine
    public delegate int determineResult(char predicted); // map prediction to score, based on what actual result was

    public class Predictor<T>
    {
        BasePredictor<T> BasePredictor;
        scorer Scorer;
        public char NextPrediction;
        public double Score = 0;
        pickSide whos;

        public Predictor(BasePredictor<T> bp, scorer s, pickSide wrapper)
        {
            BasePredictor = bp;
            Scorer = s;
            whos = wrapper;
        }

        public void UpdateScore(determineResult determiner)
        {
            Score = Scorer(Score, determiner(NextPrediction));
        }

        public void MakeNextPrediction()
        {
            NextPrediction = whos(BasePredictor.NextPrediction);
        }

    }
}
