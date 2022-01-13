using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magitek.Logic.Sage
{
    internal static class Shield
    {
        private static async Task<bool> ShieldHealers()
        {
            if (!SageSettings.Instance.ShieldOnHealers)
                return false;

            var shieldTarget = SageSettings.Instance.ShieldKeepUpOnHealers
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && r.IsHealer() && !r.HasAura(Auras.EukrasianDiagnosis, true))
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && r.IsHealer() && r.CurrentHealthPercent <= SageSettings.Instance.ShieldHealthPercent && !r.HasAura(Auras.EukrasianDiagnosis, true));

            if (shieldTarget == null)
                return false;

            if (!await Heal.UseEukrasia(targetObject: shieldTarget))
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(shieldTarget, Auras.EukrasianDiagnosis);
        }
        private static async Task<bool> ShieldTanks()
        {
            if (!SageSettings.Instance.ShieldOnTanks)
                return false;

            var shieldTarget = SageSettings.Instance.ShieldKeepUpOnTanks ?
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && !r.HasAura(Auras.EukrasianDiagnosis, true)) :
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && !r.HasAura(Auras.EukrasianDiagnosis, true) && r.CurrentHealthPercent <= SageSettings.Instance.ShieldHealthPercent);

            if (!MovementManager.IsMoving && SageSettings.Instance.OnlyShieldWhileMoving)
                return false;

            if (shieldTarget == null)
                return false;

            if (!await Heal.UseEukrasia(targetObject: shieldTarget))
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(shieldTarget, Auras.EukrasianDiagnosis); ;
        }

        private static async Task<bool> ShieldDps()
        {
            if (!SageSettings.Instance.ShieldOnDps)
                return false;

            var shieldTarget = SageSettings.Instance.ShieldKeepUpOnDps
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && !r.IsTank() && !r.IsHealer() && !r.HasAura(Auras.EukrasianDiagnosis, true))
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && !r.IsTank() && !r.IsHealer() && !r.HasAura(Auras.EukrasianDiagnosis, true) && r.CurrentHealthPercent <= SageSettings.Instance.ShieldHealthPercent);

            if (shieldTarget == null)
                return false;

            if (!await Heal.UseEukrasia(targetObject: shieldTarget))
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(shieldTarget, Auras.EukrasianDiagnosis);
        }

        public static async Task<bool> ShieldsUpRedAlert()
        {
            if (!SageSettings.Instance.Shield)
                return false;

            if (Globals.InParty)
            {
                if (await ShieldTanks()) return true;
                if (await ShieldHealers()) return true;
                return await ShieldDps();
            }
            else
            {
                if (Core.Me.HasAura(Auras.EukrasianDiagnosis))
                    return false;

                if (!SageSettings.Instance.ShieldKeepUpOnHealers && Core.Me.CurrentHealthPercent > SageSettings.Instance.ShieldHealthPercent)
                    return false;

                if (!await Heal.UseEukrasia())
                    return false;

                return await Spells.EukrasianDiagnosis.HealAura(Core.Me, Auras.EukrasianDiagnosis);
            }
        }
    }
}
