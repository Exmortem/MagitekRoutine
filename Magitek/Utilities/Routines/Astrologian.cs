using Clio.Utilities;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Astrologian
    {
        public static bool OnGcd => Spells.Malefic.Cooldown.TotalMilliseconds > 100;

        public static HashSet<string> DontBenefic = new HashSet<string>();
        public static HashSet<string> DontBenefic2 = new HashSet<string>();
        public static HashSet<string> DontDiurnalBenefic = new HashSet<string>();
        public static HashSet<string> DontNocturnalBenefic = new HashSet<string>();
        public static HashSet<string> DontEssentialDignity = new HashSet<string>();
        public static HashSet<string> DontCelestialIntersection = new HashSet<string>();

        public static List<Character> AllianceBeneficOnly = new List<Character>();

        public static bool NeedToInterruptCast()
        {
            /*if (Casting.CastingTankBuster)
                return false;*/

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (Casting.CastingSpell != Spells.Ascend && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Resurrection: Unit Died");
                return true;
            }

            if (Casting.CastingSpell == Spells.Ascend && Casting.SpellTarget?.HasAura(Auras.Raise) == true)
            {
                Logger.Error($@"Stopped Resurrection: Unit has raise aura");
                return true;
            }

            if (AstrologianSettings.Instance.InterruptHealing && Casting.DoHealthChecks &&
                Casting.SpellTarget?.CurrentHealthPercent >= AstrologianSettings.Instance.InterruptHealingHealthPercent)
            {
                if (Casting.CastingSpell == Spells.Helios && PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r =>
                        r.CurrentHealth > 0 && r.Distance(Core.Me) <= Spells.Helios.Radius && r.CurrentHealthPercent <=
                        AstrologianSettings.Instance.HeliosHealthPercent) <
                    AstrologianSettings.Instance.HeliosAllies)
                {
                    Logger.Error($@"Stopped Healing: Party's Health Too High");
                    return true;
                }
                if (Casting.CastingSpell == Spells.AspectedHelios)
                {
                    if (PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r =>
                            r.CurrentHealth > 0 &&
                            r.Distance(Core.Me) <= Spells.AspectedHelios.Radius &&
                            r.CurrentHealthPercent <=
                            AstrologianSettings.Instance.DiurnalHeliosHealthPercent &&
                            !r.HasAura(Auras.AspectedHelios, true)) < AstrologianSettings.Instance.DiurnalHeliosAllies)
                    {
                        Logger.Error($@"Stopped Healing: Party's Health Too High");
                        return true;
                    }
                }
                else
                {
                    Logger.Error($@"Stopped Healing: Target's Health Too High");
                    return true;
                }
            }

            if (AstrologianSettings.Instance.InterruptDamageToHeal && !Core.Me.HasAura(1495))
            {
                if (Casting.CastingSpell == Spells.Malefic || Casting.CastingSpell == Spells.Malefic2 ||
                    Casting.CastingSpell == Spells.Malefic3 || Casting.CastingSpell == Spells.PvpMalefic3 ||
                    Casting.CastingSpell == Spells.Gravity)
                {

                    var lowestHealthToInterruptList = new[]
                    {
                        AstrologianSettings.Instance.Benefic ? AstrologianSettings.Instance.BeneficHealthPercent -10 : 0,
                        AstrologianSettings.Instance.Benefic2 ? AstrologianSettings.Instance.Benefic2HealthPercent -10 : 0
                    };

                    var lowestHealthToInterrupt = lowestHealthToInterruptList.Max();

                    if (PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Any(r => r.CurrentHealth > 0 &&
                        r.CurrentHealthPercent <= lowestHealthToInterrupt && r.Distance() < 30 && r.InLineOfSight()))
                    {
                        Logger.Error($@"Stopping Cast: Need To Heal Someone In The Party");
                        return true;
                    }
                }
            }

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            return false;
        }

        public static void GroupExtension()
        {
            Group.UpdateAlliance(
                AstrologianSettings.Instance.IgnoreAlliance,
                AstrologianSettings.Instance.HealAllianceDps,
                AstrologianSettings.Instance.HealAllianceHealers,
                AstrologianSettings.Instance.HealAllianceTanks,
                AstrologianSettings.Instance.ResAllianceDps,
                AstrologianSettings.Instance.ResAllianceHealers,
                AstrologianSettings.Instance.ResAllianceTanks
            );
        }

        public static readonly uint[] ShieldAuraList = {
            Auras.NocturnalField,
            Auras.Galvanize
        };

        public static Vector3 EarthlyStarLocation { get; set; }

    }
}
