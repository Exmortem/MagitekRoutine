using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Models.QueueSpell;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using MachinistRoutine = Magitek.Utilities.Routines.Machinist;

namespace Magitek.Logic.Machinist
{
    internal static class SingleTarget
    {
        public static async Task<bool> HeatedSplitShot()
        {
            //One to disable them all
            if (!MachinistSettings.Instance.UseSplitShotCombo)
                return false;

            if (MachinistSettings.Instance.UseDrill && Spells.Drill.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.IsKnownAndReady(200) && ActionResourceManager.Machinist.Battery <= 80)
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.IsKnownAndReady(200) && ActionResourceManager.Machinist.Battery <= 80)
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.Reassembled))
                return false;

            return await MachinistRoutine.HeatedSplitShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedSlugShot()
        {
            if (!MachinistRoutine.CanContinueComboAfter(Spells.SplitShot))
                return false;

            if (MachinistSettings.Instance.UseDrill && Spells.Drill.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.IsKnownAndReady(200))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.Reassembled))
                return false;

            return await MachinistRoutine.HeatedSlugShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatedCleanShot()
        {
            if (!MachinistRoutine.CanContinueComboAfter(Spells.SlugShot))
                return false;

            if (MachinistSettings.Instance.UseDrill && Spells.Drill.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseHotAirAnchor && Spells.AirAnchor.IsKnownAndReady(200))
                return false;

            if (MachinistSettings.Instance.UseChainSaw && Spells.ChainSaw.IsKnownAndReady(200))
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.Reassembled))
                return false;

            return await MachinistRoutine.HeatedCleanShot.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Drill()
        {
            if (!MachinistSettings.Instance.UseDrill)
                return false;

            if (!Spells.Drill.IsKnownAndReady())
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff))
                return false;

            /*
            if (MachinistSettings.Instance.UseReassembleOnDrill && !Core.Me.HasAura(Auras.Reassembled) && Core.Me.ClassLevel >= 10)
            {
               if ((Core.Me.ClassLevel > 83 && Spells.Reassemble.Charges >= 1 && Spells.Reassemble.IsKnown())
                   || (Core.Me.ClassLevel < 84 && Spells.Reassemble.IsKnownAndReady()) )
               {
                    SpellQueueLogic.SpellQueue.Clear();
                    SpellQueueLogic.Timeout.Start();
                    SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 3000;
                    SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Reassemble, TargetSelf = true });
                    SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Drill });
                    return true;
                }
            }
            */

            return await Spells.Drill.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HotAirAnchor()
        {
            if (!MachinistSettings.Instance.UseHotAirAnchor)
                return false;

            if (!Spells.AirAnchor.IsKnownAndReady() && !Spells.HotShot.IsKnownAndReady())
                return false;

            if (ActionResourceManager.Machinist.OverheatRemaining > TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff))
                return false;

            /*
            if (MachinistSettings.Instance.UseReassembleOnAA && !Core.Me.HasAura(Auras.Reassembled) && Core.Me.ClassLevel >= 10)
            {
                if ((Core.Me.ClassLevel > 83 && Spells.Reassemble.Charges >= 1 && Spells.Reassemble.IsKnown())
                    || (Core.Me.ClassLevel < 84 && Spells.Reassemble.IsKnownAndReady()))
                {
                    SpellQueueLogic.SpellQueue.Clear();
                    SpellQueueLogic.Timeout.Start();
                    SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 3000;
                    SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Reassemble, TargetSelf = true });
                    SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = MachinistRoutine.HotAirAnchor });
                    return true;
                }
            }
            */

            return await MachinistRoutine.HotAirAnchor.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HeatBlast()
        {
            if (ActionResourceManager.Machinist.OverheatRemaining == TimeSpan.Zero)
                return false;

            return await Spells.HeatBlast.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GaussRound()
        {
            if (!MachinistSettings.Instance.UseGaussRound)
                return false;

            if (Casting.LastSpell == Spells.Wildfire || Casting.LastSpell == Spells.Hypercharge || Casting.LastSpell == Spells.Ricochet)
                return false;

            if (Spells.Wildfire.IsKnownAndReady() && Spells.Hypercharge.IsKnownAndReady() && Spells.GaussRound.Charges < 1.5f)
                return false;

            if (Core.Me.ClassLevel >= 45)
            {
                if (Spells.GaussRound.Charges < 1.5f && Spells.Wildfire.IsKnownAndReady(2000))
                    return false;

                // Do not run Gauss if an hypercharge is almost ready and not enough charges available for Rico and Gauss
                if (ActionResourceManager.Machinist.Heat > 40 || Spells.Hypercharge.IsKnownAndReady())
                {
                    if (Spells.GaussRound.Charges < 1.5f && Spells.Ricochet.Charges < 0.5f)
                        return false;
                }
            }

            return await Spells.GaussRound.Cast(Core.Me.CurrentTarget);
        }
    }
}
