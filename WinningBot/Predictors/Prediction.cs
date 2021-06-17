using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinningBot.Predictors
{
    public class Prediction
    {
        public char Mine;
        public char Yours;

        public Prediction(char mine, char yours)
        {
            Mine = mine;
            Yours = yours;
        }
    }
}
