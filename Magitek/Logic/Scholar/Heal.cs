using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
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

        public static async Task<bool> Physick()
        {
            if (!ScholarSettings.Instance.Physick)
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

        public static async Task<bool> EmergencyTacticsAdloquium() {
            if (!ScholarSettings.Instance.Adloquium || !ScholarSettings.Instance.EmergencyTacticsAdloquium)
                return false;

            if (Spells.EmergencyTactics.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty) {
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

            bool CanAdlo(Character unit) {
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

                bool CanAdlo(Character unit) {
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
        
        public static async Task<bool> EmergencyTacticsSuccor() {
            if (!ScholarSettings.Instance.Succor || !ScholarSettings.Instance.EmergencyTactics || !ScholarSettings.Instance.EmergencyTacticsSuccor)
                return false;

            if (Spells.EmergencyTactics.Cooldown != TimeSpan.Zero)
                return false;

            var needSuccor = Group.CastableAlliesWithin15.Count(r => r.IsAlive &&
                                                                     r.CurrentHealthPercent <= ScholarSettings.Instance.EmergencyTacticsSuccorHealthPercent) >= ScholarSettings.Instance.SuccorNeedHealing;

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
                                                                     !r.HasAura(Auras.Galvanize)) >= ScholarSettings.Instance.SuccorNeedHealing;

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

            if (Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= ScholarSettings.Instance.IndomitabilityHpPercent) > ScholarSettings.Instance.IndomitabilityNeedHealing)
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

            bool CanLustrate(Character unit) {
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

            if (Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= ScholarSettings.Instance.IndomitabilityHpPercent) < ScholarSettings.Instance.IndomitabilityNeedHealing)
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
                    && x.BattleCharacter.Distance(r) <= 15) >= ScholarSettings.Instance.SacredSoilNeedHealing);

            if (sacredSoilTarget == null)
                return false;

            return await Spells.SacredSoil.Cast(sacredSoilTarget);
        }

        public static async Task<bool> Resurrection()
        {
            if (!Globals.InParty)
                return false;

            var deadList = Group.DeadAllies.Where(u => u.CurrentHealth == 0 &&
                                                       !u.HasAura(Auras.Raise) &&
                                                       u.Distance(Core.Me) <= 30)
                .OrderByDescending(r => r.GetResurrectionWeight());

            var deadTarget = deadList.FirstOrDefault();

            if (deadTarget == null)
                return false;

            if (!deadTarget.IsVisible || !deadTarget.IsTargetable)
                return false;
            
            if (Globals.PartyInCombat) {
                if (ScholarSettings.Instance.SwiftcastRes && Spells.Swiftcast.Cooldown == TimeSpan.Zero) {
                    if (await Buff.Swiftcast()) {
                        while (Core.Me.HasAura(Auras.Swiftcast)) {
                            if (await Spells.Resurrection.CastAura(deadTarget, Auras.Raise))
                                return true;
                            await Coroutine.Yield();
                        }
                    }
                }
            }

            if (Globals.PartyInCombat && ScholarSettings.Instance.SlowcastRes || !Globals.PartyInCombat && ScholarSettings.Instance.ResOutOfCombat) {
                //delay casting raise on the same target in case they are already in the resurrect animation and the buff is gone for some reason
                //but this shouldn't be needed outside of Trust NPCs
                //if(Casting.SpellCastHistory.Take(5).Any(s => s.Spell == Spells.Resurrection && s.SpellTarget == deadTarget))
                //    return false;
                
                return await Spells.Resurrection.CastAura(deadTarget, Auras.Raise);
            }
            
            return false;
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

            if (Globals.InParty)
            {
                var canWhisperingDawnTargets = Group.CastableAlliesWithin30.Where(CanWhisperingDawn).ToList();

                if (canWhisperingDawnTargets.Count < ScholarSettings.Instance.WhisperingDawnNeedHealing)
                    return false;

                if (ScholarSettings.Instance.WhisperingDawnOnlyWithTank && !canWhisperingDawnTargets.Any(r => r.IsTank()))
                    return false;

                return await Spells.WhisperingDawn.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.WhisperingDawnHealthPercent)
                return false;

            return await Spells.WhisperingDawn.Cast(Core.Me);

            bool CanWhisperingDawn(Character unit) {
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

            if (Globals.InParty)
            {
                var canFeyIlluminationTargets = Group.CastableAlliesWithin30.Where(CanFeyIllumination).ToList();

                if (canFeyIlluminationTargets.Count < ScholarSettings.Instance.FeyIlluminationNeedHealing)
                    return false;

                if (ScholarSettings.Instance.FeyIlluminationOnlyWithTank && !canFeyIlluminationTargets.Any(r => r.IsTank()))
                    return false;

                return await Spells.FeyIllumination.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.FeyIlluminationHpPercent)
                return false;

            return await Spells.FeyIllumination.Cast(Core.Me);

            bool CanFeyIllumination(Character unit) {
                if (unit == null)
                    return false;
                if (unit.CurrentHealthPercent > ScholarSettings.Instance.FeyIlluminationHpPercent)
                    return false;

                return unit.Distance(Core.Me.Pet) <= 15;
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

                if (canFeyBlessingTargets.Count < ScholarSettings.Instance.FeyBlessingNeedHealing)
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

        public static async Task<bool> SummonSeraph()
        {
            if (!ScholarSettings.Instance.SummonSeraph)
                return false;

            if (Core.Me.Pet == null)
                return false;

            if (!Core.Me.InCombat)
                return false;

            // check if seraph is already active
            if (Core.Me.Pet.EnglishName == "Seraph")
                return false;

            if (Globals.InParty)
            {
                if (Group.CastableAlliesWithin30.Count(CanSummonSeraph) < ScholarSettings.Instance.SummonSeraphNeedHealing)
                    return false;

                return await Spells.SummonSeraph.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.SummonSeraphHpPercent)
                return false;

            return await Spells.SummonSeraph.Cast(Core.Me);

            bool CanSummonSeraph(Character unit)
            {
                if (unit == null)
                    return false;
                return unit.CurrentHealthPercent < ScholarSettings.Instance.SummonSeraphHpPercent;
            }
        }

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

            if (Globals.InParty)
            {
                var canConsolationTargets = Group.CastableAlliesWithin20.Where(CanConsolation).ToList();

                if (canConsolationTargets.Count < ScholarSettings.Instance.ConsolationNeedHealing)
                    return false;

                if (ScholarSettings.Instance.ConsolationOnlyWithTank && !canConsolationTargets.Any(r => r.IsTank()))
                    return false;
                
                return await Spells.Consolation.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.ConsolationHpPercent)
                return false;
            
            return await Spells.Consolation.Cast(Core.Me);

            bool CanConsolation(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.ConsolationHpPercent)
                    return false;

                if (unit.HasAura(Auras.SeraphicVeil))
                    return false;

                return unit.Distance(Core.Me.Pet) <= 20;
            }
        }
    }
}
