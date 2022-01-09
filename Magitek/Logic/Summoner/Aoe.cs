using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using Magitek.Utilities.Routines;
using Pathfinding;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using ArcResources = ff14bot.Managers.ActionResourceManager.Arcanist;
using SmnResources = ff14bot.Managers.ActionResourceManager.Summoner;
using static Magitek.Utilities.Routines.Summoner;


namespace Magitek.Logic.Summoner
{
    internal static class Aoe
    {
        public static async Task<bool> AstralFlow()
        {
            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.SummonedPet() == SmnPets.Bahamut) return await Deathflare();
            if (ArcResources.TranceTimer > 0 && Core.Me.SummonedPet() == SmnPets.Carbuncle) return await Deathflare();

            if (Core.Me.SummonedPet() == SmnPets.Pheonix) return await Rekindle();

            if (SmnResources.ActivePet == SmnResources.ActivePetType.Ifrit) return await CrimsonCyclone();
            if (SmnResources.ActivePet == SmnResources.ActivePetType.Titan) return await MountainBuster();
            if (SmnResources.ActivePet == SmnResources.ActivePetType.Garuda) return await Slipstream();

            return false;
        }
        public static async Task<bool> Deathflare()
        {
            if (!SummonerSettings.Instance.Deathflare)
                return false;

            if (!Spells.Deathflare.IsKnownAndReady())
                return false;

            if (GlobalCooldown.CanWeave(1))
                return false;
            
            if (!GlobalCooldown.CanWeave())
                return false;
            
            return await Spells.Deathflare.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Rekindle()
        {
            if (!SummonerSettings.Instance.Rekindle)
                return false;

            if (!Spells.Rekindle.IsKnownAndReady())
                return false;
            
            if (GlobalCooldown.CanWeave(1))
                return false;
            
            if (!GlobalCooldown.CanWeave())
                return false;

            var targetNeedsHealing = Group.CastableAlliesWithin30
                .FirstOrDefault(x => x.CurrentHealthPercent < SummonerSettings.Instance.RekindleHPThreshold);

            if (targetNeedsHealing == null)
                return false;

            return await Spells.Rekindle.HealAura(targetNeedsHealing, Auras.Rekindle);
        }
        
        public static async Task<bool> CrimsonCyclone()
        {
            if (!SummonerSettings.Instance.CrimsonCyclone)
                return false;

            if (await CrimsonStrike()) return true;
            
            if (!Spells.CrimsonCyclone.IsKnownAndReady())
                return false;

            if (SmnResources.ElementalAttunement > 1)
                return false;

            if (!Core.Me.HasAura(Auras.IfritsFavor))
                return false;

            return await Spells.CrimsonCyclone.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> CrimsonStrike()
        {
            if (!SummonerSettings.Instance.CrimsonStrike)
                return false;

            if (!Spells.CrimsonStrike.IsKnownAndReady())
                return false;
            
            return await Spells.CrimsonStrike.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> MountainBuster()
        {
            if (!SummonerSettings.Instance.MountainBuster)
                return false;

            if (!Spells.MountainBuster.IsKnownAndReady())
                return false;

            if (!Core.Me.HasAura(Auras.TitansFavor))
                return false;
            
            return await Spells.MountainBuster.Cast(Core.Me.CurrentTarget);
        }
        
        public static async Task<bool> Slipstream()
        {
            if (!SummonerSettings.Instance.Slipstream)
                return false;

            if (!Spells.Slipstream.IsKnownAndReady())
                return false;

            if (!Core.Me.HasAura(Auras.GarudasFavor))
                return false;
            
            return await Spells.Slipstream.Cast(Core.Me.CurrentTarget);
        }
        public static async Task<bool> EnergySiphon()
        {
            if (!SummonerSettings.Instance.EnergySiphon)
                return false;

            if (!Spells.EnergySiphon.IsKnownAndReady())
                return false;
            
            if (SmnResources.Aetherflow + ArcResources.Aetherflow != 0)
                return false;
            
            if (ArcResources.TranceTimer + SmnResources.TranceTimer == 0)
                return false;
            
            if (!GlobalCooldown.CanWeave(1))
                return false;

            return await Spells.EnergySiphon.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Outburst()
        {
            if (!SummonerSettings.Instance.Outburst)
                return false;

            if (!Spells.Outburst.IsKnownAndReady())
                return false;

            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 3)
                return false; 
            
            if (Core.Me.SummonedPet() == SmnPets.Pheonix)
                return await Spells.BrandofPurgatory.Cast(Core.Me.CurrentTarget);

            if (Core.Me.SummonedPet() == SmnPets.Bahamut)
                return await Spells.AstralFlare.Cast(Core.Me.CurrentTarget);
            
            if (Spells.AstralFlare.IsKnownAndReady() && SmnResources.TranceTimer > 0 && Core.Me.SummonedPet() == SmnPets.Carbuncle) //It means we're in Dreadwyrm Trance
                return await Spells.AstralFlare.Cast(Core.Me.CurrentTarget);

            if (Core.Me.CurrentJob == ClassJobType.Summoner)
                switch (SmnResources.ActivePet)
                {
                    case SmnResources.ActivePetType.Ifrit when Spells.RubyCatastrophe.IsKnown():
                        return await Spells.RubyCatastrophe.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Ifrit when Spells.RubyDisaster.IsKnown():
                        return await Spells.RubyDisaster.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Ifrit:
                        return await Spells.RubyOutburst.Cast(Core.Me.CurrentTarget);

                    case SmnResources.ActivePetType.Titan when Spells.TopazCatastrophe.IsKnown():
                        return await Spells.TopazCatastrophe.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Titan when Spells.TopazDisaster.IsKnown():
                        return await Spells.TopazDisaster.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Titan:
                        return await Spells.TopazOutburst.Cast(Core.Me.CurrentTarget);

                    case SmnResources.ActivePetType.Garuda when Spells.EmeraldCatastrophe.IsKnown():
                        return await Spells.EmeraldCatastrophe.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Garuda when Spells.EmeraldDisaster.IsKnown():
                        return await Spells.EmeraldDisaster.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Garuda:
                        return await Spells.EmeraldOutburst.Cast(Core.Me.CurrentTarget);
                }

            if (Core.Me.CurrentJob == ClassJobType.Arcanist)
                switch (ArcResources.ActivePet)
                {
                    case ArcResources.ActivePetType.Ruby:
                        return await Spells.RubyOutburst.Cast(Core.Me.CurrentTarget);

                    case ArcResources.ActivePetType.Topaz:
                        return await Spells.TopazOutburst.Cast(Core.Me.CurrentTarget);

                    case ArcResources.ActivePetType.Emerald:
                        return await Spells.EmeraldOutburst.Cast(Core.Me.CurrentTarget);
                }

            if (Spells.TriDisaster.IsKnown())
                return await Spells.TriDisaster.Cast(Core.Me.CurrentTarget);
                    
            return await Spells.Outburst.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Painflare()
        {
            if (!SummonerSettings.Instance.Painflare)
                return false;

            if (!Spells.Painflare.IsKnownAndReady())
                return false;

            if (SmnResources.Aetherflow + ArcResources.Aetherflow == 0)
                return false;
            
            if (Core.Me.CurrentTarget.EnemiesNearby(5).Count() < 2) 
                return false;
            
            if (GlobalCooldown.CanWeave())
                return false;
            
            if (!GlobalCooldown.CanWeave(1))
                return false;

            return await Spells.Painflare.Cast(Core.Me.CurrentTarget);
        }
    }
}