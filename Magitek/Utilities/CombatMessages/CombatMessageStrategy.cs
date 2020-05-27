namespace Magitek.Utilities.CombatMessages
{
    public delegate bool MessageTest();

    //See ICombatMessageStrategy for details on how this is used
    internal class CombatMessageStrategy : ICombatMessageStrategy
    {
        private MessageTest mTest;

        public CombatMessageStrategy(int priority, string message, MessageTest test) : this(priority, message, "", test) { }

        public CombatMessageStrategy(int priority, string message, string imageSource, MessageTest test)
        {
            Priority = priority;
            Message = message;
            ImageSource = imageSource;
            mTest = test;
        }

        public int Priority { get; private set; }
        public string Message { get; private set; }
        public string ImageSource { get; private set; }
        public bool ShowMessage() => mTest();
    }
}
