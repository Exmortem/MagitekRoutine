using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;
using Auras = Magitek.Utilities.Auras;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Bard
{
    internal static class Pvp
    {
        public static async Task<bool> PowerfulShot()
        {

            if(!Spells.PowerfulShotPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if(Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.PowerfulShotPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ApexArrow()
        {
            if (!Spells.ApexArrowPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.ApexArrowPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BlastArrow()
        {
            if (!Spells.BlastArrowPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.BlastArrowPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SilentNocturnePvp()
        {
            if (!Spells.SilentNocturnePvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.SilentNocturnePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EmpyrealArrow()
        {
            if (!BardSettings.Instance.Pvp_UseEmpyrealArrow)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 25)
                return false;

            if (BardSettings.Instance.Pvp_UseEmpyrealArrowCharges == 3)
            {
                if (!Spells.EmpyrealArrowIIIPvp.CanCast())
                    return false;

                return await Spells.EmpyrealArrowIIIPvp.Cast(Core.Me.CurrentTarget);
            }

            if (BardSettings.Instance.Pvp_UseEmpyrealArrowCharges == 2)
            {
                if (!Spells.EmpyrealArrowIIPvp.CanCast())
                    return false;

                return await Spells.EmpyrealArrowIIPvp.Cast(Core.Me.CurrentTarget);
            }

            if (!Spells.EmpyrealArrowPvp.CanCast())
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.EmpyrealArrowPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FinalFantasiaPvp()
        {

            if (!Spells.FinalFantasiaPvp.CanCast())
                return false;

            if (!BardSettings.Instance.Pvp_UseFinalFantasia)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Group.CastableAlliesWithin30.Count(AlliesInRange) < BardSettings.Instance.Pvp_UseFinalFantasiaAlliesCount)
                return false;

            return await Spells.FinalFantasiaPvp.Cast(Core.Me);

            bool AlliesInRange(GameObject unit)
            {
                if (unit.Distance(Core.Me) > 30)
                    return false;

                return true;
            }
        }

    }
}
