using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Sage
{
    internal static class Shield
    {
        private static async Task<bool> ShieldHealers()
        {
            if (!SageSettings.Instance.ShieldOnHealers)
                return false;

            return await ShieldTarget(Group.CastableAlliesWithin30.Where(r => r.IsHealer()), SageSettings.Instance.ShieldKeepUpOnHealers);
        }

        private static async Task<bool> ShieldSelf()
        {
            if (!SageSettings.Instance.ShieldOnSelf)
                return false;

            return await ShieldTarget(Group.CastableAlliesWithin30.Where(r => r == Core.Me), SageSettings.Instance.ShieldKeepUpOnSelf);
        }

        private static async Task<bool> ShieldTanks()
        {
            if (!SageSettings.Instance.ShieldOnTanks)
                return false;

            return await ShieldTarget(Group.CastableAlliesWithin30.Where(r => r.IsTank()), SageSettings.Instance.ShieldKeepUpOnTanks);
        }

        private static async Task<bool> ShieldDps()
        {
            if (!SageSettings.Instance.ShieldOnDps)
                return false;

            return await ShieldTarget(Group.CastableAlliesWithin30.Where(r => r.IsDps()), SageSettings.Instance.ShieldKeepUpOnDps);
        }

        private static async Task<bool> ShieldTarget(IEnumerable<ff14bot.Objects.Character> targetBase, bool keepUp)
        {
            var targets = targetBase.Where(r => !r.HasAura(Auras.EukrasianDiagnosis, true));
            ff14bot.Objects.Character target = null;

            if (keepUp)
            {
                if (SageSettings.Instance.ShieldKeepUpUnlessAdderstingFull && ActionResourceManager.Sage.Addersting >= 3)
                    target = null;
                else
                {
                    if (SageSettings.Instance.ShieldKeepUpOnlyOutOfCombat)
                    {
                        if (!Core.Me.InCombat)
                            target = targets.FirstOrDefault();
                    }
                    else
                        target = targets.FirstOrDefault();
                }
            }

            if (target == null)
                target = targets.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.ShieldHealthPercent);

            if (target == null)
                return false;

            if (!await Heal.UseEukrasia(targetObject: target))
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(target, Auras.EukrasianDiagnosis);
        }

        public static async Task<bool> ShieldsUpRedAlert()
        {
            if (!SageSettings.Instance.Shield)
                return false;

            if (SageSettings.Instance.OnlyShieldWhileMoving && !MovementManager.IsMoving)
                return false;

            if (Globals.InParty)
            {
                if (await ShieldTanks()) return true;
                if (await ShieldSelf()) return true;
                if (await ShieldHealers()) return true;
                return await ShieldDps();
            }
            else
            {
                if (Core.Me.HasAura(Auras.EukrasianDiagnosis))
                    return false;

                var keepUpOnMe = SageSettings.Instance.ShieldKeepUpOnHealers || SageSettings.Instance.ShieldKeepUpOnSelf;
                var useOnMe = SageSettings.Instance.ShieldOnHealers || SageSettings.Instance.ShieldOnSelf;

                if (!useOnMe)
                    return false;

                if (!keepUpOnMe && Core.Me.CurrentHealthPercent > SageSettings.Instance.ShieldHealthPercent)
                    return false;

                if (keepUpOnMe && SageSettings.Instance.ShieldKeepUpUnlessAdderstingFull && ActionResourceManager.Sage.Addersting >= 3)
                    return false;

                if (keepUpOnMe && SageSettings.Instance.ShieldKeepUpOnlyOutOfCombat && Core.Me.InCombat)
                    return false;

                if (!await Heal.UseEukrasia())
                    return false;

                return await Spells.EukrasianDiagnosis.HealAura(Core.Me, Auras.EukrasianDiagnosis);
            }
        }
    }
}
