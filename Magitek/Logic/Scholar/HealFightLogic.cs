using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Sage;
using Magitek.Models.Scholar;
using Magitek.Properties;
using Magitek.Toggles;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Sage;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Scholar
{
    internal static class HealFightLogic
    {
        public static async Task<bool> Aoe()
        {
            if (!Globals.InParty)
                return false;

            if (!FightLogic.ZoneHasFightLogic())
                return false;

            if (FightLogic.EnemyIsCastingBigAoe()) return await BigAoe();

            if (FightLogic.EnemyIsCastingAoe()) return await JustAoe(); 

            return false;
        }

        private static async Task<bool> BigAoe()
        {
            if (!Spells.Succor.IsKnownAndReady())
                return false;

            var enemyTarget = (Character) Core.Me.CurrentTarget; 
            if (enemyTarget.SpellCastInfo.RemainingCastTime <= Spells.Succor.AdjustedCastTime)
                return false;

            if (ScholarSettings.Instance.FightLogicAdloDeployBigAoe && 
                Spells.DeploymentTactics.IsKnownAndReady())
            {
                var target = Group.CastableParty.FirstOrDefault(x => x.HasAura(Auras.Catalyze));

                if (target == null) target = Group.CastableParty.FirstOrDefault(x => x.HasAura(Auras.Galvanize));

                if (target == null)
                {
                    target = Core.Me;
                    if (!await Spells.Adloquium.Cast(target))
                        return false;
                }

                return await FightLogic.DoAndBuffer(Spells.DeploymentTactics.Cast(target));
            }

            if (ScholarSettings.Instance.FightLogicRecitSuccorBigAoe && 
                Spells.Recitation.IsKnownAndReady() && 
                !Core.Me.HasAura(Auras.EmergencyTactics) && 
                Group.CastableParty.Count(x => x.HasAura(Auras.Galvanize)) < AoeThreshold)
            {
                if (!await Spells.Recitation.Cast(Core.Me))
                    return false;

                return await FightLogic.DoAndBuffer(Spells.Succor.Cast(Core.Me));
            }

            if (ScholarSettings.Instance.FightLogicSoilBigAoe && 
                Spells.SacredSoil.IsKnownAndReady() &&
                Core.Me.HasAetherflow())
                return await FightLogic.DoAndBuffer(Spells.SacredSoil.Cast(Core.Me));

            if (ScholarSettings.Instance.FightLogicSuccorAoe && 
                Spells.Succor.IsKnownAndReady() && 
                Group.CastableParty.Count(x => x.HasAura(Auras.Galvanize)) < AoeThreshold)
                return await FightLogic.DoAndBuffer(Spells.Succor.Cast(Core.Me));
            
            return false;
        }

        private static async Task<bool> JustAoe()
        {
            if (!ScholarSettings.Instance.FightLogicSuccorAoe) return false;
            
            if (!Spells.Succor.IsKnownAndReady())
                return false;

            var enemyTarget = (Character) Core.Me.CurrentTarget;
            if (enemyTarget.SpellCastInfo.RemainingCastTime <= Spells.Succor.AdjustedCastTime)
            {
                Logger.WriteInfo($"Enemy Target: ${enemyTarget}\t Remaining Cast Time ${enemyTarget.SpellCastInfo.RemainingCastTime}\t Succor Cast Time ${Spells.Succor.AdjustedCastTime}\t Logic Challenge: ${enemyTarget.SpellCastInfo.RemainingCastTime <= Spells.Succor.AdjustedCastTime}");
                return false;
            }

            if (Core.Me.HasAura(Auras.EmergencyTactics))
                return false;

            if (await FightLogic.DoAndBuffer(Spells.Succor.Heal(Core.Me)))
                return await Coroutine.Wait(2500,
                    () => Casting.LastSpell == Spells.Succor || MovementManager.IsMoving);

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

            if (!target.BeingTargetedBy(Core.Me.CurrentTarget))
            {
                while (Group.CastableTanks.Any(r => !r.HasAura(Auras.Galvanize)))
                {
                    await FightLogic.DoAndBuffer(
                        Spells.Adloquium.Heal(Group.CastableTanks.FirstOrDefault(r => !r.HasAura(Auras.Galvanize))));
                    
                    await Coroutine.Yield();
                }

                return true;
            }


            if (ScholarSettings.Instance.FightLogicExcogTank && 
                Spells.Excogitation.IsKnownAndReady() && 
                Core.Me.HasAetherflow() && 
                !target.HasAura(Auras.Excogitation))
                return await FightLogic.DoAndBuffer(Spells.Excogitation.CastAura(target,Auras.Excogitation));


            if (ScholarSettings.Instance.FightLogicAdloTank &&
                Spells.Adloquium.IsKnownAndReady() &&
                !target.HasAura(Auras.Galvanize))
                return await FightLogic.DoAndBuffer(Spells.Adloquium.HealAura(target, Auras.Galvanize));

            return false;
        }
        
        public static int AoeThreshold => PartyManager.NumMembers == 4 ? 2 : 3;
        
    }
}
