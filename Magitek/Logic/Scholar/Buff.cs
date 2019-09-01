using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Scholar
{
    internal static class Buff
    {
        public static async Task<bool> SummonPet()
        {
            if (Core.Me.Pet != null)
                return false;
            if (Core.Me.HasAura(Auras.Dissipation))
                return false;

            if (Casting.LastSpell == Spells.SummonEos)
                return false;

            if (Casting.LastSpell == Spells.SummonSelene)
                return false;

            switch (ScholarSettings.Instance.SelectedPet)
            {
                case ScholarPets.None:
                    return false;

                case ScholarPets.Eos:
                    if (!await Spells.SummonEos.Cast(Core.Me))
                        return false;
                    break;

                case ScholarPets.Selene:
                    if (!await Spells.SummonSelene.Cast(Core.Me))
                        return false;
                    break;

                default:
                    return false;
            }

            return await Coroutine.Wait(5000, () => Core.Me.Pet != null);
        }

        public static async Task<bool> Swiftcast()
        {
            if (await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast))
            {
                return await Coroutine.Wait(15000, () => Core.Me.HasAura(Auras.Swiftcast, true, 7000));
            }

            return false;
        }

        public static async Task<bool> EmergencyTactics()
        {
            if (Spells.EmergencyTactics.Cooldown != TimeSpan.Zero)
                return false;

            if (await Spells.EmergencyTactics.CastAura(Core.Me, Auras.EmergencyTactics))
            {
                return await Coroutine.Wait(15000, () => Core.Me.HasAura(Auras.EmergencyTactics, true, 10000));
            }

            return false;
        }

        public static async Task<bool> Aetherflow()
        {
            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.HasAetherflow())
                return false;
            if (Spells.Aetherflow.Cooldown.TotalMilliseconds > 1500)
                return false;
            if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                    return true;
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
            var deploymentTacticsTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.HasAura(Auras.Galvanize) && r.HasAura(Auras.Catalyze) && Group.CastableAlliesWithin30.Count(x => x.Distance(r) <= 10) >= ScholarSettings.Instance.DeploymentTacticsAllyInRange);

            if (deploymentTacticsTarget == null)
                return false;
            if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                    return true;
            return await Spells.DeploymentTactics.Cast(deploymentTacticsTarget);
        }

        public static async Task<bool> LucidDreaming()
        {
            if (!ScholarSettings.Instance.LucidDreaming)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.CurrentManaPercent > ScholarSettings.Instance.LucidDreamingManaPercent)
                return false;

            if (Spells.LucidDreaming.Cooldown.TotalMilliseconds > 1500)
                return false; 

            //force ruin 2 cast to open GCD 
            if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                    return true;
            return await Spells.LucidDreaming.Cast(Core.Me);
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

                    var chainStrategemsTarget = GameObjectManager.Attackers.FirstOrDefault(r => r.Distance(Core.Me) <= 25 && r.HasTarget && r.TargetGameObject.IsTank());

                    if (chainStrategemsTarget == null)
                        return false;
                    if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                        if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                            return true;
                    return await Spells.ChainStrategem.Cast(chainStrategemsTarget);

                case ChainStrategemStrategemStrategy.OnlyBosses:
                    if (!Globals.InParty)
                        return await Spells.ChainStrategem.Cast(Core.Me.CurrentTarget);

                    var chainStrategemsBossTarget = GameObjectManager.Attackers.FirstOrDefault(r => r.Distance(Core.Me) <= 25 && r.IsBoss() && r.HasTarget && r.TargetGameObject.IsTank());

                    if (chainStrategemsBossTarget == null)
                        return false;
                    if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                        if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                            return true;
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
            if (Casting.LastSpell != Spells.Biolysis || Casting.LastSpell != Spells.ArtOfWar || Casting.LastSpell != Spells.Adloquium || Casting.LastSpell != Spells.Succor)
                if (await Spells.Ruin2.Cast(Core.Me.CurrentTarget))
                    return true;
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
    }
}
