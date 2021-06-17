using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinningBot.Sequences;
using static WinningBot.Constants;
using static WinningBot.Scorers;

namespace WinningBot.Predictors
{
    public class PredictedMoveAction
    {
        public char YourMove;
        public char Play;
        public double Score;

        public PredictedMoveAction(char yours, double score)
        {
            YourMove = yours;
            Play = BeatWithoutDynamite(yours);
            Score = score;
        }
    }

    public class TranscendentMovePredictor
    {
        public static determineResult PredictingMyMoveResult(char actual)
        {
            return prediction =>
            {
                if (actual == D) { return -1; }
                else { return RESULT[actual][prediction]; }
            };
        }

        public static determineResult PredictingYourMoveResult(char actual)
        {
            return prediction =>
            {
                if (prediction == actual) { return 1; }
                else if (prediction == W)
                {
                    if (actual != D) { return 0; }
                    else { return -1; }
                }
                else if (prediction == D)
                {
                    if (actual == W) { return 0; }
                    else { return -1; }
                }
                return RESULT[actual][prediction] == 1 ? 0 : -1;
            };
        }

        List<MetaPredictor<MoveSequence>> MyMetaPredictors;
        List<MetaPredictor<MoveSequence>> YourMetaPredictors;

        public TranscendentMovePredictor(List<scorer> Scorers, List<BasePredictor<MoveSequence>> Predictors)
        {
            MyMetaPredictors = Scorers.Select(s => new MetaPredictor<MoveSequence>(Predictors, s, p => p.Mine)).ToList();
            YourMetaPredictors = Scorers.Select(s => new MetaPredictor<MoveSequence>(Predictors, s, p => p.Yours)).ToList();
        }

        public PredictedMoveAction GetNextPrediction()
        {
            MetaPredictor<MoveSequence> MyBestPredictor = MaxBy(MyMetaPredictors, p => p.Score);
            MetaPredictor<MoveSequence> YourBestPredictor = MaxBy(YourMetaPredictors, p => p.Score);
            if (MyBestPredictor.Score > YourBestPredictor.Score)
            {
                return new PredictedMoveAction(BeatWithoutDynamite(MyBestPredictor.NextPrediction), MyBestPredictor.Score);
            } else
            {
                return new PredictedMoveAction(YourBestPredictor.NextPrediction, YourBestPredictor.Score);
            }
        }

        public void UpdateScores(char actualYourMove)
        {
            YourMetaPredictors.ForEach(mp => mp.UpdateScores(PredictingYourMoveResult(actualYourMove)));
            // if the opponent isn't actually trying to intelligently predict my own move, this score will end up much lower
            // and the bot will just focus on predicting their moves, not making itself less predictable.
            MyMetaPredictors.ForEach(mp => mp.UpdateScores(PredictingMyMoveResult(actualYourMove)));
        }
        public void MakePredictions()
        {
            YourMetaPredictors.ForEach(mp => mp.MakePredictions());
            MyMetaPredictors.ForEach(mp => mp.MakePredictions());
        }
    }

    public class PredictedClassAction
    {
        public char YourClass;
        public double Score;

        public PredictedClassAction(char yours, double score)
        {
            YourClass = yours;
            Score = score;
        }
    }


    public class TranscendentClassPredictor
    {
        public static determineResult PredictingYourClassResult(char actual)
        {
            return prediction =>
                actual == D ? (prediction == D ? 1 : -1) :
                actual == W ? (prediction == W ? 1 : 0) :
                prediction == D ? -1 : 0;
        }

        List<MetaPredictor<ClassSequence>> YourMetaPredictors;

        public TranscendentClassPredictor(List<scorer> Scorers, List<BasePredictor<ClassSequence>> Predictors)
        {
            YourMetaPredictors = Scorers.Select(s => new MetaPredictor<ClassSequence>(Predictors, s, p => p.Yours)).ToList();
        }

        public PredictedClassAction GetNextPrediction()
        {
            MetaPredictor<ClassSequence> BestPredictor = MaxBy(YourMetaPredictors, p => p.Score);
            return new PredictedClassAction(BestPredictor.NextPrediction, BestPredictor.Score);
        }

        public void UpdateScores(char actualYourMove)
        {
            YourMetaPredictors.ForEach(mp => mp.UpdateScores(PredictingYourClassResult(actualYourMove)));
        }
        public void MakePredictions()
        {
            YourMetaPredictors.ForEach(mp => mp.MakePredictions());
        }
    }



    public class TranscendentDrawPredictor
    {

        public static determineResult PredictingYourDrawResult(char actual)
        {
            return prediction => {
                int result = CLASS_RESULT[CLASS[actual]][prediction];
                return result == 0 ? 1 : (result == 1 ? 0 : -2);
            };
        }

        public static determineResult PredictingMyDrawResult(char actual)
        {
            return prediction =>
            {
                int result = CLASS_RESULT[CLASS[actual]][prediction];
                return result == 1 ? 1 : (result == -1 ? 0 : -2);
            };
        }

        List<MetaPredictor<ClassSequence>> YourMetaPredictors;
        List<MetaPredictor<ClassSequence>> MyMetaPredictors;

        public TranscendentDrawPredictor(List<scorer> Scorers, List<BasePredictor<ClassSequence>> Predictors)
        {
            YourMetaPredictors = Scorers.Select(s => new MetaPredictor<ClassSequence>(Predictors, s, p => p.Yours)).ToList();
            MyMetaPredictors = Scorers.Select(s => new MetaPredictor<ClassSequence>(Predictors, s, p => p.Yours)).ToList();
        }

        public PredictedClassAction GetNextPrediction()
        {
            MetaPredictor<ClassSequence> MyBestPredictor = MaxBy(YourMetaPredictors, p => p.Score);
            MetaPredictor<ClassSequence> YourBestPredictor = MaxBy(YourMetaPredictors, p => p.Score);
            if (MyBestPredictor.Score > YourBestPredictor.Score)
            {
                return new PredictedClassAction(BeatClass(MyBestPredictor.NextPrediction), MyBestPredictor.Score);
            } else
            {
                return new PredictedClassAction(YourBestPredictor.NextPrediction, YourBestPredictor.Score);
            }
        }

        public void UpdateScores(char actualYourMove)
        {
            YourMetaPredictors.ForEach(mp => mp.UpdateScores(PredictingYourDrawResult(actualYourMove)));
            MyMetaPredictors.ForEach(mp => mp.UpdateScores(PredictingMyDrawResult(actualYourMove)));
        }
        public void MakePredictions()
        {
            YourMetaPredictors.ForEach(mp => mp.MakePredictions());
            MyMetaPredictors.ForEach(mp => mp.MakePredictions());
        }
    }
}
