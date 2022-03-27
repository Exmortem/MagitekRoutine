using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Astrologian;
using Magitek.Models.Astrologian;
using Magitek.Utilities;
using System.Threading.Tasks;
using Magitek.Models.Account;
using System.Linq;
using System.Windows.Controls;
using static Magitek.Utilities.Routines.Astrologian;

namespace Magitek.Rotations
{
    public static class Astrologian
    {
        public static Task<bool> Rest()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (WorldManager.InSanctuary)
                return false;

            if (Core.Me.IsMounted)
                return false;

            if (Core.Me.IsCasting)
                return true;

            await Casting.CheckForSuccessfulCast();

            if (Globals.OnPvpMap)
                return false;

            var cardDrawn = ActionResourceManager.Astrologian.Arcana != ActionResourceManager.Astrologian.AstrologianCard.None
                && ActionResourceManager.Astrologian.Arcana != ActionResourceManager.Astrologian.AstrologianCard.LordofCrowns
                && ActionResourceManager.Astrologian.Arcana != ActionResourceManager.Astrologian.AstrologianCard.LadyofCrowns;

            if (!cardDrawn && AstrologianSettings.Instance.UseDraw)
                if (await Spells.Draw.Cast(Core.Me))
                    return await Coroutine.Wait(750, () => ActionResourceManager.Astrologian.Arcana != ActionResourceManager.Astrologian.AstrologianCard.None);

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }
            else
            {
                if (Globals.InParty)
                {
                    if (!AstrologianSettings.Instance.DoDamage)
                        return false;
                }
            }

            if (Core.Me.InCombat)
                return false;

            return await SingleTarget.Malefic();
        }

        public static async Task<bool> Heal()
        {
            if (WorldManager.InSanctuary)
                return false;

            if (Core.Me.IsMounted)
                return false;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            if (await GambitLogic.Gambit())
                return true;

            if (await Heals.Ascend()) return true;
            if (await Dispel.Execute()) return true;

            if ((AstrologianSettings.Instance.WeaveOGCDHeals && GlobalCooldown.CanWeave(1)) || Casting.LastSpellTimeFinishAge.ElapsedMilliseconds > Spells.Malefic.AdjustedCooldown.TotalMilliseconds || !Casting.LastSpellTimeFinishAge.IsRunning)
            {
                if (await Heals.EssentialDignity()) return true;
                if (await Buff.LucidDreaming()) return true;
                if (await Buff.Lightspeed()) return true;
                if (await Buff.NeutralSect()) return true;
            }

            if (Globals.InActiveDuty || Core.Me.InCombat)
            {
                if ((AstrologianSettings.Instance.WeaveOGCDHeals && GlobalCooldown.CanWeave(1)) || Casting.LastSpellTimeFinishAge.ElapsedMilliseconds > Spells.Malefic.AdjustedCooldown.TotalMilliseconds || !Casting.LastSpellTimeFinishAge.IsRunning)
                {
                    if (await Heals.EssentialDignity()) return true;
                    if (await Heals.CelestialIntersection()) return true;
                    if (await Heals.Macrocosmos()) return true;
                    if (await Heals.CelestialOpposition()) return true;
                    if (await Heals.LadyOfCrowns()) return true;
                    if (await Heals.Horoscope()) return true;
                    if (await Heals.HoroscopePop()) return true;
                    if (await Heals.Exaltation()) return true;
                    if (await Heals.CollectiveUnconscious()) return true;
                    if (await Buff.Synastry()) return true;
                }
                
                if (await Heals.AspectedHelios()) return true;
                if (await Heals.Helios()) return true;
                if (await Heals.Benefic2()) return true;
                if (await Heals.Benefic()) return true;
                if (await Heals.AspectedBenefic()) return true;
                if (await Heals.EarthlyStar()) return true;
                if (await Heals.DontLetTheDrkDie()) return true;
            }

            return await HealAlliance();
        }

        public static async Task<bool> HealAlliance()
        {
            if (Group.CastableAlliance.Count == 0)
                return false;

            Group.SwitchCastableToAlliance();
            var res = await DoHeal();
            Group.SwitchCastableToParty();
            return res;

            async Task<bool> DoHeal()
            {
                if (await Heals.Ascend()) return true;

                if (AstrologianSettings.Instance.HealAllianceOnlyBenefic)
                {
                    return await Heals.Benefic();
                }

                if (await Heals.EssentialDignity()) return true;
                if (await Heals.Benefic2()) return true;
                if (await Heals.Benefic()) return true;
                return await Heals.AspectedBenefic();
            }
        }

        public static async Task<bool> CombatBuff()
        {
            //Added redundancy to make sure buffs go off
            if (await Buff.LucidDreaming()) return true;
            if (await Buff.Lightspeed()) return true;
            if (await Buff.Synastry()) return true;
            if (await Buff.NeutralSect()) return true;


            if (await Buff.Divination()) return true;
            if (await Cards.AstroDyne()) return true;
            return await Cards.PlayCards();
        }

        public static async Task<bool> Combat()
        {
            await CombatBuff();

            if (Globals.InParty)
            {
                if (!AstrologianSettings.Instance.DoDamage)
                    return false;

                if (Core.Me.CurrentManaPercent < AstrologianSettings.Instance.MinimumManaPercentToDoDamage
                    && Core.Target.CombatTimeLeft() > AstrologianSettings.Instance.DoDamageIfTimeLeftLessThan)
                    return false;
            }

            if (!GameSettingsManager.FaceTargetOnAction && !BaseSettings.Instance.AssumeFaceTargetOnAction)
            {
                if (!Core.Me.CurrentTarget.InView())
                    return false;

            }

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {

                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);

                }
            }

            if (!Core.Me.HasTarget
                || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (Globals.OnPvpMap)
            {
                if (await Pvp.Disable()) return true; //Damage
                return await Pvp.Malefic(); //Damage
            }

            if (await Aoe.AggroAst()) return true;
            if (await Aoe.LordOfCrown()) return true;
            if (await Aoe.Gravity()) return true;
            if (await SingleTarget.Combust()) return true;
            if (await SingleTarget.CombustMultipleTargets()) return true;
            return await SingleTarget.Malefic();
        }

        //public static async Task<bool> PvP()
        //{
        //    if (Globals.OnPvpMap)
        //    {
        //        if (await Pvp.EssentialDignity()) return true; //Heal
        //        if (await Pvp.Purify()) return true; //Dispel/Heal
        //        if (await Pvp.Muse()) return true; //Self-Buff
        //        if (await Pvp.Lightspeed()) return true; //CombatBuff
        //        if (await Pvp.Synastry()) return true; //Heal
        //        if (await Pvp.Deorbit()) return true; //Heal
        //        if (await Pvp.EmpyreanRain()) return true; //Heal
        //        if (await Pvp.Recuperate()) return true; //Self-Heal
        //        if (await Pvp.Benefic2()) return true; //Heal
        //        if (await Pvp.Benefic()) return true; //Heal
        //        if (await Pvp.Concentrate()) return true; //CombatBuff/Heal
        //        return await Pvp.Safeguard();
        //    }

        //    return false;
        //}
    }
}
