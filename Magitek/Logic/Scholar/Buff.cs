using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using Magitek.Toggles;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Scholar
{
    internal static class Buff
    {
        // Prevents double summoning of fairy
        public static DateTime FairySummonCooldown = DateTime.Now;

        public static async Task<bool> SummonPet()
        {
            if (Core.Me.Pet != null)
                return false;

            if (Core.Me.HasAura(Auras.Dissipation))
                return false;

            if (Casting.LastSpell == Spells.SummonSeraph)
                return false;

            if (DateTime.Now <= FairySummonCooldown)
                return false;

            // To prevent routine recasting fairy when the game nulls the pet during Seraph transition back to fairy.
            if (Spells.SummonSeraph.Cooldown.TotalSeconds - 90 > 0)
                return false;

            switch (ScholarSettings.Instance.SelectedPet)
            {
                case ScholarPets.None:
                    return false;

                case ScholarPets.Eos:
                    if (await Spells.SummonEos.Cast(Core.Me))
                    {
                        FairySummonCooldown = DateTime.Now.AddSeconds(10);
                        break;
                    }
                    return false;

                case ScholarPets.Selene:
                    if (await Spells.SummonSelene.Cast(Core.Me))
                    {
                        FairySummonCooldown = DateTime.Now.AddSeconds(10);
                        break;
                    }
                    return false;

                default:
                    return false;
            }

            return await Coroutine.Wait(5000, () => Core.Me.Pet != null);
        }

        public static async Task<bool> SummonSeraph()
        {
            if (!ScholarSettings.Instance.SummonSeraph)
                return false;

            if (Core.Me.Pet == null)
                return false;

            if (!Core.Me.InCombat)
                return false;

            // check if seraph is already active
            if (Core.Me.Pet.EnglishName == "Seraph")
                return false;

            if (Globals.InParty)
            {
                if (Group.CastableAlliesWithin30.Count(CanSummonSeraph) < ScholarSettings.Instance.SummonSeraphNeedHealing)
                    return false;

                FairySummonCooldown = DateTime.Now.AddSeconds(30);
                return await Spells.SummonSeraph.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.SummonSeraphHpPercent)
                return false;

            FairySummonCooldown = DateTime.Now.AddSeconds(30);
            return await Spells.SummonSeraph.Cast(Core.Me);

            bool CanSummonSeraph(Character unit)
            {
                if (unit == null)
                    return false;
                return unit.CurrentHealthPercent < ScholarSettings.Instance.SummonSeraphHpPercent;
            }
        }

        public static async Task<bool> Swiftcast()
        {
            if (await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast))
            {
                return await Coroutine.Wait(15000, () => Core.Me.HasAura(Auras.Swiftcast, true, 7000));
            }

            return false;
        }
        public static async Task<bool> ForceSeraph()
        {
            if (!ScholarSettings.Instance.ForceSeraph)
                return false;

            if (!await Spells.SummonSeraph.Cast(Core.Me)) return false;
            ScholarSettings.Instance.ForceSeraph = false;
            TogglesManager.ResetToggles();
            return true;
        }
        public static async Task<bool> EmergencyTactics()
        {
            if (!ScholarSettings.Instance.EmergencyTactics)
                return false;

            if (Spells.EmergencyTactics.Cooldown != TimeSpan.Zero)
                return false;

            if (!await Spells.EmergencyTactics.CastAura(Core.Me, Auras.EmergencyTactics))
                return false;

            return await Coroutine.Wait(1500, () => Core.Me.HasAura(Auras.EmergencyTactics) && ActionManager.CanCast(Spells.Adloquium.Id, Core.Me));

            //if (await Spells.EmergencyTactics.CastAura(Core.Me, Auras.EmergencyTactics)) {
            //    return await Coroutine.Wait(1700, () => Core.Me.HasAura(Auras.EmergencyTactics, true));
            //}

            //return false;
        }

        public static async Task<bool> Aetherflow()
        {
            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.HasAetherflow())
                return false;

            if (Spells.Aetherflow.Cooldown.TotalMilliseconds > 1500)
            {
                Logger.Error("Aetherflow on cooldown");
                return false;
            }

            //if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
            //    if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
            //        return true;
            return await Spells.Aetherflow.Cast(Core.Me);
        }

        public static async Task<bool> DeploymentTactics()
        {
            if (!ScholarSettings.Instance.DeploymentTactics)
                return false;
            // Stop if we're in Combat, we can waste this when we don't know if the tank is pulling or not
            if (!Core.Me.InCombat)
                return false;
            if (Spells.DeploymentTactics.Cooldown.TotalMilliseconds > 1500)
                return false;
            // Find someone who has the right amount of allies around them based on the users settings
            var deploymentTacticsTarget = Group.CastableAlliesWithin30.FirstOrDefault(r =>
                r.HasAura(Auras.Galvanize, true)
                && r.HasAura(Auras.Catalyze, true)
                //Range now 30y
                && Group.CastableAlliesWithin30.Count(x => x.Distance(r) <= 30 + x.CombatReach) >= ScholarSettings.Instance.DeploymentTacticsAllyInRange);

            if (deploymentTacticsTarget == null)
                return false;
            //if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
            //    if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
            //        return true;
            return await Spells.DeploymentTactics.Cast(deploymentTacticsTarget);
        }

        public static async Task<bool> LucidDreaming()
        {
            return await Roles.Healer.LucidDreaming(ScholarSettings.Instance.LucidDreaming, ScholarSettings.Instance.LucidDreamingManaPercent);
        }

        public static async Task<bool> ChainStrategem()
        {
            if (!Core.Me.InCombat)
                return false;

            if (!ActionManager.HasSpell(Spells.ChainStrategem.Id))
                return false;

            if (Spells.ChainStrategem.Cooldown.TotalMilliseconds > 1500)
                return false;

            switch (ScholarSettings.Instance.ChainStrategemsStrategy)

            {

                case ChainStrategemStrategemStrategy.Never:
                    return false;

                case ChainStrategemStrategemStrategy.Always:
                    if (!Globals.InParty)
                        return await Spells.ChainStrategem.Cast(Core.Me.CurrentTarget);

                    var chainStrategemsTarget = GameObjectManager.Attackers.FirstOrDefault(r => r.Distance(Core.Me) <= 25 && r.HasAura(Auras.ChainStratagem) == false && r.HasTarget && r.TargetGameObject.IsTank());

                    if (chainStrategemsTarget == null || !chainStrategemsTarget.ThoroughCanAttack())
                        return false;
                    //if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                    //    if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                    //        return true;
                    return await Spells.ChainStrategem.Cast(chainStrategemsTarget);

                case ChainStrategemStrategemStrategy.OnlyBosses:
                    if (!Globals.InParty && Core.Me.CurrentTarget.IsBoss())
                        return await Spells.ChainStrategem.Cast(Core.Me.CurrentTarget);

                    var chainStrategemsBossTarget = GameObjectManager.Attackers.FirstOrDefault(r => r.Distance(Core.Me) <= 25 && r.IsBoss() && r.HasAura(Auras.ChainStratagem) == false && r.HasTarget && r.TargetGameObject.IsTank());

                    if (chainStrategemsBossTarget == null || !chainStrategemsBossTarget.ThoroughCanAttack())
                        return false;
                    //if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                    //    if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                    //        return true;
                    return await Spells.ChainStrategem.Cast(chainStrategemsBossTarget);

                default:
                    return false;
            }
        }

        public static async Task<bool> Aetherpact()
        {
            // Already checking for a null pet in the rotation

            if (!ScholarSettings.Instance.Aetherpact)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Casting.LastSpell == Spells.Aetherpact)
                return false;

            if (!ActionManager.HasSpell(Spells.Aetherpact.Id))
                return false;

            if (Group.CastableAlliesWithin30.Any(r => r.HasAura(Auras.FeyUnion) || r.HasAura(Auras.FeyUnion2)))
                return false;

            if (ActionResourceManager.Scholar.FaerieGauge < ScholarSettings.Instance.AetherpactMinimumFairieGauge)
                return false;

            var aetherpactTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanAetherpact);

            if (aetherpactTarget == null)
                return false;
            //if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
            //    if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
            //        return true;
            return await Spells.Aetherpact.Cast(aetherpactTarget);

            bool CanAetherpact(GameObject unit)
            {
                if (!unit.IsTank())
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.AetherpactHealthPercent)
                    return false;

                if (unit.HasAura(Auras.FeyUnion) || unit.HasAura(Auras.FeyUnion2))
                    return false;

                return true;
            }

        }

        public static async Task<bool> BreakAetherpact()
        {
            if (!ScholarSettings.Instance.Aetherpact)
                return false;

            if (!Globals.InParty)
                return false;

            if (!ActionManager.HasSpell(Spells.Aetherpact.Id))
                return false;

            if (!Group.CastableAlliesWithin30.Any(r => r.HasAura(Auras.FeyUnion) || r.HasAura(Auras.FeyUnion2)))
                return false;

            var aetherpactTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanDeAetherpact);

            if (aetherpactTarget == null)
                return false;

            return await Spells.Aetherpact.Cast(aetherpactTarget);

            bool CanDeAetherpact(GameObject unit)
            {
                if (unit.EnemiesNearby(6).Count() > ScholarSettings.Instance.AetherpactEnemies)
                    return false;

                if (unit.CurrentHealthPercent >= ScholarSettings.Instance.BreakAetherpactHp)
                    return false;

                if (!unit.HasAura(Auras.FeyUnion) || !unit.HasAura(Auras.FeyUnion2))
                    return false;

                return true;
            }
        }

        public static async Task<bool> Expedient()
        {
            if (!ScholarSettings.Instance.Expedient)
                return false;

            if (Core.Me.ClassLevel < Spells.Expedient.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.Expedient.Cooldown != TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.Expedience))
                return false;

            if (Globals.InParty)
            {
                var canExpedientTargets = Group.CastableAlliesWithin30.Where(CanExpedient).ToList();

                if (canExpedientTargets.Count < ScholarSettings.Instance.ExpedientNeedHealing)
                    return false;

                return await Spells.Expedient.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.ExpedientHealthPercent)
                return false;

            return await Spells.Expedient.Cast(Core.Me);

            bool CanExpedient(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.HasAura(Auras.Expedience))
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.ExpedientHealthPercent)
                    return false;

                //Radius is now 30y
                return unit.Distance(Core.Me) <= 30;
            }
        }

        public static async Task<bool> Protraction()
        {
            if (!ScholarSettings.Instance.Protraction)
                return false;

            if (Core.Me.ClassLevel < Spells.Protraction.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.Protraction.Cooldown != TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.Protraction))
                return false;

            if (Globals.InParty)
            {
                var canProtractionTargets = Group.CastableAlliesWithin30.Where(CanProtraction).ToList();

                var protractionTarget = canProtractionTargets.FirstOrDefault();

                if (protractionTarget == null)
                    return false;

                return await Spells.Protraction.CastAura(protractionTarget, Auras.Protraction);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.ProtractionHealthPercent)
                return false;

            return await Spells.Protraction.CastAura(Core.Me, Auras.Protraction);

            bool CanProtraction(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.HasAura(Auras.Protraction))
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.ProtractionHealthPercent)
                    return false;

                if (ScholarSettings.Instance.ProtractionOnlyTank && !unit.IsTank())
                    return false;

                return unit.Distance(Core.Me) <= 30;
            }
        }
    }
}
