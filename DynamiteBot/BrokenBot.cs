namespace DynamiteBot {
    public class BrokenBot : ProgramGeneral {
        public BrokenBot(int i = 2) : base( new BrokenMarkov(i, 0.01), i) {}
    }
}
