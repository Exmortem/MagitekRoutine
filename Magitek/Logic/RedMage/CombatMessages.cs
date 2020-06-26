using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using Magitek.Extensions;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Utilities.CombatMessages;
using Magitek.Utilities.Routines;
using static Magitek.Utilities.Routines.RedMage;

namespace Magitek.Logic.RedMage
{
    class CombatMessages
    {
        private static HashSet<RdmStateIds> mComboStates = new HashSet<RdmStateIds>() { RdmStateIds.Zwerchhau, RdmStateIds.Redoublement, RdmStateIds.VerflareOrVerholy, RdmStateIds.Scorch };

        public static void RegisterCombatMessages(StateMachine<RdmStateIds> stateMachine)
        {
            Func<bool> bossPresenceOk = () => !RedMageSettings.Instance.MeleeComboBossesOnly || Combat.Enemies.Any(e => e.IsBoss());
            Func<bool> InMeleeCombo = () => mComboStates.Contains(RdmStateMachine.StateMachine.CurrentState);

            //Highest priority: Don't show anything if we're not in combat
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(100,
                                          "",
                                          () => !Core.Me.InCombat));

            //Second priority: Melee combo is ready
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(200,
                                          "Combo Ready!",
                                          () =>    stateMachine.CurrentState == RdmStateIds.ReadyForCombo
                                                && SmUtil.SyncedLevel >= Spells.Redoublement.LevelAcquired
                                                && bossPresenceOk()));

            //Third priority (tie): Melee combo will be ready soon (full combo, no manafication)
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    WithinManaOf(11, 80)
                                                && SmUtil.SyncedLevel >= Spells.Redoublement.LevelAcquired
                                                && !InMeleeCombo()
                                                && bossPresenceOk()));

            //Third priority (tie): Melee combo will be ready soon (full combo, manafication)
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    !WithinManaOf(0, RedMageSettings.Instance.ManaficationMaximumBlackAndWhiteMana)
                                                && (   (   WithinManaOf(0, RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                                                        && Spells.Manafication.Cooldown.TotalMilliseconds <= 1000)
                                                    || (   WithinManaOf(11, RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                                                        && !WithinManaOf(0, RedMageSettings.Instance.ManaficationMinimumBlackAndWhiteMana)
                                                        && Spells.Manafication.Cooldown.TotalMilliseconds <= 5000))
                                                && ManaficationEnabled
                                                && Spells.Manafication.Cooldown.TotalMilliseconds == 0
                                                && !InMeleeCombo()
                                                && bossPresenceOk()));

            //Third priority (tie): Melee combo will be ready soon (no redoublement)
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    WithinManaOf(11, 55)
                                                && SmUtil.SyncedLevel < Spells.Redoublement.LevelAcquired
                                                && SmUtil.SyncedLevel >= Spells.Zwerchhau.LevelAcquired
                                                && !InMeleeCombo()
                                                && bossPresenceOk()));
        }
    }
}
