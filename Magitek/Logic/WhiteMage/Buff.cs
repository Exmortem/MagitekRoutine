using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
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
            if (!WhiteMageSettings.Instance.LucidDreaming)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.CurrentManaPercent > WhiteMageSettings.Instance.LucidDreamingManaPercent)
                return false;

            return await Spells.LucidDreaming.Cast(Core.Me);
        }

        public static async Task<bool> PresenceOfMind()
        {
            if (!WhiteMageSettings.Instance.PresenceOfMind)
                return false;

            if (Casting.LastSpell == Spells.Benediction)
                return false;

            if (WhiteMageSettings.Instance.DontBuffIfYouHaveOneAlready)
            {
                if (Core.Me.HasAura(Auras.DivineSeal))
                    return false;
            }

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
                if (Casting.LastSpell != Spells.Dia || Casting.LastSpell != Spells.Regen || Casting.LastSpell != Spells.AfflatusMisery || Casting.LastSpell != Spells.AfflatusRapture || Casting.LastSpell != Spells.AfflatusSolace)
                    return false;
                return await Spells.PresenceofMind.Cast(Core.Me);
            }
        }

        public static async Task<bool> AssizeForMana()
        {
            if (!WhiteMageSettings.Instance.Assize)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!WhiteMageSettings.Instance.AssizeForMana)
                return false;

            if (Core.Me.CurrentManaPercent > WhiteMageSettings.Instance.AssizeManaPercent)
                return false;

            if (Core.Me.HasAura(Auras.ShroudOfSaints))
                return false;

            return await Spells.Assize.Cast(Core.Me);
        }

        public static async Task<bool> ThinAir(bool preAir)
        {
            if (!WhiteMageSettings.Instance.ThinAir)
                return false;

            if (Core.Me.ClassLevel < 62)
                return false;

            if(Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.ThinAirManaPercent)
                return await Spells.ThinAir.Cast(Core.Me);

            if (!preAir)
                return false;

            return await Spells.ThinAir.Cast(Core.Me);
        }

        public static async Task<bool> DivineBenison()
        {
            if (!WhiteMageSettings.Instance.DivineBenison)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            var divineBenisonTarget = Group.CastableTanks.FirstOrDefault();

            if (divineBenisonTarget == null)
                return false;
            if (Casting.LastSpell == Spells.Aero)
                return await Spells.DivineBenison.Cast(divineBenisonTarget);
            return false;
        }

        public static async Task<bool> Temperance()
        {
            if (!WhiteMageSettings.Instance.Temperance)
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
    }
}