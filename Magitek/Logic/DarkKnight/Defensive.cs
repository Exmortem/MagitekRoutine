using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.DarkKnight
{
    internal static class Defensive
    {
        public static async Task<bool> ExecuteTankBusters()
        {
            if (!DarkKnightSettings.Instance.UseTankBusters)
                return false;

            var targetAsCharacter = (Core.Me.CurrentTarget as Character);

            if (targetAsCharacter == null)
                return false;

            var castingSpell = targetAsCharacter.CastingSpellId;
            var targetIsMe = targetAsCharacter.TargetGameObject == Core.Me;

            if (!TankBusterManager.DarkKnightTankBusters.ContainsKey(castingSpell))
                return false;

            var tankBuster = TankBusterManager.DarkKnightTankBusters.First(r => r.Key == castingSpell).Value;

            if (tankBuster == null)
                return false;

            if (tankBuster.ShadowWall && targetIsMe && await Spells.ShadowWall.CastAura(Core.Me, Auras.ShadowWall)) return true;
            if (tankBuster.DarkMind && targetIsMe && await Spells.DarkMind.CastAura(Core.Me, Auras.DarkMind)) return true;
            if (tankBuster.ReprisalDk && await Spells.Reprisal.CastAura(Core.Me.CurrentTarget, Auras.Reprisal)) return true;
            return tankBuster.RampartDk && targetIsMe && await Spells.Rampart.CastAura(Core.Me, Auras.Rampart);
        }
        
        public static async Task<bool> Execute()
        {
            if (!DarkKnightSettings.Instance.UseDefensives)
                return false;
            
            if (DarkKnightSettings.Instance.UseDefensivesOnlyOnTankBusters)
                return false;
            
            if (Core.Me.HasAura(Auras.LivingDead))
                return false;

            if (await LivingDead()) return true;

            var currentAuras = Core.Me.CharacterAuras.Select(r => r.Id).Where(r => Utilities.Routines.Paladin.Defensives.Contains(r)).ToList();

            if (currentAuras.Count >= DarkKnightSettings.Instance.MaxDefensivesAtOnce)
            {
                if (currentAuras.Count >= DarkKnightSettings.Instance.MaxDefensivesUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= DarkKnightSettings.Instance.MoreDefensivesHp)
                    return false;
            }

            if (await ShadowWall()) return true;
            if (await Rampart()) return true;
            return await Rampart();
        }
        
        private static async Task<bool> LivingDead()
        {
            if (!DarkKnightSettings.Instance.LivingDead)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.LivingDeadHealth)
                return false;
            
            return await Spells.LivingDead.CastAura(Core.Me, Auras.LivingDead);
        }
                
        private static async Task<bool> ShadowWall()
        {
            if (!DarkKnightSettings.Instance.ShadowWall)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.ShadowWallHealth)
                return false;
            
            return await Spells.ShadowWall.CastAura(Core.Me, Auras.ShadowWall);
        }

        private static async Task<bool> Rampart()
        {
            if (!DarkKnightSettings.Instance.UseRampart)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.RampartHpPercentage)
                return false;
            
            return await Spells.Rampart.CastAura(Core.Me, Auras.Rampart);
        }
        public static async Task<bool> TheBlackestNight()
        {
            if (!DarkKnightSettings.Instance.UseTheBlackestNight)
                return false;

            if (!ActionManager.HasSpell(Spells.TheBlackestNight.Id))
                return false;

            if (ActionResourceManager.DarkKnight.BlackBlood < 50)
                return false;

            return await Spells.TheBlackestNight.CastAura(Core.Me, Auras.BlackestNight);
        }
    }
}