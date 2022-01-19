using ff14bot.Helpers;
using Magitek.Enumerations;
using PropertyChanged;
using System.ComponentModel;
using System.Configuration;

namespace Magitek.Models.Summoner
{
    [AddINotifyPropertyChangedInterface]
    public class SummonerSettings : JsonSettings, IRoutineSettings
    {
        public SummonerSettings() : base(CharacterSettingsDirectory + "/Magitek/Summoner/SummonerSettings.json") { }

        public static SummonerSettings Instance { get; set; } = new SummonerSettings();

        #region Pet Summons

        [Setting]
        [DefaultValue(true)]
        public bool SummonCarbuncle { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool SummonRubyIfrit { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool SummonTopazTitan { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool SummonEmeraldGaruda { get; set; }

        #endregion

        #region Trances

        [Setting]
        [DefaultValue(true)]
        public bool Aethercharge { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool DreadwyrmTrance { get; set; }


        [Setting]
        [DefaultValue(true)]
        public bool SummonBahamut { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SummonPhoenix { get; set; }

        #endregion

        #region Single Target

        [Setting]
        [DefaultValue(true)]
        public bool Ruin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Fester { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EnergyDrain { get; set; }     
        
        [Setting]
        [DefaultValue(true)]
        public bool SwiftRubyRite { get; set; }

        #endregion

        #region Aoes

        [Setting]
        [DefaultValue(true)]
        public bool Outburst { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool Ruin4 { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool EnergySiphon { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool Painflare { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool EnkindleBahamut { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool EnkindlePhoenix { get; set; }
        
        [Setting]
        [DefaultValue(false)]
        public bool SmartAoe { get; set; }

        #endregion

        #region Astral Flow

        [Setting]
        [DefaultValue(true)]
        public bool Deathflare { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool Rekindle { get; set; }
        
        [Setting]
        [DefaultValue(80f)]
        public float RekindleHPThreshold { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CrimsonCyclone { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool CrimsonStrike { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool MountainBuster { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool Slipstream { get; set; }
        
        #endregion

        #region Buffs

        [Setting]
        [DefaultValue(true)]
        public bool SearingLight { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool RadiantAegis { get; set; }
        
        [Setting]
        [DefaultValue(80f)]
        public float RadiantAegisHPThreshold { get; set; }

        #endregion

        #region Heals

        
        [Setting]
        [DefaultValue(true)]
        public bool Physick { get; set; }
        
        [Setting]
        [DefaultValue(60f)]
        public float PhysickHPThreshold { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SwiftcastRes { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool SlowcastRes { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        public bool ResOutOfCombat { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ResuSwift { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceResuSwift { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceResu { get; set; }

        #endregion

        #region Others

        [Setting]
        [DefaultValue(true)]
        public bool LucidDreaming { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float LucidDreamingManaPercent { get; set; }
        
        [Setting]
        [DefaultValue(false)]
        public bool ThrottleEgiSummonsWithTTL { get; set; }
        
        [Setting]
        [DefaultValue(false)]
        public bool ThrottleTranceSummonsWithTTL { get; set; }
        
        #endregion
    }
}