using System;

namespace Magitek.Models.QueueSpell
{
    public class QueueSpellCheck
    {
        public string Name { get; set; }
        public Func<bool> Check { get; set; }
        public bool EndQueueIfCheckFailed { get; set; }
        public bool SilentMode { get; set; }
    }
}