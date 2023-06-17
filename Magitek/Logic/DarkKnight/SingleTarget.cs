using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.DarkKnight
{
    internal static class SingleTarget
    {
        public static async Task<bool> HardSlash()
        {

            if (Core.Me.HasAura(Auras.Delirium))
                return false;

            return await Spells.HardSlash.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SyphonStrike()
        {
            if (ActionManager.LastSpell != Spells.HardSlash)
                return false;

            if (Core.Me.HasAura(Auras.Delirium))
                return false;

            return await Spells.SyphonStrike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SoulEater()
        {
            if (ActionManager.LastSpell != Spells.SyphonStrike)
                return false;

            if (Core.Me.HasAura(Auras.Delirium))
                return false;

            return await Spells.Souleater.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Bloodspiller()
        {
            if (!DarkKnightSettings.Instance.UseBloodspiller)
                return false;

            return await Spells.Bloodspiller.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> Shadowbringer()
        {
            if (!DarkKnightSettings.Instance.UseShadowbringer)
                return false;

            return await Spells.Shadowbringer.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EdgeofDarknessShadow()
        {

            if (Core.Me.HasDarkArts())
                return await Spells.EdgeofDarkness.Cast(Core.Me.CurrentTarget);

            if (Core.Me.CurrentMana < DarkKnightSettings.Instance.SaveXMana + 3000)
                return false;

            if (Core.Me.CurrentMana < 6000 && DarkKnightSettings.Instance.UseTheBlackestNight)
                return false;

            return await Spells.EdgeofDarkness.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CarveAndSpit()
        {
            if (!DarkKnightSettings.Instance.UseCarveAndSpit)
                return false;

            if (DarkKnightSettings.Instance.UseCarveOnlyWithBloodWeapon && !Core.Me.HasAura(Auras.BloodWeapon))
                return false;

            return await Spells.CarveandSpit.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Plunge()
        {
            if (!DarkKnightSettings.Instance.UsePlunge)
                return false;

            if (DarkKnightSettings.Instance.PlungeOnlyInMelee
                && Core.Me.CurrentTarget != null
                && !Core.Me.CurrentTarget.WithinSpellRange(3))
            {
                return false;
            }

            if (Spells.Plunge.Charges < DarkKnightSettings.Instance.SavePlungeCharges + 1)
            {
                return false;
            }

            return await Spells.Plunge.Cast(Core.Me.CurrentTarget);
        }

        /*********************************************************************
         *                         Unmend
         *********************************************************************/

        public static async Task<bool> UnmendToDps()
        {
            if (!DarkKnightSettings.Instance.UnmendToDps)
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit()
                        || !Core.Me.CurrentTarget.NotInvulnerable()
                        || Core.Me.CurrentTarget.TimeInCombat() <= 0
                        || Core.Me.CurrentTarget.Distance(Core.Me) < Core.Me.CombatReach + Core.Me.CurrentTarget.CombatReach + DarkKnightSettings.Instance.UnmendMinDistance
                        || Core.Me.CurrentTarget.Distance(Core.Me) > 20 + Core.Me.CurrentTarget.CombatReach
                        || (Core.Me.CurrentTarget as BattleCharacter).TargetGameObject == null)
                return false;

            if (!await Spells.Unmend.Cast(Core.Me.CurrentTarget))
                return false;

            Logger.WriteInfo($@"Unmend On {Core.Me.CurrentTarget.Name} To DPS");
            return true;
        }


        public static async Task<bool> UnmendToPullOrAggro()
        {
            if (!DarkKnightSettings.Instance.UnmendToPullOrAggro)
                return false;

            if (Core.Me.HasAura(Auras.Delirium))
                return false;

            // If we're in AOE situation we're going to likely use Unleash which has a 5y range
            // but if we're not then we're going to melee which has a 0.66 (CombatReach) range
            // This extra bit of complexity helps make this do the right thing in more scenarios
            var enemyCount = Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach);
            var enoughEnemies = enemyCount >= DarkKnightSettings.Instance.UnleashEnemies;
            var calculatedCombatReach = (DarkKnightSettings.Instance.UseUnleash && enoughEnemies)
                ? 5
                : Core.Me.CombatReach + DarkKnightSettings.Instance.UnmendMinDistance;

            //find target already pulled on which I lose aggro
            var unmendTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit()
                                                                    && r.NotInvulnerable()
                                                                    && r.Distance(Core.Me) >= calculatedCombatReach + r.CombatReach
                                                                    && r.Distance(Core.Me) <= 20 + r.CombatReach
                                                                    && r.TargetGameObject != Core.Me);

            if (unmendTarget == null)
            {
                unmendTarget = (BattleCharacter)Core.Me.CurrentTarget;

                if (!unmendTarget.ValidAttackUnit()
                    || !unmendTarget.NotInvulnerable()
                    || unmendTarget.Distance(Core.Me) < Core.Me.CombatReach + unmendTarget.CombatReach + DarkKnightSettings.Instance.UnmendMinDistance
                    || unmendTarget.Distance(Core.Me) > 20 + unmendTarget.CombatReach
                    || unmendTarget.TargetGameObject != null)
                    return false;
            }

            if (!await Spells.Unmend.Cast(unmendTarget))
                return false;

            Logger.WriteInfo($@"Unmend On {unmendTarget.Name} to pull or get back aggro");
            return true;
        }
    }
}