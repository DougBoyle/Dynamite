using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinningBot.Sequences;
using static WinningBot.Constants;

namespace WinningBot.Predictors
{
    // Only for MoveSequence, and takes various inputs
    // to determine which of mine/yours/result/pairs should be used to make predictions.
    // (Not fully made use of yet)
    public class MarkovPredictor
    {
        public static Dictionary<char, double> initialMoveProbs = new Dictionary<char, double> {
            {R, 0.3}, {P, 0.3}, {S, 0.3}, {D, 0.4}, {W, 0.1}
        };


        private int Order;
        // TODO: Actually slightly inefficient indexing both separately under same index.
        //       Should really store a pair of dictionaries as the contents (or just Prediction objects) of a single dictionary
        private Dictionary<string, Dictionary<char, double>> MyMatrix = new Dictionary<string, Dictionary<char, double>>();
        private Dictionary<string, Dictionary<char, double>> YourMatrix = new Dictionary<string, Dictionary<char, double>>();

        // used when no history, and to smooth predictions initially when subsequence not seen before 
        private Dictionary<char, double> Initial;
        private Func<MoveSequence, string> SequenceSelector;

        public MarkovPredictor(Func<MoveSequence, string> sequenceSelector, Dictionary<char, double> initial, int order = 1)
        {
            Order = order;
            SequenceSelector = sequenceSelector;
            Initial = initial;
        }

        public static char ChooseWeighted(Dictionary<char, double> probs)
        {
            var p = Rand.NextDouble() * probs.Select(pair => pair.Value).Sum();
            foreach (KeyValuePair<char, double> pair in probs)
            {
                if (p > pair.Value) return pair.Key;
                else p -= pair.Value;
            }
            return probs.First().Key;
        }

        public Prediction MakePrediction(MoveSequence moveSequence)
        {
            string sequence = SequenceSelector(moveSequence);
            int rounds = sequence.Length;
            // TODO: Update matrix before making prediction
            if (rounds > Order)
            {
                char yourMove = moveSequence.YourMove(rounds - 1);
                char myMove = moveSequence.MyMove(rounds - 1);
                string lastSegment = sequence.Substring(rounds - Order - 1, Order);
                MyMatrix[lastSegment][myMove]++;
                YourMatrix[lastSegment][yourMove]++;
            }

            // Make next prediction
            if (rounds < Order) return new Prediction(ChooseWeighted(Initial), ChooseWeighted(Initial));

            string segment = sequence.Substring(rounds - Order, Order);

            if (!MyMatrix.ContainsKey(segment))
            {
                MyMatrix[segment] = new Dictionary<char, double>(Initial);
                YourMatrix[segment] = new Dictionary<char, double>(Initial);
            }
            
            return new Prediction(ChooseWeighted(MyMatrix[segment]), ChooseWeighted(YourMatrix[segment]));
        }

        public BasePredictor<MoveSequence> GetBasePredictor()
        {
            return new BasePredictor<MoveSequence>(MakePrediction);
        }
    }
}
