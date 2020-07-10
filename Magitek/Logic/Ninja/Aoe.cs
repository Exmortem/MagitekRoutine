using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class Aoe
    {
        public static async Task<bool> DeathBlossom()
        {
            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (!NinjaSettings.Instance.UseDeathBlossom)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < NinjaSettings.Instance.DeathBlossomEnemies)
                return false;

            return await Spells.DeathBlossom.Cast(Core.Me);
        }

        public static async Task<bool> HakkeMujinsatsu()
        {
            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 5 + r.CombatReach) < NinjaSettings.Instance.DeathBlossomEnemies)
                return false;

            if (ActionManager.LastSpell == Spells.DeathBlossom)
                return await Spells.HakkeMujinsatsu.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> HellfrogMedium()
        {
            if (!NinjaSettings.Instance.UseAoe)
                return false;

            if (!NinjaSettings.Instance.UseHellfrogMedium)
                return false;

            if (Core.Me.ClassLevel < 68 && Core.Me.ClassLevel > 62)
                return await Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget);

            if (Combat.Enemies.Count(r => r.Distance(Core.Me.CurrentTarget) <= 6 + r.CombatReach) < 2)
                return false;

            if (ActionResourceManager.Ninja.NinkiGauge >= 100 && Spells.Bunshin.Cooldown.TotalMilliseconds < Spells.TrickAttack.Cooldown.TotalMilliseconds && Spells.TrickAttack.Cooldown.TotalSeconds > 3)
                return await (Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget));

            if (ActionResourceManager.Ninja.NinkiGauge >= 50 && Spells.Mug.Cooldown.TotalMilliseconds < Spells.TrickAttack.Cooldown.TotalMilliseconds + 1000 && Spells.Bunshin.Cooldown.TotalMilliseconds > Spells.TrickAttack.Cooldown.TotalMilliseconds)
                return await (Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget));

            if (NinkiGauge < 90 && Spells.TrickAttack.Cooldown.TotalMilliseconds < 46000)
                return false;


            if (ActionResourceManager.Ninja.NinkiGauge < 50)
                return false;

            if (Spells.Bunshin.Cooldown.TotalMilliseconds < Spells.TrickAttack.Cooldown.TotalMilliseconds)
            {
                //Logger.Write("Bunshin Check");
                int canwesafelycastthis = 0;
                double cooldown = Spells.Bunshin.Cooldown.TotalMilliseconds;
                double gcd = 2100;
                double ninjutsu = Spells.Jin.Cooldown.TotalMilliseconds;
                double ninadjust = 0;

                //Are we going to Ninjutsu before we Bunshin?
                if (cooldown > ninjutsu)
                {
                    ninadjust = 3;
                }

                //Check if we have time to use 3rd skill of combo which gives us 10 ninki instead of 5
                int third = (int)cooldown / 3000;
                //Logger.Write("Third:" + third);

                ////cooldown = cooldown - third;

                //Logger.Write("Cooldown:" + cooldown);

                double calc = cooldown / gcd;

                calc = calc - third;

                third = third - 2;

                //Logger.Write("Calc:" + calc);

                //Logger.Write("First Calc:" + (calc - ninadjust) + "Plus " + third * 10 + "NINKI: " + Utilities.Routines.Ninja.ninki);
                if ((((calc - ninadjust) * 5) + (third * 10) + Utilities.Routines.Ninja.ninki) - 5 >= 90)
                    canwesafelycastthis = 1;

                if (canwesafelycastthis == 1)
                {

                    return await (Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget));
                }
            }
            else
            {
                //Logger.Write("Trick Attack Check");
                int canwesafelycastthis = 0;
                double cooldown = Spells.TrickAttack.Cooldown.TotalMilliseconds;
                double gcd = 2100;
                double ninjutsu = Spells.Jin.Cooldown.TotalMilliseconds;
                double ninadjust = 0;

                //Are we going to Ninjutsu before we TA?
                if (cooldown > ninjutsu)
                {
                    ninadjust = 3;
                }

                //Check if we have time to use 3rd skill of combo which gives us 10 ninki instead of 5
                int third = (int)cooldown / 3000;
                //Logger.Write("Third:" + third);


                //Logger.Write("Cooldown:" + cooldown);

                double calc = cooldown / gcd;

                calc = calc - third;

                third = third - 2;

                //Logger.Write("Calc:" + calc);

                //Logger.Write("First Calc:" + (calc - ninadjust) + "Plus " + third * 10 + "NINKI: " + Utilities.Routines.Ninja.ninki);
                if ((((calc - ninadjust) * 5) + (third * 10) + Utilities.Routines.Ninja.ninki) - 5 > 70)
                    canwesafelycastthis = 1;

                if (canwesafelycastthis == 1)
                {
                    return await (Spells.HellfrogMedium.Cast(Core.Me.CurrentTarget));
                }
            }
            return false;
        }
    }
}
