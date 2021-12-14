using System.Collections.Generic;
using System.Linq;
using ff14bot.Enums;
using Magitek.Models;

namespace Magitek.Utilities.CombatMessages
{
    class CombatMessageManager
    {
        private static List<ICombatMessageStrategy> mMessageStrategies = new List<ICombatMessageStrategy>();

        public static void ClearMessageStrategies()
        {
            mMessageStrategies.Clear();
        }

        public static void RegisterMessageStrategy(ICombatMessageStrategy strategy)
        {
            if (strategy != null)
            {
                mMessageStrategies.Add(strategy);
                //Keep the list sorted. Would be much more efficient to sort at end, but
                //we're unlikely to have enough strategies for it to be a problem.
                mMessageStrategies = mMessageStrategies.OrderBy(m => m.Priority).ToList();
            }
        }

        //Called by the pulse handler to set the appropriate message
        //If no messages should be shown, the existing message is cleared
        public static void UpdateDisplayedMessage()
        {
            foreach (ICombatMessageStrategy message in mMessageStrategies)
            {
                if (message.ShowMessage())
                {
                    CombatMessageModel.Instance.Message = message.Message;
                    CombatMessageModel.Instance.ImageSource = message.ImageSource;
                    return;
                }
            }
            CombatMessageModel.Instance.ClearMessage();
        }

        static ClassJobType? mCurrentClass = null;

        //To add combat messages for a class, update this method to call out to a method that will register the
        //desired class's message strategies.
        //
        //Each method called by this method should call RegisterMessage() to add all desired message strategies
        //for its respective class
        //
        //See Rotations.RedMage.RegisterCombatMessages() for an example. For details on how the message strategies
        //work, see ICombatMessageStrategy
        //
        public static void RegisterMessageStrategiesForClass(ClassJobType currentClass)
        {
            if (mCurrentClass == currentClass)
            {
                return;
            }

            mCurrentClass = currentClass;

            mMessageStrategies.Clear();

            switch (currentClass)
            {
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    break;

                case ClassJobType.Pugilist:
                case ClassJobType.Monk:
                    Rotations.Monk.RegisterCombatMessages();
                    break;

                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                    break;

                case ClassJobType.Lancer:
                case ClassJobType.Dragoon:
                    break;

                case ClassJobType.Archer:
                case ClassJobType.Bard:
                    break;

                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                    break;

                case ClassJobType.Thaumaturge:
                case ClassJobType.BlackMage:
                    break;

                case ClassJobType.Arcanist:
                case ClassJobType.Summoner:
                    break;

                case ClassJobType.Scholar:
                    break;

                case ClassJobType.Rogue:
                case ClassJobType.Ninja:
                    break;

                case ClassJobType.Machinist:
                    break;

                case ClassJobType.DarkKnight:
                    break;

                case ClassJobType.Astrologian:
                    break;

                case ClassJobType.Samurai:
                    break;

                case ClassJobType.BlueMage:
                    break;

                case ClassJobType.RedMage:
                    Rotations.RedMage.RegisterCombatMessages();
                    break;

                case ClassJobType.Gunbreaker:
                    break;

                case ClassJobType.Dancer:
                    break;

                case ClassJobType.Reaper:
                    break;
            }
        }
    }
}
