using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;

namespace Magitek.Utilities.Routines
{
    internal static class Bard
    {
        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;
        public static int SnapShotCheck = 0;

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(15);
            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();
        }
        
        public static List<uint> DotsList => Core.Me.ClassLevel >= 64 ?
            new List<uint>() { Auras.StormBite, Auras.CausticBite } :
            new List<uint>() { Auras.Windbite, Auras.VenomousBite };

        public static List<SpellData> OGCDSpells = new List<SpellData>() {Spells.RagingStrikes, Spells.Barrage, Spells.BattleVoice,
                                                                            Spells.PitchPerfect, Spells.Bloodletter, Spells.EmpyrealArrow, 
                                                                            Spells.RainofDeath, Spells.Shadowbite, Spells.Sidewinder,
                                                                            Spells.TheWanderersMinuet, Spells.MagesBallad, Spells.ArmysPaeon,
                                                                            Spells.Troubadour, Spells.NaturesMinne, Spells.TheWardensPaean,
                                                                            Spells.HeadGraze, Spells.SecondWind, Spells.ArmsLength
                                                                            };

        public static int CheckLastSpellsForWeaving()
        {
            var weavingCounter = 0;

            if (Casting.SpellCastHistory.Count < 2)
                return 0;

            var lastSpellCast = Casting.SpellCastHistory.ElementAt(0).Spell;
            var secondLastSpellCast = Casting.SpellCastHistory.ElementAt(1).Spell;

            if (OGCDSpells.Contains(lastSpellCast))
                weavingCounter += 1;
            else
                return 0;

            if (OGCDSpells.Contains(secondLastSpellCast))
                weavingCounter += 1;

            return weavingCounter;
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

        public static bool HealthCheck(GameObject tar)
        {
            if (tar == null)
                return false;

            if (tar.EnglishName.Contains("Dummy"))
                return true;

            if (BardSettings.Instance.DontDotIfMultiDotTargetIsDyingSoon)
            {
                // Target doesn't have a combat time left yet
                if (Combat.CurrentTargetCombatTimeLeft < 0)
                    return true;

                return Combat.CurrentTargetCombatTimeLeft > BardSettings.Instance.DontDotIfMultiDotTargetIsDyingWithinXSeconds;
            }

            if (tar.IsBoss())
                return true;

            return tar.CurrentHealthPercent > 20;
        }
    }
}