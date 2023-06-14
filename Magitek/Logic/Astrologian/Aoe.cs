using ff14bot;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Logic.Astrologian
{
    internal static class Aoe
    {
        public static async Task<bool> AggroAst()
        {
            if (!AstrologianSettings.Instance.AggroAst)
                return false;

            if (!Core.Me.InCombat)
                return false;
            
            if (Core.Me.CurrentTarget.IsBoss() || Combat.Enemies.Count() > 4)

            if (await AggroLightSpeed()) return true;
            if (await AggroEarthlyStar()) return true;
            return await AggroMacrocosmos();
        }

        private static async Task<bool> AggroLightSpeed()
        {
            if (!Spells.Lightspeed.IsKnownAndReady())
                return false;

            if (Core.Me.HasAura(Auras.Lightspeed))
                return false;

            return await Spells.Lightspeed.CastAura(Core.Me, Auras.Lightspeed);
        }

        private static async Task<bool> AggroEarthlyStar()
        {
            if (!Spells.EarthlyStar.IsKnownAndReady())
                return false;

            if (Core.Me.HasAnyAura(new uint[] {Auras.EarthlyDominance,Auras.GiantDominance}))
                return false;
                
            
            if (!await Spells.EarthlyStar.Cast(Core.Me.CurrentTarget)) return false;
            
            Utilities.Routines.Astrologian.EarthlyStarLocation = Core.Target.Location;
            return true;
        }
        private static async Task<bool> AggroMacrocosmos()
        {
            if (!Spells.Macrocosmos.IsKnownAndReady())
                return false;

            if (Core.Me.HasAura(Auras.Macrocosmos))
                return false;

            return await Spells.Macrocosmos.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> Gravity()
        {
            if (!AstrologianSettings.Instance.Gravity)
                return false;

            if (!AstrologianSettings.Instance.DoDamage)
                return false;

            var target = Combat.SmartAoeTarget(Spells.Gravity, AstrologianSettings.Instance.SmartAoe);
            if (target == null)
                return false;
            
            if (Combat.Enemies.Count(r => r.Distance(target) <= Spells.Gravity.Radius) < AstrologianSettings.Instance.GravityEnemies)
                return false;
            
            return await Spells.Gravity.Cast(target);
        }

        public static async Task<bool> LordOfCrown()
        {
            if (!Core.Me.HasAura(Auras.LordOfCrownsDrawn))
                return false;


            if (Combat.Enemies.Count(r => r.Distance(Core.Me.Location) <= Spells.LordofCrowns.Radius) >= AstrologianSettings.Instance.LordOfCrownsEnemies)
                return await Spells.LordofCrowns.Cast(Core.Me);

            if (Spells.MinorArcana.IsReady())
                return await Spells.LordofCrowns.Cast(Core.Me);

            return false;
        }

    }
}