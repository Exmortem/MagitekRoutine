using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;
using System.Linq;
using System.Threading.Tasks;
using BlackMageRoutine = Magitek.Utilities.Routines.BlackMage;

namespace Magitek.Logic.BlackMage
{
    internal static class Pvp
    {
        public static async Task<bool> Fire()
        {

            if(!Spells.FirePvp.CanCast())
                return false;

            if (!BlackMageSettings.Instance.Pvp_ToggleFireOrIceCombo)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if(Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.FirePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Blizzard()
        {

            if (!Spells.BlizzardPvp.CanCast())
                return false;

            if (BlackMageSettings.Instance.Pvp_ToggleFireOrIceCombo)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.BlizzardPvp.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> Burst()
        {

            if (!Spells.BurstPvp.CanCast())
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < 1)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.BurstPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Paradox()
        {

            if (!Spells.ParadoxPvp.CanCast())
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (BlackMageSettings.Instance.Pvp_UseParadoxOnFire && Core.Me.CurrentTarget.HasAura(Auras.AstralWarmth))
                return await Spells.ParadoxPvp.Cast(Core.Me.CurrentTarget);

            if (BlackMageSettings.Instance.Pvp_UseParadoxOnIce && Core.Me.CurrentTarget.HasAura(Auras.UmbralFreeze))
                return await Spells.ParadoxPvp.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> SuperFlare()
        {

            if (!Spells.SuperFlarePvp.CanCast())
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 30 + x.CombatReach) < 1)
                return false;

            if (Spells.ParadoxPvp.IsKnownAndReady()) 
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (BlackMageSettings.Instance.Pvp_UseSuperFlareOnFire && Core.Me.CurrentTarget.HasAura(Auras.AstralWarmth))
                return await Spells.SuperFlarePvp.Cast(Core.Me.CurrentTarget);

            if (BlackMageSettings.Instance.Pvp_UseSuperFlareOnIce && Core.Me.CurrentTarget.HasAura(Auras.UmbralFreeze))
                return await Spells.SuperFlarePvp.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> AetherialManipulation()
        {

            if (!Spells.AetherialManipulationPvp.CanCast())
                return false;

            if (!BlackMageSettings.Instance.Pvp_UseAetherialManipulation)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.DeepFreeze))
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            if (Core.Me.CurrentHealthPercent < BlackMageSettings.Instance.Pvp_UseAetherialManipulationtHealthPercent)
                return false;

            return await Spells.AetherialManipulationPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FoulPvp()
        {

            if (!Spells.FoulPvp.CanCast())
                return false;

            if (!BlackMageSettings.Instance.Pvp_SoulResonance)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Core.Me.CurrentTarget.ValidAttackUnit() || !Core.Me.CurrentTarget.InLineOfSight())
                return false;

            return await Spells.FoulPvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SoulResonancePvp()
        {

            if (!Spells.SoulResonancePvp.CanCast())
                return false;

            if (!BlackMageSettings.Instance.Pvp_SoulResonance)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            return await Spells.SoulResonancePvp.Cast(Core.Me);
        }
    }
}
