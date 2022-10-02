using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Dragoon;
using Magitek.Toggles;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using DragoonRoutine = Magitek.Utilities.Routines.Dragoon;

namespace Magitek.Logic.Dragoon
{
    internal static class Buff
    {
        public static async Task<bool> LanceCharge() //Damage +10%
        {
            if (!DragoonSettings.Instance.UseBuffs)
                return false;

            if (!DragoonSettings.Instance.UseLanceCharge)
                return false;

            if (Core.Me.HasAura(Auras.LanceCharge))
                return false;

            //Exec LanceCharge after Disembowel or RaidenThrust if only 1 ennemy
            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 10 + x.CombatReach) == 1)
            {
                if (ActionManager.LastSpell.Id != Spells.Disembowel.Id && ActionManager.LastSpell.Id != Spells.RaidenThrust.Id)
                    return false;
            }

            return await Spells.LanceCharge.Cast(Core.Me);
        }

        public static async Task<bool> BattleLitany() // Crit +10%
        {
            if (!DragoonSettings.Instance.UseBuffs)
                return false;

            if (!DragoonSettings.Instance.UseBattleLitany)
                return false;

            if (!Core.Me.HasAura(Auras.LanceCharge))
                return false;

            return await Spells.BattleLitany.Cast(Core.Me);
        }

        public static async Task<bool> LifeSurge() // Crit +10%
        {
            if (!DragoonSettings.Instance.UseBuffs)
                return false;

            if (!DragoonSettings.Instance.UseLifeSurge)
                return false;

            if (!Core.Me.HasAura(Auras.LanceCharge))
                return false;

            if (Casting.LastSpell == Spells.LifeSurge)
                return false;

            //Life surge only for HeavensThrust / FangAndClaw for SingleTarget or DraconianFury / CoerthanTorment for AOE 
            if (
                (ActionManager.LastSpell.Id != Spells.WheelingThrust.Id || !Core.Me.HasAura(Auras.SharperFangandClaw))
                && (ActionManager.LastSpell.Id != Spells.HeavensThrust.Id || !Core.Me.HasAura(Auras.SharperFangandClaw))
                && ActionManager.LastSpell.Id != Spells.VorpalThrust.Id
                && (!Spells.CoerthanTorment.IsKnown() || ActionManager.LastSpell.Id != Spells.SonicThrust.Id)
                && (!Spells.CoerthanTorment.IsKnown() || ActionManager.LastSpell.Id != Spells.CoerthanTorment.Id)
                && (Spells.CoerthanTorment.IsKnown() || !Spells.SonicThrust.IsKnown() || ActionManager.LastSpell.Id != Spells.DoomSpike.Id))
                return false;

            return await Spells.LifeSurge.Cast(Core.Me);
        }

        public static async Task<bool> DragonSight() //Damage +10%
        {
            if (!DragoonSettings.Instance.UseBuffs)
                return false;

            if (!DragoonSettings.Instance.UseDragonSight)
                return false;

            if (!SpellDataExtensions.CanCast(Spells.DragonSight, Core.Me))
                return false;

            if (!Core.Me.HasTarget)
                return false;

            if (!Core.Me.InCombat)
                return false;

            IEnumerable<Character> allyList = null;
            if (Globals.InParty)
            {
                switch (DragoonSettings.Instance.SelectedStrategy)
                {
                    case DragonSightStrategy.Self:
                        return await Spells.DragonSight.Cast(Core.Me);
                    
                    case DragonSightStrategy.ClosestDps:
                        allyList = Group.CastableAlliesWithin12.Where(a => a != null && a.IsAlive && a.IsVisible && !a.IsMe && a.IsDps()).OrderBy(DragoonRoutine.GetWeight);
                        break;

                    case DragonSightStrategy.MeleeDps:
                        allyList = Group.CastableAlliesWithin12.Where(a => a != null && a.IsAlive && a.IsVisible && !a.IsMe && a.IsMeleeDps()).OrderBy(DragoonRoutine.GetWeight);
                        break;

                    case DragonSightStrategy.RangedDps:
                        allyList = Group.CastableAlliesWithin12.Where(a => a != null && a.IsAlive && a.IsVisible && !a.IsMe && a.IsRangedDpsCard()).OrderBy(DragoonRoutine.GetWeight);
                        break;

                    case DragonSightStrategy.Tank:
                        allyList = Group.CastableAlliesWithin12.Where(a => a != null && a.IsAlive && a.IsVisible && !a.IsMe && a.IsTank()).OrderBy(DragoonRoutine.GetWeight);
                        break;

                    case DragonSightStrategy.Healer:
                        allyList = Group.CastableAlliesWithin12.Where(a => a != null && a.IsAlive && a.IsVisible && !a.IsMe && a.IsHealer()).OrderBy(DragoonRoutine.GetWeight);
                        break;
                }

                if (allyList == null)
                {
                    if (DragoonSettings.Instance.UseSmartDragonSight)
                        return await Spells.DragonSight.Cast(Core.Me);
                    else
                        return false;
                }

                return await Spells.DragonSight.CastAura(allyList.FirstOrDefault(), Auras.LeftEye);
            }

            return await Spells.DragonSight.Cast(Core.Me);
        }

        public static async Task<bool> ForceDragonSight()
        {
            if (!DragoonSettings.Instance.ForceDragonSight)
                return false;

            if (!await DragonSight()) 
                return false;

            DragoonSettings.Instance.ForceDragonSight = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> UsePotion()
        {
            if (Spells.BattleLitany.IsKnown() && !Spells.BattleLitany.IsReady(5000))
                return false;

            return await PhysicalDps.UsePotion(DragoonSettings.Instance);
        }

    }

}
