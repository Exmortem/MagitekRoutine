using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Bard
{
    internal static class DamageOverTime
    {

        #region MainTargetDoTs

        public static async Task<bool> HandleDots()
        {
            if (Core.Me.ClassLevel < 6) //No Dots at this point
                return false;

            if (BardSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= BardSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            if (await Windbite(Core.Me.CurrentTarget))
                return true;

            if (await VenomousBite(Core.Me.CurrentTarget))
                return true;

            if (await IronJaws(Core.Me.CurrentTarget))
                return true;

            return false;
        }

        public static async Task<bool> Windbite(GameObject windbiteTarget)
        {
            if (!BardSettings.Instance.UseWindBite)
                return false;

            if (Core.Me.ClassLevel < 30 || !ActionManager.HasSpell(Spells.Windbite.Id))
                return false;

            if (Core.Me.ClassLevel < 64)
            {
                if (Casting.LastSpell == Spells.Windbite && Casting.LastSpellTarget == windbiteTarget)
                    return false;

                if (windbiteTarget.HasAura(Auras.Windbite, true))
                    return false;

                return await Spells.Windbite.Cast(windbiteTarget);
            }
            if (Casting.LastSpell == Spells.Stormbite && Casting.LastSpellTarget == windbiteTarget)
                return false;

            if (windbiteTarget.HasAura(Auras.StormBite, true))
                return false;

            return await Spells.Stormbite.Cast(windbiteTarget);

        }

        public static async Task<bool> VenomousBite(GameObject venomousBiteTarget)
        {
            if (!BardSettings.Instance.UseVenomousBite)
                return false;

            if (Core.Me.ClassLevel < 64)
            {
                if (Casting.LastSpell == Spells.VenomousBite && Casting.LastSpellTarget == venomousBiteTarget)
                    return false;

                if (venomousBiteTarget.HasAura(Auras.VenomousBite, true))
                    return false;

                return await Spells.VenomousBite.Cast(venomousBiteTarget);
            }
            if (Casting.LastSpell == Spells.CausticBite && Casting.LastSpellTarget == venomousBiteTarget)
                return false;

            if (venomousBiteTarget.HasAura(Auras.CausticBite, true))
                return false;

            return await Spells.CausticBite.Cast(venomousBiteTarget);

        }

        //Still missing snapshot logic
        public static async Task<bool> IronJaws(GameObject ironJawsTarget)
        {
            if (!BardSettings.Instance.UseIronJaws)
                return false;

            if (Casting.LastSpell == Spells.IronJaws && Casting.LastSpellTarget == ironJawsTarget)
                return false;

            if (Core.Me.ClassLevel < 56 || !ActionManager.HasSpell(Spells.IronJaws.Id))
                return false;

            if (ironJawsTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true, BardSettings.Instance.RefreshDotsWithLessThanXSecondsRemaining * 1000))
                return false;

            return await Spells.IronJaws.Cast(ironJawsTarget);
        }

        #endregion

        #region MultiDoTs

        public static bool IsValidTargetToApplyDoT(BattleCharacter unit, uint spell)
        {
            if (unit == Core.Me.CurrentTarget)
                return false;
            if (!unit.InLineOfSight())
                return false;
            if (unit.CombatTimeLeft() <= BardSettings.Instance.DontDotIfMultiDotTargetIsDyingWithinXSeconds)
                return false;

            return !unit.HasAura(spell, true);
        }

        public static async Task<bool> WindBiteMultiDoT()
        {
            BattleCharacter windBiteMultiDoTTarget = null;

            if (Core.Me.ClassLevel < 64)
            {
                if (Core.Me.ClassLevel < 30 || !ActionManager.HasSpell(Spells.Windbite.Id))
                    return false;
                if (BardSettings.Instance.MultiDotWindBiteUpToXEnemies != 0 
                    && BardSettings.Instance.MultiDotWindBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(Auras.Windbite, true))) return false;

                windBiteMultiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, Auras.Windbite));

                if (windBiteMultiDoTTarget == null) return false;
                if (!await Spells.Windbite.Cast(windBiteMultiDoTTarget)) return false;
                Logger.WriteInfo($@"[MultiDot] Windbite on {windBiteMultiDoTTarget.Name}");
                return true;
            }

            if (BardSettings.Instance.MultiDotWindBiteUpToXEnemies != 0 
                && BardSettings.Instance.MultiDotWindBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(Auras.StormBite, true))) return false;

            windBiteMultiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, Auras.StormBite));

            if (windBiteMultiDoTTarget == null) return false;
            if (!await Spells.Stormbite.Cast(windBiteMultiDoTTarget)) return false;
            Logger.WriteInfo($@"[MultiDot] Stormbite on {windBiteMultiDoTTarget.Name}");
            return true;

        }

        public static async Task<bool> VenomousBiteMultiDoT()
        {
            BattleCharacter venomousBiteMultiDoTTarget = null;

            if (Core.Me.ClassLevel < 64)
            {
                if (BardSettings.Instance.MultiDotVenomousBiteUpToXEnemies != 0 &&
                    BardSettings.Instance.MultiDotVenomousBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(Auras.VenomousBite, true))) return false;

                venomousBiteMultiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, Auras.VenomousBite));

                if (venomousBiteMultiDoTTarget == null) return false;
                if (!await Spells.VenomousBite.Cast(venomousBiteMultiDoTTarget)) return false;
                Logger.WriteInfo($@"[MultiDot] Venomous Bite on {venomousBiteMultiDoTTarget.Name}");
                return true;
            }

            if (BardSettings.Instance.MultiDotVenomousBiteUpToXEnemies != 0 &&
                BardSettings.Instance.MultiDotVenomousBiteUpToXEnemies <= Combat.Enemies.Count(r => r.HasAura(Auras.CausticBite, true))) return false;

            venomousBiteMultiDoTTarget = Combat.Enemies.FirstOrDefault(r => IsValidTargetToApplyDoT(r, Auras.CausticBite));

            if (venomousBiteMultiDoTTarget == null) return false;
            if (!await Spells.CausticBite.Cast(venomousBiteMultiDoTTarget)) return false;
            Logger.WriteInfo($@"[MultiDot] Caustic Bite on {venomousBiteMultiDoTTarget.Name}");
            return true;
        }

        public static async Task<bool> HandleMultiDotting()
        {
            if (Core.Me.ClassLevel < 6) //No Dots at this point
                return false;

            if (!BardSettings.Instance.EnableMultiDotting) return false;

            BattleCharacter multiDotTarget = null;

            if (BardSettings.Instance.UseWindBite && BardSettings.Instance.MultiDotWindBite)
                if (await WindBiteMultiDoT()) return true;

            if (BardSettings.Instance.UseVenomousBite && BardSettings.Instance.MultiDotVenomousBite)
                if (await VenomousBiteMultiDoT()) return true;

            //We wont need to IJ when we dont want double DoTs on everything
            if (!BardSettings.Instance.MultiDotVenomousBite || !BardSettings.Instance.MultiDotWindBite) return false;
            if (Core.Me.ClassLevel < 56 || !ActionManager.HasSpell(Spells.IronJaws.Id))
                return false;

            multiDotTarget = Combat.Enemies.FirstOrDefault(IsValidIronJawsTarget);

            if (multiDotTarget == null) return false;
            if (!await Spells.IronJaws.Cast(multiDotTarget)) return false;
            Logger.WriteInfo($@"[MultiDot] Iron Jaws on {multiDotTarget.Name}");
            return true;

            bool IsValidIronJawsTarget(BattleCharacter unit)
            {
                if (unit == Core.Me.CurrentTarget)
                    return false;
                if (!unit.InLineOfSight())
                    return false;
                if (unit.CombatTimeLeft() <= BardSettings.Instance.DontDotIfMultiDotTargetIsDyingWithinXSeconds)
                    return false;

                if (Core.Me.ClassLevel < 64)
                {
                    if (unit.HasAura(Auras.Windbite, true) && unit.HasAura(Auras.VenomousBite, true))
                        return false;

                    if (!unit.HasAura(Auras.Windbite, true,BardSettings.Instance.RefreshDotsWithLessThanXSecondsRemaining * 1000)
                        || !unit.HasAura(Auras.VenomousBite, true,BardSettings.Instance.RefreshDotsWithLessThanXSecondsRemaining * 1000))
                        return true;

                }

                if (!unit.HasAura(Auras.StormBite, true) || !unit.HasAura(Auras.CausticBite, true))
                    return false;

                return !unit.HasAura(Auras.StormBite, true, BardSettings.Instance.RefreshDotsWithLessThanXSecondsRemaining * 1000)
                       || !unit.HasAura(Auras.CausticBite, true, BardSettings.Instance.RefreshDotsWithLessThanXSecondsRemaining * 1000);
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


