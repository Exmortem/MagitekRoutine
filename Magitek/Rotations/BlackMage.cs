using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Buddy.Coroutines;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.BlackMage;
using Magitek.Models.BlackMage;
using Magitek.Utilities;
using Magitek.Utilities.Routines;
using CombatUtil = Magitek.Utilities.Combat;
using Auras = Magitek.Utilities.Auras;
using Magitek.Models.Account;

namespace Magitek.Rotations
{
    public enum BlmStateIds
    {
        Start,
        SingleTarget,
        AoE,
        FreezeSucceeded,
        AoEThunderSucceeded,
        TriplecastSucceeded,
        AoEFire3Succeeded,
        Fire3SucceededAvoidSwiftcast,
        InitialFlareSucceeded,
        InitialFlareSucceededAvoidSwiftcast,
        FinalFlareSucceeded,
        ManaFontSucceeded,
        TransposeSucceeded,
        FireUntilLowMana,
        FreezeSpam,
        FreezeSpamNoThunder,
        Blizzard2Spam,
        Blizzard2SpamNoThunder,
        Fire2Spam,
        Fire2SpamNoThunder,
        Blizzard3Succeeded,
        STThunderSucceeded,
        Blizzard4Succeeded,
        STFire3Succeeded,
        Fire4Spam,
        FirstFire4Succeeded,
        SecondFire4Succeeded,
        ThirdFire4Succeeded,
        DespairSucceeded,
        BeginRecovery,
        Recovery,
        RecoveryNoThunder,
        LowLevelSTRotation,
        LowLevelSTRotationNoThunder,
        Blizzard,
        MidLevelSTRotation,
        MidLevelSTRotationNoThunder,
        FireSpam,
        MidLevelRecovery
    }
    
    public static class BlackMage
    {
        #region Forced Level Sync
        //Pretend we're level synced to test out rotations without having to go into a dungeon
        private const int ForcedSyncLevel = 80;
        private static int ForcedSyncClassLevel => Math.Min(ForcedSyncLevel, (int)Core.Me.ClassLevel);
        
        private static async Task<bool> ForcedSyncCast(SpellData spell, GameObject target)
        {
            if (spell.LevelAcquired > ForcedSyncClassLevel)
                return false;

            //Logger.WriteInfo($"Trying to cast {spell.Name}");
            return await spell.Cast(target);
        }
        #endregion

        private static StateMachine<BlmStateIds> mStateMachine;

        //TODO What to do if when synced target spell can be cast but swiftcast cannot
        private static async Task<bool> Swiftcast(SpellData spell, GameObject target)
        {
            if (spell.LevelAcquired > ForcedSyncClassLevel)
                return false;
            if (await ForcedSyncCast(Spells.Swiftcast, Core.Me))
            {
                await Coroutine.Wait(2000, () => Core.Me.HasAura(Auras.Swiftcast));
                await Coroutine.Wait(2000, () => ActionManager.CanCast(spell, target));
                return await spell.Cast(target);
            }
            return false;
        }
        
        private static bool TimeSince(SpellData spell, int minimumMs)
        {
            SpellCastHistoryItem mf = Casting.SpellCastHistory.FirstOrDefault(s => s.Spell == spell);

            if (mf == null)
                return true;

            if (DateTime.UtcNow.Subtract(mf.TimeCastUtc).TotalMilliseconds >= minimumMs)
                return true;

            return false;
        }

        private static bool ManaFontDelayElapsed => TimeSince(Spells.ManaFont, 2500);

        private static bool CanCastAnother(SpellData spell, int remainingManaNeeded, double remainingTimeNeededMs)
        {
            return Core.Me.CurrentMana >= spell.AdjustedSpellCostBlm() + remainingManaNeeded
                   && ActionResourceManager.BlackMage.StackTimer.TotalMilliseconds >= spell.AdjustedCastTime.TotalMilliseconds + remainingTimeNeededMs;
        }

        private static bool CanCastFoul()
        {
            return ActionManager.HasSpell(Spells.Foul.Id) && ActionResourceManager.BlackMage.PolyglotCount > 0;
        }

        private static bool SpamSpell(SpellData spell)
        {
            Aura thunder2 = (Core.Me.CurrentTarget as Character)?.Auras.FirstOrDefault(x => x.Id == Auras.Thunder2 && x.CasterId == Core.Player.ObjectId);
            Aura thunder4 = (Core.Me.CurrentTarget as Character)?.Auras.FirstOrDefault(x => x.Id == Auras.Thunder4 && x.CasterId == Core.Player.ObjectId);

            if (thunder2 != null && thunder2.TimespanLeft.TotalMilliseconds - spell.AdjustedCastTime.TotalMilliseconds > Spells.Thunder2.AdjustedCastTime.TotalMilliseconds)
                return true;

            if (thunder4 != null && thunder4.TimespanLeft.TotalMilliseconds - spell.AdjustedCastTime.TotalMilliseconds > Spells.Thunder4.AdjustedCastTime.TotalMilliseconds)
                return true;

            return false;
        }

        private static bool HasMyAura(GameObject target, uint aura)
        {
            return (target as Character)?.Auras.Any(x => x.Id == aura && x.CasterId == Core.Player.ObjectId) ?? false;
        }

        private static async Task<bool> CastThunder(GameObject target)
        {
            if (AoEMode)
            {
                if (await ForcedSyncCast(Spells.Thunder4, target))
                    return true;

                return await ForcedSyncCast(Spells.Thunder2, target);
            }
            else
            {
                if (await ForcedSyncCast(Spells.Thunder3, target))
                    return true;

                return await ForcedSyncCast(Spells.Thunder, target);
            }           
        }

        private static bool OnGCD()
        {
            return Spells.Fire4.Cooldown > TimeSpan.Zero;
        }

        private static async Task<bool> NoOp()
        {
            return true;
        }

        static BlackMage()
        {
            mStateMachine = new StateMachine<BlmStateIds>(
                BlmStateIds.Start,
                new Dictionary<BlmStateIds, State<BlmStateIds>>()
                {
                    {
                        BlmStateIds.Start,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode, () => NoOp(), BlmStateIds.AoE,          true),
                                new StateTransition<BlmStateIds>(() => true,    () => NoOp(), BlmStateIds.SingleTarget, true),
                            })
                    },


                    
                    //TODO Add new states to jump into based on stacks
                    {
                        BlmStateIds.SingleTarget,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                                                                                                                                                                                     () => NoOp(),                                                  BlmStateIds.Start,               true),                                
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 40,                                                                                                                                                                   () => NoOp(),                                                  BlmStateIds.LowLevelSTRotation,  true),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel >= 40 && ForcedSyncClassLevel < 60,                                                                                                                                     () => ForcedSyncCast(Spells.Blizzard3, Core.Me.CurrentTarget), BlmStateIds.MidLevelSTRotation),
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.UmbralStacks == 3 && CanCastAnother(Spells.Blizzard4, 1600, 150),                                                                                            () => ForcedSyncCast(Spells.Blizzard4, Core.Me.CurrentTarget), BlmStateIds.Blizzard4Succeeded),
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.AstralStacks == 3 && CanCastAnother(Spells.Fire4, (int)Spells.Blizzard3.AdjustedSpellCostBlm(), Spells.Blizzard3.AdjustedCastTime.TotalMilliseconds + 150),  () => ForcedSyncCast(Spells.Fire4, Core.Me.CurrentTarget),     BlmStateIds.Fire4Spam),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                                                                                                        () => ForcedSyncCast(Spells.Blizzard3, Core.Me.CurrentTarget), BlmStateIds.Blizzard3Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Blizzard3.AdjustedSpellCostBlm() && ActionResourceManager.BlackMage.AstralStacks > 0,                                                                           () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.SingleTarget)
                            })
                    },
                    {
                        BlmStateIds.LowLevelSTRotation,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                                                                                                      () => NoOp(),                                                  BlmStateIds.Start,                        true),                               
                                new StateTransition<BlmStateIds>(() => !HasMyAura(Core.Me.CurrentTarget, Auras.Thunder) && CanCastAnother(Spells.Blizzard, 0, 0),                    () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.LowLevelSTRotationNoThunder),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                                          () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.LowLevelSTRotation),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire.AdjustedSpellCostBlm() && ActionResourceManager.BlackMage.AstralStacks > 0, () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Blizzard),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire.AdjustedSpellCostBlm(),                                                     () => ForcedSyncCast(Spells.Blizzard, Core.Me.CurrentTarget),  BlmStateIds.Blizzard),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                         () => ForcedSyncCast(Spells.Fire, Core.Me.CurrentTarget),      BlmStateIds.LowLevelSTRotation)
                            })
                    },
                    {
                        BlmStateIds.LowLevelSTRotationNoThunder,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                                                                                                      () => NoOp(),                                                  BlmStateIds.Start,                        true),                                
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                                          () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.LowLevelSTRotationNoThunder),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire.AdjustedSpellCostBlm() && ActionResourceManager.BlackMage.AstralStacks > 0, () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Blizzard),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire.AdjustedSpellCostBlm(),                                                     () => ForcedSyncCast(Spells.Blizzard, Core.Me.CurrentTarget),  BlmStateIds.Blizzard),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                         () => ForcedSyncCast(Spells.Fire, Core.Me.CurrentTarget),      BlmStateIds.LowLevelSTRotationNoThunder)
                            })
                    },
                    {
                        BlmStateIds.Blizzard,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                             () => NoOp(),                                                  BlmStateIds.Start,               true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Blizzard),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana >= 9600,         () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.LowLevelSTRotation),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 9600,          () => ForcedSyncCast(Spells.Blizzard, Core.Me.CurrentTarget),  BlmStateIds.Blizzard),
                            })
                    },
                    {
                        BlmStateIds.MidLevelSTRotation,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                                                                                                      () => NoOp(),                                                  BlmStateIds.Start,                        true),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 45 && !HasMyAura(Core.Me.CurrentTarget, Auras.Thunder),                                () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.MidLevelSTRotationNoThunder),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel >= 45 && !HasMyAura(Core.Me.CurrentTarget, Auras.Thunder3),                              () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.MidLevelSTRotationNoThunder),                                
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                                          () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.MidLevelSTRotation),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire.AdjustedSpellCostBlm() && ActionResourceManager.BlackMage.AstralStacks > 0, () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.MidLevelRecovery),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire3.AdjustedSpellCostBlm(),                                                    () => ForcedSyncCast(Spells.Blizzard3, Core.Me.CurrentTarget), BlmStateIds.MidLevelRecovery),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                         () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget),     BlmStateIds.FireSpam)
                            })
                    },
                    {
                        BlmStateIds.MidLevelSTRotationNoThunder,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                             () => NoOp(),                                               BlmStateIds.Start,                       true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                   BlmStateIds.MidLevelSTRotationNoThunder),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget),  BlmStateIds.FireSpam)
                            })
                    },
                    {
                        BlmStateIds.FireSpam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                                                       () => NoOp(),                                                  BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                           () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.MidLevelSTRotationNoThunder),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.FireStarter),                            () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget),     BlmStateIds.FireSpam),
                                new StateTransition<BlmStateIds>(() => CanCastAnother(Spells.Fire, 800, 150),                         () => ForcedSyncCast(Spells.Fire, Core.Me.CurrentTarget),      BlmStateIds.FireSpam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Blizzard3.AdjustedSpellCostBlm() && ActionResourceManager.BlackMage.AstralStacks > 0, () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.MidLevelRecovery),
                                new StateTransition<BlmStateIds>(() => true,                                                          () => ForcedSyncCast(Spells.Blizzard3, Core.Me.CurrentTarget), BlmStateIds.MidLevelSTRotation)
                            })
                    },
                    //TODO Usually spams too many blizzards
                    {
                        BlmStateIds.MidLevelRecovery,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                             () => NoOp(),                                                  BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.MidLevelSTRotationNoThunder),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Blizzard3, Core.Me.CurrentTarget), BlmStateIds.MidLevelSTRotation)
                            })
                    },
                    {
                        BlmStateIds.Blizzard3Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                             () => NoOp(),                                             BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),              () => ForcedSyncCast(Spells.Enochian, Core.Me),           BlmStateIds.Blizzard3Succeeded),
                                //new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),        BlmStateIds.Blizzard3Succeeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Sharpcast, Core.Me),          BlmStateIds.Blizzard3Succeeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Xenoglossy, Core.Me.CurrentTarget), BlmStateIds.Blizzard3Succeeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => CastThunder(Core.Me.CurrentTarget),                 BlmStateIds.STThunderSucceeded)
                            })
                    },
                    //TODO Need to recover from this state if Blizzard 4 not possible
                    {
                        BlmStateIds.STThunderSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                             () => NoOp(),                                                  BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),              () => ForcedSyncCast(Spells.Enochian, Core.Me),                BlmStateIds.STThunderSucceeded),
                                //new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                    BlmStateIds.STThunderSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Blizzard4, Core.Me.CurrentTarget), BlmStateIds.Blizzard4Succeeded)
                            })
                    },
                    {
                        BlmStateIds.Blizzard4Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                             () => NoOp(),                                              BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),               () => ForcedSyncCast(Spells.Enochian, Core.Me),            BlmStateIds.Blizzard4Succeeded),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),               () => NoOp(),   BlmStateIds.BeginRecovery, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && ActionResourceManager.BlackMage.StackTimer >= Spells.Thunder3.AdjustedCooldown + Spells.Fire3.AdjustedCastTime + TimeSpan.FromMilliseconds(BaseSettings.Instance.UserLatencyOffset), () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.Blizzard4Succeeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget), BlmStateIds.Fire4Spam)
                            })
                    },
                    {
                        BlmStateIds.Fire4Spam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                                                                                                                                        () => NoOp(),                                                  BlmStateIds.Start,             true),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),                                                                                                                         () => ForcedSyncCast(Spells.Enochian, Core.Me),                BlmStateIds.Fire4Spam),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),                                                                                                                         () => NoOp(),                                                  BlmStateIds.BeginRecovery,     true),
                                new StateTransition<BlmStateIds>(() => CanCastAnother(Spells.Fire4, (int)Spells.Fire.AdjustedSpellCostBlm(), Spells.Fire.AdjustedCastTime.TotalMilliseconds + 1500),                   () => ForcedSyncCast(Spells.Fire4, Core.Me.CurrentTarget),     BlmStateIds.Fire4Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.FireStarter),                                                                                                             () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget),     BlmStateIds.Fire4Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana >= (int)Spells.Fire.AdjustedSpellCostBlm() + (int)Spells.Fire4.AdjustedSpellCostBlm(),                                      () => ForcedSyncCast(Spells.Fire, Core.Me.CurrentTarget),      BlmStateIds.Fire4Spam),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel >= 72 && CanCastAnother(Spells.Fire4, 800, Spells.Despair.AdjustedCastTime.TotalMilliseconds + 1500),                      () => ForcedSyncCast(Spells.Fire4, Core.Me.CurrentTarget),     BlmStateIds.Fire4Spam),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 72 && CanCastAnother(Spells.Fire4, 800, Spells.Blizzard3.AdjustedCastTime.TotalMilliseconds + 150),                      () => ForcedSyncCast(Spells.Fire4, Core.Me.CurrentTarget),     BlmStateIds.Fire4Spam),
                                new StateTransition<BlmStateIds>(() => CanCastAnother(Spells.Despair, 0, BaseSettings.Instance.UserLatencyOffset),                                                                     () => ForcedSyncCast(Spells.Despair, Core.Me.CurrentTarget),   BlmStateIds.DespairSucceeded),
                                new StateTransition<BlmStateIds>(() => CanCastAnother(Spells.Fire, 800, BaseSettings.Instance.UserLatencyOffset),                                                                      () => ForcedSyncCast(Spells.Fire, Core.Me.CurrentTarget),      BlmStateIds.Fire4Spam),
                                new StateTransition<BlmStateIds>(() => Spells.Fire.AdjustedCastTime > ActionResourceManager.BlackMage.StackTimer + TimeSpan.FromMilliseconds(BaseSettings.Instance.UserLatencyOffset), () => ForcedSyncCast(Spells.Transpose, Core.Me),               BlmStateIds.BeginRecovery),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                                                           () => ForcedSyncCast(Spells.Blizzard3, Core.Me.CurrentTarget), BlmStateIds.Blizzard3Succeeded)
                            })
                    },
                    {
                        BlmStateIds.DespairSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                             () => NoOp(),                                                  BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.DespairSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Blizzard3, Core.Me.CurrentTarget), BlmStateIds.Blizzard3Succeeded)
                            })
                    },
                    {
                        //TODO: Need to do something while enochian is on cooldown (lower level rotation)?
                        BlmStateIds.BeginRecovery,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,       () => NoOp(),               BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => true,          () => ForcedSyncCast(Spells.LucidDreaming, Core.Me),        BlmStateIds.BeginRecovery),
                                new StateTransition<BlmStateIds>(() => true,          () => Swiftcast(Spells.Freeze, Core.Me.CurrentTarget),      BlmStateIds.Recovery),
                                new StateTransition<BlmStateIds>(() => true,          () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.Recovery)
                            })
                    },
                    {
                        BlmStateIds.Recovery,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => AoEMode,                                           () => NoOp(),                                              BlmStateIds.Start,     true),                                
                                new StateTransition<BlmStateIds>(() => !HasMyAura(Core.Me.CurrentTarget, Auras.Thunder3), () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.Recovery),                             
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),               () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.Recovery),
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),                            () => ForcedSyncCast(Spells.Enochian, Core.Me),            BlmStateIds.Recovery),
                                new StateTransition<BlmStateIds>(() => true,                                              () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget), BlmStateIds.Fire4Spam)
                            })
                    },
                    

                    //TODO Add new states to jump based on stacks
                    {
                        BlmStateIds.AoE,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                 () => NoOp(),                                                  BlmStateIds.Start,                true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                      () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.AoE),
                                //new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 26 && Core.Me.CurrentMana >= 3400, () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Fire2SpamNoThunder),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 35 && CanCastAnother(Spells.Fire2, 1200, 100), () => ForcedSyncCast(Spells.Thunder2, Core.Me.CurrentTarget),   BlmStateIds.Fire2SpamNoThunder),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 35 && ActionResourceManager.BlackMage.AstralStacks > 0, () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 35,                                () => ForcedSyncCast(Spells.Blizzard2, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 50 && Core.Me.CurrentMana >= 1400, () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.FreezeSpamNoThunder),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel < 50,                                () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget),    BlmStateIds.FreezeSpam),
                                //new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.UmbralHearts > 0,         () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget),    BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                     () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget),    BlmStateIds.FreezeSucceeded),                                
                                new StateTransition<BlmStateIds>(() => ActionResourceManager.BlackMage.AstralStacks > 0,         () => ForcedSyncCast(Spells.Transpose, Core.Me),               BlmStateIds.AoE),                                 
                            })
                    },
                    {
                        BlmStateIds.Blizzard2Spam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => NoOp(),                                                  BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),               () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana >= 9000 && ActionResourceManager.BlackMage.UmbralStacks > 0,  () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Fire2Spam),
                                new StateTransition<BlmStateIds>(() => SpamSpell(Spells.Blizzard2),                       () => ForcedSyncCast(Spells.Blizzard2, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam)
                            })
                    },
                    {
                        BlmStateIds.Blizzard2SpamNoThunder,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => NoOp(),                                                  BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana >= 9000,         () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Fire2SpamNoThunder),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Blizzard2, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam),
                            })
                    },
                    {
                        BlmStateIds.Fire2Spam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                  () => NoOp(),                                                  BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                       () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Fire2Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire2.AdjustedSpellCostBlm(), () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Blizzard2Spam),
                                new StateTransition<BlmStateIds>(() => SpamSpell(Spells.Fire2),                                   () => ForcedSyncCast(Spells.Fire2, Core.Me.CurrentTarget),     BlmStateIds.Fire2Spam),
                                new StateTransition<BlmStateIds>(() => true,                                                      () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Fire2SpamNoThunder)
                            })
                    },
                    {
                        BlmStateIds.Fire2SpamNoThunder,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                  () => NoOp(),                                                  BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                       () => CastThunder(Core.Me.CurrentTarget),                      BlmStateIds.Fire2Spam),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < Spells.Fire2.AdjustedSpellCostBlm(), () => ForcedSyncCast(Spells.Transpose, Core.Me.CurrentTarget), BlmStateIds.Blizzard2SpamNoThunder),
                                new StateTransition<BlmStateIds>(() => true,                                                      () => ForcedSyncCast(Spells.Fire2, Core.Me.CurrentTarget),     BlmStateIds.Fire2Spam),
                            })
                    },
                    {
                        BlmStateIds.FreezeSpam,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => NoOp(),                                               BlmStateIds.Start, true),    
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                   BlmStateIds.FreezeSpam), 
                                new StateTransition<BlmStateIds>(() => SpamSpell(Spells.Freeze),            () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.FreezeSpam),
                                new StateTransition<BlmStateIds>(() => true,                                () => CastThunder(Core.Me.CurrentTarget),                   BlmStateIds.FreezeSpamNoThunder)
                            })
                    },
                    {
                        BlmStateIds.FreezeSpamNoThunder,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => NoOp(),                                               BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                   BlmStateIds.FreezeSpam),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.FreezeSpam),
                            })
                    },
                    {
                        BlmStateIds.FreezeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                              () => NoOp(),                                             BlmStateIds.Start, true),
                                //TODO Is enochian needed here (we are using it only for damage buff)
                                new StateTransition<BlmStateIds>(() => !Core.Me.HasEnochian(),                                () => ForcedSyncCast(Spells.Enochian, Core.Me),           BlmStateIds.FreezeSucceeded),
                                //TODO: Should we sharpcast before every thundercloud proc?
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && !CanCastFoul(), () => ForcedSyncCast(Spells.Sharpcast, Core.Me),          BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && !CanCastFoul(), () => CastThunder(Core.Me.CurrentTarget),                 BlmStateIds.AoEThunderSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                             () => ForcedSyncCast(Spells.Transpose, Core.Me),          BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => ForcedSyncCast(Spells.Foul, Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => ForcedSyncCast(Spells.Sharpcast, Core.Me),          BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                  () => CastThunder(Core.Me.CurrentTarget),                 BlmStateIds.AoEThunderSucceeded)
                            })
                    },                    
                    {
                        BlmStateIds.TransposeSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode, () => NoOp(),                                               BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => true,     () => ForcedSyncCast(Spells.Foul, Core.Me.CurrentTarget),   BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,     () => ForcedSyncCast(Spells.Freeze, Core.Me.CurrentTarget), BlmStateIds.FreezeSucceeded)
                            })
                    },
                    {
                        BlmStateIds.AoEThunderSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => NoOp(),                                              BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.AoEThunderSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 2000,          () => ForcedSyncCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Triplecast, Core.Me),          BlmStateIds.TriplecastSucceeded),                                
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget), BlmStateIds.AoEFire3Succeeded)
                            })
                    },
                    {
                        BlmStateIds.TriplecastSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,     () => NoOp(),                                              BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => true,         () => ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget), BlmStateIds.Fire3SucceededAvoidSwiftcast)                                
                            })
                    },
                    {
                        BlmStateIds.AoEFire3Succeeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                                                                                  () => NoOp(),                                              BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),                                                                                       () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.AoEFire3Succeeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                                                                                                 () => ForcedSyncCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel >= 68,                                                                                                () => Swiftcast(Spells.Flare, Core.Me.CurrentTarget),      BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => ForcedSyncClassLevel >= 68,                                                                                                () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && Core.Me.CurrentMana >= Spells.Fire4.AdjustedSpellCostBlm() + 800,                                 () => ForcedSyncCast(Spells.Fire4, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => (EnemiesNearTarget >= 5 || ForcedSyncClassLevel < 60) && Core.Me.CurrentMana >= Spells.Fire2.AdjustedSpellCostBlm() + 800, () => ForcedSyncCast(Spells.Fire2, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                                      () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FinalFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.FireUntilLowMana,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                                                                                                                                                               () => NoOp(),                                              BlmStateIds.Start, true),                                                                                                            
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud) && ActionResourceManager.BlackMage.StackTimer >= Spells.Thunder2.AdjustedCooldown + Spells.Flare.AdjustedCastTime + TimeSpan.FromMilliseconds(150), () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,                                                                                                                                                              () => ForcedSyncCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => EnemiesNearTarget < 5 && CanCastAnother(Spells.Fire4, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),                                                                            () => ForcedSyncCast(Spells.Fire4, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => (EnemiesNearTarget >= 5 || ForcedSyncClassLevel < 60) && CanCastAnother(Spells.Fire2, 800, Spells.Flare.AdjustedCastTime.TotalMilliseconds),                                            () => ForcedSyncCast(Spells.Fire2, Core.Me.CurrentTarget), BlmStateIds.FireUntilLowMana),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                                                                                                   () => Swiftcast(Spells.Flare, Core.Me.CurrentTarget),      BlmStateIds.FinalFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                                                                                                                                                                   () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FinalFlareSucceeded)
                            })
                    },
                    {
                        BlmStateIds.Fire3SucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => NoOp(),                                              BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.Fire3SucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800,           () => ForcedSyncCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.InitialFlareSucceededAvoidSwiftcast)
                            })
                    },
                    {
                        BlmStateIds.InitialFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => NoOp(),                                              BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),               () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800 && ManaFontDelayElapsed, () => ForcedSyncCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                              () => Swiftcast(Spells.Flare, Core.Me.CurrentTarget),      BlmStateIds.FinalFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                              () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FinalFlareSucceeded)

                            })
                    },
                    {
                        BlmStateIds.InitialFlareSucceededAvoidSwiftcast,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                                          () => NoOp(),                                              BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud),               () => CastThunder(Core.Me.CurrentTarget),                  BlmStateIds.InitialFlareSucceededAvoidSwiftcast),
                                new StateTransition<BlmStateIds>(() => Core.Me.CurrentMana < 800 && ManaFontDelayElapsed, () => ForcedSyncCast(Spells.Transpose, Core.Me),           BlmStateIds.TransposeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                              () => ForcedSyncCast(Spells.Flare, Core.Me.CurrentTarget), BlmStateIds.FinalFlareSucceeded)

                            })
                    },
                    {
                        BlmStateIds.FinalFlareSucceeded,
                        new State<BlmStateIds>(
                            new List<StateTransition<BlmStateIds>>()
                            {
                                new StateTransition<BlmStateIds>(() => !AoEMode,                            () => NoOp(),                                    BlmStateIds.Start, true),
                                new StateTransition<BlmStateIds>(() => Core.Me.HasAura(Auras.ThunderCloud), () => CastThunder(Core.Me.CurrentTarget),        BlmStateIds.FreezeSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.ManaFont, Core.Me),  BlmStateIds.InitialFlareSucceeded),
                                new StateTransition<BlmStateIds>(() => true,                                () => ForcedSyncCast(Spells.Transpose, Core.Me), BlmStateIds.TransposeSucceeded)
                            })
                    }});
            StateMachineManager.RegisterStateMachine(mStateMachine);
        }

        public static async Task<bool> Rest()
        {
            return false;
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();


            if (ActionResourceManager.BlackMage.AstralStacks > 0 && ActionResourceManager.BlackMage.UmbralStacks == 0)
            {
                if (Core.Me.CurrentManaPercent < 70 && Spells.Transpose.Cooldown == TimeSpan.Zero)
                {
                    return await ForcedSyncCast(Spells.Transpose, Core.Me);
                }
            }

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 23);
                }
            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (Core.Me.IsMounted)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();


            if (await GambitLogic.Gambit()) return true;

            // if (await Buff.TransposeMovement()) return true;

            return false;
        }

        public static async Task<bool> CombatBuff()
        {
            return false;
        }

        public static bool AoEMode => BlackMageSettings.Instance.UseAoe && EnemiesNearTarget >= BlackMageSettings.Instance.AoeEnemies && ForcedSyncClassLevel >= 18;       
        
        public static int EnemiesNearTarget => CombatUtil.Enemies.Where(e => e.Distance(Core.Me.CurrentTarget) <= 5 + e.CombatReach).Count();

        public static async Task<bool> Combat()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 23);
                }
            }

            await Casting.CheckForSuccessfulCast();

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;
            if (Core.Me.CurrentTarget.HasAnyAura(Auras.Invincibility))
                return false;
            if (await CustomOpenerLogic.Opener()) return true;

            return await mStateMachine.Pulse();
        }

        public static async Task<bool> SingleTargetCombat()
        {
            //DON'T CHANGE THE ORDER OF THESE
            if (await Buff.Enochian()) return true;
            if (await Buff.Triplecast()) return true;
            if (await Buff.Sharpcast()) return true;
            if (await Buff.ManaFont()) return true;
            if (await Buff.LeyLines()) return true;
            if (await Buff.UmbralSoul()) return true;

            if (await SingleTarget.Blizzard4()) return true;
            if (await SingleTarget.Fire()) return true;
            if (await SingleTarget.Thunder3()) return true;
            if (await SingleTarget.Xenoglossy()) return true;
            if (await SingleTarget.Fire4()) return true;
            if (await SingleTarget.Despair()) return true;

            if (await SingleTarget.Fire3()) return true;

            if (await SingleTarget.Blizzard()) return true;
            if (await SingleTarget.Blizzard3()) return true;

            if (Core.Me.ClassLevel < 80)
                return await ForcedSyncCast(Spells.Fire3, Core.Me.CurrentTarget);

            return false;
        }

        public static async Task<bool> PvP()
        {
            return false;
        }
    }
}
