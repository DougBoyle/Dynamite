using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinningBot.Sequences;
using static WinningBot.Predictors.PredictingMethods;
using static WinningBot.Constants;

namespace WinningBot.Predictors
{
    class GamePredictor
    {
        // can just use DynamiteController.ScoreKeeper.PointsForRound as consecutiveDraws
        public DynamiteController DynamiteController = new DynamiteController();
        MoveSequence MoveSequence = new MoveSequence();
        ClassSequence ClassSequence = new ClassSequence();
        ClassSequence[] DrawSequences;

        TranscendentClassPredictor ClassPredictor;
        TranscendentMovePredictor MovePredictor;
        TranscendentDrawPredictor[] DrawPredictors;


        // move/class predictors are already global instances, so no need to make them members
        public GamePredictor(List<scorer> scorers, int numDrawPredictors)
        {
            DrawSequences = new ClassSequence[numDrawPredictors];
            DrawPredictors = new TranscendentDrawPredictor[numDrawPredictors];
            for (int i = 0; i < numDrawPredictors; i++)
            {
                DrawSequences[i] = new ClassSequence();
                DrawPredictors[i] = new TranscendentDrawPredictor(scorers, AllDrawPredictors);
            }

            ClassPredictor = new TranscendentClassPredictor(scorers, AllClassPredictors);
            MovePredictor = new TranscendentMovePredictor(scorers, AllMovePredictors);

            // underlying predictions
            SimpleMovePredictors.ForEach(p => p.MakeNextPrediction(MoveSequence));
            SimpleDrawPredictors.ForEach(p => p.MakeNextPrediction(DrawSequences[0])); // sequence 0/1 identical initially
            AllClassPredictors.ForEach(p => p.MakeNextPrediction(ClassSequence));

            // metapredictions
            MovePredictor.MakePredictions();
            ClassPredictor.MakePredictions();
            foreach (TranscendentDrawPredictor p in DrawPredictors)
            {
                p.MakePredictions();
            }
        }

        public void Update(char MyMove, char YourMove)
        {
            MoveSequence.Extend(MyMove, YourMove);
            MovePredictor.UpdateScores(YourMove);
            SimpleMovePredictors.ForEach(p => p.MakeNextPrediction(MoveSequence));
            MovePredictor.MakePredictions();

            ClassSequence.Extend(MyMove, YourMove);
            ClassPredictor.UpdateScores(YourMove);
            AllClassPredictors.ForEach(p => p.MakeNextPrediction(ClassSequence));
            ClassPredictor.MakePredictions();

            int numDraws = DynamiteController.ScoreKeeper.PointsForRound;
            if (numDraws >= 2 && numDraws < 2 + DrawSequences.Length)
            {
                DrawSequences[numDraws - 2].Extend(MyMove, YourMove);
                DrawPredictors[numDraws - 2].UpdateScores(YourMove);
                // Delay making new predictions until actually needed
            }

            DynamiteController.Update(MyMove, YourMove);
        }

        public char GetNextMove()
        {
            PredictedMoveAction movePrediction = MovePredictor.GetNextPrediction();
            PredictedClassAction classPrediction = ClassPredictor.GetNextPrediction();

            bool classPredictorWins = classPrediction.YourClass != NORMAL
                && classPrediction.Score > 2 * Math.Max(0.5, movePrediction.Score);

            bool expectingD = (classPredictorWins ? classPrediction.YourClass : movePrediction.YourMove) == D;
            bool expectingW = (classPredictorWins ? classPrediction.YourClass : movePrediction.YourMove) == W;

            int numDraws = DynamiteController.ScoreKeeper.PointsForRound;
            if (numDraws >= 2 && numDraws < 2 + DrawSequences.Length)
            {
                SimpleDrawPredictors.ForEach(p => p.MakeNextPrediction(DrawSequences[numDraws - 2]));
                DrawPredictors[numDraws - 2].MakePredictions();
                PredictedClassAction drawPrediction = DrawPredictors[numDraws - 2].GetNextPrediction();
                if (drawPrediction.Score > 1.5 && drawPrediction.YourClass == D) // TODO: Parameter 1.5 to optimise
                {
                    return W;
                } else if (drawPrediction.YourClass == NORMAL) // TODO: Should probably still look at score before deciding to do this (e.g. at least > 0)
                {
                    return D;
                } else // TODO: Should fall back to move predictor rather than random? e.g. just fall through instead?
                {
                    return RandomRPS();
                }
            }

            if (numDraws == 1)
            {
                double drawsRemaining = DynamiteController.EstimateDrawsRemaining();
                double dynamitesRemaining = DynamiteController.EstimateDynamitesRemaining();
                double p = dynamitesRemaining / drawsRemaining;
                if (Rand.NextDouble() < p && !expectingW)
                {
                    return D;
                }
            }

            // TODO: Again, fall back to other predictors?
            if (numDraws >= 2)
            {
                return expectingD ? W : (expectingW ? RandomRPS() : D);
            }

            return classPredictorWins ? (classPrediction.YourClass == D ? W : RandomRPS()) : movePrediction.Play;
        }
    }
}
