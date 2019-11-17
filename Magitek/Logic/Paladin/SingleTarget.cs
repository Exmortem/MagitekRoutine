using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Paladin
{
    internal static class SingleTarget
    {      
        public static async Task<bool> ShieldLob()
        {
            if (PaladinSettings.Instance.ShieldLobToPullExtraEnemies && !BotManager.Current.IsAutonomous)
            {
                var pullTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit() && !r.Tapped && r.Distance(Core.Me) < 15 + r.CombatReach &&
                                                                                r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.TargetGameObject != Core.Me);

                if (pullTarget != null)
                {
                    return await Spells.ShieldLob.Cast(pullTarget);
                }
            }

            if (!PaladinSettings.Instance.UseShieldLob)
                return false;

            return await Spells.ShieldLob.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> ShieldLobLostAggro()
        {
            if (Globals.OnPvpMap)
                return false;

            if (!PaladinSettings.Instance.ShieldLobLostAggro)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            var shieldLobTarget = Combat.Enemies.FirstOrDefault(r =>r.Distance(Core.Me) > 5 + r.CombatReach && r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.Distance(Core.Me) <= 15 + r.CombatReach && r.TargetGameObject != Core.Me);

            if (shieldLobTarget == null)
                return false;

            if (shieldLobTarget.TargetGameObject == null)
                return false;

            if (!await Spells.ShieldLob.Cast(shieldLobTarget))
                return false;
            
            Logger.Write($@"[Magitek] Shield Lob On {shieldLobTarget.Name} To Pull Aggro");
            return true;
        }
        
        public static async Task<bool> Interject()
        {
            if (!PaladinSettings.Instance.UseInterject)
                return false;

            if (Spells.Interject.Cooldown > TimeSpan.Zero)
                return false;

            var tarasc = (Core.Me.CurrentTarget as Character);

            if (tarasc == null || !tarasc.IsCasting)
                return false;

            return (InterruptsAndStunsManager.HighPriorityInterrupts.Contains(tarasc.CastingSpellId) || InterruptsAndStunsManager.NormalInterrupts.Contains(tarasc.CastingSpellId)) && await Spells.Interject.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SpiritsWithin()
        {
            if (!PaladinSettings.Instance.SpiritsWithin)
                return false;

            if (Core.Me.CurrentHealthPercent < PaladinSettings.Instance.SpiritsWithinOnlyAboveHealth)
                return false;

            if (Casting.LastSpell == Spells.FightorFlight)
                return false;

            if (Spells.FightorFlight.Cooldown.Seconds <= 8 && !Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 8000))
            {
                //Right we want to check if we want to hold CoS.
                if (Casting.LastSpell == Spells.FastBlade)
                    return false;

                if (Casting.LastSpell == Spells.RiotBlade)
                    return false;

                if (Casting.LastSpell == Spells.Confiteor)
                    return false;

                return await Spells.SpiritsWithin.Cast(Core.Me.CurrentTarget);
            }

            return await Spells.SpiritsWithin.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> ShieldBash()
        {
            if (!PaladinSettings.Instance.ShieldBash)
                return false;

            if (Spells.ShieldBash.Cooldown > TimeSpan.Zero)
                return false;

            var tarasc = (Core.Me.CurrentTarget as Character);

            if (tarasc == null || !tarasc.IsCasting)
                return false;

            return (InterruptsAndStunsManager.HighPriorityStuns.Contains(tarasc.CastingSpellId) || InterruptsAndStunsManager.NormalStuns.Contains(tarasc.CastingSpellId)) && await Spells.ShieldBash.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> Requiescat()
        {
            if (!PaladinSettings.Instance.Requiescat)
                return false;

            if (Core.Me.CurrentMana < 8000)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true,
                19000))
                return false;

            if (PaladinSettings.Instance.FoFFirst && Spells.FightorFlight.Cooldown.Seconds < 8 && !Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 10000))
                return false;

            if (Core.Me.HasAura(Auras.FightOrFight, true, 3000))
                return false;

            return await Spells.Requiescat.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HolySpirit()
        {
            if (!PaladinSettings.Instance.HolySpirit)
                return false;

            if (Core.Me.ClassLevel < 64)
                return false;

            if (!PaladinSettings.Instance.AlwaysHolySpiritWithBuff)
                return false;

            if (!Core.Me.HasAura(Auras.Requiescat))
                return false;

            if (Core.Me.ClassLevel >= 80)
            {

                if (Core.Me.CurrentMana <= 3999)
                    return false;
            }

            return await Spells.HolySpirit.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Intervene()
        {
            if (Core.Me.ClassLevel < 74)
                return false;

            if (!PaladinSettings.Instance.Intervene)
                return false;

            if (Core.Me.HasAura(Auras.SwordOath) && Core.Me.HasAura(Auras.FightOrFight))
            {
                // We only want to use this during Sword Oath as a OGCD, one we go into this phase and once after using Atonement
                if(Casting.LastSpell == Spells.RoyalAuthority)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);

                if (Casting.LastSpell == Spells.CircleofScorn)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);

                if (Casting.LastSpell == Spells.Atonement)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> Atonement()
        {
            if (Core.Me.ClassLevel < 76 || !Core.Me.HasAura(Auras.SwordOath))
                return false;

            return await Spells.Atonement.Cast(Core.Me.CurrentTarget);
        }
    }
}