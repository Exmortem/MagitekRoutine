using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Paladin
{
    [AddINotifyPropertyChangedInterface]
    public class PaladinSettings : TankSettings, IRoutineSettings
    {
        public PaladinSettings() : base(CharacterSettingsDirectory + "/Magitek/Paladin/PaladinSettings.json") { }
        public static PaladinSettings Instance { get; set; } = new PaladinSettings();

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }

        [Setting]
        [DefaultValue(4000)]
        public int HealthSetting { get; set; }

        [Setting]
        [DefaultValue(20.0f)]
        public float HealthSettingPercent { get; set; }

        #region DamageBuff
        [Setting]
        [DefaultValue(true)]
        public bool UseFightOrFlight { get; set; }
        #endregion

        #region Utility
        [Setting]
        [DefaultValue(true)]
        public bool UseIronWill { get; set; }
        #endregion

        #region AOE
        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool UseEclipseCombo { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int TotalEclipseEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHolyCircle { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int HolyCircleEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseCircleOfScorn { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SaveCircleOfScorn { get; set; }

        [Setting]
        [DefaultValue(6)]
        public float SaveCircleOfScornMseconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseConfiteorCombo { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseExpiacion { get; set; }
        #endregion

        #region Aggro
        [Setting]
        [DefaultValue(false)]
        public bool UseShieldLob { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShieldLobToPull { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseShieldLobOnLostAggro { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseShieldLobToPullExtraEnemies { get; set; }
        #endregion

        #region Cover
        [Setting]
        [DefaultValue(true)]
        public bool UseCover { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseCoverHealer { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float UseCoverHealerHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseCoverDps { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float UseCoverDpsHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterventionOnNearbyPartyMember { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float InterventionOnNearbyPartyMemberHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterventionPartyAlwaysWOCD { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterventionPartyAlwaysWithCD { get; set; }
        #endregion

        #region Defensive
        [Setting]
        [DefaultValue(true)]
        public bool UseHallowedGround { get; set; }

        [Setting]
        [DefaultValue(20.0f)]
        public float HallowedGroundHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSentinel { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float SentinelHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSheltron { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float SheltronHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBulwark { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float BulwarkHp { get; set; }
        #endregion

        #region Dash
        [Setting]
        [DefaultValue(true)]
        public bool UseIntervene { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool InterveneOnlyInMelee { get; set; }

        /*[Setting]
        [DefaultValue(0)]
        public int SaveInterveneCharges { get; set; }*/
        #endregion

        #region DefensiveGroup
        [Setting]
        [DefaultValue(true)]
        public bool UseDivineVeil { get; set; }

        [Setting]
        [DefaultValue(80.0f)]
        public float DivineVeilHp { get; set; }
        #endregion

        #region Damage
        [Setting]
        [DefaultValue(true)]
        public bool UseGoringBlade { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHolySpirit { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHolySpiritWhenOutOfMeleeRange { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float HolySpiritWhenOutOfMeleeRangeMinMpPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHolySpiritWhenOutOfMeleeRangeWithDivineMightOnly { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHolySpiritToPull { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseRequiescat { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseAtonement { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool KeepHolySpiritAtonementinFoF { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ShieldBash { get; set; }
        #endregion

        #region Heal
        [Setting]
        [DefaultValue(true)]
        public bool UseClemency { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseClemencyHealer { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float UseClemencyHealerHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseClemencyDps { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float UseClemencyDpsHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseClemencySelf { get; set; }

        [Setting]
        [DefaultValue(30.0f)]
        public float UseClemencySelfHp { get; set; }

        [Setting]
        [DefaultValue(50.0f)]
        public float MinMpClemency { get; set; }
        #endregion

        #region PVP
        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Atonement { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_ShieldBash { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Intervene { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_SafeIntervene { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Guardian { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_HolySheltron { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Phalanx { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Pvp_Confiteor { get; set; }
        #endregion
    }
}
