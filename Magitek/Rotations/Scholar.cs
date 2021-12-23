using System.Linq;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Scholar;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Scholar
    {
        public static async Task<bool> Rest()
        {
            if (ScholarSettings.Instance.PhysickOnRest)
            {
                if (Core.Me.CurrentHealthPercent > 70 || Core.Me.ClassLevel < 4)
                    return false;

                await Spells.Physick.Heal(Core.Me);

                return true;
            }

            return false;
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

            if (CustomOpenerLogic.InOpener) return false;

            return await Buff.SummonPet();
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
                if (!ScholarSettings.Instance.DoDamage)
                    return false;
            }

            if (Core.Me.InCombat)
                return false;

            return await Heal();
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

            // Handle if Seraph is casted manually outside of the routine.
            if (Casting.LastSpell.Id == Spells.SummonSeraph.Id)
                Buff.SeraphCooldown = System.DateTime.Now.AddSeconds(30);

            if (await Buff.SummonPet()) return true;

            Casting.DoHealthChecks = false;

            if (await GambitLogic.Gambit()) return true;

            if (CustomOpenerLogic.InOpener) return false;
            
            if (await Logic.Scholar.Heal.Resurrection()) return true;

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
            {
                return await Dispel.Execute();
            }

            #region Pre-Healing Stuff

            if (await Logic.Scholar.Heal.ForceWhispDawn()) return true;
            if (await Logic.Scholar.Heal.ForceAdlo()) return true;
            if (await Logic.Scholar.Heal.ForceIndom()) return true;
            if (await Logic.Scholar.Heal.ForceExcog()) return true;

            if (await Dispel.Execute()) return true;
            if (await Buff.Aetherflow()) return true;
            if (await Buff.LucidDreaming()) return true;

            if (Globals.InParty)
            {
                if (await Buff.DeploymentTactics()) return true;
                if (await Buff.Aetherpact()) return true;
                if (await Buff.BreakAetherpact()) return true;
            }

            if (await Buff.ChainStrategem()) return true;

            #endregion

            if (await Logic.Scholar.Heal.Excogitation()) return true;
            if (await Logic.Scholar.Heal.Lustrate()) return true;

            if (Core.Me.Pet != null && Core.Me.InCombat)
            {
                if (await Logic.Scholar.Buff.SummonSeraph()) return true;
                if (await Logic.Scholar.Heal.Consolation()) return true;
                if (await Logic.Scholar.Heal.FeyIllumination()) return true;
                if (await Logic.Scholar.Heal.WhisperingDawn()) return true;
                if (await Logic.Scholar.Heal.FeyBlessing()) return true;
            }

            if (Globals.InParty)
            {
                if (await Logic.Scholar.Heal.Indomitability()) return true;
                if (await Logic.Scholar.Heal.EmergencyTacticsSuccor()) return true;
                if (await Logic.Scholar.Heal.Succor()) return true;
                if (await Logic.Scholar.Heal.SacredSoil()) return true;
            }

            if (await Buff.Expedient()) return true;
            if (await Buff.Protraction()) return true;
            if (await Logic.Scholar.Heal.EmergencyTacticsAdloquium()) return true;
            if (await Logic.Scholar.Heal.Adloquium()) return true;
            if (await Logic.Scholar.Heal.Physick()) return true;

            if (Utilities.Combat.Enemies.Count > ScholarSettings.Instance.StopDamageWhenMoreThanEnemies)
                return true;

            if (Globals.InParty)
            {
                if (!ScholarSettings.Instance.DoDamage)
                    return true;

                if (Core.Me.CurrentManaPercent < ScholarSettings.Instance.MinimumManaPercent)
                    return true;
            }

            return await Combat();
        }
        public static async Task<bool> CombatBuff()
        {
            return false;
        }
        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.CurrentManaPercent <= ScholarSettings.Instance.MinimumManaPercent)
                return false;

            if (Utilities.Combat.Enemies.Count > ScholarSettings.Instance.StopDamageWhenMoreThanEnemies)
                return true;

            if (!ScholarSettings.Instance.DoDamage)
                return true;

            if (Globals.InParty)
            {
                if (Core.Me.CurrentManaPercent < ScholarSettings.Instance.MinimumManaPercent)
                    return true;

                if (Group.CastableAlliesWithin30.Any(c => c.IsAlive && c.CurrentHealthPercent < ScholarSettings.Instance.DamageOnlyIfAboveHealthPercent))
                    return true;
            }

            if (await Aoe.ArtOfWar()) return true;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await SingleTarget.Bio()) return true;
            if (await SingleTarget.BioMultipleTargets()) return true;
            if (await SingleTarget.Ruin2()) return true;
            if (await SingleTarget.EnergyDrain2()) return true;
            return await SingleTarget.Broil();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
