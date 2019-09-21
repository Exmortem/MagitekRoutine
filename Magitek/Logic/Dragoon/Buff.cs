using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (!DragoonSettings.Instance.UseBuffs)
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
            if (!DragoonSettings.Instance.UseBuffs)
                return false;

            if (!DragoonSettings.Instance.BattleLitany)
                return false;

            return await Spells.BattleLitany.Cast(Core.Me);
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

        public static async Task<bool> DragonSight()
        {
            if (!DragoonSettings.Instance.UseBuffs)
                return false;

            if (!DragoonSettings.Instance.UseDragonSight)
                return false;

            if (!ActionManager.CanCast(Spells.DragonSight, Core.Me))
                return false;

            IEnumerable<Character> allyList = null;

            switch (DragoonSettings.Instance.SelectedStrategy)
            {
                case DragonSightStrategy.ClosestDps:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsDps()).OrderBy(GetWeight);
                    break;

                case DragonSightStrategy.Self:
                    return await Spells.DragonSight.Cast(Core.Me);

                case DragonSightStrategy.MeleeDps:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsMeleeDps()).OrderBy(GetWeight);
                    break;

                case DragonSightStrategy.RangedDps:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsRangedDpsCard()).OrderBy(GetWeight);
                    break;

                case DragonSightStrategy.Tank:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsTank()).OrderBy(GetWeight);
                    break;

                case DragonSightStrategy.Healer:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsHealer()).OrderBy(GetWeight);
                    break;
            }

            if (allyList == null)
                return false;

            return await Spells.DragonSight.CastAura(allyList.FirstOrDefault(), Auras.LeftEye);
        }
        
        private static int GetWeight(Character c)
        {
            switch (c.CurrentJob)
            {
                case ClassJobType.Astrologian:
                    return DragoonSettings.Instance.AstEyeWeight;

                case ClassJobType.Monk:
                case ClassJobType.Pugilist:
                    return DragoonSettings.Instance.MnkEyeWeight;

                case ClassJobType.BlackMage:
                case ClassJobType.Thaumaturge:
                    return DragoonSettings.Instance.BlmEyeWeight;

                case ClassJobType.Dragoon:
                case ClassJobType.Lancer:
                    return DragoonSettings.Instance.DrgEyeWeight;

                case ClassJobType.Samurai:
                    return DragoonSettings.Instance.SamEyeWeight;

                case ClassJobType.Machinist:
                    return DragoonSettings.Instance.MchEyeWeight;

                case ClassJobType.Summoner:
                case ClassJobType.Arcanist:
                    return DragoonSettings.Instance.SmnEyeWeight;

                case ClassJobType.Bard:
                case ClassJobType.Archer:
                    return DragoonSettings.Instance.BrdEyeWeight;

                case ClassJobType.Ninja:
                case ClassJobType.Rogue:
                    return DragoonSettings.Instance.NinEyeWeight;

                case ClassJobType.RedMage:
                    return DragoonSettings.Instance.RdmEyeWeight;

                case ClassJobType.Dancer:
                    return DragoonSettings.Instance.DncEyeWeight;

                case ClassJobType.Paladin:
                case ClassJobType.Gladiator:
                    return DragoonSettings.Instance.PldEyeWeight;

                case ClassJobType.Warrior:
                case ClassJobType.Marauder:
                    return DragoonSettings.Instance.WarEyeWeight;

                case ClassJobType.DarkKnight:
                    return DragoonSettings.Instance.DrkEyeWeight;

                case ClassJobType.Gunbreaker:
                    return DragoonSettings.Instance.GnbEyeWeight;

                case ClassJobType.WhiteMage:
                case ClassJobType.Conjurer:
                    return DragoonSettings.Instance.WhmEyeWeight;

                case ClassJobType.Scholar:
                    return DragoonSettings.Instance.SchEyeWeight;
            }

            return c.CurrentJob == ClassJobType.Adventurer ? 70 : 0;
        }

    }

}
