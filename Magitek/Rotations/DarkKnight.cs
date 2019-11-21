using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.DarkKnight;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;

namespace Magitek.Rotations
{
    public static class DarkKnight
    {
        public static async Task<bool> Rest()
        {
            return false;
        }

        public static async Task<bool> PreCombatBuff()
        {
            

            if (Core.Me.IsCasting)
                return true;

            await Casting.CheckForSuccessfulCast();

            Utilities.Routines.DarkKnight.PullUnleash = 0;
            return await Buff.Grit();
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);
            }

            return await Combat();
        }
        public static async Task<bool> Heal()
        {
            if (Core.Me.IsMounted)
                return true;

            if (await GambitLogic.Gambit()) return true;
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            return false;
        }
        public static async Task<bool> CombatBuff()
        {
            return false;
        }
        public static async Task<bool> Combat()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (await Defensive.ExecuteTankBusters()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);
            }

            if (await Buff.Grit()) return true;
            if (await Tank.Interrupt(DarkKnightSettings.Instance)) return true;

            if (Weaving.GetCurrentWeavingCounter() < 2 && Spells.HardSlash.Cooldown.TotalMilliseconds > 650 + BaseSettings.Instance.UserLatencyOffset)
            {
                if (await Tank.Provoke(DarkKnightSettings.Instance)) return true;
                if (await Defensive.Execute()) return true;
                if (await Defensive.TheBlackestNight()) return true;
                if (await SingleTarget.Reprisal()) return true;
                if (await SingleTarget.CarveAndSpit()) return true;
                if (await Aoe.SaltedEarth()) return true;
                if (await Aoe.AbyssalDrain()) return true;
                if (await Aoe.FloodofDarknessShadow()) return true;
                if (await SingleTarget.EdgeofDarknessShadow()) return true;
                if (await SingleTarget.Plunge()) return true;
                if (await Buff.Delirium()) return true;
                if (await Buff.BloodWeapon()) return true;
                if (await Buff.LivingShadow()) return true;
            }

            if (await SingleTarget.Unmend()) return true;
            if (await Aoe.Quietus()) return true;
            if (await Aoe.StalwartSoul()) return true;
            if (await Aoe.Unleash()) return true;

            if (await SingleTarget.Bloodspiller()) return true;
            if (await SingleTarget.SoulEater()) return true;
            if (await SingleTarget.SyphonStrike()) return true;
            return await SingleTarget.HardSlash();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
