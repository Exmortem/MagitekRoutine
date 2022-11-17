using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.DarkKnight;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;


namespace Magitek.Logic.DarkKnight
{
    internal static class Pvp
    {
        public static async Task<bool> HardSlashPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HardSlashPvp.CanCast())
                return false;

            return await Spells.HardSlashPvp.CastPvpCombo(Spells.SouleaterPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> SyphonStrikePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SyphonStrikePvp.CanCast())
                return false;

            return await Spells.SyphonStrikePvp.CastPvpCombo(Spells.SouleaterPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> SouleaterPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SouleaterPvp.CanCast())
                return false;

            return await Spells.SouleaterPvp.CastPvpCombo(Spells.SouleaterPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> BloodspillerPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BloodspillerPvp.CanCast())
                return false;

            if (!DarkKnightSettings.Instance.Pvp_Bloodspiller)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            if (!Core.Me.HasAura(Auras.PvpBlackblood))
                return false;

            return await Spells.BloodspillerPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PlungePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.PlungePvp.CanCast())
                return false;

            if (!DarkKnightSettings.Instance.Pvp_Plunge)
                return false;

            if(Core.Me.HasAura(Auras.PvpNoMercy))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 20)
                return false;

            if (DarkKnightSettings.Instance.Pvp_SafePlunge && Core.Me.CurrentTarget.Distance(Core.Me) > 3)
                return false;


            return await Spells.PlungePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> QuietusPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.QuietusPvp.CanCast())
                return false;

            if (!DarkKnightSettings.Instance.Pvp_Quietus)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.QuietusPvp.Cast(Core.Me);
        }

        public static async Task<bool> BlackestNightPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.BlackestNightPvp.CanCast())
                return false;

            if (!DarkKnightSettings.Instance.Pvp_BlackestNight)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.BlackestNightPvp.Cast(Core.Me);
        }

        public static async Task<bool> SaltedEarthPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SaltedEarthPvp.CanCast())
                return false;

            if (!DarkKnightSettings.Instance.Pvp_SaltedEarth)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.SaltedEarthPvp.Cast(Core.Me);
        }

        public static async Task<bool> ShadowbringerPvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ShadowbringerPvp.CanCast())
                return false;

            if (!DarkKnightSettings.Instance.Pvp_Shadowbringer)
                return false;

            if (Core.Me.CurrentHealthPercent < DarkKnightSettings.Instance.Pvp_ShadowbringerHealthPercent && !Core.Me.HasAura(Auras.PvpDarkArts))
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            return await Spells.ShadowbringerPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EventidePvp()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.EventidePvp.CanCast())
                return false;

            if (!DarkKnightSettings.Instance.Pvp_Eventide)
                return false;

            if(Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.Pvp_EventideHealthPercent)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 3)
                return false;

            return await Spells.EventidePvp.Cast(Core.Me.CurrentTarget);
        }
    }
}
