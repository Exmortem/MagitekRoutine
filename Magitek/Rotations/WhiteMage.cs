using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Roles;
using Magitek.Logic.WhiteMage;
using Magitek.Models.Account;
using Magitek.Models.Summoner;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Magitek.Rotations
{
    public static class WhiteMage
    {
        public static async Task<bool> Rest()
        {
            if (WhiteMageSettings.Instance.CureOnRest)
            {
                if (Core.Me.CurrentHealthPercent > 70 || Core.Me.ClassLevel < 2)
                    return false;

                return await Spells.Cure.Heal(Core.Me);
            }

            return false;
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (Core.Me.IsCasting)
                return true;

            await Casting.CheckForSuccessfulCast();

            if (WorldManager.InSanctuary)
                return false;

            if (Core.Me.IsMounted)
                return false;

            return false;
        }

        public static async Task<bool> Pull()
        {
            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20 + Core.Me.CurrentTarget.CombatReach);
            }

            if (Globals.InParty && Utilities.Combat.Enemies.Count > WhiteMageSettings.Instance.StopDamageWhenMoreThanEnemies)
                return false;

            if (!WhiteMageSettings.Instance.DoDamage)
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

            if (await Casting.TrackSpellCast()) 
                return true;
            
            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            if (await GambitLogic.Gambit()) 
                return true;

            //LimitBreak
            if (Logic.WhiteMage.Heal.ForceLimitBreak()) return true;

            //force cast logics
            if (await Logic.WhiteMage.Heal.ForceRegen()) return true;
            if (await Logic.WhiteMage.Heal.ForceBenediction()) return true;
            if (await Logic.WhiteMage.Heal.ForceMedica()) return true;
            if (await Logic.WhiteMage.Heal.ForceMedicaII()) return true;
            if (await Logic.WhiteMage.Heal.ForceAfflatusSolace()) return true;
            if (await Logic.WhiteMage.Heal.ForceAfflatusRapture()) return true;
            if (await Logic.WhiteMage.Heal.ForceCureII()) return true;
            if (await Logic.WhiteMage.Heal.ForceCureIII()) return true;
            if (await Logic.WhiteMage.Heal.ForceTetra()) return true;
            if (await SingleTarget.ForceAfflatusMisery()) return true;
            if (await Logic.WhiteMage.Heal.Raise()) return true;

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
            {
                if (await Dispel.Execute()) 
                    return true;
                
                return false;
            }

            if (Casting.LastSpell == Spells.PlenaryIndulgence)
            {
                if (await Logic.WhiteMage.Heal.AfflatusRapture()) return true;
                if (await Logic.WhiteMage.Heal.Cure3()) return true;
                if (await Logic.WhiteMage.Heal.Medica2()) return true;
                return await Spells.Medica.Cast(Core.Me);
            }

            if (await Logic.WhiteMage.Heal.Benediction()) return true;
            if (await Dispel.Execute()) return true;
            if (await Buff.Temperance()) return true;
            if (await Buff.ThinAir(false)) return true;
            if (await Buff.DivineBenison()) return true;
            if (await Logic.WhiteMage.Heal.PlenaryIndulgence()) return true;
            if (await Buff.LucidDreaming()) return true;
            if (await Buff.AssizeForMana()) return true;
            if (await Buff.PresenceOfMind()) return true;
            if (await Buff.Aquaveil()) return true;

            if (await Logic.WhiteMage.Heal.Benediction()) return true;
            if (await Logic.WhiteMage.Heal.LiturgyOfTheBell()) return true;

            if (Globals.InParty)
            {
                // if (await Logic.WhiteMage.Heal.AssizeHeal()) return true;
                if (await Logic.WhiteMage.Heal.AfflatusRapture()) return true;
                if (await Logic.WhiteMage.Heal.Cure3()) return true;
                if (await Logic.WhiteMage.Heal.Asylum()) return true;
                if (await Logic.WhiteMage.Heal.Medica2()) return true;
                if (await Logic.WhiteMage.Heal.Medica()) return true;
            }

            if (await Logic.WhiteMage.Heal.Tetragrammaton()) return true;
            if (await Logic.WhiteMage.Heal.AfflatusSolace()) return true;
            if (await Logic.WhiteMage.Heal.Cure2()) return true;
            if (await Logic.WhiteMage.Heal.Cure()) return true;
            if (await Logic.WhiteMage.Heal.Regen()) return true;

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
                if (await Logic.WhiteMage.Heal.Raise()) return true;

                if (WhiteMageSettings.Instance.HealAllianceOnlyCure)
                {
                    if (await Logic.WhiteMage.Heal.Cure()) return true;
                    return false;
                }

                if (await Logic.WhiteMage.Heal.Benediction()) return true;
                if (await Logic.WhiteMage.Heal.Tetragrammaton()) return true;
                if (await Logic.WhiteMage.Heal.AfflatusSolace()) return true;
                if (await Logic.WhiteMage.Heal.Cure2()) return true;
                if (await Logic.WhiteMage.Heal.Cure()) return true;
                if (await Logic.WhiteMage.Heal.Regen()) return true;

                return false;
            }
        }

        public static Task<bool> CombatBuff()
        {
            return Task.FromResult(false);
        }

        public static async Task<bool> Combat()
        {
            if (BaseSettings.Instance.ActivePvpCombatRoutine)
                return await PvP();

            if (Globals.InParty)
            {
                if (Utilities.Combat.Enemies.Count > WhiteMageSettings.Instance.StopDamageWhenMoreThanEnemies)
                    return false;

                if (Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.MinimumManaPercentToDoDamage && !Core.Me.HasAura(Auras.ThinAir))
                    return false;
            }

            if (!WhiteMageSettings.Instance.DoDamage)
                return false;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20 + Core.Me.CurrentTarget.CombatReach);
            }

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await SingleTarget.AfflatusMisery()) return true;
            if (await Aoe.Holy()) return true;
            if (await Aoe.AssizeDamage()) return true;

            

            if (await SingleTarget.Dots()) return true;
            if (await SingleTarget.DotMultipleTargets()) return true;
            return await SingleTarget.Stone();
        }

        public static async Task<bool> PvP()
        {
            if (!BaseSettings.Instance.ActivePvpCombatRoutine)
                return await Combat();

            if (await Healer.Guard(WhiteMageSettings.Instance)) return true;
            if (await Healer.Purify(WhiteMageSettings.Instance)) return true;
            if (await Healer.Recuperate(WhiteMageSettings.Instance)) return true;

            if (await Pvp.AfflatusPurgationPvp()) return true;

            if (await Pvp.CureIIIPvp()) return true;
            if (await Pvp.AquaveilPvp()) return true;
            if (await Pvp.CureIIPvp()) return true;

            if (!Healer.GuardCheck())
            {
                if (await Pvp.AfflatusMiseryPvp()) return true;
                if (await Pvp.MiracleOfNaturePvp()) return true;
            }

            return (await Pvp.GlareIIIPvp());
        }
    }
}
