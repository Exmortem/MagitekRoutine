using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Models.Dragoon;
using System;
using System.Collections.Generic;


namespace Magitek.Utilities.Routines
{
    internal static class Dragoon
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Dragoon, Spells.TrueThrust);

        public static SpellData HighJump => Core.Me.ClassLevel < 74
                                            ? Spells.Jump
                                            : Spells.HighJump;

        public static SpellData HeavensThrust => Core.Me.ClassLevel < 86
                                            ? Spells.FullThrust
                                            : Spells.HeavensThrust;

        public static SpellData ChaoticSpring => Core.Me.ClassLevel < 86
                                            ? Spells.ChaosThrust
                                            : Spells.ChaoticSpring;

        public static bool CanContinueComboAfter(SpellData LastSpellExecuted)
        {
            if (ActionManager.ComboTimeLeft <= 0)
                return false;

            if (ActionManager.LastSpell.Id != LastSpellExecuted.Id)
                return false;

            return true;
        }

        public static List<SpellData> JumpsList = new List<SpellData>()
        {
            HighJump,
            Spells.SpineshatterDive,
            Spells.DragonfireDive,
            Spells.MirageDive,
            Spells.Stardiver
        };

        public static List<SpellData> SingleWeaveJumpsList = new List<SpellData>()
        {
            HighJump,
            Spells.SpineshatterDive,
            Spells.DragonfireDive,
            Spells.Stardiver
        };

        public static int GetWeight(Character c)
        {
            switch (c.CurrentJob)
            {
                //Melee DPS
                case ClassJobType.Monk:
                case ClassJobType.Pugilist:
                    return DragoonSettings.Instance.MnkEyeWeight;
                case ClassJobType.Reaper:
                    return DragoonSettings.Instance.RprEyeWeight;
                case ClassJobType.Ninja:
                case ClassJobType.Rogue:
                    return DragoonSettings.Instance.NinEyeWeight;
                case ClassJobType.Dragoon:
                case ClassJobType.Lancer:
                    return DragoonSettings.Instance.DrgEyeWeight;
                case ClassJobType.Samurai:
                    return DragoonSettings.Instance.SamEyeWeight;

                //Range Physical DPS
                case ClassJobType.Machinist:
                    return DragoonSettings.Instance.MchEyeWeight;
                case ClassJobType.Bard:
                case ClassJobType.Archer:
                    return DragoonSettings.Instance.BrdEyeWeight;
                case ClassJobType.Dancer:
                    return DragoonSettings.Instance.DncEyeWeight;

                //Range Magical DPS
                case ClassJobType.RedMage:
                    return DragoonSettings.Instance.RdmEyeWeight;
                case ClassJobType.Summoner:
                case ClassJobType.Arcanist:
                    return DragoonSettings.Instance.SmnEyeWeight;
                case ClassJobType.BlackMage:
                case ClassJobType.Thaumaturge:
                    return DragoonSettings.Instance.BlmEyeWeight;

                //Tanks
                case ClassJobType.Paladin:
                case ClassJobType.Gladiator:
                    return DragoonSettings.Instance.PldEyeWeight;
                case ClassJobType.Warrior:
                case ClassJobType.Marauder:
                    return DragoonSettings.Instance.WarEyeWeight;
                case ClassJobType.DarkKnight:
                    return DragoonSettings.Instance.DrkEyeWeight;
                case ClassJobType.Gunbreaker:
                    return DragoonSettings.Instance.GnbEyeWeight;

                // Healers
                case ClassJobType.WhiteMage:
                case ClassJobType.Conjurer:
                    return DragoonSettings.Instance.WhmEyeWeight;
                case ClassJobType.Scholar:
                    return DragoonSettings.Instance.SchEyeWeight;
                case ClassJobType.Sage:
                    return DragoonSettings.Instance.SgeEyeWeight;
                case ClassJobType.Astrologian:
                    return DragoonSettings.Instance.AstEyeWeight;
            }

            return c.CurrentJob == ClassJobType.Adventurer ? 70 : 0;
        }
    }
}
