using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Paladin
{
    internal static class Buff
    {
        public static async Task<bool> Oath()
        {
            if (PaladinSettings.Instance.IronWill && !Core.Me.HasAura(Auras.IronWill))
            {
                if (PaladinSettings.Instance.OathHotSwapMode == true)
                {
                    PaladinSettings.Instance.UseDefensives = PaladinSettings.Instance.SwordDefensive;
                    PaladinSettings.Instance.UseClemency = PaladinSettings.Instance.SwordClemency;
                    PaladinSettings.Instance.UseProvoke = PaladinSettings.Instance.SwordProvoke;
                    PaladinSettings.Instance.ShieldBash = PaladinSettings.Instance.SwordShieldBash;
                    PaladinSettings.Instance.UseCover = PaladinSettings.Instance.SwordCover;
                    PaladinSettings.Instance.ShieldLobToPullExtraEnemies = PaladinSettings.Instance.SwordPullExtra;
                    PaladinSettings.Instance.ShieldLobLostAggro = PaladinSettings.Instance.SwordPullExtra;
                    PaladinSettings.Instance.Requiescat = PaladinSettings.Instance.SwordRequiecast;
                    PaladinSettings.Instance.Strategy = PaladinSettings.Instance.SwordStrategy;
                }

                return await Spells.IronWill.Cast(Core.Me);
            }


            if (!PaladinSettings.Instance.IronWill && Core.Me.HasAura(Auras.IronWill))
            {
                if (PaladinSettings.Instance.OathHotSwapMode == true)
                {
                    PaladinSettings.Instance.UseDefensives = PaladinSettings.Instance.ShieldDefensive;
                    PaladinSettings.Instance.UseClemency = PaladinSettings.Instance.ShieldClemency;
                    PaladinSettings.Instance.UseProvoke = PaladinSettings.Instance.ShieldProvoke;
                    PaladinSettings.Instance.ShieldBash = PaladinSettings.Instance.ShieldShieldBash;
                    PaladinSettings.Instance.UseCover = PaladinSettings.Instance.ShieldCover;
                    PaladinSettings.Instance.ShieldLobToPullExtraEnemies = PaladinSettings.Instance.ShieldPullExtra;
                    PaladinSettings.Instance.ShieldLobLostAggro = PaladinSettings.Instance.ShieldPullExtra;
                    PaladinSettings.Instance.Requiescat = PaladinSettings.Instance.ShieldRequiecast;
                    PaladinSettings.Instance.Strategy = PaladinSettings.Instance.ShieldStrategy;
                }

                return await Spells.IronWill.Cast(Core.Me);
            }


            return false;

        }


        public static async Task<bool> FightOrFlight()
        {
            if (!PaladinSettings.Instance.UseFightOrFlight)
                return false;

            if (Core.Me.ClassLevel >= 68 && !PaladinSettings.Instance.FoFFirst)
            {
                if (Spells.Requiescat.Cooldown.Seconds < 10)
                    return false;
            }

            if (Core.Me.HasAura(Auras.Requiescat))
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 13000))
                return false;

            if (Core.Me.ClassLevel == 80)
            {
                if (PaladinSettings.Instance.FoFFastBlade)
                {
                    if (ActionManager.LastSpell != Spells.FastBlade)
                        return false;

                    if (!Core.Me.HasAuraCharge(Auras.SwordOath))
                        return false;

                    if (Spells.FastBlade.Cooldown.TotalMilliseconds > (650 + BaseSettings.Instance.UserLatencyOffset))
                        return false;

                    return await Spells.FightorFlight.Cast(Core.Me);
                }
                else
                {
                    if (ActionManager.LastSpell != Spells.RiotBlade)
                        return false;

                    if (!Core.Me.HasAuraCharge(Auras.SwordOath))
                        return false;

                    if (Spells.FastBlade.Cooldown.TotalMilliseconds > (650 + BaseSettings.Instance.UserLatencyOffset))
                        return false;

                    return await Spells.FightorFlight.Cast(Core.Me);
                }
            }
            else
            {
                if (PaladinSettings.Instance.FoFFastBlade)
                {
                    if (ActionManager.LastSpell != Spells.FastBlade)
                        return false;

                    if (Spells.FastBlade.Cooldown.TotalMilliseconds > (650 + BaseSettings.Instance.UserLatencyOffset))
                        return false;

                    return await Spells.FightorFlight.Cast(Core.Me);
                }
                else
                {
                    if (ActionManager.LastSpell != Spells.RiotBlade)
                        return false;

                    if (Spells.FastBlade.Cooldown.TotalMilliseconds > (650 + BaseSettings.Instance.UserLatencyOffset))
                        return false;

                    return await Spells.FightorFlight.Cast(Core.Me);
                }
            }


        }

        public static async Task<bool> DivineVeil()
        {
            if (!PaladinSettings.Instance.DivineVeil)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Group.CastableAlliesWithin15.Any(r => r.CurrentHealthPercent < PaladinSettings.Instance.DivineVeilHp))
                return false;

            if (!Group.CastableAlliesWithin30.Any(r => r.IsHealer()))
                return false;

            return await Spells.DivineVeil.Cast(Core.Me);
        }

        public static async Task<bool> Sheltron()
        {
            if (!PaladinSettings.Instance.Sheltron)
                return false;

            if (ActionResourceManager.Paladin.Oath < 50)
                return false;

            if (Core.Me.CurrentHealthPercent > PaladinSettings.Instance.SheltronHp)
                return false;

            if (!ActionManager.HasSpell(Spells.Sheltron.Id))
                return false;

            if (!ActionManager.CanCast(Spells.Sheltron.Id, Core.Me))
                return false;

            if (!Core.Me.HasTarget)
                return false;

            var targetAsCharacter = Core.Me.CurrentTarget as Character;

            if (targetAsCharacter == null)
                return false;

            if (!targetAsCharacter.HasTarget)
                return false;

            if (targetAsCharacter.TargetGameObject != Core.Me)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5 + Core.Me.CurrentTarget.CombatReach)
                return false;

            return await Spells.Sheltron.Cast(Core.Me);
        }

        public static async Task<bool> Cover()
        {
            if (!PaladinSettings.Instance.UseCover)
                return false;

            if (!Globals.InParty)
                return false;

            if (Spells.Cover.Cooldown > TimeSpan.Zero)
                return false;

            if (PaladinSettings.Instance.UseCoverHealer)
            {
                GameObject coverTarget = Group.CastableAlliesWithin10.FirstOrDefault(r => r.IsHealer() && r.CurrentHealthPercent < PaladinSettings.Instance.UseCoverHealerHp);

                if (coverTarget != null)
                {
                    return await Spells.Cover.Cast(coverTarget);
                }
            }

            if (!PaladinSettings.Instance.UseCoverDps)
                return false;

            var coverDpsTarget = Group.CastableAlliesWithin10.FirstOrDefault(r => r.IsDps() && r.CurrentHealthPercent < PaladinSettings.Instance.UseCoverDpsHp);

            if (coverDpsTarget == null)
                return false;

            return await Spells.Cover.Cast(coverDpsTarget);
        }

        public static async Task<bool> Intervention()
        {
            if (!PaladinSettings.Instance.InterventionOnNearbyPartyMember)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Spells.Intervention.Cooldown > TimeSpan.Zero)
                return false;

            if (PaladinSettings.Instance.InterventionPartyAlwaysWithCD)
            {
                if (Core.Me.HasAura(Auras.Sentinel) || Core.Me.HasAura(Auras.Rampart))
                {
                    var interventionTarget = Group.CastableAlliesWithin30.OrderBy(r => r.CurrentHealthPercent).FirstOrDefault();

                    if (interventionTarget != null)
                    {
                        return await Spells.Intervention.Cast(interventionTarget);
                    }
                }

            }

            if (PaladinSettings.Instance.InterventionPartyAlwaysWOCD)
            {
                if (!Core.Me.HasAura(Auras.Sentinel) && !Core.Me.HasAura(Auras.Rampart))
                    return false;
            }

            var interventionTarget2 = Group.CastableAlliesWithin30.OrderBy(r => r.CurrentHealthPercent).FirstOrDefault(r => r.CurrentHealthPercent <= PaladinSettings.Instance.InterventionOnNearbyPartyMemberHealth);

            if (interventionTarget2 == null)
                return false;

            return await Spells.Intervention.Cast(interventionTarget2);
        }
    }
}