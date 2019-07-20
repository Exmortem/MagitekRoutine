using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Ninja;
using Magitek.Logic.Roles;
using Magitek.Models.Ninja;
using Magitek.Utilities;

namespace Magitek.Rotations.Ninja
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            if (BotManager.Current.IsAutonomous)
            {
                Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 3 + Core.Me.CurrentTarget.CombatReach);
            }

            if (!SpellQueueLogic.SpellQueue.Any())
            {
                SpellQueueLogic.InSpellQueue = false;
            }

            if (SpellQueueLogic.SpellQueue.Any())
            {
                if (await SpellQueueLogic.SpellQueueMethod()) return true;
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;


            if (await PhysicalDps.Interrupt(NinjaSettings.Instance)) return true;
            
            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Utilities.Routines.Ninja.OnGcd)
            {
                if (await PhysicalDps.SecondWind(NinjaSettings.Instance)) return true;
                if (await PhysicalDps.Bloodbath(NinjaSettings.Instance)) return true;
                if (await PhysicalDps.Feint(NinjaSettings.Instance)) return true;

                if (Core.Me.HasAura(Auras.Suiton))
                    if (await PhysicalDps.TrueNorth(NinjaSettings.Instance)) return true;

                if (await Buff.ShadeShift()) return true;
                if (await Buff.Bunshin()) return true;
                if (await SingleTarget.Assassinate()) return true;
                if (await SingleTarget.Mug()) return true;
                if (await Buff.Kassatsu()) return true;
                if (await SingleTarget.TrickAttack()) return true;
                if (await SingleTarget.DreamWithinADream()) return true;
                if (await Aoe.HellfrogMedium()) return true;
                if (await SingleTarget.Bhavacakra()) return true;
                if (await Ninjutsu.TenChiJin()) return true;
            }

            if (Ninjutsu.Huton()) return true;
            if (Ninjutsu.GokaMekkyaku()) return true;
            if (Ninjutsu.Doton()) return true;
            if (Ninjutsu.Katon()) return true;
            if (Ninjutsu.Suiton()) return true;
            if (Ninjutsu.HyoshoRanryu()) return true;
            if (Ninjutsu.Raiton()) return true;
            if (Ninjutsu.FumaShuriken()) return true;

            if (await Aoe.HakkeMujinsatsu()) return true;
            if (await Aoe.DeathBlossom()) return true;
            if (await SingleTarget.ShadowFang()) return true;
            if (await SingleTarget.ArmorCrush()) return true;
            if (await SingleTarget.AeolianEdge()) return true;
            if (await SingleTarget.GustSlash()) return true;
            return await SingleTarget.SpinningEdge();
        }
    }
}