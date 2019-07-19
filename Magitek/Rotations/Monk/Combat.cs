using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Monk;
using Magitek.Logic.Roles;
using Magitek.Models.Monk;
using Magitek.Utilities;

namespace Magitek.Rotations.Monk
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            if (await Buff.FistsOf()) return true;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            var count = Utilities.Combat.Enemies.Count;
            if (2 >= count && count < 5)
            {
                //TODO: Add 2-4 target DPS rotation
            }
            else if (count >= 5)
            {
                //TODO: Add 5+ target DPS rotation
            }

            if(Casting.LastSpell == Spells.PerfectBalance)
            {
                await SingleTarget.DragonKick();
            }
            if (Utilities.Routines.Monk.OnGcd)
            {
                if (await Buff.FistOfFire()) return true;
                if (await Buff.FistOfWind()) return true;
                if (await SingleTarget.TheForbiddenChakra()) return true;
                if (await SingleTarget.ShoulderTackle()) return true;
                //if (await Buff.PerfectBalance()) return true;
                if (await PhysicalDps.TrueNorth(MonkSettings.Instance)) return true;
                if (await Buff.RiddleOfFire()) return true;
                if (await Buff.RiddleOfEarth()) return true;
                if (await Buff.Brotherhood()) return true;
                if (await Aoe.ElixerField()) return true;
            }
            if (await Aoe.Rockbreaker()) return true;
            if (await Aoe.FourPointStrike()) return true;
            if (await SingleTarget.TwinSnakes()) return true;
            if (await SingleTarget.Demolish()) return true;
            if (await SingleTarget.SnapPunch()) return true;
            if (await SingleTarget.TrueStrike()) return true;
            if (await SingleTarget.Bootshine()) return true;
            if (await SingleTarget.DragonKick()) return true;
            return await Buff.FormShift();
        }
    }
}
