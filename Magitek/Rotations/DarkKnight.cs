using ff14bot;
using ff14bot.Managers;
using Magitek.Logic;
using Magitek.Logic.DarkKnight;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;
using System.Threading.Tasks;
using DarkKnightRoutine = Magitek.Utilities.Routines.DarkKnight;

namespace Magitek.Rotations
{
    public static class DarkKnight
    {
        public static async Task<bool> PreCombatBuff()
        {
            await Casting.CheckForSuccessfulCast();

            if (WorldManager.InSanctuary)
                return false;

            return await Buff.Grit();
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat();
        }
        public static async Task<bool> Heal()
        {
            if (await GambitLogic.Gambit())
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return false;
        }

        public static async Task<bool> Combat()
        {

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (await CustomOpenerLogic.Opener())
                return true;

            if (await Buff.Grit())
                return true;

            if (await Tank.Interrupt(DarkKnightSettings.Instance))
                return true;

            if (DarkKnightRoutine.GlobalCooldown.CountOGCDs() < 2
                && Spells.HardSlash.Cooldown.TotalMilliseconds > 650 + BaseSettings.Instance.UserLatencyOffset)
            {
                if (await Tank.Provoke(DarkKnightSettings.Instance)) return true;
                if (await Defensive.Execute()) return true;
                if (await Defensive.Oblation(true)) return true;
                if (await Defensive.Reprisal()) return true;
                if (await SingleTarget.CarveAndSpit()) return true;
                if (await Aoe.SaltedEarth()) return true;
                if (await Aoe.AbyssalDrain()) return true;
                if (await Aoe.FloodofDarknessShadow()) return true;
                if (await SingleTarget.EdgeofDarknessShadow()) return true;
                if (await SingleTarget.Plunge()) return true;
                if (await Buff.Delirium()) return true;
                if (await Buff.BloodWeapon()) return true;
                if (await Buff.LivingShadow()) return true;
                if (await SingleTarget.Shadowbringer()) return true;
            }

            if (await SingleTarget.UnmendForAggro()) return true;
            if (await Aoe.Quietus()) return true;
            if (await Aoe.StalwartSoul()) return true;
            if (await Aoe.Unleash()) return true;

            if (await SingleTarget.Bloodspiller()) return true;
            if (await SingleTarget.SoulEater()) return true;
            if (await SingleTarget.SyphonStrike()) return true;
            if (await SingleTarget.HardSlash()) return true;
            if (await SingleTarget.Unmend()) return true;

            return false;
        }
        public static Task<bool> PvP() => Task.FromResult(false);
        public static Task<bool> Rest() => Task.FromResult(false);
        public static Task<bool> CombatBuff() => Task.FromResult(false);
    }
}
