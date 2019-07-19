using Magitek.Gambits.Actions;

namespace Magitek.Gambits.Helpers
{
    internal static class ActionHelpers
    {
        public static void AddActionToGambit(Gambit gambit, GambitActionTypes selectedValue)
        {
            IGambitAction newAction;

            switch (selectedValue)
            {
                case GambitActionTypes.NoAction:
                    newAction = new NullAction();
                    break;
                case GambitActionTypes.CastSpellOnSelf:
                    newAction = new CastSpellOnSelfAction { SpellName = "The Spell's Name" };
                    break;
                case GambitActionTypes.CastSpellOnAlly:
                    newAction = new CastSpellOnAllyAction { SpellName = "The Spell's Name" };
                    break;
                case GambitActionTypes.CastSpellOnEnemy:
                    newAction = new CastSpellOnEnemyAction { SpellName = "The Spell's Name" };
                    break;
                case GambitActionTypes.CastSpellOnFriendlyNpc:
                    newAction = new CastSpellOnFriendlyNpcAction { SpellName = "The Spell's Name" };
                    break;
                case GambitActionTypes.SleepForMilliseconds:
                    newAction = new SleepForTimeAction { DurationInMilliseconds = 1000 };
                    break;
                case GambitActionTypes.ToastMessage:
                    newAction = new ToastMessageAction { displaySeconds = 2, message = "The Toast Message" };
                    break;
                case GambitActionTypes.UseItemOnSelf:
                    newAction = new UseItemOnSelfAction() { ItemName = "The Item's Name", AnyQuality = true };
                    break;
                case GambitActionTypes.CastSpellOnCurrentTarget:
                    newAction = new CastSpellOnCurrentTargetAction() { SpellName = "The Spell's Name" };
                    break;
                case GambitActionTypes.CastFillerOnCurrentTarget:
                    newAction = new CastFillerOnCurrentTargetAction() { SpellName = "The Filler's SpellName", ProcName = "The Proc's SpellName" };
                    break;
                case GambitActionTypes.PetCast:
                    newAction = new PetCastAction() { SpellName = "The Spell's Name" };
                    break;
                default:
                    return;
            }

            gambit.ActionType = selectedValue;
            gambit.Action = newAction;
        }
    }
}
