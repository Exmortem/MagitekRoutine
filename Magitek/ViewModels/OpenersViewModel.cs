using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Clio.Utilities.Collections;
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
using Magitek.Models.MagitekApi;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Newtonsoft.Json;
using PropertyChanged;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class OpenersViewModel
    {
        private static OpenersViewModel _instance;
        public static OpenersViewModel Instance => _instance ?? (_instance = new OpenersViewModel());

        public OpenersViewModel()
        {
            // Load from file
            LoadOpeners();

            // Get current job for first load
            SelectedJob = RotationManager.CurrentRotation;

            CollectionViewSource = System.Windows.Data.CollectionViewSource.GetDefaultView(OpenerGroups);
            CollectionViewSource.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));

            ResetCollectionViewSource();
        }

        public Models.Account.BaseSettings GeneralSettings => Models.Account.BaseSettings.Instance;
        public ICollectionView CollectionViewSource { get; set; }

        #region Variables
        public ClassJobType SelectedJob { get; set; }
        public bool OnlyCurrentZone { get; set; }
        public string Status { get; set; }
        #endregion

        #region Collections

        public ObservableCollection<OpenerGroup> OpenerGroups { get; set; } = new ObservableCollection<OpenerGroup>();

        #endregion

        #region Filtering

        public void ResetCollectionViewSource()
        {
            CollectionViewSource.Filter = r =>
            {
                var openerGroup = (OpenerGroup)r;

                if (openerGroup == null)
                    return false;

                if (openerGroup.Job != SelectedJob)
                    return false;

                if (OnlyCurrentZone)
                {
                    if (openerGroup.ZoneId != WorldManager.ZoneId)
                        return false;
                }

                return true;
            };
        }

        public ICommand ResetJobOpenerGroupsCommand => new DelegateCommand(ResetCollectionViewSource);

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

        public ICommand AddOpenerGroup => new DelegateCommand(() =>
        {

            OnlyCurrentZone = false;

            OpenerGroups.Add(new OpenerGroup
            {
                Name = $"Opener",
                Id = new Random().Next(int.MaxValue),
                ZoneId = 1,
                ZoneName = "Zone Name",
                Job = SelectedJob,
                Gambits = new ObservableCollection<Gambit>()
            });

            ResetCollectionViewSource();

        });

        public ICommand SetOpenerGroupCurrentZone => new DelegateCommand<OpenerGroup>(group =>
        {

            group.ZoneId = WorldManager.ZoneId;
            group.ZoneName = WorldManager.CurrentLocalizedZoneName;

        });

        public ICommand RemoveOpenerGroup => new DelegateCommand<OpenerGroup>(group =>
        {

            if (group == null)
                return;

            if (!OpenerGroups.Contains(group))
                return;

            OpenerGroups.Remove(group);

            // We also need to remove the file since we load from all files in the gambits folder
            var file = $@"{JsonSettings.CharacterSettingsDirectory}/Magitek/Openers/{group.Id}.json";

            if (!File.Exists(file))
                return;

            File.Delete(file);

        });

        public ICommand AddGambit => new DelegateCommand<OpenerGroup>(group =>
        {

            if (group == null)
                return;

            var openerGroup = OpenerGroups.FirstOrDefault(r => r.Id == group.Id);

            if (openerGroup == null)
                return;

            var newGambit = new Gambit
            {
                Title = "New Action",
                Action = new CastSpellOnCurrentTargetAction(),
                ActionType = GambitActionTypes.CastSpellOnCurrentTarget,
                IsEnabled = true,
                Order = openerGroup.Gambits.Count + 1,
                Job = SelectedJob,
                Id = new Random().Next(int.MaxValue),
                PreventSameActionForTheNextMilliseconds = 2000,
                MaxTimeToWaitForAction = 3000,
                AbandonOpenerIfActionFail = false,
                Conditions = new ObservableCollection<IGambitCondition>()
            };

            openerGroup.Gambits.Add(newGambit);

        });

        public ICommand RemoveGambit => new DelegateCommand<Gambit>(gambit =>
        {

            if (gambit == null)
                return;

            foreach (var group in OpenerGroups)
            {
                if (group.Gambits.Any(currentGambit => currentGambit.Id == gambit.Id))
                {
                    group.Gambits.Remove(gambit);
                }
            }

        });

        public ICommand AddOpenerCondition => new DelegateCommand<Tuple<OpenerGroup, string>>(tuple =>
        {

            ConditionHelpers.AddConditionToOpenerGroup(tuple.Item1, tuple.Item2);

        });

        public ICommand AddGambitCondition => new DelegateCommand<Tuple<Gambit, string>>(tuple =>
        {

            ConditionHelpers.AddConditionToGambit(tuple.Item1, tuple.Item2);

        });

        public ICommand RemoveStartOpenerCondition => new DelegateCommand<Tuple<OpenerGroup, int>>(tuple =>
        {

            var condition = tuple.Item1?.StartOpenerConditions.FirstOrDefault(r => r.Id == tuple.Item2);

            if (condition == null)
                return;

            tuple.Item1.StartOpenerConditions.Remove(condition);

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

        #region Moving and Rearranging

        public ICommand MoveGambitDownCommand => new DelegateCommand<Tuple<OpenerGroup, Gambit>>(tuple =>
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
                return;

            var newOrder = gambitToSwitch.Order;

            // Switch the orders
            gambitToSwitch.Order = tuple.Item2.Order;
            tuple.Item2.Order = newOrder;

            // Refresh the Gambits List
            var orderedEnumerable = tuple.Item1.Gambits.OrderBy(r => r.Order);
            tuple.Item1.Gambits = new ObservableCollection<Gambit>(orderedEnumerable);

        });

        public ICommand MoveGambitUpCommand => new DelegateCommand<Tuple<OpenerGroup, Gambit>>(tuple =>
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

        #endregion

        #region Saving and Loading

        private readonly string _openersFolder = @"Settings/" + Core.Me.Name + "/Magitek/Openers/";

        public ICommand Load => new DelegateCommand(LoadOpeners);
        public ICommand Save => new DelegateCommand(SaveOpeners);

        public void SaveOpeners()
        {

            try
            {
                if (OpenerGroups == null || OpenerGroups.Count == 0)
                    return;

                Parallel.ForEach(OpenerGroups, group =>
                {
                    var data = JsonConvert.SerializeObject(group, Formatting.Indented);
                    var filePath = _openersFolder + $"/{group.Id}.json";
                    var file = new FileInfo(filePath);
                    file.Directory?.Create();
                    File.WriteAllText(filePath, data);
                });

                //foreach (var group in OpenerGroups)
                //{
                //    var data = JsonConvert.SerializeObject(group, Formatting.Indented);
                //    var filePath = _openersFolder + $"/{group.Id}.json";
                //    var file = new FileInfo(filePath);
                //    file.Directory?.Create();
                //    File.WriteAllText(filePath, data);
                //}
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }

        }

        private void LoadOpeners()
        {

            try
            {
                if (!Directory.Exists(_openersFolder))
                    return;

                var directory = new DirectoryInfo(_openersFolder);
                var files = directory.GetFiles("*.json", SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    var openerGroup = JsonConvert.DeserializeObject<OpenerGroup>(File.ReadAllText(file.FullName));

                    if (openerGroup == null)
                        continue;

                    OpenerGroups.Add(openerGroup);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }

        }

        #endregion

        #region Applying Gambits

        public ICommand ApplyOpenersCommand => new DelegateCommand(ApplyOpeners);

        public void ApplyOpeners()
        {

            if (OpenerGroups == null || OpenerGroups.Count == 0)
                return;

            try
            {
                var openers = OpenerGroups.Where(r =>
                    r.Job == Core.Me.CurrentJob && (r.ZoneId == WorldManager.ZoneId || r.ZoneId == 1)).ToList();
                CustomOpenerLogic.OpenerGroups = new List<OpenerGroup>(openers);

                Logger.WriteInfo($"Added {openers.Count} Openers For This Zone");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }

        }

        #endregion  

        private const string ApiAddress = "https://api.magitek.io";

        #region Share Gambit

        public ICommand ShareOpenerGroup => new AwaitableDelegateCommand<OpenerGroup>(async group =>
        {
            if (group == null)
            {
                Status = $"Opener Is Null?";
                return;
            }

            if (group.Gambits.Count == 0)
            {
                Status = $"The Opener You're Trying To Share Has No Actions";
                return;
            }

            #region Check To See If A Description Exists

            if (string.IsNullOrWhiteSpace(group.Description) || string.IsNullOrEmpty(group.Description))
            {
                Status = $"You Must Enter A Description For The Opener";
                return;
            }

            #endregion

            try
            {
                var zone = group.ZoneId == 1 ? "Any" : group.ZoneName;
                var numberOfActions = group.Gambits.Count;

                var openerString = JsonConvert.SerializeObject(group);

                var testGambitGroup = new SharedOpener
                {
                    Id = group.Id,
                    Job = group.Job.ToString(),
                    Name = group.Name,
                    Description = group.Description,
                    File = openerString,
                    Created = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    PosterId = "Anonymous",
                    Zone = zone,
                    NumberOfActions = numberOfActions
                };

                var result = await HttpHelpers.Post($"{ApiAddress}/sharedopeners/add", testGambitGroup);
                var response = JsonConvert.DeserializeObject<MagitekApiResult>(result.Content);

                Status = $"Shared Opener";
                GambitBrowser.Instance.UpdateDisplayedGambits();

            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }

        });

        #endregion

        #region Gambits Browser

        public ICommand ShowOpenersBrowserModal => new DelegateCommand(() =>
        {

            Magitek.Form.ShowModal(new Controls.OpenersBrowser());

        });

        #endregion

        #region Overlay
        public bool OpenersAvailable { get; set; }
        public AsyncObservableCollection<string> AvailableOpeners { get; set; } = new AsyncObservableCollection<string>();
        #endregion
    }
}
