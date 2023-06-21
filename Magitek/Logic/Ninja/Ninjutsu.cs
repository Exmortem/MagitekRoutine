using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using Magitek.Utilities.GamelogManager;
using System;
using System.Linq;
using System.Threading.Tasks;
using NinjaRoutine = Magitek.Utilities.Routines.Ninja;


namespace Magitek.Logic.Ninja
{
    internal static class Ninjutsu
    {

        public static async Task<bool> Huton()
        {

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (ActionResourceManager.Ninja.HutonTimer > new TimeSpan(0))
                return false;

            return await NinjaRoutine.PrepareNinjutsu(Spells.Ten, 3, Core.Me);

        }

        public static async Task<bool> Suiton()
        {

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if (Spells.TrickAttack.Cooldown >= new TimeSpan(0, 0, 12))
                return false;

            if (Core.Me.HasMyAura(Auras.Suiton))
                return false;

            return await NinjaRoutine.PrepareNinjutsu(Spells.Jin, 3, Core.Me.CurrentTarget);

        }

        #region PrePull

        public static async Task<bool> PrePullHutonRamp()
        {

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (ActionResourceManager.Ninja.HutonTimer > new TimeSpan(0))
                return false;

            if (!GamelogManagerCountdown.IsCountdownRunning())
                return false;

            if (GamelogManagerCountdown.GetCurrentCooldown() > 11)
                return false;

            if (NinjaRoutine.UsedMudras.Count >= 3)
                return false;

            return await NinjaRoutine.PrepareNinjutsu(Spells.Ten, 3, Core.Me);

        }

        public static async Task<bool> PrePullHutonUse()
        {

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (ActionResourceManager.Ninja.HutonTimer > new TimeSpan(0))
                return false;

            if (!GamelogManagerCountdown.IsCountdownRunning())
                return false;

            if (GamelogManagerCountdown.GetCurrentCooldown() > 11)
                return false;

            if (NinjaRoutine.UsedMudras.Count != 3)
                return false;

            return await NinjaRoutine.PrepareNinjutsu(Spells.Ten, 3, Core.Me);

        }

        public static async Task<bool> PrePullSuitonRamp()
        {

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (!GamelogManagerCountdown.IsCountdownRunning())
                return false;

            if (GamelogManagerCountdown.GetCurrentCooldown() > 6 || GamelogManagerCountdown.GetCurrentCooldown() < 1)
                return false;

            if (NinjaRoutine.UsedMudras.Count >= 3)
                return false;

            return await NinjaRoutine.PrepareNinjutsu(Spells.Jin, 3, Core.Me.CurrentTarget);
        }

        public static async Task<bool> PrePullSuitonUse()
        {

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (!GamelogManagerCountdown.IsCountdownRunning())
                return false;

            if (GamelogManagerCountdown.GetCurrentCooldown() > 1)
                return false;

            if (NinjaRoutine.UsedMudras.Count != 3)
                return false;

            return await NinjaRoutine.PrepareNinjutsu(Spells.Jin, 3, Core.Me.CurrentTarget);

        }

        #endregion

        #region TenChiJin

        public static async Task<bool> TenChiJin()
        {

            if (Core.Me.ClassLevel < 70)
                return false;

            //Dont use TCJ when under the affect of kassatsu or in process building a ninjutsu
            if ( ( Core.Me.HasMyAura(Auras.Kassatsu) || (Casting.SpellCastHistory.Count() > 0 && Casting.SpellCastHistory.First().Spell == Spells.Kassatsu) )
                || Core.Me.HasMyAura(Auras.Mudra) || NinjaRoutine.UsedMudras.Count() > 0)
                return false;

            if (!Spells.TenChiJin.IsKnown())
                return false;

            if (Spells.TrickAttack.Cooldown == new TimeSpan(0, 0, 0))
                return false;

            if (Spells.Chi.Charges >= 1f)
                return false;

            return await Spells.TenChiJin.Cast(Core.Me);
        }

        public static async Task<bool> TenChiJin_FumaShuriken()
        {

            if (!Core.Me.HasMyAura(Auras.TenChiJin))
                return false;

            if (NinjaRoutine.UsedMudras.Count() >= 1)
                return false;

            if (await Spells.Ten.Cast(Core.Me.CurrentTarget))
            {
                NinjaRoutine.UsedMudras.Add(Spells.Ten);
                return true;
            }
            return false;
        }

        public static async Task<bool> TenChiJin_Raiton()
        {

            if (!Core.Me.HasMyAura(Auras.TenChiJin))
                return false;

            if (NinjaRoutine.UsedMudras.Count() >= 2)
                return false;

            if (await Spells.Chi.Cast(Core.Me.CurrentTarget))
            {
                NinjaRoutine.UsedMudras.Add(Spells.Chi);
                return true;
            }
            return false;
        }

        public static async Task<bool> TenChiJin_Suiton()
        {

            if (!Core.Me.HasMyAura(Auras.TenChiJin))
                return false;

            if (NinjaRoutine.UsedMudras.Count() >= 3)
                return false;

            if (await Spells.Jin.Cast(Core.Me.CurrentTarget))
            {
                NinjaRoutine.UsedMudras.Add(Spells.Jin);
                return true;
            }
            return false;
        }

        #endregion

        #region Kassatsu

        //Missing target count logic
        public static async Task<bool> HyoshoRanryu()
        {

            if (Core.Me.ClassLevel < 76)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (!Core.Me.HasAura(Auras.Kassatsu) && (Casting.SpellCastHistory.Count() > 0 && Casting.SpellCastHistory.First().Spell != Spells.Kassatsu))
                return false;

            //if (Spells.TrickAttack.Cooldown == new TimeSpan(0, 0, 0))
            //    return false;

            if (ActionManager.LastSpell == Spells.GustSlash) 
                return false;

            return await NinjaRoutine.PrepareNinjutsu(Spells.Jin, 2, Core.Me.CurrentTarget);

        }

        #endregion

        public static async Task<bool> Raiton()
        {

            if (Core.Me.ClassLevel < 35)
                return false;

            if (!Spells.Chi.IsKnown())
                return false;

            if (Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            if ( Spells.Chi.Charges < 1.8 && NinjaRoutine.UsedMudras.Count() == 0 && Spells.TrickAttack.Cooldown < new TimeSpan(0, 0, 45))
                return false;

            if ( Core.Me.Auras.Where(x => x.Id == Auras.RaijuReady && x.Value == 2).Count() != 0)
                return false;

            return await NinjaRoutine.PrepareNinjutsu(Spells.Raiton, Core.Me.CurrentTarget);

        }


    }
}
