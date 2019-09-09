using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using static ff14bot.Managers.ActionResourceManager.Gunbreaker;

namespace Magitek.Logic.Gunbreaker
{
    internal static class Buff
    {
        public static async Task<bool> RoyalGuard()
        {
            switch (GunbreakerSettings.Instance.UseRoyalGuard)
            {
                case true:
                    if (!Core.Me.HasAura(Auras.RoyalGuard))
                        return await Spells.RoyalGuard.CastAura(Core.Me, Auras.RoyalGuard);
                    break;

                case false:
                    if (Core.Me.HasAura(Auras.RoyalGuard))
                        return await Spells.RoyalGuard.Cast(Core.Me);
                    break;
            }
            return false;
        }

        public static async Task<bool> NoMercy()
        {
            if (!GunbreakerSettings.Instance.UseNoMercy)
                return false;
            //Use on last end of GCD
            if (Spells.KeenEdge.Cooldown.TotalMilliseconds > 850)
                return false;

            return await Spells.NoMercy.Cast(Core.Me);

        
        }

        public static async Task<bool> Bloodfest()
        {
            if (Cartridge != 0)
                return false;

            if (!GunbreakerSettings.Instance.UseBloodfest)
                return false;

            if (Spells.KeenEdge.Cooldown.TotalMilliseconds < 850)
                return false;

            return await Spells.Bloodfest.Cast(Core.Me.CurrentTarget);
        }
    }
}