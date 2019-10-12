using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dragoon;
using Magitek.Utilities;

namespace Magitek.Logic.Dragoon
{
    internal static class Jumps
    {
        public static async Task<bool> Execute()
        {
            if (!DragoonSettings.Instance.UseJumps)
                return false;

            if (DragoonSettings.Instance.SafeJumpLogic)
            {
                if (!Core.Me.CurrentTarget.InView())
                    return false;
            }

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (Utilities.Routines.Dragoon.Jumps.Contains(Casting.LastSpell))
                return false;

            if (Core.Me.ClassLevel >= 54 && Combat.CombatTime.Elapsed.Seconds < 20 && ActionResourceManager.Dragoon.Timer.Seconds == 0)
                return false;

            if (await Jump()) return true;
            if (await Stardiver()) return true;
            if (await SpineshatterDive()) return true;
            return await DragonfireDive();
        }

        public static async Task<bool> MirageDive()
        {
            if (!DragoonSettings.Instance.MirageDive)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            if (!Core.Me.HasAura(Auras.DiveReady))
                return false;

            // Neko: 2 things that can help. Don't mirage dive unless in Blood of the Dragon, also, don't mirage dive if at 2 eyes.

            if (ActionResourceManager.Dragoon.DragonGaze == 2)
                return false;

            if (await Spells.MirageDive.Cast(Core.Me.CurrentTarget))
            {
                Utilities.Routines.Dragoon.MirageDives++;

                if (DragoonSettings.Instance.Geirskogul)
                {
                    if (Spells.Geirskogul.Cooldown.TotalMilliseconds < 1000)
                    {
                        await Coroutine.Wait(3000, () => ActionManager.CanCast(Spells.Geirskogul.Id, Core.Me.CurrentTarget));
                        return await Spells.Geirskogul.Cast(Core.Me.CurrentTarget);
                    }
                }
            }

            return false;
        }

        public static async Task<bool> Jump()
        {
            if (!DragoonSettings.Instance.Jump)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            return await Spells.Jump.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SpineshatterDive()
        {
            if (!DragoonSettings.Instance.SpineshatterDive)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            return await Spells.SpineshatterDive.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DragonfireDive()
        {
            if (!DragoonSettings.Instance.DragonfireDive)
                return false;

            if (RoutineManager.IsAnyDisallowed(CapabilityFlags.Movement))
                return false;

            return await Spells.DragonfireDive.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Stardiver()
        {
            if (!DragoonSettings.Instance.Stardiver)
                return false;

            return await Spells.Stardiver.Cast(Core.Me.CurrentTarget);
        }
    }
}
