using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;

namespace Magitek.Logic.BlackMage
{
    internal static class Aoe
    {
        public static async Task<bool> Foul()
        {
            //requires Polyglot
            if (!ActionResourceManager.BlackMage.PolyglotStatus)
                return false;

            //Can't use whatcha don't have
            if (Core.Me.ClassLevel < 70)
                return false;

            //if you don't have Aspect Mastery, just SMASH THAT FOUL BUTTON
            if (Core.Me.ClassLevel < 80)
                if (ActionResourceManager.BlackMage.UmbralStacks == 3)
                    return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            //Only use in Umbral 3
            if (ActionResourceManager.BlackMage.AstralStacks == 3)
                return false;

            //If we have Umbral hearts, Freeze has gone off
            if (ActionResourceManager.BlackMage.UmbralHearts >= 1)
                if (Casting.LastSpell != Spells.Foul)
                    return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            if (Casting.LastSpell == Spells.Transpose)
                return await Spells.Foul.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Flare()
        {
            if (Core.Me.ClassLevel < 72)
            {
                if (Core.Me.ClassLevel >= 50
                    && Core.Me.ClassLevel < 60
                    && Casting.LastSpell == Spells.Fire2)
                {
                    if (Core.Me.CurrentMana <= 3000)
                        return await Spells.Flare.Cast(Core.Me.CurrentTarget);

                    if (Core.Me.CurrentMana < 800)
                        return await Spells.Transpose.Cast(Core.Me);

                    return false;
                }
                if (Core.Me.ClassLevel >= 60
                    && Core.Me.ClassLevel < 68
                    && Casting.LastSpell == Spells.Fire4)
                {
                    if (Core.Me.CurrentMana < 2400)
                        return await Spells.Flare.Cast(Core.Me.CurrentTarget);

                    if (Core.Me.CurrentMana < 800)
                        return await Spells.Transpose.Cast(Core.Me);

                    return false;
                }
                if (Core.Me.ClassLevel >= 68)
                {
                    if (Casting.LastSpell == Spells.Fire3)
                        return await Spells.Flare.Cast(Core.Me.CurrentTarget);

                    if (Core.Me.CurrentMana >= 800)
                        return await Spells.Flare.Cast(Core.Me.CurrentTarget);

                    if (Core.Me.CurrentMana < 800)
                        return await Spells.Transpose.Cast(Core.Me);

                    return false;
                }
                return false;
            }

            //Only cast Flare if you have enough mp
            if (Core.Me.CurrentMana < 800)
                return false;

            return await Spells.Flare.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Freeze()
        {
            //If we don't have Freeze, how can we cast it?
            if (Core.Me.ClassLevel < 35)
                return false;

            if (Core.Me.ClassLevel >= 35
                && Core.Me.ClassLevel < 50)
                return await Spells.Freeze.Cast(Core.Me.CurrentTarget);

            //If we have enhanced freeze and 1 Umbral heart, stop
            if (Core.Me.ClassLevel >= 68
                && ActionResourceManager.BlackMage.UmbralHearts >= 1)
                return false;

            //Wait until we don't have enough mp to cast Flare  
            if (Core.Me.CurrentMana > 799)
                return false;

            return await Spells.Freeze.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Thunder4()
        {
            //Only cast in Umbral 3
            if (ActionResourceManager.BlackMage.AstralStacks > 0)
                return false;

            //If we don't need to refresh Thunder, skip
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder4, true, 4500))
                return false;

            //Same for Thunder2
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder2, true, 4500))
                return false;

            //Same for Thunder3
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder3, true, 4500))
                return false;

            //Same for Thunder
            if (!Core.Me.CurrentTarget.HasAura(Auras.Thunder, true, 4500))
                return false;

            if (Core.Me.ClassLevel < 68)
                if (Casting.LastSpell != Spells.Thunder2)
                    if (Casting.LastSpell == Spells.Transpose
                        || Casting.LastSpell == Spells.Blizzard2
                        || Casting.LastSpell == Spells.Freeze)
                        return await Spells.Thunder2.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel < 72)
                if (Casting.LastSpell != Spells.Thunder4)
                {
                    if (Casting.LastSpell == Spells.Transpose
                        || Casting.LastSpell == Spells.Foul
                        || Casting.LastSpell == Spells.Freeze)
                        return await Spells.Thunder4.Cast(Core.Me.CurrentTarget);
                }

            if (Core.Me.ClassLevel >= 72)
                if (Casting.LastSpell != Spells.Thunder4)
                    if (Casting.LastSpell == Spells.Freeze
                        || Casting.LastSpell == Spells.Foul)
                        return await Spells.Thunder4.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Fire2()
        {
            if (Core.Me.ClassLevel > 35
                && Core.Me.ClassLevel < 50)
                return false;

            if (Core.Me.ClassLevel >= 60)
                return false;

            if (Core.Me.ClassLevel >= 50
                && Core.Me.ClassLevel < 60)
            {
                if (Casting.LastSpell == Spells.Fire3)
                    return await Spells.Fire2.Cast(Core.Me.CurrentTarget);

                if (Core.Me.CurrentMana >= 3800)
                    return await Spells.Fire2.Cast(Core.Me.CurrentTarget);

                return await Spells.Transpose.Cast(Core.Me);
            }
            return false;
        }

        public static async Task<bool> Blizzard2()
        {
            if (Core.Me.ClassLevel > 35)
                return false;

            if (ActionResourceManager.BlackMage.UmbralStacks > 0)
            {
                if (Core.Me.CurrentMana < 10000)
                {
                    return await Spells.Blizzard2.Cast(Core.Me);
                }
                return await Spells.Transpose.Cast(Core.Me);
            }

            return false;
        }
        public static async Task<bool> Fire4()
        {
            if (Core.Me.ClassLevel < 60)
                return false;

            if (Core.Me.ClassLevel > 67)
                return false;

            if (Casting.LastSpell == Spells.Fire3)
                return await Spells.Fire4.Cast(Core.Me.CurrentTarget);

            if (Core.Me.CurrentMana >= 2400
                && ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds >= 6000)
                return await Spells.Fire4.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> Fire3()
        {
            if (Core.Me.ClassLevel < 50)
                return false;

            if (Core.Me.ClassLevel > 71)
                return false;

            if (Casting.LastSpell == Spells.Thunder2
                || Casting.LastSpell == Spells.Thunder4)
                if (ActionResourceManager.BlackMage.UmbralStacks > 0)
                    return await Spells.Fire3.Cast(Core.Me.CurrentTarget);
            return false;
        }
    }
}


