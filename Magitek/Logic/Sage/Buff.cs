using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Sage;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Sage
{
    internal static class Buff
    {
        public static async Task<bool> Swiftcast()
        {
            if (await Spells.Swiftcast.CastAura(Core.Me, Auras.Swiftcast))
            {
                return await Coroutine.Wait(15000, () => Core.Me.HasAura(Auras.Swiftcast, true, 7000));
            }

            return false;
        }
        public static async Task<bool> LucidDreaming()
        {
            if (!SageSettings.Instance.LucidDreaming)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.CurrentManaPercent > SageSettings.Instance.LucidDreamingManaPercent)
                return false;

            if (!Globals.InParty)
                return await Spells.LucidDreaming.CastAura(Core.Me, Auras.LucidDreaming);

            if (Combat.CombatTotalTimeLeft <= 20)
                return false;

            return await Spells.LucidDreaming.Cast(Core.Me);
        }
        public static async Task<bool> Kardia()
        {
            if (!SageSettings.Instance.Kardia)
                return false;

            if (Spells.Kardia.Cooldown != TimeSpan.Zero)
                return false;

            if (!Globals.InParty)
            {
                var currentKardiaTarget = Group.CastableAlliesWithin30.Where(a => a.HasAura(Auras.Kardion, true)).FirstOrDefault();

                if (SageSettings.Instance.KardiaSwitchTargets)
                {
                    var canKardiaTargets = Group.CastableAlliesWithin30.Where(CanKardia).ToList();

                    if (canKardiaTargets.Contains(currentKardiaTarget))
                        return false;

                    var kardiaTarget = canKardiaTargets.FirstOrDefault();

                    if (kardiaTarget == null)
                        return false;

                    return await Spells.Kardia.CastAura(kardiaTarget, Auras.Kardia);
                }
                else
                {
                    var kardiaTarget = Group.CastableAlliesWithin30.Where(a => a.IsAlive && a.IsMainTank()).FirstOrDefault();

                    if (kardiaTarget == null)
                        return false;

                    if (kardiaTarget == currentKardiaTarget)
                        return false;

                    return await Spells.Kardia.CastAura(kardiaTarget, Auras.Kardia);
                }
            }
            else
            {
                if (Core.Me.HasAura(Auras.Kardia, true)
                || Core.Me.HasAura(Auras.Kardion))
                    return false;

                if (ChocoboManager.Summoned)
                {
                    return await Spells.Kardia.CastAura(ChocoboManager.Object, Auras.Kardia);
                }
                return await Spells.Kardia.CastAura(Core.Me, Auras.Kardia);
            }

            bool CanKardia(Character unit)
            {
                if (unit == null)
                    return false;

                if (!unit.IsAlive)
                    return false;

                if (unit.CurrentHealthPercent > SageSettings.Instance.KardiaSwitchTargetsHealthPercent)
                    return false;

                if (!SageSettings.Instance.KardiaTank && unit.IsTank())
                    return false;

                if (!SageSettings.Instance.KardiaHealer && unit.IsHealer())
                    return false;

                if (!SageSettings.Instance.KardiaDps && unit.IsDps())
                    return false;

                return unit.Distance(Core.Me) <= 30;
            }
        }
        public static async Task<bool> Soteria()
        {
            if (!SageSettings.Instance.Soteria)
                return false;

            if (Core.Me.ClassLevel < Spells.Soteria.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Spells.Soteria.Cooldown != TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.Soteria))
                return false;

            var kardionTarget = Group.CastableAlliesWithin30.Where(r => r.HasAura(Auras.Kardion, true)).FirstOrDefault();

            if (kardionTarget == null)
                return false;

            if (kardionTarget.CurrentHealthPercent > SageSettings.Instance.SoteriaHealthPercent)
                return false;

            if (SageSettings.Instance.SoteriaTankOnly && !kardionTarget.IsTank())
                return false;

            return await Spells.Soteria.CastAura(Core.Me, Auras.Soteria);
        }
        public static async Task<bool> Kerachole()
        {
            if (!SageSettings.Instance.Kerachole)
                return false;

            if (Core.Me.ClassLevel < Spells.Kerachole.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.Kerachole.Cooldown != TimeSpan.Zero)
                return false;

            if (Addersgall == 0)
                return false;

            if (Core.Me.HasAura(Auras.Taurochole))
                return false;

            if (Globals.InParty)
            {
                var canKeracholeTargets = Group.CastableAlliesWithin30.Where(CanKerachole).ToList();

                if (canKeracholeTargets.Count < SageSettings.Instance.KeracholeNeedHealing)
                    return false;

                if (SageSettings.Instance.KeracholeOnlyWithTank && !canKeracholeTargets.Any(r => r.IsTank()))
                    return false;

                return await Spells.Kerachole.Cast(Core.Me);
            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.KeracholeHealthPercent)
                return false;

            return await Spells.Kerachole.Cast(Core.Me);

            bool CanKerachole(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > SageSettings.Instance.KeracholeHealthPercent)
                    return false;

                return unit.Distance(Core.Me) <= 15;
            }
        }
        public static async Task<bool> Rhizomata()
        {
            if (!SageSettings.Instance.Rhizomata)
                return false;

            if (Core.Me.ClassLevel < Spells.Rhizomata.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Spells.Rhizomata.Cooldown != TimeSpan.Zero)
                return false;

            if (Addersgall >= 2)
                return false;

            return await Spells.Rhizomata.Cast(Core.Me);
        }
        public static async Task<bool> Holos()
        {
            if (!SageSettings.Instance.Holos)
                return false;

            if (Core.Me.ClassLevel < Spells.Holos.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Spells.Holos.Cooldown != TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.Soteria))
                return false;

            if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.HolosHealthPercent) < 2)
                return false;

            GameObject target = SageSettings.Instance.HolosTankOnly
                ? Group.CastableTanks.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.HolosHealthPercent
                && r.IsTank())
                : Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.HolosHealthPercent);

            if (target == null)
                return false;

            return await Spells.Holos.Heal(Core.Me);
        }
        public static async Task<bool> Krasis()
        {
            if (!SageSettings.Instance.Krasis)
                return false;

            if (Core.Me.ClassLevel < Spells.Krasis.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (Spells.Krasis.Cooldown != TimeSpan.Zero)
                return false;

            if (Core.Me.HasAura(Auras.Krasis))
                return false;

            GameObject target = SageSettings.Instance.KrasisTankOnly
                ? Group.CastableTanks.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.KrasisHealthPercent
                && r.IsTank() && !r.HasAura(Auras.Krasis))
                : Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.KrasisHealthPercent
                && !r.HasAura(Auras.Krasis));

            if (target == null)
                return false;

            return await Spells.Krasis.CastAura(target, Auras.Krasis);
        }

    }
}
