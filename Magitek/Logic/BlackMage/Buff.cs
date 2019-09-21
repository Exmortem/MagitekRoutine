using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlackMage;
using Magitek.Utilities;

namespace Magitek.Logic.BlackMage
{
    internal static class Buff
    {
        public static async Task<bool> Triplecast()
        {
            if (!BlackMageSettings.Instance.TripleCast)
                return false;

            if (ActionResourceManager.BlackMage.UmbralHearts == 3 && Casting.LastSpell == Spells.Fire3)
                return await Spells.Triplecast.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> Enochian()
        {
            if (Core.Me.HasEnochian())
                return false;
            if (Core.Me.ClassLevel < 56)
                return false;
            return await Spells.Enochian.Cast(Core.Me);
        }

        public static async Task<bool> Sharpcast()
        {
            if (!BlackMageSettings.Instance.Sharpcast)
                return false;

            // If we used something that opens the GCD
           
                if (Casting.LastSpell == Spells.Fire3 ||  Casting.LastSpell == Spells.Thunder3 || Core.Me.HasAura(Auras.Triplecast) || Casting.LastSpell == Spells.Xenoglossy) 
                    return await Spells.Sharpcast.Cast(Core.Me);
            
            return false;
        }

        public static async Task<bool> LeyLines()
        {
            if (!BlackMageSettings.Instance.LeyLines)
                return false;

            if (BlackMageSettings.Instance.LeyLinesBossOnly && !Core.Me.CurrentTarget.IsBoss())
                return false;

            // Do not Ley Lines if we don't have 3 astral stacks
            if (ActionResourceManager.BlackMage.AstralStacks != 3)
                return false;

            // Do not Ley Lines if we don't have any umbral hearts (roundabout check to see if we're at the begining of astral)
            if (ActionResourceManager.BlackMage.UmbralHearts < 1)
                return false;
            // If we used something that opens the GCD
            if (Casting.LastSpell == Spells.Fire3 || Casting.LastSpell == Spells.Blizzard3 || Casting.LastSpell == Spells.Thunder3 || Core.Me.HasAura(Auras.Triplecast) || Casting.LastSpell == Spells.Xenoglossy)
                return await Spells.LeyLines.Cast(Core.Me);
            return false;
            
        }

        public static async Task<bool> UmbralSoul()
        {
            if (!Core.Me.HasEnochian())
                return false;

            // Do not Umbral Soul unless we have 1 umbral stack
            if (ActionResourceManager.BlackMage.UmbralStacks != 1)
                return false;

            return await Spells.UmbralSoul.Cast(Core.Me);
        }

        public static async Task<bool> ManaFont()
        {
            if (Casting.LastSpell == Spells.Fire3 && Spells.Fire.Cooldown.TotalMilliseconds > 700)
                return await Spells.ManaFont.Cast(Core.Me);
            return false;
        }

        
    }

}
