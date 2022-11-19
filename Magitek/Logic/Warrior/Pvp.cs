using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Warrior;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Warrior
{
    internal static class Pvp
    {

        public static async Task<bool> HeavySwingPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HeavySwingPvp.CanCast())
                return false;

            return await Spells.HeavySwingPvp.CastPvpCombo(Spells.StormPathPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> MaimPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.MaimPvp.CanCast())
                return false;

            return await Spells.MaimPvp.CastPvpCombo(Spells.StormPathPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> StormPathPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.StormPathPvp.CanCast())
                return false;

            return await Spells.StormPathPvp.CastPvpCombo(Spells.StormPathPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> FellCleavePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.FellCleavePvp.CanCast())
                return false;

            if (!WarriorSettings.Instance.Pvp_FellCleave)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.FellCleavePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BlotaPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BlotaPvp.CanCast())
                return false;

            if (!WarriorSettings.Instance.Pvp_Blota)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.BlotaPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> OrogenyPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.OrogenyPvp.CanCast())
                return false;

            if(Core.Me.CurrentTarget.HasAura(Auras.PvpOrogeny))
                return false;

            if (!WarriorSettings.Instance.Pvp_Orogeny)
                return false;

            if (Core.Me.CurrentHealthPercent < WarriorSettings.Instance.Pvp_OrogenyHealthPercent)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.OrogenyPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BloodwhettingPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BloodwhettingPvp.CanCast())
                return false;

            if (!WarriorSettings.Instance.Pvp_Bloodwhetting)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.BloodwhettingPvp.Cast(Core.Me);
        }

        public static async Task<bool> ChaoticCyclonePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ChaoticCyclonePvp.CanCast())
                return false;

            if (!WarriorSettings.Instance.Pvp_ChaoticCyclone)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.ChaoticCyclonePvp.Cast(Core.Me);
        }

        public static async Task<bool> PrimalRendPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.PrimalRendPvp.CanCast())
                return false;

            if (!WarriorSettings.Instance.Pvp_PrimalRend)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (WarriorSettings.Instance.Pvp_SafePrimalRendNOnslaught && Core.Me.CurrentTarget.Distance(Core.Me) > 3)
                return false;

            return await Spells.PrimalRendPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> OnslaughtPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.OnslaughtPvp.CanCast())
                return false;

            if (!WarriorSettings.Instance.Pvp_Onslaught)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.PvpOnslaught))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (Core.Me.CurrentHealthPercent < WarriorSettings.Instance.Pvp_OnslaughtHealthPercent)
                return false;

            if (WarriorSettings.Instance.Pvp_SafePrimalRendNOnslaught && Core.Me.CurrentTarget.Distance(Core.Me) > 3)
                return false;

            return await Spells.OnslaughtPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PrimalScreamPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.PrimalScreamPvp.CanCast())
                return false;

            if (!WarriorSettings.Instance.Pvp_PrimalScream)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) < 12 + x.CombatReach) < 1)
                return false;

            return await Spells.PrimalScreamPvp.Cast(Core.Me.CurrentTarget);
        }


    }
}