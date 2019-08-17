﻿using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Utilities;
using Magitek.Logic.Astrologian;

namespace Magitek.Rotations.Astrologian
{
    internal static class Heal
    {
        public static async Task<bool> Execute()
        {
            if (WorldManager.InSanctuary)
                return false;

            if (Core.Me.IsMounted)
                return false;

            if (Duty.State() == Duty.States.Ended)
                return false;

            if (await Chocobo.HandleChocobo()) return true;

            Group.UpdateAllies(Utilities.Routines.Astrologian.GroupExtension);
            Globals.HealTarget = Group.CastableAlliesWithin30.OrderBy(x => x.CurrentHealthPercent).FirstOrDefault();
            
            if (await Casting.TrackSpellCast()) return true;
            await Casting.CheckForSuccessfulCast();

            Casting.DoHealthChecks = false;

            Globals.InParty = PartyManager.IsInParty || Globals.InGcInstance;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2) || Core.Me.InCombat;
            Globals.OnPvpMap = Core.Me.OnPvpMap();

            if (await GambitLogic.Gambit()) return true;

            #region PvP
            if (Globals.OnPvpMap)
            {
                if (await Pvp.EssentialDignity()) return true; //Heal
                if (await Pvp.Purify()) return true; //Dispel/Heal
                if (await Pvp.Muse()) return true; //Self-Buff
                if (await Pvp.Lightspeed()) return true; //CombatBuff
                if (await Pvp.Synastry()) return true; //Heal
                if (await Pvp.Deorbit()) return true; //Heal
                if (await Pvp.EmpyreanRain()) return true; //Heal
                if (await Pvp.Recuperate()) return true; //Self-Heal
                if (await Pvp.Benefic2()) return true; //Heal
                if (await Pvp.Benefic()) return true; //Heal
                if (await Pvp.Concentrate()) return true; //CombatBuff/Heal
                return await Pvp.Safeguard();
            }
            #endregion

            if (Globals.PartyInCombat && Globals.InParty)
            {
                if (await TankBusters.Execute()) return true;
            }
            
            if (await Logic.Astrologian.Heal.Ascend()) return true;
            if (await Logic.Astrologian.Heal.EssentialDignity()) return true;
            if (await Dispel.Execute()) return true;
            if (await Buff.LucidDreaming()) return true;
            if (await Buff.Lightspeed()) return true;
            if (await Buff.Synastry()) return true;
            if (await Buff.NeutralSect()) return true;

            if (DutyManager.InInstance || Core.Me.InCombat)
            {
                if (Globals.InParty)
                {
                    if (await Logic.Astrologian.Heal.EssentialDignity()) return true;
                    if (await Logic.Astrologian.Heal.CelestialIntersection()) return true;
                    if (await Logic.Astrologian.Heal.CelestialOpposition()) return true;
                    if (await Logic.Astrologian.Heal.Horoscope()) return true;
                    if (await Logic.Astrologian.Heal.HoroscopePop()) return true;
                    if (await Logic.Astrologian.Heal.AspectedHelios()) return true;
                    if (await Logic.Astrologian.Heal.CollectiveUnconscious()) return true;
                    if (await Logic.Astrologian.Heal.Helios()) return true;
                    if (await Logic.Astrologian.Heal.Benefic2()) return true;
                }

                if (await Logic.Astrologian.Heal.Benefic()) return true;
                if (await Logic.Astrologian.Heal.AspectedBenefic()) return true;
                if (await Logic.Astrologian.Heal.EarthlyStar()) return true;
            }

            return await Logic.Astrologian.Heal.Benefic();
        }
    }
}
