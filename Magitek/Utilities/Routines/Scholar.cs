using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Scholar
    {
        public static List<Character> AlliancePhysickOnly = new List<Character>();

        public static double SeraphTimeRemaining()
        {
            return Math.Max(Spells.SummonSeraph.Cooldown.TotalSeconds - 98, 0);
        }
        /*private static bool NeedAdloquiumTankBuster
        {
            get
            {
                var castingTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting);

                if (castingTarget == null)
                    return false;

                var castingInfo = castingTarget.SpellCastInfo;

                if (!TankBusterManager.AdloquiumList.ContainsKey(castingInfo.ActionId))
                    return false;

                var adloquiumTankBuster = TankBusterManager.AdloquiumList.FirstOrDefault(r => r.Key == castingInfo.ActionId);

                if (castingInfo.CurrentCastTime.TotalMilliseconds < adloquiumTankBuster.Value)
                    return false;

                return !castingTarget.HasAura(Auras.Galvanize);
            }   
        }

        private static bool NeedExcogitationTankBuster
        {
            get
            {
                var excogitationTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting && TankBusterManager.ExcogitationList.ContainsKey(r.CastingSpellId))?.TargetCharacter;

                if (excogitationTarget == null)
                    return false;

                return !excogitationTarget.HasAura(Auras.Exogitation);
            }
        }

        private static bool NeedSuccorTankBuster
        {
            get
            {
                var castingTarget = Combat.Enemies.FirstOrDefault(r => r.IsCasting);

                if (castingTarget == null)
                    return false;

                var castingInfo = castingTarget.SpellCastInfo;

                if (!TankBusterManager.SuccorList.ContainsKey(castingInfo.ActionId))
                    return false;

                var succorTankBuster = TankBusterManager.SuccorList.FirstOrDefault(r => r.Key == castingInfo.ActionId);

                if (castingInfo.CurrentCastTime.TotalMilliseconds < succorTankBuster.Value)
                    return false;

                return !Group.CastableAlliesWithin15.All(r => r.HasAura(Auras.Galvanize));
            }
        }*/

        public static bool NeedToInterruptCast()
        {
            /*if (Casting.CastingTankBuster)
                return false;
                */
            if (Casting.CastingSpell != Spells.Resurrection && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Cast: Unit Died");
                return true;
            }

            if (Casting.CastingSpell == Spells.Resurrection && (Casting.SpellTarget?.HasAura(Auras.Raise) == true || Casting.SpellTarget?.CurrentHealth > 0))
            {
                Logger.Error($@"Stopped Resurrection: Unit has raise aura");
                return true;
            }

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (Casting.CastingSpell == Spells.Succor || Casting.CastingSpell == Spells.Adloquium)
                return false;

            if (ScholarSettings.Instance.InterruptHealing && Casting.DoHealthChecks && Casting.SpellTarget?.CurrentHealthPercent >= ScholarSettings.Instance.InterruptHealingPercent)
            {
                Logger.Error($@"Stopped Healing: Target's Health Too High");
                return true;
            }

            if (ScholarSettings.Instance.StopCastingIfBelowHealthPercent && Globals.InParty)
            {
                if (Casting.CastingSpell == Spells.Broil ||
                    Casting.CastingSpell == Spells.Broil2 ||
                    Casting.CastingSpell == Spells.Broil3 ||
                    Casting.CastingSpell == Spells.Ruin
                    )
                {
                    if (Group.CastableAlliesWithin30.Any(c => c?.CurrentHealthPercent < ScholarSettings.Instance.DamageOnlyIfAboveHealthPercent && c.IsAlive))
                    {
                        Logger.Error($@"Stopped Cast: Ally below {ScholarSettings.Instance.DamageOnlyIfAboveHealthPercent}% Health");
                        return true;
                    }
                }
            }

            //if (ScholarSettings.Instance.SlowcastRes) {
            //    if (Casting.CastingSpell == Spells.Resurrection && Casting.SpellTarget?.CurrentHealth > 1) {
            //        Logger.Error($@"Stopped Cast: Target is not Dead");
            //        return true;
            //    }
            //}

            /*if (!ScholarSettings.Instance.UseTankBusters || !ScholarSettings.Instance.PrioritizeTankBusters)
                return false;*/

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            /*if (Core.Me.CurrentManaPercent < ScholarSettings.Instance.TankBusterMinimumMpPercent)
                return false;

            if (!NeedAdloquiumTankBuster && !NeedSuccorTankBuster && !NeedExcogitationTankBuster)
                return false;

            Logger.Error($@"Stopping Cast: Need To Use A Tank Buster");*/
            return false;
        }

        public static void GroupExtension()
        {
            Group.UpdateAlliance(
                ScholarSettings.Instance.IgnoreAlliance,
                ScholarSettings.Instance.HealAllianceDps,
                ScholarSettings.Instance.HealAllianceHealers,
                ScholarSettings.Instance.HealAllianceTanks,
                ScholarSettings.Instance.ResAllianceDps,
                ScholarSettings.Instance.ResAllianceHealers,
                ScholarSettings.Instance.ResAllianceTanks
            );
        }
    }
}
