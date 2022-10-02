using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.DarkKnight;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using DarkKnightRoutine = Magitek.Utilities.Routines.DarkKnight;

namespace Magitek.Logic.DarkKnight
{
    internal static class Defensive
    {
        public static async Task<bool> Execute()
        {
            if (!DarkKnightSettings.Instance.UseDefensives)
                return false;

            if (Core.Me.HasAura(Auras.LivingDead))
                return false;

            if (await LivingDead()) return true;

            var currentAuras = Core.Me.CharacterAuras
                .Select(r => r.Id)
                .Where(r => DarkKnightRoutine.Defensives.Contains(r))
                .ToList();

            if (currentAuras.Count >= DarkKnightSettings.Instance.MaxDefensivesAtOnce)
            {
                if (currentAuras.Count >= DarkKnightSettings.Instance.MaxDefensivesUnderHp)
                    return false;

                if (Core.Me.CurrentHealthPercent >= DarkKnightSettings.Instance.MoreDefensivesHp)
                    return false;
            }

            if (await DarkMind()) return true;
            if (await ShadowWall()) return true;
            if (await TheBlackestNight()) return true;
            if (await DarkMissionary()) return true;
            if (await Oblation(false)) return true;
            if (await Rampart()) return true;

            return false;
        }

        public static async Task<bool> Oblation(bool excludeSelf)
        {
            if (!DarkKnightSettings.Instance.UseOblation)
                return false;

            var target = Group.CastableAlliesWithin30
                .Where(CanOblation)
                .OrderByDescending(DispelManager.GetWeight)
                .ThenBy(c => c.CurrentHealthPercent)
                .ToList()
                .FirstOrDefault();

            if (target == null)
                return false;

            return await Spells.Oblation.CastAura(Core.Me, Auras.Oblation);

            bool CanOblation(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.HasAura(Auras.Oblation))
                    return false;

                if (unit.CurrentHealthPercent > DarkKnightSettings.Instance.UseOblationAtHpPercent)
                    return false;

                if (unit.IsMe && !excludeSelf)
                    return true;

                if (DarkKnightSettings.Instance.UseOblationOnTanks && unit.IsTank())
                    return true;

                if (DarkKnightSettings.Instance.UseOblationOnHealers && unit.IsHealer())
                    return true;

                if (DarkKnightSettings.Instance.UseOblationOnDPS && unit.IsDps())
                    return true;

                return false;
            }
        }

        private static async Task<bool> LivingDead()
        {
            if (!DarkKnightSettings.Instance.UseLivingDead)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.LivingDeadHealth)
                return false;

            return await Spells.LivingDead.CastAura(Core.Me, Auras.LivingDead);
        }

        private static async Task<bool> DarkMind()
        {
            if (!DarkKnightSettings.Instance.UseDarkMind)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.DarkMindHealth)
                return false;

            return await Spells.DarkMind.CastAura(Core.Me, Auras.DarkMind);
        }

        private static async Task<bool> ShadowWall()
        {
            if (!DarkKnightSettings.Instance.UseShadowWall)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.ShadowWallHealth)
                return false;

            return await Spells.ShadowWall.CastAura(Core.Me, Auras.ShadowWall);
        }

        public static async Task<bool> TheBlackestNight()
        {
            if (!DarkKnightSettings.Instance.UseTheBlackestNight)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.TheBlackestNightHealth)
                return false;

            if (ActionResourceManager.DarkKnight.BlackBlood < 50)
                return false;

            if (Core.Me.CurrentMana < DarkKnightSettings.Instance.SaveXMana + 3000)
                return false;

            return await Spells.TheBlackestNight.CastAura(Core.Me, Auras.BlackestNight);
        }

        private static async Task<bool> DarkMissionary()
        {
            if (!DarkKnightSettings.Instance.UseDarkMissionary)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.DarkMissionaryHealth)
                return false;

            return await Spells.DarkMissionary.CastAura(Core.Me, Auras.DarkMissionary);
        }

        private static async Task<bool> Rampart()
        {
            if (!DarkKnightSettings.Instance.UseRampart)
                return false;

            if (Core.Me.CurrentHealthPercent > DarkKnightSettings.Instance.RampartHpPercentage)
                return false;

            return await Spells.Rampart.CastAura(Core.Me, Auras.Rampart);
        }

        public static async Task<bool> Reprisal()
        {
            if (!DarkKnightSettings.Instance.UseReprisal)
                return false;

            return await Spells.Reprisal.Cast(Core.Me.CurrentTarget);
        }

        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            return Tank.ForceLimitBreak(DarkKnightSettings.Instance, Spells.ShieldWall, Spells.Stronghold, Spells.DarkForce, Spells.HardSlash);
        }
    }
}