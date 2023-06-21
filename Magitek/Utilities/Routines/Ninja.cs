using ff14bot;
using ff14bot.Enums;
using ff14bot.Objects;
using Magitek.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Utilities.Routines
{
    internal static class Ninja
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Ninja, Spells.SpinningEdge, new List<SpellData>() { Spells.Ten, Spells.Jin, Spells.Chi, Spells.Ninjutsu });

        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;

        private static bool TenChiJin = false;

        public static List<SpellData> UsedMudras = new List<SpellData>();
        public static int OpenerBurstAfterGCD = 2;

        private static readonly List<SpellData> Mudras = new List<SpellData>() { Spells.Ten, Spells.Jin, Spells.Chi };

        public static async Task<bool> PrepareNinjutsu(SpellData endMudra, int ninjustsuLength, GameObject target)
        {

            if (UsedMudras.Count < ninjustsuLength)
            {

                if (UsedMudras.Count < ninjustsuLength - 1)
                {
                    List<SpellData> availableMudras = Mudras.FindAll(x => x != endMudra && !UsedMudras.Contains(x));

                    if (await availableMudras[new Random().Next(availableMudras.Count)].Cast(Core.Me))
                    {
                        await Casting.CheckForSuccessfulCast();
                        UsedMudras.Add(Casting.SpellCastHistory.First().Spell);
                        return true;
                    }

                }

                else if (await endMudra.Cast(Core.Me))
                {
                    UsedMudras.Add(endMudra);
                    return true;
                }
            }

            return await Spells.Ninjutsu.Cast(target);

        }

        public static async Task<bool> PrepareNinjutsu(SpellData ninjutsu, GameObject target)
        {

            Dictionary<SpellData, SpellData> NinjutsuEndMudra = new Dictionary<SpellData, SpellData>
            {
                { Spells.FumaShuriken   , Spells.Ten },
                { Spells.Raiton         , Spells.Chi },
                { Spells.Katon          , Spells.Ten },
                //Kassatsu Ninjutsu
                { Spells.GokaMekkyaku   , Spells.Ten },
                { Spells.Hyoton         , Spells.Jin },
                //Kassatsu Ninjutsu
                { Spells.HyoshoRanryu   , Spells.Jin },
                { Spells.Suiton         , Spells.Jin },
                { Spells.Doton          , Spells.Chi },
                { Spells.Huton          , Spells.Ten }
            };

            Dictionary<SpellData, int> NinjutsuComplexity = new Dictionary<SpellData, int>
            {
                { Spells.FumaShuriken   , 1 },
                { Spells.Raiton         , 2 },
                { Spells.Katon          , 2 },
                //Kassatsu Ninjutsu
                { Spells.GokaMekkyaku   , 2 },
                { Spells.Hyoton         , 2 },
                //Kassatsu Ninjutsu
                { Spells.HyoshoRanryu   , 2 },
                { Spells.Suiton         , 3 },
                { Spells.Doton          , 3 },
                { Spells.Huton          , 3 }
            };

            if (TenChiJin)
            {
                NinjutsuEndMudra = new Dictionary<SpellData, SpellData>
                {
                    { Spells.FumaShuriken   , Spells.Ten },
                    { Spells.Raiton         , Spells.Chi },
                    { Spells.Suiton         , Spells.Jin }
                };

                NinjutsuComplexity = new Dictionary<SpellData, int>
                {
                    { Spells.FumaShuriken   , 1 },
                    { Spells.Raiton         , 1 },
                    { Spells.Suiton         , 1 }
                };
            }

            if (!NinjutsuEndMudra.ContainsKey(ninjutsu))
                return false;

            if (UsedMudras.Count < NinjutsuComplexity[ninjutsu])
            {

                if (UsedMudras.Count < NinjutsuComplexity[ninjutsu] - 1)
                {
                    List<SpellData> availableMudras = Mudras.FindAll(x => x != NinjutsuEndMudra[ninjutsu] && !UsedMudras.Contains(x));

                    if (await availableMudras[new Random().Next(availableMudras.Count)].Cast(Core.Me))
                    {
                        await Casting.CheckForSuccessfulCast();
                        UsedMudras.Add(Casting.SpellCastHistory.First().Spell);
                        return true;
                    }

                }

                else if (await NinjutsuEndMudra[ninjutsu].Cast(Core.Me))
                {
                    UsedMudras.Add(NinjutsuEndMudra[ninjutsu]);
                    return true;
                }
            }

            if (TenChiJin)
                return await NinjutsuEndMudra[ninjutsu].Cast(target);

            return await ninjutsu.Cast(target);

        }

        public static void RefreshVars()
        {

            switch (UsedMudras.Count)
            {
                case 0:
                    break; 

                case 1:

                    if (Core.Me.HasMyAura(Auras.Mudra) || Core.Me.HasMyAura(Auras.TenChiJin))
                        break;

                    if (!Core.Me.HasMyAura(Auras.TenChiJin) && !Core.Me.HasMyAura(Auras.Mudra) && new List<SpellData>() { Spells.Ten, Spells.Chi, Spells.Jin }.Contains(Casting.SpellCastHistory.First().Spell))
                        break;
                                       
                    UsedMudras.Clear();
                    break;

                case 2:

                case 3:

                    if (Core.Me.HasMyAura(Auras.Mudra))
                        break;

                    UsedMudras.Clear();
                    break;

                default:
                    break;
            }

            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            if (!TenChiJin && Casting.SpellCastHistory.Count() > 0 && Casting.SpellCastHistory.First().Spell == Spells.TenChiJin)
            {
                TenChiJin = true;
            }
            if (TenChiJin && !Core.Me.HasMyAura(Auras.TenChiJin) && Casting.SpellCastHistory.Count() > 0 && Casting.SpellCastHistory.First().Spell != Spells.TenChiJin)
            {
                TenChiJin = false;
            }

            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();

        }
    }
}
