using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using ICSharpCode.SharpZipLib.Zip;
using static ff14bot.Managers.ActionResourceManager.Astrologian;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using Newtonsoft.Json;
using TreeSharp;
using static Magitek.Utilities.Routines.Astrologian;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Astrologian
{
    internal static class Card
    {
        public static async Task<bool> Play()
        {
            if (!AstrologianSettings.Instance.Play) return false;

            if (Core.Me.HasAura(Auras.Lightspeed)) return false;

            if (OnGcd) return false;

            if (Core.Me.IsCasting) return false;

            //if (DrawnCard() != AstrologianCard.None) return await PlayDrawn();
            if (DrawnCard() != AstrologianCard.None) return await PlayDrawnFromJson();

            //if (HeldCard() != AstrologianCard.None) return await PlayHeld();
            if (HeldCard() != AstrologianCard.None) return await PlayHeldFromJson();

            return false;

        }

        public static async Task<bool> Draw()
        {
            if (!AstrologianSettings.Instance.Draw)
                return false;

            if (DrawnCard() != AstrologianCard.None)
                return false;

            if (Core.Me.HasAura(Auras.Lightspeed)) return false;

            if (Globals.OnPvpMap)
            {
                if (Spells.PvpDraw.Cooldown != TimeSpan.Zero) return false;

                if (!Core.Me.InCombat) return false;

                if (Globals.InParty &&
                    PartyManager.VisibleMembers.Count(r => r.GameObject.Distance() < 30 && r.GameObject.HasCardAura()) > 2)
                    return false;

                return await Spells.PvpDraw.Cast(Core.Me);
            }
            if (Spells.Draw.Cooldown != TimeSpan.Zero) return false;

            CanRedraw = (ActionManager.HasSpell(Spells.Redraw.Id) && Spells.Redraw.Cooldown == TimeSpan.Zero);
            CanMinorArcana = (ActionManager.HasSpell(Spells.MinorArcana.Id) &&
                              Spells.MinorArcana.Cooldown == TimeSpan.Zero && Arcana == AstrologianCard.None);

            if (!ShouldPrepCardsOutOfCombat()) return false;

            if (Core.Me.InCombat && Combat.CombatTotalTimeLeft <
                AstrologianSettings.Instance.DontDrawWhenCombatTimeIs) return false;

            if (!Core.Me.InCombat)
            {
                if (!AstrologianSettings.Instance.CardRuleDefaultToMinorArcana || !CanMinorArcana) return false;
            }


            if (Globals.InParty && CardBuff() == AstrologianCardBuff.Shared &&
                Group.CastableAlliesWithin30.Count(r => r.HasCardAura()) >= 3)
                return false;

            return await Spells.Draw.Cast(Core.Me);
        }

        private static async Task<bool> PlayDrawnSolo()
        {
            switch (DrawnCard())
            {
                case AstrologianCard.Arrow:
                case AstrologianCard.Balance:
                case AstrologianCard.Spear:
                {
                   if (Core.Me.InCombat) return await Spells.PlayDrawn.Cast(Core.Me);

                    break;
                }
                case AstrologianCard.Bole:
                {
                    if (CardBuff() == AstrologianCardBuff.Duration || CardBuff() == AstrologianCardBuff.Potency)
                    {
                        if (Core.Me.InCombat) return await Spells.PlayDrawn.Cast(Core.Me);

                        if (CanRedraw) return await Spells.Redraw.Cast(Core.Me);
                        
                        return false;
                    }

                    if (Core.Me.InCombat) return await Spells.PlayDrawn.Cast(Core.Me);

                    break;
                }
                case AstrologianCard.Ewer:
                {
                    if (CardBuff() == AstrologianCardBuff.Duration || CardBuff() == AstrologianCardBuff.Potency)
                    {
                        if (Core.Me.InCombat) return await Spells.PlayDrawn.Cast(Core.Me);

                        if (CanRedraw) return await Spells.Redraw.Cast(Core.Me);

                        return false;
                    }

                    if (Core.Me.InCombat) return await Spells.PlayDrawn.Cast(Core.Me);

                    break;
                }
                case AstrologianCard.Spire:
                {
                    if (CanRedraw) return await Spells.Redraw.Cast(Core.Me);

                    if (Core.Me.InCombat) return await Spells.PlayDrawn.Cast(Core.Me);

                    break;
                }
                
            }
            return false;
        }

        private static async Task<bool> PlayDrawnFromJson() //All PlayDrawnFromJson Logic flows through here.
        {
            
            var drawncard = DrawnCard();

            if (drawncard == AstrologianCard.None) return false;

            if (Core.Me.IsCasting) return false;

            if (OnGcd) return false;
            
            if (AstrologianSettings.Instance.CardRules == null)
            {
                Logger.WriteInfo(@"No Card Rules Found... Writing " +
                                 AstrologianDefaultCardRules.DefaultCardRules.Count + " Rules to Settings.");
                AstrologianSettings.Instance.CardRules = AstrologianDefaultCardRules.DefaultCardRules;
                Logger.WriteInfo(@"Default Card Rules Write complete.");
            }

            if (Globals.OnPvpMap) return await ProcessCardLogic(CardLogicType.Pvp, CardPlayType.Drawn, drawncard);

            CanRedraw = (ActionManager.HasSpell(Spells.Redraw.Id) && Spells.Redraw.Cooldown == TimeSpan.Zero);
            CanMinorArcana = (ActionManager.HasSpell(Spells.MinorArcana.Id) &&
                              Spells.MinorArcana.Cooldown == TimeSpan.Zero && Arcana == AstrologianCard.None);
            CanUndraw = (ActionManager.HasSpell(Spells.Undraw.Id) && Spells.Undraw.Cooldown == TimeSpan.Zero &&
                         DrawnCard() != AstrologianCard.None);

            if (!Globals.InParty) return await ProcessCardLogic(CardLogicType.Solo, CardPlayType.Drawn, drawncard);

            if (PartyManager.NumMembers <= 4) return await ProcessCardLogic(CardLogicType.Party, CardPlayType.Drawn, drawncard);

            if (PartyManager.NumMembers > 4) return await ProcessCardLogic(CardLogicType.LargeParty, CardPlayType.Drawn, drawncard);

            return false;
        }

        private static async Task<bool> PlayHeldFromJson() //All PlayHeldFromJson Logic flows through here
        {
            var heldcard = HeldCard();

            if (heldcard == AstrologianCard.None) return false;

            if (Globals.OnPvpMap) return await ProcessCardLogic(CardLogicType.Pvp, CardPlayType.Held, heldcard);

            CanRedraw = (ActionManager.HasSpell(Spells.Redraw.Id) && Spells.Redraw.Cooldown == TimeSpan.Zero);
            CanUndraw = (ActionManager.HasSpell(Spells.Undraw.Id) && Spells.Undraw.Cooldown == TimeSpan.Zero &&
                         DrawnCard() != AstrologianCard.None);
            CanMinorArcana = (ActionManager.HasSpell(Spells.MinorArcana.Id) &&
                              Spells.MinorArcana.Cooldown == TimeSpan.Zero && Arcana == AstrologianCard.None);
           
            if (!Globals.InParty) return await ProcessCardLogic(CardLogicType.Solo, CardPlayType.Held, heldcard);

            if (PartyManager.NumMembers <= 4) return await ProcessCardLogic(CardLogicType.Party, CardPlayType.Held, heldcard);

            if (PartyManager.NumMembers > 4) return await ProcessCardLogic(CardLogicType.LargeParty, CardPlayType.Held, heldcard);

            return false;
        }


        private static async Task<bool> ProcessCardLogic(CardLogicType logictype, CardPlayType playtype, AstrologianCard card)
        {
            if (AstrologianSettings.Instance.CardRules == null) return false;

            //if (TimeSinceTheLastCardAction < LastCardThreshold) return false;

            if (!LastCardAction.CanCastNewAction) return false;

            var ruleincombat = Core.Me.InCombat;
            var cardRulesToProcess = AstrologianSettings.Instance.CardRules.Where(r => r.Card == card && r.LogicType == logictype && r.PlayType == playtype).OrderBy(r => r.CardPriority);

            return await ProcessCardRule(cardRulesToProcess, ruleincombat, playtype, logictype);

        }

        private static async Task<bool> ProcessCardRule(IEnumerable<CardRule> cardRulesToProcess, bool ruleincombat, CardPlayType playtype, CardLogicType logictype)
        {
            if (cardRulesToProcess == null) return false;

            var rulesToProcess = cardRulesToProcess as IList<CardRule> ?? cardRulesToProcess.ToList();
            var heldcard = HeldCard();

            var processed = false;
            
            //Logger.WriteInfo($@"Processing up to {rulesToProcess.Count} {logictype} rules");

            foreach (var cardRule in rulesToProcess)
            {
                //Logger.WriteInfo($"Processing rule: {cardRule.CardPriority}"); //For testing that the card rule processing is going by priority
                await Coroutine.Yield();
                if (processed)
                {
                    Logger.WriteInfo($"Detected that we've already processed a rule for {cardRule.Card}");
                    return true;
                }
                
                if (playtype == CardPlayType.Drawn && cardRule.Card != DrawnCard()) return false;
                if (playtype == CardPlayType.Held && cardRule.Card != HeldCard()) return false;
                
                var action = cardRule.Action;
                // ReSharper disable once SuggestVarOrType_SimpleTypes
                Conditions conditions = cardRule.Conditions;
                var targetrule = cardRule.Target;
                CardTargets.Clear();

                GameObject target = Core.Me;
                // ReSharper disable once SuggestVarOrType_SimpleTypes
                TargetConditions targetconditions = cardRule.TargetConditions;

                if (conditions != null)
                {
                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"InCombat Check");
                    if (conditions.InCombat != null && conditions.InCombat != ruleincombat) continue;
                    
                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"HasHeldCard Check");
                    if (conditions.HasHeldCard?.Count > 0)
                    {
                        if (conditions.HasHeldCard.All(r => r != heldcard)) continue;
                    }

                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"DoesntHaveHeldCard Check");
                    if (conditions.DoesntHaveHeldCard?.Count > 0)
                    {
                        if (conditions.DoesntHaveHeldCard.Any(r => r == heldcard)) continue;
                    }

                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"JobsInParty Check");
                    if (conditions.JobsNotInParty?.Count > 0 && Globals.InParty)
                    {
                        if (PartyManager.AllMembers.Any(r => conditions.JobsNotInParty.Contains(r.Class))) continue;
                    }

                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"RolesNotInParty Check");
                    if (conditions.RolesNotInParty?.Count > 0 && Globals.InParty)
                    {
                        if (PartyManager.AllMembers.Any(r =>
                            (conditions.RolesNotInParty.Contains(CardRole.Tank) && r.GameObject.IsTank()) ||
                            (conditions.RolesNotInParty.Contains(CardRole.Healer) && r.GameObject.IsHealer()) ||
                            (conditions.RolesNotInParty.Contains(CardRole.Dps) && r.GameObject.IsDps()))) continue;
                    }
                }

                //if (cardRule.CardPriority == 33) Logger.WriteInfo($"Building Target List");
                if (targetrule == CardTarget.Me) CardTargets.Add(Core.Me);
                if (targetrule == CardTarget.PartyMember)
                {
                    CardTargets = PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Where(r =>
                        r.IsTargetable && r.InLineOfSight() && r.Icon != PlayerIcon.Viewing_Cutscene).ToList();
                    
                }

                //if (cardRule.CardPriority == 33) Logger.WriteInfo($@"Processing Card Action: {action} on: {target}");
                switch (action)
                {
                    case CardAction.Redraw:
                        if (!CanRedraw) continue;
                        if (!await Spells.Redraw.Cast(Core.Me)) return false;
                        LogRuleProcessed(cardRule, ruleincombat, heldcard);
                        processed = true;
                        return true;
                    case CardAction.MinorArcana:
                        if (!CanMinorArcana) continue;
                        if (!await Spells.MinorArcana.Cast(Core.Me)) return false;
                        LogRuleProcessed(cardRule, ruleincombat, heldcard);
                        processed = true;
                        return true;
                    case CardAction.Play:
                        {
                            //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets Starting Count: {CardTargets.Count()}");
                            CardTargets.RemoveAll(r => r.HasCardAura() || r.CurrentHealth < 1 || r.IsDead || !r.IsValid);

                            //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets After Death Clean: {CardTargets.Count()}");

                            if (targetconditions != null)
                            {

                                var statLessThanFlag = false;
                                
                                if (targetconditions.MpLessThan != null && targetconditions.MpLessThan > 0 && targetconditions.MpLessThan < 100)
                                {
                                    CardTargets.RemoveAll(r => r.CurrentManaPercent > targetconditions.MpLessThan);
                                    CardTargets = CardTargets.OrderBy(r => r.CurrentManaPercent).ToList();
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets After MP Clean: {CardTargets.Count()}");
                                    statLessThanFlag = true;
                                }

                                if (targetconditions.HpLessThan != null && targetconditions.HpLessThan > 0 && targetconditions.HpLessThan < 100)
                                {
                                    CardTargets.RemoveAll(r => r.CurrentHealthPercent > targetconditions.HpLessThan);
                                    CardTargets = CardTargets.OrderBy(r => r.CurrentHealthPercent).ToList();
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets After HP Clean: {CardTargets.Count()}");
                                    statLessThanFlag = true;
                                }

                                if (targetconditions.HasTarget != null)
                                {
                                    CardTargets.RemoveAll(r => r.HasTarget != targetconditions.HasTarget);
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets After Has Target Clean: {CardTargets.Count()}");
                                }

                                if (targetconditions.IsRole?.Count > 0)
                                {
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"Comparing IsRole Clean: {targetconditions.IsRole.Select(r => r.ToString())}");
                                    if (targetconditions.IsRole.All(r => r != CardRole.Dps)) CardTargets.RemoveAll(r => r.IsDps());
                                    if (targetconditions.IsRole.All(r => r != CardRole.Tank)) CardTargets.RemoveAll(r => r.IsTank());
                                    if (targetconditions.IsRole.All(r => r != CardRole.Healer))
                                        CardTargets.RemoveAll(r => r.IsHealer());
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets After IsRole Clean: {CardTargets.Count()}");
                                }

                                if (targetconditions.IsJob?.Count > 0)
                                {
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"Comparing IsJob Clean: {targetconditions.IsJob.Select(r => r.ToString())}");
                                    CardTargets.RemoveAll(r => !targetconditions.IsJob.Contains(r.CurrentJob));
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets After IsJob Clean: {CardTargets.Count()}");
                                }

                                if (targetconditions.JobOrder != null)
                                {
                                    CardTargets = CardTargets.OrderBy(x =>
                                    {
                                        var index = targetconditions.JobOrder.IndexOf(x.CurrentJob);

                                        if (index == -1)
                                            index = int.MaxValue;

                                        return index;
                                    }).ToList();

                                    statLessThanFlag = true;
                                }

                                if (targetconditions.WithAlliesNearbyMoreThan != null && targetconditions.WithAlliesNearbyMoreThan > 0)
                                {
                                    CardTargets.RemoveAll(r => r.PartyMembersNearby(15).Count() <= targetconditions.WithAlliesNearbyMoreThan);
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets After WithAlliesNearbyMoreThan Clean: {CardTargets.Count()}");
                                }

                                if (targetrule == CardTarget.PartyMember)
                                {
                                    if (targetconditions.PlayerName != null)
                                    {
                                        target = CardTargets.FirstOrDefault(r => r.Name == targetconditions.PlayerName);
                                    }
                                    else if ((targetconditions.Choice == CardChoiceType.Random || targetconditions.Choice == null) && !statLessThanFlag)
                                    {
                                        var random = new Random();
                                        target = CardTargets.ElementAtOrDefault(random.Next(0, CardTargets.Count));
                                    }
                                    else
                                    {
                                        target = CardTargets.FirstOrDefault();
                                    }
                                    
                                    if (target == null)
                                    {
                                        //if (cardRule.CardPriority == 33) Logger.WriteInfo($"No Viable Target based on Conditions");            
                                        continue;
                                    }
                                }
                            }

                            //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets Ending Count: {CardTargets.Count()}");

                            //if (cardRule.CardPriority == 33) Logger.WriteInfo($"CardTargets Attempting to use {cardRule.Card} on {target}");

                            switch (playtype)
                            {
                                case CardPlayType.Drawn:
                                    if (logictype == CardLogicType.Pvp)
                                    {
                                        //Logger.WriteInfo($@"Trying to Pvp Play Drawn {cardRule.Card} on {target.Name} (CanAttack: {target.CanAttack})");
                                        if (!await Spells.PvpPlayDrawn.Cast(target)) return false;
                                        LogRuleProcessed(cardRule, ruleincombat, heldcard);
                                        processed = true;
                                        return true;
                                    }
                                    //if (cardRule.CardPriority == 33) Logger.WriteInfo($@"Trying to Normal Play Drawn {cardRule.Card} on {target.Name} (CanAttack: {target.CanAttack})");
                                    if (!await Spells.PlayDrawn.Cast(target)) return false;
                                    LogRuleProcessed(cardRule, ruleincombat, heldcard);
                                    processed = true;
                                    return true;
                                case CardPlayType.Held:
                                    if (!await Spells.PlaySpread.Cast(target)) return false;
                                    LogRuleProcessed(cardRule, ruleincombat, heldcard);
                                    processed = true;
                                    return true;
                                default:
                                    processed = true;
                                    return false;
                            }
                        }

                    case CardAction.StopLogic:
                        processed = true;
                        return false;
                    case CardAction.Undraw:
                        if (!CanUndraw) continue;
                        if (!await Spells.Undraw.Cast(Core.Me)) return false;
                        LogRuleProcessed(cardRule, ruleincombat, heldcard);
                        processed = true;
                        return true;
                    default:
                        continue;
                }
            }

            if (playtype != CardPlayType.Drawn || logictype == CardLogicType.Pvp) return false;
            var drawncard = DrawnCard();
            if (AstrologianSettings.Instance.CardRuleDefaultToMinorArcana && CanMinorArcana)
            {
                if (!await Spells.MinorArcana.Cast(Core.Me)) return false;
                if (BaseSettings.Instance.DebugPlayerCasting) Logger.WriteInfo($"No appropriate Card Rule to process. Using Minor Arcana on {drawncard} for Lord Of Crowns.");
                processed = true;
                return true;
            }

            if (!AstrologianSettings.Instance.CardRuleDefaultToUndraw) return false;
            if (!CanUndraw) return false;
            if (!await Spells.Undraw.Cast(Core.Me)) return false;
            if (BaseSettings.Instance.DebugPlayerCasting) Logger.WriteInfo($"No appropriate Card Rule to process. Undrawing {drawncard}.");
            processed = true;
            return true;
        }

        private static void LogRuleDetails(CardRule rule, bool incombat, AstrologianCard heldcard)
        {
            Logger.WriteInfo(@"========================================Card Rule========================================");
            if (rule == null) Logger.WriteInfo("\t" + @"CardRule is null. Nothing to output.");
            else
            {
                Logger.WriteInfo("\t" + $@"LogicType: {rule.LogicType}" + "\t" + $@"PlayType: {rule.PlayType}" + "\t" + $@"Card: {rule.Card}" + "\t" + $@"Priority: {rule.CardPriority}");
                if (rule.Conditions != null) 
                {
                    Logger.WriteInfo("\t" + @"Conditions:");
                    if (rule.Conditions.InCombat != null) Logger.WriteInfo("\t\t" + $@"InCombat {rule.Conditions.InCombat}");
                    if (rule.Conditions.HasHeldCard != null) Logger.WriteInfo("\t\t" + $@"HasHeldCard: {string.Join(",", rule.Conditions.HasHeldCard.Select(r => r.ToString()))}");
                    if (rule.Conditions.DoesntHaveHeldCard != null) Logger.WriteInfo("\t\t" + $@"DoesntHaveHeldCard: {string.Join(",", rule.Conditions.DoesntHaveHeldCard.Select(r => r.ToString()))}");
                    if (rule.Conditions.JobsNotInParty != null) Logger.WriteInfo("\t\t" + $@"JobsNotInParty: {string.Join(",", rule.Conditions.JobsNotInParty.Select(r => r.ToString()))}");
                    if (rule.Conditions.RolesNotInParty != null) Logger.WriteInfo("\t\t" + $@"RolesNotInParty: {string.Join(",", rule.Conditions.RolesNotInParty.Select(r => r.ToString()))}");
                }
                Logger.WriteInfo("\t" + $@"Action: {rule.Action}" + "\t" + $@"Target: {rule.Target}");
                if (rule.TargetConditions != null)
                {
                    Logger.WriteInfo("\tTargetConditions:");
                    if (rule.TargetConditions.HasTarget != null) Logger.WriteInfo("\t\t" + $@"HasTarget {rule.TargetConditions.HasTarget}");
                    if (rule.TargetConditions.HpLessThan != null) Logger.WriteInfo("\t\t" + $@"HpLessThan {rule.TargetConditions.HpLessThan}");
                    if (rule.TargetConditions.MpLessThan != null) Logger.WriteInfo("\t\t" + $@"MpLessThan {rule.TargetConditions.MpLessThan}");
                    if (rule.TargetConditions.TpLessThan != null) Logger.WriteInfo("\t\t" + $@"TpLessThan {rule.TargetConditions.TpLessThan}");
                    if (rule.TargetConditions.IsRole != null) Logger.WriteInfo("\t\t" + $@"IsRole {string.Join(",", rule.TargetConditions.IsRole.Select(r => r.ToString()))}");
                    if (rule.TargetConditions.JobOrder != null) Logger.WriteInfo("\t\t" + $@"JobOrder {string.Join(",", rule.TargetConditions.JobOrder.Select(r => r.ToString()))}");
                    if (rule.TargetConditions.IsJob != null) Logger.WriteInfo("\t\t" + $@"IsJob {string.Join(",", rule.TargetConditions.IsJob.Select(r => r.ToString()))}");
                    if (rule.TargetConditions.Choice != null) Logger.WriteInfo("\t\t" + $@"Choice {rule.TargetConditions.Choice}");
                    if (rule.TargetConditions.PlayerName != null) Logger.WriteInfo("\t\t" + $@"PlayerName {rule.TargetConditions.PlayerName}");
                    if (rule.TargetConditions.WithAlliesNearbyMoreThan != null) Logger.WriteInfo("\t\t" + $@"WithAlliesNearbyMoreThan {rule.TargetConditions.WithAlliesNearbyMoreThan}");
                }
                Logger.WriteInfo("\tCurrent Information");
                Logger.WriteInfo("\t\t" + $@"InCombat: {incombat}" + "\t" + $@"Held Card: {heldcard}");
                Logger.WriteInfo("\t\t" + $@"CanRedraw: {CanRedraw}");
            }
            Logger.WriteInfo(@"========================================Card Rule========================================");
        }
        
        private static void LogRuleProcessed(CardRule rule, bool incombat, AstrologianCard heldcard)
        {
            if (rule == null) return;
            
            LastCardActionDateTime = DateTime.Now;
            LastCardAction.LastActionDateTime = DateTime.Now;
            
            var targetToPrint = "";
            if (rule.Target == CardTarget.Me) targetToPrint = "Me";
            if (rule.Target == CardTarget.PartyMember) targetToPrint = "a Party Member";
            if (rule.TargetConditions?.PlayerName != null)
                targetToPrint = targetToPrint + $" named {rule.TargetConditions?.PlayerName}";
            
            var relevantConditions = "";
            
            var relevantTargetConditions = "";
            
            switch (rule.Action)
            {
                case CardAction.Play:
                    if (rule.LogicType == CardLogicType.Pvp) LastCardAction.LastAction = Spells.PvpPlayDrawn;
                    else switch (rule.PlayType)
                    {
                        case CardPlayType.Held:
                            LastCardAction.LastAction = Spells.PlaySpread;
                            break;
                        case CardPlayType.Drawn:
                            LastCardAction.LastAction = Spells.PlayDrawn;
                            break;
                    }
                    Logger.WriteInfo("\t" + $@"We're Playing the {rule.PlayType} card {rule.Card} on {targetToPrint}");
                    break;
               case CardAction.MinorArcana:
                    LastCardAction.LastAction = Spells.MinorArcana;
                    Logger.WriteInfo("\t" + $@"We're using Minor Arcana on the card {rule.Card}.");
                    break;
                case CardAction.Redraw:
                    LastCardAction.LastAction = Spells.Redraw;
                    LastCardAction.CardBeforeRedrawn = rule.Card;
                    Logger.WriteInfo("\t" + $@"We're Redrawing the card {rule.Card}.");
                    break;
                case CardAction.Undraw:
                    LastCardAction.LastAction = Spells.Undraw;
                    Logger.WriteInfo("\t" + $@"We're Undrawing the card {rule.Card}.");
                    break;
            }
            if (!BaseSettings.Instance.DebugPlayerCasting) return;
            Logger.WriteInfo($"Drawn Card is {DrawnCard()} Held Card is {HeldCard()}");
            Logger.WriteInfo("[CardRules]\t" + $@"LogicType: {rule.LogicType}" + "\t" + $@"PlayType: {rule.PlayType}" + "\t" +
                             $@"Card: {rule.Card}" + "\t" + $@"Priority: {rule.CardPriority}");
            
            if (rule.Conditions != null)
            {
                if (rule.Conditions.InCombat != null) relevantConditions = relevantConditions + $"InCombat ({rule.Conditions.InCombat}) ";
                if (rule.Conditions.HasHeldCard.Count > 0) relevantConditions = relevantConditions + $@"HasHeldCard: ({string.Join(",",rule.Conditions.HasHeldCard.Select(r => r.ToString()))}) ";
                if (rule.Conditions.DoesntHaveHeldCard.Count > 0) relevantConditions = relevantConditions + $@"DoesntHaveHeldCard: ({string.Join(",",rule.Conditions.DoesntHaveHeldCard.Select(r => r.ToString()))}) ";
                if (rule.Conditions.JobsNotInParty.Count > 0) relevantConditions = relevantConditions + $@"JobsNotInParty: ({string.Join(",", rule.Conditions.JobsNotInParty.Select(r => r.ToString()))}) ";
                if (rule.Conditions.RolesNotInParty.Count > 0) relevantConditions = relevantConditions + $@"RolesNotInParty: ({string.Join(",", rule.Conditions.RolesNotInParty.Select(r => r.ToString()))}) ";

                if (relevantConditions != "") Logger.WriteInfo("[CardRules]\t" + $"Relevant Conditions: {relevantConditions}");
            }
            if (rule.TargetConditions == null) return;
            if (rule.TargetConditions.HasTarget != null) relevantTargetConditions = relevantTargetConditions + $@"HasTarget ({rule.TargetConditions.HasTarget}) "; 
            if (rule.TargetConditions.HpLessThan != null && rule.TargetConditions.HpLessThan > 1 && rule.TargetConditions.HpLessThan < 100) relevantTargetConditions = relevantTargetConditions + $@"HpLessThan ({rule.TargetConditions.HpLessThan}) ";
            if (rule.TargetConditions.MpLessThan != null && rule.TargetConditions.MpLessThan > 1 && rule.TargetConditions.MpLessThan < 100) relevantTargetConditions = relevantTargetConditions + $@"MpLessThan ({rule.TargetConditions.MpLessThan}) ";
            if (rule.TargetConditions.TpLessThan != null && rule.TargetConditions.TpLessThan > 1 && rule.TargetConditions.TpLessThan < 100) relevantTargetConditions = relevantTargetConditions + $@"TpLessThan ({rule.TargetConditions.TpLessThan}) ";
            if (rule.TargetConditions.IsRole.Count > 0) relevantTargetConditions = relevantTargetConditions + $@"IsRole ({string.Join(",", rule.TargetConditions.IsRole.Select(r => r.ToString()))}) ";
            if (rule.TargetConditions.JobOrder.Count > 0) relevantTargetConditions = relevantTargetConditions + $@"JobOrder ({string.Join(",",rule.TargetConditions.JobOrder.Select(r => r.ToString()))}) ";
            if (rule.TargetConditions.IsJob.Count > 0) relevantTargetConditions = relevantTargetConditions + $@"IsJob ({string.Join(",",rule.TargetConditions.IsJob.Select(r => r.ToString()))}) ";
            //if (rule.TargetConditions.Choice != null) relevantTargetConditions = relevantTargetConditions + $@"Choice ({rule.TargetConditions.Choice}) ";
            if (rule.TargetConditions.PlayerName != null) relevantTargetConditions = relevantTargetConditions + $@"PlayerName ({rule.TargetConditions.PlayerName}) "; 
            if (rule.TargetConditions.WithAlliesNearbyMoreThan != null && rule.TargetConditions.WithAlliesNearbyMoreThan > 0) relevantTargetConditions = relevantTargetConditions + $@"WithAlliesNearbyMoreThan ({rule.TargetConditions.WithAlliesNearbyMoreThan}) ";

            if (relevantTargetConditions != "")
                Logger.WriteInfo("[CardRules]\t" + $"Relevant Target Conditions: {relevantTargetConditions}");
        }

        private static List<BattleCharacter> CardTargets = new List<BattleCharacter>();

        private const double LastCardThreshold = 500;
        private static DateTime LastCardActionDateTime { get; set; }
        public static double TimeSinceTheLastCardAction => (DateTime.Now - LastCardActionDateTime).Milliseconds;

        private static class LastCardAction
        {
            private const double Threshold = 500;
            public static SpellData LastAction { private get; set; }

            public static DateTime LastActionDateTime { private get; set; }
            
            public static AstrologianCard CardBeforeRedrawn { private get; set; }

            private static double TimeSinceLastCardAction => (DateTime.Now - LastActionDateTime).Milliseconds;
            
            public static bool CanCastNewAction 
            {
                get
                {
                    if (LastAction == null) return true;
                    var lastActionOnCooldown = LastAction.Cooldown > TimeSpan.Zero;
                    var lastActionGoingOnCooldown = TimeSinceLastCardAction < LastAction.AdjustedCooldown.Milliseconds;
                    var lastActionDoneLongTimeAgo = TimeSinceLastCardAction > LastAction.AdjustedCooldown.Milliseconds;
                    var lastActionMoreThanThresholdAgo = true;
                    if (LastAction == Spells.Redraw)
                    {
                        if (DrawnCard() != CardBeforeRedrawn)
                            lastActionMoreThanThresholdAgo =
                                (DateTime.Now - LastActionDateTime).Milliseconds > Threshold;
                        else {
                            lastActionMoreThanThresholdAgo =
                                (DateTime.Now - LastActionDateTime).Seconds > 2;
                        }
                    }
                    return (lastActionOnCooldown || lastActionDoneLongTimeAgo) && !lastActionGoingOnCooldown && lastActionMoreThanThresholdAgo;
                }
            }
        }
    }
    
}