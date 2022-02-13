using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Gunbreaker;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Gunbreaker
{
    internal static class Heal
    {
        public static async Task<bool> Aurora()
        {
            if (!GunbreakerSettings.Instance.UseAurora)
                return false;

            if (Spells.Aurora.Charges < 1)
                return false;

            var auroraTargets = Group.CastableAlliesWithin30.Where(CanGetAurora).OrderBy(AuroraPriority).ToList();

            if (auroraTargets == null)
                return false;

            if (!await Spells.Aurora.Cast(auroraTargets.FirstOrDefault()))
                return false;

            Logger.WriteInfo($@"Aurora On {auroraTargets.FirstOrDefault().CurrentJob} - {auroraTargets.FirstOrDefault().Name} with HP = {auroraTargets.FirstOrDefault().CurrentHealthPercent}");
            return true;

            bool CanGetAurora(Character unit)
            {
                if (unit == null)
                    return false;

                if (!unit.IsAlive)
                    return false;

                if (unit.Distance(Core.Me) > 30)
                    return false;

                if (unit.HasAura(Auras.Aurora))
                    return false;

                if (unit.IsMe && GunbreakerSettings.Instance.UseAuroraSelf && unit.CurrentHealthPercent < GunbreakerSettings.Instance.AuroraSelfHealthPercent)
                    return true;

                if (unit.IsMainTank() && GunbreakerSettings.Instance.UseAuroraMainTank && unit.CurrentHealthPercent < GunbreakerSettings.Instance.AuroraMainTankHealthPercent)
                    return true;

                if (unit.IsTank() && GunbreakerSettings.Instance.UseAuroraTank && unit.CurrentHealthPercent < GunbreakerSettings.Instance.AuroraTankHealthPercent)
                    return true; 
                
                if (unit.IsHealer() && GunbreakerSettings.Instance.UseAuroraHealer && unit.CurrentHealthPercent < GunbreakerSettings.Instance.AuroraHealerHealthPercent)
                    return true;

                if (unit.IsDps() && GunbreakerSettings.Instance.UseAuroraDps && unit.CurrentHealthPercent < GunbreakerSettings.Instance.AuroraDpsHealthPercent)
                    return true;

                return false;
            }

            int AuroraPriority(Character unit)
            {
                if (unit.IsMe)
                    return GunbreakerSettings.Instance.AuroraPrioritySelf; 
                if (unit.IsMainTank())
                    return GunbreakerSettings.Instance.AuroraPriorityMainTank;
                if (unit.IsTank())
                    return GunbreakerSettings.Instance.AuroraPriorityTank;
                if (unit.IsHealer())
                    return GunbreakerSettings.Instance.AuroraPriorityHealer;
                if (unit.IsDps())
                    return GunbreakerSettings.Instance.AuroraPriorityDps;

                return 100;
            }
        }
    }
}
