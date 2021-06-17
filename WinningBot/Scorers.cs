using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinningBot.Predictors;

namespace WinningBot
{
    public class Scorers
    {
        public static double ActualScorer(double current, int result)
        {
            return current + result;
        }
        public static double WeightedScorer(double current, int result)
        {
            return current*0.85 + result;
        }
        public static double ChainScorer(double current, int result)
        {
            return (result == -1) ? 0 : current + result;
        }
        public static double MetaScorer(double current, int result)
        {
            return current*0.96 + result;
        }

        public static List<scorer> AllScorers = new List<scorer> { ActualScorer, WeightedScorer, ChainScorer };

        public static T MaxBy<T>(IEnumerable<T> collection, Func<T, double> transformer)
        {
            T maxObject = collection.First();
            double max = transformer(maxObject);
            foreach (T item in collection)
            {
                if (transformer(item) > max)
                {
                    maxObject = item;
                    max = transformer(item);
                }
            }
            return maxObject;
        }
    }
}
