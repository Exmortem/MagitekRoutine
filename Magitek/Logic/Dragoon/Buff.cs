using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
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

            IEnumerable<Character> ally = null;

            switch (DragoonSettings.Instance.SelectedStrategy)
            {
                case DragonSightStrategy.ClosestDps:
                    ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && (a.IsMeleeDps() || a.IsRangedDpsCard()));

                    break;

                case DragonSightStrategy.Self:
                    return await Spells.DragonSight.Cast(Core.Me);

                case DragonSightStrategy.MeleeDps:
                    ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && a.IsMeleeDps());
                    break;

                case DragonSightStrategy.RangedDps:
                    ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && a.IsRangedDpsCard());
                    break;

                case DragonSightStrategy.Tank:
                    ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && a.IsTank());
                    break;

                case DragonSightStrategy.Healer:
                    ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && a.IsHealer());
                    break;
            }

            if (ally == null)
                return false;

            return await Spells.DragonSight.Cast(ally?.FirstOrDefault());
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
