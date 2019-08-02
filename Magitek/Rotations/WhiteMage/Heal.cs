using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Utilities;
using Magitek.Logic.WhiteMage;

namespace Magitek.Rotations.WhiteMage
{
    internal static class Heal
    {
        public static async Task<bool> Execute()
        {
            Group.UpdateAllies(Utilities.Routines.WhiteMage.GroupExtension);
            Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
            
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            Globals.InParty = PartyManager.IsInParty || Globals.InGcInstance; 
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2) || Core.Me.InCombat;
            Globals.OnPvpMap = Core.Me.OnPvpMap();

            if (Core.Me.OnPvpMap())
            {
                await PvpRotation();
                return true;
            }

            if (Duty.State() == Duty.States.Ended)
                return false;

            if (await GambitLogic.Gambit()) return true;

            if (await Chocobo.HandleChocobo()) return true;

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

        public static async Task<bool> PvpRotation()
        {
            if (await Pvp.Benediction()) return true;
            if (await Pvp.FluidAura()) return true;
            if (await Pvp.Safeguard()) return true;
            if (await Pvp.Muse()) return true;
            if (await Pvp.DivineBenison()) return true;
            if (await Pvp.Assize()) return true;
            if (await Pvp.Cure2()) return true;
            if (await Pvp.Cure()) return true;
            if (await Pvp.Regen()) return true;
            return await Pvp.Stone3();
        }
    }
}
