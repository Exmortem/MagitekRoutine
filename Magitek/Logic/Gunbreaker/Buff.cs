using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Gunbreaker;
using GunbreakerRoutine = Magitek.Utilities.Routines.Gunbreaker;

namespace Magitek.Logic.Gunbreaker
{
    internal static class Buff
    {
        public static async Task<bool> RoyalGuard() //Tank stance
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

        public static async Task<bool> NoMercy() // Damage Buff +20%
        {
            if (!GunbreakerSettings.Instance.UseNoMercy)
                return false;

            //Force Delay CD
            if (Spells.KeenEdge.Cooldown.TotalMilliseconds > Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset + 100)
                return false;

            return await Spells.NoMercy.Cast(Core.Me);
        }

        public static async Task<bool> Bloodfest() // +2 or +3 cartrige
        {
            if (!GunbreakerSettings.Instance.UseBloodfest)
                return false;

            if (Cartridge > GunbreakerRoutine.MaxCartridge - GunbreakerRoutine.AmountCartridgeFromBloodfest)
                return false;

            return await Spells.Bloodfest.Cast(Core.Me.CurrentTarget);
        }
    }
}