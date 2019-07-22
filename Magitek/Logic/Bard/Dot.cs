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
        public static async Task<bool> Windbite(GameObject windbiteTarget)
        {
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

        public static async Task<bool> IronJaws(GameObject ironJawsTarget)
        {
            if (Casting.LastSpell == Spells.IronJaws && Casting.LastSpellTarget == ironJawsTarget)
                return false;

            if (Core.Me.ClassLevel < 56 || !ActionManager.HasSpell(Spells.IronJaws.Id))
                return false;

            if (ironJawsTarget.HasAllAuras(Utilities.Routines.Bard.DotsList, true, BardSettings.Instance.DotRefreshTime * 1000))
                return false;

            return await Spells.IronJaws.Cast(ironJawsTarget);
        }

        public static async Task<bool> HandleDots()
        {
            if (Core.Me.ClassLevel < 6) //No Dots at this point
                return false;

            if (await Windbite(Core.Me.CurrentTarget))
                return true;

            if (await VenomousBite(Core.Me.CurrentTarget))
                return true;

            if (await IronJaws(Core.Me.CurrentTarget))
                return true;

            return false;
        }

        //We could nest this more, but in this way its more customizable
        public static async Task<bool> HandleMultiDots()
        {
            if (Core.Me.ClassLevel < 6) //No Dots at this point
                return false;

            if (BardSettings.Instance.DotMultipleTargets)
            {

                BattleCharacter multiDotTarget = null;
                int numberOfAttackers = GameObjectManager.NumberOfAttackers;

                if (BardSettings.Instance.MultiDotWindbite)
                {
                    if (BardSettings.Instance.MultiDotWindbiteMaxTargets == 0 ||
                        BardSettings.Instance.MultiDotWindbiteMaxTargets <= numberOfAttackers)
                    {
                        if (Core.Me.ClassLevel < 64)
                        {
                            if (Core.Me.ClassLevel >= 30 && ActionManager.HasSpell(Spells.Windbite.Id))
                            {
                                multiDotTarget = Combat.Enemies.FirstOrDefault(r =>
                                    r.InLineOfSight() && !r.HasAura(Auras.Windbite)
                                                      && r.CombatTimeLeft() > BardSettings.Instance
                                                          .DontDotIfEnemyIsDyingWithinSeconds
                                                      && r != Core.Me.CurrentTarget);

                                if (multiDotTarget != null)
                                {
                                    if (await Spells.Windbite.Cast(multiDotTarget))
                                    {
                                        Logger.WriteInfo($@"[MultiDot] Windbite on {multiDotTarget.Name}");
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            multiDotTarget = Combat.Enemies.FirstOrDefault(r =>
                                r.InLineOfSight() && !r.HasAura(Auras.StormBite)
                                                  && r.CombatTimeLeft() > BardSettings.Instance
                                                      .DontDotIfEnemyIsDyingWithinSeconds
                                                  && r != Core.Me.CurrentTarget);

                            if (multiDotTarget != null)
                            {
                                if (await Spells.Stormbite.Cast(multiDotTarget))
                                {
                                    Logger.WriteInfo($@"[MultiDot] Stormbite on {multiDotTarget.Name}");
                                    return true;
                                }
                            }
                        }
                    }
                }

                if (BardSettings.Instance.MultiDotVenomousBite)
                {
                    if (BardSettings.Instance.MultiDotVenomousBiteMaxTargets == 0 ||
                        BardSettings.Instance.MultiDotVenomousBiteMaxTargets <= numberOfAttackers)
                    {
                        if (Core.Me.ClassLevel < 64)
                        {
                            multiDotTarget = Combat.Enemies.FirstOrDefault(r => r.InLineOfSight() && !r.HasAura(Auras.VenomousBite)
                                                                                                  && r.CombatTimeLeft() > BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds
                                                                                                  && r != Core.Me.CurrentTarget);

                            if (multiDotTarget != null)
                            {
                                if (await Spells.VenomousBite.Cast(multiDotTarget))
                                {
                                    Logger.WriteInfo($@"[MultiDot] Venomous Bite on {multiDotTarget.Name}");
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            multiDotTarget = Combat.Enemies.FirstOrDefault(r => r.InLineOfSight() && !r.HasAura(Auras.CausticBite)
                                                                                                  && r.CombatTimeLeft() > BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds
                                                                                                  && r != Core.Me.CurrentTarget);

                            if (multiDotTarget != null)
                            {
                                if (await Spells.CausticBite.Cast(multiDotTarget))
                                {
                                    Logger.WriteInfo($@"[MultiDot] Caustic Bite on {multiDotTarget.Name}");
                                    return true;
                                }
                            }
                        }
                    }
                }
                //We wont need to IJ when we dont want double DoTs on everything
                //Also is a bandaid workaround to fix casting IJ when only 1 DoT is active, gonna look into that logic at a later point
                if (/*BardSettings.Instance.MultiDotIronJaws &&*/ BardSettings.Instance.MultiDotVenomousBite && BardSettings.Instance.MultiDotWindbite) 
                {
                    if (Core.Me.ClassLevel < 56 || !ActionManager.HasSpell(Spells.IronJaws.Id))
                        return false;

                    multiDotTarget = Combat.Enemies.FirstOrDefault(r => r.InLineOfSight() && !r.HasAllAuras(Utilities.Routines.Bard.DotsList, true, BardSettings.Instance.DotRefreshTime * 1000)
                                                                                          && r.CombatTimeLeft() > BardSettings.Instance.DontDotIfEnemyIsDyingWithinSeconds
                                                                                          && r != Core.Me.CurrentTarget);
                    if (multiDotTarget != null)
                    {
                        if (await Spells.IronJaws.Cast(multiDotTarget))
                        {
                            Logger.WriteInfo($@"[MultiDot] Iron Jaws on {multiDotTarget.Name}");
                            return true;
                        }
                    }
                }
                
            }

            return false;
        }
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


