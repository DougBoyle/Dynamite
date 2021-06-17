using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static WinningBot.Constants;

namespace WinningBot.Sequences
{
    public class ClassSequence
    {
        public String mine = "", yours = "", results = "", pairs = "";

        public void Extend(char MyMove, char YourMove)
        {
            mine += CLASS[MyMove];
            yours += CLASS[YourMove];
            results += RESULT_CHAR[MyMove][YourMove];
            pairs += CLASS_PAIR_CHARS[MyMove][YourMove];
        }

        // TODO: May be more effort than its worth keeping these separate (makes everything else generic)
        //       Could instead just have Mine/Yours as functions, and have both Class/MoveSequence under Sequence Interface
        public char MyClass(int index)
        {
            return mine[index];
        }
        public char YourClass(int index)
        {
            return yours[index];
        }
    }
}
