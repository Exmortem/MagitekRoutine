using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Bard;
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

            return await Spells.PowerfulShotPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ApexArrow()
        {
            if (!Spells.ApexArrowPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.ApexArrowPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BlastArrow()
        {
            if (!Spells.BlastArrowPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.BlastArrowPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EmpyrealArrow()
        {
            if (!BardSettings.Instance.UseEmpyrealArrowPVP)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (BardSettings.Instance.UseEmpyrealArrowCharges == 3)
            {
                if (!Spells.EmpyrealArrowIIIPvp.CanCast())
                    return false;

                return await Spells.EmpyrealArrowIIIPvp.Cast(Core.Me.CurrentTarget);
            }

            if (BardSettings.Instance.UseEmpyrealArrowCharges == 2)
            {
                if (!Spells.EmpyrealArrowIIPvp.CanCast())
                    return false;

                return await Spells.EmpyrealArrowIIPvp.Cast(Core.Me.CurrentTarget);
            }

            return await Spells.EmpyrealArrowPvp.Cast(Core.Me.CurrentTarget);
        }

    }
}
