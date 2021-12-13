using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Bard
{
    internal static class DamageOverTime
    {

        #region MainTargetDoTs

        public static async Task<bool> WindbiteOnCurrentTarget()
        {
            if (!BardSettings.Instance.UseWindBite)
                return false;

            if (Core.Me.ClassLevel < 64)
            {
                if (Core.Me.ClassLevel < 30 || !ActionManager.HasSpell(Spells.Windbite.Id))
                    return false;

                if (!Core.Me.CurrentTarget.InLineOfSight())
                    return false;

                if (BardSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= BardSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                    return false;

                if (Core.Me.CurrentTarget.HasAura(Auras.Windbite, true))
                    return false;

                if (!await Spells.Windbite.Cast(Core.Me.CurrentTarget)) return false;
                Logger.WriteInfo($@"[DoT-Effect] Windbite on {Core.Me.CurrentTarget.Name}");
                return true;
            }

            if (!ActionManager.HasSpell(Spells.Windbite.Id))
                return false;

            if (!Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (BardSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= BardSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.StormBite, true))
                return false;

            if (!await Spells.Stormbite.Cast(Core.Me.CurrentTarget)) return false;
            Logger.WriteInfo($@"[DoT-Effect] Stormbite on {Core.Me.CurrentTarget.Name}");
            return true;
        }

        public static async Task<bool> VenomousBiteOnCurrentTarget()
        {
            if (!BardSettings.Instance.UseVenomousBite)
                return false;

            if (Core.Me.ClassLevel < 64)
            {
                if (Core.Me.ClassLevel < 6 || !ActionManager.HasSpell(Spells.VenomousBite.Id))
                    return false;

                if (!Core.Me.CurrentTarget.InLineOfSight())
                    return false;

                if (BardSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= BardSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                    return false;

                if (Core.Me.CurrentTarget.HasAura(Auras.VenomousBite, true))
                    return false;

                if (!await Spells.VenomousBite.Cast(Core.Me.CurrentTarget)) return false;
                Logger.WriteInfo($@"[DoT-Effect] VenomousBite on {Core.Me.CurrentTarget.Name}");
                return true;
            }

            if (!ActionManager.HasSpell(Spells.CausticBite.Id))
                return false;

            if (!Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (BardSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= BardSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.CausticBite, true))
                return false;

            if (!await Spells.CausticBite.Cast(Core.Me.CurrentTarget)) return false;
            Logger.WriteInfo($@"[DoT-Effect] CausticBite on {Core.Me.CurrentTarget.Name}");
            return true;
        }

        public static async Task<bool> IronJawsOnCurrentTarget()
        {
            //No Dots at this point
            if (Core.Me.ClassLevel < 6)
                return false;

            if (!BardSettings.Instance.UseIronJaws)
                return false;

            if (Core.Me.ClassLevel < 56 || !ActionManager.HasSpell(Spells.IronJaws.Id))
                return false;

            if (!BardSettings.Instance.UseWindBite || !BardSettings.Instance.UseVenomousBite)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Utilities.Routines.Bard.Windbite, true) || !Core.Me.CurrentTarget.HasAura(Utilities.Routines.Bard.VenomousBite, true))
                return false;

            Aura windbite = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == Utilities.Routines.Bard.Windbite && x.CasterId == Core.Player.ObjectId);
            Aura venomousbite = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == Utilities.Routines.Bard.VenomousBite && x.CasterId == Core.Player.ObjectId);

            if (windbite.TimespanLeft.TotalMilliseconds - Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds > BardSettings.Instance.RefreshDotsWithXmsLeftAfterLastGCD
                && venomousbite.TimespanLeft.TotalMilliseconds - Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds > BardSettings.Instance.RefreshDotsWithXmsLeftAfterLastGCD)
                return false;

            if (!await Spells.IronJaws.Cast(Core.Me.CurrentTarget)) return false;
            Logger.WriteInfo($@"[DoT-Refresh] Iron Jaws on {Core.Me.CurrentTarget.Name}");
            Logger.WriteInfo($@"[DoT-Refresh] Windbite TimeLeft : {windbite.TimespanLeft.TotalMilliseconds}");
            Logger.WriteInfo($@"[DoT-Refresh] VenomousBite TimeLeft : {venomousbite.TimespanLeft.TotalMilliseconds}");
            return true;
        }

        public static async Task<bool> SnapShotIronJawsOnCurrentTarget()
        {
            if (!BardSettings.Instance.SnapShotWithIronJaws)
                return false;

            //No Dots at this point
            if (Core.Me.ClassLevel < 6)
                return false;

            if (!BardSettings.Instance.UseIronJaws)
                return false;

            if (Core.Me.ClassLevel < 56 || !ActionManager.HasSpell(Spells.IronJaws.Id))
                return false;

            if (!BardSettings.Instance.UseWindBite || !BardSettings.Instance.UseVenomousBite)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Utilities.Routines.Bard.Windbite, true) || !Core.Me.CurrentTarget.HasAura(Utilities.Routines.Bard.VenomousBite, true))
                return false;

            Aura windbite = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == Utilities.Routines.Bard.Windbite && x.CasterId == Core.Player.ObjectId);
            Aura venomousbite = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == Utilities.Routines.Bard.VenomousBite && x.CasterId == Core.Player.ObjectId);

            if (!Core.Me.Auras.Any(x => x.Id == Auras.RagingStrikes && x.TimespanLeft.TotalMilliseconds < 3000)
                && !Core.Me.Auras.Any(x => x.Id == Auras.RadiantFinale && x.TimespanLeft.TotalMilliseconds < 3000)
                && !Core.Me.Auras.Any(x => x.Id == Auras.BattleVoice && x.TimespanLeft.TotalMilliseconds < 3000))
                return false;

            if (Utilities.Routines.Bard.AlreadySnapped)
                return false;

            if (!await Spells.IronJaws.Cast(Core.Me.CurrentTarget)) return false;
            Logger.WriteInfo($@"[DoT-Refresh] Snap Jaws on {Core.Me.CurrentTarget.Name}");
            Logger.WriteInfo($@"[DoT-Refresh] Windbite TimeLeft : {windbite.TimespanLeft.TotalMilliseconds}");
            Logger.WriteInfo($@"[DoT-Refresh] VenomousBite TimeLeft : {venomousbite.TimespanLeft.TotalMilliseconds}");
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

        public static async Task<bool> WindbiteOnOffTarget()
        {
            if (!BardSettings.Instance.EnableMultiDotting) return false;

            if (!BardSettings.Instance.UseWindBite || !BardSettings.Instance.MultiDotWindBite)
                return false;

            BattleCharacter windBiteMultiDoTTarget = null;

            if (Core.Me.ClassLevel < 64)
            {
                if (Core.Me.ClassLevel < 30 || !ActionManager.HasSpell(Spells.Windbite.Id))
                    return false;
                if (BardSettings.Instance.MultiDotWindBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(Auras.Windbite, true) && r != Core.Me.CurrentTarget))
                    return false;

                windBiteMultiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, Auras.Windbite));
            }
            else
            {
                if (BardSettings.Instance.MultiDotWindBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(Auras.StormBite, true) && r != Core.Me.CurrentTarget))
                    return false;

                windBiteMultiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, Auras.StormBite));
            }

            if (windBiteMultiDoTTarget == null) return false;
            if (!await Spells.Stormbite.Cast(windBiteMultiDoTTarget)) return false;
            Logger.WriteInfo($@"[MultiDot] Stormbite on {windBiteMultiDoTTarget.Name}");
            return true;

        }

        public static async Task<bool> VenomousBiteOnOffTarget()
        {
            if (!BardSettings.Instance.EnableMultiDotting)
                return false;

            if (!BardSettings.Instance.UseVenomousBite || !BardSettings.Instance.MultiDotVenomousBite)
                return false;

            BattleCharacter venomousBiteMultiDoTTarget = null;

            if (Core.Me.ClassLevel < 64)
            {
                if (BardSettings.Instance.MultiDotVenomousBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(Auras.VenomousBite, true) && r != Core.Me.CurrentTarget))
                    return false;

                venomousBiteMultiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, Auras.VenomousBite));

            }
            else
            {
                if (BardSettings.Instance.MultiDotVenomousBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(Auras.CausticBite, true) && r != Core.Me.CurrentTarget))
                    return false;

                venomousBiteMultiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, Auras.CausticBite));
            }

            if (venomousBiteMultiDoTTarget == null) return false;
            if (!await Spells.CausticBite.Cast(venomousBiteMultiDoTTarget)) return false;
            Logger.WriteInfo($@"[MultiDot] Caustic Bite on {venomousBiteMultiDoTTarget.Name}");
            return true;
        }

        public static async Task<bool> IronJawsOnOffTarget()
        {
            if (Core.Me.ClassLevel < 6) //No Dots at this point
                return false;

            if (!BardSettings.Instance.EnableMultiDotting) return false;

            //We wont need to IJ when we dont want double DoTs on everything
            if (!BardSettings.Instance.MultiDotVenomousBite || !BardSettings.Instance.MultiDotWindBite)
                return false;

            if (Core.Me.ClassLevel < 56 || !ActionManager.HasSpell(Spells.IronJaws.Id))
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

                if (!unit.HasAura(Utilities.Routines.Bard.Windbite, true) || !unit.HasAura(Utilities.Routines.Bard.VenomousBite, true))
                    return false;

                Aura windbite = unit.Auras.FirstOrDefault(x => x.Id == Utilities.Routines.Bard.Windbite && x.CasterId == Core.Player.ObjectId);
                Aura venomousbite = unit.Auras.FirstOrDefault(x => x.Id == Utilities.Routines.Bard.VenomousBite && x.CasterId == Core.Player.ObjectId);

                if (windbite.TimespanLeft.TotalMilliseconds - Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds > BardSettings.Instance.RefreshDotsWithXmsLeftAfterLastGCD
                    && venomousbite.TimespanLeft.TotalMilliseconds - Spells.HeavyShot.AdjustedCooldown.TotalMilliseconds > BardSettings.Instance.RefreshDotsWithXmsLeftAfterLastGCD)
                    return false;

                return true;
            }

        }

        #endregion


        /*
        Still here for some ideas
        public static async Task<bool> IronJaws()
        {

            if (!Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true))
                return false;

            // state 0 - haven't used IJ yet since Buff up
            // state 1 - have used IJ already since buff up

            if (Utilities.Routines.Bard.SnapShotCheck == 0)
            {

                if (Core.Me.HasAura(786)) // Battle Littany
                {
                    if (Casting.LastSpell == Spells.IronJaws)
                    {
                        Utilities.Routines.Bard.SnapShotCheck = 1;
                        Logger.WriteInfo($"Value of IronJaws Battle Littany Check {Utilities.Routines.Bard.SnapShotCheck}");
                        return false;
                    }
                    Logger.WriteInfo("We just IJ'd during Battle Littany");
                    return await Spells.IronJaws.Cast(Core.Me.CurrentTarget);
                }

                if (Core.Me.CurrentTarget.HasAura(1221)) // Chain Stratagem
                {
                    if (Casting.LastSpell == Spells.IronJaws)
                    {
                        Utilities.Routines.Bard.SnapShotCheck = 2;
                        Logger.WriteInfo($"Value of IronJaws Chain Stratagem Check {Utilities.Routines.Bard.SnapShotCheck}");
                        return false;
                    }
                    Logger.WriteInfo("We just IJ'd during Chain Stratagem");
                    return await Spells.IronJaws.Cast(Core.Me.CurrentTarget);
                }
            }

            if (Utilities.Routines.Bard.SnapShotCheck == 1)
            {
                if (!Core.Me.HasAura(786))
                {
                    Utilities.Routines.Bard.SnapShotCheck = 0;
                    Logger.WriteInfo("We just passed the IJ Gate and BattleLitany has dropped,Reset.");
                    return false;
                }
            }

            if (Utilities.Routines.Bard.SnapShotCheck == 2)
            {
                if (!Core.Me.CurrentTarget.HasAura(1221))
                {
                    Utilities.Routines.Bard.SnapShotCheck = 0;
                    Logger.WriteInfo("We just passed the IJ Gate and ChainStrat has dropped,Reset.");
                    return false;
                }
            }

            if (Core.Me.CurrentTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true, BardSettings.Instance.DotRefreshTime * 1000 + (int)Spells.HeavyShot.Cooldown.TotalMilliseconds))
                return false;

            if (Casting.LastSpell == Spells.IronJaws && Casting.LastSpellTarget == Core.Me.CurrentTarget)
                return false;

            return await Spells.IronJaws.Cast(Core.Me.CurrentTarget);

        }
        */

    }
}


