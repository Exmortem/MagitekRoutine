using System;
using Magitek.Gambits.Conditions;

namespace Magitek.Gambits.Helpers
{
    internal static class ConditionHelpers
    {
        public static void AddConditionToGambit(Gambit gambit, string conditionString)
        {
            var randomNumber = new Random().Next(int.MaxValue);
            var condition = ConditionFromString(conditionString);
            condition.Id = randomNumber;
            gambit.Conditions.Add(condition);
        }

        public static void AddConditionToOpenerGroup(OpenerGroup openerGroup, string conditionString)
        {
            var randomNumber = new Random().Next(int.MaxValue);
            var condition = ConditionFromString(conditionString);
            condition.Id = randomNumber;
            openerGroup.StartOpenerConditions.Add(condition);
        }

        private static IGambitCondition ConditionFromString(string conditionString)
        {
            IGambitCondition condition;

            switch (conditionString)
            {
                 case "EnemyCastingSpell":
                    condition = new EnemyCastingSpellCondition { SpellName = "Spell Name", TargetAnyone = true };
                    break;

                case "HasAura":
                    condition = new HasAuraCondition { AuraName = "Aura Name" };
                    break;

                case "SpellOffCooldown":
                    condition = new SpellOffCooldownCondition { SpellName = "The Spell's Name" };
                    break;

                case "HpPercentBelow":
                    condition = new HpPercentBelowCondition();
                    break;

                case "HpPercentBetween":
                    condition = new HpPercentBetweenCondition();
                    break;

                 case "InInstance":
                     condition = new InInstanceCondition();
                     break;

                 case "NotInInstance":
                     condition = new NotInInstanceCondition();
                     break;

                case "IsJob":
                    condition = new IsJobCondition();
                    break;

                case "IsRole":
                    condition = new IsRoleCondition();
                    break;

                case "PlayerHasAura":
                    condition = new PlayerHasAuraCondition { AuraName = "Aura Name" };
                    break;

                case "TargetName":
                    condition = new TargetNameCondition { TargetName = "Target Name" };
                    break;

                case "LastSpell":
                    condition = new LastSpellCondition { SpellName = "Spell Name" };
                    break;

                case "EnemiesNearby":
                    condition = new EnemiesNearbyCondition { Count = 2, Range = 5, ByPlayer = false, ByTargetObject = true };
                    break;

                case "PlayerDoesNotHaveAura":
                    condition = new PlayerDoesNotHaveAuraCondition() { AuraName = "Aura Name" };
                    break;

                case "DoesNotHaveAura":
                    condition = new DoesNotHaveAuraCondition() { AuraName = "Aura Name" };
                    break;

                case "PlayerInCombat":
                    condition = new PlayerInCombatCondition();
                    break;

                case "PlayerNotInCombat":
                    condition = new PlayerNotInCombatCondition();
                    break;

                case "MonkChakra":
                    condition = new MonkChakraCondition();
                    break;

                case "MonkGreasedLightning":
                    condition = new MonkGreasedLightningCondition();
                    break;

                case "BardRepertoire":
                    condition = new BardRepertoireCondition();
                    break;

                case "MachinistHeat":
                    condition = new MachinistHeatCondition();
                    break;

                case "DragoonGaze":
                    condition = new DragoonGazeCondition();
                    break;

                case "DragoonGaugeTimer":
                    condition = new DragoonGaugeTimerCondition();
                    break;

                case "SamuraiSen":
                    condition = new SamuraiSenCondition();
                    break;

                case "SamuraiKenki":
                    condition = new SamuraiKenkiCondition();
                    break;

                case "NinjaNinki":
                    condition = new NinjaNinkiCondition();
                    break;

                case "NinjaHuton":
                    condition = new NinjaHutonCondition();
                    break;

                case "TargetIsBoss":
                    condition = new TargetIsBossCondition();
                    break;

                case "HasPet":
                    condition = new HasPetCondition();
                    break;

                case "CombatTime":
                    condition = new CombatTimeCondition();
                    break;

                default:
                    return null;
            }

            return condition;
        }
    }
}
