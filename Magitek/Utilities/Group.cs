using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Debugging;
using Magitek.ViewModels;
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

        private static readonly uint[] PetIds = {1398, 1399, 1400, 1401, 1402, 1403, 1404, 5478};

        public static void UpdateAllies(Action extensions = null)
        {
            DeadAllies.Clear();
            CastableTanks.Clear();
            CastableAlliesWithin30.Clear();
            CastableAlliesWithin20.Clear();
            CastableAlliesWithin15.Clear();
            CastableAlliesWithin10.Clear();

            if (!PartyManager.IsInParty)
            {
                if (RaptureAtkUnitManager.Controls.Any(r => r.Name == "GcArmyOrder"))
                {
                    Globals.InGcInstance = true;

                    foreach (var ally in GameObjectManager.GetObjectsOfType<BattleCharacter>().Where(r => !r.CanAttack))
                    {
                        if (!ally.IsTargetable || !ally.InLineOfSight() || ally.Icon == PlayerIcon.Viewing_Cutscene)
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

                        var distance = ally.Distance(Core.Me);

                        if (distance <= 30) { CastableAlliesWithin30.Add(ally); }
                        if (distance <= 30) { CastableAlliesWithin20.Add(ally); }
                        if (distance <= 15) { CastableAlliesWithin15.Add(ally); }
                        if (distance <= 10) { CastableAlliesWithin10.Add(ally); }

                        CastableAlliesWithin30.Add(Core.Me);
                        CastableAlliesWithin20.Add(Core.Me);
                        CastableAlliesWithin15.Add(Core.Me);
                        CastableAlliesWithin10.Add(Core.Me);
                    }
                }
                else
                {
                    Globals.InGcInstance = false;
                }
            }
            
            foreach (var ally in PartyManager.RawMembers.Select(r => r.BattleCharacter))
            {
                if (ally == null)
                    continue;

                if (!ally.IsTargetable || !ally.InLineOfSight() || ally.Icon == PlayerIcon.Viewing_Cutscene)
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

                if (WorldManager.InPvP)
                {
                    if (ally.HasAura(Auras.MountedPvp))
                        continue;
                }

                if (ally.IsTank())
                {
                    CastableTanks.Add(ally);
                }

                var distance = ally.Distance(Core.Me);

                if (distance <= 30) { CastableAlliesWithin30.Add(ally); }
                if (distance <= 30) { CastableAlliesWithin20.Add(ally); }
                if (distance <= 15) { CastableAlliesWithin15.Add(ally); }
                if (distance <= 10) { CastableAlliesWithin10.Add(ally); }
            }

            extensions?.Invoke();
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

        public static readonly List<Character> DeadAllies = new List<Character>();       
        public static readonly List<Character> CastableTanks = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin30 = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin20 = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin15 = new List<Character>();
        public static readonly List<Character> CastableAlliesWithin10 = new List<Character>();
    }
}
