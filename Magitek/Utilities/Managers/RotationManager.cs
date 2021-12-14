using ff14bot;
using ff14bot.Enums;
using PropertyChanged;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Utilities.Managers
{
    [AddINotifyPropertyChangedInterface]
    internal static class RotationManager
    {
        public static IRotation Rotation => new RotationComposites();
        public static ClassJobType CurrentRotation => Core.Me.CurrentJob;
    }

    public interface IRotation
    {
        Task<bool> Rest();
        Task<bool> PreCombatBuff();
        Task<bool> Pull();
        Task<bool> Heal();
        Task<bool> CombatBuff();
        Task<bool> Combat();
        Task<bool> PvP();
    }

    public abstract class Rotation : IRotation
    {
        public abstract Task<bool> Rest();
        public abstract Task<bool> PreCombatBuff();
        public abstract Task<bool> Pull();
        public abstract Task<bool> Heal();
        public abstract Task<bool> CombatBuff();
        public abstract Task<bool> Combat();
        public abstract Task<bool> PvP();
    }

    [AddINotifyPropertyChangedInterface]
    internal class RotationComposites : Rotation
    {
        public override async Task<bool> Rest()
        {
            await Chocobo.HandleChocobo();

            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    return await Rotations.Paladin.Rest();

                case ClassJobType.Pugilist:
                case ClassJobType.Monk:
                    return await Rotations.Monk.Rest();

                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                    return await Rotations.Warrior.Rest();

                case ClassJobType.Lancer:
                case ClassJobType.Dragoon:
                    return await Rotations.Dragoon.Rest();

                case ClassJobType.Archer:
                case ClassJobType.Bard:
                    return await Rotations.Bard.Rest();

                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                    return await Rotations.WhiteMage.Rest();

                case ClassJobType.Thaumaturge:
                case ClassJobType.BlackMage:
                    return await Rotations.BlackMage.Rest();

                case ClassJobType.Arcanist:
                case ClassJobType.Summoner:
                    return await Rotations.Summoner.Rest();

                case ClassJobType.Scholar:
                    return await Rotations.Scholar.Rest();

                case ClassJobType.Rogue:
                case ClassJobType.Ninja:
                    return await Rotations.Ninja.Rest();

                case ClassJobType.Machinist:
                    return await Rotations.Machinist.Rest();

                case ClassJobType.DarkKnight:
                    return await Rotations.DarkKnight.Rest();

                case ClassJobType.Astrologian:
                    return await Rotations.Astrologian.Rest();

                case ClassJobType.Samurai:
                    return await Rotations.Samurai.Rest();

                case ClassJobType.BlueMage:
                    return await Rotations.BlueMage.Rest();

                case ClassJobType.RedMage:
                    return await Rotations.RedMage.Rest();

                case ClassJobType.Gunbreaker:
                    return await Rotations.Gunbreaker.Rest();

                case ClassJobType.Dancer:
                    return await Rotations.Dancer.Rest();

                case ClassJobType.Reaper:
                    return await Rotations.Reaper.Rest();

                default:
                    return false;
            }
        }

        public override async Task<bool> PreCombatBuff()
        {
            Group.UpdateAllies();
            await Chocobo.HandleChocobo();

            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    return await Rotations.Paladin.PreCombatBuff();

                case ClassJobType.Pugilist:
                case ClassJobType.Monk:
                    return await Rotations.Monk.PreCombatBuff();

                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                    return await Rotations.Warrior.PreCombatBuff();

                case ClassJobType.Lancer:
                case ClassJobType.Dragoon:
                    return await Rotations.Dragoon.PreCombatBuff();

                case ClassJobType.Archer:
                case ClassJobType.Bard:
                    return await Rotations.Bard.PreCombatBuff();

                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                    Group.UpdateAllies(Routines.WhiteMage.GroupExtension);
                    Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
                    return await Rotations.WhiteMage.PreCombatBuff();

                case ClassJobType.Thaumaturge:
                case ClassJobType.BlackMage:
                    return await Rotations.BlackMage.PreCombatBuff();

                case ClassJobType.Arcanist:
                case ClassJobType.Summoner:
                    return await Rotations.Summoner.PreCombatBuff();

                case ClassJobType.Scholar:
                    Group.UpdateAllies(Routines.Scholar.GroupExtension);
                    Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
                    return await Rotations.Scholar.PreCombatBuff();

                case ClassJobType.Rogue:
                case ClassJobType.Ninja:
                    return await Rotations.Ninja.PreCombatBuff();

                case ClassJobType.Machinist:
                    return await Rotations.Machinist.PreCombatBuff();

                case ClassJobType.DarkKnight:
                    return await Rotations.DarkKnight.PreCombatBuff();

                case ClassJobType.Astrologian:
                    Group.UpdateAllies(Routines.Astrologian.GroupExtension);
                    Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
                    return await Rotations.Astrologian.PreCombatBuff();

                case ClassJobType.Samurai:
                    return await Rotations.Samurai.PreCombatBuff();

                case ClassJobType.BlueMage:
                    return await Rotations.BlueMage.PreCombatBuff();

                case ClassJobType.RedMage:
                    return await Rotations.RedMage.PreCombatBuff();

                case ClassJobType.Gunbreaker:
                    return await Rotations.Gunbreaker.PreCombatBuff();

                case ClassJobType.Dancer:
                    return await Rotations.Dancer.PreCombatBuff();

                case ClassJobType.Reaper:
                    return await Rotations.Reaper.PreCombatBuff();

                default:
                    return false;
            }
        }

        public override async Task<bool> Pull()
        {
            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    return await Rotations.Paladin.Pull();

                case ClassJobType.Pugilist:
                case ClassJobType.Monk:
                    return await Rotations.Monk.Pull();

                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                    return await Rotations.Warrior.Pull();

                case ClassJobType.Lancer:
                case ClassJobType.Dragoon:
                    return await Rotations.Dragoon.Pull();

                case ClassJobType.Archer:
                case ClassJobType.Bard:
                    return await Rotations.Bard.Pull();

                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                    return await Rotations.WhiteMage.Pull();

                case ClassJobType.Thaumaturge:
                case ClassJobType.BlackMage:
                    return await Rotations.BlackMage.Pull();

                case ClassJobType.Arcanist:
                case ClassJobType.Summoner:
                    return await Rotations.Summoner.Pull();

                case ClassJobType.Scholar:
                    return await Rotations.Scholar.Pull();

                case ClassJobType.Rogue:
                case ClassJobType.Ninja:
                    return await Rotations.Ninja.Pull();

                case ClassJobType.Machinist:
                    return await Rotations.Machinist.Pull();

                case ClassJobType.DarkKnight:
                    return await Rotations.DarkKnight.Pull();

                case ClassJobType.Astrologian:
                    return await Rotations.Astrologian.Pull();

                case ClassJobType.Samurai:
                    return await Rotations.Samurai.Pull();

                case ClassJobType.BlueMage:
                    return await Rotations.BlueMage.Pull();

                case ClassJobType.RedMage:
                    return await Rotations.RedMage.Pull();

                case ClassJobType.Gunbreaker:
                    return await Rotations.Gunbreaker.Pull();

                case ClassJobType.Dancer:
                    return await Rotations.Dancer.Pull();

                case ClassJobType.Reaper:
                    return await Rotations.Reaper.Pull();

                default:
                    return false;
            }
        }

        public override async Task<bool> Heal()
        {
            Group.UpdateAllies();
            await Chocobo.HandleChocobo();

            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    return await Rotations.Paladin.Heal();

                case ClassJobType.Pugilist:
                case ClassJobType.Monk:
                    return await Rotations.Monk.Heal();

                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                    return await Rotations.Warrior.Heal();

                case ClassJobType.Lancer:
                case ClassJobType.Dragoon:
                    return await Rotations.Dragoon.Heal();

                case ClassJobType.Archer:
                case ClassJobType.Bard:
                    return await Rotations.Bard.Heal();

                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                    Group.UpdateAllies(Routines.WhiteMage.GroupExtension);
                    Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
                    return await Rotations.WhiteMage.Heal();

                case ClassJobType.Thaumaturge:
                case ClassJobType.BlackMage:
                    return await Rotations.BlackMage.Heal();

                case ClassJobType.Arcanist:
                case ClassJobType.Summoner:
                    return await Rotations.Summoner.Heal();

                case ClassJobType.Scholar:
                    Group.UpdateAllies(Routines.Scholar.GroupExtension);
                    Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
                    return await Rotations.Scholar.Heal();

                case ClassJobType.Rogue:
                case ClassJobType.Ninja:
                    return await Rotations.Ninja.Heal();

                case ClassJobType.Machinist:
                    return await Rotations.Machinist.Heal();

                case ClassJobType.DarkKnight:
                    return await Rotations.DarkKnight.Heal();

                case ClassJobType.Astrologian:
                    Group.UpdateAllies(Routines.Astrologian.GroupExtension);
                    Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
                    return await Rotations.Astrologian.Heal();

                case ClassJobType.Samurai:
                    return await Rotations.Samurai.Heal();

                case ClassJobType.BlueMage:
                    return await Rotations.BlueMage.Heal();

                case ClassJobType.RedMage:
                    return await Rotations.RedMage.Heal();

                case ClassJobType.Gunbreaker:
                    return await Rotations.Gunbreaker.Heal();

                case ClassJobType.Dancer:
                    return await Rotations.Dancer.Heal();

                case ClassJobType.Reaper:
                    return await Rotations.Reaper.Heal();

                default:
                    return false;
            }
        }

        public override async Task<bool> CombatBuff()
        {
            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    return await Rotations.Paladin.CombatBuff();

                case ClassJobType.Pugilist:
                case ClassJobType.Monk:
                    return await Rotations.Monk.CombatBuff();

                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                    return await Rotations.Warrior.CombatBuff();

                case ClassJobType.Lancer:
                case ClassJobType.Dragoon:
                    return await Rotations.Dragoon.CombatBuff();

                case ClassJobType.Archer:
                case ClassJobType.Bard:
                    return await Rotations.Bard.CombatBuff();

                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                    return await Rotations.WhiteMage.CombatBuff();

                case ClassJobType.Thaumaturge:
                case ClassJobType.BlackMage:
                    return await Rotations.BlackMage.CombatBuff();

                case ClassJobType.Arcanist:
                case ClassJobType.Summoner:
                    return await Rotations.Summoner.CombatBuff();

                case ClassJobType.Scholar:
                    return await Rotations.Scholar.CombatBuff();

                case ClassJobType.Rogue:
                case ClassJobType.Ninja:
                    return await Rotations.Ninja.CombatBuff();

                case ClassJobType.Machinist:
                    return await Rotations.Machinist.CombatBuff();

                case ClassJobType.DarkKnight:
                    return await Rotations.DarkKnight.CombatBuff();

                case ClassJobType.Astrologian:
                    return await Rotations.Astrologian.CombatBuff();

                case ClassJobType.Samurai:
                    return await Rotations.Samurai.CombatBuff();

                case ClassJobType.BlueMage:
                    return await Rotations.BlueMage.CombatBuff();

                case ClassJobType.RedMage:
                    return await Rotations.RedMage.CombatBuff();

                case ClassJobType.Gunbreaker:
                    return await Rotations.Gunbreaker.CombatBuff();

                case ClassJobType.Dancer:
                    return await Rotations.Dancer.CombatBuff();

                case ClassJobType.Reaper:
                    return await Rotations.Reaper.CombatBuff();

                default:
                    return false;
            }
        }

        public override async Task<bool> Combat()
        {
            Group.UpdateAllies();
            await Chocobo.HandleChocobo();

            switch (RotationManager.CurrentRotation)
            {
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    return await Rotations.Paladin.Combat();

                case ClassJobType.Pugilist:
                case ClassJobType.Monk:
                    return await Rotations.Monk.Combat();

                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                    return await Rotations.Warrior.Combat();

                case ClassJobType.Lancer:
                case ClassJobType.Dragoon:
                    return await Rotations.Dragoon.Combat();

                case ClassJobType.Archer:
                case ClassJobType.Bard:
                    return await Rotations.Bard.Combat();

                case ClassJobType.Conjurer:
                case ClassJobType.WhiteMage:
                    return await Rotations.WhiteMage.Combat();

                case ClassJobType.Thaumaturge:
                case ClassJobType.BlackMage:
                    return await Rotations.BlackMage.Combat();

                case ClassJobType.Arcanist:
                case ClassJobType.Summoner:
                    return await Rotations.Summoner.Combat();

                case ClassJobType.Scholar:
                    return await Rotations.Scholar.Combat();

                case ClassJobType.Rogue:
                case ClassJobType.Ninja:
                    return await Rotations.Ninja.Combat();

                case ClassJobType.Machinist:
                    return await Rotations.Machinist.Combat();

                case ClassJobType.DarkKnight:
                    return await Rotations.DarkKnight.Combat();

                case ClassJobType.Astrologian:
                    return await Rotations.Astrologian.Combat();

                case ClassJobType.Samurai:
                    return await Rotations.Samurai.Combat();

                case ClassJobType.BlueMage:
                    return await Rotations.BlueMage.Combat();

                case ClassJobType.RedMage:
                    return await Rotations.RedMage.Combat();

                case ClassJobType.Gunbreaker:
                    return await Rotations.Gunbreaker.Combat();

                case ClassJobType.Dancer:
                    return await Rotations.Dancer.Combat();

                case ClassJobType.Reaper:
                    return await Rotations.Reaper.Combat();

                default:
                    return false;
            }
        }

        public override async Task<bool> PvP()
        {
            switch (RotationManager.CurrentRotation)
            {
                //case ClassJobType.Gladiator:
                //case ClassJobType.Paladin:
                //    return await Rotations.Paladin.PvP();    

                //case ClassJobType.Pugilist:
                //case ClassJobType.Monk:
                //    return await Rotations.Monk.PvP();    

                //case ClassJobType.Marauder:
                //case ClassJobType.Warrior:
                //    return await Rotations.Warrior.PvP();    

                //case ClassJobType.Lancer:
                //case ClassJobType.Dragoon:
                //    return await Rotations.Dragoon.PvP();    

                //case ClassJobType.Archer:
                //case ClassJobType.Bard:
                //    return await Rotations.Bard.PvP();    

                //case ClassJobType.Conjurer:
                //case ClassJobType.WhiteMage:
                //    return await Rotations.WhiteMage.PvP();    

                //case ClassJobType.Thaumaturge:
                //case ClassJobType.BlackMage:
                //    return await Rotations.BlackMage.PvP();    

                //case ClassJobType.Arcanist:
                //case ClassJobType.Summoner:
                //    return await Rotations.Summoner.PvP();    

                //case ClassJobType.Scholar:
                //    return await Rotations.Scholar.PvP();    

                //case ClassJobType.Rogue:
                //case ClassJobType.Ninja:
                //    return await Rotations.Ninja.PvP();    

                //case ClassJobType.Machinist:
                //    return await Rotations.Machinist.PvP();    

                //case ClassJobType.DarkKnight:
                //    return await Rotations.DarkKnight.PvP();    

                //case ClassJobType.Astrologian:
                  //  return await Rotations.Astrologian.PvP();

                //case ClassJobType.Samurai:
                //    return await Rotations.Samurai.PvP();

                //case ClassJobType.BlueMage:
                //    return await Rotations.BlueMage.PvP(); 

                //case ClassJobType.RedMage:
                //    return await Rotations.RedMage.PvP();

                //case ClassJobType.Gunbreaker:
                //    return await Rotations.Gunbreaker.PvP();    

                //case ClassJobType.Dancer:
                //    return await Rotations.Dancer.PvP();    

                default:
                    return false;
            }
        }
    }
}
