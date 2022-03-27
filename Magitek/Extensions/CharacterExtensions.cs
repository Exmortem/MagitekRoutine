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
                                                        Auras.TheBalance,
                                                        Auras.TheBole,
                                                        Auras.TheArrow,
                                                        Auras.TheSpear,
                                                        Auras.TheEwer,
                                                        Auras.TheSpire,
                                                        Auras.LordofCrowns,
                                                        Auras.LordofCrowns2,
                                                        Auras.LadyofCrowns,
                                                        Auras.LadyofCrowns2
            });
        }

        public static bool HasAnyDpsCardAura(this Character unit)
        {
            return unit.HasAnyAura(new uint[] {     Auras.TheBole,
                                                    Auras.TheEwer,
                                                    Auras.TheSpire,
                                                    Auras.TheArrow,
                                                    Auras.TheSpear,
                                                    Auras.TheBalance,
            });
        }

        public static bool HasAnyHealerRegen(this Character unit)
        {
            return unit.HasAnyAura(HealerRegens);
        }

        public static float AdjustHealthThresholdByRegen(this Character target, float healthThreshold)
        {

            var regens = HealerRegens;
            var matchingAuras = target.CharacterAuras.Count(r => regens.Contains(r.Id));

            return healthThreshold + (2 * matchingAuras);
        }

        public static uint[] HealerRegens = new uint[] {
                Auras.Regen,
                Auras.Regen2,
                Auras.Medica2,
                Auras.AsylumReceiver,
                Auras.SacredSoilReceiver,
                Auras.WhisperingDawn,
                Auras.AngelsWhisper,
                Auras.AspectedBenefic,
                Auras.AspectedHelios,
                Auras.Kerakeia,
                Auras.PhysisII,
                Auras.CrestOfTimeReturned,
                Auras.Opposition,
                Auras.WheelOfFortune
        };

        public static uint[] HealerShields = new uint[]
        {
            Auras.NocturnalField,
            Auras.Galvanize,
            Auras.EukrasianDiagnosis,
            Auras.EukrasianPrognosis,
            Auras.Haimatinon,
            Auras.Haima,
            Auras.Panhaima,
            Auras.Panhaimatinon,
            Auras.ShakeItOff,
            Auras.BlackestNight
        };

        public static uint[] BuffIgnore = new uint[]
        {
            Auras.DancePartner,
            Auras.ClosedPosition,
            Auras.IronWill,
            Auras.Defiance,
            Auras.Grit,
            Auras.RoyalGuard,
            Auras.EyesOpen,
            Auras.LeftEye,
            Auras.RightEye,
            Auras.Kardia,
            Auras.Kardion,
            Auras.Eukrasia
        };
    }
}
