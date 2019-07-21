using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Machinist;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Machinist
{
    internal static class Buff
    {
        public static async Task<bool> Reassemble()
        {
            if (!MachinistSettings.Instance.UseReassemble)
                return false;

            return await Spells.Reassemble.CastAura(Core.Me, Auras.Reassembled);
        }

        public static async Task<bool> BarrelStabilizer()
        {
            if (!MachinistSettings.Instance.UseBarrelStabilizer)
                return false;

            if (ActionResourceManager.Machinist.Heat > 45)
                return false;

            return await Spells.BarrelStabilizer.Cast(Core.Me);
        }

        public static async Task<bool> Hypercharge()
        {
            if (!MachinistSettings.Instance.UseHypercharge)
                return false;

            if (!MachinistSettings.Instance.SyncHyperchargeWithWildfire || !MachinistSettings.Instance.UseWildfire || !ActionManager.CurrentActions.Values.Contains(Spells.Wildfire))
                return await Spells.Hypercharge.Cast(Core.Me);

            if (Spells.Wildfire.Cooldown.Seconds > MachinistSettings.Instance.MaxSecondsToHoldHyperchargeForWildfire || Spells.Wildfire.Cooldown.Seconds < 1)
                    return await Spells.Hypercharge.Cast(Core.Me);
                    
            return false;
        }

        public static async Task<bool> Wildfire()
        {
            if (!MachinistSettings.Instance.UseWildfire)
                return false;

            if (Core.Me.HasAura(Auras.WildfireBuff, true))
                return false;
            
            if (MachinistSettings.Instance.SyncHyperchargeWithWildfire || !MachinistSettings.Instance.UseHypercharge)
                if (ActionResourceManager.Machinist.Heat <= 45 && Casting.SpellCastHistory.Take(5).All(s => s.Spell != Spells.Hypercharge))
                    return false;

            return await Spells.Wildfire.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Tactician()
        {
            if (!MachinistSettings.Instance.UseTactician)
                return false;

            if(PartyManager.IsInParty)
                if (Group.CastableAlliesWithin20.Any(r => r.HasAura(Auras.Troubadour) || r.HasAura(Auras.ShieldSamba)))
                    return false;

            return await Spells.Tactician.Cast(Core.Me);
        }
    }
}