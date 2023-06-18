using Buddy.Coroutines;
using Clio.Utilities.Helpers;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Models.QueueSpell;
using Magitek.Toggles;
using Magitek.Utilities;
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

    }
}
