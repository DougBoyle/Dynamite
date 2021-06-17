using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static WinningBot.Constants;

namespace WinningBot.Sequences
{
    public class MoveSequence
    {
        public String mine = "", yours = "", results = "", pairs = "";

        public void Extend(char MyMove, char YourMove)
        {
            mine += MyMove;
            yours += YourMove;
            results += RESULT_CHAR[MyMove][YourMove];
            pairs += PAIR_CHARS[MyMove][YourMove];
        }

        public char MyMove(int index)
        {
            return mine[index];
        }
        public char YourMove(int index)
        {
            return yours[index];
        }
    }
}
