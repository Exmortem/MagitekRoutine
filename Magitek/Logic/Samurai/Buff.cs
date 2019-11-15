using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Samurai
{
    internal static class Buff
    {
        public static async Task<bool> MeikyoShisui()
        {
            if (Core.Me.ClassLevel < 50)
                return false;

            if (Core.Me.HasAura(Auras.MeikyoShisui))
                return false;

            if (Spells.KaeshiSetsugekka.Cooldown.TotalMilliseconds > 4700 || Spells.KaeshiGoken.Cooldown.TotalMilliseconds > 4700 || Spells.KaeshiHiganbana.Cooldown.TotalMilliseconds > 4700)
                return false;

            if (Utilities.Routines.Samurai.CastDuringMeikyo.Any())
            {
                Utilities.Routines.Samurai.CastDuringMeikyo.Clear();
            }
           
            if (SamuraiSettings.Instance.MeikyoOnlyWithZeroSen && Utilities.Routines.Samurai.SenCount != 0)
                return false;

            if (Utilities.Routines.Samurai.SenCount == 3)
                return false;

            //Don't use mid combo
            if (ActionManager.LastSpell == Spells.Hakaze || ActionManager.LastSpell == Spells.Shifu || ActionManager.LastSpell == Spells.Jinpu || ActionManager.LastSpell == Spells.Fuga
                || Casting.LastSpell == Spells.Hakaze || Casting.LastSpell == Spells.Shifu || Casting.LastSpell == Spells.Jinpu || Casting.LastSpell == Spells.Fuga)
                return false;

            if (!Core.Me.HasAura(Auras.Jinpu, true, 7000) || !Core.Me.HasAura(Auras.Shifu, true, 7000))
                return false;

            if (!await Spells.MeikyoShisui.CastAura(Core.Me, Auras.MeikyoShisui))
                return false;

            //Removing logic as shouldn't be needed here

            //// We need to figure out the order in which to cast Kasha (Shifu), Gekko (Jinpu) and Yukikaze (Slashing Debuff)
            //var buffList = new List<Tuple<SpellData, float>>();

            //// Get the time left remaining on aura (0 if it's null)
            //var shifu = Core.Me.HasAura(Auras.Shifu) ? Core.Me.GetAuraById(Auras.Shifu).TimeLeft : 0f;
            //var jinpu = Core.Me.HasAura(Auras.Jinpu) ? Core.Me.GetAuraById(Auras.Jinpu).TimeLeft : 0f;
            //var slashing = 40f; //Slashing no longer exists, so it can be lowest prio

            //// Add the tuples to the list
            //buffList.Add(new Tuple<SpellData, float>(Spells.Kasha, shifu));
            //buffList.Add(new Tuple<SpellData, float>(Spells.Gekko, jinpu));
            //buffList.Add(new Tuple<SpellData, float>(Spells.Yukikaze, slashing));

            //// Run through the list sorted by the time remaining left
            //foreach (var tuple in buffList.OrderBy(r => r.Item2))
            //{
            //    // Add the abilities to the queue in order
            //    Utilities.Routines.Samurai.CastDuringMeikyo.Enqueue(tuple.Item1);
            //    Logger.WriteInfo($@"Adding {tuple.Item1.Name} To Meikyo Queue With {tuple.Item2} Time Left");
            //}

            return true;
        }

        public static async Task<bool> ThirdEye()
        {
            return await Spells.ThirdEye.Cast(Core.Me);
        }

        public static async Task<bool> Ikishoten()
        {
            if (Core.Me.ClassLevel < 68)
                return false;

            if (ActionResourceManager.Samurai.Kenki > 50)
                return false;

            return await Spells.Ikishoten.Cast(Core.Me);
        }

        public static async Task<bool> BloodBath()
        {
            if (!SamuraiSettings.Instance.Bloodbath)
                return false;

            if (Core.Me.CurrentHealthPercent > SamuraiSettings.Instance.BloodbathHealthPercent)
                return false;

            return await Spells.Bloodbath.Cast(Core.Me);
        }

        public static async Task<bool> SecondWind()
        {
            if (!SamuraiSettings.Instance.SecondWind)
                return false;

            if (Core.Me.CurrentHealthPercent > SamuraiSettings.Instance.SecondWindHealthPercent) return false;

            return await Spells.SecondWind.Cast(Core.Me);
        }

        public static async Task<bool> TrueNorth()
        {
            if (!SamuraiSettings.Instance.TrueNorth)
                return false;

            if (ViewModels.BaseSettings.Instance.PositionalStatus != "OutOfPosition") //TODO: gross
                return false;

            if (Casting.LastSpell == Spells.TrueNorth)
                return false;

            if (Core.Me.HasAura(Auras.TrueNorth))
                return false;

            if (Casting.LastSpell != Spells.Shifu && Casting.LastSpell != Spells.Jinpu)
                return false;

            return await Spells.TrueNorth.Cast(Core.Me);
        }
    }
}