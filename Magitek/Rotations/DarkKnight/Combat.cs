using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.DarkKnight;
using Magitek.Logic.Roles;
using Magitek.Models.DarkKnight;
using Magitek.Models.QueueSpell;
using Magitek.Utilities;

namespace Magitek.Rotations.DarkKnight
{
    internal static class Combat
    {
        public static Queue<QueueSpell> SpellQueue { get; } = new Queue<QueueSpell>();
        private static bool InSpellQueue { get; set; }
        private static bool AoeCheck => DarkKnightSettings.Instance.UseAoe && Utilities.Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) >= DarkKnightSettings.Instance.AoeEnemies;

        public static async Task<bool> Execute()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (!SpellQueue.Any())
            {
                InSpellQueue = false;
            }

            if (SpellQueue.Any())
            {
                if (await SpellQueueMethod()) return true;
            }

            if (await Defensive.ExecuteTankBusters()) return true;
            
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);
            }

            if (await Buff.Grit()) return true;
            if (await Defensive.TheBlackestNight()) return true;
            if (await Buff.Delirium()) return true;
            if (await Buff.BloodWeapon()) return true;

            if (Utilities.Routines.DarkKnight.OnGcd)
            {
                if (await Tank.Provoke(DarkKnightSettings.Instance)) return true;
                if (await Defensive.Execute()) return true;
                if (await Buff.LivingShadow()) return true;
                if (await SingleTarget.CarveAndSpit()) return true;

                if (AoeCheck)
                {
                    if (await Aoe.SaltedEarth()) return true;
                    if (await Aoe.AbyssalDrain()) return true;
                    if (await Aoe.FloodofDarknessShadow()) return true;
                }

                if (await SingleTarget.EdgeofDarknessShadow()) return true;
                if (await SingleTarget.Plunge()) return true;
                if (await SingleTarget.Reprisal()) return true;
            }

            if (await SingleTarget.Unmend()) return true;
            
            if (AoeCheck)
            {
                if (await Aoe.Quietus()) return true;
                if (await Aoe.StalwartSoul()) return true;
                if (await Aoe.Unleash()) return true;
            }

            if (await SingleTarget.Bloodspiller()) return true;
            if (await SingleTarget.SoulEater()) return true;
            if (await SingleTarget.SyphonStrike()) return true;
            return await SingleTarget.HardSlash();
        }

        private static async Task<bool> SpellQueueMethod()
        {
            InSpellQueue = true;
            var spell = SpellQueue.Peek();

            if (spell.Wait != null)
            {
                if (await Coroutine.Wait(spell.Wait.WaitTime, spell.Wait.Check))
                {
                    Logger.Write($"Spell Queue Wait: {spell.Wait.Name}");
                }
                else
                {
                    SpellQueue.Dequeue();
                    return true;
                }
            }

            if (spell.Checks.Any(x => !x.Check.Invoke()))
            {
                SpellQueue.Dequeue();
                Logger.Write($"Removing {spell.Spell.LocalizedName} from the Spell Queue because it failed it's checks.");
                foreach (var check in spell.Checks.Where(x => !x.Check.Invoke()))
                {
                    Logger.Write($"Failed Check: {check.Name}");
                }
                return true;
            }

            var target = spell.Target();

            if (target != null)
            {
                if (await spell.Spell.Cast(target))
                {
                    SpellQueue.Dequeue();
                    return true;
                }
            }

            return SpellQueue.Any();
        }
    }
}