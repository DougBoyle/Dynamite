using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Bot;
using BotInterface.Game;
using DynamiteBot;

namespace DyanmiteBot2 {
    public class BrokenBot : ProgramGeneral {
        public BrokenBot(int i = 2) : base( new BrokenMarkov(i, 0.01), i) {}
    }
}
