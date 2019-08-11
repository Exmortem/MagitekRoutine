using System.ComponentModel;
using ff14bot.Enums;

namespace Magitek.Enumerations
{
    public enum MagitekClassJobType : uint
    {
        [Description("SCH")]
        Scholar = ClassJobType.Scholar,

        [Description("WHM")]
        WhiteMage = ClassJobType.WhiteMage,

        [Description("AST")]
        Astrologian = ClassJobType.Astrologian,

        [Description("WAR")]
        Warrior = ClassJobType.Warrior,

        [Description("PLD")]
        Paladin = ClassJobType.Paladin,

        [Description("DRK")]
        DarkKnight = ClassJobType.DarkKnight,

        [Description("GNB")]
        Gunbreaker = ClassJobType.Gunbreaker,

        [Description("BRD")]
        Bard = ClassJobType.Bard,

        [Description("MCH")]
        Machinist = ClassJobType.Machinist,

        [Description("DNC")]
        Dancer = ClassJobType.Dancer,

        [Description("BLM")]
        BlackMage = ClassJobType.BlackMage,

        [Description("RDM")]
        RedMage = ClassJobType.RedMage,

        [Description("SMN")]
        Summoner = ClassJobType.Summoner,

        [Description("DRG")]
        Dragoon = ClassJobType.Dragoon,

        [Description("MNK")]
        Monk = ClassJobType.Monk,

        [Description("NIN")]
        Ninja = ClassJobType.Ninja,

        [Description("SAM")]
        Samurai = ClassJobType.Samurai
    }   
}
