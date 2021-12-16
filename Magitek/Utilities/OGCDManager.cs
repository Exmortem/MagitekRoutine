using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Utilities
{
    internal static class OGCDManager
    {
        //Melee
        public static List<ClassJobType> NinjaJobTypes = new List<ClassJobType>() { ClassJobType.Rogue, ClassJobType.Ninja };
        public static List<ClassJobType> MonkJobTypes = new List<ClassJobType>() { ClassJobType.Pugilist, ClassJobType.Monk };
        public static List<ClassJobType> DragoonJobTypes = new List<ClassJobType>() { ClassJobType.Lancer, ClassJobType.Dragoon };
        public static List<ClassJobType> SamuraiJobTypes = new List<ClassJobType>() { ClassJobType.Samurai };
        public static List<ClassJobType> ReaperJobTypes = new List<ClassJobType>() { ClassJobType.Reaper };

        //Range Physical
        public static List<ClassJobType> BardJobTypes = new List<ClassJobType>() { ClassJobType.Archer, ClassJobType.Bard };
        public static List<ClassJobType> MachinistJobTypes = new List<ClassJobType>() { ClassJobType.Machinist };
        public static List<ClassJobType> DancerJobTypes = new List<ClassJobType>() { ClassJobType.Dancer };

        //Range Magical
        public static List<ClassJobType> BlackMageJobTypes = new List<ClassJobType>() { ClassJobType.Thaumaturge, ClassJobType.BlackMage };
        public static List<ClassJobType> SummonerJobTypes = new List<ClassJobType>() { ClassJobType.Arcanist, ClassJobType.Summoner };
        public static List<ClassJobType> RedMageJobTypes = new List<ClassJobType>() { ClassJobType.RedMage };
        public static List<ClassJobType> BlueMageJobTypes = new List<ClassJobType>() { ClassJobType.BlueMage };

        //Tank
        public static List<ClassJobType> WarriorJobTypes = new List<ClassJobType>() { ClassJobType.Marauder, ClassJobType.Warrior };
        public static List<ClassJobType> PaladinJobTypes = new List<ClassJobType>() { ClassJobType.Gladiator, ClassJobType.Paladin };
        public static List<ClassJobType> DarkKnightJobTypes = new List<ClassJobType>() { ClassJobType.DarkKnight };
        public static List<ClassJobType> GunbreakerJobTypes = new List<ClassJobType>() { ClassJobType.Gunbreaker };

        //Healer
        public static List<ClassJobType> WhiteMageJobTypes = new List<ClassJobType>() { ClassJobType.Conjurer, ClassJobType.WhiteMage };
        public static List<ClassJobType> ScholarJobTypes = new List<ClassJobType>() { ClassJobType.Arcanist, ClassJobType.Scholar };
        public static List<ClassJobType> AstrologianJobTypes = new List<ClassJobType>() { ClassJobType.Astrologian };
        public static List<ClassJobType> SageJobTypes = new List<ClassJobType>() { ClassJobType.Sage };


        public static List<ClassJobType> PhysicalRangedDpsJobs = new List<ClassJobType>()
        {
            ClassJobType.Archer, ClassJobType.Bard,
            ClassJobType.Machinist,
            ClassJobType.Dancer
        };
        public static List<ClassJobType> MagicalRangedDpsJobs = new List<ClassJobType>()
        {
            ClassJobType.Arcanist, ClassJobType.Summoner,
            ClassJobType.Thaumaturge, ClassJobType.BlackMage,
            ClassJobType.RedMage,
            ClassJobType.BlueMage
        };
        public static List<ClassJobType> MeleeDpsJobs = new List<ClassJobType>()
        {
            ClassJobType.Pugilist, ClassJobType.Monk,
            ClassJobType.Lancer, ClassJobType.Dragoon,
            ClassJobType.Rogue, ClassJobType.Ninja,
            ClassJobType.Samurai,
            ClassJobType.Reaper
        };
        public static List<ClassJobType> TankJobs = new List<ClassJobType>()
        {
            ClassJobType.Gladiator, ClassJobType.Paladin,
            ClassJobType.Marauder, ClassJobType.Warrior,
            ClassJobType.DarkKnight,
            ClassJobType.Gunbreaker
        };
        public static List<ClassJobType> HealerJobs = new List<ClassJobType>()
        {
            ClassJobType.Conjurer, ClassJobType.WhiteMage,
            ClassJobType.Scholar,
            ClassJobType.Astrologian,
            ClassJobType.Sage
        };

        public static List<ClassJobType> CombatJobs = new List<ClassJobType>()
        {
            //Melee
            ClassJobType.Pugilist, ClassJobType.Monk,
            ClassJobType.Lancer, ClassJobType.Dragoon,
            ClassJobType.Rogue, ClassJobType.Ninja,
            ClassJobType.Samurai,
            ClassJobType.Reaper,

            //Physical Range
            ClassJobType.Archer, ClassJobType.Bard,
            ClassJobType.Machinist,
            ClassJobType.Dancer,

            //Magical Range
            ClassJobType.Arcanist, ClassJobType.Summoner,
            ClassJobType.Thaumaturge, ClassJobType.BlackMage,
            ClassJobType.RedMage,
            ClassJobType.BlueMage,

            //Tank
            ClassJobType.Gladiator, ClassJobType.Paladin,
            ClassJobType.Marauder, ClassJobType.Warrior,
            ClassJobType.DarkKnight,
            ClassJobType.Gunbreaker,

            //Healer
            ClassJobType.Conjurer, ClassJobType.WhiteMage,
            ClassJobType.Scholar,
            ClassJobType.Astrologian,
            ClassJobType.Sage
        };


        static OGCDManager()
        {
            DataManager.SpellCache.Values.Where(x => x.IsPlayerAction
                                                     && (x.SpellType == SpellType.System ||
                                                         x.SpellType == SpellType.Ability ||
                                                         x.SpellType == SpellType.Spell)
                                                     && CombatJobs.Contains(x.Job)).ToList();

            foreach (var y in DataManager.SpellCache.Values.Where(x => (x.IsPlayerAction 
                                                                                    && (x.SpellType == SpellType.Ability /*|| x.SpellType == SpellType.Spell*/) 
                                                                                    && x.JobTypes.Any(CombatJobs.Contains))
                                                                                    || (x.SpellType == SpellType.System && x.Job == ClassJobType.Adventurer)).ToList()) 
            {
            }



            foreach (KeyValuePair<uint, SpellData> action in DataManager.SpellCache)
            {
                if (action.Value.Job == ClassJobType.Adventurer && action.Value.SpellType == SpellType.System)
                {
                    //SystemAbilitys.Add(action.Value);
                    continue;
                }

                if (!action.Value.IsPlayerAction ||
                    !(action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)) continue;

            }
        }
    }
}
