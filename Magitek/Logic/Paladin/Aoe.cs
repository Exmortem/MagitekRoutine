using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using PaladinRoutine = Magitek.Utilities.Routines.Paladin;


namespace Magitek.Logic.Paladin
{
    internal static class Aoe
    {

        /*************************************************************************************
         *                                    AOE on me
         * ***********************************************************************************/
        public static async Task<bool> CircleOfScorn()
        {
            if (!PaladinSettings.Instance.UseCircleOfScorn)
                return false;

            if (Core.Me.HasAura(Auras.Requiescat))
                return false;

            var ennemiesCount = Combat.Enemies.Count(x => x.Distance(Core.Me) <= Spells.CircleofScorn.Radius + Core.Me.CombatReach);            
            if (ennemiesCount < 1)
                return false;
                        
            if (ennemiesCount < PaladinSettings.Instance.TotalEclipseEnemies)
            {
                if (Spells.FightorFlight.IsKnownAndReady(5000))
                    return false;

                if (Core.Me.HasAura(Auras.FightOrFight, true) && !Spells.FightorFlight.IsReady(53500))
                    return false;

                if (Casting.LastSpell == Spells.FightorFlight)
                    return false;
            }

            return await Spells.CircleofScorn.Cast(Core.Me);
        }

        public static async Task<bool> HolyCircle()
        {
            if (!PaladinSettings.Instance.UseAoe)
                return false;

            if (!PaladinSettings.Instance.UseHolyCircle)
                return false;

            if (!Spells.HolyCircle.IsKnownAndReady())
                return false;

            if (PaladinRoutine.RequiescatStackCount <= 1)
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= Spells.HolyCircle.Radius + Core.Me.CombatReach) < PaladinSettings.Instance.HolyCircleEnemies)
                return false;

            return await Spells.HolyCircle.Cast(Core.Me);
        }

        /*************************************************************************************
         *                                    AOE on Target
         * ***********************************************************************************/
        public static async Task<bool> Expiacion() //SpiritsWithin or Expiacion
        {
            if (!PaladinSettings.Instance.UseExpiacon)
                return false;

            if (Casting.LastSpell == Spells.FightorFlight)
                return false;

            if (Spells.CircleofScorn.IsKnown() && !Core.Me.CurrentTarget.HasAura(Auras.CircleofScorn, true))
                return false;

            if (!Spells.Expiacion.IsKnown())
                return await Spells.SpiritsWithin.Cast(Core.Me.CurrentTarget);

            return await Spells.Expiacion.Cast(Core.Me.CurrentTarget);
        }



        /*************************************************************************************
         *                                    Combo
         * ***********************************************************************************/
        public static async Task<bool> TotalEclipse()
        {
            if (!PaladinSettings.Instance.UseAoe)
                return false;

            if (!PaladinSettings.Instance.UseEclipseCombo)
                return false;

            if (!Spells.TotalEclipse.IsKnownAndReady())
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= Spells.TotalEclipse.Radius + Core.Me.CombatReach) < PaladinSettings.Instance.TotalEclipseEnemies)
                return false;

            return await Spells.TotalEclipse.Cast(Core.Me);
        }

        public static async Task<bool> Prominence()
        {
            if (!PaladinSettings.Instance.UseAoe)
                return false;

            if (!PaladinSettings.Instance.UseEclipseCombo)
                return false;

            if (!Spells.Prominence.IsKnownAndReady())
                return false;

            if (!PaladinRoutine.CanContinueComboAfter(Spells.TotalEclipse))
                return false;

            return await Spells.Prominence.Cast(Core.Me);
        }

        /*************************************************************************************
         *                                    Combo 2
         * ***********************************************************************************/
        public static async Task<bool> Confiteor()
        {
            if (!PaladinSettings.Instance.UseConfiteorCombo)
                return false;

            if (!Spells.Confiteor.IsKnown())
                return false;

            if (PaladinRoutine.RequiescatStackCount > 1)
            {
                var requiescatAura = (Core.Me as Character).Auras.FirstOrDefault(x => x.Id == Auras.Requiescat && x.CasterId == Core.Player.ObjectId);
                if (requiescatAura != null && requiescatAura.TimespanLeft.TotalMilliseconds <= 2700)
                    return await Spells.Confiteor.Cast(Core.Me.CurrentTarget);

                return false;
            }

            return await Spells.Confiteor.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BladeOfFaith()
        {
            if (!PaladinSettings.Instance.UseConfiteorCombo)
                return false;

            if (!Spells.BladeOfFaith.IsKnownAndReady())
                return false;

            if (!PaladinRoutine.CanContinueComboAfter(Spells.Confiteor))
                return false;

            return await Spells.BladeOfFaith.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BladeOfTruth()
        {
            if (!PaladinSettings.Instance.UseConfiteorCombo)
                return false;

            if (!Spells.BladeOfTruth.IsKnownAndReady())
                return false;

            if (!PaladinRoutine.CanContinueComboAfter(Spells.BladeOfFaith))
                return false;

            return await Spells.BladeOfTruth.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BladeOfValor()
        {
            if (!PaladinSettings.Instance.UseConfiteorCombo)
                return false;

            if (!Spells.BladeOfValor.IsKnownAndReady())
                return false;

            if (!PaladinRoutine.CanContinueComboAfter(Spells.BladeOfTruth))
                return false;

            return await Spells.BladeOfValor.Cast(Core.Me.CurrentTarget);
        }
    }
}