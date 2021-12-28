using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
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

            //Life surge only for HeavensThrust or FangAndClaw or WheelingThrust or CoerthanTorment (AOE) 
            if (ActionManager.LastSpell.Id != Spells.VorpalThrust.Id && ActionManager.LastSpell.Id != Spells.SonicThrust.Id && !Core.Me.HasAura(Auras.SharperFangandClaw) && !Core.Me.HasAura(Auras.EnhancedWheelingThrust))
                return false;

            return await Spells.LifeSurge.Cast(Core.Me);
        }

        public static async Task<bool> DragonSight() //Damage +10%
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
                    allyList = Group.CastableAlliesWithin12.Where(a => a.IsAlive && !a.IsMe && a.IsDps()).OrderBy(DragoonRoutine.GetWeight);
                    break;

                case DragonSightStrategy.Self:
                    return await Spells.DragonSight.Cast(Core.Me);

                case DragonSightStrategy.MeleeDps:
                    allyList = Group.CastableAlliesWithin12.Where(a => a.IsAlive && !a.IsMe && a.IsMeleeDps()).OrderBy(DragoonRoutine.GetWeight);
                    break;

                case DragonSightStrategy.RangedDps:
                    allyList = Group.CastableAlliesWithin12.Where(a => a.IsAlive && !a.IsMe && a.IsRangedDpsCard()).OrderBy(DragoonRoutine.GetWeight);
                    break;

                case DragonSightStrategy.Tank:
                    allyList = Group.CastableAlliesWithin12.Where(a => a.IsAlive && !a.IsMe && a.IsTank()).OrderBy(DragoonRoutine.GetWeight);
                    break;

                case DragonSightStrategy.Healer:
                    allyList = Group.CastableAlliesWithin12.Where(a => a.IsAlive && !a.IsMe && a.IsHealer()).OrderBy(DragoonRoutine.GetWeight);
                    break;
            }

            if (DragoonSettings.Instance.SmartDragonSight)
            {
                if (allyList == null && Globals.InParty || !Globals.InParty)
                    return await Spells.DragonSight.Cast(Core.Me);
            }
            else
            {
                if (allyList == null)
                    return false;
            }
            return await Spells.DragonSight.CastAura(allyList.FirstOrDefault(), Auras.LeftEye);
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

        

    }

}
