using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Sage;
using Magitek.Models.Sage;
using Magitek.Utilities;
using SageRoutine = Magitek.Utilities.Routines.Sage;
using System.Threading.Tasks;
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
            if (WorldManager.InSanctuary)
                return false;

            if (Core.Me.IsMounted)
                return false;

            if (Core.Me.IsCasting)
                return true;

            await Casting.CheckForSuccessfulCast();

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
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }
            else
            {
                if (Globals.InParty)
                {
                    if (!SageSettings.Instance.DoDamage)
                        return false;
                }
            }

            if (Core.Me.InCombat)
                return false;

            return await SingleTarget.Dosis();
        }

        public static async Task<bool> Heal()
        {
            if (WorldManager.InSanctuary)
                return false;

            if (Core.Me.IsMounted)
                return false;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            if (await GambitLogic.Gambit())
                return true;

            if (await Logic.Sage.Heal.Egeiro()) return true;
            if (await Dispel.Execute()) return true;

            if (SageRoutine.GlobalCooldown.CanWeave())
            {
                if (await Buff.LucidDreaming()) return true;
                if (await Buff.Kardia()) return true;
                if (await Buff.Soteria()) return true;
                if (await Buff.Rhizomata()) return true;
                if (await Buff.Krasis()) return true;
            }

            if (Globals.InActiveDuty || Core.Me.InCombat)
            {
                if (!SageSettings.Instance.WeaveOGCDHeals || SageRoutine.GlobalCooldown.CanWeave(1))
                {
                    if (await Buff.Kerachole()) return true;
                    if (await Buff.Holos()) return true;
                    if (await Logic.Sage.Heal.Taurochole()) return true;
                    if (await Logic.Sage.Heal.Panhaima()) return true;
                    if (await Logic.Sage.Heal.Haima()) return true;
                    if (await Logic.Sage.Heal.Pepsis()) return true;
                    if (await Logic.Sage.Heal.Physis()) return true;
                    if (await Logic.Sage.Heal.Ixochole()) return true;
                    if (await Logic.Sage.Heal.Druochole()) return true;
                }

                if (await Logic.Sage.Heal.PepsisEukrasianPrognosis()) return true;
                if (await Logic.Sage.Heal.Shield()) return true;
                if (await Logic.Sage.Heal.ZoePneuma()) return true;
                if (await Logic.Sage.Heal.Pneuma()) return true;
                if (await Logic.Sage.Heal.EukrasianPrognosis()) return true;
                if (await Logic.Sage.Heal.Prognosis()) return true;
                if (await Logic.Sage.Heal.EukrasianDiagnosis()) return true;
                if (await Logic.Sage.Heal.Diagnosis()) return true;
            }

            return false;
        }

        public static async Task<bool> CombatBuff()
        {
            if (await Buff.Kardia()) return true;

            return false;
        }

        public static async Task<bool> Combat()
        {
            await CombatBuff();

            //Only stop doing damage when in party
            if (Globals.InParty && Utilities.Combat.Enemies.Count > SageSettings.Instance.StopDamageWhenMoreThanEnemies)
                return false;

            if (Globals.InParty)
            {
                if (!SageSettings.Instance.DoDamage)
                    return true;

                if (Core.Me.CurrentManaPercent < SageSettings.Instance.MinimumManaPercentToDoDamage
                    && Core.Target.CombatTimeLeft() > SageSettings.Instance.DoDamageIfTimeLeftLessThan)
                    return true;
            }

            if (!GameSettingsManager.FaceTargetOnAction
                && !Core.Me.CurrentTarget.InView())
                return false;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if (!Core.Me.HasTarget
                || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Globals.OnPvpMap)
            {
                return false;
            }

            if (await AoE.Phlegma()) return true;
            if (await AoE.Toxikon()) return true;
            if (await AoE.Pneuma()) return true;
            if (await AoE.Dyskrasia()) return true;
            if (await SingleTarget.EukrasianDosis()) return true;
            if (await SingleTarget.DotMultipleTargets()) return true;
            return await SingleTarget.Dosis();
        }

    }
}
