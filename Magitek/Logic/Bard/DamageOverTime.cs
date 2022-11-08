using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using BardRoutine = Magitek.Utilities.Routines.Bard;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Bard
{
    internal static class DamageOverTime
    {

        #region MainTargetDoTs

        public static async Task<bool> StormbiteOnCurrentTarget()
        {
            if (!BardSettings.Instance.UseStormbite)
                return false;

            if (!BardRoutine.Stormbite.IsKnown())
                return false;

            if (!Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (BardSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= BardSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            if (Core.Me.CurrentTarget.HasAura(BardRoutine.StormbiteAura, true))
                return false;

            if (!await BardRoutine.Stormbite.Cast(Core.Me.CurrentTarget))
                return false;
            
            Logger.WriteInfo($@"[DoT-Effect] Stormbite/Windbite on {Core.Me.CurrentTarget.Name}");
            return true;
        }

        public static async Task<bool> CausticBiteOnCurrentTarget()
        {
            if (!BardSettings.Instance.UseCausticBite)
                return false;

            if (!BardRoutine.CausticBite.IsKnown())
                return false;

            if (!Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (BardSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= BardSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            if (Core.Me.CurrentTarget.HasAura(BardRoutine.CausticBiteAura, true))
                return false;

            if (!await BardRoutine.CausticBite.Cast(Core.Me.CurrentTarget)) 
                return false;
            
            Logger.WriteInfo($@"[DoT-Effect] CausticBite/VenomousBite on {Core.Me.CurrentTarget.Name}");
            return true;
        }

        public static async Task<bool> IronJawsOnCurrentTarget()
        {
            if (!BardSettings.Instance.UseIronJaws)
                return false;

            if (!Spells.IronJaws.IsKnown())
                return false;

            if (!BardSettings.Instance.UseStormbite || !BardSettings.Instance.UseCausticBite)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(BardRoutine.StormbiteAura, true) || !Core.Me.CurrentTarget.HasAura(BardRoutine.CausticBiteAura, true))
                return false;

            Aura stormbiteAura = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == BardRoutine.StormbiteAura && x.CasterId == Core.Player.ObjectId);
            Aura causticbiteAura = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == BardRoutine.CausticBiteAura && x.CasterId == Core.Player.ObjectId);
            if (stormbiteAura.TimespanLeft.TotalMilliseconds - Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds > BardSettings.Instance.RefreshDotsWithXmsLeftAfterLastGCD
                && causticbiteAura.TimespanLeft.TotalMilliseconds - Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds > BardSettings.Instance.RefreshDotsWithXmsLeftAfterLastGCD)
                return false;

            if (!await Spells.IronJaws.Cast(Core.Me.CurrentTarget)) return false;
            Logger.WriteInfo($@"[DoT-Refresh] Iron Jaws on {Core.Me.CurrentTarget.Name} | Stormbite/Windbite TimeLeft : {stormbiteAura.TimespanLeft.TotalMilliseconds} | CausticBite/VenomousBite TimeLeft : {causticbiteAura.TimespanLeft.TotalMilliseconds}");
            return true;
        }

        public static async Task<bool> SnapShotIronJawsOnCurrentTarget()
        {
            if (!BardSettings.Instance.SnapShotWithIronJaws)
                return false;

            if (!BardSettings.Instance.UseIronJaws)
                return false;

            if (!Spells.IronJaws.IsKnown())
                return false;

            if (!BardSettings.Instance.UseStormbite || !BardSettings.Instance.UseCausticBite)
                return false;

            //if we dont don't have DOTs, we don't consider any snapshot
            if (!Core.Me.CurrentTarget.HasAura(BardRoutine.StormbiteAura, true) || !Core.Me.CurrentTarget.HasAura(BardRoutine.CausticBiteAura, true))
                return false;

            //if we dont don't have 3 buff, we don't consider any snapshot
            if (!Core.Me.HasAura(Auras.RagingStrikes) || !Core.Me.HasAura(Auras.RadiantFinale) || !Core.Me.HasAura(Auras.BattleVoice))
                return false;

            double ragingStrikesAuraTimeleft = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.RagingStrikes).TimespanLeft.TotalMilliseconds;
            double radiantFinaleAuraTimeleft = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.RadiantFinale).TimespanLeft.TotalMilliseconds;
            double battleVoiceAuraTimeleft = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.BattleVoice).TimespanLeft.TotalMilliseconds;
            double stormbiteTimeleft = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == BardRoutine.StormbiteAura && x.CasterId == Core.Player.ObjectId).TimespanLeft.TotalMilliseconds;
            double causticbiteTimeleft = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == BardRoutine.CausticBiteAura && x.CasterId == Core.Player.ObjectId).TimespanLeft.TotalMilliseconds;

            if (Core.Me.HasAura(Auras.RagingStrikes) && Core.Me.HasAura(Auras.RadiantFinale) && Core.Me.HasAura(Auras.BattleVoice)
                && ragingStrikesAuraTimeleft > 3000 && radiantFinaleAuraTimeleft > 3000 && battleVoiceAuraTimeleft > 3000)
                return false;

            if (BardRoutine.AlreadySnapped)
                return false;

            if (!await Spells.IronJaws.Cast(Core.Me.CurrentTarget)) 
                return false;

            Logger.WriteInfo($@"[DoT-Refresh] Snap Jaws on {Core.Me.CurrentTarget.Name} | Stormbite/Windbite TimeLeft : {stormbiteTimeleft} | Caustic/VenomousBite TimeLeft : {causticbiteTimeleft} | RS TimeLeft : {ragingStrikesAuraTimeleft}| RF TimeLeft : {radiantFinaleAuraTimeleft} | BV TimeLeft : {battleVoiceAuraTimeleft}");
            return true;
        }

        #endregion

        #region MultiDoTs

        public static bool IsValidTargetToApplyDoT(BattleCharacter unit, uint spell)
        {
            if (unit == Core.Me.CurrentTarget)
                return false;

            if (!unit.InLineOfSight())
                return false;
            
            if (unit.CombatTimeLeft() <= BardSettings.Instance.DontDotIfMultiDotTargetIsDyingWithinXSeconds && BardSettings.Instance.DontDotIfMultiDotTargetIsDyingWithinXSeconds != 0)
                return false;

            return !unit.HasAura(spell, true);
        }

        public static async Task<bool> StormbiteOnOffTarget()
        {
            if (!BardSettings.Instance.EnableMultiDotting) 
                return false;

            if (!BardSettings.Instance.UseStormbite || !BardSettings.Instance.MultiDotWindBite)
                return false;

            if (!BardRoutine.Stormbite.IsKnown())
                return false;

            if (BardSettings.Instance.MultiDotWindBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(BardRoutine.StormbiteAura, true) && r != Core.Me.CurrentTarget))
                return false;

            BattleCharacter multiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, BardRoutine.StormbiteAura));
            if (multiDoTTarget == null) 
                return false;
            
            if (!await Spells.Stormbite.Cast(multiDoTTarget)) return false;
            Logger.WriteInfo($@"[MultiDot] Stormbite/WindBite on {multiDoTTarget.Name}");
            return true;

        }

        public static async Task<bool> CausticBiteOnOffTarget()
        {
            if (!BardSettings.Instance.EnableMultiDotting)
                return false;

            if (!BardSettings.Instance.UseCausticBite || !BardSettings.Instance.MultiDotVenomousBite)
                return false;

            if (!BardRoutine.CausticBite.IsKnown())
                return false;

            if (BardSettings.Instance.MultiDotVenomousBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(BardRoutine.CausticBiteAura, true) && r != Core.Me.CurrentTarget))
                return false;

            BattleCharacter multiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, BardRoutine.CausticBiteAura));
            if (multiDoTTarget == null) 
                return false;
            
            if (!await Spells.CausticBite.Cast(multiDoTTarget)) return false;
            Logger.WriteInfo($@"[MultiDot] Caustic/Venomous Bite on {multiDoTTarget.Name}");
            return true;
        }

        public static async Task<bool> IronJawsOnOffTarget()
        {
            if (!BardSettings.Instance.EnableMultiDotting) 
                return false;

            //We wont need to IJ when we dont want double DoTs on everything
            if (!BardSettings.Instance.MultiDotVenomousBite || !BardSettings.Instance.MultiDotWindBite)
                return false;

            if (!Spells.IronJaws.IsKnown())
                return false;

            BattleCharacter multiDotTarget = Combat.Enemies.FirstOrDefault(IsValidIronJawsTarget);
            if (multiDotTarget == null)
                return false;

            if (!await Spells.IronJaws.Cast(multiDotTarget))
                return false;

            Logger.WriteInfo($@"[MultiDot] Iron Jaws on {multiDotTarget.Name}");
            return true;

            bool IsValidIronJawsTarget(BattleCharacter unit)
            {
                if (unit == Core.Me.CurrentTarget)
                    return false;

                if (!unit.InLineOfSight())
                    return false;

                if (BardSettings.Instance.DontDotIfMultiDotTargetIsDyingSoon && unit.CombatTimeLeft() <= BardSettings.Instance.DontDotIfMultiDotTargetIsDyingWithinXSeconds)
                    return false;

                if (!unit.HasAura(BardRoutine.StormbiteAura, true) || !unit.HasAura(BardRoutine.CausticBiteAura, true))
                    return false;

                Aura stormbiteAura = unit.Auras.FirstOrDefault(x => x.Id == BardRoutine.StormbiteAura && x.CasterId == Core.Player.ObjectId);
                Aura causticBiteAura = unit.Auras.FirstOrDefault(x => x.Id == BardRoutine.CausticBiteAura && x.CasterId == Core.Player.ObjectId);

                if (stormbiteAura.TimespanLeft.TotalMilliseconds - Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds > BardSettings.Instance.RefreshDotsWithXmsLeftAfterLastGCD
                    && causticBiteAura.TimespanLeft.TotalMilliseconds - Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds > BardSettings.Instance.RefreshDotsWithXmsLeftAfterLastGCD)
                    return false;

                return true;
            }
        }

        #endregion

    }
}


