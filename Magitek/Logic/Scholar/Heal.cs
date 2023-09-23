using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Scholar;
using Magitek.Toggles;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Scholar
{
    internal static class Heal
    {

        public static int AoeNeedHealing => PartyManager.NumMembers > 4 ? ScholarSettings.Instance.AoeNeedHealingFullParty : ScholarSettings.Instance.AoeNeedHealingLightParty;

        public static bool NeedAoEHealing()
        {
            var targets = Group.CastableAlliesWithin30.Where(r => r.CurrentHealthPercent <= ScholarSettings.Instance.AoEHealHealthPercent);

            var needAoEHealing = targets.Count() >= AoeNeedHealing;

            if (needAoEHealing)
                return true;

            return false;
        }

        #region ForceToggle

        public static async Task<bool> ForceWhispDawn()
        {
            if (!ScholarSettings.Instance.ForceWhispDawn)
                return false;

            if (!await Spells.WhisperingDawn.Cast(Core.Me)) return false;
            ScholarSettings.Instance.ForceWhispDawn = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceAdlo()
        {
            if (!ScholarSettings.Instance.ForceAdlo)
                return false;

            if (!await Spells.Adloquium.Cast(Core.Me.CurrentTarget)) return false;
            ScholarSettings.Instance.ForceAdlo = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceIndom()
        {
            if (!ScholarSettings.Instance.ForceIndom)
                return false;

            if (!await Spells.Indomitability.Cast(Core.Me)) return false;
            ScholarSettings.Instance.ForceIndom = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceExcog()
        {
            if (!ScholarSettings.Instance.ForceExcog)
                return false;

            if (!await Spells.Excogitation.Cast(Core.Me)) return false;
            ScholarSettings.Instance.ForceExcog = false;
            TogglesManager.ResetToggles();
            return true;
        }

        #endregion

        public static async Task<bool> Physick()
        {
            if (!ScholarSettings.Instance.Physick)
                return false;

            if (ScholarSettings.Instance.DisableSingleHealWhenNeedAoeHealing && NeedAoEHealing())
                return false;

            if (Globals.InParty)
            {
                var physickTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent < ScholarSettings.Instance.PhysickHpPercent);

                if (physickTarget != null)
                    return await Spells.Physick.Heal(physickTarget);

                if (!ScholarSettings.Instance.HealAllianceOnlyPhysick)
                    return false;

                physickTarget = Utilities.Routines.Scholar.AlliancePhysickOnly.FirstOrDefault(r => r.CurrentHealthPercent < ScholarSettings.Instance.PhysickHpPercent);

                if (physickTarget == null)
                    return false;

                return await Spells.Physick.Heal(physickTarget);

            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.PhysickHpPercent)
                return false;

            return await Spells.Physick.Heal(Core.Me);
        }

        public static async Task<bool> EmergencyTacticsAdloquium()
        {
            if (!ScholarSettings.Instance.Adloquium || !ScholarSettings.Instance.EmergencyTacticsAdloquium)
                return false;

            if (Spells.EmergencyTactics.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty)
            {
                var adloTarget = Group.CastableAlliesWithin30.Where(CanAdlo).OrderBy(a => a.CurrentHealthPercent).FirstOrDefault();

                if (adloTarget == null)
                    return false;

                if (!await Buff.EmergencyTactics())
                    return false;

                return await Spells.Adloquium.Heal(adloTarget, false);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.EmergencyTacticsAdloquiumHealthPercent)
                return false;

            if (!await Buff.EmergencyTactics())
                return false;

            return await Spells.Adloquium.Heal(Core.Me, false);

            bool CanAdlo(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.EmergencyTacticsAdloquiumHealthPercent)
                    return false;

                if (unit.HasAura(Auras.Excogitation))
                    return false;

                if (!ScholarSettings.Instance.AdloquiumOnlyHealer && !ScholarSettings.Instance.AdloquiumOnlyTank)
                    return true;

                if (ScholarSettings.Instance.AdloquiumOnlyHealer && unit.IsHealer())
                    return true;

                return ScholarSettings.Instance.AdloquiumOnlyTank && unit.IsTank();
            }
        }

        public static async Task<bool> Adloquium()
        {
            if (!ScholarSettings.Instance.Adloquium)
                return false;

            if (!ScholarSettings.Instance.AdloOutOfCombat && !Core.Me.InCombat)
                return false;

            if (ScholarSettings.Instance.DisableSingleHealWhenNeedAoeHealing && NeedAoEHealing())
                return false;

            if (Globals.InParty)
            {
                // If the lowest heal target is higher than Adloquium health, check to see if the user wants us to Galvanize the tank
                if (ScholarSettings.Instance.AdloquiumTankForBuff && Globals.HealTarget?.CurrentHealthPercent > ScholarSettings.Instance.AdloquiumHpPercent)
                {
                    // Pick any tank who doesn't have Galvanize on them
                    var tankAdloTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.IsTank() && !r.HasAura(Auras.Galvanize));

                    if (tankAdloTarget == null)
                        return false;

                    await UseRecitation();

                    return await Spells.Adloquium.HealAura(tankAdloTarget, Auras.Galvanize, false);
                }

                var adloTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanAdlo);

                if (adloTarget == null)
                    return false;

                await UseRecitation();

                return await Spells.Adloquium.HealAura(adloTarget, Auras.Galvanize);

                bool CanAdlo(Character unit)
                {
                    if (unit == null)
                        return false;

                    if (unit.CurrentHealthPercent > ScholarSettings.Instance.AdloquiumHpPercent)
                        return false;

                    if (unit.HasAura(Auras.Galvanize))
                        return false;

                    if (unit.HasAura(Auras.Excogitation))
                        return false;

                    if (!ScholarSettings.Instance.AdloquiumOnlyHealer && !ScholarSettings.Instance.AdloquiumOnlyTank)
                        return true;

                    if (ScholarSettings.Instance.AdloquiumOnlyHealer && unit.IsHealer())
                        return true;

                    return ScholarSettings.Instance.AdloquiumOnlyTank && unit.IsTank();
                }
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.AdloquiumHpPercent || Core.Me.HasAura(Auras.Galvanize))
                return false;

            return await Spells.Adloquium.HealAura(Core.Me, Auras.Galvanize);

            async Task UseRecitation()
            {
                if (!ScholarSettings.Instance.Recitation)
                    return;
                if (!ScholarSettings.Instance.RecitationWithAdlo)
                    return;
                if (Spells.Recitation.Cooldown != TimeSpan.Zero)
                    return;
                if (ScholarSettings.Instance.RecitationOnlyNoAetherflow && Core.Me.HasAetherflow())
                    return;
                if (!await Spells.Recitation.Cast(Core.Me))
                    return;
                if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Recitation)))
                    return;
                await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Adloquium.Id, Core.Me));
            }
        }

        public static async Task<bool> EmergencyTacticsSuccor()
        {
            if (!ScholarSettings.Instance.Succor || !ScholarSettings.Instance.EmergencyTactics || !ScholarSettings.Instance.EmergencyTacticsSuccor)
                return false;

            if (Spells.EmergencyTactics.Cooldown != TimeSpan.Zero)
                return false;

            var needSuccor = Group.CastableAlliesWithin15.Count(r => r.IsAlive &&
                                                                     r.CurrentHealthPercent <= ScholarSettings.Instance.EmergencyTacticsSuccorHealthPercent) >= AoeNeedHealing;

            if (!needSuccor)
                return false;

            if (!await Buff.EmergencyTactics())
                return false;

            if (await Spells.Succor.Heal(Core.Me))
                return await Coroutine.Wait(2500, () => Casting.LastSpell == Spells.Succor || MovementManager.IsMoving);

            return false;
        }

        public static async Task<bool> Succor()
        {
            if (!ScholarSettings.Instance.Succor)
                return false;

            //if (Casting.LastSpell == Spells.Indomitability)
            //    return false;

            //if (Casting.LastSpell == Spells.Succor)
            //    return false;

            var needSuccor = Group.CastableAlliesWithin15.Count(r => r.IsAlive &&
                                                                     r.CurrentHealthPercent <= ScholarSettings.Instance.SuccorHpPercent &&
                                                                     !r.HasAura(Auras.Galvanize)) >= AoeNeedHealing;

            if (!needSuccor)
                return false;

            if (await Spells.Succor.Heal(Core.Me))
            {
                return await Coroutine.Wait(2500, () => Casting.LastSpell == Spells.Succor || MovementManager.IsMoving);
            }

            return false;
        }

        public static async Task<bool> Excogitation()
        {
            if (!ScholarSettings.Instance.Excogitation)
                return false;

            if (!Core.Me.HasAetherflow())
                return false;

            if (Spells.Excogitation.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty)
            {
                var excogitationTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanExcogitation);

                if (excogitationTarget == null)
                    return false;

                await UseRecitation();

                return await Spells.Excogitation.Cast(excogitationTarget);
            }

            if (Core.Me.HasAura(Auras.Excogitation))
                return false;

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.ExcogitationHpPercent)
                return false;

            await UseRecitation();

            return await Spells.Excogitation.Cast(Core.Me);

            bool CanExcogitation(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.HasAura(Auras.Excogitation))
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.ExcogitationHpPercent)
                    return false;

                if (Casting.LastSpellTarget?.ObjectId == unit.ObjectId)
                {
                    if (Casting.LastSpell == Spells.Lustrate || Casting.LastSpell == Spells.Excogitation)
                        return false;
                }

                if (!ScholarSettings.Instance.ExcogitationOnlyHealer && !ScholarSettings.Instance.ExcogitationOnlyTank)
                    return true;

                if (ScholarSettings.Instance.ExcogitationOnlyHealer && unit.IsHealer())
                    return true;

                return ScholarSettings.Instance.ExcogitationOnlyTank && unit.IsTank();
            }
            async Task UseRecitation()
            {
                if (!ScholarSettings.Instance.Recitation)
                    return;

                if (!ScholarSettings.Instance.RecitationWithExcog)
                    return;
                if (Spells.Recitation.Cooldown != TimeSpan.Zero)
                    return;
                if (ScholarSettings.Instance.RecitationOnlyNoAetherflow && Core.Me.HasAetherflow())
                    return;

                if (!await Spells.Recitation.Cast(Core.Me))
                    return;

                if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Recitation)))
                    return;

                await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Excogitation.Id, Core.Me));
            }
        }

        public static async Task<bool> Lustrate()
        {
            if (!ScholarSettings.Instance.Lustrate)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Core.Me.HasAetherflow())
                return false;

            if (Spells.Lustrate.Cooldown != TimeSpan.Zero)
                return false;

            if (Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= ScholarSettings.Instance.IndomitabilityHpPercent) > AoeNeedHealing)
                return false;

            if (Globals.InParty)
            {
                var lustrateTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanLustrate);

                if (lustrateTarget == null)
                    return false;

                await UseRecitation();

                return await Spells.Lustrate.Cast(lustrateTarget);
            }

            if (Core.Me.HasAura(Auras.Excogitation))
                return false;

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.LustrateHpPercent)
                return false;

            await UseRecitation();

            return await Spells.Lustrate.Cast(Core.Me);

            bool CanLustrate(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.HasAura(Auras.Excogitation))
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.LustrateHpPercent)
                    return false;

                if (Casting.LastSpellTarget?.ObjectId == unit.ObjectId)
                {
                    if (Casting.LastSpell == Spells.Lustrate || Casting.LastSpell == Spells.Excogitation)
                        return false;
                }

                if (!ScholarSettings.Instance.LustrateOnlyHealer && !ScholarSettings.Instance.LustrateOnlyTank)
                    return true;

                if (ScholarSettings.Instance.LustrateOnlyHealer && unit.IsHealer())
                    return true;

                return ScholarSettings.Instance.LustrateOnlyTank && unit.IsTank();
            }

            async Task UseRecitation()
            {
                if (!ScholarSettings.Instance.Recitation)
                    return;

                if (!ScholarSettings.Instance.RecitationWithLustrate)
                    return;

                if (ScholarSettings.Instance.RecitationOnlyNoAetherflow && Core.Me.HasAetherflow())
                    return;

                if (!await Spells.Recitation.Cast(Core.Me))
                    return;

                if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Recitation)))
                    return;

                await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Lustrate.Id, Core.Me));
            }
        }

        public static async Task<bool> Indomitability()
        {
            if (!ScholarSettings.Instance.Indomitability)
                return false;

            if (!Core.Me.HasAetherflow())
                return false;

            if (Spells.Indomitability.Cooldown != TimeSpan.Zero)
                return false;

            if (Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= ScholarSettings.Instance.IndomitabilityHpPercent) < AoeNeedHealing)
                return false;

            await UseRecitation();

            return await Spells.Indomitability.Cast(Core.Me);

            async Task UseRecitation()
            {
                if (!ScholarSettings.Instance.Recitation)
                    return;

                if (!ScholarSettings.Instance.RecitationWithIndomitability)
                    return;

                if (ScholarSettings.Instance.RecitationOnlyNoAetherflow && Core.Me.HasAetherflow())
                    return;

                if (!await Spells.Recitation.Cast(Core.Me))
                    return;

                if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Recitation)))
                    return;

                await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Indomitability.Id, Core.Me));
            }
        }

        public static async Task<bool> SacredSoil()
        {
            if (!ScholarSettings.Instance.SacredSoil)
                return false;

            // Extra double cast spell since Sacred Soil is a quick animation instant spell
            if (Casting.LastSpell == Spells.SacredSoil)
                return false;

            if (!Core.Me.HasAetherflow())
                return false;

            if (Spells.SacredSoil.Cooldown != TimeSpan.Zero)
                return false;

            var sacredSoilTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => PartyManager.VisibleMembers.Count(x => x.BattleCharacter.CurrentHealthPercent < ScholarSettings.Instance.SacredSoilHpPercent
                    && x.BattleCharacter.Distance(r) <= 15) >= AoeNeedHealing);

            if (sacredSoilTarget == null)
                return false;

            return await Spells.SacredSoil.Cast(sacredSoilTarget);
        }

        public static async Task<bool> Resurrection()
        {
            return await Roles.Healer.Raise(
                Spells.Resurrection,
                ScholarSettings.Instance.SwiftcastRes,
                ScholarSettings.Instance.SlowcastRes,
                ScholarSettings.Instance.ResOutOfCombat
            );
        }

        public static async Task<bool> WhisperingDawn()
        {
            if (!ScholarSettings.Instance.WhisperingDawn)
                return false;

            if (Core.Me.Pet == null)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.WhisperingDawn.Cooldown != TimeSpan.Zero)
                return false;

            if (ScholarSettings.Instance.WhisperingDawnOnlyWithSeraph && Core.Me.Pet.EnglishName != "Seraph")
                return false;

            if (ScholarSettings.Instance.ForceWhisperingDawnWithSeraph && Utilities.Routines.Scholar.SeraphTimeRemaining() < 15)
                return await Spells.WhisperingDawn.Cast(Core.Me);

            if (Globals.InParty)
            {
                var canWhisperingDawnTargets = Group.CastableAlliesWithin30.Where(CanWhisperingDawn).ToList();

                if (canWhisperingDawnTargets.Count < AoeNeedHealing)
                    return false;

                if (ScholarSettings.Instance.WhisperingDawnOnlyWithTank && !canWhisperingDawnTargets.Any(r => r.IsTank()))
                    return false;

                return await Spells.WhisperingDawn.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.WhisperingDawnHealthPercent)
                return false;

            return await Spells.WhisperingDawn.Cast(Core.Me);

            bool CanWhisperingDawn(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.WhisperingDawnHealthPercent)
                    return false;

                return unit.Distance(Core.Me.Pet) <= 15;
            }
        }

        public static async Task<bool> FeyIllumination()
        {
            if (!ScholarSettings.Instance.FeyIllumination)
                return false;

            if (Core.Me.Pet == null)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.FeyIllumination.Cooldown != TimeSpan.Zero)
                return false;

            if (ScholarSettings.Instance.FeyIlluminationOnlyWithSeraph && Core.Me.Pet.EnglishName != "Seraph")
                return false;

            if (ScholarSettings.Instance.ForceFeyIlluminationWithSeraph && Utilities.Routines.Scholar.SeraphTimeRemaining() < 15)
                return await Spells.FeyIllumination.Cast(Core.Me);

            if (Globals.InParty)
            {
                var canFeyIlluminationTargets = Group.CastableAlliesWithin30.Where(CanFeyIllumination).ToList();

                if (canFeyIlluminationTargets.Count < AoeNeedHealing)
                    return false;

                if (ScholarSettings.Instance.FeyIlluminationOnlyWithTank && !canFeyIlluminationTargets.Any(r => r.IsTank()))
                    return false;

                return await Spells.FeyIllumination.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.FeyIlluminationHpPercent)
                return false;

            return await Spells.FeyIllumination.Cast(Core.Me);

            bool CanFeyIllumination(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.FeyIlluminationHpPercent)
                    return false;

                //Radius is now 30y
                return unit.Distance(Core.Me.Pet) <= 30;
            }
        }

        public static async Task<bool> FeyBlessing()
        {
            if (!ScholarSettings.Instance.FeyBlessing)
                return false;

            if (Core.Me.Pet == null)
                return false;

            if (Core.Me.Pet.EnglishName == "Seraph")
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.FeyBlessing.Cooldown != TimeSpan.Zero)
                return false;

            if (ActionResourceManager.Scholar.FaerieGauge < ScholarSettings.Instance.FeyBlessingMinimumFairieGauge)
                return false;

            if (Globals.InParty)
            {
                var canFeyBlessingTargets = Group.CastableAlliesWithin30.Where(CanFeyBlessing).ToList();

                if (canFeyBlessingTargets.Count < AoeNeedHealing)
                    return false;

                if (ScholarSettings.Instance.FeyBlessingOnlyWithTank && !canFeyBlessingTargets.Any(r => r.IsTank()))
                    return false;

                return await Spells.FeyBlessing.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.FeyBlessingHpPercent)
                return false;

            return await Spells.FeyBlessing.Cast(Core.Me);

            bool CanFeyBlessing(Character unit)
            {
                if (unit == null)
                    return false;
                if (unit.CurrentHealthPercent > ScholarSettings.Instance.FeyBlessingHpPercent)
                    return false;

                return unit.Distance(Core.Me.Pet) <= 20;
            }
        }

        // Prevent blowing the second consolation stack before seraph gets a chance to cast
        // the first one.
        public static DateTime ConsolationCooldown = DateTime.Now;

        public static async Task<bool> Consolation()
        {
            if (!ScholarSettings.Instance.Consolation)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.Pet == null)
                return false;

            if (Core.Me.Pet.EnglishName != "Seraph")
                return false;

            if (DateTime.Now <= ConsolationCooldown)
                return false;

            if (Utilities.Routines.Scholar.SeraphTimeRemaining() <= 6.5)
                return await Spells.Consolation.Cast(Core.Me);

            if (Globals.InParty)
            {
                var canConsolationTargets = Group.CastableAlliesWithin30.Where(CanConsolation).ToList();

                if (canConsolationTargets.Count < AoeNeedHealing)
                    return false;

                if (Utilities.Routines.Scholar.SeraphTimeRemaining() >= 10 && ScholarSettings.Instance.ConsolationOnlyWithTank && !canConsolationTargets.Any(r => r.IsTank()))
                    return false;

                ConsolationCooldown = DateTime.Now.AddSeconds(5);

                return await Spells.Consolation.Cast(Core.Me);
            }

            if (Utilities.Routines.Scholar.SeraphTimeRemaining() >= 10 && Core.Me.CurrentHealthPercent > ScholarSettings.Instance.ConsolationHpPercent)
                return false;

            ConsolationCooldown = DateTime.Now.AddSeconds(5);

            return await Spells.Consolation.Cast(Core.Me);

            bool CanConsolation(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.ConsolationHpPercent)
                    return false;

                if (unit.HasAura(Auras.SeraphicVeil))
                    return false;


                //Range is now 30y
                return unit.Distance(Core.Me.Pet) <= 30;
            }
        }

        /**********************************************************************************************
        *                              Limit Break
        * ********************************************************************************************/
        public static bool ForceLimitBreak()
        {
            return Healer.ForceLimitBreak(Spells.HealingWind, Spells.BreathoftheEarth, Spells.AngelFeathers, Spells.Ruin);
        }
    }
}
