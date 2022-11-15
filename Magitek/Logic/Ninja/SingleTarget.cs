using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using Magitek.Toggles;
using System.Threading.Tasks;
using System.Linq;
using static ff14bot.Managers.ActionResourceManager.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class SingleTarget
    {
        public static async Task<bool> SpinningEdge()
        {
            return await Spells.SpinningEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GustSlash()
        {
            if (ActionManager.LastSpell != Spells.SpinningEdge)
                return false;

            return await Spells.GustSlash.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AeolianEdge()
        {
            if (Core.Me.ClassLevel < 26)
                return false;

            if (ActionManager.LastSpell != Spells.GustSlash)
                return false;

            return await Spells.AeolianEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ArmorCrush()
        {
            if (Core.Me.ClassLevel < 54)
                return false;

            if (ActionManager.LastSpell != Spells.GustSlash)
                return false;

            if (HutonTimer.TotalMilliseconds > 30000 || HutonTimer.TotalMilliseconds == 0)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.TrickAttack))
                return false;

            return await Spells.ArmorCrush.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> DreamWithinADream()
        {
            if (Core.Me.ClassLevel < 40)
                return false;

            if (!NinjaSettings.Instance.UseDreamWithinADream)
                return false;

            if(!Core.Me.CurrentTarget.HasAura(Auras.TrickAttack))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > Core.Me.CurrentTarget.CombatReach + 3)
                return false;

            if (Spells.DreamWithinaDream.IsKnownAndReady())
                return await Spells.DreamWithinaDream.Cast(Core.Me.CurrentTarget);

            return await Spells.Assassinate.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Mug()
        {
            if (Core.Me.ClassLevel < 15)
                return false;

            if (!NinjaSettings.Instance.UseMug)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) >= NinjaSettings.Instance.AoeEnemies && NinjaSettings.Instance.DoNotUseMugDuringAoe)
                return false;

            return await Spells.Mug.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TrickAttack()
        {
            if (Core.Me.ClassLevel < 18)
                return false;

            if (!NinjaSettings.Instance.UseTrickAttack)
                return false;

            if (!Core.Me.HasAura(Auras.Suiton))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) >= NinjaSettings.Instance.AoeEnemies && NinjaSettings.Instance.DoNotUseTrickAttackDuringAoe)
                return false;

            return await Spells.TrickAttack.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> Bhavacakra()
        {
            if (Core.Me.ClassLevel < 68)
                return false;

            if (!NinjaSettings.Instance.UseBhavacakra)
                return false;

            if (Spells.Mug.IsKnownAndReady(1000))
                return await (Spells.Bhavacakra.Cast(Core.Me.CurrentTarget));

            if (Spells.Bunshin.IsKnownAndReady())
                return false;

            if (Spells.TrickAttack.IsKnownAndReady(14000))
                return false;

            return await (Spells.Bhavacakra.Cast(Core.Me.CurrentTarget));
        }

            public static async Task<bool> FleetingRaiju()
        {
            if (Core.Me.ClassLevel < 90)
                return false;

            if (!Core.Me.HasAura(Auras.RaijuReady))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > Core.Me.CurrentTarget.CombatReach + 3)
                return false;

            return await Spells.FleetingRaiju.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> Huraijin()
        {
            if (Core.Me.ClassLevel < 60)
                return false;

            if (HutonTimer.TotalMilliseconds != 0)
                return false;

            return await Spells.Huraijin.Cast(Core.Me.CurrentTarget);
        }

        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            if (!Core.Me.HasTarget)
                return false;

            return PhysicalDps.ForceLimitBreak(Spells.Braver, Spells.Bladedance, Spells.Chimatsuri, Spells.SpinningEdge);
        }

    }
}
