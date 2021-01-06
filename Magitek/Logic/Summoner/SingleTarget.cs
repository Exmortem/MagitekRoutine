﻿using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.QueueSpell;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Summoner
{
    internal static class SingleTarget
    {
        public static async Task<bool> Ruin()
        {
            if (!SummonerSettings.Instance.Ruin) return false;

            //if ((int)PetManager.ActivePetType == 10)
            //    return await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget);

            if (Casting.LastSpell == Spells.Deathflare)
                return false;

            if (Core.Me.ClassLevel >= 40 && MovementManager.IsMoving)
            {
                if (Spells.EgiAssault.Cooldown.TotalMilliseconds > 1)
                    return await Spells.EgiAssault.Cast(Core.Me.CurrentTarget);
                if (Spells.EgiAssault2.Cooldown.TotalMilliseconds > 1)
                    return await Spells.EgiAssault2.Cast(Core.Me.CurrentTarget);
            }

            //if (Core.Me.ClassLevel >= 38 && MovementManager.IsMoving && !ActionResourceManager.Summoner.DreadwyrmTrance && !Core.Me.HasAura(Auras.EverlastingFlight))
            //    return await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget);

            return await Spells.SmnRuin.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> BahamutCheese()
        {
            if (Core.Me.HasTarget)
            {
                Logger.Error("Queuing Bahamut");

                SpellQueueLogic.SpellQueue.Clear();
                SpellQueueLogic.Timeout.Start();
                SpellQueueLogic.CancelSpellQueue = () => SpellQueueLogic.Timeout.ElapsedMilliseconds > 18000;
                SpellQueueLogic.CancelSpellQueue = () => MovementManager.IsMoving;
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ruin3, Wait = new QueueSpellWait() { Name = "Wait for Ruin3", Check = () => ActionManager.CanCast(Spells.Ruin3, null), WaitTime = 3000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ruin3, Wait = new QueueSpellWait() { Name = "Wait for Ruin3", Check = () => ActionManager.CanCast(Spells.Ruin3, null), WaitTime = 3000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ruin4, Wait = new QueueSpellWait() { Name = "Wait for Ruin4", Check = () => ActionManager.CanCast(Spells.Ruin4, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.EnkindleBahamut, Wait = new QueueSpellWait() { Name = "Wait for Enkindle", Check = () => ActionManager.CanCast(Spells.EnkindleBahamut, null), WaitTime = 3000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ruin3, Wait = new QueueSpellWait() { Name = "Wait for Ruin3", Check = () => ActionManager.CanCast(Spells.Ruin4, null), WaitTime = 3000, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ruin4, Wait = new QueueSpellWait() { Name = "Wait for Ruin4", Check = () => ActionManager.CanCast(Spells.Ruin4, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.EnergyDrain, Wait = new QueueSpellWait() { Name = "Wait for ED", Check = () => ActionManager.CanCast(Spells.EnergyDrain, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Swiftcast, TargetSelf = true, Wait = new QueueSpellWait() { Name = "Wait for SW", Check = () => ActionManager.CanCast(Spells.Swiftcast, null), WaitTime = 1500, EndQueueIfWaitFailed = true }, });
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell { Spell = Spells.Ruin3, Wait = new QueueSpellWait() { Name = "Wait for Ruin3", Check = () => ActionManager.CanCast(Spells.Ruin3, null), WaitTime = 3000, EndQueueIfWaitFailed = true }, });
                return false;
            }
            return false;
        }

        public static async Task<bool> Ruin4()
        {
            if (Core.Me.ClassLevel < 62)
            {
                return false;
            }
            if (!SummonerSettings.Instance.Ruin4) return false;

            if (!Core.Me.HasAura(Auras.FurtherRuin)) return false;

            if (Core.Me.CharacterAuras.GetAuraStacksById(Auras.FurtherRuin) == 4 && Spells.Trance.Cooldown.TotalSeconds < Spells.EgiAssault2.Cooldown.TotalSeconds && Core.Me.CharacterAuras.GetAuraStacksById(Auras.FurtherRuin) == 4 && Spells.Trance.Cooldown.TotalSeconds < Spells.EgiAssault.Cooldown.TotalSeconds)
                return false;

            if (ActionResourceManager.Summoner.DreadwyrmTrance)
            {
                //Dwyearn Still
                if (Spells.Trance.Cooldown.TotalSeconds > 48 && Spells.SummonBahamut.Cooldown.TotalMilliseconds == 0)
                    return false;

                //Bahamut Phase:
                if (Spells.SummonBahamut.Cooldown.TotalMilliseconds > 1)
                {
                    if (Core.Me.CharacterAuras.GetAuraStacksById(Auras.FurtherRuin) > 2)
                        return await Spells.Ruin4.Cast(Core.Me.CurrentTarget);

                    if (Core.Me.CharacterAuras.GetAuraStacksById(Auras.FurtherRuin) == 2 && Spells.Swiftcast.Cooldown.TotalMilliseconds == 0)
                    {
                        if (Core.Me.HasTarget)
                        {
                            return await BahamutCheese();
                        }
                    }

                    if (Core.Me.CharacterAuras.GetAuraStacksById(Auras.FurtherRuin) < 2 && Spells.Swiftcast.Cooldown.TotalMilliseconds > 0)
                    {
                        await Ruin();
                        return await Spells.Ruin4.Cast(Core.Me.CurrentTarget);
                    }
                }
            }

            if (Core.Me.CharacterAuras.GetAuraStacksById(Auras.FurtherRuin) != 4) return false;

            return await Spells.Ruin4.Cast(Core.Me.CurrentTarget);
        }

        // public static async Task<bool> Ruin4MaxStacks()
        //  {
        //      if (Core.Me.ClassLevel < 62) return false;

        //      if (!SummonerSettings.Instance.Ruin4) return false;

        //     if (Core.Me.CharacterAuras.GetAuraStacksById(Auras.FurtherRuin) < 4) return false;

        //      return await Spells.Ruin4.Cast(Core.Me.CurrentTarget);
        //  }

        public static async Task<bool> Bio()
        {
            if (!SummonerSettings.Instance.Bio) return false;

            if (Core.Me.ClassLevel < 2)
                return false;

            if (Spells.TriDisaster.Cooldown.TotalMilliseconds < 5000)
                return false;

            if (Casting.LastSpell == Spells.TriDisaster)
                return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, SummonerSettings.Instance.DotRefreshSeconds * 1000))
                return false;

            return await Spells.SmnBio.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Miasma()
        {
            if (!SummonerSettings.Instance.Miasma) return false;

            if (MovementManager.IsMoving) return false;
            var refresh = SummonerSettings.Instance.DotRefreshSeconds * 1000;

            if (Core.Me.ClassLevel < 6)
                return false;

            if (Spells.TriDisaster.Cooldown.TotalMilliseconds < 5000)
                return false;

            if (Casting.LastSpell == Spells.Miasma3)
                return false;

            if (Casting.LastSpell == Spells.TriDisaster)
                return false;

            switch (Core.Me.ClassLevel)
            {
                case var n when n < 6:
                    return false;
                case var n when n < 66:
                    return !Core.Me.CurrentTarget.HasAura(Auras.Miasma, true, refresh) &&
                           await Spells.Miasma.CastAura(Core.Me.CurrentTarget, Auras.Miasma);
                default:
                    return !Core.Me.CurrentTarget.HasAura(Auras.Miasma3, true, refresh) &&
                           await Spells.Miasma3.CastAura(Core.Me.CurrentTarget, Auras.Miasma3);
            }
        }

        public static async Task<bool> EgiAssault()
        {
            if (Spells.EgiAssault.Charges >= 1 && Spells.EgiAssault.Cooldown.TotalMilliseconds < 3000)
                return await Spells.EgiAssault.Cast(Core.Me.CurrentTarget);

            if (!SummonerSettings.Instance.EgiAssault1) return false;

            if ((int)PetManager.ActivePetType == 10) return false;
            //this logic sucks i know, but i suck too, okay? -sage
            if (MovementManager.IsMoving || Spells.EnergyDrain.Cooldown.TotalMilliseconds < 1000 || Spells.Aetherpact.Cooldown.TotalMilliseconds < 1000 || ActionResourceManager.Arcanist.Aetherflow != 0 || Spells.Trance.Cooldown.TotalMilliseconds < 1000 || Spells.SummonBahamut.Cooldown.TotalMilliseconds < 1000)
                return await Spells.EgiAssault2.Cast(Core.Me.CurrentTarget);
            return await Spells.EgiAssault.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fester()
        {
            if (Core.Me.ClassLevel < 18)
            {
                return false;
            }
            if (Spells.Fester.Cooldown.TotalMilliseconds > 1)
                return false;
            if (Spells.Ruin.Cooldown.TotalMilliseconds < 850)
                return false;
            if (!SummonerSettings.Instance.Fester) return false;

            if (ActionResourceManager.Arcanist.Aetherflow == 0) return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, 2000) && Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.MiasmaAuras, true, 2000))
                return await Spells.Fester.Cast(Core.Me.CurrentTarget);

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2 || Casting.LastSpell != Spells.EgiAssault || Casting.LastSpell != Spells.EgiAssault2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance || !Core.Me.HasAura(Auras.EverlastingFlight))
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return false;
        }

        public static async Task<bool> EnergyDrain()
        {
            if (Core.Me.ClassLevel < 18) return false;
            if (Spells.EnergyDrain.Cooldown.TotalMilliseconds > 1)
                return false;
            if (Spells.Ruin.Cooldown.TotalMilliseconds < 850)
                return false;
            if (!SummonerSettings.Instance.EnergyDrain) return false;

            if (ActionResourceManager.Arcanist.Aetherflow > 0) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2 || Casting.LastSpell != Spells.EgiAssault || Casting.LastSpell != Spells.EgiAssault2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance || !Core.Me.HasAura(Auras.EverlastingFlight))
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.EnergyDrain.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EgiAssault2()
        {

            if (Spells.EgiAssault2.Charges >= 1 && Spells.EgiAssault2.Cooldown.TotalMilliseconds < 3000)
                return await Spells.EgiAssault2.Cast(Core.Me.CurrentTarget);

            if (!SummonerSettings.Instance.EgiAssault2) return false;

            if ((int)PetManager.ActivePetType == 10) return false;

            if (MovementManager.IsMoving || Spells.EnergyDrain.Cooldown.TotalMilliseconds < 1000 || Spells.Aetherpact.Cooldown.TotalMilliseconds < 1000 || ActionResourceManager.Arcanist.Aetherflow != 0 || Spells.Trance.Cooldown.TotalMilliseconds < 1000 || Spells.SummonBahamut.Cooldown.TotalMilliseconds < 1000)
                return await Spells.EgiAssault2.Cast(Core.Me.CurrentTarget);

            return await Spells.EgiAssault2.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> Enkindle()
        {
            if (Core.Me.ClassLevel < 50) return false;
            if (Spells.Enkindle.Cooldown.TotalMilliseconds > 1)
                return false;
            if (!SummonerSettings.Instance.Enkindle) return false;
            if (Spells.Ruin.Cooldown.TotalMilliseconds < 850)
                return false;
            if ((int)PetManager.ActivePetType == 10 || (int)PetManager.ActivePetType == 14) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2 || Casting.LastSpell != Spells.EgiAssault || Casting.LastSpell != Spells.EgiAssault2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance || !Core.Me.HasAura(Auras.EverlastingFlight))
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.Enkindle.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TriDisaster()
        {
            if (Core.Me.ClassLevel < 56) return false;

            if (!SummonerSettings.Instance.TriDisaster) return false;

            if (Spells.Ruin.Cooldown.TotalMilliseconds < 850 && Spells.Trance.Cooldown.TotalMilliseconds > 2000 && Spells.TriDisaster.Cooldown.TotalMilliseconds > 0)
                return false;

            if (ActionResourceManager.Summoner.DreadwyrmTrance || Core.Me.HasAura(Auras.EverlastingFlight))
            {
                if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, 1000)) return false;
                if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.MiasmaAuras, true, 1000)) return false;

                return await Spells.TriDisaster.Cast(Core.Me.CurrentTarget);
            }

            //if (!Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, 4000) || !Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.MiasmaAuras, true, 3000))
            //    if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2 || Casting.LastSpell != Spells.EgiAssault || Casting.LastSpell != Spells.EgiAssault2 || Casting.LastSpell != Spells.Ruin4)
            //        if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
            //            return await Spells.TriDisaster.Cast(Core.Me.CurrentTarget);

            if (Spells.Trance.Cooldown.TotalMilliseconds < 2000 && !Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, 3000))
                if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2 || Casting.LastSpell != Spells.EgiAssault || Casting.LastSpell != Spells.EgiAssault2 || Casting.LastSpell != Spells.Ruin4)
                    if (await Spells.EgiAssault.Cast(Core.Me.CurrentTarget))
                        return true;

            if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, 2000)) return false;
            if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.MiasmaAuras, true, 2000)) return false;

            return await Spells.TriDisaster.Cast(Core.Me.CurrentTarget); ;
        }

        public static async Task<bool> Deathflare()
        {
            if (Core.Me.ClassLevel < 60) return false;

            if (Spells.Deathflare.Cooldown.TotalMilliseconds > 2)
                return false;

            if (Spells.SummonBahamut.Cooldown.TotalMilliseconds > 2)
                return false;

            if (!SummonerSettings.Instance.Deathflare) return false;

            if (!ActionResourceManager.Summoner.DreadwyrmTrance) return false;

            if (ActionResourceManager.Summoner.Timer.Seconds > 8) return false;

            //if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2 || Casting.LastSpell != Spells.EgiAssault || Casting.LastSpell != Spells.EgiAssault2)
            //    if (!ActionResourceManager.Summoner.DreadwyrmTrance || !Core.Me.HasAura(Auras.EverlastingFlight))
            //        if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
            //            return true;
            return await Spells.Deathflare.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EnkindleBahamut()
        {
            if (Core.Me.ClassLevel < 70) return false;
            if (Spells.EnkindleBahamut.Cooldown.TotalMilliseconds > 2)
                return false;

            if ((int)PetManager.ActivePetType != 10 && (int)PetManager.ActivePetType != 14) return false;

            if ((int)PetManager.ActivePetType == 10 && !SummonerSettings.Instance.EnkindleBahamut) return false;

            if ((int)PetManager.ActivePetType == 14 && !SummonerSettings.Instance.EnkindlePhoenix) return false;

            if (ActionResourceManager.Summoner.Timer.TotalMilliseconds > 18000) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance || !Core.Me.HasAura(Auras.EverlastingFlight))
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.EnkindleBahamut.Cast(Core.Me.CurrentTarget);
        }
    }
}