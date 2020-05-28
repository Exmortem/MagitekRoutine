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

        [Setting]
        [DefaultValue(true)]
        public bool UseJumps { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool MirageDive { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseBuffs { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BloodOfTheDragon { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BattleLitany { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Geirskogul { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LifeSurge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseDragonSight { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Defensives { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Jump { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SafeJumpLogic { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SpineshatterDive { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool UseLifeSurge { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool DragonfireDive { get; set; }

        [Setting]
        [DefaultValue(70.0f)]
        public float RestHealthPercent { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Aoe { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int AoeEnemies { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int DisembowelRefreshSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LanceCharge { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Stardiver { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TrueNorth { get; set; }

        [Setting]
        [DefaultValue(DragonSightStrategy.ClosestDps)]
        public DragonSightStrategy SelectedStrategy { get; set; }

        #region Eye Weights
        [Setting]
        [DefaultValue(1)]
        public int MnkEyeWeight { get; set; }

        [Setting]
        [DefaultValue(10)]
        public int BlmEyeWeight { get; set; }

        [Setting]
        [DefaultValue(3)]
        public int DrgEyeWeight { get; set; }

        [Setting]
        [DefaultValue(2)]
        public int SamEyeWeight { get; set; }

        [Setting]
        [DefaultValue(6)]
        public int MchEyeWeight { get; set; }

        [Setting]
        [DefaultValue(8)]
        public int SmnEyeWeight { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int BrdEyeWeight { get; set; }

        [Setting]
        [DefaultValue(4)]
        public int NinEyeWeight { get; set; }

        [Setting]
        [DefaultValue(9)]
        public int RdmEyeWeight { get; set; }

        [Setting]
        [DefaultValue(7)]
        public int DncEyeWeight { get; set; }

        [Setting]
        [DefaultValue(14)]
        public int PldEyeWeight { get; set; }

        [Setting]
        [DefaultValue(13)]
        public int WarEyeWeight { get; set; }

        [Setting]
        [DefaultValue(12)]
        public int DrkEyeWeight { get; set; }

        [Setting]
        [DefaultValue(11)]
        public int GnbEyeWeight { get; set; }

        [Setting]
        [DefaultValue(15)]
        public int WhmEyeWeight { get; set; }

        [Setting]
        [DefaultValue(16)]
        public int SchEyeWeight { get; set; }

        [Setting]
        [DefaultValue(17)]
        public int AstEyeWeight { get; set; }

        #endregion
    }
}