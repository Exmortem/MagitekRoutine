using System.Collections.Generic;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Utilities.Routines
{
    internal static class Machinist
    {
        public static int AnimationLock = 700;

        //skill upgrades
        public static SpellData HeatedSplitShot => Core.Me.ClassLevel < 54 || !ActionManager.HasSpell(Spells.HeatedSplitShot.Id)
                                                    ? Spells.SplitShot
                                                    : Spells.HeatedSplitShot;
        public static SpellData HeatedSlugShot => Core.Me.ClassLevel < 60 || !ActionManager.HasSpell(Spells.HeatedSlugShot.Id)
                                                    ? Spells.SlugShot
                                                    : Spells.HeatedSlugShot;

        public static SpellData HeatedCleanShot => Core.Me.ClassLevel < 64
                                                    ? Spells.CleanShot
                                                    : Spells.HeatedCleanShot;

        public static SpellData GaussRicochet => Core.Me.ClassLevel < 74 
                                                    ? Spells.GaussRound
                                                    : Spells.Ricochet;

        public static SpellData HotAirAnchor => Core.Me.ClassLevel < 76
                                                    ? Spells.HotShot
                                                    : Spells.AirAnchor;

        public static SpellData RookQueenPet => Core.Me.ClassLevel < 80
                                                    ? Spells.RookAutoturret
                                                    : Spells.AutomationQueen;

        public static SpellData RookQueenOverdrive => Core.Me.ClassLevel < 80
                                                    ? Spells.RookOverdrive
                                                    : Spells.QueenOverdrive;



    }
}
