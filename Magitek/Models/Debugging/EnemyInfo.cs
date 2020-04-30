using Clio.Utilities;
using ff14bot.Objects;
using System;
using System.Diagnostics;

namespace Magitek.Models.Debugging
{
    public class EnemyInfo
    {
        public DateTime CombatStart { get; set; }
        public double CombatTimeLeft { get; set; }
        public double CurrentDps { get; set; }
        public uint StartHealth { get; set; }
        public Vector3 Location { get; set; }
        public bool IsMoving { get; set; }
        public Stopwatch IsMovingChange { get; set; }
        public float LastTickHealth { get; set; }
        public BattleCharacter Unit { get; set; }
        public double TimeInCombat => (DateTime.Now - CombatStart).TotalSeconds;
    }
}
