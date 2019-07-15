using System;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;

namespace Magitek.Logic.Summoner
{
    internal static class Pets
    {
        public static async Task<bool> Summon()
        {
            if (Core.Me.ClassLevel < 4)
                return false;

            // Don't try to summon if we're mounted or moving
            if (Core.Me.IsMounted || MovementManager.IsMoving)
                return false;

            if (Core.Me.Pet != null) return false;

            //TODO: Add logic to summon pet based on Selected Pet Setting

            return await Spells.Summon.Cast(Core.Me);
        }

        public static async Task<bool> SummonBahamut()
        {
            if (Core.Me.ClassLevel < 70) return false;

            if (Core.Me.Pet == null) return false;

            if (ActionResourceManager.Arcanist.AetherAttunement < 2) return false;

            return await Spells.SummonBahamut.Cast(Core.Me);
        }
    }
}