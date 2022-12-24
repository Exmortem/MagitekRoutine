using Buddy.Coroutines;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Models.Account;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;
using Debug = Magitek.ViewModels.Debug;

namespace Magitek.Extensions
{
    internal static class SpellDataExtensions
    {
        #region PvpCombo
        public static async Task<bool> CastPvpCombo(this SpellData spell, uint spellPvpCombo, GameObject target, [CallerMemberName] string caller = null, [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = null)
        {
            if (BaseSettings.Instance.DebugCastingCallerMemberName)
            {
                Logger.WriteInfo($@"[Cast Pvp Combo] [{sourceLineNumber}] {caller}");

                if (BaseSettings.Instance.DebugCastingCallerMemberNameIncludePath)
                {
                    Logger.WriteInfo($@"[Path] {sourceFilePath}");
                }
            }

            if (!GameSettingsManager.FaceTargetOnAction && BaseSettings.Instance.AssumeFaceTargetOnAction)
                GameSettingsManager.FaceTargetOnAction = true;

            if (BotManager.Current.IsAutonomous && !GameSettingsManager.FaceTargetOnAction && !RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing) && !MovementManager.IsMoving)
                Core.Me.Face(target);

            return await DoPvPCombo(spell, spellPvpCombo, target);
        }

        private static async Task<bool> DoPvPCombo(SpellData spell, uint pvpComboId, GameObject target)
        {
            if (target == null)
                return false;

            if (target.Distance() > spell.Range)
                return false;

            if (ActionManager.GetPvPComboCurrentActionId(pvpComboId) != spell.Id)
                return false;

            if (!ActionManager.DoPvPCombo(pvpComboId, target))
                return false;

            Logger.WriteCastExecuted($@"Cast PVP Combo: [{pvpComboId}] {spell}");

            if (spell.AdjustedCastTime != TimeSpan.Zero)
            {
                if (!await Coroutine.Wait(3000, () => Core.Me.IsCasting))
                    return false;
            }

            Casting.CastingSpell = spell;
            Casting.SpellCastTime = spell.AdjustedCastTime;
            Casting.CastingHeal = false;
            Casting.SpellTarget = target;
            Casting.CastingTime.Restart();

            if (!BaseSettings.Instance.DebugPlayerCasting)
                return true;

            Debug.Instance.CastingSpell = spell;
            Debug.Instance.CastingHeal = false;
            Debug.Instance.SpellTarget = target;

            return true;
        }
        #endregion

        public static async Task<bool> Cast(this SpellData spell, GameObject target, [CallerMemberName] string caller = null, [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = null)
        {
            if (BaseSettings.Instance.DebugCastingCallerMemberName)
            {
                Logger.WriteInfo($@"[Cast] [{sourceLineNumber}] {caller}");

                if (BaseSettings.Instance.DebugCastingCallerMemberNameIncludePath)
                {
                    Logger.WriteInfo($@"[Path] {sourceFilePath}");
                }
            }
            
            if (!GameSettingsManager.FaceTargetOnAction && BaseSettings.Instance.AssumeFaceTargetOnAction)
                GameSettingsManager.FaceTargetOnAction = true;

            if (BotManager.Current.IsAutonomous && !GameSettingsManager.FaceTargetOnAction && !RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing) && !MovementManager.IsMoving)
                Core.Me.Face(target);

            return await DoAction(spell, target);
        }

        public static async Task<bool> CastAura(this SpellData spell, GameObject target, uint aura, bool useRefreshTime = false, int refreshTime = 0, bool needAura = true, [CallerMemberName] string caller = null, [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = null)
        {
            if (BaseSettings.Instance.DebugCastingCallerMemberName)
            {
                Logger.WriteInfo($@"[CastAura] [{sourceLineNumber}] {caller}");

                if (BaseSettings.Instance.DebugCastingCallerMemberNameIncludePath)
                {
                    Logger.WriteInfo($@"[Path] {sourceFilePath}");
                }
            }

            if (!GameSettingsManager.FaceTargetOnAction && BaseSettings.Instance.AssumeFaceTargetOnAction)
                GameSettingsManager.FaceTargetOnAction = true;

            if (BotManager.Current.IsAutonomous && !GameSettingsManager.FaceTargetOnAction && !RoutineManager.IsAnyDisallowed(CapabilityFlags.Facing) && !MovementManager.IsMoving)
                Core.Me.Face(target);

            return await DoAction(spell, target, aura, needAura, useRefreshTime, refreshTime);
        }

        public static async Task<bool> Heal(this SpellData spell, GameObject target, bool healthChecks = true, [CallerMemberName] string caller = null, [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = null)
        {
            if (BaseSettings.Instance.DebugCastingCallerMemberName)
            {
                Logger.WriteInfo($@"[Heal] [{sourceLineNumber}] {caller}");

                if (BaseSettings.Instance.DebugCastingCallerMemberNameIncludePath)
                {
                    Logger.WriteInfo($@"[Path] {sourceFilePath}");
                }
            }

            return await DoActionHeal(spell, target, healthChecks);
        }

        public static async Task<bool> HealAura(this SpellData spell, GameObject target, uint aura, bool healthChecks = true, bool needAura = true, bool useRefreshTime = false, int refreshTime = 0, [CallerMemberName] string caller = null, [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFilePath = null)
        {
            if (BaseSettings.Instance.DebugCastingCallerMemberName)
            {
                Logger.WriteInfo($@"[HealAura] [{sourceLineNumber}] {caller}");

                if (BaseSettings.Instance.DebugCastingCallerMemberNameIncludePath)
                {
                    Logger.WriteInfo($@"[Path] {sourceFilePath}");
                }
            }

            return await DoActionHeal(spell, target, healthChecks, aura, needAura, useRefreshTime, refreshTime);
        }

        private static bool Check(SpellData spell, GameObject target)
        {
            // If we're using an autonomous bot base and we're running out of avoidance then don't cast
            if (BotManager.Current.IsAutonomous)
            {
                if (AvoidanceManager.IsRunningOutOfAvoid && !(Core.Me.HasAura(Auras.Swiftcast) || spell.AdjustedCastTime <= TimeSpan.Zero))
                    return false;
            }

            if (target == null)
                return false;

            if (!ActionManager.HasSpell(spell.Id) && !Core.Me.OnPvpMap())
                return false;

            if (!BaseSettings.Instance.UseCastOrQueue)
            {
                if (!ActionManager.CanCast(spell, target) && !Core.Me.OnPvpMap())
                    return false;
            }
            else
            {
                if (!ActionManager.CanCastOrQueue(spell, target) && !Core.Me.OnPvpMap())
                    return false;
            }

            //if (Core.Me.OnPvpMap())
            //{
            //    if (Core.Me.IsCasting && Core.Me.CastingSpellId == spell.Id)
            //        return false;

            //    if (spell != Spells.PvpPlayDrawn)
            //    {
            //        if (target.Distance() > spell.Range)
            //            return false;

            //        if (spell != Spells.PvpPlayDrawn && spell.Cooldown != TimeSpan.Zero)
            //            return false;
            //    }
            //    else
            //    {
            //        if (target.Distance() > 30) return false;
            //    }
            //}

            if(!BotManager.Current.IsAutonomous) 
                return Core.Me.HasAura(Auras.Swiftcast) || !MovementManager.IsMoving || spell.AdjustedCastTime <= TimeSpan.Zero;
            //we don't care if the bot is moving in Autonomous mode, so we should always return true
            return true;
        }

        public static bool CanCast(this SpellData spell, GameObject target)
        {
            if (!BaseSettings.Instance.UseCastOrQueue)
            {
               return ActionManager.CanCast(spell, target);
            }
            else
            {
               return ActionManager.CanCastOrQueue(spell, target);
            }
        }


        public static bool CanCast(this SpellData spell)
        {
            if (!BaseSettings.Instance.UseCastOrQueue)
            {
                return ActionManager.CanCast(spell, Core.Me);
            }
            else
            {
                return ActionManager.CanCastOrQueue(spell, Core.Me);
            }
        }

        private static async Task<bool> DoAction(SpellData spell, GameObject target, uint aura = 0, bool needAura = false, bool useRefreshTime = false, int refreshTime = 0, bool canCastCheck = true)
        {
            if (!Check(spell, target))
                return false;

            if (spell.GroundTarget)
            {
                if (!ActionManager.DoActionLocation(spell.Id, target.Location))
                    return false;
            }
            else
            {
                if (!ActionManager.DoAction(spell, target))
                    return false;
            }

            Logger.WriteCastExecuted($@"Cast: {spell}");

            if (spell.AdjustedCastTime != TimeSpan.Zero)
            {
                if (!await Coroutine.Wait(3000, () => Core.Me.IsCasting))
                {
                    return false;
                }
            }

            Casting.CastingSpell = spell;
            Casting.SpellCastTime = spell.AdjustedCastTime;
            Casting.CastingHeal = false;
            Casting.SpellTarget = target;
            Casting.NeedAura = needAura;
            Casting.Aura = aura;
            Casting.UseRefreshTime = useRefreshTime;
            Casting.RefreshTime = refreshTime;
            Casting.CastingTime.Restart();

            if (!BaseSettings.Instance.DebugPlayerCasting)
                return true;

            Debug.Instance.CastingSpell = spell;
            Debug.Instance.CastingHeal = false;
            Debug.Instance.SpellTarget = target;
            Debug.Instance.NeedAura = needAura;
            Debug.Instance.Aura = aura;
            Debug.Instance.UseRefreshTime = useRefreshTime;
            Debug.Instance.RefreshTime = refreshTime;

            return true;
        }

        private static async Task<bool> DoActionHeal(SpellData spell, GameObject target, bool healthChecks = true, uint aura = 0, bool needAura = false, bool useRefreshTime = false, int refreshTime = 0)
        {
            if (!Check(spell, target))
                return false;

            if (!ActionManager.DoAction(spell, target))
                return false;

            Logger.WriteCastExecuted($@"Cast: {spell}");

            if (spell.AdjustedCastTime != TimeSpan.Zero)
            {
                if (!await Coroutine.Wait(3000, () => Core.Me.IsCasting))
                {
                    return false;
                }
            }

            Casting.CastingHeal = true;
            Casting.CastingSpell = spell;
            Casting.SpellCastTime = spell.AdjustedCastTime;
            Casting.SpellTarget = target;
            Casting.NeedAura = needAura;
            Casting.Aura = aura;
            Casting.UseRefreshTime = useRefreshTime;
            Casting.DoHealthChecks = healthChecks;
            Casting.RefreshTime = refreshTime;
            Casting.CastingTime.Restart();

            if (!BaseSettings.Instance.DebugPlayerCasting)
                return true;

            Debug.Instance.CastingHeal = true;
            Debug.Instance.CastingSpell = spell;
            Debug.Instance.SpellTarget = target;
            Debug.Instance.NeedAura = needAura;
            Debug.Instance.Aura = aura;
            Debug.Instance.UseRefreshTime = useRefreshTime;
            Debug.Instance.DoHealthChecks = healthChecks;
            Debug.Instance.RefreshTime = refreshTime;

            return true;
        }

        public static bool IsKnown(this SpellData spell)
        {
            return ActionManager.HasSpell(spell.Id);
        }

        public static bool IsReady(this SpellData spell, int remainingTimeInMs = 0)
        {
            if (!BaseSettings.Instance.UseCastOrQueue)
            {
                return spell.Cooldown.TotalMilliseconds <= remainingTimeInMs;
            }
            else
            {
                return spell.Cooldown.TotalMilliseconds <= 500;
            }
            
        }

        public static bool IsKnownAndReady(this SpellData spell, int remainingTimeInMs = 0)
        {
            return spell.IsKnown() && spell.IsReady(remainingTimeInMs);
        }

        public static string IconUrl(this SpellData spell)
        {
            var icon = (decimal)spell.Icon;
            var folder = (Math.Floor(icon / 1000) * 1000).ToString(CultureInfo.InvariantCulture).Trim().PadLeft(6, '0');
            var image = spell.Icon.ToString(CultureInfo.InvariantCulture).Trim().PadLeft(6, '0');
            return $@"https://secure.xivdb.com/img/game/{folder}/{image}.png";
        }
    }
}
