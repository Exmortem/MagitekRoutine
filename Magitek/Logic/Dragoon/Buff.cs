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

            if (Core.Me.ClassLevel == 70)
            {
                if (DateTime.Now < Utilities.Routines.Dragoon.CanBloodOfTheDragonAgain)
                    return false;
            }

            return await Spells.BloodoftheDragon.Cast(Core.Me);
        }

        public static async Task<bool> BattleLitany()
        {
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

            if (DragoonSettings.Instance.DragonSightOnSelf)
                return await Spells.DragonSight.Cast(Core.Me);

            return false;
        }
    }
}
