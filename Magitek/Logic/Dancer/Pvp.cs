using ff14bot;
using ff14bot.Enums;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Dancer;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Dancer
{
    internal static class Pvp
    {
        public static async Task<bool> Cascade()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.CascadePvp.CanCast())
                return false;

            return await Spells.CascadePvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fountain()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;
            
            if (!Spells.FountainPvp.CanCast())
                return false;

            return await Spells.FountainPvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> ReverseCascade()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ReverseCascadePvp.CanCast())
                return false;

            return await Spells.ReverseCascadePvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> SaberDance()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.SaberDancePvp.CanCast())
                return false;

            return await Spells.SaberDancePvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }

        public static async Task<bool> FountainFall()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.FountainFallPvp.CanCast())
                return false;

            return await Spells.FountainFallPvp.CastPvpCombo(Spells.FountainPvpCombo, Core.Me.CurrentTarget);
        }


        public static async Task<bool> StarfallDance()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.StarfallDancePvp.CanCast())
                return false;

            return await Spells.StarfallDancePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> FanDance()
        {
            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.FanDancePvp.CanCast())
                return false;

            return await Spells.FanDancePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> HoningDance()
        {
            if (!DancerSettings.Instance.Pvp_UseHoningDance)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.HoningDancePvp.CanCast())
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 5 + x.CombatReach) < DancerSettings.Instance.Pvp_HoningDanceMinimumEnemies)
                return false;

            return await Spells.HoningDancePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Contradance()
        {
            if (!DancerSettings.Instance.Pvp_UseContradance)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.ContradancePvp.CanCast())
                return false;

            if (Combat.Enemies.Count(x => x.Distance(Core.Me) <= 15 + x.CombatReach) < DancerSettings.Instance.Pvp_ContradanceMinimumEnemies)
                return false;

            return await Spells.ContradancePvp.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ClosedPosition()
        {
            if (!DancerSettings.Instance.Pvp_UseClosedPosition)
                return false;

            if (!Globals.InParty)
                return false;

            if (Core.Me.HasAura(Auras.ClosedPosition))
                return false;

            if (!Spells.ClosedPositionPvp.CanCast())
                return false;

            IEnumerable<Character> allyList = null;
            switch (DancerSettings.Instance.Pvp_DancePartnerSelectedStrategy)
            {
                case DancePartnerStrategy.ClosestDps:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsDps()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.MeleeDps:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsMeleeDps()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.RangedDps:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsRangedDpsCard()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.Tank:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsTank()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.Healer:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsHealer()).OrderBy(GetWeight);
                    break;
            }

            if (allyList == null)
                return false;

            return await Spells.ClosedPositionPvp.CastAura(allyList.FirstOrDefault(), Auras.ClosedPosition);
        }

        public static async Task<bool> CuringWaltz()
        {
            if (!DancerSettings.Instance.Pvp_UseCuringWaltz)
                return false;

            if (Core.Me.HasAura(Auras.Guard))
                return false;

            if (!Spells.CuringWaltzPvp.CanCast())
                return false;

            var cureTargets = Group.CastableParty.Count(x => x.IsValid && x.CurrentHealthPercent < DancerSettings.Instance.Pvp_CuringWaltzHP && x.Distance(Core.Me) < 5);

            if (Core.Me.HasAura(Auras.ClosedPosition))
            {
                var DancePartner = Group.CastableParty.FirstOrDefault(x => x.HasMyAura(Auras.DancePartner));

                if (DancePartner != null)
                    cureTargets += Group.CastableParty.Count(x => x.IsValid && x.CurrentHealthPercent < DancerSettings.Instance.Pvp_CuringWaltzHP && x.Distance(DancePartner) < 5);
            }

            if (cureTargets < 1)
                return false;

            return await Spells.CuringWaltzPvp.Cast(Core.Me.CurrentTarget);
        }

        private static int GetWeight(Character c)
        {
            switch (c.CurrentJob)
            {
                case ClassJobType.Reaper:
                    return DancerSettings.Instance.Pvp_RprPartnerWeight;

                case ClassJobType.Sage:
                    return DancerSettings.Instance.Pvp_SagPartnerWeight;

                case ClassJobType.Astrologian:
                    return DancerSettings.Instance.Pvp_AstPartnerWeight;

                case ClassJobType.Monk:
                case ClassJobType.Pugilist:
                    return DancerSettings.Instance.Pvp_MnkPartnerWeight;

                case ClassJobType.BlackMage:
                case ClassJobType.Thaumaturge:
                    return DancerSettings.Instance.Pvp_BlmPartnerWeight;

                case ClassJobType.Dragoon:
                case ClassJobType.Lancer:
                    return DancerSettings.Instance.Pvp_DrgPartnerWeight;

                case ClassJobType.Samurai:
                    return DancerSettings.Instance.Pvp_SamPartnerWeight;

                case ClassJobType.Machinist:
                    return DancerSettings.Instance.Pvp_MchPartnerWeight;

                case ClassJobType.Summoner:
                case ClassJobType.Arcanist:
                    return DancerSettings.Instance.Pvp_SmnPartnerWeight;

                case ClassJobType.Bard:
                case ClassJobType.Archer:
                    return DancerSettings.Instance.Pvp_BrdPartnerWeight;

                case ClassJobType.Ninja:
                case ClassJobType.Rogue:
                    return DancerSettings.Instance.Pvp_NinPartnerWeight;

                case ClassJobType.RedMage:
                    return DancerSettings.Instance.Pvp_RdmPartnerWeight;

                case ClassJobType.Dancer:
                    return DancerSettings.Instance.Pvp_DncPartnerWeight;

                case ClassJobType.Paladin:
                case ClassJobType.Gladiator:
                    return DancerSettings.Instance.Pvp_PldPartnerWeight;

                case ClassJobType.Warrior:
                case ClassJobType.Marauder:
                    return DancerSettings.Instance.Pvp_WarPartnerWeight;

                case ClassJobType.DarkKnight:
                    return DancerSettings.Instance.Pvp_DrkPartnerWeight;

                case ClassJobType.Gunbreaker:
                    return DancerSettings.Instance.Pvp_GnbPartnerWeight;

                case ClassJobType.WhiteMage:
                case ClassJobType.Conjurer:
                    return DancerSettings.Instance.Pvp_WhmPartnerWeight;

                case ClassJobType.Scholar:
                    return DancerSettings.Instance.Pvp_SchPartnerWeight;
            }

            return c.CurrentJob == ClassJobType.Adventurer ? 70 : 0;
        }
    }
}
