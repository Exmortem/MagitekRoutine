using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Bard
    {
        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;
        public static List<DateTime> DoTProcEvents = new List<DateTime>();
        public static int OldSoulVoice;
        public static bool AlreadySnapped = false;


        public static SpellData LadonsBite => Core.Me.ClassLevel < 82
                                            ? Spells.QuickNock
                                            : Spells.Ladonsbite;

        public static void RefreshVars()
        {
            CheckForDoTProcs();
            CleanUpDoTProcList();

            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(15);
            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();

            if (Casting.LastSpell == Spells.IronJaws && Core.Me.HasAura(Auras.RagingStrikes))
            {
                AlreadySnapped = true;
            }
            AlreadySnapped = AlreadySnapped && Core.Me.HasAura(Auras.RagingStrikes);


        }

        public static uint Windbite => (uint)(Core.Me.ClassLevel < 64 ? Auras.Windbite : Auras.StormBite);
        public static uint VenomousBite => (uint)(Core.Me.ClassLevel < 64 ? Auras.VenomousBite : Auras.CausticBite);

        public static List<uint> DotsList => Core.Me.ClassLevel >= 64 ?
            new List<uint>() { Auras.StormBite, Auras.CausticBite } :
            new List<uint>() { Auras.Windbite, Auras.VenomousBite };

        public static void CleanUpDoTProcList()
        {
            //If the proc event is older than 45s then remove it, its safe to assume the dot dropped or target died
            //The chances to get at least one dot proc within 30s are 99.4~% ( 1 dot 10 ticks )
            foreach (var _TickTime in DoTProcEvents.Reverse<DateTime>())
            {
                if (!(DateTime.Now.Subtract(_TickTime).TotalMilliseconds >= 45000)) continue;
                DoTProcEvents.Remove(_TickTime);
            }

            //The More Enemies the more Proc data we need
            if (DoTProcEvents.Count > Combat.Enemies.Count)
                DoTProcEvents.Remove(DoTProcEvents.Last());
        }


        public static void CheckForDoTProcs()
        {
            //Doenst matter if something procs here
            if (Combat.CombatTime.ElapsedMilliseconds < 2800)
            {
                OldSoulVoice = ActionResourceManager.Bard.SoulVoice;
                DoTProcEvents.Clear();
                return;
            }

            if (OldSoulVoice == ActionResourceManager.Bard.SoulVoice)
                return;

            OldSoulVoice = ActionResourceManager.Bard.SoulVoice;

            if (Casting.LastSpell == Spells.EmpyrealArrow || Casting.LastSpell == Spells.ApexArrow || Casting.LastSpell == Spells.BlastArrow)
                return;

            DateTime newProcTime = DateTime.Now;

            DoTProcEvents.Insert(0, newProcTime);

        }

        public static double TimeUntilNextPossibleDoTTick()
        {
            double potentialTickInXms = 999999;

            foreach (var dotTickTime in DoTProcEvents)
            {
                double _tmpTime = DateTime.Now.Subtract(dotTickTime).TotalMilliseconds;
                if (potentialTickInXms > _tmpTime % 3000)
                    potentialTickInXms = _tmpTime % 3000;
            }

            if (potentialTickInXms != 999999)
                return 3000 - potentialTickInXms; // DateTime.Now.Subtract(dotTickTime).TotalMilliseconds % 3000 = INVERTED TIME
            return 0;

        }

        public static bool CheckCurrentDamageIncrease(int _neededDmgIncrease)
        {
            double _dmgIncrease = 1;
            bool _isEndingSoon = false;

            //This should be changed into some IDs
            List<string> fivePercentBuffs = new List<string>();
            fivePercentBuffs.Add("Technical Finish");
            fivePercentBuffs.Add("LeftEye");
            fivePercentBuffs.Add("Devotion");
            fivePercentBuffs.Add("Divination");
            fivePercentBuffs.Add("Brotherhood");

            List<string> astLowCards = new List<string>();
            astLowCards.Add("The Balance");
            astLowCards.Add("The Arrow");
            astLowCards.Add("The Spear");

            List<string> astHighCards = new List<string>();
            astLowCards.Add("The Bole");
            astLowCards.Add("The Ewer");
            astLowCards.Add("The Spire");

            foreach (var _auraCache in Core.Me.Auras)
            {
                if (fivePercentBuffs.Contains(_auraCache.Name))
                {
                    _dmgIncrease *= 1.05;
                    if (_auraCache.TimespanLeft.TotalMilliseconds < 3000)
                        _isEndingSoon = true;
                }

                if (astLowCards.Contains(_auraCache.Name))
                {
                    _dmgIncrease *= 1.03;
                    if (_auraCache.TimespanLeft.TotalMilliseconds < 3000)
                        _isEndingSoon = true;
                }

                if (astHighCards.Contains(_auraCache.Name))
                {
                    _dmgIncrease *= 1.06;
                    if (_auraCache.TimespanLeft.TotalMilliseconds < 3000)
                        _isEndingSoon = true;
                }

                if (_auraCache.Name == "Lord of Crowns")
                {
                    _dmgIncrease *= 1.04;
                    if (_auraCache.TimespanLeft.TotalMilliseconds < 3000)
                        _isEndingSoon = true;
                }

                if (_auraCache.Name == "Lady of Crowns")
                {
                    _dmgIncrease *= 1.08;
                    if (_auraCache.TimespanLeft.TotalMilliseconds < 3000)
                        _isEndingSoon = true;
                }

                if (_auraCache.Name == "Raging Strikes")
                {
                    _dmgIncrease *= 1.1;
                    if (_auraCache.TimespanLeft.TotalMilliseconds < 3000)
                        _isEndingSoon = true;
                }

                if (_auraCache.Name == "Radiant Finale")
                {
                    _dmgIncrease *= 1.06;
                    if (_auraCache.TimespanLeft.TotalMilliseconds < 3000)
                        _isEndingSoon = true;
                }

                if (_auraCache.Name == "Embolden")
                {
                    if (_auraCache.TimespanLeft.TotalMilliseconds > 16000)
                        _dmgIncrease *= 1.1;
                    if (_auraCache.TimespanLeft.TotalMilliseconds > 12000)
                        _dmgIncrease *= 1.08;
                    if (_auraCache.TimespanLeft.TotalMilliseconds > 8000)
                        _dmgIncrease *= 1.06;
                    if (_auraCache.TimespanLeft.TotalMilliseconds >= 4000)
                        _dmgIncrease *= 1.04;
                    if (_auraCache.TimespanLeft.TotalMilliseconds < 4000)
                    {
                        _dmgIncrease *= 1.1;
                        _isEndingSoon = true;
                    }
                }
            }

            //Trick Attack Check
            if (Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack))
            {
                _dmgIncrease *= 1.1;
                if (Core.Me.CurrentTarget.HasAura(Auras.VulnerabilityTrickAttack, false, 3000))
                    _isEndingSoon = true;
            }

            return _dmgIncrease >= (1 + (double)_neededDmgIncrease / 100) && _isEndingSoon;
        }
    }
}