using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Paladin
{
    internal static class SingleTarget
    {
        public static async Task<bool> ShieldLob()
        {
            if (PaladinSettings.Instance.ShieldLobToPullExtraEnemies && !BotManager.Current.IsAutonomous)
            {
                var pullTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit() && !r.Tapped && r.Distance(Core.Me) < 15 + r.CombatReach &&
                                                                                r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.TargetGameObject != Core.Me);

                if (pullTarget != null)
                {
                    return await Spells.ShieldLob.Cast(pullTarget);
                }
            }

            if (!PaladinSettings.Instance.UseShieldLob)
                return false;

            return await Spells.ShieldLob.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShieldLobLostAggro()
        {
            if (Globals.OnPvpMap)
                return false;

            if (!PaladinSettings.Instance.ShieldLobLostAggro)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            var shieldLobTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) > 5 + r.CombatReach && r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.Distance(Core.Me) <= 15 + r.CombatReach && r.TargetGameObject != Core.Me);

            if (shieldLobTarget == null)
                return false;

            if (shieldLobTarget.TargetGameObject == null)
                return false;

            if (!await Spells.ShieldLob.Cast(shieldLobTarget))
                return false;

            Logger.Write($@"[Magitek] Shield Lob On {shieldLobTarget.Name} To Pull Aggro");
            return true;
        }

        public static async Task<bool> SpiritsWithin()
        {
            if (!PaladinSettings.Instance.SpiritsWithin)
                return false;

            if (Core.Me.CurrentHealthPercent < PaladinSettings.Instance.SpiritsWithinOnlyAboveHealth)
                return false;

            if (Casting.LastSpell == Spells.FightorFlight)
                return false;

            if (Spells.FightorFlight.Cooldown.Seconds <= 8 && !Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 8000))
            {
                //Right we want to check if we want to hold CoS.
                if (Casting.LastSpell == Spells.FastBlade)
                    return false;

                if (Casting.LastSpell == Spells.RiotBlade)
                    return false;

                if (Casting.LastSpell == Spells.Confiteor)
                    return false;

                return await Spells.SpiritsWithin.Cast(Core.Me.CurrentTarget);
            }

            return await Spells.SpiritsWithin.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Requiescat()
        {
            if (!PaladinSettings.Instance.Requiescat)
                return false;

            if (Core.Me.CurrentMana < 8000)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true,
                1900) || Core.Me.HasAuraCharge(Auras.SwordOath))
                return false;

            if (PaladinSettings.Instance.FoFFirst && Spells.FightorFlight.Cooldown.Seconds < 8 && !Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 10000))
                return false;

            if (Core.Me.HasAura(Auras.FightOrFight, true, 3000))
                return false;

            return await Spells.Requiescat.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HolySpirit()
        {
            if (!PaladinSettings.Instance.HolySpirit)
                return false;

            if (Core.Me.ClassLevel < 64)
                return false;

            if (!PaladinSettings.Instance.AlwaysHolySpiritWithBuff)
                return false;

            if (!Core.Me.HasAura(Auras.Requiescat))
                return false;

            if (Core.Me.ClassLevel >= 80)
            {

                if (Core.Me.CurrentMana <= 3999)
                    return false;
            }

            return await Spells.HolySpirit.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Intervene()
        {
            if (Core.Me.ClassLevel < 74)
                return false;

            if (!PaladinSettings.Instance.Intervene)
                return false;

            if (Core.Me.HasAura(Auras.SwordOath) && Core.Me.HasAura(Auras.FightOrFight))
            {
                // We only want to use this during Sword Oath as a OGCD, one we go into this phase and once after using Atonement
                if (Casting.LastSpell == Spells.RoyalAuthority)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);

                if (Casting.LastSpell == Spells.CircleofScorn)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);

                if (Casting.LastSpell == Spells.Atonement)
                    return await Spells.Intervene.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> Atonement()
        {
            if (Core.Me.ClassLevel < 76 || !Core.Me.HasAura(Auras.SwordOath))
                return false;

            //not in FoF Check
            if (Core.Me.HasAuraCharge(Auras.SwordOath) && !Core.Me.HasAura(Auras.FightOrFight) && Spells.FightorFlight.Cooldown.TotalMilliseconds < 12000)
                return false;

            //we are in FoF Check
            if (Core.Me.HasAuraCharge(Auras.SwordOath) && !Core.Me.HasAura(Auras.FightOrFight, true, 7000) && Core.Me.HasAura(Auras.FightOrFight))
                return false;

            //are we mid combo?
            if (ActionManager.LastSpell == Spells.FastBlade || ActionManager.LastSpell == Spells.RiotBlade)
                return false;

            return await Spells.Atonement.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Interrupt()
        {
            List<SpellData> extraStun = new List<SpellData>();

            if (PaladinSettings.Instance.ShieldBash)
            {
                extraStun.Add(Spells.ShieldBash);
            }

            return await Tank.Interrupt(PaladinSettings.Instance, extraStuns: extraStun);
        }
    }
}