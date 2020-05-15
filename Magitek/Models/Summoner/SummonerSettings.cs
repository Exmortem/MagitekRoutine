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

        [Setting]
        [DefaultValue(SummonerPets.Garuda)]
        public SummonerPets SelectedPet { get; set; }

        [Setting]
        [DefaultValue(5)]
        public int DotRefreshSeconds { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool LucidDreaming { get; set; }

        [Setting]
        [DefaultValue(60.0f)]
        public float LucidDreamingManaPercent { get; set; }


        [Setting]
        [DefaultValue(7)]
        public int BaneSecondsOnDots { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Ruin { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bio { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Physick { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Miasma { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EgiAssault1 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Resurrection { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool ResuSwift { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceResuSwift { get; set; }

        [Setting]
        [DefaultValue(false)]
        public bool ForceResu { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Fester { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EnergyDrain { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Bane { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EnergySiphon { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Outburst { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EgiAssault2 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Enkindle { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Painflare { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool TriDisaster { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool DreadwyrmTrance { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Deathflare { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Ruin4 { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool Aetherpact { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool SummonBahamut { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EnkindleBahamut { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool AkhMorn { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FirebirdTrance { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool FountainOfFire { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool BrandOfPurgatory { get; set; }

        [Setting]
        [DefaultValue(true)]
        public bool EnkindlePhoenix { get; set; }
    }
}