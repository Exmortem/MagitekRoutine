using ff14bot.Objects;
using Magitek.Utilities.Managers;
using System.Linq;
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
