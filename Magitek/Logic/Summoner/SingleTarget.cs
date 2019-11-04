using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Summoner;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Summoner
{
    internal static class SingleTarget
    {
        public static async Task<bool> Ruin()
        {
            if (!SummonerSettings.Instance.Ruin) return false;

            if ((int)PetManager.ActivePetType == 10)
                return await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget);

            if (Core.Me.ClassLevel >= 38 && MovementManager.IsMoving && !ActionResourceManager.Summoner.DreadwyrmTrance)
                return await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget);

            return await Spells.SmnRuin.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Ruin4()
        {
            if (Core.Me.ClassLevel < 62) return false;

            if (!SummonerSettings.Instance.Ruin4) return false;

            if (!Core.Me.HasAura(Auras.FurtherRuin)) return false;

            return await Spells.Ruin4.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Ruin4MaxStacks()
        {
            if (Core.Me.ClassLevel < 62) return false;

            if (!SummonerSettings.Instance.Ruin4) return false;

            if (Core.Me.CharacterAuras.GetAuraStacksById(Auras.FurtherRuin) < 4) return false;

            return await Spells.Ruin4.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Bio()
        {
            if (!SummonerSettings.Instance.Bio) return false;

            if (Spells.TriDisaster.Cooldown.TotalMilliseconds <= SummonerSettings.Instance.DotRefreshSeconds * 1000 && Core.Me.ClassLevel > 53)
                return false;

            return !Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, SummonerSettings.Instance.DotRefreshSeconds * 1000)
                   && await Spells.SmnBio.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Miasma()
        {
            if (!SummonerSettings.Instance.Miasma) return false;

            if (MovementManager.IsMoving) return false;
            var refresh = SummonerSettings.Instance.DotRefreshSeconds * 1000;

            if (Spells.TriDisaster.Cooldown.TotalMilliseconds <= refresh && Core.Me.ClassLevel > 53)
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
            if (Spells.EgiAssault.Cooldown.TotalMilliseconds > 1)
                return false;

            if (!SummonerSettings.Instance.EgiAssault1) return false;

            if ((int)PetManager.ActivePetType == 10) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;

            return await Spells.EgiAssault.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Fester()
        {
            if (Core.Me.ClassLevel < 18) return false;
            if (Spells.Fester.Cooldown.TotalMilliseconds > 1)
                return false;

            if (!SummonerSettings.Instance.Fester) return false;

            if (ActionResourceManager.Arcanist.Aetherflow == 0) return false;

            if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, 2000) && Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.MiasmaAuras, true, 2000))
                return await Spells.Fester.Cast(Core.Me.CurrentTarget);
          
            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return false;
        }

        public static async Task<bool> EnergyDrain()
        {
            if (Core.Me.ClassLevel < 18) return false;
            if (Spells.EnergyDrain.Cooldown.TotalMilliseconds > 1)
                return false;
            if (!SummonerSettings.Instance.EnergyDrain) return false;

            if (ActionResourceManager.Arcanist.Aetherflow > 0) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.EnergyDrain.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EgiAssault2()
        {
            if (Spells.EgiAssault2.Cooldown.TotalMilliseconds > 1)
                return false;

            if (!SummonerSettings.Instance.EgiAssault2) return false;

            if ((int)PetManager.ActivePetType == 10) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.EgiAssault2.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Enkindle()
        {
            if (Core.Me.ClassLevel < 50) return false;
            if (Spells.Enkindle.Cooldown.TotalMilliseconds > 1)
                return false;
            if (!SummonerSettings.Instance.Enkindle) return false;

            if ((int)PetManager.ActivePetType == 10 || (int)PetManager.ActivePetType == 14) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.Enkindle.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> TriDisaster()
        {
            if (Core.Me.ClassLevel < 56) return false;
            if (Spells.TriDisaster.Cooldown.TotalMilliseconds > 1)
                return false;
            if (!SummonerSettings.Instance.TriDisaster) return false;

            if (!ActionResourceManager.Summoner.DreadwyrmTrance && Spells.Trance.Cooldown.TotalMilliseconds > 0)
            {
                if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.BioAuras, true, 3000)) return false;
                if (Core.Me.CurrentTarget.HasAnyAura(Utilities.Routines.Summoner.MiasmaAuras, true, 3000)) return false;

                return await Spells.TriDisaster.Cast(Core.Me.CurrentTarget);
            }

            if (Core.Me.CurrentTarget.HasAura(Auras.Ruination, true)) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.TriDisaster.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> Deathflare()
        {
            if (Core.Me.ClassLevel < 60) return false;
            if (Spells.Deathflare.Cooldown.TotalMilliseconds > 1)
                return false;
            if (!SummonerSettings.Instance.Deathflare) return false;

            if (!ActionResourceManager.Summoner.DreadwyrmTrance) return false;

            if (ActionResourceManager.Summoner.Timer.TotalMilliseconds > 1000) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.Deathflare.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> EnkindleBahamut()
        {
            if (Core.Me.ClassLevel < 70) return false;
            if (Spells.EnkindleBahamut.Cooldown.TotalMilliseconds > 1)
                return false;
            if ((int)PetManager.ActivePetType != 10 && (int)PetManager.ActivePetType != 14) return false;

            if ((int)PetManager.ActivePetType == 10 && !SummonerSettings.Instance.EnkindleBahamut) return false;

            if ((int)PetManager.ActivePetType == 14 && !SummonerSettings.Instance.EnkindlePhoenix) return false;

            if (ActionResourceManager.Summoner.Timer.TotalMilliseconds > 18000) return false;

            if (Casting.LastSpell != Spells.Bio || Casting.LastSpell != Spells.Ruin2)
                if (!ActionResourceManager.Summoner.DreadwyrmTrance)
                    if (await Spells.SmnRuin2.Cast(Core.Me.CurrentTarget))
                        return true;
            return await Spells.EnkindleBahamut.Cast(Core.Me.CurrentTarget);
        }
    }
}