using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Roles;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Roles
{
    internal static class MagicDps
    {
        public static async Task<bool> Interrupt(MagicDpsSettings settings)
        {
            if (!settings.UseStunOrInterrupt)
                return false;

            List<SpellData> stuns = new List<SpellData>();
            List<SpellData> interrupts = new List<SpellData>();

            if (Core.Me.IsBlueMage())
            {
                interrupts.Add(Spells.FlyingSardine);
            }

            return await InterruptAndStunLogic.StunOrInterrupt(stuns, interrupts, settings.Strategy);
        }

        public static async Task<bool> Recuperate<T>(T settings) where T : MagicDpsSettings
        {
            if (!settings.Pvp_UseRecuperate)
                return false;

            if (!Spells.Recuperate.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentHealthPercent > settings.Pvp_RecuperateHealthPercent)
                return false;

            return await Spells.Recuperate.Cast(Core.Me);
        }

        public static async Task<bool> Purify<T>(T settings) where T : MagicDpsSettings
        {
            if (!settings.Pvp_UsePurify)
                return false;

            if (!Spells.Purify.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.HasAura("Stun") && !Core.Me.HasAura("Heavy") && !Core.Me.HasAura("Bind") && !Core.Me.HasAura("Silence") && !Core.Me.HasAura("Half-asleep") && !Core.Me.HasAura("Sleep") && !Core.Me.HasAura("Deep Freeze"))
                return false;

            return await Spells.Purify.Cast(Core.Me);
        }

        public static async Task<bool> Guard<T>(T settings) where T : MagicDpsSettings
        {
            if (!settings.Pvp_UseGuard)
                return false;

            if (!Spells.Guard.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentHealthPercent > settings.Pvp_GuardHealthPercent)
                return false;

            return await Spells.Guard.Cast(Core.Me);
        }

    }
}
