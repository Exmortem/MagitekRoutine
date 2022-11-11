using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.WhiteMage
{
    internal static class Pvp
    {
        public static async Task<bool> GlareIIIPvp()
        {

            if (!Spells.GlareIIIPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.GlareIIIPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AfflatusMiseryPvp()
        {
            if (!Spells.AfflatusMiseryPvp.CanCast())
                return false;

            if (!WhiteMageSettings.Instance.Pvp_UseAfflatusMisery)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.AfflatusMiseryPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MiracleOfNaturePvp()
        {
            if (!Spells.MiracleOfNaturePvp.CanCast())
                return false;

            if (!WhiteMageSettings.Instance.Pvp_UseMiracleOfNature)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.Guard))
                return false;

            return await Spells.MiracleOfNaturePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CureIIPvp()
        {

            if (!Spells.CureIIPvp.CanCast())
                return false;

            if(!WhiteMageSettings.Instance.Pvp_Cure)
                return false;   

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (WhiteMageSettings.Instance.Pvp_HealSelfOnly)
            {
                if(Core.Me.CurrentHealthPercent>WhiteMageSettings.Instance.Pvp_CureHealthPercent)
                    return false;

                return await Spells.CureIIPvp.Heal(Core.Me);

            } 
                if (Globals.HealTarget?.CurrentHealthPercent <= WhiteMageSettings.Instance.Pvp_CureHealthPercent)
                {
                    return await Spells.CureIIPvp.Heal(Globals.HealTarget);
                }

                var cure2Target = Group.CastableAlliesWithin30.FirstOrDefault(r =>  r.CurrentHealth > 0 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.Pvp_CureHealthPercent);

            if (cure2Target == null)
                if (Core.Me.CurrentHealthPercent <= WhiteMageSettings.Instance.Pvp_CureHealthPercent)
                    return await Spells.CureIIPvp.Heal(Core.Me);
                else
                    return false;

            return await Spells.CureIIPvp.Heal(cure2Target);
        }

        public static async Task<bool> CureIIIPvp()
        {

            if (!Spells.CureIIIPvp.CanCast())
                return false;

            if (!WhiteMageSettings.Instance.Pvp_Cure)
                return false;

            if(!Core.Me.HasAura(Auras.CureIIIReady))
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (WhiteMageSettings.Instance.Pvp_HealSelfOnly)
            {
                if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.Pvp_CureHealthPercent)
                    return false;

                return await Spells.CureIIIPvp.Heal(Core.Me);

            }
            if (Globals.HealTarget?.CurrentHealthPercent <= WhiteMageSettings.Instance.Pvp_CureHealthPercent)
            {
                return await Spells.CureIIIPvp.Heal(Globals.HealTarget);
            }

            var cure2Target = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealth > 0 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.Pvp_CureHealthPercent);

            if (cure2Target == null)
                if (Core.Me.CurrentHealthPercent <= WhiteMageSettings.Instance.Pvp_CureHealthPercent)
                    return await Spells.CureIIIPvp.Heal(Core.Me);

            return await Spells.CureIIIPvp.Heal(cure2Target);


        }

        public static async Task<bool> AquaveilPvp()
        {

            if (!Spells.AquaveilPvp.CanCast())
                return false;

            if (!WhiteMageSettings.Instance.Pvp_Aquaveil)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (WhiteMageSettings.Instance.Pvp_HealSelfOnly)
            {
                if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.Pvp_AquaveilHealthPercent)
                    return false;

                return await Spells.AquaveilPvp.CastAura(Core.Me, Auras.PvpAquaveil);
            }

            var canAquaveilTargets = Group.CastableAlliesWithin30.Where(CanAquaveil).ToList();

            var aquaveilTarget = canAquaveilTargets.FirstOrDefault();

            if (aquaveilTarget == null)
                if (Core.Me.CurrentHealthPercent <= WhiteMageSettings.Instance.Pvp_AquaveilHealthPercent)
                    return await Spells.AquaveilPvp.CastAura(Core.Me, Auras.PvpAquaveil);

            return await Spells.AquaveilPvp.CastAura(aquaveilTarget, Auras.PvpAquaveil);

            bool CanAquaveil(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > WhiteMageSettings.Instance.Pvp_AquaveilHealthPercent)
                    return false;

                return unit.Distance(Core.Me) <= 30;
            }
        }

        public static async Task<bool> AfflatusPurgationPvp()
        {

            if (!Spells.AfflatusPurgationPvp.CanCast())
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.AfflatusPurgationPvp.Cast(Core.Me.CurrentTarget);
        }
    }
}
