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
using Magitek.Logic.Roles;
using Magitek.Models.WhiteMage;

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
            
            var Arcana = ActionResourceManager.CostTypesStruct.offset_D;
            var cardDrawn = Arcana != (byte)ActionResourceManager.Astrologian.AstrologianCard.None
                && Arcana != (byte)ActionResourceManager.Astrologian.AstrologianCard.LordofCrowns
                && Arcana != (byte)ActionResourceManager.Astrologian.AstrologianCard.LadyofCrowns;

            if (!cardDrawn && AstrologianSettings.Instance.UseDraw)
                if (await Spells.Draw.Cast(Core.Me))
                    return await Coroutine.Wait(750, () => Arcana != (byte)ActionResourceManager.Astrologian.AstrologianCard.None);

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20 + Core.Me.CurrentTarget.CombatReach);
            }

            if (Globals.InParty && Utilities.Combat.Enemies.Count > AstrologianSettings.Instance.StopDamageWhenMoreThanEnemies)
                return false;

            if (!AstrologianSettings.Instance.DoDamage)
                return false;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            return await Combat();
        }

        public static async Task<bool> Heal()
        {

            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

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

            //LimitBreak
            if (Heals.ForceLimitBreak()) return true;

            if (await Heals.Ascend()) return true;
            if (await Dispel.Execute()) return true;
    
            if (AstrologianSettings.Instance.WeaveOGCDHeals && GlobalCooldown.CanWeave(1))
            {
                if (await Buff.Divination()) return true;
                if (await Buff.LucidDreaming()) return true;
                if (await Buff.Lightspeed()) return true;
                if (await Buff.NeutralSect()) return true;
                if (await Cards.AstroDyne()) return true;
                if (await Cards.RedrawOrDrawAgain(Cards.GetDrawnCard())) return true;
                if (await Cards.PlayCards()) return true;
            }

            if (Globals.InActiveDuty || Core.Me.InCombat)
            {
                if (AstrologianSettings.Instance.WeaveOGCDHeals && GlobalCooldown.CanWeave(1))
                {
                    if (await Heals.Macrocosmos()) return true;
                    if (await Heals.EarthlyStar()) return true;
                    if (await Heals.CollectiveUnconscious()) return true;
                    if (await Heals.LadyOfCrowns()) return true;
                    if (await Heals.CelestialOpposition()) return true;
                    if (await Heals.HoroscopePop()) return true;
                    if (await Buff.Synastry()) return true;
                    if (await Heals.EssentialDignity()) return true;
                    if (await Heals.CelestialIntersection()) return true;
                    if (await Heals.Horoscope()) return true;
                    if (await Heals.Exaltation()) return true;
                    if (await Cards.AstroDyne()) return true;
                    if (await Aoe.LordOfCrown()) return true;
                    if (await Cards.RedrawOrDrawAgain(Cards.GetDrawnCard())) return true;
                    if (await Cards.PlayCards()) return true;
                }
                
                if (await Heals.AspectedHelios()) return true;
                if (await Heals.Helios()) return true;
                if (await Heals.AspectedBenefic()) return true;
                if (await Heals.Benefic2()) return true;
                if (await Heals.Benefic()) return true;              
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
            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            if (AstrologianSettings.Instance.WeaveOGCDHeals && GlobalCooldown.CanWeave(1)) 
                
            {
                
                if (await Buff.Divination()) return true;
                if (await Buff.LucidDreaming()) return true;
                if (await Buff.Lightspeed()) return true;
                if (await Buff.NeutralSect()) return true;
                if (await Cards.AstroDyne()) return true;
                if (await Cards.RedrawOrDrawAgain(Cards.GetDrawnCard())) return true;
                if (await Cards.PlayCards()) return true;


            }

            if (Globals.InActiveDuty || Core.Me.InCombat)
            {
                if (AstrologianSettings.Instance.WeaveOGCDHeals && GlobalCooldown.CanWeave(1))                
                {
                    if (await Heals.Macrocosmos()) return true;
                    if (await Heals.EarthlyStar()) return true;
                    if (await Heals.CollectiveUnconscious()) return true;
                    if (await Heals.LadyOfCrowns()) return true;
                    if (await Heals.CelestialOpposition()) return true;
                    if (await Heals.HoroscopePop()) return true;
                    if (await Heals.Horoscope()) return true;
                    if (await Buff.Synastry()) return true;
                    if (await Heals.EssentialDignity()) return true;
                    if (await Heals.CelestialIntersection()) return true;
                    if (await Heals.Exaltation()) return true;
                    if (await Cards.AstroDyne()) return true;
                    if (await Aoe.LordOfCrown()) return true;
                    if (await Cards.RedrawOrDrawAgain(Cards.GetDrawnCard())) return true;
                    if (await Cards.PlayCards()) return true;
                }
                
            }
            return false;
        }

        public static async Task<bool> Combat()
        {
            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            await CombatBuff();

            if (Globals.InParty)
            {
                if (Utilities.Combat.Enemies.Count > AstrologianSettings.Instance.StopDamageWhenMoreThanEnemies)
                    return false;

                if (Core.Me.CurrentManaPercent < AstrologianSettings.Instance.MinimumManaPercentToDoDamage
                    && Core.Target.CombatTimeLeft() > AstrologianSettings.Instance.DoDamageIfTimeLeftLessThan)
                    return false;
            }

            if (!AstrologianSettings.Instance.DoDamage)
                return false;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20 + Core.Me.CurrentTarget.CombatReach);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await Aoe.AggroAst()) return true;
            //if (await Aoe.LordOfCrown()) return true;
            if (await Aoe.Gravity()) return true;
            if (await SingleTarget.Combust()) return true;
            if (await SingleTarget.CombustMultipleTargets()) return true;
            return await SingleTarget.Malefic();
        }

        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            if (await Healer.Guard(AstrologianSettings.Instance)) return true;
            if (await Healer.Purify(AstrologianSettings.Instance)) return true;
            if (await Healer.Recuperate(AstrologianSettings.Instance)) return true;

            if (await Pvp.CelestialRiverPvp()) return true;

            if (await Pvp.MacrocosmosPvp()) return true;
            if (await Pvp.MicrocosmosPvp()) return true;

            if (await Pvp.DrawPvp()) return true;
            if (await Pvp.AspectedBeneficPvp()) return true;
            if (await Pvp.DoubleAspectedBeneficPvp()) return true;

            if (!Healer.GuardCheck())
            {
                if (await Pvp.DoubleGravityIIPvp()) return true;
                if (await Pvp.DoubleFallMaleficPvp()) return true;
                if (await Pvp.GravityIIPvp()) return true;
            }

            return (await Pvp.FallMaleficPvp());
        }
    }
}
