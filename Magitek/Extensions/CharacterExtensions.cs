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

            CheckAndPopulateTbRef();

            if (!TbRef.Contains(target.TargetCharacter.CastingSpellId)) 
                return false;
            
            Logger.WriteInfo("TankBuster Detected!! \n Unit: {0} \n Targeting: {1} ({2}) \n Casting {3} ({4})",
                target.EnglishName,
                target.TargetCharacter.Name,
                target.CurrentJob,
                target.SpellCastInfo.Name,
                target.CastingSpellId);
            return true;

        }

        private static readonly List<uint> TbRef = new List<uint>();

        private static void CheckAndPopulateTbRef()
        {
            if (TbRef.Any())
            {
                Logger.WriteInfo("TankBuster List Already Populated {0}. Example: {1}",
                    TbRef.Count(),
                    TbRef.FirstOrDefault());
                return;
            }

            var tblist = typeof(TankBusters)
            .GetFields(BindingFlags.Public & BindingFlags.Static)
            .Where(f => f.FieldType == typeof(SpellData))
            .Select(buster => buster.GetValue(null))
            .Cast<SpellData>()
            .Select(r => r.Id)
            .ToList();

            TbRef.AddRange(tblist);

            Logger.WriteInfo("TankBuster List Populated {0}",tblist.Count());
        }

        private static readonly List<uint> TankBusterList = new List<uint>()
        {
            TankBusters.HydaelynEx_DichroicSpectrum.Id,
            TankBusters.HydaelynEx_MousasScorn.Id,
            TankBusters.Smileton_HeartOnFireIV.Id,
            TankBusters.Smileton_PiercingMissile.Id,
            TankBusters.Smileton_TempersFlare.Id,
            TankBusters.Aitiascope_AgleaBite.Id,
            TankBusters.Aitiascope_AnvilOfTartarus.Id,
            TankBusters.Aitiascope_AmonDarkForte.Id,
            TankBusters.Vanaspati_GnashingOfTeeth.Id,
            TankBusters.Vanaspati_LastGasp.Id,
            TankBusters.Vanaspati_TotalWreck.Id,
            TankBusters.ZodiarkEx_Ania.Id,
            TankBusters.Zodiark_Ania.Id,
            TankBusters.Zot_Bio.Id
        };

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
