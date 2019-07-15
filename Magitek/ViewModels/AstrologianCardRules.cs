using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Clio.Utilities.Collections;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Commands;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using Newtonsoft.Json;
using PropertyChanged;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class AstrologianCardRules : INotifyPropertyChanged
    {
        private static AstrologianCardRules _instance;
        public static AstrologianCardRules Instance => _instance ?? (_instance = new AstrologianCardRules());

        private AstrologianCardRules()
        {
            if (AstrologianSettings.Instance.CardRules == null)
                AstrologianSettings.Instance.CardRules = AstrologianDefaultCardRules.DefaultCardRules;

            CardRules = new ObservableCollection<CardRule>(AstrologianSettings.Instance.CardRules);
            CollectionViewSource = System.Windows.Data.CollectionViewSource.GetDefaultView(CardRules);
            CollectionViewSource.SortDescriptions.Add(new SortDescription("CardPriority", ListSortDirection.Ascending));
            ResetCollectionViewSource();
            ReloadUiElements();
        }

        public ICollectionView CollectionViewSource { get; set; }
        public ObservableCollection<CardRule> CardRules { get; set; }
        public CardRule SelectedCardRules { get; set; }
        public int SelectedCardRuleIndex { get; set; } = 0;

        #region Filter Properties
        private string _logicType;

        public string LogicType
        {
            get => _logicType;
            set
            {
                _logicType = value;
                ResetCollectionViewSource();
                OnPropertyChanged();
            }
        }

        private string _card;

        public string Card
        {
            get => _card;
            set
            {
                _card = value;
                ResetCollectionViewSource();
                OnPropertyChanged();
            }
        }

        private string _cardPlayType;

        public string CardPlayType
        {
            get => _cardPlayType;
            set
            {
                _cardPlayType = value;
                ResetCollectionViewSource();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Collection View Source Reset
        public void ResetCollectionViewSource()
        {
            CollectionViewSource.Filter = r =>
            {
                var cardRule = (CardRule) r;

                if (cardRule == null)
                    return false;

                switch (LogicType)
                {
                    case "Solo":
                        if (cardRule.LogicType != CardLogicType.Solo)
                            return false;
                        break;
                    case "Group":
                        if (cardRule.LogicType != CardLogicType.Party)
                            return false;
                        break;
                    case "Raid":
                        if (cardRule.LogicType != CardLogicType.LargeParty)
                            return false;
                        break;
                    case "Pvp":
                        if (cardRule.LogicType != CardLogicType.Pvp)
                            return false;
                        break;

                    default:
                        break;
                }

                switch (Card)
                {
                    case "All":
                        break;

                    case "Balance":
                        if (cardRule.Card != ActionResourceManager.Astrologian.AstrologianCard.Balance)
                            return false;
                        break;

                    case "Bole":
                        if (cardRule.Card != ActionResourceManager.Astrologian.AstrologianCard.Bole)
                            return false;
                        break;

                    case "Spire":
                        if (cardRule.Card != ActionResourceManager.Astrologian.AstrologianCard.Spire)
                            return false;
                        break;

                    case "Spear":
                        if (cardRule.Card != ActionResourceManager.Astrologian.AstrologianCard.Spear)
                            return false;
                        break;

                    case "Ewer":
                        if (cardRule.Card != ActionResourceManager.Astrologian.AstrologianCard.Ewer)
                            return false;
                        break;

                    case "Arrow":
                        if (cardRule.Card != ActionResourceManager.Astrologian.AstrologianCard.Arrow)
                            return false;
                        break;

                    case "Lord":
                        if (cardRule.Card != ActionResourceManager.Astrologian.AstrologianCard.LordofCrowns)
                            return false;
                        break;

                    case "Lady":
                        if (cardRule.Card != ActionResourceManager.Astrologian.AstrologianCard.LadyofCrowns)
                            return false;
                        break;

                    default:
                        break;
                }

                switch (CardPlayType)
                {
                    case "Both":
                        break;

                    case "Held":
                        if (cardRule.PlayType != Models.Astrologian.CardPlayType.Held)
                            return false;
                        break;

                    case "Drawn":
                        if (cardRule.PlayType != Models.Astrologian.CardPlayType.Drawn)
                            return false;
                        break;

                    default:
                        break;
                }

                return true;
            };            

            CollectionViewSource.Refresh();
        }
        #endregion

        #region Save / Load

        public ICommand ApplyCardRules => new DelegateCommand(ResetAstrologianCardRules);

        public ICommand SaveCardRules => new DelegateCommand(() =>
        {
            var saveFile = new SaveFileDialog
            {
                Filter = "json files (*.json)|*.json",
                Title = "Save Card Rules File",
                OverwritePrompt = true
            };

            if (saveFile.ShowDialog() != true)
                return;

            var data = JsonConvert.SerializeObject(CardRules, Formatting.Indented);
            File.WriteAllText(saveFile.FileName, data);
            Logger.Write($@"Card Rules Exported Under {saveFile.FileName} ");
        });

        public ICommand LoadCardRules => new DelegateCommand(() =>
        {
            var loadFile = new OpenFileDialog()
            {
                Filter = "json files (*.json)|*.json",
                Title = "Open Card Rules File"
            };

            if (loadFile.ShowDialog() == true)
            {
                AstrologianSettings.Instance.CardRules = JsonConvert.DeserializeObject<ObservableCollection<CardRule>>(File.ReadAllText(loadFile.FileName));
                CardRules = new ObservableCollection<CardRule>(AstrologianSettings.Instance.CardRules);
                CollectionViewSource = System.Windows.Data.CollectionViewSource.GetDefaultView(CardRules);
                CollectionViewSource.SortDescriptions.Add(new SortDescription("CardPriority", ListSortDirection.Ascending));
                ResetCollectionViewSource();
            }

            if (string.IsNullOrEmpty(loadFile.FileName))
                return;

            Logger.Write($@"Card Rules Loaded");
            SelectedCardRuleIndex = 0;
        });

        public ICommand LoadDefaultCardRules => new DelegateCommand(() =>
        {
            Logger.WriteInfo(@"Resetting Card Rules to Defaults. Writing " +
                             AstrologianDefaultCardRules.DefaultCardRules.Count + " Rules to Settings.");

            CardRules = new ObservableCollection<CardRule>(AstrologianDefaultCardRules.DefaultCardRules);
            CollectionViewSource = System.Windows.Data.CollectionViewSource.GetDefaultView(CardRules);
            CollectionViewSource.SortDescriptions.Add(new SortDescription("CardPriority", ListSortDirection.Ascending));
            ResetCollectionViewSource();
            SelectedCardRuleIndex = 0;
        });

        public void ResetAstrologianCardRules()
        {
            if (CardRules == null)
                return;

            AstrologianSettings.Instance.CardRules = new ObservableCollection<CardRule>(CardRules);
            ResetCollectionViewSource();

            if (BaseSettings.Instance.CurrentRoutine != "Astrologian")
                return;

            Logger.WriteInfo("New Card Rules Applied");
        }

        #endregion

        #region Add New Card Command
        public ICommand AddNewCard => new DelegateCommand(() =>
        {
            var newPriority = 1;

            if (CardRules.Any())
            {
                newPriority = CardRules.Max(r => r.CardPriority) + 1;
            }
            
            var newCardLogicType = CardLogicType.Party;
            var newCardPlayType = Models.Astrologian.CardPlayType.Drawn;
            var newCardCard = ActionResourceManager.Astrologian.AstrologianCard.None;

            switch (LogicType)
            {
                case ("Group"):
                    newCardLogicType = CardLogicType.Party;
                    break;
                case ("Raid"):
                    newCardLogicType = CardLogicType.LargeParty;
                    break;
                case ("Solo"):
                    newCardLogicType = CardLogicType.Solo;
                    break;
                case ("Pvp"):
                    newCardLogicType = CardLogicType.Pvp;
                    break;
                default:
                    newCardLogicType = CardLogicType.LargeParty;
                    break;
            }

            switch (CardPlayType)
            {
                case ("Held"):
                    newCardPlayType = Models.Astrologian.CardPlayType.Held;
                    break;
                default:
                    newCardPlayType = Models.Astrologian.CardPlayType.Drawn;
                    break;
            }
            switch (Card)
            {
                case ("All"):
                    newCardCard = SelectedCardRules.Card;
                    break;
                case ("Balance"):
                    newCardCard = ActionResourceManager.Astrologian.AstrologianCard.Balance;
                    break;
                case ("Bole"):
                    newCardCard = ActionResourceManager.Astrologian.AstrologianCard.Bole;
                    break;
                case ("Arrow"):
                    newCardCard = ActionResourceManager.Astrologian.AstrologianCard.Arrow;
                    break;
                case ("Spear"):
                    newCardCard = ActionResourceManager.Astrologian.AstrologianCard.Spear;
                    break;
                case ("Ewer"):
                    newCardCard = ActionResourceManager.Astrologian.AstrologianCard.Ewer;
                    break;
                case ("Spire"):
                    newCardCard = ActionResourceManager.Astrologian.AstrologianCard.Spire;
                    break;
                case ("Lady"):
                    newCardCard = ActionResourceManager.Astrologian.AstrologianCard.LadyofCrowns;
                    break;
                case ("Lord"):
                    newCardCard = ActionResourceManager.Astrologian.AstrologianCard.LordofCrowns;
                    break;
                default:
                    newCardCard = SelectedCardRules.Card;
                    break;
            }

            var newCard = new CardRule
            {
                CardPriority = newPriority,
                Card = newCardCard,
                LogicType = newCardLogicType,
                PlayType = newCardPlayType,
                Conditions = new Conditions(),
                Action = CardAction.Play,
                Target = CardTarget.Me,
                TargetConditions = new TargetConditions()
                {
                    TpLessThan = 100,
                    HpLessThan = 100,
                    MpLessThan = 100
                }
            };

            Logger.WriteInfo($"Adding new card at {newPriority}");
            CardRules.Add(newCard);
            //CardRules.Insert(newPriority, newCard);
            ResetCollectionViewSource();
            var newSelectedCardIndex = CollectionViewSource.Cast<CardRule>().Count() - 1;
            SelectedCardRuleIndex = newSelectedCardIndex;
        });
        #endregion

        #region Delete Selected Card Command
        public ICommand RemSelCard => new DelegateCommand(() =>
        {
            if (SelectedCardRules == null)
                return;

            var oldSelectedCardRuleIndex = SelectedCardRuleIndex;
            CardRules.Remove(SelectedCardRules);
            ResetCollectionViewSource();
            SelectedCardRuleIndex = oldSelectedCardRuleIndex;
        });
        #endregion

        // Toggle Buttons
        #region IsRole
        public bool IsRoleTank { get; set; }
        public bool IsRoleHealer { get; set; }
        public bool IsRoleDps { get; set; }

        public void ResetIsRoleSettingsUi()
        {
            try
            {
                if (SelectedCardRules == null)
                    return;

                if (SelectedCardRules.TargetConditions == null)
                {
                    SelectedCardRules.TargetConditions = new TargetConditions();
                }

                if (SelectedCardRules.TargetConditions.IsRole == null)
                {
                    SelectedCardRules.TargetConditions.IsRole =
                        new AsyncObservableCollection<CardRole>();
                }

                SelectedCardRules.TargetConditions.IsRole.Clear();

                if (IsRoleTank)
                {
                    SelectedCardRules.TargetConditions.IsRole.Add(CardRole.Tank);
                }

                if (IsRoleHealer)
                {
                    SelectedCardRules.TargetConditions.IsRole.Add(CardRole.Healer);
                }

                if (IsRoleDps)
                {
                    SelectedCardRules.TargetConditions.IsRole.Add(CardRole.Dps);
                }
            }
            catch
            {
                if (BaseSettings.Instance.GeneralSettings.DebugCastingCallerMemberName)
                {
                    Logger.WriteInfo(@"Something weird just happened with Card Rules. This is a known issue and is being worked on.");
                }
            }
        }

        private void LoadIsRoleSettingsUi()
        {
            if (SelectedCardRules?.TargetConditions == null)
            {
                IsRoleTank = false;
                IsRoleHealer = false;
                IsRoleDps = false;
                return;
            }

            var tempList = new List<CardRole>(SelectedCardRules.TargetConditions.IsRole);

            IsRoleTank = tempList.Contains(CardRole.Tank);
            IsRoleHealer = tempList.Contains(CardRole.Healer);
            IsRoleDps = tempList.Contains(CardRole.Dps);
        }
        #endregion

        #region IsJob
        public bool IsJobAstrologian { get; set; }
        public bool IsJobWhiteMage { get; set; }
        public bool IsJobScholar { get; set; }
        public bool IsJobPaladin { get; set; }
        public bool IsJobWarrior { get; set; }
        public bool IsJobDarkKnight { get; set; }
        public bool IsJobBard { get; set; }
        public bool IsJobMachinist { get; set; }
        public bool IsJobBlackMage { get; set; }
        public bool IsJobRedMage { get; set; }
        public bool IsJobSummoner { get; set; }
        public bool IsJobDragoon { get; set; }
        public bool IsJobMonk { get; set; }
        public bool IsJobNinja { get; set; }
        public bool IsJobSamurai { get; set; }

        public void ResetIsJobSettingsUi()
        {
            try
            {
                if (SelectedCardRules == null)
                    return;

                if (SelectedCardRules.TargetConditions == null)
                {
                    SelectedCardRules.TargetConditions = new TargetConditions();
                }

                if (SelectedCardRules.TargetConditions.IsJob == null)
                {
                    SelectedCardRules.TargetConditions.IsJob =
                        new AsyncObservableCollection<ClassJobType>();
                }

                SelectedCardRules.TargetConditions.IsJob.Clear();

                if (IsJobAstrologian)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Astrologian);
                }

                if (IsJobWhiteMage)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.WhiteMage);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Conjurer);
                }

                if (IsJobScholar)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Scholar);
                }

                if (IsJobPaladin)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Paladin);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Gladiator);
                }

                if (IsJobWarrior)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Warrior);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Marauder);
                }

                if (IsJobDarkKnight)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.DarkKnight);
                }

                if (IsJobBard)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Bard);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Archer);
                }

                if (IsJobMachinist)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Machinist);
                }

                if (IsJobBlackMage)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.BlackMage);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Thaumaturge);
                }

                if (IsJobRedMage)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.RedMage);
                }

                if (IsJobSummoner)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Summoner);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Arcanist);
                }

                if (IsJobDragoon)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Dragoon);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Lancer);
                }

                if (IsJobMonk)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Monk);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Pugilist);
                }

                if (IsJobNinja)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Ninja);
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Rogue);
                }

                if (IsJobSamurai)
                {
                    SelectedCardRules.TargetConditions.IsJob.Add(ClassJobType.Samurai);
                }
            }
            catch
            {
                if (BaseSettings.Instance.GeneralSettings.DebugCastingCallerMemberName)
                {
                    Logger.WriteInfo(@"Something weird just happened with Card Rules. This is a known issue and is being worked on.");
                }
            }
        }

        private void LoadIsJobSettingsUi()
        {
            if (SelectedCardRules?.TargetConditions == null)
            {
                IsJobAstrologian = false;
                IsJobWhiteMage = false;
                IsJobScholar = false;
                IsJobPaladin = false;
                IsJobWarrior = false;
                IsJobDarkKnight = false;
                IsJobBard = false;
                IsJobMachinist = false;
                IsJobBlackMage = false;
                IsJobRedMage = false;
                IsJobSummoner = false;
                IsJobDragoon = false;
                IsJobMonk = false;
                IsJobNinja = false;
                IsJobSamurai = false;
                return;
            }

            var tempList = new List<ClassJobType>(SelectedCardRules.TargetConditions.IsJob);

            IsJobAstrologian = tempList.Contains(ClassJobType.Astrologian);
            IsJobWhiteMage = tempList.Contains(ClassJobType.WhiteMage);
            IsJobScholar = tempList.Contains(ClassJobType.Scholar);
            IsJobPaladin = tempList.Contains(ClassJobType.Paladin);
            IsJobWarrior = tempList.Contains(ClassJobType.Warrior);
            IsJobDarkKnight = tempList.Contains(ClassJobType.DarkKnight);
            IsJobBard = tempList.Contains(ClassJobType.Bard);
            IsJobMachinist = tempList.Contains(ClassJobType.Machinist);
            IsJobBlackMage = tempList.Contains(ClassJobType.BlackMage);
            IsJobRedMage = tempList.Contains(ClassJobType.RedMage);
            IsJobSummoner = tempList.Contains(ClassJobType.Summoner);
            IsJobDragoon = tempList.Contains(ClassJobType.Dragoon);
            IsJobMonk = tempList.Contains(ClassJobType.Monk);
            IsJobNinja = tempList.Contains(ClassJobType.Ninja);
            IsJobSamurai = tempList.Contains(ClassJobType.Samurai);
        }
        #endregion

        #region HasHeldCard
        public bool DoesNotHaveHeldEwer { get; set; }
        public bool DoesNotHaveHeldBole { get; set; }
        public bool DoesNotHaveHeldBalance { get; set; }
        public bool DoesNotHaveHeldSpear { get; set; }
        public bool DoesNotHaveHeldNone { get; set; }
        public bool DoesNotHaveHeldSpire { get; set; }
        public bool DoesNotHaveHeldArrow { get; set; }

        public void ResetDoesNotHaveHeldCardSettingsUi()
        {
            try {
                if (SelectedCardRules == null)
                    return;

                if (SelectedCardRules.Conditions == null)
                {
                    SelectedCardRules.Conditions = new Conditions();
                }

                SelectedCardRules.Conditions.DoesntHaveHeldCard.Clear();

                if (DoesNotHaveHeldEwer)
                {
                    SelectedCardRules.Conditions.DoesntHaveHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard.Ewer);
                }

                if (DoesNotHaveHeldBole)
                {
                    SelectedCardRules.Conditions.DoesntHaveHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard.Bole);
                }

                if (DoesNotHaveHeldBalance)
                {
                    SelectedCardRules.Conditions.DoesntHaveHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard.Balance);
                }

                if (DoesNotHaveHeldSpear)
                {
                    SelectedCardRules.Conditions.DoesntHaveHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard.Spear);
                }

                if (DoesNotHaveHeldNone)
                {
                    SelectedCardRules.Conditions.DoesntHaveHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard.None);
                }

                if (DoesNotHaveHeldSpire)
                {
                    SelectedCardRules.Conditions.DoesntHaveHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard.Spire);
                }

                if (DoesNotHaveHeldArrow)
                {
                    SelectedCardRules.Conditions.DoesntHaveHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard.Arrow);
                }
            }
            catch
            {
                if (BaseSettings.Instance.GeneralSettings.DebugCastingCallerMemberName)
                {
                    Logger.WriteInfo(@"Something weird just happened with Card Rules. This is a known issue and is being worked on.");
                }
            }
        }

        private void LoadDoesNotHaveHeldCardSettingsUi()
        {
            if (SelectedCardRules?.Conditions == null)
            {
                DoesNotHaveHeldEwer = false;
                DoesNotHaveHeldBole = false;
                DoesNotHaveHeldBalance = false;
                DoesNotHaveHeldSpear = false;
                DoesNotHaveHeldNone = false;
                DoesNotHaveHeldSpire = false;
                DoesNotHaveHeldArrow = false;
                return;
            }

            var tempList = new List<ActionResourceManager.Astrologian.AstrologianCard>(SelectedCardRules.Conditions.DoesntHaveHeldCard);

            DoesNotHaveHeldEwer = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Ewer);
            DoesNotHaveHeldBole = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Bole);
            DoesNotHaveHeldBalance = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Balance);
            DoesNotHaveHeldSpear = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Spear);
            DoesNotHaveHeldNone = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.None);
            DoesNotHaveHeldSpire = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Spire);
            DoesNotHaveHeldArrow = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Arrow);
        }
        #endregion

        #region HasHeldCard
        public bool HasHeldEwer { get; set; }
        public bool HasHeldBole { get; set; }
        public bool HasHeldBalance { get; set; }
        public bool HasHeldSpear { get; set; }
        public bool HasHeldNone { get; set; }
        public bool HasHeldSpire { get; set; }
        public bool HasHeldArrow { get; set; }

        public void ResetHasHeldCardSettingsUi()
        {
            try
            {
                if (SelectedCardRules == null)
                    return;

                if (SelectedCardRules.Conditions == null)
                {
                    SelectedCardRules.Conditions = new Conditions();
                }

                //TODO: This null check didn't work -_-
                if (SelectedCardRules.Conditions.HasHeldCard == null)
                    return;

                SelectedCardRules.Conditions.HasHeldCard.Clear();


                if (HasHeldEwer)
                {
                    SelectedCardRules.Conditions.HasHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard
                        .Ewer);
                }

                if (HasHeldBole)
                {
                    SelectedCardRules.Conditions.HasHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard
                        .Bole);
                }

                if (HasHeldBalance)
                {
                    SelectedCardRules.Conditions.HasHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard
                        .Balance);
                }

                if (HasHeldSpear)
                {
                    SelectedCardRules.Conditions.HasHeldCard.Add(
                        ActionResourceManager.Astrologian.AstrologianCard.Spear);
                }

                if (HasHeldNone)
                {
                    SelectedCardRules.Conditions.HasHeldCard.Add(ActionResourceManager.Astrologian.AstrologianCard
                        .None);
                }

                if (HasHeldSpire)
                {
                    SelectedCardRules.Conditions.HasHeldCard.Add(
                        ActionResourceManager.Astrologian.AstrologianCard.Spire);
                }

                if (HasHeldArrow)
                {
                    SelectedCardRules.Conditions.HasHeldCard.Add(
                        ActionResourceManager.Astrologian.AstrologianCard.Arrow);
                }

            }
            catch
            {
                if (BaseSettings.Instance.GeneralSettings.DebugCastingCallerMemberName)
                {
                    Logger.WriteInfo(@"Something weird just happened with Card Rules. This is a known issue and is being worked on.");
                }
            }
        }

        private void LoadHasHeldCardSettingsUi()
        {
            if (SelectedCardRules?.Conditions == null)
            {
                HasHeldEwer = false;
                HasHeldBole = false;
                HasHeldBalance = false;
                HasHeldSpear = false;
                HasHeldNone = false;
                HasHeldSpire = false;
                HasHeldArrow = false;
                return;
            }

            var tempList = new List<ActionResourceManager.Astrologian.AstrologianCard>(SelectedCardRules.Conditions.HasHeldCard);

            HasHeldEwer = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Ewer);
            HasHeldBole = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Bole);
            HasHeldBalance = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Balance);
            HasHeldSpear = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Spear);
            HasHeldNone = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.None);
            HasHeldSpire = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Spire);
            HasHeldArrow = tempList.Contains(ActionResourceManager.Astrologian.AstrologianCard.Arrow);
        }
        #endregion
        
        #region Combat Settings
        public bool CombatSettingsInCombat { get; set; }
        public bool CombatSettingsOutOfCombat { get; set; }
        public bool CombatSettingsBoth { get; set; }

        public void ResetCombatSettingsUi()
        {
            if (SelectedCardRules == null)
                return;

            if (SelectedCardRules.Conditions == null)
            {
                SelectedCardRules.Conditions = new Conditions();
            }

            if (CombatSettingsInCombat)
            {
                SelectedCardRules.Conditions.InCombat = true;
            }

            if (CombatSettingsOutOfCombat)
            {
                SelectedCardRules.Conditions.InCombat = false;
            }

            if (CombatSettingsBoth)
            {
                SelectedCardRules.Conditions.InCombat = null;
            }
        }

        private void LoadCombatSettingsUi()
        {
            if (SelectedCardRules?.Conditions == null)
                return;

            CombatSettingsBoth = false;
            CombatSettingsOutOfCombat = false;
            CombatSettingsBoth = false;

            switch (SelectedCardRules.Conditions.InCombat)
            {
                case null:
                    CombatSettingsBoth = true;
                    return;
                case true:
                    CombatSettingsInCombat = true;
                    return;
                case false:
                    CombatSettingsOutOfCombat = true;
                    return;
                default:
                    break;
            }
        }
        #endregion

        #region TargetHasTarget
        public bool TargetHasTargetYes { get; set; }
        public bool TargetHasTargetNo { get; set; }
        public bool TargetHasTargetDoesntMatter { get; set; }

        public void ResetTargetHasTargetSettingsUi()
        {
            if (SelectedCardRules == null)
                return;

            if (SelectedCardRules.TargetConditions == null)
            {
                SelectedCardRules.TargetConditions = new TargetConditions();
            }

            if (TargetHasTargetYes)
            {
                SelectedCardRules.TargetConditions.HasTarget = true;
            }

            if (TargetHasTargetNo)
            {
                SelectedCardRules.TargetConditions.HasTarget = false;
            }

            if (TargetHasTargetDoesntMatter)
            {
                SelectedCardRules.TargetConditions.HasTarget = null;
            }
        }

        private void LoadTargetHasTargetSettingsUi()
        {
            if (SelectedCardRules?.TargetConditions == null)
                return;

            TargetHasTargetYes = false;
            TargetHasTargetNo = false;
            TargetHasTargetDoesntMatter = false;

            switch (SelectedCardRules.TargetConditions.HasTarget)
            {
                case null:
                    TargetHasTargetDoesntMatter = true;
                    return;
                case true:
                    TargetHasTargetYes = true;
                    return;
                case false:
                    TargetHasTargetNo = true;
                    return;
                default:
                    break;
            }
        }
        #endregion  

        public void ReloadUiElements()
        {
            LoadCombatSettingsUi();
            LoadHasHeldCardSettingsUi();
            LoadDoesNotHaveHeldCardSettingsUi();
            LoadIsJobSettingsUi();
            LoadIsRoleSettingsUi();
            LoadTargetHasTargetSettingsUi();
        }

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
