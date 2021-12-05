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
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (Core.Me.HasAura(Auras.WaningNocturne, true, 1000))
                return false;

            //Manage PhantomFlury 
            if (Casting.LastSpell == Spells.PhantomFlurry)
            {
                if ((Core.Me.HasAura(Auras.WaxingNocturne, true, 1000) && Core.Me.HasAura(Auras.PhantomFlurry, true, 1000)) || !Core.Me.HasAura(Auras.PhantomFlurry))
                {
                    return true;
                } else
                {
                    return await Aoe.PhantomFlurryEnd();
                }
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            //Buff should always be active
            if (await Buff.OffGuard()) return true;

            //Buff
            if (await Buff.Whistle()) return true;
            if (await Aoe.Tingle()) return true;
            if (await Buff.Bristle()) return true;
            if (await Buff.MoonFlute()) return true;

            //Jump
            if (await Aoe.JKick()) return true;

            //DOT should always be active
            if (await SingleTarget.SongOfTorment()) return true;

            if (Casting.LastSpell != Spells.Surpanakha || (Casting.LastSpell == Spells.Surpanakha && Spells.Surpanakha.Charges < 1) ) {

                //GCD
                if (await Buff.Swiftcast()) return true;
                if (await SingleTarget.TripleTrident()) return true;
                if (await SingleTarget.MatraMagic()) return true;
                if (await SingleTarget.TheRoseOfDestruction()) return true;
                if (await Aoe.Surpanakha()) return true;

                if (Utilities.Routines.BlueMage.OnGcd && Casting.LastSpell != Spells.Surpanakha)
                {
                    //put oGCD here
                    if (await Aoe.NightBloom()) return true;
                    if (await Aoe.PhantomFlurry()) return true;
                    if (await Aoe.ShockStrike()) return true;
                    if (await Aoe.GlassDance()) return true;
                    if (await Aoe.Quasar()) return true;
                    if (await Aoe.FeatherRain()) return true;
                    if (await Buff.LucidDreaming()) return true;

                    if (await Aoe.Eruption()) return true; //FeatherRain if possible, otherwise Eruption
                    if (await Aoe.MountainBuster()) return true; //ShockStrike if possible, otherwise MountainBuster
                }

                if (await SingleTarget.SharpKnife()) return true; //if melee
                if (await SingleTarget.AbyssalTransfixion()) return true; //if SonicBoom deactivated

                return await SingleTarget.SonicBoom();
            } 
            else
            {
                //Logger.WriteInfo($@"[Surpanakha] Charges = {Spells.Surpanakha.Charges}");
                return await Aoe.Surpanakha();   
            }
        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
