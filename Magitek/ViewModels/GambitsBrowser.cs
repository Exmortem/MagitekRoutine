using Clio.Utilities.Collections;
using Magitek.Commands;
using Magitek.Gambits;
using Magitek.Models.Account;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class GambitBrowser
    {
        private static GambitBrowser _instance;
        public static GambitBrowser Instance => _instance ?? (_instance = new GambitBrowser());

        public GambitBrowser()
        {
            SelectedJob = RotationManager.CurrentRotation.ToString();
            UpdateDisplayedGambits();
        }

        #region Variables
        public string SelectedJob { get; set; }
        public bool ShowAll { get; set; }
        public bool SpinnerVisible { get; set; }
        private readonly HttpClient _webClient = new HttpClient();
        private const string ApiAddress = "https://api.magitek.io";
        #endregion

        public AsyncObservableCollection<SharedGambit> GambitGroups { get; set; } = new AsyncObservableCollection<SharedGambit>();

        #region Update Gambits List

        public ICommand UpdateDisplayedGambitsCommand => new AwaitableDelegateCommand(UpdateDisplayedGambitsTask);

        public async void UpdateDisplayedGambits()
        {
            SpinnerVisible = true;
            await UpdateDisplayedGambitsTask();
            SpinnerVisible = false;
        }

        private async Task UpdateDisplayedGambitsTask()
        {
            try
            {
                var result = await _webClient.GetAsync($@"{ApiAddress}/sharedgambits/job/{SelectedJob}");

                if (!result.IsSuccessStatusCode)
                {
                    return;
                }

                var responseContent = await result.Content.ReadAsStringAsync();
                GambitGroups = new AsyncObservableCollection<SharedGambit>(JsonConvert.DeserializeObject<List<SharedGambit>>(responseContent));
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        #endregion

        #region Remove Gambit Group

        public ICommand RemoveGambitGroup => new AwaitableDelegateCommand<SharedGambit>(async gambitGroup =>
        {
            try
            {
                var posterId = "";

                if (!string.IsNullOrWhiteSpace(AuthenticationSettings.Instance.MagitekKey) && !string.IsNullOrEmpty(AuthenticationSettings.Instance.MagitekKey))
                {
                    posterId = AuthenticationSettings.Instance.MagitekKey.Substring(AuthenticationSettings.Instance.MagitekKey.Length - 5);
                }
                else if (!string.IsNullOrWhiteSpace(AuthenticationSettings.Instance.MagitekLegacyKey) && !string.IsNullOrEmpty(AuthenticationSettings.Instance.MagitekLegacyKey))
                {
                    posterId = AuthenticationSettings.Instance.MagitekLegacyKey.Substring(AuthenticationSettings.Instance.MagitekLegacyKey.Length - 5);
                }

                if (posterId == "")
                {
                    return;
                }

                var result = await _webClient.GetAsync($@"{ApiAddress}/sharedgambits/remove/{gambitGroup.Id}/{posterId}");

                if (!result.IsSuccessStatusCode)
                {
                    return;
                }

                UpdateDisplayedGambits();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        });

        #endregion

        #region Download

        public ICommand DownloadGambitGroupCommand => new DelegateCommand<string>(gambitGroupString =>
        {
            if (string.IsNullOrWhiteSpace(gambitGroupString) || string.IsNullOrEmpty(gambitGroupString))
                return;

            var gambitGroup = JsonConvert.DeserializeObject<GambitGroup>(gambitGroupString);

            if (gambitGroup?.Gambits == null)
                return;

            if (GambitsViewModel.Instance.GambitGroups.Any(r => r.Id == gambitGroup.Id))
            {
                var oldGambitGroup = GambitsViewModel.Instance.GambitGroups.FirstOrDefault(r => r.Id == gambitGroup.Id);
                GambitsViewModel.Instance.GambitGroups.Remove(oldGambitGroup);
            }

            GambitsViewModel.Instance.GambitGroups.Add(gambitGroup);
            GambitsViewModel.Instance.ResetCollectionViewSource();
        });

        #endregion

        #region Job Selection

        public ICommand JobSelectionChanged => new DelegateCommand<string>(job =>
        {
            if (job == null)
                return;

            SelectedJob = job;
            UpdateDisplayedGambits();
        });

        #endregion
    }
}
