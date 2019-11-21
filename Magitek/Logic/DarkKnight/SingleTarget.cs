using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;
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

            if (ActionResourceManager.DarkKnight.BlackBlood >= 50 || Core.Me.HasAura(Auras.Delirium))
                return await Spells.Bloodspiller.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Unmend()
        {
            if (Globals.OnPvpMap)
                return false;

            if (Core.Me.HasAura(Auras.Delirium))
                return false;

            if (!DarkKnightSettings.Instance.UnmendToPullAggro)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            var unmendTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach &&
                                                                  r.Distance(Core.Me) <= 15 + r.CombatReach &&
                                                                  r.TargetGameObject != Core.Me);

            if (unmendTarget == null)
                return false;

            if (unmendTarget.TargetGameObject == null)
                return false;

            if (!await Spells.Unmend.Cast(unmendTarget))
                return false;

            Logger.Write($@"Unmend On {unmendTarget.Name} To Pull Aggro");
            return true;
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

        public static async Task<bool> Reprisal()
        {
            if (!DarkKnightSettings.Instance.UseReprisal)
                return false;

            return await Spells.Reprisal.Cast(Core.Me.CurrentTarget);
        }
    }
}