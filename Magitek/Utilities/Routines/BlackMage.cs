using ff14bot;
using ff14bot.Managers;
using System;
using System.Linq;



namespace Magitek.Utilities.Routines
{

    internal static class BlackMage
    {
        public static bool CanQuadFlare;
        public static void RefreshVars()
        {
            var item = InventoryManager.FilledSlots.FirstOrDefault(r => r.RawItemId == Ether
            || r.RawItemId == HiEther || r.RawItemId == XEther || r.RawItemId == MegaEther
            || r.RawItemId == SuperEther);

            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            if (Spells.Triplecast.Cooldown == TimeSpan.Zero
                && Spells.ManaFont.Cooldown == TimeSpan.Zero
                && Spells.Swiftcast.Cooldown == TimeSpan.Zero
                && item.CanUse(Core.Me))
                CanQuadFlare = true;
            CanQuadFlare = false;
        }
        public static bool NeedToInterruptCast()
        {
            if (Casting.SpellTarget?.CurrentHealth == 0)
            {
                {
                    Logger.Error($"Stopped {Casting.CastingSpell.LocalizedName}: because HE'S DEAD, JIM!");
                }
                return true;
            }
            return false;
        }
        
        public static readonly uint Ether = 4555;
        public static readonly uint HiEther = 4556;
        public static readonly uint XEther = 4558;
        public static readonly uint MegaEther = 13638;        
        public static readonly uint SuperEther = 23168;
    }
}
