using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Models.Sage;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Astrologian
{
    internal static class Pvp
    {
        public static async Task<bool> FallMaleficPvp()
        {

            if (!Spells.FallMaleficPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.FallMaleficPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DoubleFallMaleficPvp()
        {

            if (!Spells.DoubleFallMaleficPvp.CanCast())
                return false;

            if (!AstrologianSettings.Instance.Pvp_DoubleCastFallMalefic)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.DoubleFallMaleficPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GravityIIPvp()
        {

            if (!Spells.GravityIIPvp.CanCast())
                return false;

            if (!AstrologianSettings.Instance.Pvp_GravityII)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.GravityIIPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> DoubleGravityIIPvp()
        {

            if (!Spells.DoubleGravityIIPvp.CanCast())
                return false;

            if (!AstrologianSettings.Instance.Pvp_DoubleCastMaleficII)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.DoubleGravityIIPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AspectedBeneficPvp()
        {

            if (!Spells.AspectedBeneficPvp.CanCast())
                return false;

            if (!AstrologianSettings.Instance.Pvp_AspectedBenefic)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (AstrologianSettings.Instance.Pvp_HealSelfOnly)
            {
                if(Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.Pvp_AspectedBeneficHealthPercent)
                    return false;

                return await Spells.AspectedBeneficPvp.Heal(Core.Me);
            }

            if (Globals.HealTarget?.CurrentHealthPercent <= AstrologianSettings.Instance.Pvp_AspectedBeneficHealthPercent)
            {
                return await Spells.AspectedBeneficPvp.Heal(Globals.HealTarget);
            }

            var Target = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealth > 0 && r.CurrentHealthPercent <= AstrologianSettings.Instance.Pvp_AspectedBeneficHealthPercent);

            if (Target == null)
                if (Core.Me.CurrentHealthPercent <= AstrologianSettings.Instance.Pvp_AspectedBeneficHealthPercent)
                    return await Spells.AspectedBeneficPvp.Heal(Core.Me);

            return await Spells.AspectedBeneficPvp.Heal(Target);

        }

        public static async Task<bool> DoubleAspectedBeneficPvp()
        {

            if (!Spells.DoubleAspectedBeneficPvp.CanCast())
                return false;


            if (!AstrologianSettings.Instance.Pvp_DoubleCastAspectedBenefic)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (AstrologianSettings.Instance.Pvp_HealSelfOnly)
            {
                if (Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.Pvp_AspectedBeneficHealthPercent)
                    return false;

                return await Spells.DoubleAspectedBeneficPvp.Heal(Core.Me);
            }

            if (Globals.HealTarget?.CurrentHealthPercent <= AstrologianSettings.Instance.Pvp_AspectedBeneficHealthPercent)
            {
                return await Spells.DoubleAspectedBeneficPvp.Heal(Globals.HealTarget);
            }

            var Target = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealth > 0 && r.CurrentHealthPercent <= AstrologianSettings.Instance.Pvp_AspectedBeneficHealthPercent);

            if (Target == null)
                if (Core.Me.CurrentHealthPercent <= AstrologianSettings.Instance.Pvp_AspectedBeneficHealthPercent)
                    return await Spells.DoubleAspectedBeneficPvp.Heal(Core.Me);
                else
                    return false;

            return await Spells.DoubleAspectedBeneficPvp.Heal(Target);

        }

        public static async Task<bool> DrawPvp()
        {

            if (!Spells.DrawPvp.CanCast())
                return false;

            if (!AstrologianSettings.Instance.Pvp_Draw)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.DrawPvp.Cast(Core.Me);
        }

        public static async Task<bool> MacrocosmosPvp()
        {

            if (!Spells.MacrocosmosPvp.CanCast())
                return false;

            if (!AstrologianSettings.Instance.Pvp_Macrocosmos)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.HasAura(Auras.PvpMacrocosmos))
                return false;

            return await Spells.MacrocosmosPvp.Cast(Core.Me);
        }

        public static async Task<bool> MicrocosmosPvp()
        {

            if (!Spells.MicrocosmosPvp.CanCast())
                return false;

            if (!AstrologianSettings.Instance.Pvp_Microcosmos)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if(Core.Me.CurrentHealthPercent > AstrologianSettings.Instance.Pvp_MicrocosmosHealthPercent)
                return false;

            return await Spells.MicrocosmosPvp.Cast(Core.Me);
        }

        public static async Task<bool> CelestialRiverPvp()
        {
            if (!Spells.CelestialRiverPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Group.CastableAlliesWithin10.Count(x => x.IsValid && x.IsAlive) < AstrologianSettings.Instance.Pvp_CelestialRiverNearbyAllies)
                return false;

            return await Spells.CelestialRiverPvp.Cast(Core.Me);
        }
    }
}