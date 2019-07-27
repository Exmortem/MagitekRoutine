using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using ActionResourceManager = Magitek.Utilities.MagitekActionResourceManager;

namespace Magitek.Logic.DarkKnight
{
    internal static class SingleTarget
    {
        public static async Task<bool> HardSlash()
        {
            return await Spells.HardSlash.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SyphonStrike()
        {
            if (ActionManager.LastSpell != Spells.HardSlash)
                return false;

            // If we have Blood Price or Blood Weapon we wanna Dark Arts this every time

            if (!Core.Me.HasBloodWeapon())
                return await Spells.SyphonStrike.Cast(Core.Me.CurrentTarget);

            return await Spells.SyphonStrike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SoulEater()
        {
            if (ActionManager.LastSpell != Spells.SyphonStrike)
                return false;

            // If we have Blood Price or Blood Weapon we wanna Dark Arts this every time

            if (!Core.Me.HasBloodWeapon())
                return await Spells.Souleater.Cast(Core.Me.CurrentTarget);

            return await Spells.Souleater.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Reprisal()
        {
            if (!DarkKnightSettings.Instance.UseReprisal)
                return false;

            return await Spells.Reprisal.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Plunge()
        {
            if (!DarkKnightSettings.Instance.Plunge)
                return false;

            return await Spells.Plunge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Unmend()
        {
            if (Core.Me.OnPvpMap())
                return false;

            if (!DarkKnightSettings.Instance.UnmendToPullAggro)
                return false;

            if (BotManager.Current.IsAutonomous)
                return false;

            if (!DutyManager.InInstance)
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

        public static async Task<bool> CarveAndSpit()
        {
            if (!DarkKnightSettings.Instance.CarveAndSpit)
                return false;

            if (!ActionManager.HasSpell(Spells.CarveandSpit.Id))
                return false;

            if (Spells.CarveandSpit.Cooldown != TimeSpan.Zero)
                return false;

            return await Spells.CarveandSpit.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Bloodspiller()
        {
            if (!DarkKnightSettings.Instance.Bloodspiller)
                return false;

            if (Core.Me.ClassLevel < 68)
                return false;

            if (ActionResourceManager.DarkKnight.BlackBlood < 50)
                return false;

            if (Spells.Bloodspiller.Cooldown != TimeSpan.Zero)
                return false;

            // We only wanna use this with Dark Arts

            //if (!Utilities.Routines.DarkKnight.CanDarkArts(Spells.Bloodspiller))
            //{
                if (DarkKnightSettings.Instance.Quietus && ActionResourceManager.DarkKnight.BlackBlood > 90)
                {
                    return await Spells.Quietus.Cast(Core.Me);
                }

            //    return false;
            //}

            return await Spells.Bloodspiller.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> LowBlow()
        {
            if (!DarkKnightSettings.Instance.LowBlow)
                return false;

            var currentTargetAsCharacter = Core.Me.CurrentTarget as Character;

            if (currentTargetAsCharacter == null)
                return false;

            if (!currentTargetAsCharacter.IsCasting)
                return false;

            if (!InterruptsAndStunsManager.AllStuns.Contains(currentTargetAsCharacter.CastingSpellId))
                return false;

            return await Spells.LowBlow.Cast(Core.Me.CurrentTarget);
        }
    }
}