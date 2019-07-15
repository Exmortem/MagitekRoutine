using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Paladin
{
    internal static class Defensive
    {
        public static async Task<bool> Defensives()
        {
            if (!PaladinSettings.Instance.UseDefensives)
                return false;

            if (PaladinSettings.Instance.UseDefensivesOnlyOnTankBusters)
                return false;

            if (Core.Me.HasAura(Auras.HallowedGround))
                return false;

            if (PaladinSettings.Instance.HallowGround)
            {
                if (Core.Me.CurrentHealthPercent <= PaladinSettings.Instance.HallowGroundHp)
                {
                    if (await Spells.HallowedGround.CastAura(Core.Me, Auras.HallowedGround)) return true;
                }                  
            }
           
            var currentAuras = Core.Me.CharacterAuras.Select(r => r.Id).Where(r => Utilities.Routines.Paladin.Defensives.Contains(r)).ToList();

            if (currentAuras.Count >= PaladinSettings.Instance.MaxDefensivesAtOnce)
            {
                if (currentAuras.Count >= PaladinSettings.Instance.MaxDefensivesUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= PaladinSettings.Instance.MoreDefensivesHp)
                    return false;
            }
           
            #region Sentinel
            if (PaladinSettings.Instance.Sentinel)
            {
                if (Core.Me.CurrentHealthPercent <= PaladinSettings.Instance.SentinelHp)
                {
                    if (await Spells.Sentinel.CastAura(Core.Me, Auras.Sentinel)) return true;
                }
            }
            #endregion
           
            #region Rampart

            if (!PaladinSettings.Instance.UseRampart)
                return false;
            
            if (Core.Me.CurrentHealthPercent > PaladinSettings.Instance.RampartHpPercentage) 
                return false;
                
            return await Spells.Rampart.CastAura(Core.Me, Auras.Rampart);

            #endregion
        }

        public static DateTime LastTankBusterTime = DateTime.Now;

        public static async Task<bool> TankBusters()
        {
            if (!PaladinSettings.Instance.UseTankBusters)
                return false;

            if (LastTankBusterTime.AddSeconds(5) > DateTime.Now)
                return false;

            if (Core.Me.HasAura(Auras.HallowedGround))
                return false;

            var targetAsCharacter = (Core.Me.CurrentTarget as Character);

            if (targetAsCharacter == null)
                return false;

            var castingSpell = targetAsCharacter.CastingSpellId;
            var targetIsMe = targetAsCharacter.TargetGameObject == Core.Me;

            if (!TankBusterManager.PaladinTankBusters.ContainsKey(castingSpell))
                return false;

            var tankBuster = TankBusterManager.PaladinTankBusters.First(r => r.Key == castingSpell).Value;

            if (tankBuster == null)
                return false;

            if (tankBuster.HallowedGround && targetIsMe && await Spells.HallowedGround.CastAura(Core.Me, Auras.HallowedGround))
            {
                LastTankBusterTime = DateTime.Now;
                return true;
            }

            if (tankBuster.DivineVeil && targetIsMe && await Spells.DivineVeil.Cast(Core.Me))
            {
                LastTankBusterTime = DateTime.Now;
                return true;
            }

            if (tankBuster.Sheltron && targetIsMe && ActionResourceManager.Paladin.Oath >= 50 && await Spells.Sheltron.Cast(Core.Me))
            {
                LastTankBusterTime = DateTime.Now;
                return true;
            }

            if (tankBuster.Sentinel && targetIsMe && await Spells.Sentinel.CastAura(Core.Me, Auras.Sentinel))
            {
                LastTankBusterTime = DateTime.Now;
                return true;
            }

            if (tankBuster.Reprisal && await Spells.Reprisal.CastAura(Core.Me.CurrentTarget, Auras.Reprisal))
            {
                LastTankBusterTime = DateTime.Now;
                return true;
            }

            if (tankBuster.Rampart && targetIsMe && await Spells.Rampart.CastAura(Core.Me, Auras.Rampart))
            {
                LastTankBusterTime = DateTime.Now;
                return true;
            }

            return false;
        }
    }
}