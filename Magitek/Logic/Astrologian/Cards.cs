using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Astrologian;

namespace Magitek.Logic.Astrologian
{
    internal static class Cards
    {

        public static async Task<bool> PlayCards()
        {
            if (!AstrologianSettings.Instance.UseDraw)
                return false;

            var cardDrawn = Arcana != AstrologianCard.None;
            
            if (!cardDrawn)
                if (await Spells.Draw.Cast(Core.Me))
                    await Coroutine.Wait(750, () => Core.Me.HasAnyCardAura());

            if (Combat.CombatTotalTimeLeft <= AstrologianSettings.Instance.DontPlayWhenCombatTimeIsLessThan)
                return false;

            if (DivinationSeals.Any(c => c == 0))
                if (Spells.Redraw.Charges > 1)
                    switch (Arcana)
                    {
                        //Solar Seal
                        case AstrologianCard.Balance:
                        case AstrologianCard.Bole:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Solar_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            break;

                        //Lunar Seal
                        case AstrologianCard.Arrow:
                        case AstrologianCard.Ewer:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Lunar_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            break;

                        //Celestial Seal
                        case AstrologianCard.Spear:
                        case AstrologianCard.Spire:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Celestial_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            break;
                    }

            if (DivinationSeals.All(c => c != 0))
                await Spells.Divination.Cast(Core.Me);

            if (DivinationSeals.Any(c => c == AstrologianSeal.Solar_Seal || c == AstrologianSeal.Lunar_Seal || c == AstrologianSeal.Celestial_Seal))
                switch (Arcana)
                {
                    //Solar Seal
                    case AstrologianCard.Balance:
                    case AstrologianCard.Bole:
                        if (DivinationSeals.Any(c => c == AstrologianSeal.Solar_Seal))
                            await Spells.MinorArcana.Cast(Core.Me);
                        break;

                    //Lunar Seal
                    case AstrologianCard.Arrow:
                    case AstrologianCard.Ewer:
                        if (DivinationSeals.Any(c => c == AstrologianSeal.Lunar_Seal))
                            await Spells.MinorArcana.Cast(Core.Me);
                        break;

                    //Celestial Seal
                    case AstrologianCard.Spear:
                    case AstrologianCard.Spire:
                        if (DivinationSeals.Any(c => c == AstrologianSeal.Celestial_Seal))
                            await Spells.MinorArcana.Cast(Core.Me);
                        break;
                }
            
            if (PartyManager.IsInParty)
                switch (Arcana)
                {
                    case AstrologianCard.Balance:
                    case AstrologianCard.Arrow:
                    case AstrologianCard.Spear:
                    case AstrologianCard.LordofCrowns:
                        return await MeleeDpsOrTank();

                    case AstrologianCard.Bole:
                    case AstrologianCard.Ewer:
                    case AstrologianCard.Spire:
                    case AstrologianCard.LadyofCrowns:
                        return await RangedDpsOrHealer();
                }

            return await Spells.Play.Cast(Core.Me);
        }

        private static async Task<bool> MeleeDpsOrTank()
        {
            var ally = Group.CastableAlliesWithin30.Where(a => (a.IsTank() || a.IsMeleeDps()) && !a.HasAnyCardAura() && a.IsAlive).OrderBy(GetWeight);
            
            return await Spells.Play.Cast(ally.FirstOrDefault());
        }

        private static async Task<bool> RangedDpsOrHealer()
        {
            var ally = Group.CastableAlliesWithin30.Where(a => (a.IsHealer() | a.IsRangedDpsCard()) && !a.HasAnyCardAura() && a.IsAlive).OrderBy(GetWeight);
            
            return await Spells.Play.Cast(ally.FirstOrDefault());
        }

        private static int GetWeight(Character c)
        {
            if (c.CurrentJob == ClassJobType.Monk)
                return AstrologianSettings.Instance.MnkCardWeight;

            if (c.CurrentJob == ClassJobType.BlackMage)
                return AstrologianSettings.Instance.BlmCardWeight;

            if (c.CurrentJob == ClassJobType.Dragoon)
                return AstrologianSettings.Instance.DrgCardWeight;

            if (c.CurrentJob == ClassJobType.Samurai)
                return AstrologianSettings.Instance.SamCardWeight;

            if (c.CurrentJob == ClassJobType.Machinist)
                return AstrologianSettings.Instance.MchCardWeight;

            if (c.CurrentJob == ClassJobType.Summoner)
                return AstrologianSettings.Instance.SmnCardWeight;

            if (c.CurrentJob == ClassJobType.Bard)
                return AstrologianSettings.Instance.BrdCardWeight;

            if (c.CurrentJob == ClassJobType.Ninja)
                return AstrologianSettings.Instance.NinCardWeight;

            if (c.CurrentJob == ClassJobType.RedMage)
                return AstrologianSettings.Instance.RdmCardWeight;

            if (c.CurrentJob == ClassJobType.Dancer)
                return AstrologianSettings.Instance.DncCardWeight;

            if (c.CurrentJob == ClassJobType.Paladin)
                return AstrologianSettings.Instance.PldCardWeight;

            if (c.CurrentJob == ClassJobType.Warrior)
                return AstrologianSettings.Instance.WarCardWeight;

            if (c.CurrentJob == ClassJobType.DarkKnight)
                return AstrologianSettings.Instance.DrkCardWeight;

            if (c.CurrentJob == ClassJobType.Gunbreaker)
                return AstrologianSettings.Instance.GnbCardWeight;

            if (c.CurrentJob == ClassJobType.WhiteMage)
                return AstrologianSettings.Instance.WhmCardWeight;

            if (c.CurrentJob == ClassJobType.Scholar)
                return AstrologianSettings.Instance.SchCardWeight;

            if (c.CurrentJob == ClassJobType.Astrologian)
                return AstrologianSettings.Instance.AstCardWeight;

            return c.CurrentJob == ClassJobType.Adventurer ? 70 : 0;
        }
    }
}
