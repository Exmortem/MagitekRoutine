using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Astrologian
{
    internal static class Pvp
    {
        #region Damage

        public static async Task<bool> Disable()
        {
            if (!AstrologianSettings.Instance.Disable)
                return false;

            if (!Core.Me.HasTarget)
                return false;

            var target = (Character)Core.Me.CurrentTarget;

            if (!target.ValidAttackUnit())
                return false;

            if (!target.HasTarget)
                return false;

            if (!target.IsCasting)
                return false;

            if (Group.CastableAlliesWithin30.All(r => r != target.TargetCharacter))
                return false;

            return await Spells.PvpDisable.Cast(target);
        }

        public static async Task<bool> Malefic()
        {
            if (!AstrologianSettings.Instance.PvpMalefic)
                return false;

            if (!Core.Me.HasTarget)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit())
                return false;

            return await Spells.PvpMalefic3.Cast(Core.Me.CurrentTarget);
        }

        #endregion

        #region Heals

        public static async Task<bool> EssentialDignity()
        {
            if (!AstrologianSettings.Instance.PvpEssentialDignity)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (AstrologianSettings.Instance.PvpEssentialDignityTankOnly)
            {
                var tar = Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontEssentialDignity.Contains(r.Name) && r.IsAlive && r.CurrentHealthPercent <= AstrologianSettings.Instance.PvpEssentialDignityHealthPercent);

                if (tar == null)
                    return false;

                return await Spells.PvpEssentialDignity.Heal(tar, false);
            }

            var essentialdignitytarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontEssentialDignity.Contains(r.Name) && r.CurrentHealth > 0 && r.CurrentHealthPercent <= AstrologianSettings.Instance.PvpEssentialDignityHealthPercent);

            if (essentialdignitytarget == null)
                return false;

            return await Spells.PvpEssentialDignity.Heal(essentialdignitytarget);
        }

        public static async Task<bool> Benefic2()
        {
            if (!AstrologianSettings.Instance.PvpBenefic2)
                return false;

            var shouldBenefic2WithAbridged = AstrologianSettings.Instance.Benefic2AlwaysWithAbridged &&
                                             Core.Me.CurrentManaPercent >= Spells.PvpBenefic2.Cost;

            if (Core.Me.HasAura(Auras.Abridged) && Globals.HealTarget?.CurrentHealthPercent <= AstrologianSettings.Instance.PvpBeneficHealthPercent && shouldBenefic2WithAbridged)
            {
                return await Spells.PvpBenefic2.Heal(Globals.HealTarget);
            }

            var benefic2Target = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Astrologian.DontBenefic2.Contains(r.Name) && r.CurrentHealth > 0 && r.CurrentHealthPercent <= AstrologianSettings.Instance.PvpBenefic2HealthPercent);

            if (benefic2Target == null)
                return false;

            return await Spells.PvpBenefic2.Heal(benefic2Target);
        }

        public static async Task<bool> Benefic()
        {
            if (!AstrologianSettings.Instance.PvpBenefic)
                return false;

            foreach (var ally in Group.CastableAlliesWithin30)
            {
                //Logging.Write("Castable Allies Within 30: {0}", Group.CastableAlliesWithin30.Count);

                if (Utilities.Routines.Astrologian.DontBenefic.Contains(ally.Name))
                    continue;

                if (ally.CurrentHealthPercent > AstrologianSettings.Instance.PvpBeneficHealthPercent || ally.CurrentHealth <= 0)
                    continue;

                return await Spells.PvpBenefic.Heal(ally);
            }

            return false;
        }

        #endregion

        #region Dispel (Purify)

        public static async Task<bool> Purify()
        {
            if (!AstrologianSettings.Instance.Purify || !ActionManager.HasSpell(Spells.Purify.Id))
                return false;

            if (Globals.InParty)
            {
                return await CheckParty();
            }

            return await CheckSelf();
        }

        private static async Task<bool> CheckParty()
        {
            // First we look for High Priority Dispels
            var dispelTarget = Group.CastableAlliesWithin30.Where(a => a.NeedsDispel(true)).OrderByDescending(DispelManager.GetWeight).FirstOrDefault();

            if (dispelTarget != null)
            {
                return await Spells.Purify.Cast(dispelTarget);
            }

            if (!AstrologianSettings.Instance.AutomaticallyPurifyAnythingThatsDispellable)
                return false;

            // Check to see if we need to heal people before we Dispel anyone
            if (AstrologianSettings.Instance.PurifyOnlyAbove && Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < AstrologianSettings.Instance.PurifyOnlyAboveHealth))
                return false;

            dispelTarget = Group.CastableAlliesWithin30.Where(a => a.HasAnyDispellableAura()).OrderByDescending(DispelManager.GetWeight).FirstOrDefault();

            if (dispelTarget == null)
                return false;

            return await Spells.Purify.Cast(dispelTarget);
        }

        private static async Task<bool> CheckSelf()
        {
            if (Core.Me.NeedsDispel(true))
            {
                return await Spells.Purify.Cast(Core.Me);
            }

            if (!AstrologianSettings.Instance.AutomaticallyPurifyAnythingThatsDispellable)
                return false;

            if (Core.Me.CurrentHealthPercent < AstrologianSettings.Instance.PurifyOnlyAboveHealth)
                return false;

            if (!Core.Me.HasAnyDispellableAura())
                return false;

            return await Spells.Purify.Cast(Core.Me);
        }

        #endregion

        #region Buffs

        public static async Task<bool> Lightspeed()
        {
            if (!AstrologianSettings.Instance.PvpLightspeed)
                return false;

            if (!Core.Me.InCombat)
                return false;

            //Add if !CardDrawn check to return false

            if (AstrologianSettings.Instance.PvpLightspeedTankOnly)
            {
                if (Group.CastableTanks.Any(r => r.CurrentHealthPercent <=
                                                 AstrologianSettings.Instance.PvpLightspeedHealthPercent))
                    return false;

                return await Spells.PvpLightspeed.CastAura(Core.Me, Auras.PvpLightspeed);
            }
            else
            {
                if (!Group.CastableAlliesWithin30.Any(
                    r => r.CurrentHealthPercent <= AstrologianSettings.Instance.PvpLightspeedHealthPercent))
                    return false;

                return await Spells.PvpLightspeed.CastAura(Core.Me, Auras.PvpLightspeed);
            }
        }

        public static async Task<bool> Synastry()
        {
            if (!AstrologianSettings.Instance.PvpSynastry)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Casting.LastSpell == Spells.PvpSynastry)
                return false;

            if (Core.Me.HasAura(Auras.PvpSynastrySource)) return false;

            GameObject target = AstrologianSettings.Instance.PvpSynastryTankOnly
                ? Group.CastableTanks.FirstOrDefault(
                    r => r.CurrentHealthPercent <= AstrologianSettings.Instance.PvpSynastryHealthPercent && r.IsTank())
                : Group.CastableTanks.FirstOrDefault(
                    r => r.CurrentHealthPercent <= AstrologianSettings.Instance.PvpSynastryHealthPercent);

            if (target == null)
                return false;

            return await Spells.PvpSynastry.CastAura(target, Auras.PvpSynastryDestination);
        }

        #endregion

        #region Cards

        public static Task<bool> Draw()
        {
            return Task.FromResult(false);
        }

        public static Task<bool> Play()
        {
            return Task.FromResult(false);
        }
        #endregion

        #region Utility

        public static async Task<bool> Deorbit()
        {
            if (!AstrologianSettings.Instance.Deorbit)
                return false;

            if (!Core.Me.InCombat)
                return false;

            //TODO: Do this better...
            var deorbittargets =
                Group.CastableAlliesWithin30.Where(r => PartyManager.VisibleMembers.Any(a => a.ObjectId == r.ObjectId) && r.CurrentHealthPercent <=
                                                        AstrologianSettings.Instance.DeorbitHealthPercent);

            if (AstrologianSettings.Instance.DeborbitOnlyIfTargetedByHostile)
                deorbittargets = deorbittargets.Where(a => Combat.Enemies.Any(b => b.TargetGameObject == a));

            var deborbittarget = deorbittargets.OrderByDescending(r => r.CurrentHealthPercent).FirstOrDefault();

            return await Spells.Deorbit.Cast(deborbittarget);

        }

        #endregion

        #region Common PVP Abilities

        public static async Task<bool> Muse()
        {
            if (!AstrologianSettings.Instance.Muse)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.CurrentManaPercent > AstrologianSettings.Instance.MuseManaPercent)
                return false;

            return await Spells.Muse.Cast(Core.Me);
        }

        public static async Task<bool> EmpyreanRain()
        {
            if (!AstrologianSettings.Instance.EmpyreanRain)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!ActionManager.CanCast(Spells.EmpyreanRain, Core.Me))
                return false;

            if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= AstrologianSettings.Instance.EmpyreanRainHealthPercent && r.CurrentHealth > 0) <= AstrologianSettings.Instance.EmpyreanRainAllies)
                return false;

            return await Spells.EmpyreanRain.Cast(Core.Me);
        }

        public static async Task<bool> Recuperate()
        {
            if (!AstrologianSettings.Instance.Recuperate)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.RecuperateHealthPercent)
                return false;

            return await Spells.Recuperate.Cast(Core.Me);
        }

        public static async Task<bool> Concentrate()
        {
            if (!AstrologianSettings.Instance.Concentrate)
                return false;

            if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.ConcentrateHealthPercent)
                return false;

            var enemiestargetingme = Combat.Enemies.Where(r => r.CurrentTargetId == Core.Me.ObjectId);

            if (enemiestargetingme.Count() < AstrologianSettings.Instance.ConcentrateEnemiesTargeting)
                return false;

            if (enemiestargetingme.All(r => !r.IsCasting))
                return false;

            return await Spells.Concentrate.Cast(Core.Me);
        }

        public static async Task<bool> Safeguard()
        {
            if (!AstrologianSettings.Instance.Safeguard)
                return false;

            if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.SafeguardHealthPercent)
                return false;

            var enemiestargetingme = Combat.Enemies.Where(r => r.CurrentTargetId == Core.Me.ObjectId);

            if (enemiestargetingme.Count() < AstrologianSettings.Instance.SafeguardEnemiesTargeting)
                return false;

            if (enemiestargetingme.All(r => !r.IsCasting))
                return false;

            return await Spells.Safeguard.Cast(Core.Me);
        }


        #endregion
    }
}