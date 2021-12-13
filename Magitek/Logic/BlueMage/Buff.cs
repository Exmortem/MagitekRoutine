using System.Threading.Tasks;
using System;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Utilities;
using Magitek.Models.BlueMage;

namespace Magitek.Logic.BlueMage
{
    internal static class Buff
    {
        public static async Task<bool> OffGuard()
        {
            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false; 
            
            if (Core.Me.CurrentTarget.HasAura(Auras.OffGuard))
                return false;

            if (Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.OffGuard.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> PeculiarLight()
        {
            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false; 
            
            if (Core.Me.CurrentTarget.HasAura(Auras.PeculiarLight))
                return false;

            return await Spells.PeculiarLight.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Whistle()
        {
            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false; 
            
            if (Core.Me.HasAura(Auras.Boost))
                return false;

            if (Core.Me.HasAura(Auras.Harmonized))
                return false;

            if (Casting.LastSpell == Spells.Bristle)
                return false;

            if (Spells.TripleTrident.Cooldown.TotalMilliseconds >= 4000)
                return false;

            if (Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            if (BlueMageSettings.Instance.UseMoonFlute && ActionManager.HasSpell(Spells.MoonFlute.Id) && !Utilities.Routines.BlueMage.IsMoonFluteWindowReady)
                return false;

            return await Spells.Whistle.Cast(Core.Me);
        }

        public static async Task<bool> Bristle()
        {
            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false; 
            
            if (Core.Me.HasAura(Auras.Boost))
                return false;

            if (Core.Me.HasAura(Auras.Harmonized))
                return false;

            if (Casting.LastSpell == Spells.Whistle)
                return false;

            if (!Core.Me.CurrentTarget.HasAura(Auras.Bleeding, true, 6000))
                return await Spells.Bristle.Cast(Core.Me);

            if (Spells.TripleTrident.Cooldown.TotalMilliseconds < 3000)
                return false;

            if (Core.Me.CurrentTarget.HasAura(Auras.Bleeding, true, 6000) && Spells.TheRoseOfDestruction.Cooldown.TotalMilliseconds >= 2500 
                && Spells.MatraMagic.Cooldown.TotalMilliseconds >= 2500 && Spells.JKick.Cooldown.TotalMilliseconds >= 2500)
                return false;

            return await Spells.Bristle.Cast(Core.Me);
        }

        public static async Task<bool> MoonFlute()
        {
            if (!ActionManager.HasSpell(Spells.MoonFlute.Id))
                return false;
            
            if (!BlueMageSettings.Instance.UseMoonFlute)
                return false;

            if (Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            Logger.WriteInfo($@"[MoonFlute] IsMoonFluteWindowReady = {Utilities.Routines.BlueMage.IsMoonFluteWindowReady}");
            if (!Utilities.Routines.BlueMage.IsMoonFluteWindowReady)
                return false;

            //Force Whistle or Bristle before MoonFlute
            if (!Core.Me.HasAura(Auras.Harmonized) && !Core.Me.HasAura(Auras.Boost))
                return false;

            return await Spells.MoonFlute.Cast(Core.Me);
        }

        public static async Task<bool> LucidDreaming()
        {
            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false;

            if (Core.Me.CurrentManaPercent > BlueMageSettings.Instance.LucidDreamingManaPercent)
                return false;

            if (Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            return await Spells.LucidDreaming.Cast(Core.Me);
        }

        public static async Task<bool> Swiftcast()
        {
            if (Utilities.Routines.BlueMage.IsSurpanakhaInProgress)
                return false; 
            
            if (BlueMageSettings.Instance.UseMoonFlute && !Core.Me.HasAura(Auras.WaxingNocturne))
                return false;

            if (Spells.MatraMagic.Cooldown.TotalMilliseconds > 1000 
                && Spells.TheRoseOfDestruction.Cooldown.TotalMilliseconds > 1000
                && Spells.TripleTrident.Cooldown.TotalMilliseconds > 1000 
                && Spells.AngelWhisper.Cooldown.TotalMilliseconds > 1000)
                return false;

            return await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast);
        }
    }
}