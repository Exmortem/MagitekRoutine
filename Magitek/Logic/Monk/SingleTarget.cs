using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Monk;
using Magitek.Utilities;
using System.Threading.Tasks;
using ff14bot.Objects;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Monk
{
    public class SingleTarget
    {
        public static async Task<bool> Bootshine()
        {
            if (Core.Me.ClassLevel < 6)
                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

            if (!Core.Me.HasAura(Auras.OpoOpoForm))
                return false;

            if (!Core.Me.HasAura(Auras.LeadenFist) && Core.Me.ClassLevel >= 50)
                return false;

            return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TrueStrike()
        {
            if (Core.Me.ClassLevel < 4)
                return false;

            if (!Core.Me.HasAura(Auras.RaptorForm))
                return false;

            if (!Core.Me.HasAura(Auras.TwinSnakes) && Core.Me.ClassLevel >= 18)
                return false;

            return await Spells.TrueStrike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SnapPunch()
        {

            if (Core.Me.ClassLevel < 6)
                return false;

            if (!Core.Me.HasAura(Auras.CoeurlForm))
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.Demolish) && Core.Me.ClassLevel >= 30)
                return false;

            return await Spells.SnapPunch.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TwinSnakes()
        {

            if (Core.Me.ClassLevel < 18)
                return false;

            if (!Core.Me.HasAura(Auras.RaptorForm))
                return false;

            if (Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
                return false;

            return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DragonKick()
        {

            if (Core.Me.ClassLevel < 50)
                return false;

            if (!Core.Me.HasAura(Auras.OpoOpoForm))
                return false;

            if (Core.Me.HasAura(Auras.LeadenFist, true, MonkSettings.Instance.DragonKickRefresh * 1000))
                return false;

            return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Demolish()
        {

            if (Core.Me.ClassLevel < 30)
                return false;

            if (!Core.Me.HasAura(Auras.CoeurlForm))
                return false;

            if (MonkSettings.Instance.DemolishUseTtd && Core.Me.CurrentTarget.CombatTimeLeft() <= MonkSettings.Instance.DemolishMinimumTtd)
                return await Spells.SnapPunch.Cast(Core.Me.CurrentTarget);

            if (Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000))
                return false;

            return await Spells.Demolish.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShoulderTackle()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseShoulderTackle)
                return false;

            if (Core.Player.HasAura(Auras.Brotherhood) || Core.Player.HasAura(Auras.FistsofFire))
                return await Spells.ShoulderTackle.Cast(Core.Me.CurrentTarget);
            if (Spells.ShoulderTackle.Cooldown.TotalMilliseconds < 1000)
                return await Spells.ShoulderTackle.Cast(Core.Me.CurrentTarget);
            return false;

        }
        public static async Task<bool> TheForbiddenChakra()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseTheForbiddenChakra)
                return false;

            if (ActionResourceManager.Monk.FithChakra < 5)
                return false;

            return await Spells.TheForbiddenChakra.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TornadoKick() {
            // Off GCD

            if (!MonkSettings.Instance.UseTornadoKick)
                return false;

            if (Core.Me.ClassLevel < 60 || !ActionManager.HasSpell(Spells.TornadoKick.Id))
                return false;

            return await Spells.TornadoKick.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ElixirField()
        {
            // Off GCD

            if (!MonkSettings.Instance.UseElixerField)
                return false;

            //var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 25 && r.InCombat);
            //var cosCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach);

            //var canCoS = cosCount >= enemyCount || cosCount > 2;

            //if (!canCoS)
            //    return false;

            if (Core.Me.CurrentTarget is BattleCharacter target && !target.WithinSpellRange(5))
                return false;

            if (Spells.ElixirField.Cooldown.Seconds != 0)
                return false;

            return await Spells.ElixirField.Cast(Core.Me);
        }

        public static async Task<bool> PerfectBalanceRoT()
        {
            if (!Core.Me.HasAura(Auras.PerfectBalance))
                return false;
            
            if (!Core.Me.HasAura(Auras.TwinSnakes, true, MonkSettings.Instance.TwinSnakesRefresh * 1000) && Casting.LastSpell != Spells.TwinSnakes)
                return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);

            if (!Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000) && Casting.LastSpell != Spells.Demolish)
                return await Spells.Demolish.Cast(Core.Me.CurrentTarget);

            if (Core.Me.HasAura(Auras.LeadenFist))
                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

            if(!ActionManager.HasSpell(Spells.DragonKick.Id))
                return await Spells.SnapPunch.Cast(Core.Me.CurrentTarget);

            return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);
        }

    }
}
