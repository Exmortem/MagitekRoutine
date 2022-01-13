using Buddy.Coroutines;
using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.WhiteMage
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
            return await Roles.Healer.LucidDreaming(WhiteMageSettings.Instance.LucidDreaming, WhiteMageSettings.Instance.LucidDreamingManaPercent);
        }

        public static async Task<bool> PresenceOfMind()
        {
            if (!WhiteMageSettings.Instance.PresenceOfMind)
                return false;

            if (Core.Me.ClassLevel < Spells.PresenceofMind.LevelAcquired)
                return false;

            if (Casting.LastSpell == Spells.Benediction)
                return false;

            if (Core.Me.IsCasting)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Globals.InParty)
            {
                if (WhiteMageSettings.Instance.PresenceOfMindTankOnly)
                {
                    if (!Group.CastableTanks.Any(r => r.CurrentHealthPercent <=
                                                      WhiteMageSettings.Instance.PresenceOfMindHealthPercent))
                        return false;

                    return await Spells.PresenceofMind.Cast(Core.Me);
                }
                else
                {
                    if (Group.CastableAlliesWithin30.Count(
                            r => r.CurrentHealthPercent <= WhiteMageSettings.Instance.PresenceOfMindHealthPercent) <
                        WhiteMageSettings.Instance.PresenceOfMindNeedHealing)
                        return false;

                    return await Spells.PresenceofMind.Cast(Core.Me);
                }
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.PresenceOfMindHealthPercent)
                    return false;


                return await Spells.PresenceofMind.Cast(Core.Me);
            }
        }

        public static async Task<bool> AssizeForMana()
        {
            if (!WhiteMageSettings.Instance.Assize)
                return false;

            if (Core.Me.ClassLevel < Spells.Assize.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!WhiteMageSettings.Instance.AssizeForMana)
                return false;

            if (Core.Me.CurrentManaPercent > WhiteMageSettings.Instance.AssizeManaPercent)
                return false;

            if (Core.Me.HasAura(Auras.LucidDreaming))
                return false;

            return await Spells.Assize.Cast(Core.Me);
        }

        public static async Task<bool> ThinAir(bool preAir)
        {

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.ClassLevel < Spells.ThinAir.LevelAcquired)
                return false;

            if (!WhiteMageSettings.Instance.ThinAir)
                return false;

            if (Core.Me.ClassLevel < 62)
                return false;

            if (Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.ThinAirManaPercent)
                return await Spells.ThinAir.Cast(Core.Me);

            if (!preAir)
                return false;

            return await Spells.ThinAir.Cast(Core.Me);
        }

        public static async Task<bool> DivineBenison()
        {
            if (!WhiteMageSettings.Instance.DivineBenison)
                return false;

            if (Core.Me.ClassLevel < Spells.DivineBenison.LevelAcquired)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            var divineBenisonTarget = Group.CastableTanks.FirstOrDefault();

            if (divineBenisonTarget == null)
                return false;
            if (Casting.LastSpell == Spells.Aero || Casting.LastSpell == Spells.Aero2 || Casting.LastSpell == Spells.Dia)
                return await Spells.DivineBenison.Cast(divineBenisonTarget);
            return false;
        }

        public static async Task<bool> Temperance()
        {
            if (!WhiteMageSettings.Instance.Temperance)
                return false;

            if (Core.Me.ClassLevel < Spells.Temperance.LevelAcquired)
                return false;

            if (Casting.LastSpell == Spells.Medica)
                return false;

            if (Casting.LastSpell == Spells.Assize)
                return false;

            if (Casting.LastSpell == Spells.AfflatusRapture)
                return false;

            var temperance = Group.CastableAlliesWithin30.Count(r => r.CurrentHealth > 0 && r.Distance(Core.Me) <= 15 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.TemperanceHealthPercent);

            if (temperance < WhiteMageSettings.Instance.TemperanceAllies)
                return false;

            return await Spells.Temperance.Cast(Core.Me);
        }

        public static async Task<bool> Aquaveil()
        {
            if (!WhiteMageSettings.Instance.Aquaveil)
                return false;

            if (Spells.Aquaveil.IsKnownAndReady())
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Globals.InParty)
            {
                var canAquaveilTargets = Group.CastableAlliesWithin30.Where(CanAquaveil).ToList();

                var aquaveilTarget = canAquaveilTargets.FirstOrDefault();

                if (aquaveilTarget == null)
                    return false;

                return await Spells.Aquaveil.Cast(aquaveilTarget);
            }

            if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.AquaveilHealthPercent)
                return false;

            return await Spells.Aquaveil.Cast(Core.Me);

            bool CanAquaveil(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > WhiteMageSettings.Instance.AquaveilHealthPercent)
                    return false;

                if (WhiteMageSettings.Instance.AquaveilTankOnly && !unit.IsTank(WhiteMageSettings.Instance.AquaveilMainTankOnly))
                    return false;

                return unit.Distance(Core.Me) <= 30;
            }
        }
    }
}
