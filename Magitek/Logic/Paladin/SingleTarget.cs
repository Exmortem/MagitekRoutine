using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Account;
using Magitek.Models.Paladin;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using PaladinRoutine = Magitek.Utilities.Routines.Paladin;


namespace Magitek.Logic.Paladin
{
    internal static class SingleTarget
    {
        public static async Task<bool> Interrupt()
        {
            List<SpellData> extraStun = new List<SpellData>();

            if (PaladinSettings.Instance.ShieldBash)
                extraStun.Add(Spells.ShieldBash);

            return await Tank.Interrupt(PaladinSettings.Instance, extraStuns: extraStun);
        }

        public static async Task<bool> ShieldLob()
        {
            if (PaladinSettings.Instance.UseShieldLobToPullExtraEnemies)
            {
                var pullTarget = Combat.Enemies.FirstOrDefault(r => r.ValidAttackUnit() && !r.Tapped && r.Distance(Core.Me) < 15 + r.CombatReach &&
                                                                                r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.TargetGameObject != Core.Me);

                if (pullTarget != null)
                    return await Spells.ShieldLob.Cast(pullTarget);
            }

            if (!PaladinSettings.Instance.UseShieldLob)
            {
                if (PaladinSettings.Instance.UseShieldLobToPull && !Core.Me.InCombat)
                {
                    return await Spells.ShieldLob.Cast(Core.Me.CurrentTarget);
                }
                return false;
            }

            return await Spells.ShieldLob.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ShieldLobOnLostAggro()
        {
            if (Globals.OnPvpMap)
                return false;

            if (!PaladinSettings.Instance.UseIronWill)
                return false;

            if (!PaladinSettings.Instance.UseShieldLobOnLostAggro)
                return false;

            var shieldLobTarget = Combat.Enemies.FirstOrDefault(r => r.Distance(Core.Me) > 5 + r.CombatReach && r.Distance(Core.Me) >= Core.Me.CombatReach + r.CombatReach && r.Distance(Core.Me) <= 15 + r.CombatReach && r.TargetGameObject != Core.Me);

            if (shieldLobTarget == null)
                return false;

            if (shieldLobTarget.TargetGameObject == null)
                return false;

            if (!await Spells.ShieldLob.Cast(shieldLobTarget))
                return false;

            Logger.Write($@"[Magitek] ShieldLob On {shieldLobTarget.Name} To Pull Aggro");
            return true;
        }

        public static async Task<bool> HolySpirit()
        {
            if (PaladinSettings.Instance.UseHolySpiritToPull && !Core.Me.InCombat)
                return await Spells.HolySpirit.Cast(Core.Me.CurrentTarget);

            if (!PaladinSettings.Instance.UseHolySpirit)
                return false;

            //Logger.Write($@"[HolySpirit] Info -> WithinSpellRange = {Core.Me.CurrentTarget.WithinSpellRange(Spells.FastBlade.Range)} | CombatReach = {Core.Me.CombatReach} / {Core.Me.CurrentTarget.CombatReach}]");
            if (PaladinSettings.Instance.UseHolySpiritWhenOutOfMeleeRange && !Core.Me.CurrentTarget.WithinSpellRange(Spells.FastBlade.Range))
            {
                //Logger.Write($@"[HolySpirit] Info -> Ennemies = {Combat.Enemies.Count(x => x.Distance(Core.Me) <= Spells.TotalEclipse.Radius + Core.Me.CombatReach)} | Distance = {Core.Me.CurrentTarget.Distance(Core.Me) - Core.Me.CombatReach}");
                return Combat.Enemies.Count(x => x.Distance(Core.Me) <= Spells.TotalEclipse.Radius + Core.Me.CombatReach) < PaladinSettings.Instance.TotalEclipseEnemies ? await Spells.HolySpirit.Cast(Core.Me.CurrentTarget) : false;
            }

            if (Core.Me.HasAura(Auras.FightOrFight, true))
                return false;

            if (!Core.Me.HasAura(Auras.Requiescat, true))
                return false;

            if (ActionManager.LastSpell != Spells.GoringBlade && Casting.LastSpell != Spells.HolySpirit)
                return false;

            if (PaladinRoutine.RequiescatStackCount <= 1 && Spells.Confiteor.IsKnown())
                return false;

            return await Spells.HolySpirit.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Intervene() //Dash
        {
            if (!PaladinSettings.Instance.UseIntervene)
                return false;
            
            if (!Spells.Intervene.IsKnown())
                return false;

            if (!Core.Me.HasAura(Auras.FightOrFight, true))
                return false;

            if (Casting.LastSpell != Spells.CircleofScorn && Casting.LastSpell != Spells.Expiacion)
                return false;

            /*
             * if (Combat.Enemies.Count(r => r.Distance(Core.Me) <= 3 + r.CombatReach) > 1)
                return false; 
            */
            
            if (PaladinSettings.Instance.InterveneOnlyInMelee && !Core.Me.CurrentTarget.WithinSpellRange(Spells.FastBlade.Range))
                return false;

            return await Spells.Intervene.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Requiescat()
        {
            if (!PaladinRoutine.ToggleAndSpellCheck(PaladinSettings.Instance.UseRequiescat, Spells.Requiescat))
                return false;

            //If many target, cast Requiescat outside FoF
            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) >= PaladinSettings.Instance.TotalEclipseEnemies)
                return await Spells.Requiescat.Cast(Core.Me.CurrentTarget);

            if (!Core.Me.HasAura(Auras.FightOrFight, true))
                return false;

            if (Core.Me.HasAura(Auras.FightOrFight, true) && Spells.FightorFlight.Cooldown.TotalMilliseconds > 49000)
                return false;

            /*var goringBladeAura = (Core.Me.CurrentTarget as Character).Auras.FirstOrDefault(x => x.Id == Auras.GoringBlade && x.CasterId == Core.Player.ObjectId);
            if (goringBladeAura == null || goringBladeAura.TimespanLeft.TotalMilliseconds <= 8000)
                return false;*/

            if (Spells.FastBlade.Cooldown.TotalMilliseconds > Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset + 100)
                return false;

            return await Spells.Requiescat.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Atonement()
        {
            if (!PaladinSettings.Instance.UseAtonement)
                return false; 
            
            if (!Core.Me.HasAura(Auras.SwordOath))
                return false;

            if (ActionManager.LastSpell != Spells.RoyalAuthority && ActionManager.LastSpell != Spells.Atonement)
                return false;

            // If we have a single charge (HasAuraCharge looks for single stack ONLY) and we're not under FoF then don't use attonement.
            if (!Core.Me.HasAura(Auras.FightOrFight, true) && Core.Me.HasAuraCharge(Auras.SwordOath, true))
                return false;

            return await Spells.Atonement.Cast(Core.Me.CurrentTarget);
        }


        /*************************************************************************************
         *                                    Combo
         * ***********************************************************************************/
        public static async Task<bool> RoyalAuthority()
        {
            if (!PaladinRoutine.CanContinueComboAfter(Spells.RiotBlade))
                return false;

            return await PaladinRoutine.RoyalAuthority.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> GoringBlade()
        {
            if (!PaladinRoutine.CanContinueComboAfter(Spells.RiotBlade))
                return false;

            //Cant be stacked with Blade Of Valor
            if (Core.Me.CurrentTarget.HasAura(Auras.BladeOfValor, true, 6500))
                return false;

            // If Mana is missing
            if (Core.Me.CurrentManaPercent < PaladinSettings.Instance.GoringBladeMpPercent)
                return await Spells.GoringBlade.Cast(Core.Me.CurrentTarget);

            //No need to refresh DOT
            if (Core.Me.CurrentTarget.HasAura(Auras.GoringBlade, true, 6500))
                return false;

            return await Spells.GoringBlade.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> RiotBlade()
        {
            if (!PaladinRoutine.CanContinueComboAfter(Spells.FastBlade))
                return false;

            return await Spells.RiotBlade.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FastBlade()
        {
            return await Spells.FastBlade.Cast(Core.Me.CurrentTarget);
        }
    }
}