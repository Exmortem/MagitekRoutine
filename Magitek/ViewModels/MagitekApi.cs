using Clio.Utilities.Collections;
using Magitek.Commands;
using Magitek.Models.Astrologian;
using Magitek.Models.Bard;
using Magitek.Models.BlackMage;
using Magitek.Models.BlueMage;
using Magitek.Models.DarkKnight;
using Magitek.Models.Dragoon;
using Magitek.Models.Gunbreaker;
using Magitek.Models.Machinist;
using Magitek.Models.MagitekApi;
using Magitek.Models.Monk;
using Magitek.Models.Ninja;
using Magitek.Models.Paladin;
using Magitek.Models.RedMage;
using Magitek.Models.Samurai;
using Magitek.Models.Scholar;
using Magitek.Models.Summoner;
using Magitek.Models.Warrior;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using System.Windows.Input;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MagitekApi
    {
        private static MagitekApi _instance;
        public static MagitekApi Instance => _instance ?? (_instance = new MagitekApi());

        public MagitekApi()
        {
            try
            {
                UpdateNews();
            }
            catch (Exception)
            { }
        }

        private readonly HttpClient _webClient = new HttpClient();
        private const string ApiAddress = "https://api.magitek.io";

        public string Status { get; set; }
        public string SettingsName { get; set; }
        public string SettingsDescription { get; set; }
        public bool SpinnerVisible { get; set; } = false;

        public AsyncObservableCollection<MagitekNews> NewsList { get; set; }

        public ICommand RefreshNewsList => new DelegateCommand(UpdateNews);

        private async void UpdateNews()
        {
            SpinnerVisible = true;

            
            var result = await _webClient.GetAsync($@"{ApiAddress}/news/");

            if (!result.IsSuccessStatusCode)
            {
                SpinnerVisible = false;
                return;
            }

            //var responseContent = await result.Content.ReadAsStringAsync();
            //NewsList = new AsyncObservableCollection<MagitekNews>(JsonConvert.DeserializeObject<List<MagitekNews>>(responseContent).OrderByDescending(r => r.Id));
            

            MagitekNews CurrentVersion = new MagitekNews
            {
                Created = "01/01/9999"
            };
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, $@"Routines\Magitek\Version.txt")))
            {
                try
                {
                    var version = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $@"Routines\Magitek\Version.txt"));
                    CurrentVersion.Title = "Current Version";
                    CurrentVersion.Message = version;
                }
                catch 
                {
                    CurrentVersion.Title = "Current Version";
                    CurrentVersion.Message = "UNKNOWN";
                }
            }

            NewsList = new AsyncObservableCollection<MagitekNews>() { CurrentVersion };
            
            SpinnerVisible = false;
        }

        public AsyncObservableCollection<MagitekSettings> AstrologianSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> WhiteMageSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> ScholarSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> PaladinSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> WarriorSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> DarkKnightSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> BardSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> MachinistSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> BlackMageSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> RedMageSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> SummonerSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> DragoonSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> MonkSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> NinjaSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> SamuraiSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> BlueMageSettingsList { get; set; }
        public AsyncObservableCollection<MagitekSettings> GunbreakerSettingsList { get; set; }

        public ICommand RefreshSettingsList => new AwaitableDelegateCommand<string>(async job =>
        {
            SpinnerVisible = true;

            var result = await _webClient.GetAsync($@"{ApiAddress}/magiteksettings/job/{job}");

            if (!result.IsSuccessStatusCode)
            {
                Logger.Error($"Could Not Retrieve {job} Settings");
                return;
            }

            var responseContent = await result.Content.ReadAsStringAsync();
            var settingsList = new AsyncObservableCollection<MagitekSettings>(JsonConvert.DeserializeObject<List<MagitekSettings>>(responseContent));

            switch (job)
            {
                case "Astrologian":
                    AstrologianSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "WhiteMage":
                    WhiteMageSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Scholar":
                    ScholarSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Paladin":
                    PaladinSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Warrior":
                    WarriorSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "DarkKnight":
                    DarkKnightSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Bard":
                    BardSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Machinist":
                    MachinistSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "BlackMage":
                    BlackMageSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "RedMage":
                    RedMageSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Summoner":
                    SummonerSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Dragoon":
                    DragoonSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Monk":
                    MonkSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Ninja":
                    NinjaSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Samurai":
                    SamuraiSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "BlueMage":
                    BlueMageSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                case "Gunbreaker":
                    GunbreakerSettingsList =
                        new AsyncObservableCollection<MagitekSettings>(settingsList.OrderByDescending(r => r.Rating));
                    break;

                default:
                    SpinnerVisible = false;
                    return;
            }

            SpinnerVisible = false;
        });

        public ICommand PostNewSettings => new AwaitableDelegateCommand<string>(async job =>
        {
            if (string.IsNullOrEmpty(job) || string.IsNullOrWhiteSpace(job))
            {
                Status = "Job Is Not Set Correctly (Please Report)";
                return;
            }

            if (string.IsNullOrEmpty(SettingsName) || string.IsNullOrWhiteSpace(SettingsName))
            {
                Status = "Please Give Your Settings A Name";
                return;
            }

            if (string.IsNullOrEmpty(SettingsDescription) || string.IsNullOrWhiteSpace(SettingsDescription))
            {
                Status = "Please Give Your Settings A Description";
                return;
            }

            if (string.IsNullOrEmpty(Models.Account.BaseSettings.Instance.ContributorKey) ||
                string.IsNullOrWhiteSpace(Models.Account.BaseSettings.Instance.ContributorKey))
            {
                Status = "You Must Set Have A Contributor Key (In General Settings) To Submit New Settings";
                return;
            }

            SpinnerVisible = true;

            var contributorKeyResult = await _webClient.GetAsync($@"{ApiAddress}/contributors/verify/{Models.Account.BaseSettings.Instance.ContributorKey}");
            var contributorResponseContent = await contributorKeyResult.Content.ReadAsStringAsync();

            if (!contributorKeyResult.IsSuccessStatusCode)
            {
                try
                {
                    Status = JsonConvert.DeserializeObject<MagitekApiResult>(contributorResponseContent).Description;
                }
                catch (Exception)
                {
                    Logger.Error("Couldn't Deserialize Magitek API Result");
                    return;
                }

                SpinnerVisible = false;
                return;
            }

            MagitekContributor contributor;

            try
            {
                contributor = JsonConvert.DeserializeObject<MagitekContributor>(contributorResponseContent);
            }
            catch (Exception)
            {
                Logger.Error("Couldn't Deserialize Magitek Contributor Object");
                return;
            }

            Status = "Uploading New Settings ...";

            string settingString;

            switch (job)
            {
                case "Astrologian":
                    settingString = JsonConvert.SerializeObject(AstrologianSettings.Instance, Formatting.None);
                    break;

                case "WhiteMage":
                    settingString = JsonConvert.SerializeObject(WhiteMageSettings.Instance, Formatting.None);
                    break;

                case "Scholar":
                    settingString = JsonConvert.SerializeObject(ScholarSettings.Instance, Formatting.None);
                    break;

                case "Paladin":
                    settingString = JsonConvert.SerializeObject(PaladinSettings.Instance, Formatting.None);
                    break;

                case "Warrior":
                    settingString = JsonConvert.SerializeObject(WarriorSettings.Instance, Formatting.None);
                    break;

                case "DarkKnight":
                    settingString = JsonConvert.SerializeObject(DarkKnightSettings.Instance, Formatting.None);
                    break;

                case "Bard":
                    settingString = JsonConvert.SerializeObject(BardSettings.Instance, Formatting.None);
                    break;

                case "Machinist":
                    settingString = JsonConvert.SerializeObject(MachinistSettings.Instance, Formatting.None);
                    break;

                case "BlackMage":
                    settingString = JsonConvert.SerializeObject(BlackMageSettings.Instance, Formatting.None);
                    break;

                case "RedMage":
                    settingString = JsonConvert.SerializeObject(RedMageSettings.Instance, Formatting.None);
                    break;

                case "Summoner":
                    settingString = JsonConvert.SerializeObject(SummonerSettings.Instance, Formatting.None);
                    break;

                case "Dragoon":
                    settingString = JsonConvert.SerializeObject(DragoonSettings.Instance, Formatting.None);
                    break;

                case "Monk":
                    settingString = JsonConvert.SerializeObject(MonkSettings.Instance, Formatting.None);
                    break;

                case "Ninja":
                    settingString = JsonConvert.SerializeObject(NinjaSettings.Instance, Formatting.None);
                    break;

                case "Samurai":
                    settingString = JsonConvert.SerializeObject(SamuraiSettings.Instance, Formatting.None);
                    break;

                case "BlueMage":
                    settingString = JsonConvert.SerializeObject(BlueMageSettings.Instance, Formatting.None);
                    break;

                case "Gunbreaker":
                    settingString = JsonConvert.SerializeObject(GunbreakerSettings.Instance, Formatting.None);
                    break;
                default:
                    SpinnerVisible = false;
                    return;
            }

            var newMagitekSettings = new MagitekSettings
            {
                Author = contributor.Name,
                Job = job,
                Name = SettingsName,
                Rating = 0,
                Description = SettingsDescription,
                File = settingString
            };

            var result = await HttpHelpers.Post($"{ApiAddress}/magiteksettings/add", newMagitekSettings);
            var response = JsonConvert.DeserializeObject<MagitekApiResult>(result.Content);
            Status = response.Description;

            SpinnerVisible = false;
        });

        public ICommand LoadSettings => new DelegateCommand<MagitekSettings>(settings =>
        {
            if (settings == null)
            {
                Logger.Error("Failed To Load Settings From The Magitek API");
                return;
            }

            Logger.WriteInfo($"Attempting To Load {settings.Name} By {settings.Author} For {settings.Job}");

            switch (settings.Job)
            {
                case "Paladin":
                    var paladinSettings = JsonConvert.DeserializeObject<PaladinSettings>(settings.File);
                    PaladinSettings.Instance = paladinSettings;
                    BaseSettings.Instance.PaladinSettings = PaladinSettings.Instance;
                    break;

                case "Scholar":
                    var scholarSettings = JsonConvert.DeserializeObject<ScholarSettings>(settings.File);
                    ScholarSettings.Instance = scholarSettings;
                    BaseSettings.Instance.ScholarSettings = ScholarSettings.Instance;
                    break;

                case "Astrologian":
                    var astrologianSettings = JsonConvert.DeserializeObject<AstrologianSettings>(settings.File);
                    AstrologianSettings.Instance = astrologianSettings;
                    BaseSettings.Instance.AstrologianSettings = AstrologianSettings.Instance;
                    break;

                case "WhiteMage":
                    var whiteMageSettings = JsonConvert.DeserializeObject<WhiteMageSettings>(settings.File);
                    WhiteMageSettings.Instance = whiteMageSettings;
                    BaseSettings.Instance.WhiteMageSettings = WhiteMageSettings.Instance;
                    break;

                case "Bard":
                    var bardSettings = JsonConvert.DeserializeObject<BardSettings>(settings.File);
                    BardSettings.Instance = bardSettings;
                    BaseSettings.Instance.BardSettings = BardSettings.Instance;
                    break;

                case "RedMage":
                    var redMageSettings = JsonConvert.DeserializeObject<RedMageSettings>(settings.File);
                    RedMageSettings.Instance = redMageSettings;
                    BaseSettings.Instance.RedMageSettings = RedMageSettings.Instance;
                    break;

                case "Dragoon":
                    var dragoonSettings = JsonConvert.DeserializeObject<DragoonSettings>(settings.File);
                    DragoonSettings.Instance = dragoonSettings;
                    BaseSettings.Instance.DragoonSettings = DragoonSettings.Instance;
                    break;

                case "Samurai":
                    var samuraiSettings = JsonConvert.DeserializeObject<SamuraiSettings>(settings.File);
                    SamuraiSettings.Instance = samuraiSettings;
                    BaseSettings.Instance.SamuraiSettings = SamuraiSettings.Instance;
                    break;

                case "BlueMage":
                    var blueMageSettings = JsonConvert.DeserializeObject<BlueMageSettings>(settings.File);
                    BlueMageSettings.Instance = blueMageSettings;
                    BaseSettings.Instance.BlueMageSettings = BlueMageSettings.Instance;
                    break;

                case "DarkKnight":
                    var darkKnightSettings = JsonConvert.DeserializeObject<DarkKnightSettings>(settings.File);
                    DarkKnightSettings.Instance = darkKnightSettings;
                    BaseSettings.Instance.DarkKnightSettings = DarkKnightSettings.Instance;
                    break;

                case "Machinist":
                    var machinistSettings = JsonConvert.DeserializeObject<MachinistSettings>(settings.File);
                    MachinistSettings.Instance = machinistSettings;
                    BaseSettings.Instance.MachinistSettings = MachinistSettings.Instance;
                    break;

                case "Warrior":
                    var warriorSettings = JsonConvert.DeserializeObject<WarriorSettings>(settings.File);
                    WarriorSettings.Instance = warriorSettings;
                    BaseSettings.Instance.WarriorSettings = WarriorSettings.Instance;
                    break;

                case "Summoner":
                    var summonerSettings = JsonConvert.DeserializeObject<SummonerSettings>(settings.File);
                    SummonerSettings.Instance = summonerSettings;
                    BaseSettings.Instance.SummonerSettings = SummonerSettings.Instance;
                    break;

                case "BlackMage":
                    var blackMageSettings = JsonConvert.DeserializeObject<BlackMageSettings>(settings.File);
                    BlackMageSettings.Instance = blackMageSettings;
                    BaseSettings.Instance.BlackMageSettings = BlackMageSettings.Instance;
                    break;

                case "Monk":
                    var monkSettings = JsonConvert.DeserializeObject<MonkSettings>(settings.File);
                    MonkSettings.Instance = monkSettings;
                    BaseSettings.Instance.MonkSettings = MonkSettings.Instance;
                    break;

                case "Ninja":
                    var ninjaSettings = JsonConvert.DeserializeObject<NinjaSettings>(settings.File);
                    NinjaSettings.Instance = ninjaSettings;
                    BaseSettings.Instance.NinjaSettings = NinjaSettings.Instance;
                    break;

                case "Gunbreaker":
                    var gunbreakerSettings = JsonConvert.DeserializeObject<GunbreakerSettings>(settings.File);
                    GunbreakerSettings.Instance = gunbreakerSettings;
                    BaseSettings.Instance.GunbreakerSettings = GunbreakerSettings.Instance;
                    break;

                default:
                    Logger.Error($"Could Not Load Settings");
                    break;
            }

            Logger.WriteInfo($"Loaded {settings.Name} By {settings.Author} For {settings.Job}");
        });

        public ICommand SaveSettings => new DelegateCommand<MagitekSettings>(settings =>
        {
            if (settings == null)
            {
                Logger.Error("Failed To Save Settings From The Magitek API");
                return;
            }

            var saveFile = new SaveFileDialog()
            {
                Filter = @"json files (*.json|*.json",
                FileName = settings.Name,
                InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location?.ToString())
            };

            if (saveFile.ShowDialog() != DialogResult.OK)
                return;

            var jsonText = JsonConvert.SerializeObject(settings.File, Formatting.Indented);
            File.WriteAllText(saveFile.FileName, jsonText);
        });
    }
}
