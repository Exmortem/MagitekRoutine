using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Dragoon;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Dragoon
{
    internal static class Buff
    {
        public static async Task<bool> LanceCharge()
        {
            if (!DragoonSettings.Instance.BuffsUse)
                return false;

            if (!DragoonSettings.Instance.LanceCharge)
                return false;

            if (Casting.LastSpell != Spells.VorpalThrust && Casting.LastSpell != Spells.Disembowel)
                return false;

            return await Spells.LanceCharge.Cast(Core.Me);
        }

        public static async Task<bool> BloodOfTheDragon()
        {
            if (!DragoonSettings.Instance.BloodOfTheDragon)
                return false;

            if (ActionResourceManager.Dragoon.Timer.TotalMilliseconds > 4000)
                return false;

            if (Core.Me.ClassLevel >= 70) //I don't think this is a thing anymore.
            {
                if (DateTime.Now < Utilities.Routines.Dragoon.CanBloodOfTheDragonAgain)
                    return false;
            }

            return await Spells.BloodoftheDragon.Cast(Core.Me);
        }

        public static async Task<bool> BattleLitany()
        {
            if (!DragoonSettings.Instance.BuffsUse)
                return false;

            if (!DragoonSettings.Instance.BattleLitany)
                return false;

            return await Spells.BattleLitany.Cast(Core.Me);
        }

        public static async Task<bool> DragonSight()
        {
            if (!DragoonSettings.Instance.BuffsUse)
                return false;

            if (!DragoonSettings.Instance.DragonSight)
                return false;

            if (!ActionManager.HasSpell(Spells.DragonSight.Id))
                return false;

            if (Globals.InParty)
            {
                if (DragoonSettings.Instance.DragonSightDpsOrTank)
                    return await MeleeDpsOrTank();
            }

            if (DragoonSettings.Instance.DragonSightOnSelf)
                return await Spells.DragonSight.Cast(Core.Me);

            return false;
        }

        private static async Task<bool> MeleeDpsOrTank()
        {
            var ally = Group.CastableAlliesWithin12.Where(a => (a.IsTank() || a.IsMeleeDps()) && a.IsAlive);

            return await Spells.DragonSight.Cast(ally.FirstOrDefault());
        }

        public static async Task<bool> TrueNorth()
        {
            if (!DragoonSettings.Instance.TrueNorth)
                return false;

            /*if (ViewModels.BaseSettings.Instance.PositionalStatus != "OutOfPosition") //TODO: gross
                return false;*/

            if (Casting.LastSpell == Spells.TrueNorth)
                return false;

            if (Core.Me.HasAura(Auras.TrueNorth))
                return false;

            if (Casting.LastSpell != Spells.FullThrust && Casting.LastSpell !=Spells.ChaosThrust  && Casting.LastSpell != Spells.WheelingThrust && Casting.LastSpell != Spells.FangAndClaw)
                return false;

            return await Spells.TrueNorth.Cast(Core.Me);
        }
    }

}
