using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Paladin;
using Magitek.Logic.Roles;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Rotations.Paladin
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            if (await Defensive.TankBusters()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 4);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;


            if (await CustomOpenerLogic.Opener()) return true;
      
            if (await Buff.Oath()) return true;


            if (Utilities.Routines.Paladin.OnGcd)
            {
                if (await Buff.Cover()) return true;
                if (await Tank.Provoke(PaladinSettings.Instance)) return true;
                if (await Defensive.Defensives()) return true;           
                if (await Buff.Intervention()) return true;
                if (await Buff.DivineVeil()) return true;
                if (await SingleTarget.Requiescat()) return true;
                if (await Buff.FightOrFlight()) return true;
                if (await SingleTarget.Interject()) return true;
                if (await SingleTarget.SpiritsWithin()) return true;
                if (await Aoe.CircleofScorn()) return true;
                if (await SingleTarget.Intervene()) return true;
                if (await Buff.Sheltron()) return true;
            }

            if (await SingleTarget.ShieldBash()) return true;
            if (await SingleTarget.ShieldLobLostAggro()) return true;
            if (await Aoe.Confiteor()) return true;
            if (await Aoe.HolyCircle()) return true;
            if (await Aoe.TotalEclipse()) return true;
            if (await SingleTarget.HolySpirit()) return true;
            if (await SingleTarget.Atonement()) return true;


            if (ActionManager.LastSpell == Spells.RiotBlade && Core.Me.ClassLevel > 25 && Core.Me.ClassLevel  <54)
            {
                var rageTarget = Utilities.Combat.Enemies.FirstOrDefault(r => r.TargetGameObject != Core.Me && ActionManager.CanCast(Spells.RageofHalone, r));

                if (rageTarget == null || Group.CastableAlliesWithin30.Any(r => r.IsTank()))
                    return await Spells.RageofHalone.Cast(Core.Me.CurrentTarget);

                return await Spells.RageofHalone.Cast(rageTarget);
            }

            #region Last Spell RiotBlade
            if (ActionManager.LastSpell == Spells.RiotBlade)
            {
                if (Core.Me.ClassLevel >= 54 && !Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, (PaladinSettings.Instance.RefreshGoringBlade) * 1000) && Core.Me.CurrentTarget.HealthCheck(PaladinSettings.Instance.HealthSetting, PaladinSettings.Instance.HealthSettingPercent))
                {
                    if (await Spells.GoringBlade.Cast(Core.Me.CurrentTarget)) return true;
                }

                if (Core.Me.ClassLevel > 59)
                    return await Spells.RoyalAuthority.Cast(Core.Me.CurrentTarget);

                return await Spells.FastBlade.Cast(Core.Me.CurrentTarget);
            }
            #endregion

            #region Last Spell FastBlade
            if (ActionManager.LastSpell == Spells.FastBlade)
            {
                // Low level just fast blade
                if (Core.Me.ClassLevel < 4)
                {
                    return await Spells.FastBlade.Cast(Core.Me.CurrentTarget);
                }
                
                //// if we have RA and RA in shield oath is enabled riot blade
                //if (ActionManager.HasSpell(Spells.RoyalAuthority.Id))
                //{
                //    if (PaladinSettings.Instance.RoyalAuthorityComboInShieldOath)
                //    {
                //        return await Spells.RiotBlade.Cast(Core.Me.CurrentTarget);
                //    }
                //}

                if (!Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, PaladinSettings.Instance.RefreshGoringBlade * 1000))
                {
                    return await Spells.RiotBlade.Cast(Core.Me.CurrentTarget);
                }

                if (Core.Me.ClassLevel >= 60)
                {
                    return await Spells.RiotBlade.Cast(Core.Me.CurrentTarget);
                }

                return false;              
            }
            #endregion
            
            if (await Spells.FastBlade.Cast(Core.Me.CurrentTarget)) return true;

            if (PaladinSettings.Instance.ShieldLobToPull && !Core.Me.InCombat)
            {
                return await Spells.ShieldLob.Cast(Core.Me.CurrentTarget);
            }

            if (PaladinSettings.Instance.HolySpiritWhenOutOfMeleeRange && Core.Me.ClassLevel >= 64 && await Spells.HolySpirit.Cast(Core.Me.CurrentTarget)) return true;
            return await SingleTarget.ShieldLob();
        }
    }
}