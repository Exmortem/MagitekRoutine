using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Scholar;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Scholar
{
    internal static class Heal
    {
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
            else
            {
                if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.PhysickHpPercent)
                    return false;

                return await Spells.Physick.Heal(Core.Me);
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

                    return await Spells.Adloquium.HealAura(tankAdloTarget, Auras.Galvanize, false);
                }

                var adloTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanAdlo);
                
                if (adloTarget == null)
                    return false;

                return await Spells.Adloquium.HealAura(adloTarget, Auras.Galvanize);

                bool CanAdlo(Character unit)
                {
                    if (unit.CurrentHealthPercent > ScholarSettings.Instance.AdloquiumHpPercent)
                        return false;

                    if (unit.HasAura(Auras.Galvanize))
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
        }

        public static async Task<bool> EmergencyTacticsAdlo()
        {
            if (!ScholarSettings.Instance.EmergencyTacticsAdloquium)
                return false;

            if (!PartyManager.IsInParty)
                return false;

            if (!ScholarSettings.Instance.EmergencyTactics || !ScholarSettings.Instance.EmergencyTacticsAdloquium)
                return false;

            var emergencyTacticsAdloTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanEmergencyTacticsAdlo);

            if (emergencyTacticsAdloTarget == null)
                return false;

            return await CastEmergencyTacticsAdlo(emergencyTacticsAdloTarget);

            bool CanEmergencyTacticsAdlo(Character unit)
            {
                if (unit.HasAura(Auras.Exogitation))
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.EmergencyTacticsAdloquiumHealthPercent)
                    return false;

                if (Casting.LastSpellTarget.ObjectId == unit.ObjectId)
                {
                    if (Casting.LastSpell == Spells.Lustrate || Casting.LastSpell == Spells.Excogitation)
                        return false;
                }

                if (!ScholarSettings.Instance.AdloquiumOnlyHealer && !ScholarSettings.Instance.AdloquiumOnlyTank)
                    return true;

                if (ScholarSettings.Instance.AdloquiumOnlyHealer && unit.IsHealer())
                    return true;

                return ScholarSettings.Instance.AdloquiumOnlyTank && unit.IsTank();
            }

            async Task<bool> CastEmergencyTacticsAdlo(Character unit)
            {
                if (!await Spells.EmergencyTactics.Cast(Core.Me))
                    return false;

                await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.EmergencyTactics));

                while (Core.Me.HasAura(Auras.EmergencyTactics))
                {
                    if (MovementManager.IsMoving)
                        await Coroutine.Yield();

                    if (await Spells.Adloquium.Heal(emergencyTacticsAdloTarget, false))
                        return true;

                    await Coroutine.Yield();
                }

                return false;
            }
        }

        public static async Task<bool> Succor()
        {
            if (!ScholarSettings.Instance.Succor)
                return false;

            if (Casting.LastSpell == Spells.Indomitability)
                return false;

            if (ScholarSettings.Instance.EmergencyTacticsSuccor)
            {
                if (Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= ScholarSettings.Instance.EmergencyTacticsSuccorHealthPercent) >= ScholarSettings.Instance.SuccorNeedHealing)
                {
                    if (await EmergencyTacticsSuccor()) return true;
                }
            }

            if (Casting.LastSpell == Spells.Succor)
                return false;

            var needSuccor = Group.CastableAlliesWithin15.Count(r => r.IsAlive &&
                                                                     r.CurrentHealthPercent <= ScholarSettings.Instance.SuccorHpPercent &&
                                                                     !r.HasAura(Auras.Galvanize)) >= ScholarSettings.Instance.SuccorNeedHealing;

            if (!needSuccor)
                return false;

            if (await Spells.Succor.Heal(Core.Me))
            {
                return await Coroutine.Wait(2000, () => Casting.LastSpell == Spells.Succor || MovementManager.IsMoving);
            }

            return false;
        }

        private static async Task<bool> EmergencyTacticsSuccor()
        {
            if (!ScholarSettings.Instance.EmergencyTactics || !ScholarSettings.Instance.EmergencyTacticsSuccor)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (!await Buff.EmergencyTactics())
                return false;

            while (Core.Me.HasAura(Auras.EmergencyTactics))
            {
                // We did a movement check earlier, but the player may have started moving at this point
                // We don't want to waste the Emergency Tactics aura, so we wait until the player stops moving
                if (MovementManager.IsMoving)
                {
                    await Coroutine.Yield();
                }

                if (await Spells.Succor.Heal(Core.Me, false)) return true;
                await Coroutine.Yield();
            }
            return false;
        }

        public static async Task<bool> Excogitation()
        {
            if (!ScholarSettings.Instance.Excogitation)
                return false;

            if (!Core.Me.HasAetherflow())
                return false;

            if (Globals.InParty)
            {
                var excogitationTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanExcogitation);

                if (excogitationTarget == null)
                    return false;

                return await Spells.Excogitation.CastAura(excogitationTarget, Auras.Exogitation);
            }

            if (Core.Me.HasAura(Auras.Exogitation))
                return false;

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.ExcogitationHpPercent)
                return false;

            return await Spells.Excogitation.CastAura(Core.Me, Auras.Exogitation);

            bool CanExcogitation(Character unit)
            {
                if (unit.HasAura(Auras.Exogitation))
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.ExcogitationHpPercent)
                    return false;

                if (Casting.LastSpellTarget.ObjectId == unit.ObjectId)
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
        }

        public static async Task<bool> Lustrate()
        {
            if (!ScholarSettings.Instance.Lustrate)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Core.Me.HasAetherflow())
                return false;

            if (Globals.InParty)
            {
                var lustrateTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanLustrate);

                if (lustrateTarget == null)
                    return false;

                await UseRecitation();
                
                return await Spells.Lustrate.Cast(lustrateTarget);
            }

            if (Core.Me.HasAura(Auras.Exogitation))
                return false;

            if (Core.Me.CurrentHealthPercent > ScholarSettings.Instance.LustrateHpPercent)
                return false;

            await UseRecitation();

            return await Spells.Lustrate.Cast(Core.Me);

            bool CanLustrate(Character unit)
            {
                if (unit.HasAura(Auras.Exogitation))
                    return false;

                if (unit.CurrentHealthPercent > ScholarSettings.Instance.LustrateHpPercent)
                    return false;

                if (Casting.LastSpellTarget.ObjectId == unit.ObjectId)
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

            if (!deadTarget.IsVisible)
                return false;

            if (!deadTarget.IsTargetable)
                return false;

            if (Globals.PartyInCombat)
            {
                if (!ScholarSettings.Instance.SwiftcastRes)
                    return false;

                if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                    return false;

                if (await Buff.Swiftcast())
                {
                    while (Core.Me.HasAura(Auras.Swiftcast))
                    {
                        if (await Spells.Resurrection.Cast(deadTarget)) return true;
                        await Coroutine.Yield();
                    }
                }
            }

            if (Globals.PartyInCombat)
                return false;

            if (!ScholarSettings.Instance.ResOutOfCombat)
                return false;

            return await Spells.Resurrection.CastAura(deadTarget, Auras.Raise);
        }

        public static async Task<bool> WhisperingDawn()
        {
            if (!ScholarSettings.Instance.WhisperingDawn)
                return false;

            if (Core.Me.Pet == null)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (PartyManager.IsInParty)
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

            bool CanWhisperingDawn(Character unit)
            {
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

            if (PartyManager.IsInParty)
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

            bool CanFeyIllumination(Character unit)
            {
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

            if (!Core.Me.InCombat)
                return false;

            if (PartyManager.IsInParty)
            {
                var canFeyBlessingTargets = Group.CastableAlliesWithin30.Where(CanFeyBlessing).ToList();

                if (canFeyBlessingTargets.Count< ScholarSettings.Instance.FeyBlessingNeedHealing)
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
                if (unit.CurrentHealthPercent > ScholarSettings.Instance.FeyBlessingHpPercent)
                    return false;

                return unit.Distance(Core.Me.Pet) <= 15;
            }
        }
    }
}
