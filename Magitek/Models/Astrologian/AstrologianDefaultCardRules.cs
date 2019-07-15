using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using Clio.Utilities.Collections;
using ff14bot.Enums;
using ff14bot.Managers;


namespace Magitek.Models.Astrologian
{
    public static class AstrologianDefaultCardRules
    {
        private static int _cardRuleCounter = 0;

        public static readonly ObservableCollection<CardRule> DefaultCardRules = new ObservableCollection<CardRule>()
        {
            #region Solo / Drawn / Balance
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Action = CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Drawn / Bole
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.StopLogic
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Action = CardAction.Spread},
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Drawn / Arrow
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Action =  CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Drawn / Spear
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Action =  CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Drawn / Ewer
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                TargetConditions = new TargetConditions()
                {
                    MpLessThan = 70f
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.StopLogic
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Action = CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Drawn / Spire
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Action = CardAction.Spread,
                },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Spread / Balance
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Spread / Bole
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Spread / Arrow
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Spread / Spear
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play
            },
            #endregion

            #region Solo / Spread / Ewer
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                TargetConditions = new TargetConditions()
                {
                    MpLessThan = 70f
                }
            },
            #endregion

            #region Solo / Spread / Spire
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Solo,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play
            },
            #endregion

            #region Pvp / Drawn / Arrow
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk,ClassJobType.Ninja,ClassJobType.Pugilist,ClassJobType.Rogue
                    },
                    JobOrder = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk,ClassJobType.Ninja,ClassJobType.Pugilist,ClassJobType.Rogue
                    }
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    },
                    Choice = CardChoiceType.Random
                }
            },
            #endregion

            #region Pvp / Drawn / Balance
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps,CardRole.Tank
                    },
                    JobOrder = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk,ClassJobType.BlackMage,ClassJobType.Ninja,ClassJobType.Summoner,ClassJobType.Pugilist,ClassJobType.Thaumaturge,ClassJobType.Rogue,ClassJobType.Arcanist
                    }
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    },
                    Choice = CardChoiceType.Random
                }
            },
            #endregion

            #region Pvp / Drawn / Bole
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    HpLessThan = 90f
                }
            },
            #endregion

            #region Pvp / Drawn / Ewer
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.WhiteMage,ClassJobType.Conjurer,ClassJobType.Astrologian
                    },
                    MpLessThan = 80f
                }
            },
            #endregion

            #region Pvp / Drawn / Spire
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk,ClassJobType.Pugilist,ClassJobType.Dragoon,ClassJobType.Lancer,ClassJobType.Ninja,ClassJobType.Rogue,ClassJobType.Paladin,ClassJobType.Gladiator,ClassJobType.Warrior,ClassJobType.Marauder,ClassJobType.DarkKnight
                    },
                    TpLessThan = 50f
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk,ClassJobType.Pugilist,ClassJobType.Dragoon,ClassJobType.Lancer,ClassJobType.Ninja,ClassJobType.Rogue,ClassJobType.Paladin,ClassJobType.Gladiator,ClassJobType.Warrior,ClassJobType.Marauder,ClassJobType.DarkKnight
                    },
                    Choice = CardChoiceType.Random
                }
            },
            #endregion

            #region Pvp / Drawn / Spear
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Pvp,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    Choice = CardChoiceType.Random
                }
            },
            #endregion

            #region Group / Drawn / Arrow
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk,ClassJobType.Ninja,ClassJobType.Pugilist,ClassJobType.Rogue
                    },
                    JobOrder = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk,ClassJobType.Ninja,ClassJobType.Pugilist,ClassJobType.Rogue
                    }
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    },
                    Choice = CardChoiceType.Random
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true,
                    DoesntHaveHeldCard = new AsyncObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>()
                    {
                        ActionResourceManager.Astrologian.AstrologianCard.Balance,ActionResourceManager.Astrologian.AstrologianCard.Bole
                    }
                },
                Action = CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsJob = new AsyncObservableCollection<ClassJobType>
                    {
                        ClassJobType.BlackMage,ClassJobType.Monk,ClassJobType.Ninja,ClassJobType.Thaumaturge,ClassJobType.Pugilist,ClassJobType.Rogue
                    },
                    JobOrder = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.BlackMage,ClassJobType.Monk,ClassJobType.Ninja,ClassJobType.Thaumaturge,ClassJobType.Pugilist,ClassJobType.Rogue
                    }
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    },
                    Choice = CardChoiceType.Random
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Action = CardAction.MinorArcana
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Action = CardAction.Undraw
            },
            #endregion

            #region Group / Drawn / Balance
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps,CardRole.Tank
                    },
                    JobOrder = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk, ClassJobType.BlackMage, ClassJobType.Ninja,ClassJobType.Summoner,ClassJobType.Pugilist,ClassJobType.Thaumaturge,ClassJobType.Rogue,ClassJobType.Arcanist
                    }
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true,
                    HasHeldCard = new AsyncObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>()
                    {
                        ActionResourceManager.Astrologian.AstrologianCard.Balance,ActionResourceManager.Astrologian.AstrologianCard.Bole
                    }
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps,CardRole.Tank
                    },
                    JobOrder = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk, ClassJobType.BlackMage, ClassJobType.Ninja,ClassJobType.Summoner,ClassJobType.Pugilist,ClassJobType.Thaumaturge,ClassJobType.Rogue,ClassJobType.Arcanist
                    }
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Action = CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    DoesntHaveHeldCard = new AsyncObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>()
                    {
                        ActionResourceManager.Astrologian.AstrologianCard.Balance,ActionResourceManager.Astrologian.AstrologianCard.Bole
                    }
                },
                Action = CardAction.UndrawSpread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.MinorArcana
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.Undraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    },
                    Choice = CardChoiceType.Random
                }
            },
            #endregion

            #region Group / Drawn / Bole
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    },
                    HpLessThan = 90f
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Action = CardAction.Spread
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.MinorArcana
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.Undraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    Choice = CardChoiceType.Random
                }
            },
            #endregion

            #region Group / Drawn / Ewer
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.WhiteMage,ClassJobType.Conjurer,ClassJobType.Astrologian
                    },
                    MpLessThan = 50f
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.WhiteMage, ClassJobType.Conjurer, ClassJobType.Astrologian
                    },
                    MpLessThan = 80f
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Action = CardAction.MinorArcana
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Action = CardAction.Undraw
            },
            #endregion
            
            #region Group / Drawn / Spire
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk, ClassJobType.Pugilist, ClassJobType.Dragoon, ClassJobType.Lancer, ClassJobType.Ninja, ClassJobType.Rogue, ClassJobType.Paladin, ClassJobType.Gladiator, ClassJobType.Warrior, ClassJobType.Marauder, ClassJobType.DarkKnight
                    },
                    TpLessThan = 50f
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions(),
                Action = CardAction.Undraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsJob = new AsyncObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Monk, ClassJobType.Pugilist, ClassJobType.Dragoon, ClassJobType.Lancer, ClassJobType.Ninja, ClassJobType.Rogue, ClassJobType.Paladin, ClassJobType.Gladiator, ClassJobType.Warrior, ClassJobType.Marauder, ClassJobType.DarkKnight
                    },
                    Choice = CardChoiceType.Random
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++, 
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Action = CardAction.Undraw
            },

            #endregion
            
            #region Group / Drawn / Spear
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.MinorArcana
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = false
                },
                Action = CardAction.Undraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    Choice = CardChoiceType.Random
                }
            },
            #endregion

            #region Group / Spread / Balance
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    },
                    WithAlliesNearbyMoreThan = 3
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    WithAlliesNearbyMoreThan = 3
                }
            },
            
            #endregion
            
            #region Group / Spread / Bole
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Tank
                    },
                    HpLessThan = 60
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    HpLessThan = 30
                }
            },

            #endregion

            #region Group / Spread / Ewer

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = null,
                    MpLessThan = 80,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Healer
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = null,
                    MpLessThan = 30,
                    IsJob = new ObservableCollection<ClassJobType>()
                    {
                        ClassJobType.DarkKnight,
                        ClassJobType.RedMage,
                        ClassJobType.Summoner,
                        ClassJobType.Bard,
                        ClassJobType.Arcanist,
                        ClassJobType.Archer
                    }
                }
            },

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.UndrawSpread
            },
            
            #endregion

            #region Group / Spread / Spire

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = null,
                    TpLessThan = 80,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Tank
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = null,
                    TpLessThan = 50,
                    IsJob = new ObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Bard,
                        ClassJobType.Archer,
                        ClassJobType.Machinist,
                        ClassJobType.Dragoon,
                        ClassJobType.Lancer,
                        ClassJobType.Monk,
                        ClassJobType.Pugilist,
                        ClassJobType.Ninja,
                        ClassJobType.Rogue,
                        ClassJobType.Samurai
                    }
                }
            },

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.UndrawSpread
            },

            #endregion

            #region Group / Spread / Arrow

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.UndrawSpread
            },

            #endregion

            #region Group / Spread / Spear

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.Me,
                TargetConditions = new TargetConditions()
                {
                    HpLessThan = 50
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.Party,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.UndrawSpread
            },

            #endregion

            #region Raid / Draw / Balance

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true,
                    HasHeldCard = new ObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>()
                    {
                        ActionResourceManager.Astrologian.AstrologianCard.Bole,
                        ActionResourceManager.Astrologian.AstrologianCard.Balance
                    }
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true                   
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Tank,
                        CardRole.Dps
                    }
                }
            },

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.Spread
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = false,
                    HasHeldCard = new ObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>()
                    {
                        ActionResourceManager.Astrologian.AstrologianCard.Bole,
                        ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                        ActionResourceManager.Astrologian.AstrologianCard.Balance,
                        ActionResourceManager.Astrologian.AstrologianCard.Spear,
                        ActionResourceManager.Astrologian.AstrologianCard.Spire,
                        ActionResourceManager.Astrologian.AstrologianCard.Arrow
                    }
                },
                Action = CardAction.Redraw
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = false,
                    HasHeldCard = new ObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>()
                    {
                        ActionResourceManager.Astrologian.AstrologianCard.Bole,
                        ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                        ActionResourceManager.Astrologian.AstrologianCard.Balance,
                        ActionResourceManager.Astrologian.AstrologianCard.Spear,
                        ActionResourceManager.Astrologian.AstrologianCard.Spire,
                        ActionResourceManager.Astrologian.AstrologianCard.Arrow
                    }
                    
                },
                Action = CardAction.MinorArcana
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = false,
                    HasHeldCard = new ObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>()
                    {
                        ActionResourceManager.Astrologian.AstrologianCard.Bole,
                        ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                        ActionResourceManager.Astrologian.AstrologianCard.Balance,
                        ActionResourceManager.Astrologian.AstrologianCard.Spear,
                        ActionResourceManager.Astrologian.AstrologianCard.Spire,
                        ActionResourceManager.Astrologian.AstrologianCard.Arrow
                    }
                    
                },
                Action = CardAction.Undraw
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true,
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Tank,
                        CardRole.Dps
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true
                }
            },
            
            #endregion

            #region Raid / Drawn / Bole

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.Redraw
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HpLessThan = 90,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Tank
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.Spread
            },

            #endregion

            #region Raid / Draw / Ewer

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    MpLessThan = 70,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Healer
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    MpLessThan = 50,
                    IsJob = new ObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Scholar,
                        ClassJobType.WhiteMage,
                        ClassJobType.Conjurer,
                        ClassJobType.Astrologian,
                        ClassJobType.RedMage,
                        ClassJobType.Summoner,
                        ClassJobType.Arcanist
                    }
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Action = CardAction.Redraw
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions(),
                Action = CardAction.Spread
            },

            #endregion

            #region Raid / Drawn / Spire
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Tank
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsJob = new ObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Bard,
                        ClassJobType.Archer,
                        ClassJobType.Machinist,
                        ClassJobType.Dragoon,
                        ClassJobType.Lancer,
                        ClassJobType.Monk,
                        ClassJobType.Pugilist,
                        ClassJobType.Ninja,
                        ClassJobType.Rogue,
                        ClassJobType.Samurai
                    }
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Action = CardAction.Redraw
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions(),
                Action = CardAction.Spread
            },

            #endregion

            #region Raid / Drawn / Arrow

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Action = CardAction.Redraw
            },   
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    }
                }
            },  
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions(),
                Action = CardAction.Spread
            },  
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true,
                    DoesntHaveHeldCard = new ObservableCollection<ActionResourceManager.Astrologian.AstrologianCard>()
                    {
                        ActionResourceManager.Astrologian.AstrologianCard.Bole,
                        ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                        ActionResourceManager.Astrologian.AstrologianCard.Balance,
                        ActionResourceManager.Astrologian.AstrologianCard.Spear,
                        ActionResourceManager.Astrologian.AstrologianCard.Spire,
                        ActionResourceManager.Astrologian.AstrologianCard.Arrow
                    }
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
            },  

            #endregion

            #region Raid / Drawn / Spear
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Action = CardAction.Redraw
            }, 
                        
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HpLessThan = 50
                }
            },  
                        
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Drawn,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions(),
                Action = CardAction.Spread
            },  

            #endregion
            
            #region Raid / Spread / Balance
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    },
                    WithAlliesNearbyMoreThan = 3
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Balance,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    WithAlliesNearbyMoreThan = 3
                }
            },
            
            #endregion
            
            #region Raid / Spread / Bole
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new AsyncObservableCollection<CardRole>()
                    {
                        CardRole.Tank
                    },
                    HpLessThan = 60
                }
            },
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Bole,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    HpLessThan = 30
                }
            },

            #endregion

            #region Raid / Spread / Ewer

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = null,
                    MpLessThan = 80,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Healer
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = null,
                    MpLessThan = 30,
                    IsJob = new ObservableCollection<ClassJobType>()
                    {
                        ClassJobType.DarkKnight,
                        ClassJobType.RedMage,
                        ClassJobType.Summoner,
                        ClassJobType.Bard,
                        ClassJobType.Arcanist,
                        ClassJobType.Archer
                    }
                }
            },

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Ewer,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.UndrawSpread
            },
            
            #endregion

            #region Raid / Spread / Spire

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = null,
                    TpLessThan = 80,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Tank
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = null,
                    TpLessThan = 50,
                    IsJob = new ObservableCollection<ClassJobType>()
                    {
                        ClassJobType.Bard,
                        ClassJobType.Archer,
                        ClassJobType.Machinist,
                        ClassJobType.Dragoon,
                        ClassJobType.Lancer,
                        ClassJobType.Monk,
                        ClassJobType.Pugilist,
                        ClassJobType.Ninja,
                        ClassJobType.Rogue,
                        ClassJobType.Samurai
                    }
                }
            },

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spire,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.UndrawSpread
            },

            #endregion

            #region Raid / Spread / Arrow

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.PartyMember,
                TargetConditions = new TargetConditions()
                {
                    HasTarget = true,
                    IsRole = new ObservableCollection<CardRole>()
                    {
                        CardRole.Dps
                    }
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Arrow,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.UndrawSpread
            },

            #endregion

            #region Raid / Spread / Spear

            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = true
                },
                Action = CardAction.Play,
                Target = CardTarget.Me,
                TargetConditions = new TargetConditions()
                {
                    HpLessThan = 50
                }
            },
            
            new CardRule()
            {
                CardPriority = _cardRuleCounter++,
                LogicType = CardLogicType.LargeParty,
                PlayType = CardPlayType.Held,
                Card = ActionResourceManager.Astrologian.AstrologianCard.Spear,
                Conditions = new Conditions()
                {
                    InCombat = null
                },
                Action = CardAction.UndrawSpread
            },

            #endregion
        };
    }
}

