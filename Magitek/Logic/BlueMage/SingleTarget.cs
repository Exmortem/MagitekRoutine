using ff14bot;
using Magitek.Extensions;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Magitek.Models.BlueMage;

namespace Magitek.Logic.BlueMage
{
    internal static class SingleTarget
    {
        /*******************************      PHYSIC      *******************************/
        public static async Task<bool> TripleTrident()
        {
            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            if (Core.Me.HasAura(Auras.Boost))
                return false;

            if (Spells.TripleTrident.Cooldown.TotalMilliseconds <= 2500)
            {
                if (!Core.Me.HasAura(Auras.Boost) && !Core.Me.HasAura(Auras.Harmonized))
                    return await Spells.Whistle.Cast(Core.Me);

                return await Spells.TripleTrident.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> SharpKnife()
        {
            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false;

            if (Core.Me.CurrentTarget.Distance(Core.Me) > 5)
                return false;

            return await Spells.SharpKnife.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AbyssalTransfixion()
        {
            if (BlueMageSettings.Instance.UseSonicBoom)
                return false;

            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false;

            return await Spells.AbyssalTransfixion.Cast(Core.Me.CurrentTarget);
        }


        /*******************************      MAGIC      *******************************/
        public static async Task<bool> SonicBoom()
        {
            if (!BlueMageSettings.Instance.UseSonicBoom)
                return false;

            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false;

            return await Spells.SonicBoom.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TheRoseOfDestruction()
        {
            if (Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            if (Spells.TheRoseOfDestruction.Cooldown.TotalMilliseconds <= 2000)
            {
                if (!Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.Boost) && !Core.Me.HasAura(Auras.Harmonized))
                    return await Spells.Bristle.Cast(Core.Me);

                return await Spells.TheRoseOfDestruction.Cast(Core.Me.CurrentTarget);
            }
            return false;
        }

        public static async Task<bool> MatraMagic()
        {
            if (Spells.MatraMagic.Cooldown.TotalMilliseconds <= 2000)
            {
                if (!Utilities.Routines.BlueMage.IsMoonFluteTakenActivatedAndWindowReady && !Core.Me.HasAura(Auras.Boost) && !Core.Me.HasAura(Auras.Harmonized))
                    return await Spells.Bristle.Cast(Core.Me);

                return await Spells.MatraMagic.Cast(Core.Me.CurrentTarget);
            }
            return false;            
        }

        public static async Task<bool> WaterCannon()
        {
            return await Spells.WaterCannon.Cast(Core.Me.CurrentTarget);
        }

        /*******************************      MAGIC DOTs      *******************************/
        public static async Task<bool> SongOfTorment()
        {
            if (Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            if (Casting.LastSpell == Spells.Surpanakha)
                return false;

            if (!Core.Me.HasAura(Auras.Boost))
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.Bleeding, true, 3000))
                return false;

            if (Spells.NightBloom.Cooldown.TotalMilliseconds < 5000)
                return false;

            return await Spells.SongOfTorment.Cast(Core.Me.CurrentTarget);
        }
    }
}