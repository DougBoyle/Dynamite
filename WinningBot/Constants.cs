using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinningBot
{
    public static class Constants
    {
        public const char R = 'R', P = 'P', S = 'S', D = 'D', W = 'W';
        public static readonly List<char> MOVES = new List<char> { R, P, S, D, W };

        public const char NORMAL = 'N';
        public static readonly List<char> CLASSES = new List<char> { NORMAL, D, W };

        public static readonly Dictionary<char, char> CLASS = new Dictionary<char, char>
        {
            {R, NORMAL}, {P, NORMAL}, {S, NORMAL}, {D, D}, {W, W}
        };

        public static readonly Dictionary<char, Dictionary<char, int>> CLASS_RESULT =
            new Dictionary<char, Dictionary<char, int>>
        {
                { NORMAL, new Dictionary<char, int> { {NORMAL, 0}, { D, -1 }, { W, 1 } } },
                { D, new Dictionary<char, int> { {NORMAL, 1}, { D, 0 }, { W, -1 } } },
                { W, new Dictionary<char, int> { {NORMAL, -1}, { D, 1 }, { W, 0 } } }
        };

        public static readonly Dictionary<char, Dictionary<char, int>> RESULT =
            new Dictionary<char, Dictionary<char, int>>
        {
                { R, new Dictionary<char, int> { {R, 0}, {P, -1}, { S, 1}, { D, -1 }, { W, 1 } } },
                { P, new Dictionary<char, int> { {R, 1}, {P, 0}, { S, -1}, { D, -1 }, { W, 1 } } },
                { S, new Dictionary<char, int> { {R, -1}, {P, 1}, { S, 0}, { D, -1 }, { W, 1 } } },
                { D, new Dictionary<char, int> { {R, 1}, {P, 1}, { S, 1}, { D, 0 }, { W, -1 } } },
                { W, new Dictionary<char, int> { {R, -1}, {P, -1}, { S, -1}, { D, 1 }, { W, 0 } } }
        };

        public static readonly Dictionary<char, Dictionary<char, char>> RESULT_CHAR =
            new Dictionary<char, Dictionary<char, char>>
        {
                { R, new Dictionary<char, char> { {R, 'D'}, {P, 'L'}, { S, 'W' }, { D, 'L' }, { W, 'W' } } },
                { P, new Dictionary<char, char> { {R, 'W'}, {P, 'D' }, { S, 'L' }, { D, 'L' }, { W, 'W' } } },
                { S, new Dictionary<char, char> { {R, 'L' }, {P, 'W' }, { S, 'D' }, { D, 'L' }, { W, 'W' } } },
                { D, new Dictionary<char, char> { {R, 'W' }, {P, 'W' }, { S, 'W' }, { D, 'D' }, { W, 'L' } } },
                { W, new Dictionary<char, char> { {R, 'L' }, {P, 'L' }, { S, 'L' }, { D, 'W' }, { W, 'D' } } }
        };
        public static readonly List<char> RESULT_CHAR_SET = new List<char> { 'D', 'W', 'L' };

        public static readonly Dictionary<char, Dictionary<char, char>> PAIR_CHARS =
            new Dictionary<char, Dictionary<char, char>>
        {
                { R, new Dictionary<char, char> { {R, 'A'}, {P, 'B'}, { S, 'C' }, { D, 'D' }, { W, 'E' } } },
                { P, new Dictionary<char, char> { {R, 'F'}, {P, 'G' }, { S, 'H' }, { D, 'I' }, { W, 'J' } } },
                { S, new Dictionary<char, char> { {R, 'K' }, {P, 'L' }, { S, 'M' }, { D, 'N' }, { W, 'O' } } },
                { D, new Dictionary<char, char> { {R, 'P' }, {P, 'Q' }, { S, 'R' }, { D, 'S' }, { W, 'T' } } },
                { W, new Dictionary<char, char> { {R, 'U' }, {P, 'V' }, { S, 'W' }, { D, 'X' }, { W, 'Y' } } }
        };
        public static readonly List<char> PAIR_CHAR_SET = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 
            'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y'};

        public static readonly Dictionary<char, Dictionary<char, char>> CLASS_PAIR_CHARS =
            new Dictionary<char, Dictionary<char, char>>
        {
                { R, new Dictionary<char, char> { {R, 'A'}, {P, 'A'}, { S, 'A' }, { D, 'B' }, { W, 'C' } } },
                { P, new Dictionary<char, char> { {R, 'A'}, {P, 'A' }, { S, 'A' }, { D, 'B' }, { W, 'C' } } },
                { S, new Dictionary<char, char> { {R, 'A' }, {P, 'A' }, { S, 'A' }, { D, 'B' }, { W, 'C' } } },
                { D, new Dictionary<char, char> { {R, 'D' }, {P, 'D' }, { S, 'D' }, { D, 'E' }, { W, 'F' } } },
                { W, new Dictionary<char, char> { {R, 'G' }, {P, 'G' }, { S, 'G' }, { D, 'H' }, { W, 'I' } } }
        };
        public static readonly List<char> CLASS_PAIR_CHAR_SET = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I'};


        // various basic functions
        public static Random Rand = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T RandomElement<T>(IEnumerable<T> choices)
        {
            return choices.ElementAt(Rand.Next(choices.Count()));
        }

        public static char BeatWithoutDynamite(char move)
        {
            return RandomElement(MOVES.Where(m => m != D && RESULT[m][move] == 1));
        }

        public const int MAX_DYNAMITE = 100;
        public const int WINNING_SCORE = 1000;

        public static char BeatClass(char c)
        {
            return c == D ? W : (c == W ? NORMAL : D);
        }
    }
}
