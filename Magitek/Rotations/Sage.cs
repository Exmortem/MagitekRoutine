using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Sage;
using Magitek.Models.Sage;
using Magitek.Utilities;
using SageRoutine = Magitek.Utilities.Routines.Sage;
using System.Threading.Tasks;
using Magitek.Logic.Roles;
using Magitek.Models.Account;

namespace Magitek.Rotations
{
    public static class Sage
    {
        public static Task<bool> Rest()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (Core.Me.IsMounted)
                return false;

            if (Core.Me.IsCasting)
                return true;

            await Casting.CheckForSuccessfulCast();

            if (WorldManager.InSanctuary)
                return false;

            if (Globals.OnPvpMap)
                return false;

            if (await Buff.Kardia()) return true;

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20 + Core.Me.CurrentTarget.CombatReach);
            }

            if (Globals.InParty && Utilities.Combat.Enemies.Count > SageSettings.Instance.StopDamageWhenMoreThanEnemies)
                return false;

            if (!SageSettings.Instance.DoDamage)
                return false;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (WorldManager.InSanctuary)
                return false;

            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            if (Core.Me.IsMounted)
                return false;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            if (await GambitLogic.Gambit())
                return true;

            //LimitBreak
            if (Logic.Sage.Heal.ForceLimitBreak()) return true;

            if (await Logic.Sage.Heal.Egeiro()) return true;
            if (await Dispel.Execute()) return true;

            if (await Logic.Sage.Heal.ForceZoePneuma()) return true;
            if (await Logic.Sage.Heal.ForceEukrasianPrognosis()) return true;
            if (await Logic.Sage.Heal.ForceHaima()) return true;
            if (await Logic.Sage.Heal.ForcePanhaima()) return true;
            if (await Logic.Sage.Heal.ForcePepsisEukrasianPrognosis()) return true;
            if (await Logic.Sage.Heal.ForceEukrasianDiagnosis()) return true;

            if (await Logic.Sage.HealFightLogic.Aoe()) return true;
            if (await Logic.Sage.HealFightLogic.Tankbuster()) return true;

            if (SageRoutine.CanWeave())
            {
                if (await Buff.LucidDreaming()) return true;
                if (await Buff.Rhizomata()) return true;
                if (await Buff.Krasis()) return true;
            }

            if (Globals.InActiveDuty || Core.Me.InCombat)
            {
                if (SageRoutine.CanWeave())
                {
                    if (await Logic.Sage.Heal.Pepsis()) return true;
                    if (await Logic.Sage.Heal.Panhaima()) return true;
                    if (await Logic.Sage.Heal.Holos()) return true;
                    if (await Logic.Sage.Heal.Physis()) return true;
                    if (await Logic.Sage.Heal.Ixochole()) return true;
                    if (await Logic.Sage.Heal.Kerachole()) return true;
                    if (await Logic.Sage.Heal.Haima()) return true;
                    if (await Logic.Sage.Heal.Taurochole()) return true;
                    if (await Logic.Sage.Heal.Druochole()) return true;
                }

                if (await Logic.Sage.Heal.ZoePneuma()) return true;
                if (await Logic.Sage.Heal.PepsisEukrasianPrognosis()) return true;
                if (await Logic.Sage.Heal.Pneuma()) return true;
                if (await Logic.Sage.Heal.EukrasianPrognosis()) return true;
                if (await Logic.Sage.Heal.Prognosis()) return true;
                if (await Logic.Sage.Heal.EukrasianDiagnosis()) return true;
                if (await Logic.Sage.Heal.Diagnosis()) return true;
                if (await Logic.Sage.Shield.ShieldsUpRedAlert()) return true;
            }

            return await HealAlliance();
        }

        public static async Task<bool> HealAlliance()
        {
            if (Group.CastableAlliance.Count == 0)
                return false;

            Group.SwitchCastableToAlliance();
            var res = await DoHeal();
            Group.SwitchCastableToParty();
            return res;

            async Task<bool> DoHeal()
            {
                if (await Logic.Sage.Heal.Egeiro()) return true;

                if (SageSettings.Instance.HealAllianceOnlyDiagnosis)
                {
                    if (await Logic.Sage.Heal.Diagnosis()) return true;
                    return false;
                }

                if (SageRoutine.CanWeave())
                {
                    if (await Logic.Sage.Heal.Taurochole()) return true;
                    if (await Logic.Sage.Heal.Haima()) return true;
                    if (await Logic.Sage.Heal.Druochole()) return true;
                }

                if (await Logic.Sage.Heal.EukrasianDiagnosis()) return true;
                if (await Logic.Sage.Heal.Diagnosis()) return true;

                return false;
            }
        }

        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> Combat()
        {
            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            //Only stop doing damage when in party
            if (Globals.InParty && Utilities.Combat.Enemies.Count > SageSettings.Instance.StopDamageWhenMoreThanEnemies)
                return false;

            if (!SageSettings.Instance.DoDamage)
                return false;

            if (!GameSettingsManager.FaceTargetOnAction
                && !Core.Me.CurrentTarget.InView())
                return false;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Globals.OnPvpMap)
            {
                return false;
            }

            if (SageRoutine.CanWeave())
            {
                if (await Buff.Kardia()) return true;
                if (await Buff.Soteria()) return true;
            }

            if (Core.Me.CurrentManaPercent < SageSettings.Instance.MinimumManaPercentToDoDamage
                && Core.Target.CombatTimeLeft() > SageSettings.Instance.DoDamageIfTimeLeftLessThan)
            {
                if (await AoE.Toxikon()) return true;
                return true;
            }

            if (await AoE.Toxikon()) return true;
            if (await AoE.Phlegma()) return true;
            if (await AoE.Pneuma()) return true;
            if (await SingleTarget.EukrasianDosis()) return true;
            if (await SingleTarget.DotMultipleTargets()) return true;
            if (await AoE.Dyskrasia()) return true;

            return await SingleTarget.Dosis();
        }


        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            SageRoutine.RefreshVars();

            if (await Healer.Guard(SageSettings.Instance)) return true;
            if (await Healer.Purify(SageSettings.Instance)) return true;
            if (await Healer.Recuperate(SageSettings.Instance)) return true;

            if (await Pvp.MesotesPvp()) return true;

            if (await Pvp.KardiaPvp()) return true;
            if (await Pvp.PneumaPvp()) return true;
            if (await Pvp.EukrasiaPvp()) return true;

            if (!Healer.GuardCheck())
            {
                if (await Pvp.PhlegmaIIIPvp()) return true;
                if (await Pvp.ToxikonPvp()) return true;
            }

            return (await Pvp.DosisIIIPvp());
        }
    }
}
