using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static WinningBot.Constants;
using WinningBot.Sequences;

namespace WinningBot.Predictors
{
    // all of the actual underlying functions used to make predictions
    class PredictingMethods
    {
        static List<char> RPS = new List<char> { R, P, S };
        public static char RandomRPS()
        {
            return RandomElement(RPS);
        }

        public static char BeatRPS(char move)
        {
            return move == R ? P : (move == S ? R : S);
        }

        public static Func<char, char> BeatWithMoveForWater(char move)
        {
            return m => m == D ? W : (m == W ? move : BeatRPS(m));
        }

        public static readonly Func<char, char> BeatWWithR = BeatWithMoveForWater(R);
        public static readonly Func<char, char> BeatWWithP = BeatWithMoveForWater(P);
        public static readonly Func<char, char> BeatWWithS = BeatWithMoveForWater(S);

        // converts a function that transforms predictions to something that transforms output of BasePredictor
        public static predict<T> AsMapping<T>(BasePredictor<T> predictor, Func<char, char> transformer)
        {
            return _ =>
            {
                Prediction p = predictor.NextPrediction;
                return new Prediction(transformer(p.Mine), transformer(p.Yours));
            };
        }

        /********************************** Predicting Moves ***********************************************/

        public static int MostRecentReoccurence(string sequence, int maxLen)
        {
            for (int l = maxLen; l > 0; l--)
            {
                if (l >= sequence.Length) { continue; }
                string subsequence = sequence.Substring(sequence.Length - l); ;
                // lastIndexOf for substrings relies on sequence being strings!
                int index = sequence.LastIndexOf(subsequence, sequence.Length - l - 1);
                if (index != -1) { return index + l; }
            }
            return -1;
        }

        public static Prediction PredictMoveFromIndex(int index, MoveSequence sequence)
        {
            if (index > -1)
            {
                return new Prediction(sequence.MyMove(index), sequence.YourMove(index));
            }
            else
            {
                return new Prediction(RandomRPS(), RandomRPS());
            }
        }

        public static predict<MoveSequence> MyMovePredictor(int maxLen)
        {
            return sequence => PredictMoveFromIndex(MostRecentReoccurence(sequence.mine, maxLen), sequence);
        }
        public static predict<MoveSequence> YourMovePredictor(int maxLen)
        {
            return sequence => PredictMoveFromIndex(MostRecentReoccurence(sequence.yours, maxLen), sequence);
        }
        public static predict<MoveSequence> ResultMovePredictor(int maxLen)
        {
            return sequence => PredictMoveFromIndex(MostRecentReoccurence(sequence.results, maxLen), sequence);
        }
        public static predict<MoveSequence> PairMovePredictor(int maxLen)
        {
            return sequence => PredictMoveFromIndex(MostRecentReoccurence(sequence.pairs, maxLen), sequence);
        }

        public static readonly List<int> lengths = new List<int> { 2, 3, 5, 8, 13 };

        // TODO: Keep some things as IEnumerables rather than forcing back into Lists
        public static List<BasePredictor<MoveSequence>> SimpleMovePredictors = 
            new List<Func<int,predict<MoveSequence>>> { MyMovePredictor, YourMovePredictor, ResultMovePredictor, PairMovePredictor }
            .SelectMany(p => lengths.Select(l => p(l)))
            .Select(p => new BasePredictor<MoveSequence>(p)).ToList();

        static List<Func<char, char>> transformers = new List<Func<char, char>> { BeatWWithP, BeatWWithR, BeatWWithS };

        public static List<BasePredictor<MoveSequence>> AllMovePredictors =
            SimpleMovePredictors.SelectMany(p => transformers.SelectMany(t =>
            {
                BasePredictor<MoveSequence> p1 = new BasePredictor<MoveSequence>(AsMapping(p, t));
                p.Derived.Add(p1);
                BasePredictor<MoveSequence> p2 = new BasePredictor<MoveSequence>(AsMapping(p1, t));
                p1.Derived.Add(p2);
                return new List<BasePredictor<MoveSequence>> { p, p1, p2 };
            }
            )).ToList();

        /********************************** Predicting Classes ***********************************************/

        public static Prediction PredictClassFromIndex(int index, ClassSequence sequence)
        {
            if (index > -1)
            {
                return new Prediction(sequence.MyClass(index), sequence.YourClass(index));
            }
            else
            {
                return new Prediction(NORMAL, NORMAL);
            }
        }

        public static predict<ClassSequence> MyClassPredictor(int maxLen)
        {
            return sequence => PredictClassFromIndex(MostRecentReoccurence(sequence.mine, maxLen), sequence);
        }
        public static predict<ClassSequence> YourClassPredictor(int maxLen)
        {
            return sequence => PredictClassFromIndex(MostRecentReoccurence(sequence.yours, maxLen), sequence);
        }
        public static predict<ClassSequence> ResultClassPredictor(int maxLen)
        {
            return sequence => PredictClassFromIndex(MostRecentReoccurence(sequence.results, maxLen), sequence);
        }
        public static predict<ClassSequence> PairClassPredictor(int maxLen)
        {
            return sequence => PredictClassFromIndex(MostRecentReoccurence(sequence.pairs, maxLen), sequence);
        }

        
        public static readonly List<BasePredictor<ClassSequence>> AllClassPredictors = new List<Func<int, predict<ClassSequence>>>
            {MyClassPredictor, YourClassPredictor, ResultClassPredictor, PairClassPredictor}
            .SelectMany(p => lengths.Select(l => p(l)))
            .Select(p => new BasePredictor<ClassSequence>(p)).ToList();


        /********************************** Predicting Draws ***********************************************/

        public static Prediction PredictDrawFromIndex(int index, ClassSequence sequence)
        {
            if (index > -1)
            {
                return new Prediction(sequence.MyClass(index), sequence.YourClass(index));
            }
            else
            {
                return new Prediction(D, NORMAL);
            }
        }

        public static predict<ClassSequence> MyDrawPredictor(int maxLen)
        {
            return sequence => PredictDrawFromIndex(MostRecentReoccurence(sequence.mine, maxLen), sequence);
        }
        public static predict<ClassSequence> YourDrawPredictor(int maxLen)
        {
            return sequence => PredictDrawFromIndex(MostRecentReoccurence(sequence.yours, maxLen), sequence);
        }
        public static predict<ClassSequence> PairDrawPredictor(int maxLen)
        {
            return sequence => PredictDrawFromIndex(MostRecentReoccurence(sequence.pairs, maxLen), sequence);
        }

        public static List<BasePredictor<ClassSequence>> SimpleDrawPredictors =
            new List<Func<int, predict<ClassSequence>>> { MyDrawPredictor, YourDrawPredictor, PairDrawPredictor }
            .SelectMany(p => lengths.Select(l => p(l)))
            .Select(p => new BasePredictor<ClassSequence>(p)).ToList();

        public static List<BasePredictor<ClassSequence>> AllDrawPredictors =
            SimpleDrawPredictors.SelectMany(p => 
            {
                BasePredictor<ClassSequence> p1 = new BasePredictor<ClassSequence>(AsMapping(p, BeatClass));
                p.Derived.Add(p1);
                BasePredictor<ClassSequence> p2 = new BasePredictor<ClassSequence>(AsMapping(p1, BeatClass));
                p1.Derived.Add(p2);
                return new List<BasePredictor<ClassSequence>> { p, p1, p2 };
            }).ToList();
    }
}
