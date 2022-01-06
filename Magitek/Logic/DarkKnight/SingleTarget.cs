using ff14bot;
using ff14bot.Managers;
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

        public static async Task<bool> UnmendForAggro()
        {
            if (Globals.OnPvpMap)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            if (!DarkKnightSettings.Instance.UnmendToPullAggro)
                return false;

            if (Core.Me.HasAura(Auras.Delirium))
                return false;

            var unmendTarget = Combat.Enemies.FirstOrDefault(r =>
                r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach
                && r.Distance(Core.Me) <= 20 + r.CombatReach
                && !r.TargetGameObject.IsMe);

            if (unmendTarget == null)
                return false;

            if (unmendTarget.TargetGameObject == null)
                return false;

            if (await Spells.Unmend.Cast(unmendTarget))
            {
                Logger.Write($@"Unmend On {unmendTarget.Name} To Pull Aggro");
                return true;
            }

            return false;
        }

        public static async Task<bool> Unmend()
        {
            if (!DarkKnightSettings.Instance.UnmendWhenOutOfMelee)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (await Spells.Unmend.Cast(Core.Me.CurrentTarget))
            {
                Logger.Write($@"Unmend On {Core.Me.CurrentTarget.Name} because I'm out of melee range");
                return true;
            }

            return false;
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

            return await Spells.Plunge.Cast(Core.Me.CurrentTarget);
        }
    }
}