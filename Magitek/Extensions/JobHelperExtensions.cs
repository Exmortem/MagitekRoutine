using Magitek.Models;
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
using Magitek.Models.Reaper;
using Magitek.Models.RedMage;
using Magitek.Models.Samurai;
using Magitek.Models.Scholar;
using Magitek.Models.Summoner;
using Magitek.Models.Warrior;
using Magitek.Models.WhiteMage;

namespace Magitek.Extensions
{
    internal static class JobHelperExtensions
    {
        public static IRoutineSettings GetIRoutineSettingsFromJobString(this string job)
        {
            switch (job)
            {
                case "Scholar":
                    return ScholarSettings.Instance;

                case "WhiteMage":
                    return WhiteMageSettings.Instance;

                case "Astrologian":
                    return AstrologianSettings.Instance;

                case "Paladin":
                    return PaladinSettings.Instance;

                case "Warrior":
                    return WarriorSettings.Instance;

                case "DarkKnight":
                    return DarkKnightSettings.Instance;

                case "Gunbreaker":
                    return GunbreakerSettings.Instance;

                case "Bard":
                    return BardSettings.Instance;

                case "Machinist":
                    return MachinistSettings.Instance;

                case "Dancer":
                    return DancerSettings.Instance;

                case "BlackMage":
                    return BlackMageSettings.Instance;

                case "Summoner":
                    return SummonerSettings.Instance;

                case "RedMage":
                    return RedMageSettings.Instance;

                case "Monk":
                    return MonkSettings.Instance;

                case "Dragoon":
                    return DragoonSettings.Instance;

                case "Ninja":
                    return NinjaSettings.Instance;

                case "Samurai":
                    return SamuraiSettings.Instance;

                case "Reaper":
                    return ReaperSettings.Instance;

                case "BlueMage":
                    return BlueMageSettings.Instance;
            }

            return null;
        }
    }
}
