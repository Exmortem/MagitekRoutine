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
            if (await Buff.Acceleration()) return true;
            if (await Buff.Embolden()) return true;
            if (await Buff.Manafication()) return true;
            if (await Buff.LucidDreaming()) return true;

            // Let's go fishing (for procs)
            if (BlackMana >=80 && WhiteMana >=80 && Spells.Acceleration.Cooldown > TimeSpan.Zero)
            {
                if (BlackMana < WhiteMana && BlackMana < 89 && WhiteMana < 91 && Core.Me.HasAura(Auras.VerfireReady))
                {
                    if (Core.Me.HasAura(Auras.Dualcast))
                    {
                        await Spells.Veraero.Cast(Core.Me.CurrentTarget);
                        return await Spells.Verfire.Cast(Core.Me.CurrentTarget);
                    }else if (!Core.Me.HasAura(Auras.Dualcast))
                    {
                        await Spells.Verfire.Cast(Core.Me.CurrentTarget);
                        return await Spells.Veraero.Cast(Core.Me.CurrentTarget);
                    }
                    return false;
                }
                if (BlackMana > WhiteMana && BlackMana < 91 && WhiteMana < 89 && Core.Me.HasAura(Auras.VerstoneReady))
                {
                    if (Core.Me.HasAura(Auras.Dualcast))
                    {
                        await Spells.Verthunder.Cast(Core.Me.CurrentTarget);
                        return await Spells.Verstone.Cast(Core.Me.CurrentTarget);
                    }else if (!Core.Me.HasAura(Auras.Dualcast))
                    {
                        await Spells.Verstone.Cast(Core.Me.CurrentTarget);
                        return await Spells.Verthunder.Cast(Core.Me.CurrentTarget);
                    }
                    return false;
                }
            }
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
                        
            if (Utilities.Routines.RedMage.OnGcd && Casting.LastSpell != Spells.CorpsACorps)
            {
                if (await Aoe.ContreSixte()) return true;
                if (await SingleTarget.Fleche()) return true;
            }

            if (RedMageSettings.Instance.UseAoe)
            {
                // Replacement for moulinet
                if (SpellQueueLogic.SpellQueue.Any())
                {
                    if (await SpellQueueLogic.SpellQueueMethod()) return true;
                }
                //if (await Aoe.Moulinet()) return true;
                if (await Aoe.Scatter()) return true;
                if (await Aoe.Veraero2()) return true;
                if (await Aoe.Verthunder2()) return true;
            }

            if (await SingleTarget.CorpsACorps()) return true;

            if (await SingleTarget.Reprise()) return true;
            if (await SingleTarget.Redoublement()) return true;
            if (await SingleTarget.Zwerchhau()) return true;
            if (await SingleTarget.Riposte()) return true;

            if (await SingleTarget.Displacement()) return true;
            if (await SingleTarget.Engagement()) return true;

            if (Casting.LastSpell == Spells.CorpsACorps) return true;

            while (Core.Me.HasAura(Auras.VerstoneReady) && Core.Me.HasAura(Auras.VerfireReady) && BlackMana < 80 && WhiteMana < 80)
            {
                if (Core.Me.HasAura(Auras.Dualcast))
                    {
                        await SingleTarget.Veraero();
                        await SingleTarget.Verstone();
                        await SingleTarget.Verthunder();
                        return await SingleTarget.Verfire();
                    }else if (!Core.Me.HasAura(Auras.Dualcast))
                    {
                        await SingleTarget.Verstone();
                        await SingleTarget.Veraero();
                        await SingleTarget.Verfire();
                        return await SingleTarget.Verthunder();
                    }
                    return false;
            }
            if (await SingleTarget.Verstone()) return true;
            if (await SingleTarget.Verfire()) return true;
            if (await SingleTarget.Veraero()) return true;
            if (await SingleTarget.Verthunder()) return true;
            return await SingleTarget.Jolt();
        }
        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
