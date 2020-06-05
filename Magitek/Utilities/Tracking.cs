using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Debugging;
using Magitek.Utilities.Routines;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using BaseSettings = Magitek.Models.Account.BaseSettings;
using Debug = Magitek.ViewModels.Debug;

namespace Magitek.Utilities
{
    internal static class Tracking
    {
        public static readonly List<EnemyInfo> EnemyInfos = new List<EnemyInfo>();
        private static List<BattleCharacter> _enemyCache = new List<BattleCharacter>();

        public static void Update()
        {
            UpdateCurrentPosition();

            if (Globals.InActiveDuty)
            {
                if (BotManager.Current.IsAutonomous)
                {
                    //In an instance, autonomous
                    //We should be a little less aggressive than if we're human-controlled, so just track attackers
                    _enemyCache = GameObjectManager.Attackers.ToList();
                }
                else
                {
                    //In an instance, not autonomous
                    //Track every tagged or aggroed enemy in the vicinity
                    _enemyCache = GameObjectManager.GetObjectsOfType<BattleCharacter>().Where(r =>    (   r.TaggerType > 0
                                                                                                       || r.HasTarget
                                                                                                       || r.IsBoss())
                                                                                                   && Core.Me.Distance(r) < 50)
                                                                                       .ToList();
                }
            }
            else
            {
                if (Globals.InParty)
                {
                    //In the open world, in a party
                    //Track everything our party tagged or every Fate Mob that is tagged by a player
                    _enemyCache = GameObjectManager.GetObjectsOfType<BattleCharacter>().Where(r => r.TaggerType == 2 || r.IsFate && r.TaggerType > 0 && !GameObjectManager.GetObjectsOfType<BattleCharacter>().Any(x => x.IsNpc && x.ObjectId == r.TaggerObjectId))
                                                                                       .ToList();
                }
                else
                {
                    //In the open world, not in a party
                    //Track the attacker list combined with every Fate Mob that is tagged by a player
                    var _fateEnemyCache = GameObjectManager.GetObjectsOfType<BattleCharacter>().Where(r => r.IsFate && r.TaggerType > 0 && !GameObjectManager.GetObjectsOfType<BattleCharacter>().Any(x => x.IsNpc && x.ObjectId == r.TaggerObjectId));
                    _enemyCache = GameObjectManager.Attackers.Union(_fateEnemyCache).ToList();
                }
            }

            Combat.Enemies.Clear();

            foreach (var unit in _enemyCache)
            {
                if (!unit.ValidAttackUnit())
                    continue;

                if (!unit.IsTargetable)
                    continue;

                if (BaseSettings.Instance.EnemySpellCastHistory)
                {
                    UpdateEnemyCastedSpells(unit);
                }

                if (BaseSettings.Instance.EnemyAuraHistory)
                {
                    UpdateEnemyAuraHistory(unit);
                }

                if (BaseSettings.Instance.EnemyTargetHistory)
                {
                    UpdateEnemyTargetHistory(unit);
                }

                Combat.Enemies.Add(unit);

                #region Dps Infos
                if (EnemyInfos.All(r => r.Unit != unit))
                {
                    var info = new EnemyInfo()
                    {
                        Unit = unit,
                        CombatStart = DateTime.Now,
                        StartHealth = unit.CurrentHealth,
                        Location = unit.Location,
                        IsMoving = false,
                        IsMovingChange = new Stopwatch()
                    };

                    info.IsMovingChange.Start();

                    EnemyInfos.Add(info);

                    if (BaseSettings.Instance.DebugEnemyInfo)
                    {
                        Logger.WriteInfo($@"[Debug] Adding {info.Unit} To Enemy Infos");
                    }
                }
                else
                {
                    var info = EnemyInfos.FirstOrDefault(r => r.Unit == unit);

                    if (info == null)
                        continue;

                    if (info.Unit == null)
                        continue;

                    info.CurrentDps = (info.StartHealth - unit.CurrentHealth) / (DateTime.Now - info.CombatStart).TotalSeconds;

                    var unitLocation = unit.Location;
                    var isNowMoving = info.Location != unitLocation;
                    if (isNowMoving != info.IsMoving) info.IsMovingChange.Restart();
                    info.IsMoving = isNowMoving;
                    info.Location = unitLocation;

                    if (info.Unit.CurrentHealthPercent > info.LastTickHealth + 2f)
                    {
                        info.CombatStart = DateTime.Now;
                        info.StartHealth = info.Unit.CurrentHealth;
                    }

                    info.CombatTimeLeft = new TimeSpan(0, 0, (int)(unit.CurrentHealth / info.CurrentDps)).TotalSeconds;
                    info.LastTickHealth = info.Unit.CurrentHealthPercent;
                }
                #endregion
            }

            if (Core.Me.HasTarget && Core.Me.CurrentTarget.ValidAttackUnit())
            {
                Combat.CurrentTargetCombatTimeLeft = Core.Me.CurrentTarget.CombatTimeLeft();
            }

            if (!EnemyInfos.Any())
            {
                Combat.CombatTotalTimeLeft = 0;
            }
            else
            {
                Combat.CombatTotalTimeLeft = (int)Math.Max(0, EnemyInfos.Select(r => r.CombatTimeLeft).Sum());
            }


            var removeDpsUnits = EnemyInfos.Where(r => !_enemyCache.Contains(r.Unit) || r.Unit == null || r.Unit.IsDead || !r.Unit.IsValid).ToArray();

            foreach (var unit in removeDpsUnits)
            {
                EnemyInfos.Remove(unit);

                if (BaseSettings.Instance.DebugEnemyInfo)
                {
                    Logger.WriteInfo($@"[Debug] Removing {unit.Unit} From Enemy Infos");
                }
            }

            Debug.Instance.Enemies = new ObservableCollection<EnemyInfo>(EnemyInfos);

            StunTracker.Update(Combat.Enemies);

            if (Core.Me.InCombat) mWasInCombat = true;
            if (!Core.Me.InCombat && mWasInCombat)
            {
                StateMachineManager.ResetRegisteredStateMachines();
                mWasInCombat = false;
            }                
        }
        
        private static bool mWasInCombat;

        private static void UpdateCurrentPosition()
        {
            var currentTarget = Core.Me.CurrentTarget;

            if (currentTarget == null)
            {
                if (ViewModels.BaseSettings.Instance.CurrentPosition != PositionalState.None)
                {
                    ViewModels.BaseSettings.Instance.CurrentPosition = PositionalState.None;
                }
                return;
            }

            if (currentTarget.IsBehind)
            {
                if (ViewModels.BaseSettings.Instance.CurrentPosition != PositionalState.Rear)
                {
                    ViewModels.BaseSettings.Instance.CurrentPosition = PositionalState.Rear;
                }
                return;
            }

            if (currentTarget.IsFlanking)
            {
                if (ViewModels.BaseSettings.Instance.CurrentPosition != PositionalState.Flank)
                {
                    ViewModels.BaseSettings.Instance.CurrentPosition = PositionalState.Flank;
                }
                return;
            }

            if (!currentTarget.IsFlanking && !currentTarget.IsBehind)
            {
                if (ViewModels.BaseSettings.Instance.CurrentPosition != PositionalState.Front)
                {
                    ViewModels.BaseSettings.Instance.CurrentPosition = PositionalState.Front;
                }
            }
        }

        private static void UpdateEnemyCastedSpells(Character unit)
        {
            if (!unit.IsCasting)
                return;

            if (Debug.Instance.EnemySpellCasts.ContainsKey(unit.CastingSpellId))
                return;

            var newSpellCast = new EnemySpellCastInfo(unit.SpellCastInfo.Name, unit.CastingSpellId, unit.Name);
            Logger.WriteInfo($@"[Debug] Adding [{newSpellCast.Id}] {newSpellCast.Name} To Enemy Spell Casts");
            Debug.Instance.EnemySpellCasts.Add(newSpellCast.Id, newSpellCast);
        }

        private static void UpdateEnemyAuraHistory(Character unit)
        {
            foreach (var aura in unit.CharacterAuras)
            {
                if (Debug.Instance.EnemyAuras.ContainsKey(aura.Id))
                    continue;

                var newAura = new TargetAuraInfo(aura.Name, aura.Id, unit.Name);
                Logger.WriteInfo($@"[Debug] Adding {aura.Name} To Enemy Aura History");
                Debug.Instance.EnemyAuras.Add(aura.Id, newAura);
            }
        }

        private static void UpdateEnemyTargetHistory(Character unit)
        {
            if (Debug.Instance.EnemyTargetHistory.ContainsKey(unit.NpcId))
                return;

            var newEnemyTargetInfo = new EnemyTargetInfo(unit.Name, unit.NpcId, WorldManager.ZoneId, unit.MaxHealth, unit.ClassLevel);
            Logger.WriteInfo($@"[Debug] Adding {newEnemyTargetInfo.Name} To Enemy Target History");
            Debug.Instance.EnemyTargetHistory.Add(newEnemyTargetInfo.Id, newEnemyTargetInfo);
        }
    }
}
