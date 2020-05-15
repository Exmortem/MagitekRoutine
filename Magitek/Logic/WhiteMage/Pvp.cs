using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.WhiteMage
{
    internal static class Pvp
    {
        public static async Task<bool> Cure()
        {
            if (MovementManager.IsMoving)
                return false;

            foreach (var ally in Group.CastableAlliesWithin30)
            {
                if (Utilities.Routines.WhiteMage.DontCure.Contains(ally.Name))
                    continue;

                if (ally.CurrentHealthPercent > WhiteMageSettings.Instance.CurePvpHealthPercent || ally.CurrentHealth <= 0)
                    continue;

                return await Spells.CurePvp.Heal(ally);
            }

            return false;
        }

        public static async Task<bool> Cure2()
        {
            if (MovementManager.IsMoving)
                return false;

            foreach (var ally in Group.CastableAlliesWithin30)
            {
                if (Utilities.Routines.WhiteMage.DontCure2.Contains(ally.Name))
                    continue;

                if (ally.CurrentHealthPercent > WhiteMageSettings.Instance.Cure2PvpHealthPercent || ally.CurrentHealth <= 0)
                    continue;

                return await Spells.Cure2Pvp.Heal(ally);
            }

            return false;
        }

        public static async Task<bool> Regen()
        {
            if (!WhiteMageSettings.Instance.RegenPvp)
                return false;

            var regenTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealth > 0 &&
                                                                               r.CurrentHealthPercent < WhiteMageSettings.Instance.RegenPvpHealthPercent &&
                                                                               !r.HasAura(Auras.RegenPvp));

            if (regenTarget == null)
                return false;

            return await Spells.RegenPvp.HealAura(regenTarget, Auras.RegenPvp);
        }

        public static async Task<bool> Benediction()
        {
            if (!WhiteMageSettings.Instance.BenedictionPvp)
                return false;

            var benedictionTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealth > 0 &&
                                                                               r.CurrentHealthPercent < WhiteMageSettings.Instance.BenedictionPvpHealthPercent);

            if (benedictionTarget == null)
                return false;

            return await Spells.RegenPvp.Heal(benedictionTarget);
        }

        public static async Task<bool> DivineBenison()
        {
            if (!WhiteMageSettings.Instance.DivineBenisonPvp)
                return false;

            if (ActionResourceManager.WhiteMage.Lily < WhiteMageSettings.Instance.DivineBenisonPvpMinimumLillies)
                return false;

            var divineBenisonTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealth > 0 &&
                                                                                       r.CurrentHealthPercent < WhiteMageSettings.Instance.DivineBenisonPvpHealthPercent &&
                                                                                       !r.HasAura(Auras.DivineBenisonPvp));

            if (divineBenisonTarget == null)
                return false;

            return await Spells.DivineBenisonPvp.CastAura(divineBenisonTarget, Auras.DivineBenisonPvp);
        }

        public static async Task<bool> Stone3()
        {
            if (!WhiteMageSettings.Instance.StonePvp)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.CanAttack)
                return false;

            return await Spells.Stone3Pvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Assize()
        {
            if (!WhiteMageSettings.Instance.AssizePvp)
                return false;

            if (ActionResourceManager.WhiteMage.Lily < WhiteMageSettings.Instance.AssizePvpMinimumLillies)
                return false;

            if (WhiteMageSettings.Instance.AssizePvpForMana)
            {
                if (Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.AssizePvpMinimumManaToAssize)
                {
                    return await Spells.AssizePvp.Cast(Core.Me);
                }
            }

            var canAssize = Group.CastableAlliesWithin30.Count(r => r.CurrentHealth > 0 &&
                                                                    r.CurrentHealthPercent < WhiteMageSettings.Instance.AssizePvpHealthPercent &&
                                                                    r.Distance(Core.Me) <= 15) >= WhiteMageSettings.Instance.AssizePvpAllies;

            if (!canAssize)
                return false;

            return await Spells.AssizePvp.Cast(Core.Me);
        }

        public static async Task<bool> FluidAura()
        {
            if (!WhiteMageSettings.Instance.FluidAuraPvp)
                return false;

            var fluidAuraTarget = GameObjectManager.GetObjectsOfType<BattleCharacter>().FirstOrDefault(r => r.CanAttack && r.TargetCharacter == Core.Me && r.InLineOfSight() && r.Distance(Core.Me) <= 5);

            if (fluidAuraTarget == null)
                return false;
            return await Spells.FluidAuraPvp.Cast(fluidAuraTarget);
        }

        public static async Task<bool> Safeguard()
        {
            if (!WhiteMageSettings.Instance.UseSafeguard)
                return false;

            if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.SafeguardHealthPercent)
                return false;

            return await Spells.Safeguard.Cast(Core.Me);
        }

        public static async Task<bool> Muse()
        {
            if (!WhiteMageSettings.Instance.UseMuse)
                return false;

            if (Core.Me.CurrentManaPercent > WhiteMageSettings.Instance.MuseManaPercent)
                return false;

            return await Spells.Muse.Cast(Core.Me);
        }
    }
}
