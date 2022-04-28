using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Ninja;
using Magitek.Logic.Roles;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using NinjaRoutine = Magitek.Utilities.Routines.Ninja;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class Ninja
    {
        public static Task<bool> Rest()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> PreCombatBuff()
        {


            await Casting.CheckForSuccessfulCast();

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!NinjaSettings.Instance.UseHutonOutsideOfCombat)
                return false;

            if (NinjaSettings.Instance.UseHutonOutsideOfCombat)
            {
                if (WorldManager.InSanctuary)
                    return false;
            }

            return Ninjutsu.Huton();
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            return await Combat();
        }
        public static async Task<bool> Heal()
        {


            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            return await GambitLogic.Gambit();
        }
        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }
        public static async Task<bool> Combat()
        {
            Utilities.Routines.Ninja.RefreshVars();
            //Logger.Write("Ninki:" + Utilities.Routines.Ninja.ninki);

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
                }
            }

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await PhysicalDps.Interrupt(NinjaSettings.Instance)) return true;

            if (Ninjutsu.ForceRaiton()) return true;
            if (NinjaRoutine.GlobalCooldown.CanWeave(1))
            {
                //Utility Force Toggle
                if (await PhysicalDps.ArmsLength(NinjaSettings.Instance)) return true;
                if (await Utility.SecondWindForce()) return true;
                if (await Utility.ShadeShift()) return true;
                if (await Utility.TrueNorth()) return true;
                if (await Utility.Feint()) return true;
                if (await Utility.BloodBath()) return true;

                if (await PhysicalDps.SecondWind(NinjaSettings.Instance)) return true;

                if (await SingleTarget.TrickAttack()) return true;
                if (await SingleTarget.Bhavacakra()) return true;
                if (await SingleTarget.Mug()) return true;
                if (await Aoe.HellfrogMedium()) return true;
                if (await Ninjutsu.TenChiJin()) return true;
                if (await Buff.Meisui()) return true;
                if (await Buff.TrueNorth()) return true;
                if (await Buff.Bunshin()) return true;
                if (await Buff.Kassatsu()) return true;
                if (await SingleTarget.DreamWithinADream()) return true;
                if (await SingleTarget.Assassinate()) return true;
            }

            //Ninjutsu
            if (NinjutsuCheck())
            {
                if (Ninjutsu.Huton()) return true;
                if (Ninjutsu.GokaMekkyaku()) return true;
                if (Ninjutsu.Doton()) return true;
                if (Ninjutsu.Katon()) return true;
                if (Ninjutsu.Suiton()) return true;
                if (Ninjutsu.HyoshoRanryu()) return true;
                if (Ninjutsu.Raiton()) return true;
                //if (Ninjutsu.FumaShuriken()) return true;
            }

            if (await SingleTarget.FleetingRaiju()) return true;
            if (await Aoe.PhantomKamaitachi()) return true;
            if (await Aoe.HakkeMujinsatsu()) return true;
            if (await Aoe.DeathBlossom()) return true;
            if (await SingleTarget.ArmorCrush()) return true;
            if (await SingleTarget.AeolianEdge()) return true;
            if (await SingleTarget.GustSlash()) return true;
            return await SingleTarget.SpinningEdge();
        }
        public static Task<bool> PvP()
        {
            return Task.FromResult(false);
        }

        public static bool NinjutsuCheck()
        {
            if (Spells.TenChiJin.Cooldown.TotalMilliseconds < 5000 && Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack))
            {
                //Logger.Write("TCJ Test");


                if (Core.Me.HasAura(Auras.Kassatsu))
                    return true;

                // TCJ Check during TA to Fix Double Raiton
                if (Spells.Ten.Charges >= 1 && Spells.Ten.Cooldown.TotalMilliseconds <= 8000)
                    return true;

                //if (Casting.SpellCastHistory.Take(5).All(s => s.Spell == Spells.Raiton) /*&& Spells.TenChiJin.Cooldown.TotalMilliseconds < 5000*/)
                //    return false;

                //if (!Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack) && Core.Me.HasAura(Auras.Suiton) && Spells.TrickAttack.Cooldown.TotalMilliseconds < 1000)
                //    return true;
            }
            else
            {
                //Logger.Write("OH FUCK");
                //Logger.Write("Test: " + Spells.TenChiJin.Cooldown.TotalMilliseconds);
                if (NinjaSettings.Instance.UseForceNinjutsu)
                    return true;

                if (Core.Me.HasAura(Auras.Kassatsu))
                    return true;

                if (Spells.TrickAttack.Cooldown.TotalMilliseconds > 45000)
                    return true;

                if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 12000 && !Core.Me.HasAura(Auras.Suiton) || Spells.Jin.Charges >= 1 && Spells.Jin.Cooldown.TotalMilliseconds < 5000)
                    return true;
            }
            return false;
        }
    }
}
