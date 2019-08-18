using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Scholar;
using Magitek.Models.Scholar;
using Magitek.Utilities;

namespace Magitek.Rotations.Scholar
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Core.Me.CurrentManaPercent <= ScholarSettings.Instance.MinimumManaPercent)
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (await Logic.Scholar.Heal.Resurrection()) return true;

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
            {
                return await Dispel.Execute();
            }

            #region Pre-Healing Stuff
            if (Globals.PartyInCombat && Globals.InParty)
            {
                if (await TankBusters.Execute()) return true;
            }

            if (await Buff.Aetherflow()) return true;
            if (await Buff.LucidDreaming()) return true;
            if (await Dispel.Execute()) return true;

            if (Globals.InParty)
            {
                if (await Buff.DeploymentTactics()) return true;
                if (await Buff.Aetherpact()) return true;
            }

            if (await Buff.ChainStrategem()) return true;

            #endregion

            if (await Logic.Scholar.Heal.Excogitation()) return true;
            if (await Logic.Scholar.Heal.Lustrate()) return true;

            if (Core.Me.Pet != null && Core.Me.InCombat)
            {
                if (await Logic.Scholar.Heal.FeyBlessing()) return true;
                if (await Logic.Scholar.Heal.WhisperingDawn()) return true;
                if (await Logic.Scholar.Heal.FeyIllumination()) return true;
                if (await Logic.Scholar.Heal.SummonSeraph()) return true;
                if (await Logic.Scholar.Heal.Consolation()) return true;
            }

            if (Globals.InParty)
            {
                if (await Logic.Scholar.Heal.Indomitability()) return true;
                if (await Logic.Scholar.Heal.Succor()) return true;
                if (await Logic.Scholar.Heal.SacredSoil()) return true;
            }

            if (await Logic.Scholar.Heal.EmergencyTacticsAdlo()) return true;
            if (await Logic.Scholar.Heal.Adloquium()) return true;
            if (await Logic.Scholar.Heal.Physick()) return true;

            if (await Buff.SummonPet()) return true;

            if (Utilities.Combat.Enemies.Count > ScholarSettings.Instance.StopDamageWhenMoreThanEnemies)
                return true;

            if (PartyManager.IsInParty)
            {
                if (!ScholarSettings.Instance.DoDamage)
                    return true;

                if (Core.Me.CurrentManaPercent < ScholarSettings.Instance.MinimumManaPercent)
                    return true;
            }

            if (await SingleTarget.Bio()) return true;
            if (await SingleTarget.BioMultipleTargets()) return true;
            if (await SingleTarget.Ruin2()) return true;
            if (await SingleTarget.EnergyDrain2()) return true;
            return await SingleTarget.Broil();
        }
    }
}
