using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinningBot.Sequences;

// TODO: With this setup, will have to divide move/class predictors much more clearly.
//       Could possibly make polymorphic? But then wouldn't be able to access the same way?
namespace WinningBot.Predictors
{
    public delegate Prediction predict<T>(T value);

    // Generic type indicates whether this takes a MoveSequence or ClassSequence
    public class BasePredictor<T>
    {
        predict<T> Predictor;
        public Prediction NextPrediction;
        public List<BasePredictor<T>> Derived = new List<BasePredictor<T>> ();

        public BasePredictor(predict<T> predictor) {
            Predictor = predictor;
        }

        public void MakeNextPrediction(T sequence)
        {
            NextPrediction = Predictor(sequence);
            Derived.ForEach(bp => bp.MakeNextPrediction(sequence));
        }
    }
}
