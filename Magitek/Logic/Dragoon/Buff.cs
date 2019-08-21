using System;
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

            if (Core.Me.HasAura(Auras.RightEye))
                return false;

            if (!ActionManager.HasSpell(Spells.DragonSight.Id))
                return false;

            if (Globals.InParty)
            {
                switch (DragoonSettings.Instance.SelectedLeftEye)
                {
                    case DragonSightStrategy.Self:
                        return await Spells.DragonSight.Cast(Core.Me);

                    case DragonSightStrategy.ClosestDps:
                        return await ClosestDps();

                    case DragonSightStrategy.MeleeDps:
                        return await MeleeDps();

                    case DragonSightStrategy.RangedDps:
                        return await RangedDps();

                    case DragonSightStrategy.Tank:
                        return await Tank();

                    case DragonSightStrategy.Healer:
                        return await Healer();
                }
            }
            return false;
        }
        //I don't know how else do do all these...
        private static async Task<bool> ClosestDps()
        {
            var ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && (a.IsMeleeDps() || a.IsRangedDpsCard()));
            return await Spells.DragonSight.Cast(target: ally.FirstOrDefault());
        }

        private static async Task<bool> MeleeDps()
        {
            var ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && a.IsMeleeDps());
            return await Spells.DragonSight.Cast(target: ally.FirstOrDefault());
        }

        private static async Task<bool> RangedDps()
        {
            var ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && a.IsRangedDpsCard());
            return await Spells.DragonSight.Cast(target: ally.FirstOrDefault());
        }

        private static async Task<bool> Tank()
        {
            var ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && a.IsTank());
            return await Spells.DragonSight.Cast(target: ally.FirstOrDefault());
        }

        private static async Task<bool> Healer()
        {
            var ally = Group.CastableAlliesWithin10.Where(a => a.IsAlive && a.IsHealer());
            return await Spells.DragonSight.Cast(target: ally.FirstOrDefault());
        }
        //What a pain in the ass this skill is...


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
