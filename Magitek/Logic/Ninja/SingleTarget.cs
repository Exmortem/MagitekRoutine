using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Ninja;
using Magitek.Utilities;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Ninja;

namespace Magitek.Logic.Ninja
{
    internal static class SingleTarget
    {
        public static async Task<bool> SpinningEdge()
        {
            return await Spells.SpinningEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GustSlash()
        {
            if (ActionManager.LastSpell != Spells.SpinningEdge)
                return false;

            return await Spells.GustSlash.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> AeolianEdge()
        {
            if (ActionManager.LastSpell != Spells.GustSlash)
                return false;

            return await Spells.AeolianEdge.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ArmorCrush()
        {
            if (ActionManager.LastSpell != Spells.GustSlash)
                return false;

            if (HutonTimer.TotalSeconds < NinjaSettings.Instance.HutonRefreshTimer)
                return await Spells.ArmorCrush.Cast(Core.Me.CurrentTarget);

            if (!Core.Me.HasAura(Auras.TrueNorth))
            {
                if (Core.Me.CurrentTarget.IsFlanking)
                    return await Spells.ArmorCrush.Cast(Core.Me.CurrentTarget);
            }

            if (Core.Me.HasAura(Auras.TrueNorth))
            {
                if (HutonTimer.TotalMilliseconds <= 30000)
                    return await Spells.ArmorCrush.Cast(Core.Me.CurrentTarget);
            }

            return false;
        }

        public static async Task<bool> ShadowFang()
        {
            if (!NinjaSettings.Instance.UseShadowFang)
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds < 45000)
                return false;

            return await Spells.ShadowFang.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Assassinate()
        {
            if (!NinjaSettings.Instance.UseAssassinate)
                return false;
            if (Spells.SpinningEdge.Cooldown.TotalMilliseconds < 850)
                return false;
            if (!Core.Me.HasAura(Auras.AssassinateReady))
                return false;
            if (Casting.LastSpell == Spells.DreamWithinaDream)
                return false;
            if (Casting.LastSpell == Spells.Bhavacakra)
                return false;
            return await Spells.Assassinate.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Mug()
        {
            if (!NinjaSettings.Instance.UseMug)
                return false;
            //if (Spells.SpinningEdge.Cooldown.TotalMilliseconds < 850)
            //    return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds > 55000)
                return false;

            if (Core.Me.HasAura(Auras.VulnerabilityTrickAttack))
                return false;

            if (NinkiGauge > 60)
                return false;

            return await Spells.Mug.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TrickAttack()
        {
            if (!NinjaSettings.Instance.UseTrickAttack)
                return false;

            if (!Core.Me.HasAura(Auras.Suiton))
                return false;

            if (Spells.TrickAttack.Cooldown.Seconds > 6)
                return false;

            //if (!Core.Me.CurrentTarget.IsBehind && !Core.Me.HasAura(Auras.TrueNorth) && Spells.TrueNorth.Charges >0)
            //    return await Spells.TrueNorth.Cast(Core.Me);

            if (Core.Me.HasAura(Auras.Suiton, true, 18000))
                return false;

            if (Core.Me.HasAura(Auras.Suiton, true, 1))
                return await Spells.TrickAttack.Cast(Core.Me.CurrentTarget);

            if (!BotManager.Current.IsAutonomous)
                return false;

            return false;
        }

        public static async Task<bool> DreamWithinADream()
        {
            if (!NinjaSettings.Instance.UseDreamWithinADream)
                return false;

            if (Spells.SpinningEdge.Cooldown.TotalMilliseconds < 850)
                return false;

            if (Spells.TrickAttack.Cooldown.TotalMilliseconds > 50000 && Spells.ShadowFang.Cooldown.TotalMilliseconds > 2000)
                return await Spells.DreamWithinaDream.Cast(Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> Bhavacakra()
        {
            if (!NinjaSettings.Instance.UseBhavacakra)
                return false;

            if (Casting.LastSpell == Spells.Bunshin)
                return false;       

            if (Casting.LastSpell == Spells.Assassinate)
                return false;

            if (Casting.LastSpell == Spells.Kassatsu)
                return false;


            //if (Spells.Ten.Charges >= 1 && Spells.Ten.Cooldown.TotalMilliseconds <= 15000)
            //    return true;

            if (NinkiGauge >= 100 && Spells.Bunshin.Cooldown.TotalMilliseconds < Spells.TrickAttack.Cooldown.TotalMilliseconds && Spells.TrickAttack.Cooldown.TotalSeconds > 3)
                return await (Spells.Bhavacakra.Cast(Core.Me.CurrentTarget));

            if (NinkiGauge >= 50 && Spells.Mug.Cooldown.TotalMilliseconds < Spells.TrickAttack.Cooldown.TotalMilliseconds + 1000 && Spells.Bunshin.Cooldown.TotalMilliseconds > Spells.TrickAttack.Cooldown.TotalMilliseconds)
                return await (Spells.Bhavacakra.Cast(Core.Me.CurrentTarget));

            if (NinkiGauge < 50)
                return false;

            if (NinkiGauge < 90 && Spells.TrickAttack.Cooldown.TotalMilliseconds < 46000)
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

                    return await (Spells.Bhavacakra.Cast(Core.Me.CurrentTarget));
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
                if ((((calc - ninadjust) * 5) + (third * 10) + Utilities.Routines.Ninja.ninki) - 5 > 90)
                    canwesafelycastthis = 1;

                if (canwesafelycastthis == 1)
                {
                    return await (Spells.Bhavacakra.Cast(Core.Me.CurrentTarget));
                }
            }

            return false;
        }
    }
}
