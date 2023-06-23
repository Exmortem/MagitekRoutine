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
using System.Text.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows.Forms;
using System.Windows.Input;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MagitekApi
    {
        private static MagitekApi _instance;
        private readonly HttpClient _webClient = new HttpClient();
        private const string ApiAddress = "https://api.magitek.io";
        private const string GithubAddress = "https://api.github.com";
        private const string VersionUrl = "https://ddjx48xxp2d6i.cloudfront.net/Version.txt";

        public static MagitekApi Instance => _instance ?? (_instance = new MagitekApi());
        public string Status { get; set; }
        public string SettingsName { get; set; }
        public string SettingsDescription { get; set; }
        public bool SpinnerVisible { get; set; } = false;
        public AsyncObservableCollection<MagitekNews> NewsList { get; set; }
        public MagitekVersion MagitekVersion { get; set; }
        public ICommand RefreshNewsList => new DelegateCommand(UpdateNews);
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

        public class Base
        {
            public string label { get; set; }
            public string @ref { get; set; }
            public string sha { get; set; }
            public User user { get; set; }
            public Repo repo { get; set; }
        }

        public class Comments
        {
            public string href { get; set; }
        }

        public class Commits
        {
            public string href { get; set; }
        }

        public class Head
        {
            public string label { get; set; }
            public string @ref { get; set; }
            public string sha { get; set; }
            public User user { get; set; }
            public Repo repo { get; set; }
        }

        public class Html
        {
            public string href { get; set; }
        }

        public class Issue
        {
            public string href { get; set; }
        }

        public class License
        {
            public string key { get; set; }
            public string name { get; set; }
            public string spdx_id { get; set; }
            public string url { get; set; }
            public string node_id { get; set; }
        }

        public class Links
        {
            public Self self { get; set; }
            public Html html { get; set; }
            public Issue issue { get; set; }
            public Comments comments { get; set; }
            public ReviewComments review_comments { get; set; }
            public ReviewComment review_comment { get; set; }
            public Commits commits { get; set; }
            public Statuses statuses { get; set; }
        }

        public class Owner
        {
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class Repo
        {
            public int id { get; set; }
            public string node_id { get; set; }
            public string name { get; set; }
            public string full_name { get; set; }
            public bool @private { get; set; }
            public Owner owner { get; set; }
            public string html_url { get; set; }
            public string description { get; set; }
            public bool fork { get; set; }
            public string url { get; set; }
            public string forks_url { get; set; }
            public string keys_url { get; set; }
            public string collaborators_url { get; set; }
            public string teams_url { get; set; }
            public string hooks_url { get; set; }
            public string issue_events_url { get; set; }
            public string events_url { get; set; }
            public string assignees_url { get; set; }
            public string branches_url { get; set; }
            public string tags_url { get; set; }
            public string blobs_url { get; set; }
            public string git_tags_url { get; set; }
            public string git_refs_url { get; set; }
            public string trees_url { get; set; }
            public string statuses_url { get; set; }
            public string languages_url { get; set; }
            public string stargazers_url { get; set; }
            public string contributors_url { get; set; }
            public string subscribers_url { get; set; }
            public string subscription_url { get; set; }
            public string commits_url { get; set; }
            public string git_commits_url { get; set; }
            public string comments_url { get; set; }
            public string issue_comment_url { get; set; }
            public string contents_url { get; set; }
            public string compare_url { get; set; }
            public string merges_url { get; set; }
            public string archive_url { get; set; }
            public string downloads_url { get; set; }
            public string issues_url { get; set; }
            public string pulls_url { get; set; }
            public string milestones_url { get; set; }
            public string notifications_url { get; set; }
            public string labels_url { get; set; }
            public string releases_url { get; set; }
            public string deployments_url { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public DateTime pushed_at { get; set; }
            public string git_url { get; set; }
            public string ssh_url { get; set; }
            public string clone_url { get; set; }
            public string svn_url { get; set; }
            public object homepage { get; set; }
            public int size { get; set; }
            public int stargazers_count { get; set; }
            public int watchers_count { get; set; }
            public string language { get; set; }
            public bool has_issues { get; set; }
            public bool has_projects { get; set; }
            public bool has_downloads { get; set; }
            public bool has_wiki { get; set; }
            public bool has_pages { get; set; }
            public bool has_discussions { get; set; }
            public int forks_count { get; set; }
            public object mirror_url { get; set; }
            public bool archived { get; set; }
            public bool disabled { get; set; }
            public int open_issues_count { get; set; }
            public License license { get; set; }
            public bool allow_forking { get; set; }
            public bool is_template { get; set; }
            public bool web_commit_signoff_required { get; set; }
            public List<object> topics { get; set; }
            public string visibility { get; set; }
            public int forks { get; set; }
            public int open_issues { get; set; }
            public int watchers { get; set; }
            public string default_branch { get; set; }
        }

        public class ReviewComment
        {
            public string href { get; set; }
        }

        public class ReviewComments
        {
            public string href { get; set; }
        }

        public class Root
        {
            public string url { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string html_url { get; set; }
            public string diff_url { get; set; }
            public string patch_url { get; set; }
            public string issue_url { get; set; }
            public int number { get; set; }
            public string state { get; set; }
            public bool locked { get; set; }
            public string title { get; set; }
            public User user { get; set; }
            public string body { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public DateTime closed_at { get; set; }
            public DateTime merged_at { get; set; }
            public string merge_commit_sha { get; set; }
            public object assignee { get; set; }
            public List<object> assignees { get; set; }
            public List<object> requested_reviewers { get; set; }
            public List<object> requested_teams { get; set; }
            public List<object> labels { get; set; }
            public object milestone { get; set; }
            public bool draft { get; set; }
            public string commits_url { get; set; }
            public string review_comments_url { get; set; }
            public string review_comment_url { get; set; }
            public string comments_url { get; set; }
            public string statuses_url { get; set; }
            public Head head { get; set; }
            public Base @base { get; set; }
            public Links _links { get; set; }
            public string author_association { get; set; }
            public object auto_merge { get; set; }
            public object active_lock_reason { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Statuses
        {
            public string href { get; set; }
        }

        public class User
        {
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }



        public MagitekApi()
        {
            SpinnerVisible = true;
            try
            {
                NewsList = new AsyncObservableCollection<MagitekNews>();
                UpdateVersion();
                UpdateNews();
            }
            catch (Exception)
            { }
            SpinnerVisible = false;
        }

        private async void UpdateVersion()
        {
            var local = "UNKNOWN";
            var distant = "UNKNOWN";
            
            try
            {
                local = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, $@"Routines\Magitek\Version.txt"));
            }
            catch
            {
                Logger.Error("Can't read local Magitek version. Please reinstall it");
            }

            try
            {
                distant = await _webClient.GetStringAsync(VersionUrl);
            }
            catch
            {
                Logger.Error("Can't read distant Magitek version. Please reinstall it");
            }
            MagitekVersion = new MagitekVersion()
            {
                LocalVersion = local,
                DistantVersion = distant
            };
        }

        private async void UpdateNews()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(GithubAddress);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Anything");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.GetFromJsonAsync<List<Root>>("repos/Exmortem/Magitekroutine/pulls?state=closed&page=1&per_page=8");

                    if (response == null)
                        return;

                    //var result = httpResponse.Content.ReadFromJsonAsync<Root>();
                    response.ForEach(x =>
                    {
                        NewsList.Add(new MagitekNews
                                            {
                                                Created = x?.merged_at.ToString("d"),
                                                Title = "Changelog",
                                                Message = "" + x?.body
                                            });
                    });
                    
                            
                    
                };
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
            }




            SpinnerVisible = false;
        }


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
            var settingsList = new AsyncObservableCollection<MagitekSettings>(JsonSerializer.Deserialize<List<MagitekSettings>>(responseContent));

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
                    Status = JsonSerializer.Deserialize<MagitekApiResult>(contributorResponseContent).Description;
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
                contributor = JsonSerializer.Deserialize<MagitekContributor>(contributorResponseContent);
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
                    settingString = JsonSerializer.Serialize(AstrologianSettings.Instance);
                    break;

                case "WhiteMage":
                    settingString = JsonSerializer.Serialize(WhiteMageSettings.Instance);
                    break;

                case "Scholar":
                    settingString = JsonSerializer.Serialize(ScholarSettings.Instance);
                    break;

                case "Paladin":
                    settingString = JsonSerializer.Serialize(PaladinSettings.Instance);
                    break;

                case "Warrior":
                    settingString = JsonSerializer.Serialize(WarriorSettings.Instance);
                    break;

                case "DarkKnight":
                    settingString = JsonSerializer.Serialize(DarkKnightSettings.Instance);
                    break;

                case "Bard":
                    settingString = JsonSerializer.Serialize(BardSettings.Instance);
                    break;

                case "Machinist":
                    settingString = JsonSerializer.Serialize(MachinistSettings.Instance);
                    break;

                case "BlackMage":
                    settingString = JsonSerializer.Serialize(BlackMageSettings.Instance);
                    break;

                case "RedMage":
                    settingString = JsonSerializer.Serialize(RedMageSettings.Instance);
                    break;

                case "Summoner":
                    settingString = JsonSerializer.Serialize(SummonerSettings.Instance);
                    break;

                case "Dragoon":
                    settingString = JsonSerializer.Serialize(DragoonSettings.Instance);
                    break;

                case "Monk":
                    settingString = JsonSerializer.Serialize(MonkSettings.Instance);
                    break;

                case "Ninja":
                    settingString = JsonSerializer.Serialize(NinjaSettings.Instance);
                    break;

                case "Samurai":
                    settingString = JsonSerializer.Serialize(SamuraiSettings.Instance);
                    break;

                case "BlueMage":
                    settingString = JsonSerializer.Serialize(BlueMageSettings.Instance);
                    break;

                case "Gunbreaker":
                    settingString = JsonSerializer.Serialize(GunbreakerSettings.Instance);
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
            var response = JsonSerializer.Deserialize<MagitekApiResult>(result.Content);
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
                    var paladinSettings = JsonSerializer.Deserialize<PaladinSettings>(settings.File);
                    PaladinSettings.Instance = paladinSettings;
                    BaseSettings.Instance.PaladinSettings = PaladinSettings.Instance;
                    break;

                case "Scholar":
                    var scholarSettings = JsonSerializer.Deserialize<ScholarSettings>(settings.File);
                    ScholarSettings.Instance = scholarSettings;
                    BaseSettings.Instance.ScholarSettings = ScholarSettings.Instance;
                    break;

                case "Astrologian":
                    var astrologianSettings = JsonSerializer.Deserialize<AstrologianSettings>(settings.File);
                    AstrologianSettings.Instance = astrologianSettings;
                    BaseSettings.Instance.AstrologianSettings = AstrologianSettings.Instance;
                    break;

                case "WhiteMage":
                    var whiteMageSettings = JsonSerializer.Deserialize<WhiteMageSettings>(settings.File);
                    WhiteMageSettings.Instance = whiteMageSettings;
                    BaseSettings.Instance.WhiteMageSettings = WhiteMageSettings.Instance;
                    break;

                case "Bard":
                    var bardSettings = JsonSerializer.Deserialize<BardSettings>(settings.File);
                    BardSettings.Instance = bardSettings;
                    BaseSettings.Instance.BardSettings = BardSettings.Instance;
                    break;

                case "RedMage":
                    var redMageSettings = JsonSerializer.Deserialize<RedMageSettings>(settings.File);
                    RedMageSettings.Instance = redMageSettings;
                    BaseSettings.Instance.RedMageSettings = RedMageSettings.Instance;
                    break;

                case "Dragoon":
                    var dragoonSettings = JsonSerializer.Deserialize<DragoonSettings>(settings.File);
                    DragoonSettings.Instance = dragoonSettings;
                    BaseSettings.Instance.DragoonSettings = DragoonSettings.Instance;
                    break;

                case "Samurai":
                    var samuraiSettings = JsonSerializer.Deserialize<SamuraiSettings>(settings.File);
                    SamuraiSettings.Instance = samuraiSettings;
                    BaseSettings.Instance.SamuraiSettings = SamuraiSettings.Instance;
                    break;

                case "BlueMage":
                    var blueMageSettings = JsonSerializer.Deserialize<BlueMageSettings>(settings.File);
                    BlueMageSettings.Instance = blueMageSettings;
                    BaseSettings.Instance.BlueMageSettings = BlueMageSettings.Instance;
                    break;

                case "DarkKnight":
                    var darkKnightSettings = JsonSerializer.Deserialize<DarkKnightSettings>(settings.File);
                    DarkKnightSettings.Instance = darkKnightSettings;
                    BaseSettings.Instance.DarkKnightSettings = DarkKnightSettings.Instance;
                    break;

                case "Machinist":
                    var machinistSettings = JsonSerializer.Deserialize<MachinistSettings>(settings.File);
                    MachinistSettings.Instance = machinistSettings;
                    BaseSettings.Instance.MachinistSettings = MachinistSettings.Instance;
                    break;

                case "Warrior":
                    var warriorSettings = JsonSerializer.Deserialize<WarriorSettings>(settings.File);
                    WarriorSettings.Instance = warriorSettings;
                    BaseSettings.Instance.WarriorSettings = WarriorSettings.Instance;
                    break;

                case "Summoner":
                    var summonerSettings = JsonSerializer.Deserialize<SummonerSettings>(settings.File);
                    SummonerSettings.Instance = summonerSettings;
                    BaseSettings.Instance.SummonerSettings = SummonerSettings.Instance;
                    break;

                case "BlackMage":
                    var blackMageSettings = JsonSerializer.Deserialize<BlackMageSettings>(settings.File);
                    BlackMageSettings.Instance = blackMageSettings;
                    BaseSettings.Instance.BlackMageSettings = BlackMageSettings.Instance;
                    break;

                case "Monk":
                    var monkSettings = JsonSerializer.Deserialize<MonkSettings>(settings.File);
                    MonkSettings.Instance = monkSettings;
                    BaseSettings.Instance.MonkSettings = MonkSettings.Instance;
                    break;

                case "Ninja":
                    var ninjaSettings = JsonSerializer.Deserialize<NinjaSettings>(settings.File);
                    NinjaSettings.Instance = ninjaSettings;
                    BaseSettings.Instance.NinjaSettings = NinjaSettings.Instance;
                    break;

                case "Gunbreaker":
                    var gunbreakerSettings = JsonSerializer.Deserialize<GunbreakerSettings>(settings.File);
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

            var jsonText = JsonSerializer.Serialize(settings.File);
            File.WriteAllText(saveFile.FileName, jsonText);
        });
    }
}
