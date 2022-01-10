using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Roles;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Roles
{
    public class Healer
    {
        public static async Task<bool> Swiftcast()
        {
            if (await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast))
            {
                return await Coroutine.Wait(15000, () => Core.Me.HasAura(Auras.Swiftcast, true, 7000));
            }

            return false;
        }

        public static async Task<bool> Raise(
            SpellData spell,
            bool SwiftcastRes,
            bool SlowcastRes,
            bool ResOutOfCombat,
            // isSwiftcast, continue
            Func<bool, Task<bool>> extensions = null
        )
        {
            if (!Globals.InParty)
                return false;

            if (!spell.IsKnown())
                return false;

            if (Core.Me.CurrentMana < spell.Cost)
                return false;

            var deadList = Group.DeadAllies.Where(u => u.CurrentHealth == 0 &&
                                                       !u.HasAura(Auras.Raise) &&
                                                       u.Distance(Core.Me) <= 30 &&
                                                       u.IsVisible &&
                                                       u.InLineOfSight() &&
                                                       u.IsTargetable)
                .OrderByDescending(r => r.GetResurrectionWeight());

            var deadTarget = deadList.FirstOrDefault();

            if (deadTarget == null)
                return false;

            if (Globals.PartyInCombat)
            {
                if (SwiftcastRes && Spells.Swiftcast.IsKnownAndReady())
                {
                    // Prevent burning switftcast if no mana to actually rez.
                    if (!ActionManager.CanCast(spell, deadTarget))
                        return false;

                    if (await Swiftcast())
                    {
                        while (Core.Me.HasAura(Auras.Swiftcast))
                        {
                            if (extensions != null && !await extensions.Invoke(true)) return false;
                            if (await spell.CastAura(deadTarget, Auras.Raise))
                                return true;
                            await Coroutine.Yield();
                        }
                    }
                }
            }

            if (Globals.PartyInCombat && SlowcastRes || !Globals.PartyInCombat && ResOutOfCombat)
            {
                if (extensions != null && !await extensions.Invoke(false)) return false;
                return await spell.CastAura(deadTarget, Auras.Raise);
            }

            return false;
        }
    }
}