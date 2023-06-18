using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Magitek.Logic.Ninja
{
    internal static class Aoe
    {
        
        public static async Task<bool> HellfrogMedium()
        {

            if (Core.Me.ClassLevel < 62)
                return false;

            if (!Spells.HellfrogMedium.IsKnown())
                return false;

            //dumping ninki before mug is missung
            if (MagitekActionResourceManager.Ninja.NinkiGauge < 90)
                return false;

            //Smart Target Logic needs to be addded
            return await Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PhantomKamaitachi()
        {

            if (Core.Me.ClassLevel < 82)
                return false;

            if (!Spells.PhantomKamaitachi.IsKnown())
                return false;

            if (ActionResourceManager.Ninja.HutonTimer.Add(new TimeSpan(0, 0, 10)) > new TimeSpan(0, 1, 0))
                return false;

            if (!Core.Me.HasMyAura(Auras.PhantomKamaitachiReady) && Casting.SpellCastHistory.Count() > 0 && Casting.SpellCastHistory.First().Spell != Spells.Bunshin)
                return false;

            return await Spells.PhantomKamaitachi.Cast(Core.Me.CurrentTarget);

        }

    }
}
