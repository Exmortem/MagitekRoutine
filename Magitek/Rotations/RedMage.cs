using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.RedMage;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using static ff14bot.Managers.ActionResourceManager.RedMage;

namespace Magitek.Rotations
{
    public static class RedMage
    {
        public static async Task<bool> Rest()
        {
            return false;
        }

        public static async Task<bool> PreCombatBuff()
        {
            

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.ClassLevel < 2 ? 3 : 20);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }
        public static async Task<bool> Heal()
        {
            

            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            if (await GambitLogic.Gambit()) return true;
            if (await Logic.RedMage.Heal.Verraise()) return true;
            return await Logic.RedMage.Heal.Vercure();
        }
        public static async Task<bool> CombatBuff()
        {
            return false;
        }
        public static async Task<bool> Combat()
        {
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, Core.Me.ClassLevel < 2 ? 3 : 20);
                }
            }

            bool FinisherReady()
            {
                if (Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.Verflare || s.Spell == Spells.Verholy))
                    return false;

                return Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.Redoublement);
            }

            if (Core.Me.ClassLevel >= Spells.Verflare.LevelAcquired
                && FinisherReady()
                && (BlackMana <= 20 || WhiteMana <= 20))
            {
                await SingleTarget.Displacement();
                if (await SingleTarget.Verholy()) return true;
                return await SingleTarget.Verflare();
            }

            if (await SingleTarget.Scorch()) return true;

            if (await Buff.Embolden()) return true;
            if (await Buff.Manafication()) return true;
            if (await Buff.LucidDreaming()) return true;

            if (Utilities.Routines.RedMage.OnGcd && Casting.LastSpell != Spells.CorpsACorps)
            {
                if (await Aoe.ContreSixte()) return true;
                if (await SingleTarget.Fleche()) return true;
            }

            if (RedMageSettings.Instance.UseAoe)
            {
                if (await Aoe.Moulinet()) return true;
                if (await Aoe.Scatter()) return true;
                if (await Aoe.Veraero2()) return true;
                if (await Aoe.Verthunder2()) return true;
            }

            if (await SingleTarget.CorpsACorps()) return true;

            if (await SingleTarget.Redoublement()) return true;
            if (await SingleTarget.Zwerchhau()) return true;
            if (await SingleTarget.Riposte()) return true;

            if (await SingleTarget.Displacement()) return true;
            if (await SingleTarget.Engagement()) return true;

            if (Casting.LastSpell == Spells.CorpsACorps)
                return true;

            if (await SingleTarget.Verstone()) return true;
            if (await SingleTarget.Verfire()) return true;
            if (await SingleTarget.Veraero()) return true;
            if (await SingleTarget.Verthunder()) return true;
            if (await Buff.Acceleration()) return true;
            return await SingleTarget.Jolt();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
