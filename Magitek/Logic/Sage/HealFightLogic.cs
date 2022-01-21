using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Toggles;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Sage;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Sage
{
    internal static class HealFightLogic
    {
        public static async Task<bool> Aoe()
        {
            if (!Globals.InParty)
                return false;

            if (!FightLogic.ZoneHasFightLogic())
                return false;

            if (!FightLogic.EnemyIsCastingBigAoe())
                return false;

            var useAoEBuffs = Heal.UseAoEHealingBuff(Group.CastableAlliesWithin15);

            if (SageSettings.Instance.FightLogic_Panhaima
                && Spells.Panhaima.IsKnownAndReady()
                && useAoEBuffs)
            {
                var targets = Group.CastableAlliesWithin15.Where(r => !r.HasAura(Auras.Panhaimatinon));

                if (targets.Count() >= SageSettings.Instance.PanhaimaNeedHealing)
                    return await FightLogic.DoAndBuffer.Aoe(Spells.Panhaima.CastAura(Core.Me, Auras.Panhaimatinon));
            }

            if (SageSettings.Instance.FightLogic_Kerachole
                && Spells.Kerachole.IsKnownAndReady()
                && Addersgall >= 1
                && useAoEBuffs)
            {
                var targets = Group.CastableAlliesWithin15.Where(r => !r.HasAura(Auras.Kerachole) && !r.HasAura(Auras.Taurochole));

                if (targets.Count() >= SageSettings.Instance.KeracholeNeedHealing)
                    return await FightLogic.DoAndBuffer.Aoe(Spells.Kerachole.CastAura(Core.Me, Auras.Kerachole));
            }

            if (SageSettings.Instance.FightLogic_Holos
                && Spells.Holos.IsKnownAndReady()
                && useAoEBuffs)
            {
                var targets = Group.CastableAlliesWithin15.Where(r => !r.HasAura(Auras.Holos));

                if (targets.Count() >= SageSettings.Instance.KeracholeNeedHealing)
                    return await FightLogic.DoAndBuffer.Aoe(Spells.Holos.CastAura(Core.Me, Auras.Holos));
            }

            if (SageSettings.Instance.FightLogic_EukrasianPrognosis
                && Core.Me.ClassLevel >= Spells.Eukrasia.LevelAcquired)
            {
                var targets = Group.CastableAlliesWithin15.Where(r => !r.HasAura(Auras.EukrasianDiagnosis)
                                                                && !r.HasAura(Auras.EukrasianPrognosis)
                                                                && !r.HasAura(Auras.Galvanize));

                if (targets.Count() >= SageSettings.Instance.EukrasianPrognosisNeedHealing)
                {
                    if (await Heal.UseEukrasia(Spells.EukrasianPrognosis.Id))
                        return await FightLogic.DoAndBuffer.Aoe(Spells.EukrasianPrognosis.HealAura(Core.Me, Auras.EukrasianPrognosis));
                }

            }

            return false;
        }

        public static async Task<bool> Tankbuster()
        {
            if (!Globals.InParty)
                return false;

            if (!FightLogic.ZoneHasFightLogic())
                return false;

            var target = FightLogic.EnemyIsCastingTankBuster();

            if (target == null)
                return false;

            if (SageSettings.Instance.FightLogic_Haima
                && Spells.Haima.IsKnownAndReady()
                && !target.HasAura(Auras.Haimatinon)
                && !target.HasAura(Auras.Panhaimatinon))
            {
                return await FightLogic.DoAndBuffer.Tankbuster(Spells.Haima.CastAura(target, Auras.Haimatinon));
            }

            if (SageSettings.Instance.FightLogic_Taurochole
                && Spells.Taurochole.IsKnownAndReady()
                && !target.HasAura(Auras.Taurochole))
            {
                return await FightLogic.DoAndBuffer.Tankbuster(Spells.Taurochole.HealAura(target, Auras.Taurochole));
            }

            if (SageSettings.Instance.FightLogic_EukrasianDiagnosis
                && Core.Me.ClassLevel >= Spells.Eukrasia.LevelAcquired
                && !target.HasAura(Auras.EukrasianDiagnosis)
                && !target.HasAura(Auras.Galvanize)
                && !target.HasAura(Auras.EukrasianPrognosis))
            {
                if (await Heal.UseEukrasia(targetObject: target))
                    return await FightLogic.DoAndBuffer.Tankbuster(Spells.EukrasianDiagnosis.HealAura(target, Auras.EukrasianDiagnosis));
            }

            return false;
        }
    }
}
