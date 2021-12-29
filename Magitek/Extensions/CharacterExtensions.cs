using System;
using System.Collections.Generic;
using ff14bot.Objects;
using Magitek.Utilities.Managers;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ff14bot;
using ff14bot.Managers;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Extensions
{
    internal static class CharacterExtensions
    {
        public static bool NeedsDispel(this Character unit, bool highPriority = false)
        {
            return unit.CharacterAuras.Select(r => r.Id).Intersect(DispelManager.HighPriorityDispels.Union(DispelManager.NormalDispels)).Any();
        }

        public static bool HasAnyDispellableAura(this Character unit)
        {
            return unit.CharacterAuras.Any(r => r.IsDispellable);
        }

        public static bool HasAnyCardAura(this Character unit)
        {
            return unit.HasAnyAura(new uint[] { Auras.TheBalance,
                                                        Auras.TheBalance2,
                                                        Auras.TheBalance3,
                                                        Auras.TheBole,
                                                        Auras.TheBole2,
                                                        Auras.TheBole3,
                                                        Auras.TheArrow,
                                                        Auras.TheArrow2,
                                                        Auras.TheSpear,
                                                        Auras.TheSpear2,
                                                        Auras.TheEwer,
                                                        Auras.TheEwer2,
                                                        Auras.TheEwer3,
                                                        Auras.TheSpire,
                                                        Auras.TheSpire2,
                                                        Auras.TheSpire3,
                                                        Auras.LordofCrowns,
                                                        Auras.LordofCrowns2,
                                                        Auras.LadyofCrowns,
                                                        Auras.LadyofCrowns2
            });
        }

        public static bool IsCastingTankBuster(this Character target)
        {
            if (!Globals.InActiveDuty)
                return false;

            if (!Core.Me.InCombat)
                return false;
                
            if (!target.IsCasting)
                return false;

            if (!target.IsNpc)
                return false;

            if (Group.CastableTanks.All(x => x != target.TargetCharacter))
                return false;

            if (!TankBusters.Contains(target.CastingSpellId)) 
                return false;
            
            return true;

        }

        private static readonly List<uint> TankBusters = new List<uint>() {
            //====Endwalker Dungeons
            
            //Lv 81 - The Tower of Zot
            25257, //Minduruva - Bio
            25290, //Minduruva - Bio
            25257, //Sanduruva - Isitva Siddhi
            25280, //Cinduruva - Isitva Siddhi

            //Lv 83 - The Tower of Babil
            
            //Lv 85 - Vanaspati
            25141, //Terminus Snatcher - Last Grasp
            25154, //Terminus Wrecker - Total Wreck
            25171, //Svarbhanu - Gnashing Of Teeth

            //Lv 87 - Ktisis Hyperboreia
            25182, //Lyssa - Skull Dasher
            25743, //Ladon - Lord Scratch

            //Lv 89 - The Aitiascope
            25686, //Rhitahtyn the Unshakable - Anvil of Tartarus
            25700, //Amon the Undying - Dark Forte
            
            //Lv 90 - The Dead Ends
            25920, //Caustic Grebuloff - Pox Flail
            25949, //Ra-la - Pity
            
            //Lv 90 - Smileton
            26434, //Face - Heart on Fire IV
            26436, //Frameworker - Steel Beam
            26449, //The Big Cheese - Piercing Missile

            //Lv 90 - The Stigma Dreamscape
            25387, //Proto-Omega - Mustard Bomb
            25525, //Arch-Lambda - Wheel


            //====Endwalker Trials
            //Lv 83 - The Dark Inside (Zodiark)
            27490, //Zodiark - Ania

            //Lv 89 - The Mothercrystal (Hydaelyn)
            //Lv 90 - The Final Day (Meteion)


            //====Endwalker Extreme Trials

            //Lv 90 - The Minstrel's Ballad: Zodiark's Fall
            26607, //Zodiark - Ania

            //Lv 90 - The Minstrel's Ballad: Hydaelyn's Call

            //====Endwalker Normal Raids

            //Lv 90 - Asphodelos: The First Circle (Erichthonios)
            26099, //Erichthonios - Heavy Hand

            //Lv 90 - Asphodelos: The Second Circle ()
            //Lv 90 - Asphodelos: The Third Circle ()
            //Lv 90 - Asphodelos: The Fourth Circle ()


            //====Endwalker Savage Raids
        };

        private static readonly List<uint> BigAoes = new List<uint>();

        public static bool HasAnyDpsCardAura(this Character unit)
        {
            return unit.HasAnyAura(new uint[] {     Auras.TheBole,
                                                    Auras.TheBole2,
                                                    Auras.TheBole3,
                                                    Auras.TheEwer,
                                                    Auras.TheEwer2,
                                                    Auras.TheEwer3,
                                                    Auras.TheSpire,
                                                    Auras.TheSpire2,
                                                    Auras.TheSpire3,
                                                    Auras.TheArrow,
                                                    Auras.TheArrow2,
                                                    Auras.TheSpear,
                                                    Auras.TheSpear2,
                                                    Auras.TheBalance,
                                                    Auras.TheBalance2,
                                                    Auras.TheBalance3,
            });
        }

        public static bool HasAnyHealerRegen(this Character unit)
        {
            return unit.HasAnyAura(new uint[]
            {
                Auras.Regen,
                Auras.Regen2,
                Auras.Medica2,
                Auras.AspectedBenefic,
                Auras.AspectedHelios
            });
        }
    }
}
