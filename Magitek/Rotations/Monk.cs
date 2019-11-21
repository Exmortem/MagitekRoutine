using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Monk;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Monk;
using Magitek.Utilities;

namespace Magitek.Rotations
{
    public static class Monk
    {
        public static async Task<bool> Rest()
        {
            return false;
        }

        public static async Task<bool> PreCombatBuff()
        {
            

            await Casting.CheckForSuccessfulCast();

            if (await Buff.FistsOf()) return true;
            if (await Buff.Meditate()) return true;

            if (MonkSettings.Instance.UsePositionalToasts && Utilities.Routines.Monk.UseToast == 9)
            {
                Logger.Write($@"[Magitek] Initiated Toast for MNK");
                Thread T = new Thread(() => PositionalToast.PositionalLogic());
                Utilities.Routines.Monk.UseToast = 0;
                PositionalToast.SendToast("Toast Overlay Initiated", 5);
                T.Start();
            }

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 2 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat();
        }
        public static async Task<bool> Heal()
        {
            

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            if (await Buff.Mantra()) return true;

            return false;
        }
        public static async Task<bool> CombatBuff()
        {
            if (await Buff.FormShift()) return true;
            return await Buff.Meditate();
        }
        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            Utilities.Routines.Monk.RefreshVars();

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (await Buff.FistsOf()) return true;

            if (await Buff.Meditate()) return true;

            //var count = Utilities.Combat.Enemies.Count;
            //if (2 >= count && count < 5)
            //{
            //    //TODO: Add 2-4 target DPS rotation
            //}
            //else if (count >= 5)
            //{
            //    //TODO: Add 5+ target DPS rotation
            //}
            if (!Core.Me.HasAura(Auras.Anatman) || MonkSettings.Instance.UseManualPB && Core.Me.HasAura(Auras.PerfectBalance))
            {
                if (Weaving.GetCurrentWeavingCounter() < 2 && Spells.Bootshine.Cooldown.TotalMilliseconds > 750 + BaseSettings.Instance.UserLatencyOffset)
                {
                    if (await PhysicalDps.SecondWind(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.Bloodbath(MonkSettings.Instance)) return true;
                    if (await PhysicalDps.Feint(MonkSettings.Instance)) return true;
                    if (await Buff.Brotherhood()) return true;
                    if (await Aoe.Enlightenment()) return true;
                    if (await SingleTarget.TheForbiddenChakra()) return true;
                    if (await SingleTarget.ShoulderTackle()) return true;
                    if (await Buff.PerfectBalance()) return true;
                    if (!Core.Me.HasAura(Auras.RiddleOfEarth))
                    {
                        if (await PhysicalDps.TrueNorth(MonkSettings.Instance)) return true;
                    }
                    if (await Buff.RiddleOfFire()) return true;
                    if (await Buff.RiddleOfEarth()) return true;
                    if (await SingleTarget.ElixerField()) return true;
                }
                if (await SingleTarget.PerfectBalanceRoT()) return true;
                if (await Aoe.Rockbreaker()) return true;
                if (await Aoe.FourPointStrike()) return true;
                if (await Aoe.ArmOfDestroyer()) return true;
                if (await SingleTarget.Demolish()) return true;
                if (await SingleTarget.SnapPunch()) return true;
                if (await SingleTarget.TwinSnakes()) return true;
                if (await SingleTarget.TrueStrike()) return true;
                if (await SingleTarget.Bootshine()) return true;
                if (await SingleTarget.DragonKick()) return true;
                return await Buff.FormShift();
            }
            else
                return false;
    }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
