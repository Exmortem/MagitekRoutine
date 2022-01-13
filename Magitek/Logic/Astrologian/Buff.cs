using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Astrologian
{
    internal static class Buff
    {
        public static async Task<bool> Swiftcast()
        {
            if (await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast))
            {
                return await Coroutine.Wait(15000, () => Core.Me.HasAura(Auras.Swiftcast, true, 7000));
            }

            return false;
        }

        public static async Task<bool> LucidDreaming()
        {
            return await Roles.Healer.LucidDreaming(AstrologianSettings.Instance.LucidDreaming, AstrologianSettings.Instance.LucidDreamingManaPercent);
        }

        public static async Task<bool> Lightspeed()
        {
            if (!AstrologianSettings.Instance.Lightspeed)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Combat.CombatTotalTimeLeft <= 25)
                return false;

            if (Spells.Lightspeed.Cooldown == TimeSpan.Zero)
                return false;

            if (Spells.EssentialDignity.Charges > 0)
                return false;

            if (Globals.InParty)
            {
                if (Group.CastableTanks.Any(r => r.CurrentHealthPercent >= 40))
                    return false;

                if (Spells.Horoscope.Cooldown == TimeSpan.Zero)
                    return false;

                if (Spells.CelestialOpposition.Cooldown == TimeSpan.Zero)
                    return false;

                if (Core.Me.HasAura(Auras.LadyOfCrownsDrawn))
                    return false;

                var partySize = PartyManager.NumMembers;
                var lightSpeedThreshold = partySize == 4 ? 2 : 3;

                if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= 60f) < lightSpeedThreshold)
                    return false;
            }

            if (Core.Me.CurrentHealthPercent >= 40f)
                return false;

            return await Spells.Lightspeed.CastAura(Core.Me, Auras.Lightspeed);
        }

        public static async Task<bool> Divination()
        {
            if (!AstrologianSettings.Instance.Play || !AstrologianSettings.Instance.Divination)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.Divination.Cooldown != TimeSpan.Zero)
                return false;

            if (Combat.CombatTotalTimeLeft <= AstrologianSettings.Instance.DontPlayWhenCombatTimeIsLessThan)
                return false;

            if (!Combat.Enemies.All(x => x.IsTargetable))
                return false;

            // Added check to see if more than configured allies are around

            var divinationTargets = Group.CastableAlliesWithin15.Count(r => r.IsAlive);

            if (divinationTargets >= AstrologianSettings.Instance.DivinationAllies)
                return await Spells.Divination.CastAura(Core.Me, Auras.Divination);

            return false;
        }

        public static async Task<bool> Synastry()
        {
            if (!AstrologianSettings.Instance.Synastry)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Casting.LastSpell == Spells.Synastry)
                return false;

            if (Core.Me.HasAura(Auras.SynastrySource))
                return false;

            if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= AstrologianSettings.Instance.SynastryHealthPercent) < AstrologianSettings.Instance.SynastryAmountOfPeople)
                return false;

            GameObject target = AstrologianSettings.Instance.SynastryTankOnly
                ? Group.CastableTanks.FirstOrDefault(r => r.CurrentHealthPercent <= AstrologianSettings.Instance.SynastryHealthPercent
                && r.IsTank())
                : Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent <= AstrologianSettings.Instance.SynastryHealthPercent);

            if (target == null)
                return false;

            return await Spells.Synastry.CastAura(target, Auras.SynastryDestination);
        }

        public static async Task<bool> NeutralSect()
        {
            if (!AstrologianSettings.Instance.NeutralSect)
                return false;

            var neutral = Group.CastableAlliesWithin15.Count(r => r.CurrentHealth > 0
            && r.CurrentHealthPercent <= AstrologianSettings.Instance.NeutralSectHealthPercent);

            if (neutral < AstrologianSettings.Instance.NeutralSectAllies)
                return false;

            return await Spells.NeutralSect.Cast(Core.Me);
        }


    }
}