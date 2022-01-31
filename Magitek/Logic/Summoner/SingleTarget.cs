using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.QueueSpell;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using Magitek.Utilities.Routines;
using Pathfinding;
using System;
using System.Linq;
using System.Threading.Tasks;
using ArcResources = ff14bot.Managers.ActionResourceManager.Arcanist;
using SmnResources = ff14bot.Managers.ActionResourceManager.Summoner;
using static Magitek.Utilities.Routines.Summoner;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Summoner
{
    internal static class SingleTarget
    {
        public static async Task<bool> Ruin()
        {
            Logger.WriteInfo("Ruin Check");
            if (!SummonerSettings.Instance.Ruin)
                return false;

            if (Core.Me.SummonedPet() == SmnPets.Pheonix)
                return await Spells.FountainofFire.Cast(Core.Me.CurrentTarget);

            if (Core.Me.SummonedPet() == SmnPets.Bahamut)
                return await Spells.AstralImpulse.Cast(Core.Me.CurrentTarget);
            
            if (Spells.AstralImpulse.IsKnownAndReady() && SmnResources.TranceTimer > 0 && Core.Me.SummonedPet() == SmnPets.Carbuncle) //It means we're in Dreadwyrm Trance
                return await Spells.AstralImpulse.Cast(Core.Me.CurrentTarget);

            if (Core.Me.CurrentJob == ClassJobType.Summoner)
                switch (SmnResources.ActivePet)
                {
                    case SmnResources.ActivePetType.Ifrit when Spells.RubyRite.IsKnown():
                        {
                            if (!Spells.Swiftcast.IsKnownAndReady())
                                return await Spells.RubyRite.Cast(Core.Me.CurrentTarget);

                            var anyDead = Group.DeadAllies.Any(u => !u.HasAura(Auras.Raise) &&
                                                                    u.Distance(Core.Me) <= 30 &&
                                                                    u.InLineOfSight() &&
                                                                    u.IsTargetable);

                            if (anyDead || SmnResources.ElementalAttunement > 1 ||
                                !SummonerSettings.Instance.SwiftRubyRite)
                                return await Spells.RubyRite.Cast(Core.Me.CurrentTarget);

                            if (await Buff.Swiftcast())
                            {
                                while (Core.Me.HasAura(Auras.Swiftcast))
                                {
                                    if (await Spells.RubyRite.Cast(Core.Me.CurrentTarget)) return true;
                                    await Coroutine.Yield();
                                }
                            }

                            return false;
                        }
                    case SmnResources.ActivePetType.Ifrit when Spells.RubyRuinIII.IsKnown():
                        return await Spells.RubyRuinIII.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Ifrit when Spells.RubyRuinII.IsKnown():
                        return await Spells.RubyRuinII.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Ifrit:
                        return await Spells.RubyRuin.Cast(Core.Me.CurrentTarget);
                    

                    case SmnResources.ActivePetType.Titan when Spells.TopazRite.IsKnown():
                        return await Spells.TopazRite.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Titan when Spells.TopazRuinIII.IsKnown():
                        return await Spells.TopazRuinIII.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Titan when Spells.TopazRuinII.IsKnown():
                        return await Spells.TopazRuinII.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Titan:
                        return await Spells.TopazRuin.Cast(Core.Me.CurrentTarget);
                        
                    

                    case SmnResources.ActivePetType.Garuda when Spells.EmeraldRite.IsKnown():
                        return await Spells.EmeraldRite.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Garuda when Spells.EmeraldRuinIII.IsKnown():
                        return await Spells.EmeraldRuinIII.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Garuda when Spells.EmeraldRuinII.IsKnown():
                        return await Spells.EmeraldRuinII.Cast(Core.Me.CurrentTarget);
                    case SmnResources.ActivePetType.Garuda:
                        return await Spells.EmeraldRuin.Cast(Core.Me.CurrentTarget);
                }

            if (Core.Me.CurrentJob == ClassJobType.Arcanist || Core.Me.ClassLevel <= 30)
                switch (ArcResources.ActivePet)
                {
                    case ArcResources.ActivePetType.Ruby when Spells.RubyRuinII.IsKnown():
                        return await Spells.RubyRuinII.Cast(Core.Me.CurrentTarget);
                    case ArcResources.ActivePetType.Ruby:
                        return await Spells.RubyRuin.Cast(Core.Me.CurrentTarget);

                    case ArcResources.ActivePetType.Topaz when Spells.TopazRuinII.IsKnown():
                        return await Spells.TopazRuinII.Cast(Core.Me.CurrentTarget);
                    case ArcResources.ActivePetType.Topaz:
                        return await Spells.TopazRuin.Cast(Core.Me.CurrentTarget);

                    case ArcResources.ActivePetType.Emerald when Spells.EmeraldRuinII.IsKnown():
                        return await Spells.EmeraldRuinII.Cast(Core.Me.CurrentTarget);
                    case ArcResources.ActivePetType.Emerald:
                        return await Spells.EmeraldRuin.Cast(Core.Me.CurrentTarget);
                }    


            if (Spells.Ruin3.IsKnown())
                return await Spells.Ruin3.Cast(Core.Me.CurrentTarget);
                    
            return Spells.Ruin2.IsKnown()
                ? await Spells.Ruin2.Cast(Core.Me.CurrentTarget)
                : await Spells.Ruin.Cast(Core.Me.CurrentTarget);

        }

        public static async Task<bool> Fester()
        {
            if (!SummonerSettings.Instance.Fester)
                return false;
            
            if (!Spells.Fester.IsKnownAndReady())
                return false;
            
            if (SmnResources.Aetherflow + ArcResources.Aetherflow == 0)
                return false;
            
            if (!GlobalCooldown.CanWeave())
                return false;

            return await Spells.Fester.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EnergyDrain()
        {
            if (!SummonerSettings.Instance.EnergyDrain)
                return false;
            
            if (!Spells.EnergyDrain.IsKnownAndReady())
                return false;
            
            if (SmnResources.Aetherflow + ArcResources.Aetherflow != 0)
                return false;
            
            if (ArcResources.TranceTimer + SmnResources.TranceTimer == 0)
                return false;
            
            if (!GlobalCooldown.CanWeave())
                return false;

            return await Spells.EnergyDrain.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Enkindle()
        {
            if (!Core.Me.InCombat)
                return false;

            if (Core.Me.SummonedPet() == SmnPets.Bahamut) return await EnkindleBahamut();
            if (Core.Me.SummonedPet() == SmnPets.Pheonix) return await EnkindlePhoenix();

            return false;
        }

        public static async Task<bool> EnkindleBahamut()
        {
            if (!SummonerSettings.Instance.EnkindleBahamut)
                return false;
                
            if (!Spells.EnkindleBahamut.IsKnownAndReady())
                return false;

            if (!GlobalCooldown.CanWeave())
                return false;
            
            return await Spells.EnkindleBahamut.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EnkindlePhoenix()
        {
            if (!SummonerSettings.Instance.EnkindlePhoenix)
                return false;
                
            if (!Spells.EnkindlePhoenix.IsKnownAndReady())
                return false;
            
            if (!GlobalCooldown.CanWeave())
                return false;
            
            return await Spells.EnkindlePhoenix.Cast(Core.Me.CurrentTarget);
        }
    }
}