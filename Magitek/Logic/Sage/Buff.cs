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
            return await Roles.Healer.LucidDreaming(SageSettings.Instance.LucidDreaming, SageSettings.Instance.LucidDreamingManaPercent);
        }
        public static async Task<bool> Kardia()
        {
            if (!SageSettings.Instance.Kardia)
                return false;

            if (!Spells.Kardia.IsKnownAndReady())
                return false;

            if (Casting.LastSpell == Spells.Kardia || Casting.CastingSpell == Spells.Kardia)
                return false;

            if (Globals.InParty)
            {
                var currentKardiaTarget = Group.CastableAlliesWithin30.Where(a => a.HasAura(Auras.Kardion, true)).FirstOrDefault();

                if (SageSettings.Instance.KardiaSwitchTargets
                    && currentKardiaTarget != null
                    && Core.Me.InCombat
                    && (!SageSettings.Instance.KardiaSwitchTargetsCurrent || currentKardiaTarget.CurrentHealthPercent >= SageSettings.Instance.KardiaSwitchTargetsCurrentHealthPercent))
                {
                    var canKardiaTargets = Group.CastableAlliesWithin30.Where(CanKardia).Where(CanKardiaSwitch).OrderByDescending(KardiaPriority).ToList();

                    if (canKardiaTargets.Contains(currentKardiaTarget))
                        return false;

                    var kardiaTargetSwitch = canKardiaTargets.FirstOrDefault();

                    if (kardiaTargetSwitch != null)
                        return await Spells.Kardia.Cast(kardiaTargetSwitch);
                    return false;
                }
                else
                {
                    if (Core.Me.HasAura(Auras.Kardia, true))
                        return false;

                    var kardiaTarget = Group.CastableAlliesWithin30.Where(CanKardia).OrderByDescending(KardiaPriority).FirstOrDefault();

                    if (kardiaTarget == null)
                        return false;

                    if (kardiaTarget == currentKardiaTarget)
                        return false;

                    return await Spells.Kardia.Cast(kardiaTarget);
                }
            }
            else
            {
                if (Core.Me.HasAura(Auras.Kardia, true)
                || Core.Me.HasAura(Auras.Kardion, true))
                    return false;

                if (ChocoboManager.Summoned)
                {
                    return await Spells.Kardia.Cast(ChocoboManager.Object);
                }
                return await Spells.Kardia.Cast(Core.Me);
            }

            bool CanKardiaSwitch(Character unit)
            {
                if (unit.CurrentHealthPercent > SageSettings.Instance.KardiaSwitchTargetsHealthPercent)
                    return false;

                return true;
            }

            bool CanKardia(Character unit)
            {
                if (unit == null)
                    return false;

                if (!unit.IsAlive)
                    return false;

                if (unit.Distance(Core.Me) > 30)
                    return false;

                if (unit.IsHealer() && SageSettings.Instance.KardiaHealer)
                    return true;

                if (unit.IsDps() && SageSettings.Instance.KardiaDps)
                    return true;

                if (unit.IsMainTank() && SageSettings.Instance.KardiaMainTank)
                    return true;

                if (unit.IsTank() && SageSettings.Instance.KardiaTank)
                    return true;

                return false;
            }

            int KardiaPriority(Character unit)
            {
                if (unit.IsMainTank())
                    return 100;
                if (unit.IsTank())
                    return 90;
                if (unit.IsHealer())
                    return 80;
                if (unit.IsDps())
                    return 70;
                return 0;
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
                var canKeracholeTargets = Group.CastableAlliesWithin15.Where(CanKerachole).ToList();

                if (canKeracholeTargets.Count < SageSettings.Instance.KeracholeNeedHealing)
                    return false;

                if (SageSettings.Instance.KeracholeOnlyWithTank && !Group.CastableAlliesWithin15.Any(r => r.IsTank(SageSettings.Instance.KeracholeOnlyWithMainTank)))
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

                //Updated to 30y
                return unit.Distance(Core.Me) <= 30;
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

            var targets = Group.CastableAlliesWithin30.Where(r => r.CurrentHealthPercent <= SageSettings.Instance.HolosHealthPercent);

            if (SageSettings.Instance.HolosTankOnly)
                targets = targets.Where(r => r.IsTank(SageSettings.Instance.HolosMainTankOnly));

            var target = targets.FirstOrDefault();

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

            var targets = Group.CastableAlliesWithin30.Where(r => r.CurrentHealthPercent < SageSettings.Instance.KrasisHealthPercent
                                                                  && !r.HasAura(Auras.Krasis));

            if (SageSettings.Instance.KrasisTankOnly)
                targets = targets.Where(r => r.IsTank(SageSettings.Instance.KrasisMainTankOnly));

            var target = targets.FirstOrDefault();

            if (target == null)
                return false;

            return await Spells.Krasis.CastAura(target, Auras.Krasis);
        }

    }
}
