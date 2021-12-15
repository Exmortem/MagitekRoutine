using System.Linq;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Reaper;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Reaper;
using Magitek.Utilities;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Reaper
    {
        public static async Task<bool> Rest()
        {
            return Core.Me.CurrentHealthPercent < ReaperSettings.Instance.RestHealthPercent;
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (Core.Me.IsCasting)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            //Openers.OpenerCheck();

            if (Core.Me.HasTarget && Core.Me.CurrentTarget.CanAttack)
                return false;


            if (Globals.OnPvpMap)
                return false;

            return false;

            //return await PhysicalDps.Peloton(BardSettings.Instance);
        }

        public static async Task<bool> Pull()
        {
            Utilities.Routines.Bard.RefreshVars();

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            return await GambitLogic.Gambit();
        }

        public static async Task<bool> CombatBuff()
        {
            return false;
        }

        public static async Task<bool> Combat()
        {
            if (Core.Me.IsCasting)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Utilities.Routines.Reaper.RefreshVars();

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (Weaving.GetCurrentWeavingCounter() < 2 && Spells.Slice.Cooldown.TotalMilliseconds >
                650 + BaseSettings.Instance.UserLatencyOffset)
            {
                if (await Cooldown.Enshroud()) return true;
                if (await SingleTarget.LemuresSlice()) return true;
                if (await Cooldown.Gluttony()) return true;
                if (await SingleTarget.BloodStalk()) return true;
            }

            if (await AoE.Communio()) return true;
            if (await SingleTarget.VoidAndCrossReaping()) return true;
            if (await AoE.WhorlofDeath()) return true;
            if (await SingleTarget.ShadowOfDeath()) return true;
            if (await SingleTarget.GibbetAndGallows()) return true;
            if (await SingleTarget.SoulSlice()) return true;
            
            if (await AoE.NightmareScythe()) return true;
            if (await SingleTarget.InfernalSlice()) return true;
            if (await SingleTarget.WaxingSlice()) return true;
            if (await AoE.SpinningScythe()) return true;
            return await SingleTarget.Slice();
        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}


