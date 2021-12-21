using Clio.Utilities.Collections;
using Magitek.Commands;
using Magitek.Enumerations;
using Magitek.Models.Account;
using Magitek.Models.Astrologian;
using Magitek.Models.Bard;
using Magitek.Models.BlackMage;
using Magitek.Models.BlueMage;
using Magitek.Models.Dancer;
using Magitek.Models.DarkKnight;
using Magitek.Models.Dragoon;
using Magitek.Models.Gunbreaker;
using Magitek.Models.Machinist;
using Magitek.Models.Monk;
using Magitek.Models.Ninja;
using Magitek.Models.Paladin;
using Magitek.Models.RedMage;
using Magitek.Models.Samurai;
using Magitek.Models.Reaper;
using Magitek.Models.Sage;
using Magitek.Models.Scholar;
using Magitek.Models.Summoner;
using Magitek.Models.Warrior;
using Magitek.Models.WhiteMage;
using Magitek.Utilities.Overlays;
using Magitek.Views;
using PropertyChanged;
using System.Windows.Input;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class BaseSettings
    {
        private static BaseSettings _instance;
        public static BaseSettings Instance => _instance ?? (_instance = new BaseSettings());

        public ICommand ShowSettingsModal => new DelegateCommand(() =>
        {
            Magitek.Form.ShowModal(new SettingsModal());
        });

        public ICommand ResetOverlayPositions => new DelegateCommand(() =>
        {
            Models.Account.BaseSettings.ResetOverlayPositions();

            OverlayManager.RestartMainOverlay();
            OverlayManager.RestartCombatMessageOverlay();
        });

        public ICommand RestartOverlay => new DelegateCommand(() =>
        {
            OverlayManager.RestartMainOverlay();
        });

        public ICommand RestartCombatMessageOverlay => new DelegateCommand(() =>
        {
           OverlayManager.RestartCombatMessageOverlay();
        });

        public AsyncObservableCollection<double> FontSizes { get; set; } = new AsyncObservableCollection<double>() { 9, 10, 11, 12 };

        public Models.Account.BaseSettings GeneralSettings => Models.Account.BaseSettings.Instance;
        public AuthenticationSettings AuthenticationSettings => AuthenticationSettings.Instance;

        public ScholarSettings ScholarSettings { get; set; } = ScholarSettings.Instance;
        public WhiteMageSettings WhiteMageSettings { get; set; } = WhiteMageSettings.Instance;
        public AstrologianSettings AstrologianSettings { get; set; } = AstrologianSettings.Instance;
        public PaladinSettings PaladinSettings { get; set; } = PaladinSettings.Instance;
        public DarkKnightSettings DarkKnightSettings { get; set; } = DarkKnightSettings.Instance;
        public WarriorSettings WarriorSettings { get; set; } = WarriorSettings.Instance;
        public BardSettings BardSettings { get; set; } = BardSettings.Instance;
        public DancerSettings DancerSettings { get; set; } = DancerSettings.Instance;
        public MachinistSettings MachinistSettings { get; set; } = MachinistSettings.Instance;
        public DragoonSettings DragoonSettings { get; set; } = DragoonSettings.Instance;
        public MonkSettings MonkSettings { get; set; } = MonkSettings.Instance;
        public NinjaSettings NinjaSettings { get; set; } = NinjaSettings.Instance;
        public SamuraiSettings SamuraiSettings { get; set; } = SamuraiSettings.Instance;
        public BlueMageSettings BlueMageSettings { get; set; } = BlueMageSettings.Instance;
        public BlackMageSettings BlackMageSettings { get; set; } = BlackMageSettings.Instance;
        public RedMageSettings RedMageSettings { get; set; } = RedMageSettings.Instance;
        public SummonerSettings SummonerSettings { get; set; } = SummonerSettings.Instance;
        public GunbreakerSettings GunbreakerSettings { get; set; } = GunbreakerSettings.Instance;
        public ReaperSettings ReaperSettings { get; set; } = ReaperSettings.Instance;
        public SageSettings SageSettings { get; set; } = SageSettings.Instance;
        public string CurrentRoutine { get; set; }
        public string RoutineSelectedInUi { get; set; }
        public bool InPvp { get; set; }

        public string PositionalText { get; set; } = "Positional!";
        public string PositionalStatus { get; set; } = "Neutral";

        public PositionalState CurrentPosition { get; set; } = PositionalState.None;
        public PositionalState ExpectedPosition { get; set; } = PositionalState.None;
        public PositionalState UpcomingPosition { get; set; } = PositionalState.None;
    }
}
