using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using SageRoutine = Magitek.Utilities.Routines.Sage;

namespace Magitek.Logic.Sage
{
    internal static class Pvp
    {
        public static async Task<bool> DosisIIIPvp()
        {

            if (!Spells.DosisIIIPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.DosisIIIPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PhlegmaIIIPvp()
        {
            if (!Spells.PhlegmaIIIPvp.CanCast())
                return false;

            if (!SageSettings.Instance.Pvp_PhlegmaIII)
                return false;

            if (SageRoutine.AoeEnemies5Yards < 1)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.PhlegmaIIIPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ToxikonPvp()
        {
            if (!Spells.ToxikonPvp.CanCast())
                return false;

            if (!SageSettings.Instance.Pvp_Toxikon)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.ToxikonPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PneumaPvp()
        {
            if (!Spells.PneumaPvp.CanCast())
                return false;

            if (!SageSettings.Instance.Pvp_Pneuma)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.PneumaPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EukrasiaPvp()
        {
            if (!Spells.EukrasiaPvp.CanCast())
                return false;

            if (!SageSettings.Instance.Pvp_Pneuma)
                return false;

            if (Core.Me.HasAura(Auras.PvpEukrasias))
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.EukrasiaPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> KardiaPvp()
        {
            if (!Spells.KardiaPvp.CanCast())
                return false;

            if (!SageSettings.Instance.Pvp_Kardia)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.HasAura(Auras.PvpKardia, true))
                return false;

            var currentKardiaTarget = Group.CastableAlliesWithin30.Where(a => a.HasAura(Auras.PvpKardion, true)).FirstOrDefault();

            var kardiaTarget = Group.CastableAlliesWithin30.Where(CanKardia).OrderByDescending(KardiaPriority).FirstOrDefault();

            if (kardiaTarget == null)
                kardiaTarget = Core.Me;

            if (kardiaTarget == currentKardiaTarget)
                return false;

            return await Spells.KardiaPvp.CastAura(kardiaTarget, Auras.PvpKardion);

            bool CanKardia(Character unit)
            {
                if (unit == null)
                    return false;

                if (!unit.IsAlive)
                    return false;

                if (unit.Distance(Core.Me) > 30)
                    return false;

                return false;
            }

            int KardiaPriority(Character unit)
            {
                if (unit.IsTank())
                    return 100;
                if (unit.IsMeleeDps())
                    return 90;
                if (unit.IsRangedDps())
                    return 80;
                if (unit.IsHealer())
                    return 70;
                return 0;
            }
        }
    }
}
