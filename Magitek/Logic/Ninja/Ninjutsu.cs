using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using NinjaRoutine = Magitek.Utilities.Routines.Ninja;


namespace Magitek.Logic.Ninja
{
    internal static class Ninjutsu
    {

        #region Utility Ninjustsus

        public static async Task<bool> Huton()
        {

            //if (!NinjaSettings.Instance.UseHuton)
            //    return false;

            if (Core.Me.ClassLevel < 45)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (Core.Me.HasAura(Auras.TenChiJin) || Core.Me.HasAura(Auras.Kassatsu))
                return false;

            /*
             * Missing condition for death
             * fight duration according to guides you want to use Huraijin if you dropped huton by accident
             */
            if (ActionResourceManager.Ninja.HutonTimer > new TimeSpan(0))
                return false;

            switch (NinjaRoutine.UsedMudras.Count)
            {

                case 0:
                    if (await Spells.Chi.Cast(Core.Me))
                    {
                        NinjaRoutine.UsedMudras.Add(Spells.Chi);
                        return true;
                    }
                    return false;

                case 1:
                    if (NinjaRoutine.UsedMudras.Last() == Spells.Chi)
                    {
                        if (await Spells.Jin.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Jin);
                            return true;
                        }
                    }
                    else if (NinjaRoutine.UsedMudras.Last() == Spells.Jin)
                    {
                        if (await Spells.Chi.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Chi);
                            return true;
                        }
                    }
                    return false;

                case 2:

                    if (!NinjaRoutine.UsedMudras.Contains(Spells.Ten))
                    {
                        if (await Spells.Ten.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Ten);
                            return true;
                        }
                    }
                    return false;

                case 3:
                    if (await Spells.Ninjutsu.Cast(Core.Me))
                    {
                        NinjaRoutine.UsedMudras.Clear();
                        return true;
                    }
                    return false;

                default:
                    break;
            }

            return false;

        }

        public static async Task<bool> Suiton()
        {

            //if (!NinjaSettings.Instance.UseHuton)
            //    return false;

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

            switch (NinjaRoutine.UsedMudras.Count)
            {

                case 0:
                    if (await Spells.Ten.Cast(Core.Me))
                    {
                        NinjaRoutine.UsedMudras.Add(Spells.Ten);
                        return true;
                    }
                    return false;

                case 1:
                    if (NinjaRoutine.UsedMudras.Last() == Spells.Ten)
                    {
                        if (await Spells.Chi.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Chi);
                            return true;
                        }
                    }
                    else if (NinjaRoutine.UsedMudras.Last() == Spells.Chi)
                    {
                        if (await Spells.Ten.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Ten);
                            return true;
                        }
                    }
                    return false;

                case 2:

                    if (!NinjaRoutine.UsedMudras.Contains(Spells.Jin))
                    {
                        if (await Spells.Jin.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Jin);
                            return true;
                        }
                    }
                    return false;

                case 3:
                    if (await Spells.Ninjutsu.Cast(Core.Me.CurrentTarget))
                    {
                        NinjaRoutine.UsedMudras.Clear();
                        return true;
                    }
                    return false;

                default:
                    break;
            }

            return false;

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

            //if (!NinjaSettings.Instance.UseHuton)
            //    return false;

            if (Core.Me.ClassLevel < 76)
                return false;

            if (!Spells.Jin.IsKnown())
                return false;

            if (!Core.Me.HasAura(Auras.Kassatsu) && (Casting.SpellCastHistory.Count() > 0 && Casting.SpellCastHistory.First().Spell != Spells.Kassatsu))
                return false;

            if (Spells.TrickAttack.Cooldown == new TimeSpan(0, 0, 0))
                return false;

            if (ActionManager.LastSpell == Spells.GustSlash) 
                return false;

            switch (NinjaRoutine.UsedMudras.Count)
            {

                case 0:
                    if (await Spells.Chi.Cast(Core.Me))
                    {
                        NinjaRoutine.UsedMudras.Add(Spells.Chi);
                        return true;
                    }
                    return false;

                case 1:
                    if (NinjaRoutine.UsedMudras.Last() == Spells.Ten)
                    {
                        if (await Spells.Jin.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Jin);
                            return true;
                        }
                    }
                    else if (NinjaRoutine.UsedMudras.Last() == Spells.Chi)
                    {
                        if (await Spells.Jin.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Jin);
                            return true;
                        }
                    }
                    return false;

                case 2:
                    if (await Spells.Ninjutsu.Cast(Core.Me.CurrentTarget))
                    {
                        NinjaRoutine.UsedMudras.Clear();
                        return true;
                    }
                    return false;

                default:
                    break;
            }

            return false;

        }

        #endregion

        public static async Task<bool> Raiton()
        {

            //if (!NinjaSettings.Instance.UseHuton)
            //    return false;

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

            switch (NinjaRoutine.UsedMudras.Count)
            {

                case 0:
                    if (await Spells.Ten.Cast(Core.Me))
                    {
                        NinjaRoutine.UsedMudras.Add(Spells.Ten);
                        return true;
                    }
                    return false;

                case 1:
                    if (NinjaRoutine.UsedMudras.Last() == Spells.Ten)
                    {
                        if (await Spells.Chi.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Chi);
                            return true;
                        }
                    }
                    else if (NinjaRoutine.UsedMudras.Last() == Spells.Jin)
                    {
                        if (await Spells.Chi.Cast(Core.Me))
                        {
                            NinjaRoutine.UsedMudras.Add(Spells.Chi);
                            return true;
                        }
                    }
                    return false;


                case 2:
                    if (await Spells.Ninjutsu.Cast(Core.Me.CurrentTarget))
                    {
                        NinjaRoutine.UsedMudras.Clear();
                        return true;
                    }
                    return false;

                default:
                    break;
            }

            return false;

        }


    }
}
