using System.Linq;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Logic;
using Magitek.Models.Astrologian;
using Magitek.Models.Bard;
using Magitek.Models.BlackMage;
using Magitek.Models.Dancer;
using Magitek.Models.DarkKnight;
using Magitek.Models.Dragoon;
using Magitek.Models.Gunbreaker;
using Magitek.Models.Machinist;
using Magitek.Models.Monk;
using Magitek.Models.Ninja;
using Magitek.Models.Paladin;
using Magitek.Models.RedMage;
using Magitek.Models.Samurai;
using Magitek.Models.Scholar;
using Magitek.Models.Summoner;
using Magitek.Models.Warrior;
using Magitek.Models.WhiteMage;
using Magitek.ViewModels;
using PropertyChanged;
using TreeSharp;

namespace Magitek.Utilities.Managers
{
    [AddINotifyPropertyChangedInterface]
    internal static class RotationManager
    {
        public static ClassJobType CurrentRotation { get; set; }

        public static void Reset()
        {
            var hotkeys = HotkeyManager.RegisteredHotkeys.Select(r => r.Name).Where(r => r.Contains("Magitek"));

            foreach (var hk in hotkeys)
            {
                HotkeyManager.Unregister(hk);
            }
            OpenerLogic.OpenerQueue.Clear();
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (Core.Me.CurrentJob)
            {
                    case ClassJobType.Gladiator:
                    case ClassJobType.Paladin:
                        SetPaladin();
                        break;
                    case ClassJobType.Pugilist:
                    case ClassJobType.Monk:
                        SetMonk();
                        break;
                    case ClassJobType.Marauder:
                    case ClassJobType.Warrior:
                        SetWarrior();
                        break;
                    case ClassJobType.Lancer:
                    case ClassJobType.Dragoon:
                        SetDragoon();
                        break;
                    case ClassJobType.Archer:
                    case ClassJobType.Bard:
                        SetBard();
                        break;
                    case ClassJobType.Conjurer:
                    case ClassJobType.WhiteMage:
                        SetWhiteMage();
                        break;
                    case ClassJobType.Thaumaturge:
                    case ClassJobType.BlackMage:
                        SetBlackMage();
                        break;
                    case ClassJobType.Arcanist:
                    case ClassJobType.Summoner:
                        SetSummoner();
                        break;
                    case ClassJobType.Scholar:
                        SetScholar();
                        break;
                    case ClassJobType.Rogue:
                    case ClassJobType.Ninja:
                        SetNinja();
                        break;
                    case ClassJobType.Machinist:
                        SetMachinist();
                        break;
                    case ClassJobType.DarkKnight:
                        SetDarkKnight();
                        break;
                    case ClassJobType.Astrologian:
                        SetAstrologian();
                        break;
                    case ClassJobType.Samurai:
                        SetSamurai();
                        break;
                    case ClassJobType.RedMage:
                        SetRedMage();
                        break;
                    case ClassJobType.Gunbreaker:
                        SetGunbreaker();
                        break;
                    case ClassJobType.Dancer:
                        SetDancer();
                        break;

                    //case ClassJobType.BlueMage:
                    //    SetBlueMage();
                    //    break;
            }

            ResetRbComposites();
        }

        public static void ResetRbComposites()
        {
            TreeHooks.Instance.ReplaceHook("Rest", Magitek.RestBehavior);
            TreeHooks.Instance.ReplaceHook("PreCombat", Magitek.PreCombatBuffBehavior);
            TreeHooks.Instance.ReplaceHook("Pull", Magitek.PullBehavior);
            TreeHooks.Instance.ReplaceHook("Heal", Magitek.HealBehavior);
            TreeHooks.Instance.ReplaceHook("CombatBuff", Magitek.CombatBuffBehavior);
            TreeHooks.Instance.ReplaceHook("Combat", Magitek.CombatBehavior);
        }

        private static void SetScholar()
        {
            CurrentRotation = ClassJobType.Scholar;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Scholar.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Scholar.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Scholar.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Scholar.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Scholar.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Scholar.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Scholar.Rest.Execute());
            ScholarSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Scholar";
        }
  
        private static void SetPaladin()
        {
            CurrentRotation = ClassJobType.Paladin;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Paladin.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Paladin.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Paladin.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Paladin.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Paladin.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Paladin.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Paladin.Rest.Execute());
            PaladinSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Paladin";
        }

        private static void SetWhiteMage()
        {
            CurrentRotation = ClassJobType.WhiteMage;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.WhiteMage.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.WhiteMage.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.WhiteMage.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.WhiteMage.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.WhiteMage.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.WhiteMage.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.WhiteMage.Rest.Execute());
            WhiteMageSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "WhiteMage";
        }
        
        private static void SetBard()
        {
            CurrentRotation = ClassJobType.Bard;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Bard.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Bard.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Bard.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Bard.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Bard.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Bard.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Bard.Rest.Execute());
            BardSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Bard";
        }

        private static void SetAstrologian()
        {
            CurrentRotation = ClassJobType.Astrologian;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Astrologian.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Astrologian.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Astrologian.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Astrologian.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Astrologian.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Astrologian.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Astrologian.Rest.Execute());
            AstrologianSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Astrologian";
        }

        private static void SetDarkKnight()
        {
            CurrentRotation = ClassJobType.DarkKnight;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.DarkKnight.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.DarkKnight.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.DarkKnight.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.DarkKnight.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.DarkKnight.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.DarkKnight.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.DarkKnight.Rest.Execute());
            DarkKnightSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "DarkKnight";
        }

        private static void SetDragoon()
        {
            CurrentRotation = ClassJobType.Dragoon;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Dragoon.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Dragoon.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Dragoon.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Dragoon.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Dragoon.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Dragoon.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Dragoon.Rest.Execute());
            DragoonSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Dragoon";
        }

        private static void SetWarrior()
        {
            CurrentRotation = ClassJobType.Warrior;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Warrior.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Warrior.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Warrior.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Warrior.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Warrior.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Warrior.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Warrior.Rest.Execute());
            WarriorSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Warrior";
        }

        private static void SetMonk()
        {
            CurrentRotation = ClassJobType.Monk;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Monk.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Monk.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Monk.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Monk.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Monk.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Monk.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Monk.Rest.Execute());
            MonkSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Monk";
        }

        private static void SetNinja()
        {
            CurrentRotation = ClassJobType.Ninja;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Ninja.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Ninja.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Ninja.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Ninja.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Ninja.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Ninja.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Ninja.Rest.Execute());
            NinjaSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Ninja";
        }

        private static void SetSamurai()
        {
            CurrentRotation = ClassJobType.Samurai;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Samurai.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Samurai.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Samurai.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Samurai.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Samurai.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Samurai.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Samurai.Rest.Execute());
            SamuraiSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Samurai";
        }

        private static void SetMachinist()
        {
            CurrentRotation = ClassJobType.Machinist;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Machinist.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Machinist.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Machinist.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Machinist.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Machinist.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Machinist.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Machinist.Rest.Execute());
            MachinistSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Machinist";
        }

        private static void SetBlackMage()
        {
            CurrentRotation = ClassJobType.BlackMage;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.BlackMage.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.BlackMage.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.BlackMage.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.BlackMage.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.BlackMage.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.BlackMage.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.BlackMage.Rest.Execute());
            BlackMageSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "BlackMage";
        }

        private static void SetRedMage()
        {
            CurrentRotation = ClassJobType.RedMage;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.RedMage.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.RedMage.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.RedMage.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.RedMage.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.RedMage.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.RedMage.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.RedMage.Rest.Execute());
            RedMageSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "RedMage";
        }

        private static void SetSummoner()
        {
            CurrentRotation = ClassJobType.Summoner;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Summoner.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Summoner.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Summoner.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Summoner.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Summoner.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Summoner.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Summoner.Rest.Execute());
            SummonerSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Summoner";
        }

        private static void SetDancer()
        {
            CurrentRotation = ClassJobType.Dancer;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Dancer.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Dancer.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Dancer.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Dancer.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Dancer.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Dancer.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Dancer.Rest.Execute());
            DancerSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Dancer";
        }
        private static void SetGunbreaker()
        {
            CurrentRotation = ClassJobType.Gunbreaker;
            Magitek.CombatBehavior = new ActionRunCoroutine(r => Rotations.Gunbreaker.Combat.Execute());
            Magitek.HealBehavior = new ActionRunCoroutine(r => Rotations.Gunbreaker.Heal.Execute());
            Magitek.PreCombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Gunbreaker.PreCombatBuff.Execute());
            Magitek.PullBehavior = new ActionRunCoroutine(r => Rotations.Gunbreaker.Pull.Execute());
            Magitek.PullBuffBehavior = new ActionRunCoroutine(r => Rotations.Gunbreaker.PullBuff.Execute());
            Magitek.CombatBuffBehavior = new ActionRunCoroutine(r => Rotations.Gunbreaker.CombatBuff.Execute());
            Magitek.RestBehavior = new ActionRunCoroutine(r => Rotations.Gunbreaker.Rest.Execute());
            GunbreakerSettings.Instance.Save();
            BaseSettings.Instance.CurrentRoutine = "Gunbreaker";
        }
    }
}
