using System.ComponentModel;
using System.Configuration;
using Magitek.Enumerations;
using Magitek.Models.Roles;
using PropertyChanged;

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
        public bool BuffsUse { get; set; }

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
        public bool DragonSight { get; set; }

        [Setting]
        [DefaultValue(DragonSightStrategy.Self)]
        public DragonSightStrategy SelectedLeftEye { get; set; }

        [Setting]
        [DefaultValue(DragonSightStrategy.ClosestDps)]
        public DragonSightStrategy SelectedStrategy { get; set; }

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
        public bool DragonfireDive { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Stun { get; set; }

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
    }
}