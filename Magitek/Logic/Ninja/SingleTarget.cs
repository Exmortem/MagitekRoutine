using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using static ff14bot.Managers.ActionResourceManager.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class SingleTarget
    {
        public static async Task<bool> SpinningEdge()
        {
            return await Spells.SpinningEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GustSlash()
        {
            if (ActionManager.LastSpell != Spells.SpinningEdge)
                return false;

            return await Spells.GustSlash.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AeolianEdge()
        {
            if (ActionManager.LastSpell != Spells.GustSlash)
                return false;
            
            return await Spells.AeolianEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ArmorCrush()
        {
            if (ActionManager.LastSpell != Spells.GustSlash)
                return false;

            if(Core.Me.CurrentTarget.IsFlanking)
                return await Spells.ArmorCrush.Cast(Core.Me.CurrentTarget);

            if (HutonTimer.Seconds == 0)
                return false;

            if (HutonTimer.TotalMilliseconds > 30000)
                return false;

            return await Spells.ArmorCrush.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShadowFang()
        {
            if (!NinjaSettings.Instance.UseShadowFang)
                return false;

            if (ActionManager.LastSpell != Spells.SpinningEdge)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.ShadowFang, true, NinjaSettings.Instance.ShadowFangRefresh * 1000))
                return false;

            if (NinjaSettings.Instance.ShadowFangUseTtd)
            {
                if (Core.Me.CurrentTarget.CombatTimeLeft() < NinjaSettings.Instance.ShadowFangMinimumTtd)
                    return false;
            }
            else
            {
                if (!Core.Me.CurrentTarget.IsBoss())
                {
                    if (!Core.Me.CurrentTarget.HealthCheck(NinjaSettings.Instance.ShadowFangMinimumHealth, NinjaSettings.Instance.ShadowFangMinimumHealthPercent))
                        return false;
                }
            }

            return await Spells.ShadowFang.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Assassinate()
        {
            if (!NinjaSettings.Instance.UseAssassinate)
                return false;

            return await Spells.Assassinate.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Mug()
        {
            if (!NinjaSettings.Instance.UseMug)
                return false;

            if (NinkiGauge > 70 && Core.Me.ClassLevel > 65)
                return false;

            return await Spells.Mug.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TrickAttack()
        {
            if (!NinjaSettings.Instance.UseTrickAttack)
                return false;

            if (!Core.Me.HasAura(Auras.Suiton))
                return false;

            if (Core.Me.CurrentTarget.IsBehind || Core.Me.HasAura(Auras.TrueNorth) || !Core.Me.HasAura(Auras.Suiton, true, 3000))
                return await Spells.TrickAttack.Cast(Core.Me.CurrentTarget);
            
            if (!BotManager.Current.IsAutonomous)
                return false;

            return await Spells.TrickAttack.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DreamWithinADream()
        {
            if (!NinjaSettings.Instance.UseDreamWithinADream)
                return false;

            if (Spells.TrickAttack.Cooldown.Seconds > 10)
                return false;

            return await Spells.DreamWithinaDream.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Bhavacakra()
        {
            if (!NinjaSettings.Instance.UseBhavacakra)
                return false;

            return await Spells.Bhavacakra.Cast(Core.Me.CurrentTarget);
        }
    }
}
