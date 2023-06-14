using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using Magitek.Commands;
using Magitek.Gambits;
using Magitek.Gambits.Actions;
using Magitek.Gambits.Conditions;
using Magitek.Gambits.Helpers;
using Magitek.Logic;
using Magitek.Models.Account;
using Magitek.Models.MagitekApi;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class GambitsViewModel
    {
        private static GambitsViewModel _instance;
        public static GambitsViewModel Instance => _instance ?? (_instance = new GambitsViewModel());

        public GambitsViewModel()
        {
            // Load from file
            LoadGambits();

            // Get current job for first load
            SelectedJob = RotationManager.CurrentRotation;

            // Can we recover old Gambits?
            if (File.Exists(_oldGambitsFile))
            {
                CanRecoverOldGambits = true;
            }

            CollectionViewSource = System.Windows.Data.CollectionViewSource.GetDefaultView(GambitGroups);
            CollectionViewSource.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));

            ResetCollectionViewSource();
        }

        public ICollectionView CollectionViewSource { get; set; }

        #region Variables
        public ClassJobType SelectedJob { get; set; }
        public bool OnlyCurrentZone { get; set; }
        public string Status { get; set; }
        #endregion

        #region Collections

        public ObservableCollection<GambitGroup> GambitGroups { get; set; } = new ObservableCollection<GambitGroup>();

        #endregion

        #region Filtering

        public void ResetCollectionViewSource()
        {
            CollectionViewSource.Filter = r =>
            {
                var gambitGroup = (GambitGroup)r;

                if (gambitGroup == null)
                    return false;

                if (gambitGroup.Job != SelectedJob)
                    return false;

                if (OnlyCurrentZone)
                {
                    if (gambitGroup.ZoneId != WorldManager.ZoneId)
                        return false;
                }

                return true;
            };
        }

        public ICommand ResetJobGambitGroupsCommand => new DelegateCommand(ResetCollectionViewSource);

        #endregion

        #region Job Selection

        public ICommand JobSelectionChanged => new DelegateCommand<string>(job =>
        {
            if (job == null)
                return;

            var jobType = (ClassJobType)Enum.Parse(typeof(ClassJobType), job);
            SelectedJob = jobType;
            ResetCollectionViewSource();
        });

        #endregion

        #region Main Commands - Adding and Removing

        public ICommand AddGambitGroup => new DelegateCommand(() =>
        {
            OnlyCurrentZone = false;

            GambitGroups.Add(new GambitGroup
            {
                Name = $"Gambit Group",
                Id = new Random().Next(int.MaxValue),
                ZoneId = 1,
                ZoneName = "Zone Name",
                Job = SelectedJob,
                Gambits = new ObservableCollection<Gambit>()
            });

            ResetCollectionViewSource();
        });

        public ICommand SetGambitGroupCurrentZone => new DelegateCommand<GambitGroup>(group =>
        {
            group.ZoneId = WorldManager.ZoneId;
            group.ZoneName = WorldManager.CurrentLocalizedZoneName;
        });

        public ICommand RemoveGambitGroup => new DelegateCommand<GambitGroup>(group =>
        {
            if (group == null)
                return;

            if (!GambitGroups.Contains(group))
                return;

            GambitGroups.Remove(group);

            // We also need to remove the file since we load from all files in the gambits folder
            var file = $@"{JsonSettings.CharacterSettingsDirectory}/Magitek/Gambits/{group.Id}.json";

            if (!File.Exists(file))
                return;

            File.Delete(file);
        });

        public ICommand AddGambit => new DelegateCommand<GambitGroup>(group =>
        {
            if (group == null)
                return;

            var gambitGroup = GambitGroups.FirstOrDefault(r => r.Id == group.Id);

            if (gambitGroup == null)
                return;

            var newGambit = new Gambit
            {
                Title = "New Gambit",
                Action = new CastSpellOnEnemyAction(),
                ActionType = GambitActionTypes.CastSpellOnEnemy,
                IsEnabled = true,
                Order = group.Gambits.Count + 1,
                Job = SelectedJob,
                Id = new Random().Next(int.MaxValue),
                PreventSameActionForTheNextMilliseconds = 2000,
                Conditions = new ObservableCollection<IGambitCondition>()
            };

            gambitGroup.Gambits.Add(newGambit);
        });

        public ICommand RemoveGambit => new DelegateCommand<Gambit>(gambit =>
        {
            if (gambit == null)
                return;

            foreach (var group in GambitGroups)
            {
                if (group.Gambits.Any(currentGambit => currentGambit.Id == gambit.Id))
                {
                    group.Gambits.Remove(gambit);
                }
            }
        });

        public ICommand MoveGambitDownCommand => new DelegateCommand<Tuple<GambitGroup, Gambit>>(tuple =>
        {
            if (tuple.Item1 == null || tuple.Item2 == null)
                return;

            // If order of our current gambit is equal to the count, then it must mean that it is the last gambit in the list
            // and that it cannot be moved down
            if (tuple.Item2.Order == tuple.Item1.Gambits.Count)
                return;

            // Get the gambit that is below the one we wanna switch
            var gambitToSwitch = tuple.Item1.Gambits.FirstOrDefault(r => r.Order == tuple.Item2.Order + 1);

            if (gambitToSwitch == null)
            {
                tuple.Item2.Order++;
                return;
            }

            var newOrder = gambitToSwitch.Order;

            // Switch the orders
            gambitToSwitch.Order = tuple.Item2.Order;
            tuple.Item2.Order = newOrder;

            // Refresh the Gambits List
            var orderedEnumerable = tuple.Item1.Gambits.OrderBy(r => r.Order);
            tuple.Item1.Gambits = new ObservableCollection<Gambit>(orderedEnumerable);

        });

        public ICommand MoveGambitUpCommand => new DelegateCommand<Tuple<GambitGroup, Gambit>>(tuple =>
        {

            if (tuple.Item1 == null || tuple.Item2 == null)
                return;

            // If the gambit has an order of 1 then it means that it is at the top and cannot be moved higher
            if (tuple.Item2.Order == 1)
                return;

            // Get the gambit that is below the one we wanna switch
            var gambitToSwitch = tuple.Item1.Gambits.FirstOrDefault(r => r.Order == tuple.Item2.Order - 1);

            if (gambitToSwitch == null)
                return;

            var newOrder = gambitToSwitch.Order;

            // Switch the orders
            gambitToSwitch.Order = tuple.Item2.Order;
            tuple.Item2.Order = newOrder;

            // Refresh the Gambits List
            var orderedEnumerable = tuple.Item1.Gambits.OrderBy(r => r.Order);
            tuple.Item1.Gambits = new ObservableCollection<Gambit>(orderedEnumerable);

        });

        public ICommand AddGambitCondition => new DelegateCommand<Tuple<Gambit, string>>(tuple =>
        {
            ConditionHelpers.AddConditionToGambit(tuple.Item1, tuple.Item2);
        });

        public ICommand RemoveGambitCondition => new DelegateCommand<Tuple<Gambit, int>>(tuple =>
        {
            var condition = tuple.Item1?.Conditions.FirstOrDefault(r => r.Id == tuple.Item2);

            if (condition == null)
                return;

            tuple.Item1.Conditions.Remove(condition);
        });

        public void ActionSelectionChange(Gambit gambit, GambitActionTypes selectedValue)
        {
            ActionHelpers.AddActionToGambit(gambit, selectedValue);
        }

        #endregion

        #region Copying and Pasting

        public string CopiedGambit { get; set; }
        public string CopiedGambitGroup { get; set; }

        public ICommand CopyGambit => new DelegateCommand<Gambit>(gambit =>
        {
            if (gambit == null)
                return;

            CopiedGambit = JsonConvert.SerializeObject(gambit);
        });

        public ICommand PasteCopiedGambit => new DelegateCommand<GambitGroup>(group =>
        {
            if (group == null)
                return;

            if (CopiedGambit == null)
                return;

            var newGambit = JsonConvert.DeserializeObject<Gambit>(CopiedGambit);
            newGambit.Job = group.Job;
            newGambit.Id = new Random().Next(int.MaxValue);
            newGambit.Order = group.Gambits.Count + 1;
            group.Gambits.Add(newGambit);
        });

        public ICommand CopyGambitGroup => new DelegateCommand<GambitGroup>(group =>
        {
            if (group == null)
                return;

            CopiedGambitGroup = JsonConvert.SerializeObject(group);
        });

        public ICommand PasteCopiedGambitGroup => new DelegateCommand<GambitGroup>(group =>
        {
            if (group == null)
                return;

            if (CopiedGambitGroup == null)
                return;

            var newGambitGroup = JsonConvert.DeserializeObject<GambitGroup>(CopiedGambitGroup);

            newGambitGroup.Id = new Random().Next(int.MaxValue);
            newGambitGroup.Job = group.Job;

            foreach (var gambit in newGambitGroup.Gambits)
            {
                gambit.Job = group.Job;
                gambit.Id = new Random().Next(int.MaxValue);
            }

            GambitGroups.Add(newGambitGroup);

            ResetCollectionViewSource();
        });

        public ICommand CopyGambitToClipboard => new DelegateCommand<Gambit>(gambit =>
        {
            if (gambit == null)
                return;

            var serializedGambit = JsonConvert.SerializeObject(gambit, Formatting.Indented);
            Clipboard.SetDataObject(serializedGambit);
        });

        #endregion

        #region Saving and Loading

        private readonly string _gambitsFolder = @"Settings/" + Core.Me.Name + "/Magitek/Gambits/";

        public ICommand Load => new DelegateCommand(LoadGambits);
        public ICommand Save => new DelegateCommand(SaveGambits);

        public void SaveGambits()
        {
            try
            {
                if (GambitGroups == null || GambitGroups.Count == 0)
                    return;

                foreach (var group in GambitGroups)
                {
                    var data = JsonConvert.SerializeObject(group, Formatting.Indented);
                    var filePath = _gambitsFolder + $"/{group.Id}.json";
                    var file = new FileInfo(filePath);
                    file.Directory?.Create();
                    File.WriteAllText(filePath, data);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        private void LoadGambits()
        {
            try
            {
                if (!Directory.Exists(_gambitsFolder))
                    return;

                var directory = new DirectoryInfo(_gambitsFolder);
                var files = directory.GetFiles("*.json", SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    var gambitGroup = JsonConvert.DeserializeObject<GambitGroup>(File.ReadAllText(file.FullName));

                    if (gambitGroup == null)
                        continue;

                    GambitGroups.Add(gambitGroup);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        #endregion

        #region Applying Gambits

        public ICommand ApplyGambitsCommand => new DelegateCommand(ApplyGambits);

        public void ApplyGambits()
        {
            if (GambitGroups == null || GambitGroups.Count == 0)
                return;

            try
            {
                var gambits = GambitGroups.Where(r => r.Job == Core.Me.CurrentJob && (r.ZoneId == WorldManager.ZoneId || r.ZoneId == 1)).SelectMany(x => x.Gambits).Where(x => x.IsEnabled).ToList();

                var interruptGambits = gambits.Where(x => x.InterruptCast &&
                                                         !x.OnlyUseInChain &&
                                                         (x.ActionType == GambitActionTypes.CastSpellOnAlly || x.ActionType == GambitActionTypes.CastSpellOnEnemy || x.ActionType == GambitActionTypes.CastSpellOnFriendlyNpc || x.ActionType == GambitActionTypes.CastSpellOnSelf));

                var toastGambits = gambits.Where(x => x.ActionType == GambitActionTypes.ToastMessage && !x.OnlyUseInChain);

                GambitLogic.StaticGambitQueue = new Queue<Gambit>(gambits);
                GambitLogic.StaticInterruptGambitQueue = new Queue<Gambit>(interruptGambits);
                GambitLogic.StaticToastGambitQueue = new Queue<Gambit>(toastGambits);

                Logger.WriteInfo($"Added {gambits.Count} Gambits To The Gambit Queue");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        #endregion

        #region Recover old gambits

        private readonly string _oldGambitsFile = @"Settings/" + Core.Me.Name + "/Magitek/Gambits.json";

        public bool CanRecoverOldGambits { get; set; }

        public ICommand RecoverOldGambitsCommand => new DelegateCommand(RecoverOldGambits);

        public void RecoverOldGambits()
        {
            try
            {
                if (!File.Exists(_oldGambitsFile))
                    return;

                var oldGambits = JsonConvert.DeserializeObject<List<Gambit>>(File.ReadAllText(_oldGambitsFile));

                Logger.WriteInfo($"Trying To Recover {oldGambits.Count} Gambits");

                foreach (var gambit in oldGambits)
                {
                    // If a recovery group doesn't exist, create one
                    if (!GambitGroups.Any(r => r.Job == gambit.Job && r.Name == "Recovered Gambits"))
                    {
                        var newGambitGroup = new GambitGroup
                        {
                            Name = "Recovered Gambits",
                            Id = new Random().Next(int.MaxValue),
                            ZoneId = 1,
                            ZoneName = "Zone Name",
                            Job = gambit.Job,
                            Gambits = new ObservableCollection<Gambit>()
                        };

                        GambitGroups.Add(newGambitGroup);
                    }

                    // find the recovery group
                    var gambitGroup = GambitGroups.FirstOrDefault(r => r.Job == gambit.Job && r.Name == "Recovered Gambits");

                    // add the gambit to the group
                    gambitGroup?.Gambits.Add(gambit);
                }

            }
            catch (Exception)
            {
                //  
            }

            CanRecoverOldGambits = false;

            // Rename the file
            File.Move($@"{JsonSettings.CharacterSettingsDirectory}/Magitek/Gambits.json", $@"{JsonSettings.CharacterSettingsDirectory}/Magitek/OldGambitsBackup.json");
        }

        #endregion

        private const string ApiAddress = "https://api.magitek.io";

        #region Share Gambit

        public ICommand ShareGambitGroup => new AwaitableDelegateCommand<GambitGroup>(async group =>
        {
            if (group == null)
            {
                Status = $"Gambit Is Null?";
                return;
            }

            if (group.Gambits.Count == 0)
            {
                Status = $"The Group You're Trying To Share Has No Gambits";
                return;
            }

            #region Check To See If A Description Exists

            if (string.IsNullOrWhiteSpace(group.Description) || string.IsNullOrEmpty(group.Description))
            {
                Status = $"You Must Enter A Description For the Gambit Group";
                return;
            }

            #endregion

            try
            {
                var zone = group.ZoneId == 1 ? "Any" : group.ZoneName;
                var numberOfGambits = group.Gambits.Count;

                var gambitString = JsonConvert.SerializeObject(group);

                var testGambitGroup = new SharedGambit
                {
                    Id = new Random().Next(int.MaxValue),
                    Job = group.Job.ToString(),
                    Name = group.Name,
                    Description = group.Description,
                    File = gambitString,
                    Created = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    IsGambitGroup = true,
                    PosterId = "Anonymous",
                    Zone = zone,
                    NumberOfGambits = numberOfGambits
                };

                var result = await HttpHelpers.Post($"{ApiAddress}/sharedgambits/add", testGambitGroup);
                var response = JsonConvert.DeserializeObject<MagitekApiResult>(result.Content);

                Status = $"Shared Gambit Group";
                GambitBrowser.Instance.UpdateDisplayedGambits();

            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        });

        #endregion

        #region Gambits Browser

        public ICommand ShowGambitsBrowserModal => new DelegateCommand(() =>
        {
            Magitek.Form.ShowModal(new Controls.GambitBrowser());
        });

        #endregion
    }
}
