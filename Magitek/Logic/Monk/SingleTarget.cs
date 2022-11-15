using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Monk;
using Magitek.Utilities;
using System.Threading.Tasks;
using System.Linq;
using Auras = Magitek.Utilities.Auras;
using static ff14bot.Managers.ActionResourceManager.Monk;

namespace Magitek.Logic.Monk
{
    public class SingleTarget
    {
        public static async Task<bool> Bootshine()
        {
            if (Core.Me.ClassLevel < 6)
                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

            if (!Core.Me.HasAura(Auras.OpoOpoForm) && Core.Me.ClassLevel >= 52)
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

            if (!Core.Me.HasAura(Auras.DisciplinedFist) && Core.Me.ClassLevel >= 18)
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

            if (Core.Me.HasAura(Auras.DisciplinedFist, true, MonkSettings.Instance.TwinSnakesRefresh * 1000))
                return false;

            return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DragonKick()
        {

            if (Core.Me.ClassLevel < 50)
                return false;

            if (!Core.Me.HasAura(Auras.OpoOpoForm) && !Core.Me.HasAura(Auras.FormlessFist))
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

        public static async Task<bool> TheForbiddenChakra()
        {
            if (Core.Me.ClassLevel < 15)
                return false;

            if (!MonkSettings.Instance.UseTheForbiddenChakra)
                return false;

            if (ActionResourceManager.Monk.ChakraCount < 5)
                return false;

            if (!Core.Me.HasAura(Auras.DisciplinedFist))
                return false;

            return await Spells.SteelPeak.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PerfectBalance()
        {
            if (Core.Me.ClassLevel < 50)
                return false;

            if (Core.Me.ClassLevel > 60)
                return false;

            if (!MonkSettings.Instance.UsePerfectBalance)
                return false;

            if (!Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) > 2)
            {
                if (!Core.Me.HasAura(Auras.DisciplinedFist, true, 6000) && Casting.LastSpell != Spells.FourPointFury)
                    return await Spells.FourPointFury.Cast(Core.Me);

                return await Spells.Rockbreaker.Cast(Core.Me);
            }
            else 
            { 
                if (!Core.Me.HasAura(Auras.LeadenFist) && Casting.LastSpell != Spells.DragonKick)
                    return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);

                return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);
            }
        }

            public static async Task<bool> PerfectBalanceRoT()
        {
            if (Core.Me.ClassLevel < 60)
                return false;

            if (!MonkSettings.Instance.UsePerfectBalance)
                return false;

            if (!Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if (ActionResourceManager.Monk.ActiveNadi.HasFlag(Nadi.Both) || (!ActionResourceManager.Monk.ActiveNadi.HasFlag(Nadi.Both) && ActionResourceManager.Monk.ActiveNadi.HasFlag(Nadi.Solar)))
            {

                if (!Core.Me.HasAura(Auras.FistsofWind) && Spells.RiddleofWind.IsKnownAndReady())
                    return await Spells.RiddleofWind.Cast(Core.Me);

                if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) > 2)
                {
                    if (!Core.Me.HasAura(Auras.DisciplinedFist, true, 6000) && Casting.LastSpell != Spells.FourPointFury)
                        return await Spells.FourPointFury.Cast(Core.Me);

                    return await Spells.Rockbreaker.Cast(Core.Me);
                }
                else
                {
  
                    if (!Core.Me.HasAura(Auras.DisciplinedFist, true, MonkSettings.Instance.TwinSnakesRefresh * 1000) && Casting.LastSpell != Spells.TwinSnakes)
                        return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);

                    if (!Core.Me.CurrentTarget.HasAura(Auras.Demolish, true, MonkSettings.Instance.DemolishRefresh * 1000) && Casting.LastSpell != Spells.Demolish)
                        return await Spells.Demolish.Cast(Core.Me.CurrentTarget);

                    if (!Core.Me.HasAura(Auras.LeadenFist) && Casting.LastSpell != Spells.DragonKick)
                        return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);

                    return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);
                }
            }

            return false;
        }

        public static async Task<bool> PerfectBalancePhoenix()
        {
            if (Core.Me.ClassLevel < 60)
                return false;

            if (!MonkSettings.Instance.UsePerfectBalance)
                return false;

            if (!Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if (!ActionResourceManager.Monk.ActiveNadi.HasFlag(Nadi.Lunar) || ActionResourceManager.Monk.ActiveNadi.HasFlag(Nadi.Both))
                return false;

            if (!Core.Me.HasAura(Auras.FistsofWind) && Spells.RiddleofWind.IsKnownAndReady())
                return await Spells.RiddleofWind.Cast(Core.Me);

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) > 2)
            {
                if (ActionResourceManager.Monk.MasterGaugeCount == 0)
                    return await Spells.FourPointFury.Cast(Core.Me);

                if (ActionResourceManager.Monk.MasterGaugeCount == 1)
                    return await Spells.Rockbreaker.Cast(Core.Me);

                if (ActionResourceManager.Monk.MasterGaugeCount == 2)
                    return await Spells.ArmOfTheDestroyer.Cast(Core.Me);
            }
            else
            {

                if (ActionResourceManager.Monk.MasterGaugeCount == 0)
                    return await Spells.TwinSnakes.Cast(Core.Me.CurrentTarget);

                if (ActionResourceManager.Monk.MasterGaugeCount == 1)
                    return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);

                if (ActionResourceManager.Monk.MasterGaugeCount == 2)
                    return await Spells.Demolish.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> PerfectBalanceElixir()
        {
            if (Core.Me.ClassLevel < 60)
                return false;

            if (!MonkSettings.Instance.UsePerfectBalance)
                return false;

            if (!Core.Me.HasAura(Auras.PerfectBalance))
                return false;

            if (ActionResourceManager.Monk.ActiveNadi.HasFlag(Nadi.Lunar) || ActionResourceManager.Monk.ActiveNadi.HasFlag(Nadi.Solar) || ActionResourceManager.Monk.ActiveNadi.HasFlag(Nadi.Both))
                return false;

            if (!Core.Me.HasAura(Auras.FistsofWind) && Spells.RiddleofWind.IsKnownAndReady())
                return await Spells.RiddleofWind.Cast(Core.Me);

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) > 2)
            {
                if (ActionResourceManager.Monk.MasterGaugeCount == 0)
                {
                    if((Core.Me.ClassLevel >= 82)){
                         return await Spells.ShadowOfTheDestroyer.Cast(Core.Me);
                    }
                    else
                    {
                        return await Spells.Rockbreaker.Cast(Core.Me);
                    }
                }

                if (ActionResourceManager.Monk.MasterGaugeCount == 1)
                {
                    if ((Core.Me.ClassLevel >= 82))
                    {
                        return await Spells.ShadowOfTheDestroyer.Cast(Core.Me);
                    }
                    else
                    {
                        return await Spells.Rockbreaker.Cast(Core.Me);
                    }
                }
                if (ActionResourceManager.Monk.MasterGaugeCount == 2)
                {
                    if ((Core.Me.ClassLevel >= 82))
                    {
                        return await Spells.ShadowOfTheDestroyer.Cast(Core.Me);
                    }
                    else
                    {
                        return await Spells.Rockbreaker.Cast(Core.Me);
                    }
                }
            }
            else
            {
                if (ActionResourceManager.Monk.MasterGaugeCount == 0)
                    return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);

                if (ActionResourceManager.Monk.MasterGaugeCount == 1)
                    return await Spells.Bootshine.Cast(Core.Me.CurrentTarget);

                if (ActionResourceManager.Monk.MasterGaugeCount == 2)
                    return await Spells.DragonKick.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            if (!Core.Me.HasTarget)
                return false;

            return PhysicalDps.ForceLimitBreak(Spells.Braver, Spells.Bladedance, Spells.FinalHeaven, Spells.Bootshine);
        }
    }
}
