using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Dragoon
{
    [AddINotifyPropertyChangedInterface]
    public class DragoonSettings : PhysicalDpsSettings, IRoutineSettings
    {
        public DragoonSettings() : base(CharacterSettingsDirectory + "/Magitek/Dragoon/DragoonSettings.json") { }

        public static DragoonSettings Instance { get; set; } = new DragoonSettings();

        #region General
        [Setting]
        [DefaultValue(false)]
        public bool HidePositionalMessage { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool EnemyIsOmni { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceLimitBreak { get; set; }
        #endregion

        #region Jumps
        [Setting]
        [DefaultValue(true)]
        public bool UseJumps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SafeJumpLogic { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseHighJump { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseMirageDive { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseSpineshatterDive { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDragonfireDive { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseStardiver { get; set; }

        #endregion

        #region Buff
        [Setting]
        [DefaultValue(true)]
        public bool UseBuffs { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBattleLitany { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseLanceCharge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDragonSight { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceDragonSight { get; set; }

        [Setting]
        [DefaultValue(DragonSightStrategy.ClosestDps)]
        public DragonSightStrategy SelectedStrategy { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool UseSmartDragonSight { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseLifeSurge { get; set; }

        #endregion

        #region Aoe
        [Setting]
        [DefaultValue(true)]
        public bool UseAoe { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int AoeEnemies { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseGeirskogul { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseNastrond { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseWyrmwindThrust { get; set; }

    #endregion

        #region Defensives

        [Setting]
        [DefaultValue(true)]
        public bool Defensives { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }

        #endregion

        #region Eye Weights
        [Setting]
        [DefaultValue(1)]
        public int MnkEyeWeight { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int RprEyeWeight { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int SamEyeWeight { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int DrgEyeWeight { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int NinEyeWeight { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int BrdEyeWeight { get; set; }

        [Setting]
        [DefaultValue(11)]
        public int MchEyeWeight { get; set; }

        [Setting]
        [DefaultValue(12)]
        public int DncEyeWeight { get; set; }

        [Setting]
        [DefaultValue(20)]
        public int SmnEyeWeight { get; set; }

        [Setting]
        [DefaultValue(21)]
        public int RdmEyeWeight { get; set; }

        [Setting]
        [DefaultValue(22)]
        public int BlmEyeWeight { get; set; }

        [Setting]
        [DefaultValue(30)]
        public int GnbEyeWeight { get; set; }

        [Setting]
        [DefaultValue(31)]
        public int DrkEyeWeight { get; set; }

        [Setting]
        [DefaultValue(32)]
        public int WarEyeWeight { get; set; }

        [Setting]
        [DefaultValue(33)]
        public int PldEyeWeight { get; set; }

        [Setting]
        [DefaultValue(40)]
        public int WhmEyeWeight { get; set; }

        [Setting]
        [DefaultValue(41)]
        public int SchEyeWeight { get; set; }

        [Setting]
        [DefaultValue(42)]
        public int SgeEyeWeight { get; set; }

        [Setting]
        [DefaultValue(43)]
        public int AstEyeWeight { get; set; }

        #endregion
    }
}