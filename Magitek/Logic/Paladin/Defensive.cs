using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Paladin
{
    internal static class Defensive
    {
        private static bool UseDefensives()
        {
            if (!PaladinSettings.Instance.UseDefensives)
                return false;

            if (Core.Me.HasAura(Auras.HallowedGround))
                return false;

            var currentAuras = Core.Me.CharacterAuras.Select(r => r.Id).Where(r => Utilities.Routines.Paladin.Defensives.Contains(r)).ToList();

            if (currentAuras.Count >= PaladinSettings.Instance.MaxDefensivesAtOnce)
            {
                if (currentAuras.Count >= PaladinSettings.Instance.MaxDefensivesUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= PaladinSettings.Instance.MoreDefensivesHp)
                    return false;
            }

            return true;
        }

         public static async Task<bool> Rampart()
        {
            if (!PaladinSettings.Instance.UseRampart)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > PaladinSettings.Instance.RampartHpPercentage)
                return false;

            return await Spells.Rampart.CastAura(Core.Me, Auras.Rampart);
        }

        public static async Task<bool> Sentinel()
        {
            if (!PaladinSettings.Instance.UseSentinel)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > PaladinSettings.Instance.SentinelHp)
                return false;

            return await Spells.Sentinel.CastAura(Core.Me, Auras.Sentinel);
        }

        public static async Task<bool> Reprisal()
        {
            if (!PaladinSettings.Instance.UseReprisal)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > PaladinSettings.Instance.ReprisalHealthPercent)
                return false;

            return await Spells.Reprisal.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> HallowedGround()
        {
            if (!PaladinSettings.Instance.UseHallowedGround)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > PaladinSettings.Instance.HallowedGroundHp)
                return false;

            return await Spells.HallowedGround.CastAura(Core.Me.CurrentTarget, Auras.HallowedGround);
        }

        public static async Task<bool> Sheltron()
        {
            if (!PaladinSettings.Instance.UseSheltron)
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

        public static async Task<bool> DivineVeil()
        {
            if (!PaladinSettings.Instance.UseDivineVeil)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < PaladinSettings.Instance.DivineVeilHp))
                return false;

            if (!Group.CastableAlliesWithin30.Any(r => r.IsHealer()))
                return false;

            return await Spells.DivineVeil.Cast(Core.Me);
        }

        public static async Task<bool> Bulwark()
        {
            if (!PaladinSettings.Instance.UseBulwark)
                return false;

            if (!UseDefensives())
                return false;

            if (Core.Me.CurrentHealthPercent > PaladinSettings.Instance.BulwarkHp)
                return false;

            return await Spells.Bulwark.Cast(Core.Me);
        }


        /* ***********************************************************************************************************
         *                                               COVER
         *************************************************************************************************************/
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

        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            return Tank.ForceLimitBreak(Spells.ShieldWall, Spells.Stronghold, Spells.LastBastion, Spells.FastBlade);
        }

    }
}