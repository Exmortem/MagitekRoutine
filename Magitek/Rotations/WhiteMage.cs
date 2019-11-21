using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.WhiteMage;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;

namespace Magitek.Rotations
{
    public static class WhiteMage
    {
        public static async Task<bool> Rest()
        {
            if (Core.Me.CurrentHealthPercent > 70 || Core.Me.ClassLevel < 2)
                return false;

            await Spells.Cure.Heal(Core.Me);
            return true;
        }

        public static async Task<bool> PreCombatBuff()
        {
            if (Core.Me.IsCasting)
                return true;

            await Casting.CheckForSuccessfulCast();

            if (Core.Me.IsMounted)
                return false;

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
                if (!WhiteMageSettings.Instance.DoDamage)
                    return false;
            }

            if (Core.Me.InCombat)
                return false;

            return await Combat();
        }

        public static async Task<bool> Heal()
        {
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            if (await GambitLogic.Gambit()) return true;
            
            if (Globals.PartyInCombat && Globals.InParty)
            {
                if (await TankBusters.Execute()) return true;
            }

            if (await Logic.WhiteMage.Heal.Raise()) return true;

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
            {
                if (await Dispel.Execute()) return true;
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
            if (await Logic.WhiteMage.Heal.Tetragrammaton()) return true;
            if (await Logic.WhiteMage.Heal.AfflatusSolace()) return true;
            if (await Logic.WhiteMage.Heal.Cure3()) return true;

            if (Globals.InParty)
            {
                if (await Logic.WhiteMage.Heal.AssizeHeal()) return true;
                if (await Logic.WhiteMage.Heal.Benediction()) return true;
                if (await Logic.WhiteMage.Heal.Tetragrammaton()) return true;
                if (await Logic.WhiteMage.Heal.AfflatusRapture()) return true;
                if (await Logic.WhiteMage.Heal.Medica()) return true;
                if (await Logic.WhiteMage.Heal.Medica2()) return true;
                if (await Logic.WhiteMage.Heal.Cure3()) return true;
                if (await Logic.WhiteMage.Heal.Asylum()) return true;
            }

            if (await Logic.WhiteMage.Heal.Cure2()) return true;
            if (await Logic.WhiteMage.Heal.Cure()) return true;
            if (await SingleTarget.FluidAura()) return true;
            return await Logic.WhiteMage.Heal.Regen();
        }

        public static async Task<bool> CombatBuff()
        {
            return false;
        }

        public static async Task<bool> Combat()
        {
            if (Utilities.Combat.Enemies.Count > WhiteMageSettings.Instance.StopDamageWhenMoreThanEnemies)
                return false;

            if (Globals.InParty)
            {
                if (!WhiteMageSettings.Instance.DoDamage)
                    return true;

                if (Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.MinimumManaPercentToDoDamage && !Core.Me.HasAura(Auras.ThinAir))
                    return true;
            }

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if (await Aoe.Holy()) return true;
            if (await Aoe.AssizeDamage()) return true;

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await SingleTarget.AfflatusMisery()) return true;
            if (await SingleTarget.Dots()) return true;
            return await SingleTarget.Stone();
        }

    }
}
