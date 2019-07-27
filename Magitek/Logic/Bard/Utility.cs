using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Bard;
using Magitek.Utilities;

namespace Magitek.Logic.Bard
{
    internal static class Utility
    {

        public static async Task<bool> Troubadour()
        {

            if (!BardSettings.Instance.ForceTroubadour)
                return false;

            if (!await Spells.Troubadour.Cast(Core.Me)) return false;
            BardSettings.Instance.ForceTroubadour = false;
            return true;

        }

        public static async Task<bool> HeadGraze()
        {

            if (!BardSettings.Instance.UseHeadGraze)
                return false;

            BattleCharacter interruptTarget = null;

            if (BardSettings.Instance.OnlyInterruptCurrentTarget)
                interruptTarget = Combat.Enemies.FirstOrDefault(r => r.InView() && r == Core.Me.CurrentTarget && r.IsCasting && r.SpellCastInfo.Interruptible);
            else
                interruptTarget = Combat.Enemies.FirstOrDefault(r => r.InView() && r.IsCasting && r.SpellCastInfo.Interruptible);

            if (interruptTarget == null)
                return false;

            return await Spells.HeadGraze.Cast(interruptTarget);

        }

    }
}