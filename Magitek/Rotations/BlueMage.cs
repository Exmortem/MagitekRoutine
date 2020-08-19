using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Magitek.Logic.BlueMage;


namespace Magitek.Rotations
{
    public static class BlueMage
    {
        public static async Task<bool> Rest()
        {
            return Core.Me.CurrentHealthPercent < 75 || Core.Me.CurrentManaPercent < 50;
        }

        public static async Task<bool> PreCombatBuff()
        {


            await Casting.CheckForSuccessfulCast();


            return false;
        }

        public static async Task<bool> Pull()
        {
            if (!BotManager.Current.IsAutonomous)
            {
                return await Combat();
            }

            Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);

            return await Combat();
        }
        public static async Task<bool> Heal()
        {


            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            return await GambitLogic.Gambit();
        }
        public static async Task<bool> CombatBuff()
        {
            return false;
        }
        public static async Task<bool> Combat()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await GambitLogic.Gambit()) return true;

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;
            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!Models.BlueMage.BlueMageSettings.Instance.OnlyWaterCannon)
            { 
                if (await Buff.OffGuard()) return true;
                if (await SingleTarget.Surpanakha()) return true;
                if (await SingleTarget.SongOfTorment()) return true;
                if (Utilities.Routines.BlueMage.OnGcd)
                {
                    if (await SingleTarget.Eruption()) return true;
                    if (await SingleTarget.GlassDance()) return true;
                    if (await SingleTarget.ShockStrike()) return true;
                    if (await SingleTarget.Quasar()) return true;
                }

                if (await SingleTarget.SharpKnife()) return true;
                return await SingleTarget.AbyssalTransfixion();
            }
        return await SingleTarget.WaterCannon();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
