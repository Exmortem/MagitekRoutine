using ff14bot;
using ff14bot.Objects;
using System.Collections.Generic;

namespace Magitek.Models.QueueSpell
{
    public class QueueSpell
    {
        public SpellData Spell { get; set; }

        public GameObject Target()
        {
            return TargetSelf ? Core.Me : Core.Me.CurrentTarget;
        }

        public bool TargetSelf { get; set; }
        public IEnumerable<QueueSpellCheck> Checks { get; set; } = new List<QueueSpellCheck>();
        public QueueSpellWait Wait { get; set; }
        public bool SleepBefore { get; set; }
        public int SleepMilliseconds { get; set; }
        public string PositionalText { get; set; }
        public bool UsePotion { get; set; }
        public uint PotionId { get; set; }
    }
}