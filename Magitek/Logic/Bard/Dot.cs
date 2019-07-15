using System.Linq;
using System.Threading.Tasks;
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
    internal static class Dot
    {
        public static async Task<bool> DotMultipleTargets()
        {
            if (!BardSettings.Instance.DotMultipleTargets)
                return false;

            if (GameObjectManager.NumberOfAttackers > BardSettings.Instance.MaximumTargetsToMultiDot)
                return false;

            if (Core.Me.ClassLevel < 30)
                return false;
            
            if (Globals.OnPvpMap)
                return false;

            bool CheckIronJaws(BattleCharacter unit)
            {
                var combatTimeLeft = unit.CombatTimeLeft();

                if (combatTimeLeft < 3)
                    return false;

                if (!unit.InLineOfSight())
                    return false;

                if (combatTimeLeft < BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds)
                    return false;

                if (!unit.HasAllAuras(Utilities.Routines.Bard.DotsList))
                    return false;

                return !unit.HasAllAuras(Utilities.Routines.Bard.DotsList, true, BardSettings.Instance.DotRefreshTime * 1000 + (int) Spells.HeavyShot.Cooldown.TotalMilliseconds);
            }


            if (ActionManager.HasSpell(Spells.IronJaws.Id))
            {
                var ironJawsTarget = Combat.Enemies.FirstOrDefault(CheckIronJaws);

                if (ironJawsTarget != null)
                {
                    if (await Spells.IronJaws.Cast(ironJawsTarget))
                    {
                        Logger.WriteInfo($@"[MultiDot] Ironjaws on {ironJawsTarget.Name}");
                        return await Coroutine.Wait(2000, () => MovementManager.IsMoving || ironJawsTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true, 13000));
                    }
                }
            }

            if (Core.Me.ClassLevel < 64)
            {
                var windBiteTarget = Combat.Enemies.FirstOrDefault(r => r.InLineOfSight() && !r.HasAura(Auras.Windbite) &&
                                                                        r.CombatTimeLeft() >
                                                                        BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds);

                if (windBiteTarget != null)
                {
                    if (await Spells.Windbite.Cast(windBiteTarget))
                    {
                        Logger.WriteInfo($@"[MultiDot] Windbite on {windBiteTarget.Name}");
                        return true;
                    }
                }

                var venomousBiteTarget = Combat.Enemies.FirstOrDefault(r => r.InLineOfSight() && !r.HasAura(Auras.VenomousBite) &&
                                                                            r.CombatTimeLeft() >
                                                                            BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds);

                if (venomousBiteTarget == null)
                    return false;

                if (await Spells.VenomousBite.Cast(venomousBiteTarget))
                {
                    Logger.WriteInfo($@"[MultiDot] Venomous Bite on {venomousBiteTarget.Name}");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var windBiteTarget = Combat.Enemies.FirstOrDefault(r => r.InLineOfSight() && !r.HasAura(Auras.StormBite) &&
                                                                        r.CombatTimeLeft() >
                                                                        BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds);

                if (windBiteTarget != null)
                {
                    if (await Spells.Windbite.Cast(windBiteTarget))
                    {
                        Logger.WriteInfo($@"[MultiDot] Stormbite on {windBiteTarget.Name}");
                        return true;
                    }
                }

                var venomousBiteTarget = Combat.Enemies.FirstOrDefault(r => r.InLineOfSight() && !r.HasAura(Auras.CausticBite) &&
                                                                            r.CombatTimeLeft() >
                                                                            BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds);

                if (venomousBiteTarget == null)
                    return false;

                if (await Spells.VenomousBite.Cast(venomousBiteTarget))
                {
                    Logger.WriteInfo($@"[MultiDot] Caustic Bite on {venomousBiteTarget.Name}");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public static async Task<bool> Windbite()
        {
            if (Casting.LastSpell == Spells.Windbite && Casting.LastSpellTarget == Core.Me.CurrentTarget)
                return false;

            if (Core.Me.ClassLevel < 30)
                return false;

            if (Core.Me.ClassLevel >= 64)
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.StormBite, true, BardSettings.Instance.DotRefreshTime * 1000 + (int)Spells.HeavyShot.Cooldown.TotalMilliseconds))
                    return false;
            }
            else
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.Windbite, true, BardSettings.Instance.DotRefreshTime * 1000 + (int)Spells.HeavyShot.Cooldown.TotalMilliseconds))
                    return false;
            }

            if (BardSettings.Instance.DontDotIfEnemyIsDyingWithin)
            {
                if (Core.Me.CurrentTarget.CombatTimeLeft() < BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds)
                    return false;
            }

            return await Spells.Windbite.Cast(Core.Me.CurrentTarget);         
        }
        
        public static async Task<bool> VenomousBite()
        {
            if (Casting.LastSpell == Spells.VenomousBite && Casting.LastSpellTarget == Core.Me.CurrentTarget)
                return false;

            if (Core.Me.ClassLevel < 24)
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.VenomousBite, true, 2 * 1000))
                    return false;
            }

            if (Core.Me.ClassLevel >= 64)
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.CausticBite, true, BardSettings.Instance.DotRefreshTime * 1000 + (int)Spells.HeavyShot.Cooldown.TotalMilliseconds))
                    return false;
            }
            else
            {
                if (Core.Me.CurrentTarget.HasAura(Auras.VenomousBite, true, BardSettings.Instance.DotRefreshTime * 1000 + (int)Spells.HeavyShot.Cooldown.TotalMilliseconds))
                    return false;
            }

            if (BardSettings.Instance.DontDotIfEnemyIsDyingWithin)
            {
                if (Core.Me.CurrentTarget.CombatTimeLeft() < BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds)
                    return false;
            }

            return await Spells.VenomousBite.Cast(Core.Me.CurrentTarget);           
        }
        
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

    }
}


