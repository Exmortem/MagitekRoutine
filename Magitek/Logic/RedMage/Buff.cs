using ff14bot;
using Magitek.Models.RedMage;
using Magitek.Utilities;
using Magitek.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.RedMage;
using static Magitek.Logic.RedMage.Utility;
using ff14bot.Managers;
using Magitek.Toggles;


namespace Magitek.Logic.RedMage
{
    internal class Buff
    {
        public static async Task<bool> Acceleration()
        {
            if (!RedMageSettings.Instance.Acceleration)
                return false;

            if (Core.Me.ClassLevel < Spells.Acceleration.LevelAcquired)
                return false;

            if (Spells.Acceleration.Cooldown != TimeSpan.Zero
                && Spells.Acceleration.Charges == 0)
                return false;

            if (Core.Me.HasAura(Auras.Swiftcast)
                || Core.Me.HasAura(Auras.Dualcast)
                || Core.Me.HasAura(Auras.Acceleration))
                return false;

            if (!MovementManager.IsMoving)
                return false;        

            if (InComboEnder())
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            return await Spells.Acceleration.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Embolden()
        {
            if (!RedMageSettings.Instance.Embolden)
                return false;

            if (Core.Me.ClassLevel < Spells.Embolden.LevelAcquired)
                return false;

            if (Spells.Embolden.Cooldown != TimeSpan.Zero)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            return await Spells.Embolden.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Manafication()
        {
            if (!RedMageSettings.Instance.Manafication)
                return false;

            if (Core.Me.ClassLevel < Spells.Manafication.LevelAcquired)
                return false;

            if (Spells.Manafication.Cooldown != TimeSpan.Zero)
                return false;

            //Manafication cancels any combo

            if (InAoeCombo())
                return false;
                       
            if (InCombo())
                return false;

            if (InComboEnder())
                return false;
                        
            if (Spells.Embolden.Cooldown.TotalMilliseconds <= 13000) //trying a little more leeway
                return false;

            if (WhiteMana <= 50
            && BlackMana <= 50)
                return await Spells.Manafication.Cast(Core.Me.CurrentTarget);

            return false;
        }
        public static async Task<bool> MagickBarrier()
        {
            if (!RedMageSettings.Instance.MagickBarrier)
               return false;

            if (Core.Me.ClassLevel < Spells.MagickBarrier.LevelAcquired)
                return false;

            if (Spells.MagickBarrier.Cooldown != TimeSpan.Zero)
                return false;

            if (RedMageSettings.Instance.ForceMagickBarrier)
            {
                if (await Spells.MagickBarrier.Cast(Core.Me))
                {
                    RedMageSettings.Instance.ForceMagickBarrier = false;
                    TogglesManager.ResetToggles();
                    return true;
                }
                return false;
            }

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            return false;
        }
        public static async Task<bool> LucidDreaming()
        {
            if (!RedMageSettings.Instance.LucidDreaming)
                return false;

            if (Core.Me.ClassLevel < Spells.LucidDreaming.LevelAcquired)
                return false;

            if (Core.Me.CurrentManaPercent > RedMageSettings.Instance.LucidDreamingManaPercent)
                return false;

            if (Spells.LucidDreaming.Cooldown != TimeSpan.Zero)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            return await Spells.LucidDreaming.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Swiftcast()
        {
            if (Core.Me.ClassLevel < Spells.Swiftcast.LevelAcquired)
                return false;

            if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                return false;

            if (InAoeCombo())
                return false;

            if (InCombo())
                return false;

            if (!MovementManager.IsMoving)
                return false;

            if (Core.Me.HasAura(Auras.Dualcast)
                 || Core.Me.HasAura(Auras.Acceleration))
                return false;

            if (!RedMageSettings.Instance.SwiftcastVerthunderVeraero
                && !RedMageSettings.Instance.SwiftcastScatter)
                return false;

            return await Spells.Swiftcast.Cast(Core.Me.CurrentTarget);
        }
        
    }
}
