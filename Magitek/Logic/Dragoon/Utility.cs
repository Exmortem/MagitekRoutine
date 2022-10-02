using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Dragoon;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Dragoon
{
    internal static class Utility
    {

        public static async Task<bool> TrueNorth()
        {
            if (DragoonSettings.Instance.EnemyIsOmni || !DragoonSettings.Instance.UseTrueNorth) 
                return false;

            if (Casting.LastSpell == Spells.TrueNorth)
                return false;

            if (Core.Me.HasAura(Auras.TrueNorth))
                return false;

            if (Casting.LastSpell != Spells.FullThrust && Casting.LastSpell != Spells.ChaosThrust && Casting.LastSpell != Spells.WheelingThrust && Casting.LastSpell != Spells.FangAndClaw)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 10 + x.CombatReach) >= DragoonSettings.Instance.AoeEnemies)
                return false;

            if (Spells.TrueThrust.Cooldown.TotalMilliseconds > Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset + 100)
                return false;

            if (ActionManager.LastSpell == Spells.Disembowel)
            {
                if (Core.Me.CurrentTarget.IsBehind)
                    return false;

                return await Spells.TrueNorth.CastAura(Core.Me, Auras.TrueNorth);
            }

            if (Core.Me.HasAura(Auras.EnhancedWheelingThrust))
            {
                if (Core.Me.CurrentTarget.IsBehind)
                    return false;

                return await Spells.TrueNorth.CastAura(Core.Me, Auras.TrueNorth);
            }

            if (Core.Me.HasAura(Auras.SharperFangandClaw))
            {
                if (Core.Me.CurrentTarget.IsFlanking)
                    return false;

                return await Spells.TrueNorth.CastAura(Core.Me, Auras.TrueNorth);
            }

            return false;
        }

    }
}