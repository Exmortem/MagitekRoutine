using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlueMage;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.BlueMage
{
    internal static class Heal
    {
        public static async Task<bool> SelfCure()
        {
            if (!BlueMageSettings.Instance.SelfCure)
                return false;

            if (Core.Me.CurrentHealthPercent > BlueMageSettings.Instance.SelfCureHealthPercent)
                return false;

            if (Core.Me.HasAura(Auras.AetherialMimicryHealer) && ActionManager.HasSpell(Spells.PomCure.Id))
                return await Spells.PomCure.Heal(Core.Me);

            if (!ActionManager.HasSpell(Spells.WhiteWind.Id))
                return false;
            
            if (Core.Me.CurrentMana < Spells.WhiteWind.Cost)
                return false;

            return await Spells.WhiteWind.Heal(Core.Me);
        }

        public static async Task<bool> AngelWhisper()
        {
            if (!BlueMageSettings.Instance.Raise)
                return false;

            if (!ActionManager.HasSpell(Spells.AngelWhisper.Id))
                return false;

            if (!Globals.InParty)
                return false;

            if (Core.Me.CurrentMana < Spells.AngelWhisper.Cost)
                return false;

            var deadList = Group.DeadAllies.Where(u => !u.HasAura(Auras.Raise) &&
                                                       u.Distance(Core.Me) <= 30 &&
                                                       u.InLineOfSight() &&
                                                       u.IsTargetable)
                                           .OrderByDescending(r => r.GetResurrectionWeight());

            var deadTarget = deadList.FirstOrDefault();

            if (deadTarget == null)
                return false;

            if (!deadTarget.IsVisible)
                return false;

            if (!deadTarget.IsTargetable)
                return false;

            if (Core.Me.InCombat || Globals.OnPvpMap)
            {
                if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                    return false;

                if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                    return false;

                if (await Buff.Swiftcast())
                {
                    while (Core.Me.HasAura(Auras.Swiftcast))
                    {
                        if (await Spells.AngelWhisper.HealAura(deadTarget, Auras.Raise)) return true;
                        await Coroutine.Yield();
                    }
                }
            }

            if (Core.Me.InCombat)
                return false;

            return await Spells.AngelWhisper.HealAura(deadTarget, Auras.Raise);
        }
    }
}
