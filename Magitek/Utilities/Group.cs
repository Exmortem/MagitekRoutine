using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Debugging;
using Magitek.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using BaseSettings = Magitek.Models.Account.BaseSettings;

namespace Magitek.Utilities
{
    internal static class Group
    {
        public static IEnumerable<Character> AllianceMembers
        {
            get
            {
                return GameObjectManager.GetObjectsOfType<BattleCharacter>().Where(r => r.Type == GameObjectType.Pc && r.IsTargetable && r.InLineOfSight());
            }
        }

        public static IEnumerable<Character> Pets
        {
            get
            {
                return GameObjectManager.GetObjectsByNPCIds<GameObject>(PetIds).Where(r => r.IsTargetable && r.InLineOfSight() && r.Distance(Core.Me) <= 30).Select(r => r as Character);
            }
        }

        private static readonly uint[] PetIds = { 1398, 1399, 1400, 1401, 1402, 1403, 1404, 5478 };

        public static void UpdateAllies(Action extensions = null)
        {
            DeadAllies.Clear();
            CastableTanks.Clear();
            CastableAlliesWithin30.Clear();
            CastableAlliesWithin20.Clear();
            CastableAlliesWithin15.Clear();
            CastableAlliesWithin12.Clear();
            CastableAlliesWithin10.Clear();
            HealableAlliance.Clear();

            if (!Globals.InParty)
            {
                if (Globals.InGcInstance)
                {
                    AddAllyToCastable(Core.Me);

                    foreach (var ally in GameObjectManager.GetObjectsOfType<BattleCharacter>().Where(r => !r.CanAttack))
                    {
                        //if (!ally.IsTargetable || !ally.InLineOfSight() || ally.Icon == PlayerIcon.Viewing_Cutscene)
                        //TODO: This is a temporary fix for wrong PlayerIcon Enum: 15 = Viewing_Cutscene
                        if (!ally.IsTargetable || !ally.InLineOfSight() || ally.Icon == (PlayerIcon)15)
                            continue;

                        if (BaseSettings.Instance.PartyMemberAuraHistory)
                        {
                            UpdatePartyMemberHistory(ally);
                        }

                        if (ally.CurrentHealth <= 0 || ally.IsDead)
                        {
                            DeadAllies.Add(ally);
                            continue;
                        }

                        if (ally.IsTank())
                        {
                            CastableTanks.Add(ally);
                        }

                        AddAllyToCastable(ally);
                    }
                }
            }

            foreach (var ally in PartyManager.AllMembers.Select(r => r.BattleCharacter))
            {
                if (ally == null)
                    continue;

                if (BaseSettings.Instance.DebugHealingLists == true)
                {
                    Logger.WriteInfo($@"[Debug] PartyManager {ally.Name} is a valid Party Member in PartyManager.");
                }

                //if (!ally.IsTargetable || !ally.InLineOfSight() || ally.Icon == PlayerIcon.Viewing_Cutscene)
                //TODO: This is a temporary fix for wrong PlayerIcon Enum: 15 = Viewing_Cutscene
                if (!ally.IsTargetable || !ally.InLineOfSight() || ally.Icon == (PlayerIcon)15)
                    continue;

                if (BaseSettings.Instance.PartyMemberAuraHistory)
                {
                    UpdatePartyMemberHistory(ally);
                }


                if (WorldManager.InPvP)
                {
                    if (ally.HasAura(Auras.MountedPvp))
                        continue;
                }

                AddAllyToCastable(ally);
            }

            extensions?.Invoke();
        }

        public static void UpdateAlliance(
            bool IgnoreAlliance,
            bool HealAllianceDps,
            bool HealAllianceHealers,
            bool HealAllianceTanks,
            bool ResAllianceDps,
            bool ResAllianceHealers,
            bool ResAllianceTanks
        )
        {
            HealableAlliance.Clear();

            // Should we be ignoring our alliance?
            if (!IgnoreAlliance && (Globals.InActiveDuty || WorldManager.InPvP))
            {
                // Create a list of alliance members that we need to check
                if (HealAllianceDps || HealAllianceHealers || HealAllianceTanks)
                {
                    var allianceToHeal = AllianceMembers.Where(a => !a.CanAttack && !a.HasAura(Auras.MountedPvp) && (
                                                                          HealAllianceDps && a.IsDps() ||
                                                                          HealAllianceTanks && a.IsTank() ||
                                                                          HealAllianceHealers && a.IsDps()));

                    foreach (var ally in allianceToHeal)
                    {
                        if (ally.Distance(Core.Me) <= 30)
                            HealableAlliance.Add(ally);
                    }
                }

                if (ResAllianceDps || ResAllianceHealers || ResAllianceTanks)
                {
                    var allianceToRes = AllianceMembers.Where(a => a.CurrentHealth <= 0 &&
                                                                   (ResAllianceDps && a.IsDps() ||
                                                                    ResAllianceTanks && a.IsTank() ||
                                                                    ResAllianceHealers && a.IsDps()));

                    foreach (var ally in allianceToRes)
                    {
                        DeadAllies.Add(ally);
                    }
                }
            }
        }

        public static void SwitchCastableToAlliance()
        {
            ClearCastable();

            foreach (var ally in HealableAlliance)
            {
                AddAllyToCastable(ally);
            }
        }


        private static void UpdatePartyMemberHistory(Character unit)
        {
            foreach (var aura in unit.CharacterAuras)
            {
                if (Debug.Instance.PartyMemberAuras.ContainsKey(aura.Id))
                    continue;

                var newAura = new TargetAuraInfo(aura.Name, aura.Id, unit.Name);
                Logger.WriteInfo($@"[Debug] Adding {aura.Name} To Party Member Aura History");
                Debug.Instance.PartyMemberAuras.Add(aura.Id, newAura);
            }
        }
        private static void AddAllyToCastable(Character ally)
        {
            if (ally.CurrentHealth <= 0 || ally.IsDead)
            {
                DeadAllies.Add(ally);
                return;
            }

            if (ally.IsTank())
                CastableTanks.Add(ally);

            var distance = ally.Distance(Core.Me);
            if (distance <= 30) { CastableAlliesWithin30.Add(ally); }
            if (distance <= 25) { CastableAlliesWithin25.Add(ally); }
            if (distance <= 20) { CastableAlliesWithin20.Add(ally); }
            if (distance <= 15) { CastableAlliesWithin15.Add(ally); }
            if (distance <= 12) { CastableAlliesWithin12.Add(ally); }
            if (distance <= 10) { CastableAlliesWithin10.Add(ally); }
        }

        private static void ClearCastable()
        {
            DeadAllies.Clear();
            CastableTanks.Clear();
            CastableAlliesWithin30.Clear();
            CastableAlliesWithin25.Clear();
            CastableAlliesWithin20.Clear();
            CastableAlliesWithin15.Clear();
            CastableAlliesWithin12.Clear();
            CastableAlliesWithin10.Clear();
            HealableAlliance.Clear();
        }

        public static readonly List<Character> DeadAllies = new List<Character>();
        public static readonly List<Character> CastableTanks = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin30 = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin25 = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin20 = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin15 = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin12 = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin10 = new List<Character>();
        public static readonly List<Character> HealableAlliance = new List<Character>();
    }
}
