using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Buddy.Coroutines;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using RedMageRoutines = Magitek.Utilities.Routines.RedMage;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Logic.RedMage
{
    internal static class Aoe
    {
        public static async Task<bool> Scatter()
        {
            if (!RedMageSettings.Instance.Scatter)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.ScatterEnemies)
                return false;

            if (!Core.Me.HasAura(Auras.Dualcast))
            {
                //TODO: The Balance says for single target, we should hold this for when we're moving around. Is that true for AoE too?
                if (RedMageSettings.Instance.SwiftcastScatter)
                {
                    if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                        return false;

                    if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                        return false;

                    if (!RedMageRoutines.CanWeave)
                        return false;

                    if (await Spells.Swiftcast.Cast(Core.Me))
                    {
                        await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                        await Coroutine.Wait(2000, () => ActionManager.CanCast(Spells.Scatter, Core.Me.CurrentTarget));
                        return await Spells.Scatter.Cast(Core.Me.CurrentTarget);
                    }
                }
                else
                    return false;
            }

            return await Spells.Scatter.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ContreSixte()
        {
            if (!RedMageSettings.Instance.UseContreSixte)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.ContreSixteEnemies)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            else
                return await Spells.ContreSixte.Cast(Core.Me.CurrentTarget);
        }

        public static int EnemiesInMeleeRange => Combat.Enemies.Count(r => r.InView() && r.Distance(Core.Me) <= 6 + r.CombatReach);

        private static int EnemiesInMeleeRangeWithEnoughHealth => Combat.Enemies.Count(r =>    r.InView()
                                                                                            && r.Distance(Core.Me) <= 6 + r.CombatReach
                                                                                            && r.CurrentHealthPercent >= RedMageSettings.Instance.EmboldenFinisherPercent);

        //TODO: Should we be trying to weave this?
        //TODO: Should we only use this if the pack has a certain amount of health left?
        public static async Task<bool> Embolden()
        {
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (!RedMageSettings.Instance.Embolden)
                return false;

            if (Core.Me.ClassLevel < Spells.Embolden.LevelAcquired)
                return false;

            //We only use this in conjunction with Moulinet
            if (EnemiesInMeleeRange < RedMageSettings.Instance.MoulinetEnemies)
                return false;

            //We only use this if enough enemies have enough health
            if (EnemiesInMeleeRangeWithEnoughHealth < 3)
                return false;

            if (BlackMana < 90 || WhiteMana < 90)
                return false;

            if (   BlackMana == 100
                && WhiteMana == 100)
            {
                return await Spells.Embolden.Cast(Core.Me.CurrentTarget);
            }

            if (Core.Me.HasAura(Auras.Manafication))
            {
                return await Spells.Embolden.Cast(Core.Me.CurrentTarget);
            }

            if (   Spells.Manafication.Cooldown == TimeSpan.Zero
                && RedMageSettings.Instance.Manafication
                && Core.Me.ClassLevel >= Spells.Manafication.LevelAcquired)
            {
                return await Spells.Embolden.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        //TODO: Should we be trying to weave this?
        //TODO: Should we only use this if the pack has a certain amount of health left?
        public static async Task<bool> Manafication()
        {
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (!RedMageSettings.Instance.Manafication)
                return false;

            if (Core.Me.ClassLevel < Spells.Manafication.LevelAcquired)
                return false;

            //We only use this in conjunction with Moulinet
            if (EnemiesInMeleeRange < RedMageSettings.Instance.MoulinetEnemies)
                return false;

            //We only use this if enough enemies have enough health
            if (EnemiesInMeleeRangeWithEnoughHealth < 3)
                return false;

            //Only use Manafication for AoE between 50 and 70 mana
            if (    BlackMana < 50
                ||  WhiteMana < 50
                || (BlackMana > 70 && WhiteMana > 70))
                return false;
            else
                return await Spells.Manafication.Cast(Core.Me);
        }

        public static async Task<bool> Moulinet()
        {
            if (!RedMageSettings.Instance.UseMelee)
                return false;

            if (!RedMageSettings.Instance.Moulinet)
                return false;

            if (Core.Me.ClassLevel < Spells.Moulinet.LevelAcquired)
                return false;
                
            if (BlackMana < 20 || WhiteMana < 20)
                return false;

            //Use the dualcast before Moulinet
            if (Core.Me.HasAura(Auras.Dualcast))
                return false;

            if (EnemiesInMeleeRange < RedMageSettings.Instance.MoulinetEnemies)
                return false;

            if (EnemiesInMeleeRange >= 3
                && Core.Me.ClassLevel < Spells.Embolden.LevelAcquired)
            {
                return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
            }

            if (   EnemiesInMeleeRange >= 1
                && (Core.Me.HasAura(Auras.Embolden) || Core.Me.HasAura(Auras.Manafication)))
            {
                return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
            }

            if (EnemiesInMeleeRange >= 1
                && BlackMana >= 90
                && WhiteMana >= 90)
            {
                return await Spells.Moulinet.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        public static async Task<bool> Veraero2()
        {
            if (WhiteMana > BlackMana)
                return false;

            if (!RedMageSettings.Instance.Ver2)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.Ver2Enemies)
                return false;

            else
                return await Spells.Veraero2.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Verthunder2()
        {
            //We're willing to go unbalanced if we don't have Veraero2 yet
            if (BlackMana > WhiteMana && Core.Me.ClassLevel >= Spells.Veraero2.LevelAcquired)
                return false;

            if (!RedMageSettings.Instance.Ver2)
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < RedMageSettings.Instance.Ver2Enemies)
                return false;

            else
                return await Spells.Verthunder2.Cast(Core.Me.CurrentTarget);
        }
    }
}
