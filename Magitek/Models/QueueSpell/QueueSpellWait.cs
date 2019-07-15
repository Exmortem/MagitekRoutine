using System;

namespace Magitek.Models.QueueSpell
{
    public class QueueSpellWait
    {
        public string Name { get; set; }
        public int WaitTime { get; set; }
        public Func<bool> Check { get; set; }
        public bool EndQueueIfWaitFailed { get; set; }
    }
}
