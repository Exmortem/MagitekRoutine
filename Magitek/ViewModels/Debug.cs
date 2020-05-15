using Clio.Utilities.Collections;
using ff14bot.Objects;
using Magitek.Models;
using Magitek.Models.Debugging;
using Magitek.Models.QueueSpell;
using Magitek.Utilities;
using Magitek.Utilities.Collections;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class Debug
    {
        private static Debug _instance;
        public static Debug Instance => _instance ?? (_instance = new Debug());

        public Models.Account.BaseSettings Settings => Models.Account.BaseSettings.Instance;

        public bool CastingHeal { get; set; }
        public SpellData CastingSpell { get; set; }
        public SpellData LastSpell { get; set; }
        public GameObject LastSpellTarget { get; set; }
        public GameObject SpellTarget { get; set; }
        public bool DoHealthChecks { get; set; }
        public bool NeedAura { get; set; }
        public bool CastingGambit { get; set; }
        public uint Aura { get; set; }
        public bool UseRefreshTime { get; set; }
        public int RefreshTime { get; set; }
        public string CastingTime { get; set; }
        public long InCombatTime { get; set; }
        public int InCombatTimeLeft { get; set; }
        public int OutOfCombatTime { get; set; }
        public int InCombatMovingTime { get; set; }
        public int NotMovingInCombatTime { get; set; }
        public double TargetCombatTimeLeft { get; set; }
        public Duty.States DutyState { get; set; } = Duty.States.NotInDuty;
        public long DutyTime { get; set; }
        public string IsBoss { get; set; }

        public ObservableCollection<EnemyInfo> Enemies { get; set; } = new AsyncObservableCollection<EnemyInfo>();
        public ConcurrentObservableDictionary<uint, EnemySpellCastInfo> EnemySpellCasts { get; set; } = new ConcurrentObservableDictionary<uint, EnemySpellCastInfo>();
        public ConcurrentObservableDictionary<uint, TargetAuraInfo> EnemyAuras { get; set; } = new ConcurrentObservableDictionary<uint, TargetAuraInfo>();
        public ConcurrentObservableDictionary<uint, EnemyTargetInfo> EnemyTargetHistory { get; set; } = new ConcurrentObservableDictionary<uint, EnemyTargetInfo>();
        public ConcurrentObservableDictionary<uint, TargetAuraInfo> PartyMemberAuras { get; set; } = new ConcurrentObservableDictionary<uint, TargetAuraInfo>();
        public ObservableCollection<QueueSpell> Queue { get; set; } = new AsyncObservableCollection<QueueSpell>();
        public AsyncObservableCollection<Enmity> Enmity { get; set; } = new AsyncObservableCollection<Enmity>();
        public ObservableCollection<GameObject> CastableWithin10 { get; set; } = new ObservableCollection<GameObject>();
        public ObservableCollection<GameObject> CastableWithin15 { get; set; } = new ObservableCollection<GameObject>();
        public ObservableCollection<GameObject> CastableWithin30 { get; set; } = new ObservableCollection<GameObject>();
        public System.Collections.Generic.List<SpellCastHistoryItem> SpellCastHistory { get; set; } = new System.Collections.Generic.List<SpellCastHistoryItem>();
    }
}
