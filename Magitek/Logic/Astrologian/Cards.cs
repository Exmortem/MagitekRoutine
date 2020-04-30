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
using Auras = Magitek.Utilities.Auras;

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
                if (ActionManager.CanCast(Spells.Draw, Core.Me) && AstrologianSettings.Instance.UseDraw)
                    if (await Spells.Draw.Cast(Core.Me))
                        await Coroutine.Wait(750, () => Arcana != AstrologianCard.None);

            if (Combat.CombatTotalTimeLeft <= AstrologianSettings.Instance.DontPlayWhenCombatTimeIsLessThan)
                return false;

            if (AstrologianSettings.Instance.UseReDraw)
            {
                if (DivinationSeals.Any(c => c == 0))
                    if (Spells.Redraw.Charges > 1)
                    {
                        switch (Arcana)
                        {
                            //Solar Seal
                            case AstrologianCard.Balance:
                                if (DivinationSeals.Any(c => c == AstrologianSeal.Solar_Seal))
                                    return await Spells.Redraw.Cast(Core.Me);
                                if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                    return await MeleeDpsOrTank(false);
                                break;
                            case AstrologianCard.Bole:
                                if (DivinationSeals.Any(c => c == AstrologianSeal.Solar_Seal))
                                    return await Spells.Redraw.Cast(Core.Me);
                                if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                    return await RangedDpsOrHealer(false);
                                break;

                            //Lunar Seal
                            case AstrologianCard.Arrow:
                                if (DivinationSeals.Any(c => c == AstrologianSeal.Lunar_Seal))
                                    return await Spells.Redraw.Cast(Core.Me);
                                if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                    return await MeleeDpsOrTank(false);
                                break;
                            case AstrologianCard.Ewer:
                                if (DivinationSeals.Any(c => c == AstrologianSeal.Lunar_Seal))
                                    return await Spells.Redraw.Cast(Core.Me);
                                if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                    return await RangedDpsOrHealer(false);
                                break;

                            //Celestial Seal
                            case AstrologianCard.Spear:
                                if (DivinationSeals.Any(c => c == AstrologianSeal.Celestial_Seal))
                                    return await Spells.Redraw.Cast(Core.Me);
                                if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                    return await MeleeDpsOrTank(false);
                                break;
                            case AstrologianCard.Spire:
                                if (DivinationSeals.Any(c => c == AstrologianSeal.Celestial_Seal))
                                    return await Spells.Redraw.Cast(Core.Me);
                                if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                    return await RangedDpsOrHealer(false);
                                break;
                        }
                    }
                //Redundant case for if 0 redraw but seal matches
                if (Spells.Redraw.Charges == 0)
                {
                    switch (Arcana)
                    {
                        //Solar Seal
                        case AstrologianCard.Balance:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Solar_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                return await MeleeDpsOrTank(false);
                            break;
                        case AstrologianCard.Bole:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Solar_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                return await RangedDpsOrHealer(false);
                            break;

                        //Lunar Seal
                        case AstrologianCard.Arrow:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Lunar_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                return await MeleeDpsOrTank(false);
                            break;
                        case AstrologianCard.Ewer:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Lunar_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                return await RangedDpsOrHealer(false);
                            break;

                        //Celestial Seal
                        case AstrologianCard.Spear:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Celestial_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                return await MeleeDpsOrTank(false);
                            break;
                        case AstrologianCard.Spire:
                            if (DivinationSeals.Any(c => c == AstrologianSeal.Celestial_Seal))
                                return await Spells.Redraw.Cast(Core.Me);
                            if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                                return await RangedDpsOrHealer(false);
                            break;
                    }
                }

                //Minor Arcana
                switch (Arcana)
                {
                    case AstrologianCard.Balance:
                        if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                            return await MeleeDpsOrTank(true);
                        break;
                    case AstrologianCard.Arrow:
                        if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                            return await MeleeDpsOrTank(true);
                        break;
                    case AstrologianCard.Spear:
                        if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                            return await MeleeDpsOrTank(true);
                        break;
                    case AstrologianCard.Bole:
                        if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                            return await RangedDpsOrHealer(true);
                        break;
                    case AstrologianCard.Ewer:
                        if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                            return await RangedDpsOrHealer(true);
                        break;
                    case AstrologianCard.Spire:
                        if (Core.Me.InCombat && AstrologianSettings.Instance.Play)
                            return await RangedDpsOrHealer(true);
                        break;
                }
                return false;
            }

            if (Combat.CombatTotalTimeLeft <= AstrologianSettings.Instance.DontPlayWhenCombatTimeIsLessThan)
                return false;

            if (!AstrologianSettings.Instance.Play)
                return false;

            if (Globals.InParty)
                switch (Arcana)
                {
                    case AstrologianCard.Balance:
                        if (Core.Me.InCombat)
                            return await MeleeDpsOrTank(false);
                        break;
                    case AstrologianCard.Arrow:
                        if (Core.Me.InCombat)
                            return await MeleeDpsOrTank(false);
                        break;
                    case AstrologianCard.Spear:
                        if (Core.Me.InCombat)
                            return await MeleeDpsOrTank(false);
                        break;

                    case AstrologianCard.Bole:
                        if (Core.Me.InCombat)
                            return await RangedDpsOrHealer(false);
                        break;
                    case AstrologianCard.Ewer:
                        if (Core.Me.InCombat)
                            return await RangedDpsOrHealer(false);
                        break;
                    case AstrologianCard.Spire:
                        if (Core.Me.InCombat)
                            return await RangedDpsOrHealer(false);
                        break;
                }

            return await Spells.Play.Cast(Core.Me);
        }
        public static async Task<bool> Divination()
        {
            if (Combat.CombatTotalTimeLeft <= AstrologianSettings.Instance.DontPlayWhenCombatTimeIsLessThan)
                return false;

            if (!AstrologianSettings.Instance.Play || !AstrologianSettings.Instance.Divination)
                return false;

            if (DivinationSeals.Any(c => c == 0))
                return false;

            // Added check to see if more than 2 people are around

            var divinationTargets = Group.CastableAlliesWithin15.Count(r => r.IsAlive);

            if (divinationTargets >= 2)
                return await Spells.Divination.CastAura(Core.Me, Auras.Divination);

            return false;
        }
        private static async Task<bool> MeleeDpsOrTank(bool minor)
        {
            var ally = Group.CastableAlliesWithin30.Where(a => !a.HasAnyCardAura() && a.IsAlive && (a.IsTank() || a.IsMeleeDps())).OrderBy(GetWeight);

            if (minor)
                // Action changed to LordofCrowns from MinorArcana
                return await Spells.LordofCrowns.Cast(ally.FirstOrDefault());

            return await Spells.Play.Cast(ally.FirstOrDefault());
        }

        private static async Task<bool> RangedDpsOrHealer(bool minor)
        {
            var ally = Group.CastableAlliesWithin30.Where(a => !a.HasAnyCardAura() && a.IsAlive && (a.IsHealer() || a.IsRangedDpsCard())).OrderBy(GetWeight);

            if (minor)
                // Action changed to Lady of Crowns from MinorArcana
                return await Spells.LadyofCrowns.Cast(ally.FirstOrDefault());

            return await Spells.Play.Cast(ally.FirstOrDefault());
        }

        private static int GetWeight(Character c)
        {
            switch (c.CurrentJob)
            {
                case ClassJobType.Astrologian:
                    return AstrologianSettings.Instance.AstCardWeight;

                case ClassJobType.Monk:
                case ClassJobType.Pugilist:
                    return AstrologianSettings.Instance.MnkCardWeight;

                case ClassJobType.BlackMage:
                case ClassJobType.Thaumaturge:
                    return AstrologianSettings.Instance.BlmCardWeight;

                case ClassJobType.Dragoon:
                case ClassJobType.Lancer:
                    return AstrologianSettings.Instance.DrgCardWeight;

                case ClassJobType.Samurai:
                    return AstrologianSettings.Instance.SamCardWeight;

                case ClassJobType.Machinist:
                    return AstrologianSettings.Instance.MchCardWeight;

                case ClassJobType.Summoner:
                case ClassJobType.Arcanist:
                    return AstrologianSettings.Instance.SmnCardWeight;

                case ClassJobType.Bard:
                case ClassJobType.Archer:
                    return AstrologianSettings.Instance.BrdCardWeight;

                case ClassJobType.Ninja:
                case ClassJobType.Rogue:
                    return AstrologianSettings.Instance.NinCardWeight;

                case ClassJobType.RedMage:
                    return AstrologianSettings.Instance.RdmCardWeight;

                case ClassJobType.Dancer:
                    return AstrologianSettings.Instance.DncCardWeight;

                case ClassJobType.Paladin:
                case ClassJobType.Gladiator:
                    return AstrologianSettings.Instance.PldCardWeight;

                case ClassJobType.Warrior:
                case ClassJobType.Marauder:
                    return AstrologianSettings.Instance.WarCardWeight;

                case ClassJobType.DarkKnight:
                    return AstrologianSettings.Instance.DrkCardWeight;

                case ClassJobType.Gunbreaker:
                    return AstrologianSettings.Instance.GnbCardWeight;

                case ClassJobType.WhiteMage:
                case ClassJobType.Conjurer:
                    return AstrologianSettings.Instance.WhmCardWeight;

                case ClassJobType.Scholar:
                    return AstrologianSettings.Instance.SchCardWeight;
            }

            return c.CurrentJob == ClassJobType.Adventurer ? 70 : 0;
        }
    }
}
