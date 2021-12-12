using Buddy.Coroutines;
using ff14bot;
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
                if (AvoidanceManager.IsRunningOutOfAvoid)
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

            return Core.Me.HasAura(Auras.Swiftcast) || !MovementManager.IsMoving || spell.AdjustedCastTime <= TimeSpan.Zero;
        }

        public static bool CanCast(this SpellData spell, GameObject target) {
            
            return ActionManager.CanCast(spell, target);

        }

        public static bool CanCast(this SpellData spell) {
            
            return CanCast(spell, Core.Me);
            
        }

        private static async Task<bool> DoAction(SpellData spell, GameObject target, uint aura = 0, bool needAura = false, bool useRefreshTime = false, int refreshTime = 0)
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

            Logger.WriteCast($@"Cast: {spell}");

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

            Logger.WriteCast($@"Cast: {spell}");

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

        public static uint AdjustedSpellCostBlm(this SpellData spell)
        {
            // If it's a Fire spell
            if (AstralSpells.Contains(spell.Id))
            {
                if (ActionResourceManager.BlackMage.AstralStacks > 0
                    // If we have Umbral Hearts, its free
                    && ActionResourceManager.BlackMage.UmbralHearts == 0)
                {
                    return spell.Cost * 2;
                }
                if (ActionResourceManager.BlackMage.AstralStacks > 0
                    // If we have Umbral Hearts, its free
                    && ActionResourceManager.BlackMage.UmbralHearts > 0)
                {
                    if (spell == Spells.Flare)
                        return spell.Cost / 3;
                    return spell.Cost * 0;
                }
                // Umbral makes a Fire spell cost less
                switch (ActionResourceManager.BlackMage.UmbralStacks)
                {
                    // If we have aspect mastery, its free
                    case 3:
                        if (Core.Me.ClassLevel >= 72 &&
                            //Except for Flare =(
                            spell != Spells.Flare)
                            return spell.Cost * 0;
                        break;
                    case 2:
                        return spell.Cost / 4;

                    case 1:
                        return spell.Cost / 2;
                }

                return spell.Cost;
            }

            if (!UmbralSpells.Contains(spell.Id))
                return spell.Cost;

            // Astral makes a Blizzard spell cost less
            switch (ActionResourceManager.BlackMage.AstralStacks)
            {
                case 3:
                    // If we have aspect mastery, its free
                    if (Core.Me.ClassLevel >= 72)
                        return spell.Cost * 0;
                    break;
                case 2:
                    return spell.Cost / 4;

                case 1:
                    return spell.Cost / 2;

            }

            return spell.Cost;
        }

        private static readonly HashSet<uint> AstralSpells = new HashSet<uint>()
        {
            Spells.Fire.Id,
            Spells.Fire2.Id,
            Spells.Fire3.Id,
            Spells.Fire4.Id,
            Spells.Flare.Id
        };

        private static readonly HashSet<uint> UmbralSpells = new HashSet<uint>()
        {
            Spells.Blizzard.Id,
            Spells.Blizzard3.Id,
            Spells.Blizzard4.Id,
            Spells.Freeze.Id
        };

        public static string IconUrl(this SpellData spell)
        {
            var icon = (decimal)spell.Icon;
            var folder = (Math.Floor(icon / 1000) * 1000).ToString(CultureInfo.InvariantCulture).Trim().PadLeft(6, '0');
            var image = spell.Icon.ToString(CultureInfo.InvariantCulture).Trim().PadLeft(6, '0');
            return $@"https://secure.xivdb.com/img/game/{folder}/{image}.png";
        }
    }
}
