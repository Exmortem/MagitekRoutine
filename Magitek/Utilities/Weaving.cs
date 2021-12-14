using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities
{
    internal static class Weaving
    {
        public static Dictionary<ClassJobType, List<SpellData>> NonWeaponskills = new Dictionary<ClassJobType, List<SpellData>>();

        static Weaving()
        {
            //System Actions
            List<SpellData> SystemAbilitys = new List<SpellData>();

            //Role Actions
            List<SpellData> PhysicalRangedRoleAbilitys = new List<SpellData>();
            List<SpellData> MagicRangedRoleAbilitys = new List<SpellData>();
            List<SpellData> MeleeRoleAbilitys = new List<SpellData>();
            List<SpellData> TankRoleAbilitys = new List<SpellData>();
            List<SpellData> HealerRoleAbilitys = new List<SpellData>();

            //Jobs

            //Special Kids
            List<SpellData> Arcanist = new List<SpellData>();

            //Ranged Physical DPS
            List<SpellData> Archer = new List<SpellData>();
            List<SpellData> Bard = new List<SpellData>();
            List<SpellData> Machinist = new List<SpellData>();
            List<SpellData> Dancer = new List<SpellData>();

            //Ranged Magic DPS
            List<SpellData> Thaumaturge = new List<SpellData>();
            List<SpellData> BlackMage = new List<SpellData>();
            List<SpellData> Summoner = new List<SpellData>();
            List<SpellData> RedMage = new List<SpellData>();
            List<SpellData> BlueMage = new List<SpellData>();

            //Melee DPS
            List<SpellData> Rogue = new List<SpellData>();
            List<SpellData> Ninja = new List<SpellData>();
            List<SpellData> Pugilist = new List<SpellData>();
            List<SpellData> Monk = new List<SpellData>();
            List<SpellData> Lancer = new List<SpellData>();
            List<SpellData> Dragoon = new List<SpellData>();
            List<SpellData> Samurai = new List<SpellData>();
            List<SpellData> Reaper = new List<SpellData>();

            //Tank
            List<SpellData> Gladiator = new List<SpellData>();
            List<SpellData> Paladin = new List<SpellData>();
            List<SpellData> Marauder = new List<SpellData>();
            List<SpellData> Warrior = new List<SpellData>();
            List<SpellData> DarkKnight = new List<SpellData>();
            List<SpellData> Gunbreaker = new List<SpellData>();

            //Healer
            List<SpellData> Conjurer = new List<SpellData>();
            List<SpellData> WhiteMage = new List<SpellData>();
            List<SpellData> Scholar = new List<SpellData>();
            List<SpellData> Astrologian = new List<SpellData>();


            foreach (var action in DataManager.SpellCache)
            {
                //System Abilitys Like Sprint/Dig/Return
                if (action.Value.Job == ClassJobType.Adventurer && action.Value.SpellType == SpellType.System)
                {
                    SystemAbilitys.Add(action.Value);
                    continue;
                }

                #region RoleActions

                //Adding Physical Ranged Role Abilitys
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                               && action.Value.JobTypes.Contains(ClassJobType.Bard) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Archer)
                                               && action.Value.JobTypes.Contains(ClassJobType.Machinist) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Dancer))
                    PhysicalRangedRoleAbilitys.Add(action.Value);

                //Adding Magic Ranged Role Abilitys
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                               && action.Value.JobTypes.Contains(ClassJobType.Thaumaturge) 
                                               && action.Value.JobTypes.Contains(ClassJobType.BlackMage)
                                               && action.Value.JobTypes.Contains(ClassJobType.Arcanist) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Summoner)
                                               && action.Value.JobTypes.Contains(ClassJobType.RedMage)
                                               && action.Value.JobTypes.Contains(ClassJobType.BlueMage))
                    MagicRangedRoleAbilitys.Add(action.Value);

                //Adding Melee Role Abilitys
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                               && action.Value.JobTypes.Contains(ClassJobType.Pugilist) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Monk)
                                               && action.Value.JobTypes.Contains(ClassJobType.Rogue) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Ninja)
                                               && action.Value.JobTypes.Contains(ClassJobType.Lancer) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Dragoon)
                                               && action.Value.JobTypes.Contains(ClassJobType.Samurai))
                    MeleeRoleAbilitys.Add(action.Value);

                //Adding Tank Role Abilitys
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                               && action.Value.JobTypes.Contains(ClassJobType.Gunbreaker) 
                                               && action.Value.JobTypes.Contains(ClassJobType.DarkKnight)
                                               && action.Value.JobTypes.Contains(ClassJobType.Gladiator) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Paladin)
                                               && action.Value.JobTypes.Contains(ClassJobType.Marauder) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Warrior))
                    TankRoleAbilitys.Add(action.Value);

                //Adding Healer Role Abilitys
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                               && action.Value.JobTypes.Contains(ClassJobType.Conjurer) 
                                               && action.Value.JobTypes.Contains(ClassJobType.WhiteMage)
                                               && action.Value.JobTypes.Contains(ClassJobType.Scholar) 
                                               && action.Value.JobTypes.Contains(ClassJobType.Astrologian))
                    HealerRoleAbilitys.Add(action.Value);

                #endregion

                #region Jobs

                #region RangedPhysicalDPS

                //Archer
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Archer)
                    Archer.Add(action.Value);

                //Bard
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && (action.Value.Job == ClassJobType.Archer || action.Value.Job == ClassJobType.Bard))
                {
                    Bard.Add(action.Value);
                    continue;
                }

                //Machinist
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Machinist)
                {
                    Machinist.Add(action.Value);
                    continue;
                }

                //Dancer
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Dancer)
                {
                    Dancer.Add(action.Value);
                    continue;
                }

                #endregion

                #region RangedMagicDPS

                //Thaumaturge
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && action.Value.Job == ClassJobType.Thaumaturge)
                    Thaumaturge.Add(action.Value);

                //BlackMage
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && (action.Value.Job == ClassJobType.Thaumaturge || action.Value.Job == ClassJobType.BlackMage))
                {
                    BlackMage.Add(action.Value);
                    continue;
                }

                //Arcanist
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && action.Value.Job == ClassJobType.Arcanist)
                    Arcanist.Add(action.Value);

                //Summoner
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && (action.Value.Job == ClassJobType.Arcanist || action.Value.Job == ClassJobType.Summoner))
                    Summoner.Add(action.Value);


                //RedMage
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && action.Value.Job == ClassJobType.RedMage)
                {
                    RedMage.Add(action.Value);
                    continue;
                }

                //BlueMage
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && action.Value.Job == ClassJobType.BlueMage)
                {
                    BlueMage.Add(action.Value);
                    continue;
                }

                #endregion

                #region MeleeDPS

                //Rogue
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Rogue)
                    Rogue.Add(action.Value);

                //Ninja
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && (action.Value.Job == ClassJobType.Rogue || action.Value.Job == ClassJobType.Ninja))
                {
                    Ninja.Add(action.Value);
                    continue;
                }

                //Pugilist
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Pugilist)
                    Pugilist.Add(action.Value);

                //Monk
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && (action.Value.Job == ClassJobType.Pugilist || action.Value.Job == ClassJobType.Monk))
                {
                    Monk.Add(action.Value);
                    continue;
                }

                //Lancer
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Lancer)
                    Lancer.Add(action.Value);

                //Dragoon
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && (action.Value.Job == ClassJobType.Lancer || action.Value.Job == ClassJobType.Dragoon))
                {
                    Dragoon.Add(action.Value);
                    continue;
                }

                //Samurai
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Samurai)
                {
                    Samurai.Add(action.Value);
                    continue;
                }

                //Reaper
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Samurai)
                {
                    Reaper.Add(action.Value);
                    continue;
                }

                #endregion

                #region Tanks

                //Gladiator
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Gladiator)
                    Gladiator.Add(action.Value);

                //Paladin
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && (action.Value.Job == ClassJobType.Gladiator || action.Value.Job == ClassJobType.Paladin))
                {
                    Paladin.Add(action.Value);
                    continue;
                }

                //Marauder
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Marauder)
                    Marauder.Add(action.Value);

                //Warrior
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && (action.Value.Job == ClassJobType.Marauder || action.Value.Job == ClassJobType.Warrior))
                {
                    Warrior.Add(action.Value);
                    continue;
                }

                //DarkKnight
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && action.Value.Job == ClassJobType.DarkKnight)
                {
                    DarkKnight.Add(action.Value);
                    continue;
                }

                //Gunbreaker
                if (action.Value.IsPlayerAction && (action.Value.SpellType == SpellType.Ability || action.Value.SpellType == SpellType.Spell)
                                                && action.Value.Job == ClassJobType.Gunbreaker)
                {
                    Gunbreaker.Add(action.Value);
                    continue;
                }

                #endregion

                #region Healer

                //Conjurer
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && action.Value.Job == ClassJobType.Conjurer)
                    Conjurer.Add(action.Value);

                //WhiteMage
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && (action.Value.Job == ClassJobType.Conjurer || action.Value.Job == ClassJobType.WhiteMage))
                {
                    WhiteMage.Add(action.Value);
                    continue;
                }

                //Scholar
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && (action.Value.Job == ClassJobType.Arcanist || action.Value.Job == ClassJobType.Scholar))
                {
                    Scholar.Add(action.Value);
                    continue;
                }

                //Astrologian
                if (action.Value.IsPlayerAction && action.Value.SpellType == SpellType.Ability
                                                && action.Value.Job == ClassJobType.Astrologian)
                    Astrologian.Add(action.Value);

                #endregion

                #endregion

            }

            //MCH Flamethrower is an Ability but will trigger the GCD and cant be used during GCD
            Machinist.Remove(Spells.Flamethrower);

            //BLU Phantom Flurry is an Ability but will trigger the GCD and cant be used during GCD
            BlueMage.Remove(Spells.PhantomFlurry);

            //Adding All Skills Into Our Dictionary

            //Ranged Physical DPS
            NonWeaponskills.Add(ClassJobType.Archer, Archer.Concat(PhysicalRangedRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Bard, Bard.Concat(PhysicalRangedRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Machinist, Machinist.Concat(PhysicalRangedRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Dancer, Dancer.Concat(PhysicalRangedRoleAbilitys).Concat(SystemAbilitys).ToList());

            //Ranged Magic DPS
            NonWeaponskills.Add(ClassJobType.Thaumaturge, Thaumaturge.Concat(MagicRangedRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.BlackMage, BlackMage.Concat(MagicRangedRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Summoner, Summoner.Concat(MagicRangedRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.RedMage, RedMage.Concat(MagicRangedRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.BlueMage, BlueMage.Concat(MagicRangedRoleAbilitys).Concat(SystemAbilitys).ToList());

            //Melee DPS
            NonWeaponskills.Add(ClassJobType.Rogue, Rogue.Concat(MeleeRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Ninja, Ninja.Concat(MeleeRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Pugilist, Pugilist.Concat(MeleeRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Monk, Monk.Concat(MeleeRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Lancer, Lancer.Concat(MeleeRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Dragoon, Dragoon.Concat(MeleeRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Samurai, Samurai.Concat(MeleeRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Reaper, Reaper.Concat(MeleeRoleAbilitys).Concat(SystemAbilitys).ToList());

            //Tanks
            NonWeaponskills.Add(ClassJobType.Gladiator, Gladiator.Concat(TankRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Paladin, Paladin.Concat(TankRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Marauder, Marauder.Concat(TankRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Warrior, Warrior.Concat(TankRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.DarkKnight, DarkKnight.Concat(TankRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Gunbreaker, Gunbreaker.Concat(TankRoleAbilitys).Concat(SystemAbilitys).ToList());

            //Healer
            NonWeaponskills.Add(ClassJobType.Conjurer, Conjurer.Concat(HealerRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.WhiteMage, WhiteMage.Concat(HealerRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Scholar, Scholar.Concat(HealerRoleAbilitys).Concat(SystemAbilitys).ToList());
            NonWeaponskills.Add(ClassJobType.Astrologian, Astrologian.Concat(HealerRoleAbilitys).Concat(SystemAbilitys).ToList());

            //Special Kids
            NonWeaponskills.Add(ClassJobType.Arcanist, Arcanist.Concat(MagicRangedRoleAbilitys).Concat(SystemAbilitys).ToList());

        }

        public static int GetCurrentWeavingCounter()
        {
            var weavingCounter = 0;

            if (Casting.SpellCastHistory.Count < 2)
                return 0;

            if (NonWeaponskills[Core.Me.CurrentJob].Contains(Casting.SpellCastHistory.ElementAt(0).Spell))
                weavingCounter += 1;
            else
                return 0;

            if (NonWeaponskills[Core.Me.CurrentJob].Contains(Casting.SpellCastHistory.ElementAt(1).Spell))
                weavingCounter += 1;

            return weavingCounter;
        }
    }
}