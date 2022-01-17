using Buddy.Coroutines;
using Clio.Common;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Models.Account;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Extensions
{
    internal static class GameObjectExtensions
    {
        public static bool ThoroughCanAttack(this GameObject unit)
        {
            if (unit == null)
                return false;

            if (WorldManager.ZoneId == 732)
            {
                return unit.Type != GameObjectType.Pc;
            }

            return unit.CanAttack;
        }

        public static bool BeingTargeted(this GameObject unit)
        {
            return Combat.Enemies.Any(x => x.TargetCharacter == unit);
        }

        public static bool BeingTargetedBy(this GameObject unit, GameObject other)
        {
            var lp = other as Character;
            return lp != null && lp.TargetGameObject == unit;
        }

        public static bool WithinSpellRange(this GameObject unit, float range)
        {
            return (Core.Me.Distance2D(unit) - Core.Me.CombatReach - unit.CombatReach) <= range;
        }

        public static async Task<bool> UseItem(this GameObject unit, uint itemId, bool lookForMedicated = false)
        {
            var item = InventoryManager.FilledSlots.FirstOrDefault(r => r.RawItemId == itemId);

            if (item == null)
                return false;

            if (!item.CanUse(unit))
                return false;

            while (item.CanUse(unit))
            {
                item.UseItem();
                await Coroutine.Yield();
            }

            // Potions give a Medicated aura
            if (lookForMedicated)
            {
                await Coroutine.Wait(3000, () => unit.HasAura(Auras.Medicated));
            }

            return true;
        }

        public static int CombatTimeLeft(this GameObject unit)
        {
            if (unit == null)
                return 0;

            if (unit.EnglishName.Contains("Dummy"))
                return 9999;

            var haveUnit = Tracking.EnemyInfos.Any(r => r.Unit == unit);

            return haveUnit ? Convert.ToInt32(Tracking.EnemyInfos.First(r => r.Unit == unit).CombatTimeLeft) : 0;
        }

        public static double TimeInCombat(this GameObject unit)
        {
            if (unit == null)
                return 0;

            var haveUnit = Tracking.EnemyInfos.Any(r => r.Unit == unit);

            return haveUnit ? Tracking.EnemyInfos.First(r => r.Unit == unit).TimeInCombat : 0;
        }

        public static bool HasAura(this GameObject unit, uint spell, bool isMyAura = false, int msLeft = 0)
        {
            var unitAsCharacter = unit as Character;

            if (unitAsCharacter == null || !unitAsCharacter.IsValid)
            {
                return false;
            }

            var auras = isMyAura
                ? unitAsCharacter.CharacterAuras.Where(r => r.CasterId == Core.Player.ObjectId && r.Id == spell)
                : unitAsCharacter.CharacterAuras.Where(r => r.Id == spell);

            return auras.Any(aura => aura.TimespanLeft.TotalMilliseconds >= msLeft);
        }

        public static bool HasAuraCharge(this GameObject unit, uint spell, bool isMyAura = false)
        {
            var unitAsCharacter = unit as Character;

            if (unitAsCharacter == null || !unitAsCharacter.IsValid)
            {
                return false;
            }

            var auras = isMyAura
                ? unitAsCharacter.CharacterAuras.Where(r => r.CasterId == Core.Player.ObjectId && r.Id == spell)
                : unitAsCharacter.CharacterAuras.Where(r => r.Id == spell);

            return auras.Any(aura => aura.Value == 1);
        }

        public static bool HasAnyAura(this GameObject unit, uint[] auras, bool isMyAura = false, int msLeft = 0)
        {
            var unitAsCharacter = unit as Character;

            if (unitAsCharacter == null || !unitAsCharacter.IsValid)
                return false;

            return isMyAura
                ? unitAsCharacter.CharacterAuras.Any(r => r.CasterId == Core.Player.ObjectId && auras.Contains(r.Id) && r.TimespanLeft.TotalMilliseconds >= msLeft)
                : unitAsCharacter.CharacterAuras.Any(r => auras.Contains(r.Id) && r.TimespanLeft.TotalMilliseconds >= msLeft);

        }

        public static bool HasAnyAura(this GameObject unit, List<uint> auras, bool isMyAura = false, int msLeft = 0)
        {
            var unitAsCharacter = unit as Character;

            if (unitAsCharacter == null || !unitAsCharacter.IsValid)
            {
                return false;
            }

            return isMyAura
                ? unitAsCharacter.CharacterAuras.Any(r => r.CasterId == Core.Player.ObjectId && auras.Contains(r.Id) && r.TimespanLeft.TotalMilliseconds >= msLeft)
                : unitAsCharacter.CharacterAuras.Any(r => auras.Contains(r.Id) && r.TimespanLeft.TotalMilliseconds >= msLeft);
        }

        public static bool HasAllAuras(this GameObject unit, List<uint> auras, bool areMyAuras = false, int msLeft = 0)
        {
            var unitAsCharacter = unit as Character;

            if (unitAsCharacter == null || !unitAsCharacter.IsValid)
            {
                return false;
            }

            return areMyAuras
                ? unitAsCharacter.CharacterAuras.Where(x => x.CasterId == Core.Player.ObjectId && (x.TimespanLeft.TotalMilliseconds >= msLeft || x.TimespanLeft.TotalMilliseconds < 0)).Select(r => r.Id).ToList().Intersect(auras).Count() == auras.Count
                : unitAsCharacter.CharacterAuras.Where(x => (x.TimespanLeft.TotalMilliseconds >= msLeft || x.TimespanLeft.TotalMilliseconds < 0)).Select(r => r.Id).ToList().Intersect(auras).Count() == auras.Count;
        }

        public static bool ValidAttackUnit(this GameObject unit)
        {
            return unit != null && unit.IsValid && unit.IsTargetable && unit.CanAttack && unit.CurrentHealth > 0;
        }

        public static bool NotInvulnerable(this GameObject unit)
        {
            return unit != null && !unit.HasAnyAura(Auras.Invincibility);
        }

        public static IEnumerable<BattleCharacter> EnemiesNearby(this GameObject unit, float distance)
        {
            return Combat.Enemies.Where(r => r.Distance(unit) <= distance + Core.Me.CombatReach + unit.CombatReach);
        }

        public static IEnumerable<BattleCharacter> EnemiesNearbyOoc(this GameObject unit, float distance)
        {
            return GameObjectManager.GetObjectsOfType<BattleCharacter>().Where(r => r.IsTargetable && r.CurrentHealth > 0 && r.CanAttack && r.Distance(unit) <= distance);
        }

        public static IEnumerable<BattleCharacter> EnemiesNearbyWithMyAura(this GameObject unit, float distance, uint aura)
        {
            return Combat.Enemies.Where(r => r.Distance(unit) <= distance && r.HasAura(aura, true));
        }

        public static bool IsTank(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && Tanks.Contains(gameObject.CurrentJob);
        }

        public static bool IsMainTank(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null
                && Tanks.Contains(gameObject.CurrentJob)
                && (gameObject.BeingTargetedBy(Core.Me.CurrentTarget)
                    || gameObject.BeingTargetedBy(gameObject.TargetGameObject)
                    || PartyManager.AllMembers.Select(r => r.BattleCharacter).Where(r => Tanks.Contains(r.CurrentJob)).Count() == 1);
        }

        public static bool IsTank(this GameObject unit, bool mainTank)
        {
            if (mainTank)
                return unit.IsMainTank();
            else
                return unit.IsTank();
        }

        public static bool IsHealer(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && Healers.Contains(gameObject.CurrentJob);
        }

        public static bool IsDps(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && Dps.Contains(gameObject.CurrentJob);
        }

        public static bool IsRangedDps(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && RangedDps.Contains(gameObject.CurrentJob);
        }

        public static bool IsBlueMage(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && ClassJobType.BlueMage.Equals(gameObject.CurrentJob);
        }

        public static bool IsBlueMageHealer(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && ClassJobType.BlueMage.Equals(gameObject.CurrentJob) && gameObject.HasAura(Auras.AetherialMimicryHealer);
        }

        public static bool IsBlueMageTank(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && ClassJobType.BlueMage.Equals(gameObject.CurrentJob) && gameObject.HasAura(Auras.AetherialMimicryTank);
        }

        public static bool IsBlueMageDps(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && ClassJobType.BlueMage.Equals(gameObject.CurrentJob) && gameObject.HasAura(Auras.AetherialMimicryDps);
        }

        public static bool IsRangedDpsCard(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && RangedDpsCard.Contains(gameObject.CurrentJob);
        }

        public static bool IsMeleeDps(this GameObject unit)
        {
            var gameObject = unit as Character;
            return gameObject != null && MeleeDps.Contains(gameObject.CurrentJob);
        }

        public static bool HasMyRegen(this GameObject unit)
        {
            return unit.HasAnyAura(new uint[]
            {
                Auras.Regen,
                Auras.Regen2,
                Auras.AspectedBenefic,
                Auras.AspectedHelios
            });
        }

        public static bool HealthCheck(this GameObject tar, int healthSetting, float healthSettingPercent)
        {
            if (tar == null)
                return false;

            if (tar.IsBoss())
                return true;

            if (tar.EnglishName.Contains("Dummy"))
                return true;

            if (tar.CurrentHealth < healthSetting || tar.CurrentHealthPercent < healthSettingPercent)
                return false;

            // If our target has more health than our setting and more health percent than our health percent setting, return true
            if (tar.CurrentHealth > healthSetting && tar.CurrentHealthPercent > healthSettingPercent)
                return true;

            // If our target has more hp percent than our hp percent setting but has less health than our health setting, return true
            if (tar.CurrentHealthPercent > healthSettingPercent && tar.CurrentHealth < healthSetting)
                return true;

            // if our target has more health than our setting but less health percent than our hp percent setting, return true
            if (tar.CurrentHealth > healthSetting && tar.CurrentHealthPercent < healthSettingPercent)
                return true;

            // if our target has less health than our setting and less health than our percent setting, return false

            return tar.CurrentHealth >= healthSetting || !(tar.CurrentHealthPercent < healthSettingPercent);
        }

        public static AstrologianSect Sect(this GameObject unit)
        {
            if (unit.HasAura(Auras.DiurnalSect)) return AstrologianSect.Diurnal;
            if (unit.HasAura(Auras.NocturnalSect)) return AstrologianSect.Nocturnal;
            return AstrologianSect.None;
        }

        public static bool IsBoss(this GameObject unit)
        {
            return unit != null && (XivDataHelper.BossDictionary.ContainsKey(unit.NpcId) || unit.EnglishName.Contains("Dummy"));
        }

        public static float GetResurrectionWeight(this GameObject c)
        {
            if (c.IsHealer() || c.IsBlueMageHealer())
                return 100;

            if (c.IsTank() || c.IsBlueMageTank())
                return 90;

            if (c.IsDps() || c.IsBlueMageDps())
            {
                var cha = c as Character;
                if (cha.CurrentJob == ClassJobType.RedMage && cha.ClassLevel >= Spells.Verraise.LevelAcquired)
                    return 80;
                if (cha.CurrentJob == ClassJobType.Summoner && cha.ClassLevel >= Spells.Resurrection.LevelAcquired)
                    return 70;
                return 60;
            }

            return 0;
        }

        public static float GetHealingWeight(this GameObject c)
        {
            if (!BaseSettings.Instance.UseWeightedHealingPriority)
                return 1;

            var cha = c as Character;

            var roleWeight = cha.IsTank() ?
                BaseSettings.Instance.WeightedTankRole :
                cha.IsHealer() ?
                BaseSettings.Instance.WeightedHealerRole :
                cha.CurrentJob == ClassJobType.RedMage || cha.CurrentJob == ClassJobType.Summoner ?
                BaseSettings.Instance.WeightedRezMageRole :
                BaseSettings.Instance.WeightedDpsRole;
            var selfWeight = c == Core.Me ? BaseSettings.Instance.WeightedSelf : 1.0f;
            var regens = CharacterExtensions.HealerRegens;
            var shields = CharacterExtensions.HealerShields;
            var ignores = CharacterExtensions.BuffIgnore;
            var auras = cha.CharacterAuras.Where(a => !ignores.Contains(a.Id));
            var debuffWeight = (float)Math.Pow(BaseSettings.Instance.WeightedDebuff, auras.Count(r => r.IsDebuff));
            var buffWeight = (float)Math.Pow(BaseSettings.Instance.WeightedBuff, auras.Count(r => !r.IsDebuff && !regens.Contains(r.Id) && !shields.Contains(r.Id)));
            var regenWeight = (float)Math.Pow(BaseSettings.Instance.WeightedRegen, auras.Count(r => regens.Contains(r.Id)));
            var shieldWeight = (float)Math.Pow(BaseSettings.Instance.WeightedShield, auras.Count(r => shields.Contains(r.Id)));
            var weaknessWeight = (float)Math.Pow(BaseSettings.Instance.WeightedWeakness, cha.HasAura(Auras.Weakness) ? 1f : 0f);
            var distanceMinWeight = BaseSettings.Instance.WeightedDistanceMin;
            var distanceMaxWeight = BaseSettings.Instance.WeightedDistanceMax;
            var distanceWeight = distanceMinWeight + (distanceMaxWeight - distanceMinWeight) * (Core.Me.Distance(c) / 30);
            /*
             * Logger.WriteInfo($"{c.Name} - \n" +
                $"hp {c.CurrentHealthPercent}\n" +
                $"self {selfWeight}\n" +
                $"role {roleWeight}\n" +
                $"debuff {debuffWeight}\n" +
                $"regen {regenWeight}\n" +
                $"shield {shieldWeight}\n" +
                $"weakness {weaknessWeight}\n" +
                $"distance {distanceWeight}\n");
            */

            var weight = c.CurrentHealthPercent
                * selfWeight
                * roleWeight
                * debuffWeight
                * buffWeight
                * regenWeight
                * shieldWeight
                * weaknessWeight
                * distanceWeight;

            return weight;
        }

        //This method return the heading from the player to the target object in radians.
        //A circle has 2*Pi radians, so an angle of 90 degrees would be Pi/2, and an angle
        //of 30 degrees would be Pi/6, etc.
        //It returns the absolute value, so targets to the left and right both return
        //positive values. A target directly in front would return 0.
        public static float RadiansFromPlayerHeading(this GameObject target)
        {
            var playerLocation = Core.Me.Location;
            var playerHeading = GameSettingsManager.FaceTargetOnAction && BaseSettings.Instance.UseAutoFaceChecks ?
                MathEx.NormalizeRadian(MathHelper.CalculateHeading(playerLocation, Core.Me.CurrentTarget.Location) + (float)Math.PI)
                :
                Core.Me.Heading;
            var targetLocation = target.Location;
            var d = Math.Abs(MathEx.NormalizeRadian(playerHeading - MathEx.NormalizeRadian(MathHelper.CalculateHeading(playerLocation, targetLocation) + (float)Math.PI)));

            if (d > Math.PI)
            {
                d = Math.Abs(d - 2 * (float)Math.PI);
            }

            return d;
        }

        public static bool InView(this GameObject target)
        {
            if (target == null)
                return false;

            if (target == Core.Me)
                return true;

            return target.RadiansFromPlayerHeading() < 0.78539f; //This is Pi/4 radians, or 45 degrees left or right
        }

        public static bool InCustomRadiantCone(this GameObject target, float angle)
        {
            if (target == null)
                return false;

            if (target == Core.Me)
                return true;

            return target.RadiansFromPlayerHeading() < angle;
        }

        public static bool InCustomDegreeCone(this GameObject target, int angle)
        {
            if (target == null)
                return false;

            if (target == Core.Me)
                return true;

            float radians = ((float)Math.PI / 180) * angle;

            return target.RadiansFromPlayerHeading() < radians;
        }


        private static readonly List<ClassJobType> Tanks = new List<ClassJobType>()
        {
            ClassJobType.Gladiator,
            ClassJobType.Paladin,
            ClassJobType.Marauder,
            ClassJobType.Warrior,
            ClassJobType.DarkKnight,
            ClassJobType.Gunbreaker,
            ClassJobType.BlueMage,
        };

        private static readonly List<ClassJobType> Healers = new List<ClassJobType>()
        {
            ClassJobType.Arcanist,
            ClassJobType.Scholar,
            ClassJobType.Conjurer,
            ClassJobType.WhiteMage,
            ClassJobType.Astrologian,
            ClassJobType.BlueMage,
            ClassJobType.Sage,
        };

        private static readonly List<ClassJobType> Dps = new List<ClassJobType>()
        {
            ClassJobType.Archer,
            ClassJobType.Bard,
            ClassJobType.Thaumaturge,
            ClassJobType.BlackMage,
            ClassJobType.Lancer,
            ClassJobType.Dragoon,
            ClassJobType.Pugilist,
            ClassJobType.Monk,
            ClassJobType.Rogue,
            ClassJobType.Ninja,
            ClassJobType.Machinist,
            ClassJobType.RedMage,
            ClassJobType.Samurai,
            ClassJobType.Summoner,
            ClassJobType.Dancer,
            ClassJobType.BlueMage,
            ClassJobType.Reaper,
        };

        private static readonly List<ClassJobType> RangedDps = new List<ClassJobType>()
        {
            ClassJobType.Archer,
            ClassJobType.Bard,
            ClassJobType.Machinist,
            ClassJobType.Dancer,
            ClassJobType.BlueMage,
        };

        private static readonly List<ClassJobType> MeleeDps = new List<ClassJobType>()
        {
            ClassJobType.Lancer,
            ClassJobType.Dragoon,
            ClassJobType.Pugilist,
            ClassJobType.Monk,
            ClassJobType.Rogue,
            ClassJobType.Ninja,
            ClassJobType.Samurai,
            ClassJobType.BlueMage,
            ClassJobType.Reaper,
        };

        private static readonly List<ClassJobType> RangedDpsCard = new List<ClassJobType>()
        {
            ClassJobType.Archer,
            ClassJobType.Bard,
            ClassJobType.Machinist,
            ClassJobType.Dancer,
            ClassJobType.Thaumaturge,
            ClassJobType.BlackMage,
            ClassJobType.Machinist,
            ClassJobType.RedMage,
            ClassJobType.Summoner,
            ClassJobType.BlueMage,
        };
    }
}
