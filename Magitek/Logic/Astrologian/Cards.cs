﻿using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Astrologian;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Astrologian
{
    internal static class Cards
    {

        public enum NewAstroCards
        {
            None,
            Balance,
            Bole,
            Arrow,
            Spear,
            Ewer,
            Spire
        }

        private static NewAstroCards GetDrawnCard()
        {
            var drawnCard = (int) Arcana;

            if (drawnCard == 1 || drawnCard == 113 || drawnCard == 129) return NewAstroCards.Balance;
            if (drawnCard == 2 || drawnCard == 114 || drawnCard == 130) return NewAstroCards.Bole;
            if (drawnCard == 3 || drawnCard == 115 || drawnCard == 131) return NewAstroCards.Arrow;
            if (drawnCard == 4 || drawnCard == 116 || drawnCard == 132) return NewAstroCards.Spear;
            if (drawnCard == 5 || drawnCard == 117 || drawnCard == 133) return NewAstroCards.Ewer;
            if (drawnCard == 6 || drawnCard == 118 || drawnCard == 134) return NewAstroCards.Spire;

            return NewAstroCards.None;
        }
        public static async Task<bool> PlayCards()
        {
            var drawnCard = GetDrawnCard();
            
            var cardDrawn = drawnCard != NewAstroCards.None;

            /*
            Looks like Arcana is now filled with either the Crown Draw, Or the Arcana Draw with Arcana Draw taking priority.
            
            The Card ID's have changed... but there's some goof with Reborn where whether or not you have Lord, Lady, or nothing, the Card ID drawn changes:
                Balance = 1, 113, 129.
                Bole = 2, 114, 130.
                Arrow = 3, 115, 131.
                Spear = 4, 116, 132.
                Ewer = 5, 117, 133.
                Spire = 6, 118, 134.

            There's some temporary workarounds above until reborn has this fixed.

            */

            if (ActionManager.CanCast(Spells.Draw, Core.Me)
                && AstrologianSettings.Instance.UseDraw
                && !cardDrawn)
                if (await Spells.Draw.Cast(Core.Me))
                    await Coroutine.Wait(750, () => GetDrawnCard() != NewAstroCards.None);

            if (!cardDrawn)
                return false;

            if (Core.Me.InCombat && Spells.MinorArcana.IsKnownAndReady())
                if (!Core.Me.HasAnyAura(new uint[] { Auras.LadyOfCrownsDrawn, Auras.LordOfCrownsDrawn }))
                    return await Spells.MinorArcana.Cast(Core.Me);

            if (Combat.CombatTotalTimeLeft <= AstrologianSettings.Instance.DontPlayWhenCombatTimeIsLessThan)
                return false;

            if (await RedrawOrDrawAgain(drawnCard))
                return true;
            
            if (Globals.InParty && Core.Me.InCombat && AstrologianSettings.Instance.Play)
            {
                switch (drawnCard)
                {
                    //Solar Seal
                    case NewAstroCards.Balance:
                        if (DivinationSeals.All(seal => seal != AstrologianSeal.Solar_Seal))
                        {
                            return await MeleeDpsOrTank();
                        }
                        break;
                    case NewAstroCards.Bole:
                        if (DivinationSeals.All(seal => seal != AstrologianSeal.Solar_Seal))
                        {
                            return await RangedDpsOrHealer();
                        }
                        break;

                    //Lunar Seal
                    case NewAstroCards.Arrow:
                        if (DivinationSeals.All(seal => seal != AstrologianSeal.Lunar_Seal))
                        {
                            return await MeleeDpsOrTank();
                        }
                        break;
                    case NewAstroCards.Ewer:
                        if (DivinationSeals.All(seal => seal != AstrologianSeal.Lunar_Seal))
                        {
                            return await RangedDpsOrHealer();
                        }
                        break;

                    //Celestial Seal
                    case NewAstroCards.Spear:
                        if (DivinationSeals.All(seal => seal != AstrologianSeal.Celestial_Seal))
                        {
                            return await MeleeDpsOrTank();
                        }
                        break;
                    case NewAstroCards.Spire:
                        if (DivinationSeals.All(seal => seal != AstrologianSeal.Celestial_Seal))
                        {
                            return await RangedDpsOrHealer();
                        }
                        break;
                }
            }

            if (!AstrologianSettings.Instance.Play)
                return false;

            if (!Globals.InParty && Core.Me.InCombat)
                return await Spells.Play.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> RedrawOrDrawAgain(NewAstroCards drawnCard)
        {
            if (!AstrologianSettings.Instance.UseReDraw)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (DivinationSeals.All(seal => seal == 0))
                return false;

            if (Group.CastableAlliesWithin30.All(r => r.HasAnyCardAura()))
                return false;

            if (drawnCard == NewAstroCards.Balance && DivinationSeals.All(seal => seal != AstrologianSeal.Solar_Seal))
                return false;

            if (drawnCard == NewAstroCards.Bole && DivinationSeals.All(seal => seal != AstrologianSeal.Solar_Seal))
                return false;

            if (drawnCard == NewAstroCards.Arrow && DivinationSeals.All(seal => seal != AstrologianSeal.Lunar_Seal))
                return false;

            if (drawnCard == NewAstroCards.Ewer && DivinationSeals.All(seal => seal != AstrologianSeal.Lunar_Seal))
                return false;

            if (drawnCard == NewAstroCards.Spear && DivinationSeals.All(seal => seal != AstrologianSeal.Celestial_Seal))
                return false;

            if (drawnCard == NewAstroCards.Spire && DivinationSeals.All(seal => seal != AstrologianSeal.Celestial_Seal))
                return false;

            if (Spells.Redraw.IsKnownAndReady() && Spells.Redraw.Charges >= 1 && Core.Me.HasAura(Auras.ClarifyingDraw))
                return await Spells.Redraw.Cast(Core.Me);

            if (Spells.Draw.Charges >= 1)
                return await Spells.Draw.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> AstroDyne()
        {
            if (!AstrologianSettings.Instance.Play || !AstrologianSettings.Instance.AstroDyne)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (!Spells.Astrodyne.IsKnownAndReady())
                return false;

            if (Combat.CombatTotalTimeLeft <= AstrologianSettings.Instance.DontPlayWhenCombatTimeIsLessThan)
                return false;

            if (DivinationSeals.All(seal => seal == 0))
                return false;

            //1 = MP Regen

            if (DivinationSeals.Count(seal => seal != 0) == 1 && (Core.Me.CurrentManaPercent > AstrologianSettings.Instance.LucidDreamingManaPercent || Spells.LucidDreaming.IsReady() || Core.Me.HasAura(Auras.LucidDreaming)))
                return false;

            //2 = MP Regen + Haste
            var hasteThreshold = PartyManager.NumMembers == 4 ? 2 : 3;

            if (DivinationSeals.Count(seal => seal != 0) == 2 && (Spells.EssentialDignity.Charges > 0 || Group.CastableAlliesWithin30.Count(r => r.CurrentManaPercent <= 60) <= hasteThreshold || Spells.Lightspeed.IsReady()))
                return false;

            if (Core.Me.HasAura(Auras.Lightspeed))
                return false;

            //3 = MP Regen + Haste + Dmg (or shortcut from above logic)
            return await Spells.Astrodyne.Cast(Core.Me);
        }
        private static async Task<bool> MeleeDpsOrTank()
        {
            var ally = Group.CastableAlliesWithin30.Where(a => !a.HasAnyCardAura() && a.CurrentHealth > 0 && (a.IsTank() || a.IsMeleeDps())).OrderBy(GetWeight);
            
            if (ally == null)
                return false;

            return await Spells.Play.Cast(ally.FirstOrDefault());
        }

        private static async Task<bool> RangedDpsOrHealer()
        {
            var ally = Group.CastableAlliesWithin30.Where(a => !a.HasAnyCardAura() && a.CurrentHealth > 0 && (a.IsHealer() || a.IsRangedDpsCard())).OrderBy(GetWeight);

            if (ally == null)
                return false;

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
                
                case ClassJobType.Reaper:
                    return AstrologianSettings.Instance.RprCardWeight;
                
                case ClassJobType.Sage:
                    return AstrologianSettings.Instance.SgeCardWeight;

                case ClassJobType.BlueMage:
                    return AstrologianSettings.Instance.BluCardWeight;
            }

            return c.CurrentJob == ClassJobType.Adventurer ? 70 : 0;
        }
    }
}
