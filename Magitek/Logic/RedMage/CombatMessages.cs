using System;
using ff14bot;
using Magitek.Utilities;
using Magitek.Utilities.CombatMessages;
using Magitek.Utilities.Routines;
using static Magitek.Utilities.Routines.RedMage;

namespace Magitek.Logic.RedMage
{
    class CombatMessages
    {
        public static void RegisterCombatMessages(StateMachine<RdmStateIds> stateMachine)
        {
            Func<bool> bossPresenceOk = () => CanComboSomeEnemySt && !AoeMode;

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
                                                && SmUtil.SyncedLevel >= Spells.Redoublement.LevelAcquired));

            //Third priority:
            CombatMessageManager.RegisterMessageStrategy(
                new CombatMessageStrategy(300,
                                          "Combo Soon",
                                          () =>    WithinManaOf(15, ComboTargetMana)
                                                && bossPresenceOk()));
        }
    }
}
