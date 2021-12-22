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
using Auras = Magitek.Utilities.Auras;
using static ff14bot.Managers.ActionResourceManager.Sage;

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
            if (Core.Me.HasAura(Auras.Kardia, true)
                || Core.Me.HasAura(Auras.Kardion))
                return false;

            var ally = Group.CastableAlliesWithin30.Where(a => a.IsAlive && (a.IsTank()));

            if (!Globals.InParty)
                return await Spells.Kardia.Cast(Core.Me);

            return await Spells.Kardia.CastAura(ally.FirstOrDefault(), Auras.Kardia);
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

            if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.SoteriaHealthPercent) < 2)
                return false;

            GameObject target = SageSettings.Instance.SoteriaTankOnly
                ? Group.CastableTanks.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.SoteriaHealthPercent
                && r.IsTank())
                : Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.SoteriaHealthPercent);

            if (target == null)
                return false;

            return await Spells.Soteria.CastAura(target, Auras.Soteria);
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

            if (Addersgall == 3)
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

            return await Spells.Holos.CastAura(target, Auras.Holos);
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

            if (Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.KrasisHealthPercent) < 2)
                return false;

            GameObject target = SageSettings.Instance.KrasisTankOnly
                ? Group.CastableTanks.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.KrasisHealthPercent
                && r.IsTank())
                : Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.KrasisHealthPercent);

            if (target == null)
                return false;

            return await Spells.Krasis.CastAura(target, Auras.Krasis);
        }
        public static async Task<bool> Zoe()
        {
            if (!SageSettings.Instance.Zoe)
                return false;

            if (Spells.Zoe.Cooldown != TimeSpan.Zero)
                return false;

            if (!await Spells.Zoe.CastAura(Core.Me, Auras.Zoe))
                return false;

            return await Coroutine.Wait(1500, () => Core.Me.HasAura(Auras.Zoe) && ActionManager.CanCast(Spells.Diagnosis.Id, Core.Me));

        }
    }
}
