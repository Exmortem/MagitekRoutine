using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.BlueMage;
using Magitek.Logic.Roles;
using Magitek.Models.BlueMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;


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

            //Dispell Party if necessary
            if (await Dispel.Exuviation()) return true;

            //Self Heal if necessary
            if (await Logic.BlueMage.Heal.SelfCure()) return true;

            //Raise if necessary
            if (await Logic.BlueMage.Heal.AngelWhisper()) return true;

            return await GambitLogic.Gambit();
        }
        
        public static async Task<bool> CombatBuff()
        {
            return false;
        }

        public static async Task<bool> Combat()
        {
            if (await GambitLogic.Gambit()) 
                return true;

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            // Can't attack, so just exit
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            // Can't attack, so just exit
            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;

            // Can't attack, so just exit
            if (Core.Me.HasAura(Auras.WaningNocturne, true, 1000))
                return false;

            //Opener
            if (await CustomOpenerLogic.Opener()) return true;

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            //Interrupt
            if (await MagicDps.Interrupt(BlueMageSettings.Instance)) return true;

            //Manage PhantomFlury 
            ff14bot.Objects.Aura waxingNocturne = Core.Me.Auras.FirstOrDefault(x => x.Id == Auras.WaxingNocturne && x.CasterId == Core.Player.ObjectId);
            ff14bot.Objects.Aura phantomFlurry = Core.Me.Auras.FirstOrDefault(x => x.Id == Auras.PhantomFlurry && x.CasterId == Core.Player.ObjectId);

            if (Core.Me.InCombat && BlueMageSettings.Instance.UsePhantomFlurry && Casting.LastSpell == Spells.PhantomFlurry)
            {    
                if (Core.Me.HasAura(Auras.WaxingNocturne) && waxingNocturne.TimespanLeft.TotalMilliseconds <= 1000)
                    return await Aoe.PhantomFlurryEnd();

                if (Core.Me.HasAura(Auras.PhantomFlurry) && phantomFlurry.TimespanLeft.TotalMilliseconds <= 1000)
                    return await Aoe.PhantomFlurryEnd();

                return true;
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
                    if (Weaving.GetCurrentWeavingCounter() < 2)
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
