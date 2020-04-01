using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Toggles;
using Magitek.Utilities;

namespace Magitek.Logic.Summoner
{
    internal static class Heal
    {
        public static async Task<bool> Physick()
        {
            if (Core.Me.ClassLevel < 4) return false;

            //return await Spells.Physick.Heal(Core.Me);
            return await Task.FromResult(false);
        }

        public static async Task<bool> ForceRaise()
        {
            if (!SummonerSettings.Instance.ForceResuSwift)
                return false;

            if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                return false;

            if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                return false;

            if (!await ForceRaiseLogic()) return false;
            SummonerSettings.Instance.ForceResuSwift = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceHardRaise()
        {
            if (!SummonerSettings.Instance.ForceResu)
                return false;

            if (!await HardRaise()) return false;
            SummonerSettings.Instance.ForceResu = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> Raise()
        {
            if (!SummonerSettings.Instance.Resurrection)
                return false;

            if (!Globals.InParty)
                return false;

            if (Core.Me.CurrentMana < Spells.Resurrection.Cost)
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
                if (Core.Me.ClassLevel < 28)
                    return false;

                if (await Buff.Swiftcast())
                {
                    while (Core.Me.HasAura(Auras.Swiftcast))
                    {
                        if (await Spells.Resurrection.Cast(deadTarget)) return true;
                        await Coroutine.Yield();
                    }
                }
            }

            if (Core.Me.InCombat)
                return false;

            return await Spells.Resurrection.Cast(deadTarget);
        }

        public static async Task<bool> ForceRaiseLogic()
        {
            if (!Globals.InParty)
                return false;

            if (Core.Me.CurrentMana < Spells.Resurrection.Cost)
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
                if (Core.Me.ClassLevel < 28)
                    return false;

                if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                    return false;

                if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                    return false;

                if (await Buff.Swiftcast())
                {
                    while (Core.Me.HasAura(Auras.Swiftcast))
                    {
                        if (await Spells.Resurrection.Cast(deadTarget)) return true;
                        await Coroutine.Yield();
                    }
                }
            }

            if (Core.Me.InCombat)
                return false;

            return await Spells.Resurrection.Cast(deadTarget);
        }

        public static async Task<bool> HardRaise()
        {

            if (!Globals.InParty)
                return false;

            if (Core.Me.CurrentMana < Spells.Resurrection.Cost)
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

            return await Spells.Resurrection.Cast(deadTarget);
        }
    }
}