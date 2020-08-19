using ff14bot;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.BlueMage
{
    internal static class SingleTarget
    {

        public static async Task<bool> SharpKnife()
        {
            if (Casting.LastSpell == Spells.Surpanakha)
            {
                if (Spells.Surpanakha.Charges >= 1.0f)
                    return false;

                return await Spells.SharpKnife.Cast(Core.Me.CurrentTarget);
            }

            return await Spells.SharpKnife.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Surpanakha()
        {
            if (Spells.Surpanakha.Charges < 4 && Casting.LastSpell != Spells.Surpanakha)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.OffGuard))
                return false;

            return await Spells.Surpanakha.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Eruption()
        {
            return await Spells.Eruption.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GlassDance()
        {
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 10 + r.CombatReach) < 1)
                return false;

            return await Spells.GlassDance.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Quasar()
        {
            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 12 + r.CombatReach) < 1)
                return false;

            return await Spells.Quasar.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> WaterCannon()
        {
            return await Spells.WaterCannon.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShockStrike()
        {
            return await Spells.ShockStrike.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> AbyssalTransfixion()
        {
            if (Casting.LastSpell == Spells.Surpanakha)
            {
                if (Spells.Surpanakha.Charges >= 1.0f)
                    return false;

                return await Spells.AbyssalTransfixion.Cast(Core.Me.CurrentTarget);
            }
            return await Spells.AbyssalTransfixion.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> SongOfTorment()
        {
            if (Casting.LastSpell == Spells.Surpanakha)
                return false;

            if(!Core.Me.CurrentTarget.HasAura(Auras.Bleeding, true, 6000))
            {
                if (!Core.Me.HasAura(Auras.Boost))
                    return await Spells.Bristle.Cast(Core.Me);

                if (Core.Me.HasAura(Auras.Boost))
                    return await Spells.SongOfTorment.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

    }
}