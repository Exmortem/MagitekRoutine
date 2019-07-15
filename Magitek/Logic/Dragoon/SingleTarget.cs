using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dragoon;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Dragoon
{
    internal static class SingleTarget
    {
        public static async Task<bool> TrueThrust()
        {
            if (Core.Me.HasAura(Auras.RaidenReady))
                return await Spells.RaidenThrust.Cast(Core.Me.CurrentTarget);

            return await Spells.TrueThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> VorpalThrust()
        {
            if (ActionManager.LastSpell != Spells.TrueThrust && ActionManager.LastSpell != Spells.RaidenThrust)
                return false;

            return await Spells.VorpalThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Disembowel()
        {
            if (ActionManager.LastSpell != Spells.TrueThrust && ActionManager.LastSpell != Spells.RaidenThrust)
                return false;

            if (Core.Me.HasAura(Auras.Disembowel, true, DragoonSettings.Instance.DisembowelRefreshSeconds * 1000))
                return false;

            return await Spells.Disembowel.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FullThrust()
        {
            if (ActionManager.LastSpell != Spells.VorpalThrust)
                return false;

            if (Spells.LifeSurge.Cooldown == TimeSpan.Zero)
            {
                if (await Spells.LifeSurge.Cast(Core.Me))
                    await Coroutine.Wait(2000, () => ActionManager.CanCast(Spells.FullThrust.Id, Core.Me.CurrentTarget));
            }

            return await Spells.FullThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ChaosThrust()
        {
            // Should automatically refresh after Disembowel
            if (ActionManager.LastSpell != Spells.Disembowel)
                return false;

            return await Spells.ChaosThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WheelingThrust()
        {
            if (!Core.Me.HasAura(Auras.EnhancedWheelingThrust))
                return false;

            return await Spells.WheelingThrust.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FangAndClaw()
        {
            if (!Core.Me.HasAura(Auras.SharperFangandClaw))
                return false;

            return await Spells.FangAndClaw.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Geirskogul()
        {
            if (!DragoonSettings.Instance.Geirskogul)
                return false;

            // Second Geirskogul should only happen after the second Mirage Dive
            if (Utilities.Routines.Dragoon.MirageDives == 1)
                return false;
            
            if (ActionResourceManager.Dragoon.Timer.TotalMilliseconds > 28000)
                return await Spells.Geirskogul.Cast(Core.Me.CurrentTarget);

            if (Casting.LastSpell != Spells.MirageDive)
                return false;

            return await Spells.Geirskogul.Cast(Core.Me.CurrentTarget);
        }
    }
}
