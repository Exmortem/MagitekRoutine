using Magitek.Enumerations;
using PropertyChanged;
using System;
using System.Globalization;

namespace Magitek.Models.WebResources
{
    [AddINotifyPropertyChangedInterface]
    public class XivDbItem
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string NameJa { get; set; }
        public string NameFr { get; set; }
        public string NameDe { get; set; }
        public string NameCns { get; set; }
        public double Icon { get; set; }

        public bool HighPriority { get; set; }
        public bool Stun { get; set; }
        public bool Interrupt { get; set; }
        public bool Scholar { get; set; }
        public bool WhiteMage { get; set; }
        public bool Astrologian { get; set; }
        public bool Bard { get; set; }
        public bool Paladin { get; set; }
        public bool Warrior { get; set; }
        public bool DarkKnight { get; set; }
        public bool Machinist { get; set; }
        public bool Dragoon { get; set; }
        public bool Monk { get; set; }
        public bool Ninja { get; set; }
        public bool BlueMage { get; set; }

        public ScholarTbStrategies ScholarTbStrategy { get; set; } = ScholarTbStrategies.None;
        public WhiteMageTbStrategies WhiteMageTbStrategy { get; set; } = WhiteMageTbStrategies.None;
        public AstrologianTbStrategies AstrologianTbStrategy { get; set; } = AstrologianTbStrategies.None;

        //public uint TankBusterTimeIntoCast { get; set; }

        public string IconUrl
        {
            get
            {
                var folder = (Math.Floor(Icon / 1000) * 1000).ToString(CultureInfo.InvariantCulture).Trim().PadLeft(6, '0');
                var image = Icon.ToString(CultureInfo.InvariantCulture).Trim().PadLeft(6, '0');
                return $@"https://secure.xivdb.com/img/game/{folder}/{image}.png";
            }
        }
        public bool DivineVeil { get; set; }
        public bool Sheltron { get; set; }
        public bool HallowedGround { get; set; }
        public bool Sentinel { get; set; }
        public bool Rampart { get; set; }
        public bool Reprisal { get; set; }

        //public bool PaladinTankBuster => DivineVeil || Sheltron || HallowedGround || Sentinel  || Rampart;

        public bool RampartDk { get; set; }
        public bool LivingDead { get; set; }
        public bool ShadowWall { get; set; }
        public bool DarkMind { get; set; }
        public bool DarkMissionary { get; set; }
        public bool TheBlackestNight { get; set; }
        public bool ReprisalDk { get; set; }

        //public bool DarkKnightTankBuster => LivingDead || DarkMissionary || TheBlackestNight || RampartDk || ShadowWall || DarkMind;

        public bool RampartWar { get; set; }
        public bool ReprisalWar { get; set; }
        public bool InnerBeast { get; set; }
        public bool Holmgang { get; set; }
        public bool Vengeance { get; set; }
        public bool RawIntuition { get; set; }

        //public bool WarriorTankBuster => RampartWar || ReprisalWar || InnerBeast || Holmgang || Vengeance || RawIntuition;

        public bool RampartGnb { get; set; }
        public bool ReprisalGnb { get; set; }
        public bool Camouflage { get; set; }
        public bool Nebula { get; set; }
        public bool Aurora { get; set; }
        public bool HeartofLight { get; set; }
        public bool HeartofStone { get; set; }
        public bool Superbolide { get; set; }

        //public bool GunbreakerTankBuster => RampartGnb || ReprisalGnb || HeartofLight || HeartofStone || Camouflage || Nebula || Aurora || Superbolide;
    }
}
