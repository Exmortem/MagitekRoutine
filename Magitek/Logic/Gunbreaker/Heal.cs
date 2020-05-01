using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Gunbreaker
{
    internal static class Heal
    {
        public static async Task<bool> Aurora()
        {
            if (!GunbreakerSettings.Instance.UseAurora)
                return false;

            if (Casting.LastSpell == Spells.Aurora)
                return false;

            if (GunbreakerSettings.Instance.UseAuroraSelf)
            {
                if (Core.Me.CurrentHealthPercent < GunbreakerSettings.Instance.UseAuroraSelfHealthPercent)
                {
                    return await Spells.Aurora.Cast(Core.Me);
                }
            }

            if (!Globals.InParty)
                return false;

            if (Core.Me.CurrentManaPercent < GunbreakerSettings.Instance.MinMpAurora)
                return false;

            var anyHealers = Group.CastableAlliesWithin30.Any(r => r.IsHealer());

            if (GunbreakerSettings.Instance.UseAuroraHealer && anyHealers)
            {
                var healerTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.IsHealer() && r.CurrentHealthPercent < GunbreakerSettings.Instance.UseAuroraHealerHealthPercent);

                if (healerTarget != null)
                {
                    return await Spells.Aurora.Cast(healerTarget);
                }
            }

            var anyDps = Group.CastableAlliesWithin30.Any(r => r.IsDps());

            if (GunbreakerSettings.Instance.UseAuroraDps && anyDps)
            {
                var dpsTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.IsDps() && r.CurrentHealthPercent < GunbreakerSettings.Instance.UseAuroraDpsHealthPercent);

                if (dpsTarget != null)
                {
                    return await Spells.Aurora.Cast(dpsTarget);
                }
            }

            if (!GunbreakerSettings.Instance.UseAuroraSelf || anyHealers)
                return false;

            if (Core.Me.CurrentHealthPercent < GunbreakerSettings.Instance.UseAuroraSelfHealthPercent)
            {
                return await Spells.Aurora.Cast(Core.Me);
            }

            return false;
        }
    }
}
