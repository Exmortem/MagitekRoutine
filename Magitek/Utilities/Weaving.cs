using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot.Objects;

namespace Magitek.Utilities
{
    class Weaving
    {

        public List<SpellData> OgcdActions;

        public Weaving(List<SpellData> jobsOgcdAction)
        {
            OgcdActions = jobsOgcdAction;
        }

        public int CheckLastSpellsForWeaving()
        {
            var weavingCounter = 0;

            if (Casting.SpellCastHistory.Count < 2)
                return 0;

            var lastSpellCast = Casting.SpellCastHistory.ElementAt(0).Spell;
            var secondLastSpellCast = Casting.SpellCastHistory.ElementAt(1).Spell;

            if (OgcdActions.Contains(lastSpellCast))
                weavingCounter += 1;
            else
                return 0;

            if (OgcdActions.Contains(secondLastSpellCast))
                weavingCounter += 1;

            return weavingCounter;
        }
    }
}