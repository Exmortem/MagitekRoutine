using Clio.Utilities;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Utilities.Collections;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Debug = Magitek.ViewModels.Debug;
using DebugSettings = Magitek.Models.Account.BaseSettings;

namespace Magitek.Utilities
{
    public static class FightLogic
    {
        internal enum FfxivExpansion
        {
            ARealmReborn,
            Heavensward,
            Stormblood,
            Shadowbringers,
            Endwalker,
        }

        private static readonly Stopwatch FlStopwatch = new Stopwatch();

        private static readonly Stopwatch GetEnemyLogicAndEnemyCacheAge = new Stopwatch();

        private static TimeSpan FlCooldown
        {
            get
            {
                if (!FlStopwatch.IsRunning) return TimeSpan.Zero;

                var timeRemaining = new TimeSpan(0, 0, 0, 5).Subtract(FlStopwatch.Elapsed);

                if (timeRemaining > TimeSpan.Zero) return timeRemaining;

                FlStopwatch.Reset();

                return TimeSpan.Zero;
            }
        }

        public static bool IsFlReady => FlCooldown == TimeSpan.Zero;

        private static (Encounter, Enemy, BattleCharacter) GetEnemyLogicAndEnemyCached { get; set; }

        public static async Task<bool> DoAndBuffer(Task<bool> task)
        {
            if (!await task) return false;

            FlStopwatch.Start();
            return true;
        }

        public static Character EnemyIsCastingTankBuster()
        {
            if (!IsFlReady)
                return null;

            var (encounter, enemyLogic, enemy) = GetEnemyLogicAndEnemy();

            if (enemyLogic?.TankBusters == null)
                return EnemyIsCastingSharedTankBuster();

            var output = enemyLogic.TankBusters.Contains(enemy.CastingSpellId)
                ? Group.CastableTanks.FirstOrDefault(x => x == enemy.TargetCharacter)
                : null;

            if (output != null && DebugSettings.Instance.DebugFightLogic)
                Logger.WriteInfo(
                    $"[TankBuster Detected] {encounter.Name} {enemy.Name} casting {enemy.SpellCastInfo.Name} on {output.Name} ({output.CurrentJob})");

            if (output != null)
                FlStopwatch.Start();

            return output;
        }

        public static Character EnemyIsCastingSharedTankBuster()
        {
            if (!IsFlReady)
                return null;

            var (encounter, enemyLogic, enemy) = GetEnemyLogicAndEnemy();

            if (enemyLogic?.SharedTankBusters == null)
                return null;

            var output = enemyLogic.SharedTankBusters.Contains(enemy.CastingSpellId)
                ? Group.CastableTanks.FirstOrDefault(x => x != enemy.TargetCharacter)
                : null;

            if (output != null && DebugSettings.Instance.DebugFightLogic)
                Logger.WriteInfo(
                    $"[Shared TankBuster Detected] {encounter.Name} {enemy.Name} casting {enemy.SpellCastInfo.Name}. Handling for {output.Name} ({output.CurrentJob})");

            if (output != null)
                FlStopwatch.Start();

            return output;

        }

        public static bool EnemyIsCastingAoe()
        {
            if (!IsFlReady)
                return false;

            var (encounter, enemyLogic, enemy) = GetEnemyLogicAndEnemy();

            if (enemyLogic?.Aoes == null)
                return false;

            var output = enemyLogic.Aoes.Contains(enemy.CastingSpellId);

            if (output && DebugSettings.Instance.DebugFightLogic)
                Logger.WriteInfo($"[AOE Detected] {encounter.Name} {enemy.Name} casting {enemy.SpellCastInfo.Name}");

            return output;
        }

        public static bool EnemyIsCastingBigAoe()
        {
            if (!IsFlReady)
                return false;

            var (encounter, enemyLogic, enemy) = GetEnemyLogicAndEnemy();

            if (enemyLogic == null)
                return false;

            if (enemyLogic.BigAoes == null)
                return EnemyIsCastingAoe();

            var output = enemyLogic.BigAoes.Contains(enemy.CastingSpellId);

            if (output && DebugSettings.Instance.DebugFightLogic)
                Logger.WriteInfo(
                    $"[BIG AOE Detected] {encounter.Name} {enemy.Name} casting {enemy.SpellCastInfo.Name}");

            return output;
        }

        public static bool ZoneHasFightLogic()
        {
            if (!DebugSettings.Instance.UseFightLogic)
                return false;

            if (!Globals.InActiveDuty)
                return false;

            if (!Core.Me.InCombat)
                return false;

            return Encounters.Any(x => x.ZoneId == WorldManager.RawZoneId);
        }

        public static bool EnemyHasAnyTankbusterLogic()
        {
            if (ZoneHasFightLogic())
            {
                var (encounter, enemyLogic, enemy) = GetEnemyLogicAndEnemy();

                return (enemyLogic?.TankBusters != null || enemyLogic?.SharedTankBusters != null);
            }

            return false;
        }

        public static bool EnemyHasAnyAoeLogic()
        {
            if (ZoneHasFightLogic())
            {
                var (encounter, enemyLogic, enemy) = GetEnemyLogicAndEnemy();

                return (enemyLogic?.Aoes != null || enemyLogic?.BigAoes != null);
            }

            return false;
        }

        private static (Encounter, Enemy, BattleCharacter) GetEnemyLogicAndEnemy()
        {
            if (GetEnemyLogicAndEnemyCacheAge.IsRunning && GetEnemyLogicAndEnemyCacheAge.ElapsedMilliseconds < 1000)
                return GetEnemyLogicAndEnemyCached;

            Encounter encounter = null;
            Enemy enemyLogic = null;
            BattleCharacter enemy = null;

            if (!DebugSettings.Instance.UseFightLogic)
                return SetAndReturn();

            if (!Globals.InActiveDuty)
                return SetAndReturn();

            if (!Core.Me.InCombat)
                return SetAndReturn();

            encounter = Encounters.FirstOrDefault(x => x.ZoneId == WorldManager.RawZoneId);

            if (encounter == null)
                return SetAndReturn();

            enemyLogic = encounter.Enemies.FirstOrDefault(x => Combat.Enemies.Any(y => x.Id == y.NpcId));

            if (enemyLogic == null)
                return SetAndReturn();

            enemy = Combat.Enemies.FirstOrDefault(y => enemyLogic.Id == y.NpcId);

            return SetAndReturn();

            (Encounter, Enemy, BattleCharacter) SetAndReturn()
            {
                if (DebugSettings.Instance.DebugFightLogicFound)
                {
                    Debug.Instance.FightLogicData =
                        $"You are currently in {WorldManager.CurrentZoneName} ({WorldManager.RawZoneId})\n\n";

                    if (encounter == null && enemyLogic == null && enemy == null)
                        Debug.Instance.FightLogicData += "There is no Fight Logic for this zone. \n";
                    else
                    {
                        Debug.Instance.FightLogicData +=
                            $"Fight Logic Recognized for {encounter.Name} from ({encounter.Expansion})\n" +
                            $"There is Logic for {encounter.Enemies.Count()} enemies.\n\n";

                        encounter.Enemies.ForEach(element =>
                        {
                            Debug.Instance.FightLogicData += $"Enemy: {element.Name} ({element.Id}):\n";

                            if (element.TankBusters != null)
                                Debug.Instance.FightLogicData +=
                                    $"\tTankbusters:\n{string.Join("", element.TankBusters.Select(tb => $"\t\t{DataManager.GetSpellData(tb).Name} ({tb})\n"))}";

                            if (element.SharedTankBusters != null)
                                Debug.Instance.FightLogicData +=
                                    $"\tShared Tankbusters:\n{string.Join("", element.SharedTankBusters.Select(stb => $"\t\t{DataManager.GetSpellData(stb).Name} ({stb})\n"))}";

                            if (element.Aoes != null)
                                Debug.Instance.FightLogicData +=
                                    $"\tAoes:\n{string.Join("", element.Aoes.Select(aoe => $"\t\t{DataManager.GetSpellData(aoe).Name} ({aoe})\n"))}";

                            if (element.BigAoes != null)
                                Debug.Instance.FightLogicData +=
                                    $"\tBig Aoes:\n{string.Join("", element.BigAoes.Select(baoe => $"\t\t{DataManager.GetSpellData(baoe).Name} ({baoe})\n"))}";

                            Debug.Instance.FightLogicData += $"\n\nLast Mechanic Detected: {FlCooldown.TotalMilliseconds}";
                        });
                    }
                }

                GetEnemyLogicAndEnemyCached = (encounter, enemyLogic, enemy);
                if (!GetEnemyLogicAndEnemyCacheAge.IsRunning)
                    GetEnemyLogicAndEnemyCacheAge.Start();
                else GetEnemyLogicAndEnemyCacheAge.Restart();
                return GetEnemyLogicAndEnemyCached;
            }
        }

        private static readonly List<Encounter> Encounters = new List<Encounter> {
            #region A Realm Reborn: Normal Raids

            new Encounter {
                ZoneId = ZoneId.TheBindingCoilOfBahamutTurn5,
                Name = "Normal Raid: The Binding Coil of Bahamut - Turn 5 (T5 - Twintania)",
                Expansion = FfxivExpansion.ARealmReborn,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 1482,
                        Name = "Twintania",
                        TankBusters = new List<uint> {
                            1458 //Death Sentence
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },

            #endregion

            #region Heavensward: Alliance Raids

            new Encounter {
                ZoneId = ZoneId.DunScaith,
                Name = "Alliance Raid: Dun Scaith",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5509,
                        Name = "Ferdiad Hollow",
                        TankBusters = new List<uint> {
                            7320 //Jongleur
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 3780,
                        Name = "Proto Ultima",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            7762, // Aetherochemical Flare & Supernova
                            7581 // Aetherochemical Flare & Supernova
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 5515,
                        Name = "Scathach",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            7474 //Thirty Souls
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 2564,
                        Name = "Diabolos",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            7184, //Ruinous Omen
                            7185 //Ruinous Omen
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 5526,
                        Name = "Diabolos Hollow",
                        TankBusters = new List<uint> {
                            7193 //Camisado
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = new List<uint> {
                            7202, //Omen
                            7203 //Omen
                        }
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheWeepingCityOfMhach,
                Name = "Alliance Raid: The Weeping City of Mhach",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 4878,
                        Name = "Forgall",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6091 //Hell Wind
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 4897,
                        Name = "Calofisteri",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6169 //Dancing Mad
                        },
                        BigAoes = null
                    }
                }
            },

            #endregion

            #region Heavensward: Dungeons

            new Encounter {
                ZoneId = ZoneId.TheAetherochemicalResearchFacility,
                Name = "Aetherochemical Research Facility",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 3822,
                        Name = "Igeyorhm",
                        TankBusters = new List<uint> {
                            4348 //Dark Orb
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 2143,
                        Name = "Lahabrea",
                        TankBusters = new List<uint> {
                            4348 //Dark Orb
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 3829,
                        Name = "Ascian Prime",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            4361, //Shadowflare
                            4362 //Annihilation
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.BaelsarsWall,
                Name = "Dungeon: Baelsar's Wall",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5560,
                        Name = "Magitek Predator",
                        TankBusters = new List<uint> {
                            7346 //Magitek Claw
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            7356 //Launcher
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 5564,
                        Name = "The Griffin",
                        TankBusters = new List<uint> {
                            7362 //Claw
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            7363 //Beak
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheFractalContinuum,
                Name = "Dungeon: Fractal Continuum",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 3428,
                        Name = "Phantom Ray",
                        TankBusters = new List<uint> {
                            3962 //Rapid Sever
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheGreatGubalLibraryHard,
                Name = "Dungeon: Gubal Library (Hard)",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5218,
                        Name = "Liquid Flame",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6469 //Bibliocide
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 5219,
                        Name = "Strix",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6485 //Properties of Darkness II
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SohmAlHard,
                Name = "Dungeon: Sohm Al (Hard)",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5529,
                        Name = "The Leightonward",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            7216 //Inflammable Fumes
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 5531,
                        Name = "Lava Scorpion",
                        TankBusters = new List<uint> {
                            7232, //Deadly Thrust
                            7240 //Deadly Thrust
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 5533,
                        Name = "The Scoprion's Tail",
                        TankBusters = new List<uint> {
                            7232, //Deadly Thrust
                            7240 //Deadly Thrust
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheVault,
                Name = "Dungeon: The Vault",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 3634,
                        Name = "Ser Adelphel",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            4126 //Holiest of Holy
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 3642,
                        Name = "Ser Charibert",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            4149 //Altar Pyre
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.Xelphatol,
                Name = "Dungeon: Xelphatol",
                Expansion = FfxivExpansion.ARealmReborn,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5265,
                        Name = "Nuzal Hueloc",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6600 //Long Burst
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 5272,
                        Name = "Tozol Huatotl",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6613 //Ixali Aero III
                        },
                        BigAoes = null
                    }
                }
            },

            #endregion

            #region Heavensward: Normal Raids

            new Encounter {
                ZoneId = ZoneId.AlexanderTheCuffOfTheSon,
                Name = "Normal Raid: Alexander - The Cuff of The Son - A6N",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 4705,
                        Name = "Swindler",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            5919 //Bio-arithmeticks
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheBurdenOfTheSon,
                Name = "Normal Raid: Alexander - The Burden of The Son - A8N",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 4707,
                        Name = "Onslaughter",
                        TankBusters = new List<uint> {
                            5936 //Perpetual Ray
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 4708,
                        Name = "Brute Justice",
                        TankBusters = new List<uint> {
                            5966 //Double Rocket Punch
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheSoulOfTheCreator,
                Name = "Normal Raid: Alexander - The Soul Of The Creator - A12N",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5193,
                        Name = "Alexander Prime",
                        TankBusters = new List<uint> {
                            6884 //Punishing Heat
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6887 //Mega Holy
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9220,
                        Name = "Alexander Prime",
                        TankBusters = new List<uint> {
                            6884 //Punishing Heat
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6887 //Mega Holy
                        },
                        BigAoes = null
                    }
                }
            },


            #endregion

            #region Heavensward: Savage Raids

            new Encounter {
                ZoneId = ZoneId.AlexanderTheFistOfTheFatherSavage,
                Name = "Normal Raid: Alexander - The First of The Father (Savage) - A1S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 3747,
                        Name = "Oppressor",
                        TankBusters = new List<uint> {
                            3658 //Hypercompressed
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 3748,
                        Name = "Oppressor 0.5",
                        TankBusters = new List<uint> {
                            3658 //Hypercompressed
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheArmOfTheFatherSavage,
                Name = "Normal Raid: Alexander - The Arm of The Father (Savage) - A3S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9211,
                        Name = "Living Liquid",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            3838 //Cascade
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheBurdenOfTheFatherSavage,
                Name = "Normal Raid: Alexander - The Burden of The Father (Savage) - A4S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 3772,
                        Name = "The Manipulator",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            5095 //Mortal Revolution
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheCuffOfTheSonSavage,
                Name = "Normal Raid: Alexander - The Cuff of The Son (Savage) - A6S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 4705,
                        Name = "Swindler",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            5648 //Bio-arithmeticks
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheArmOfTheSonSavage,
                Name = "Normal Raid: Alexander - The Arm of The Son (Savage) - A7S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 3376,
                        Name = "Quickthinx Allthoughts",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            5880 //Sizzlespark
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheBurdenOfTheSonSavage,
                Name = "Normal Raid: Alexander - The Burden of The Son (Savage) - A8S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 4707,
                        Name = "Onslaughter",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            5675 //Perpetual Ray
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 4705,
                        Name = "Swindler",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            5706 //Bio-Arithmeticks
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 4708,
                        Name = "Brute Justice",
                        TankBusters = new List<uint> {
                            5900 //Final Punch
                        },
                        SharedTankBusters = new List<uint> {
                            5731 //Double Rocket Punch
                        },
                        Aoes = new List<uint> {
                            5736 //Short Needle
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheBreathOfTheCreatorSavage,
                Name = "Normal Raid: Alexander - The Breath of The Creator (Savage) - A10S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5180,
                        Name = "Lamebrix Strikebocks",
                        TankBusters = new List<uint> {
                            6815 //Gobrush Rushgob
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 5181,
                        Name = "Lamebrix Strikebocks",
                        TankBusters = new List<uint> {
                            6815 //Gobrush Rushgob
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheHeartOfTheCreatorSavage,
                Name = "Normal Raid: Alexander - The Heart of The Creator (Savage) - A11S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9218,
                        Name = "Cruise Chaser",
                        TankBusters = null,
                        SharedTankBusters = new List<uint> {
                            6783 //Laser X Sword
                        },
                        Aoes = new List<uint> {
                            6788 //Whirlwind
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlexanderTheSoulOfTheCreatorSavage,
                Name = "Normal Raid: Alexander - The Soul Of The Creator (Savage) - A12S",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5193,
                        Name = "Alexander Prime",
                        TankBusters = new List<uint> {
                            6633 //Punishing Heat
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6638 //Mega Holy
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9220,
                        Name = "Alexander Prime",
                        TankBusters = new List<uint> {
                            6633, //Punishing Heat
                            6669 //Chastening Heat
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            6638 //Mega Holy
                        },
                        BigAoes = null
                    }
                }
            },


            #endregion

            #region Heavensward: Extreme Trials

            new Encounter {
                ZoneId = ZoneId.ContainmentBayP1T6Extreme,
                Name = "Trial: Sophia (Extreme)",
                Expansion = FfxivExpansion.Heavensward,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5199,
                        Name = "Sophia",
                        TankBusters = new List<uint> {
                            6596 //Tankbuster Swap?
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },


            #endregion

            #region Stormblood: Alliance Raids
            new Encounter {
                ZoneId = ZoneId.TheOrbonneMonastery,
                Name = "Alliance Raid: The Orbonne Monastery",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7919,
                        Name = "Mustadio",
                        TankBusters = new List<uint>() {
                            14137 //Arm Shot
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7899,
                        Name = "The Thunder God",
                        TankBusters = new List<uint>() {
                            14162, //Crush Helm (Healer)
                            14163 //Crush Helm (Tank)
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7657,
                        Name = "Ultima, The High Seraph",
                        TankBusters = new List<uint> {
                            14506 //Redemption
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7909,
                        Name = "Ultima, The High Seraph",
                        TankBusters = new List<uint> {
                            14506 //Redemption
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheRidoranaLighthouse,
                Name = "Alliance Raid: The Ridorana Lighthouse",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7245,
                        Name = "Famfrit, The Darkening Cloud",
                        TankBusters = new List<uint>() {
                            11326 //Tide Pode
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7223,
                        Name = "Belias, The Gigas",
                        TankBusters = new List<uint>() {
                            11483 //Fire
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7237,
                        Name = "Construct 7",
                        TankBusters = new List<uint>() {
                            11354, //Destroy
                            11377 //Destroy
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7070,
                        Name = "Yiazmat",
                        TankBusters = new List<uint>() {
                            11598 //Rake Buster
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheRoyalCityOfRabanastre,
                Name = "Alliance Raid: The Royal City of Rabanastre",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6922,
                        Name = "Hashmal, Bringer Of Order",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            9688 //Quake IV
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6941,
                        Name = "Rofocale",
                        TankBusters = new List<uint>() {
                            9856 //Crush Helm
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6925,
                        Name = "Argath Thadalfus",
                        TankBusters = new List<uint>() {
                            9773 //Crippling Blow
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            9754 //Fire IV
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion
            
            #region Stormblood: Dungeons
            new Encounter {
                ZoneId = ZoneId.AlaMhigo,
                Name = "Dungeon: Ala Mhigo",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6037,
                        Name = "Magitek Scorpion",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8269 //Electromagnetic Field
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6038,
                        Name = "Aulus Mal Asina",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8271 //Mana Burst
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6039,
                        Name = "Zenos Yae Galvus",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8301 //Concentrativity
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.BardamsMettle,
                Name = "Dungeon: Bardam's Mettle",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6173,
                        Name = "Garula",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            7930 //War Cry
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6155,
                        Name = "Yol",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            7946 //Wind Unbound
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.CastrumAbania,
                Name = "Dungeon: Castrum Abania",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6263,
                        Name = "Magna Roader",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            7958 //Fire III
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6267,
                        Name = "Number XXIV",
                        TankBusters = new List<uint>() {
                            7963 //Stab
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6268,
                        Name = "Inferno",
                        TankBusters = new List<uint>() {
                            7974, //Ketu Slash
                            8331, //Ketu Slash
                            8332 //Ketu Slash
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheDrownedCityOfSkalla,
                Name = "Dungeon: The Drowned City of Skalla",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6907,
                        Name = "Kelpie",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            9808 //Rising Seas
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheFractalContinuumHard,
                Name = "Dungeon: The Fractal Continuum (Hard)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7055,
                        Name = "The Ultima Warrior",
                        TankBusters = new List<uint>() {
                            10131 //Aetheroplasm
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7058,
                        Name = "The Ultima Beast",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            10162 //Demi Ultima
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheGhimlytDark,
                Name = "Dungeon: The Ghimlyt Dark",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7855,
                        Name = "Mark III-B Magitek Colossus",
                        TankBusters = new List<uint>() {
                            14190 //Jarring Blow
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            14195 //Ceruleum Vent
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7856,
                        Name = "Prometheus",
                        TankBusters = new List<uint>() {
                            13401 //Cermet Drill
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            13397 //Nitrospin
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7857,
                        Name = "Julia Quo Soranus",
                        TankBusters = new List<uint>() {
                            14121 //Dark Innocence
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            14119 //Artifical Plasma
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7858,
                        Name = "Annia Quo Soranus",
                        TankBusters = new List<uint>() {
                            14122 //Delta Trance
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.KuganeCastle,
                Name = "Dungeon: Kugane Castle",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6085,
                        Name = "Zuiko-Maru",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            7827 //Kenki Release
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheSirensongSea,
                Name = "Dungeon: The Sirensong Sea",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6072,
                        Name = "The Governor",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8028 //Bloodburst
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6074,
                        Name = "Lorelei",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8039 //Somber Melody
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SaintMociannesArboretumHard,
                Name = "Dungeon: Saint Mociannes Arboretum (Hard)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7659,
                        Name = "Nullchu",
                        TankBusters = new List<uint>() {
                            11848 //Vine Whip
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7660,
                        Name = "Lakhamu",
                        TankBusters = new List<uint>() {
                            12586 //Stone III
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            12588 //Tectonics
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7662,
                        Name = "Tokkapchi",
                        TankBusters = new List<uint>() {
                            12597 //Mudsling
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheSwallowsCompass,
                Name = "Dungeon: The Swallows Compass",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7200,
                        Name = "Otengu",
                        TankBusters = new List<uint>() {
                            11156 //Tengu Might
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            11157 //Tengu Clout
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7202,
                        Name = "Daidarabotchi",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            11171 //Mythmaker
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7203,
                        Name = "Qitian Dasheng",
                        TankBusters = new List<uint>() {
                            11174, //Short End
                            11527 //Short End
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            11178, //Mount Huaguo
                            11528 //Mount Huaguo
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7204,
                        Name = "Shadow Of The Sage",
                        TankBusters = new List<uint>() {
                            11174, //Short End
                            11527 //Short End
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            11178, //Mount Huaguo
                            11528 //Mount Huaguo
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheTempleOfTheFist,
                Name = "Dungeon: The Temple of The Fist",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6119,
                        Name = "Coeurl Sruti",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8150 //Electric Burst Sruti
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6120,
                        Name = "Coeurl Smriti",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8150 //Electric Burst Smriti
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6118,
                        Name = "Arbuda",
                        TankBusters = new List<uint>() {
                            8153 //Fourfold Shear
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 6117,
                        Name = "Ivon Coeurlfist",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8167 //Spirit Wave
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheBurn,
                Name = "Dungeon: The Burn",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7667,
                        Name = "Hedetet",
                        TankBusters = new List<uint>() {
                            12691 //Crystal Needle
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7669,
                        Name = "Defective Drone",
                        TankBusters = new List<uint>() {
                            11634 //Aetherochemical Coil
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            11635 //Aetherochemical Flame
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7672,
                        Name = "Mist Dragon",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            12619 //Rime Wreath
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion
            
            #region Stormblood: Eureka
            new Encounter {
                ZoneId = ZoneId.TheForbiddenLandEurekaAnemos,
                Name = "Eureka: Anemos",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7143,
                        Name = "Pazuzu",
                        TankBusters = new List<uint>() {
                            10399 //Camisado
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheForbiddenLandEurekaHydatos,
                Name = "Eureka: Hydatos",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7968,
                        Name = "Art",
                        TankBusters = new List<uint>() {
                            14644 //BA: Art - Tankbuster
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7970,
                        Name = "Owain",
                        TankBusters = new List<uint>() {
                            14661 //BA: Owain - Tankbuster
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7973,
                        Name = "Raiden",
                        TankBusters = new List<uint>() {
                            14459 //BA: Raiden - Tankbuster
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7976,
                        Name = "Absolute Virtue",
                        TankBusters = new List<uint>() {
                            14234 //BA: AV Tankbuster
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Stormblood: Normal Raids
            new Encounter {
                ZoneId = ZoneId.DeltascapeV10,
                Name = "Normal Raid: Deltascape V1.0 (O1N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5629,
                        Name = "Alte Roite",
                        TankBusters = new List<uint>() {
                            9175 //Twin Bolt
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            9180, //Roar
                            9179 //Charybdis
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.DeltascapeV20,
                Name = "Normal Raid: Deltascape V2.0 (O2N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5631,
                        Name = "Catastrophe",
                        TankBusters = new List<uint>() {
                            9487 //Evilsphere
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            9488 //Gravitational Wave
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SigmascapeV10,
                Name = "Normal Raid: Sigmascape V1.0 (O5N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7127,
                        Name = "Phantom Train",
                        TankBusters = new List<uint>() {
                            10403 //Doom Strike
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            10427 //Acid Rain                            
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SigmascapeV20,
                Name = "Normal Raid: Sigmascape V2.0 (O6N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7097,
                        Name = "Demon Chadarnook",
                        TankBusters = new List<uint>() {
                            10282 //Demonic Shear
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            10284 //Demonic Howl                            
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SigmascapeV30,
                Name = "Normal Raid: Sigmascape V3.0 (O7N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7107,
                        Name = "Guardian",
                        TankBusters = new List<uint>() {
                            10092 //Arm And Hammer
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            10094 //Diffractive Plasma                            
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SigmascapeV40,
                Name = "Normal Raid: Sigmascape V4.0 (O8N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7131,
                        Name = "Kefka",
                        TankBusters = new List<uint>() {
                            10542 //Hyper Drive
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            10541 //Ultima Upsurge                            
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlphascapeV10,
                Name = "Normal Raid: Alphascape V1.0 (O9N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7691,
                        Name = "Chaos",
                        TankBusters = new List<uint>() {
                            12623 //Chaotic Dispersion
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            12645, //Blaze
                            12646, //Tsunami
                            12647, //Cyclone
                            12648 //Earthquake
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlphascapeV20,
                Name = "Normal Raid: Alphascape V2.0 (O10N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7641,
                        Name = "Midgardsormr",
                        TankBusters = new List<uint>() {
                            12741 //Tail End
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7645,
                        Name = "Ancient Dragon",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            13295 //Rime Wreath
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlphascapeV30,
                Name = "Normal Raid: Alphascape V3.0 (O11N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7695,
                        Name = "Omega",
                        TankBusters = new List<uint>() {
                            12935 //Mustard Bomb
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            12934 //Atomic Ray
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlphascapeV40,
                Name = "Normal Raid: Alphascape V4.0 (O12N)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7635,
                        Name = "Omega",
                        TankBusters = new List<uint>() {
                            13089, //Optimized Blade Dance
                            13090 //Optimized Blade Dance
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            13073, //Laser Shower
                            13074 //Laser Shower
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7666,
                        Name = "Omega",
                        TankBusters = new List<uint>() {
                            13089, //Optimized Blade Dance
                            13090 //Optimized Blade Dance
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            13073, //Laser Shower
                            13074 //Laser Shower
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7695,
                        Name = "Omega",
                        TankBusters = new List<uint>() {
                            13089, //Optimized Blade Dance
                            13090 //Optimized Blade Dance
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            13073, //Laser Shower
                            13074 //Laser Shower
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7633,
                        Name = "Omega-M",
                        TankBusters = new List<uint>() {
                            13089, //Optimized Blade Dance
                            13090 //Optimized Blade Dance
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            13073, //Laser Shower
                            13074 //Laser Shower
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Stormblood: Savage Raids
            new Encounter {
                ZoneId = ZoneId.DeltascapeV10Savage,
                Name = "Normal Raid: Deltascape V1.0 (Savage) (O1S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5629,
                        Name = "Alte Roite",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            7892, //Roar
                            7891 //Charybdis
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.DeltascapeV20Savage,
                Name = "Normal Raid: Deltascape V2.0 (Savage) (O2S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5631,
                        Name = "Catastrophe",
                        TankBusters = new List<uint>() {
                            9073 //Evilsphere
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            9074 //Gravitational Wave
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.DeltascapeV30Savage,
                Name = "Normal Raid: Deltascape V3.0 (Savage) (O3S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5633,
                        Name = "Halicarnassus",
                        TankBusters = new List<uint>() {
                            8939 //Critical Hit
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8950 //Dimensional Wave
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SigmascapeV10Savage,
                Name = "Normal Raid: Sigmascape V1.0 (Savage) (O5S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7127,
                        Name = "Phantom Train",
                        TankBusters = new List<uint>() {
                            10417 //Doom Strike
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SigmascapeV20Savage,
                Name = "Normal Raid: Sigmascape V2.0 (Savage) (O6S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7097,
                        Name = "Demon Chadarnook",
                        TankBusters = new List<uint>() {
                            10281 //Demonic Shear
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SigmascapeV30Savage,
                Name = "Normal Raid: Sigmascape V3.0 (Savage) (O7S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7107,
                        Name = "Guardian",
                        TankBusters = new List<uint>() {
                            10121 //Arm And Hammer
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.SigmascapeV40Savage,
                Name = "Normal Raid: Sigmascape V4.0 (Savage) (O8S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7131,
                        Name = "Kefka",
                        TankBusters = new List<uint>() {
                            10472, //Hyper Drive
                            10514 //Hyper Drive
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlphascapeV10Savage,
                Name = "Normal Raid: Alphascape V1.0 (Savage) (O9S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7691,
                        Name = "Chaos",
                        TankBusters = new List<uint>() {
                            12656 //Chaotic Dispersion
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlphascapeV20Savage,
                Name = "Normal Raid: Alphascape V2.0 (Savage) (O10S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7641,
                        Name = "Midgardsormr",
                        TankBusters = new List<uint>() {
                            12714 //Tail End
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlphascapeV30Savage,
                Name = "Normal Raid: Alphascape V3.0 (Savage) (O11S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7695,
                        Name = "Omega",
                        TankBusters = new List<uint>() {
                            12909 //Mustard Bomb
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AlphascapeV40Savage,
                Name = "Normal Raid: Alphascape V4.0 (Savage) (O12S)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7635,
                        Name = "Omega",
                        TankBusters = new List<uint>() {
                            13136, //Solar Ray
                            13137, //Solar Ray
                            13131, //Optimized Blade Dance
                            13132 //Optimized Blade Dance
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7666,
                        Name = "Omega",
                        TankBusters = new List<uint>() {
                            13136, //Solar Ray
                            13137, //Solar Ray
                            13131, //Optimized Blade Dance
                            13132 //Optimized Blade Dance
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7695,
                        Name = "Omega",
                        TankBusters = new List<uint>() {
                            13136, //Solar Ray
                            13137, //Solar Ray
                            13131, //Optimized Blade Dance
                            13132 //Optimized Blade Dance
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 7633,
                        Name = "Omega-M",
                        TankBusters = new List<uint>() {
                            13136, //Solar Ray
                            13137, //Solar Ray
                            13131, //Optimized Blade Dance
                            13132 //Optimized Blade Dance
                            
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            13138 //Laser Shower
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Stormblood: Trials
            new Encounter {
                ZoneId = ZoneId.TheJadeStoa,
                Name = "Trial: The Jade Stoa",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7092,
                        Name = "Byakko",
                        TankBusters = new List<uint>() {
                            10797 //Heavenly Strike
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            10799 //Storm Pulse
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.Emanation,
                Name = "Trial: Emanation",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6385,
                        Name = "Lakshmi",
                        TankBusters = new List<uint>() {
                            9362, //Pull of Light
                            9363 //Pull of Light
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            9374 //Stotram
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheWreathOfSnakes,
                Name = "Trial: The Wreath of Snakes",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7922,
                        Name = "Seiryu",
                        TankBusters = new List<uint>() {
                            14333 //Infirm Soul
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            14334 //Fifth Element
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheRoyalMenagerie,
                Name = "Trial: The Royal Menagerie",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5641,
                        Name = "Shinryu: Left Wing",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8098 //Ice Storm
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.ThePoolOfTribute,
                Name = "Trial: The Pool of Tribute",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6221,
                        Name = "Susano",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8230 //Ukehi
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.HellsKier,
                Name = "Trial: Hells Kier",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7702,
                        Name = "Suzaku",
                        TankBusters = new List<uint>() {
                            12832, //Cremate
                            12849 //Phantom Flurry
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            12833, //Screams of the Damned
                            12852 //Shouthron Star
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.CastrumFluminis,
                Name = "Trial: Castrum Fluminis",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7225,
                        Name = "Tsukuyomi",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            11234, //Reprimand
                            11440, //Nightbloom
                            11261 //Dance of the Dead
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Stormblood: Extreme Trials
            new Encounter {
                ZoneId = ZoneId.TheJadeStoaExtreme,
                Name = "Trial: The Jade Stoa (Extreme)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7092,
                        Name = "Byakko",
                        TankBusters = new List<uint>() {
                            10202 //Heavenly Strike
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            10204 //Storm Pulse
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.Emanation,
                Name = "Trial: Emanation (Extreme)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6385,
                        Name = "Lakshmi",
                        TankBusters = new List<uint>() {
                            8542 //Pull of Light (Tank)
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheMinstrelsBalladShinryusDomain,
                Name = "Trial: The Minstrel's Ballad: Shinryu's Domain (Extreme)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 5641,
                        Name = "Shinryu",
                        TankBusters = new List<uint>() {
                            9803 //Tera Slash
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.ThePoolOfTributeExtreme,
                Name = "Trial: The Pool of Tribute (Extreme)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 6221,
                        Name = "Susano",
                        TankBusters = new List<uint>() {
                            8243 //Stormsplitter
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.HellsKierExtreme,
                Name = "Trial: Hells Kier (Extreme)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 7702,
                        Name = "Suzaku",
                        TankBusters = new List<uint>() {
                            13009 //Cremate
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Stormblood: Ultimate Raids
            new Encounter {
                ZoneId = ZoneId.TheUnendingCoilOfBahamutUltimate,
                Name = "Utlimate Raid: The Unending Coil of Bahamut (Ultimate)",
                Expansion = FfxivExpansion.Stormblood,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 2612,
                        Name = "Nael deus Darnus",
                        TankBusters = new List<uint>() {
                            9910 //Ravensbeak
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Shadowbringers: Alliance Raids
            new Encounter {
                ZoneId = ZoneId.TheCopiedFactory,
                Name = "Alliance Raid: The Copied Factory",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9141,
                        Name = "Serial-Jointed Command Model",
                        TankBusters = new List<uint>() {
                            18638 //Clanging Blow
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            18639 //Forceful Impact
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9142,
                        Name = "Serial-Jointed Command Model",
                        TankBusters = new List<uint>() {
                            18638 //Clanging Blow
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            18639 //Forceful Impact
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9143,
                        Name = "Hobbes",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18437 //Laser-Resistance Test
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9144,
                        Name = "Hobbes",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18437 //Laser-Resistance Test
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9145,
                        Name = "Hobbes",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18437 //Laser-Resistance Test
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9140,
                        Name = "Flight Unit",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18753 //360 Bombing Manuever
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9147,
                        Name = "Engels",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18261 //Diffuse Laser
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9153,
                        Name = "9S-Operated Walking Fortress",
                        TankBusters = new List<uint>() {
                            18677 //Neutralization
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18678 //Laser Saturation
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.ThePuppetsBunker,
                Name = "Alliance Raid: The Puppets Bunker",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9652,
                        Name = "Light Artillery Unit",
                        TankBusters = new List<uint>() {
                            21011 //Maneuver Martial Arm Target
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9364,
                        Name = "724P-operated superior flight unit (A-lpha)",
                        TankBusters = new List<uint>() {
                            20421 //Guided Missile
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20413 //Missile Command
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9617,
                        Name = "767P-operated superior flight unit (B-eta)",
                        TankBusters = new List<uint>() {
                            20421 //Guided Missile
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20413 //Missile Command
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9618,
                        Name = "772P-operated superior flight unit (C-hi)",
                        TankBusters = new List<uint>() {
                            20421 //Guided Missile
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20413 //Missile Command
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9650,
                        Name = "905P-Operated Heavy Artillery Unit",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20486 //Volt Array
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9646,
                        Name = "The Compound",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20920 //Mechanical Laceration
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9644,
                        Name = "Compound 2P",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20920 //Centrifugal Slice
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheTowerAtParadigmsBreach,
                Name = "Alliance Raid: The Tower At Paradigm's Breach",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9955,
                        Name = "Knave Of Hearts",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24245 //Roar
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9988,
                        Name = "Hansel",
                        TankBusters = new List<uint>() {
                            23672, //Crippling Blow
                            23673 //Crippling Blow
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23670, //Wail
                            23671, //Wail
                            23667, //Lamentation
                            23668 //Lamentation
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9989,
                        Name = "Gretel",
                        TankBusters = new List<uint>() {
                            23672, //Crippling Blow
                            23673 //Crippling Blow
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23670, //Wail
                            23671, //Wail
                            23667, //Lamentation
                            23668 //Lamentation
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9920,
                        Name = "Red Girl",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24594, //Cruelty
                            24595 //Cruelty
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9950,
                        Name = "Red Girl",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24594, //Cruelty
                            24595 //Cruelty
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9922,
                        Name = "Meng-Zi",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23558 //Universal Assault
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9921,
                        Name = "Xun-Zi",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23558 //Universal Assault
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9948,
                        Name = "False Idol",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23517 //Screaming Score
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9949,
                        Name = "Her Inflorescence",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23541 //Screaming Score
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Shadowbringers: Dungeons
            new Encounter {
                ZoneId = ZoneId.AkadaemiaAnyder,
                Name = "Dungeon: Akadaemia Anyder",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8235,
                        Name = "Cladoselache",
                        TankBusters = new List<uint>() {
                            15876 //Puncture
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15878 //Marine Mayhem
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8236,
                        Name = "Doliodus",
                        TankBusters = new List<uint>() {
                            15876 //Puncture
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15878 //Marine Mayhem
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8272,
                        Name = "Marquis Morbol",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15895 //Arbor Storm
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8273,
                        Name = "Quetzalcoatl",
                        TankBusters = new List<uint>() {
                            15907 //Shockbolt
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15908 //Thunderbolt
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.Amaurot,
                Name = "Dungeon: Amaurot",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8210,
                        Name = "Therion",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15587 //Shadow Wreck
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AnamnesisAnyder,
                Name = "Dungeon: Anamnesis Anyder",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9260,
                        Name = "Unknown",
                        TankBusters = new List<uint>() {
                            19305, //Fetid Fang
                            19314 //Fetid Fang
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19306 //Inscrutability
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9261,
                        Name = "Unknown",
                        TankBusters = new List<uint>() {
                            19305, //Fetid Fang
                            19314 //Fetid Fang
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19306 //Inscrutability
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9263,
                        Name = "Kyklops",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19288 //The Final Verse
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9264,
                        Name = "Rukshs Dheem",
                        TankBusters = new List<uint>() {
                            19340 //Bonebreaker
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.DohnMheg,
                Name = "Dungeon: Dohn Mheg",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8141,
                        Name = "Aenc Thon, Lord Of The Lingering Gaze",
                        TankBusters = new List<uint>() {
                            8857 //Candy Cane
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            7822 //Landsblood
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8143,
                        Name = "Griaule",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            8915 //Timber
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8146,
                        Name = "Aenc Thon, Lord Of The Lengthsome Gait",
                        TankBusters = new List<uint>() {
                            13732 //Crippling Blow                            
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            13708 //Virtuosic Cappriccio
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheHeroesGauntlet,
                Name = "Dungeon: The Heroes Gauntlet",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9505,
                        Name = "Spectral Thief",
                        TankBusters = new List<uint>() {
                            20427 //Spectral Dream
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20428 //Spectral Whirlwind
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9506,
                        Name = "Spectral Thief",
                        TankBusters = new List<uint>() {
                            20427 //Spectral Dream
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20428 //Spectral Whirlwind
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9508,
                        Name = "Spectral Necromancer",
                        TankBusters = new List<uint>() {
                            20318 //Twisted Touch
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20320 //Chaos Storm
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9511,
                        Name = "Spectral Berserker",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            21004 //Beastly Fury
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.HolminsterSwitch,
                Name = "Dungeon: Holmnister Switch",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8445,
                        Name = "Forgiven Dissonance",
                        TankBusters = new List<uint>() {
                            15812 //Pillory
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15813 //Path of Light
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8300,
                        Name = "Tesleen, The Forgiven",
                        TankBusters = new List<uint>() {
                            15823 //Tickler
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15824 //Bridle
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8301,
                        Name = "Philia",
                        TankBusters = new List<uint>() {
                            15831 //Head Crusher
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15832 //Scavenger
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.MalikahsWell,
                Name = "Dungeon: Malikahs Well",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8252,
                        Name = "Greater Armadillo",
                        TankBusters = new List<uint>() {
                            15589 //Stone Flail
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8250,
                        Name = "Amphibious Talos",
                        TankBusters = new List<uint>() {
                            15595 //Efface
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8249,
                        Name = "Storge",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15601 //Intestinal Crank
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.MatoyasRelict,
                Name = "Dungeon: Matoyas Relict",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9735,
                        Name = "Mudman",
                        TankBusters = new List<uint>() {
                            21631, //Hard Rock
                            21649 //Stone Age
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9741,
                        Name = "Mother Porxie",
                        TankBusters = new List<uint>() {
                            22801 //Minced Meat
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22803 //Tender Loin
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.MtGulg,
                Name = "Dungeon: Mt Gulg",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8260,
                        Name = "Forgiven Cruelty",
                        TankBusters = new List<uint>() {
                            15611 //Rake
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15612 //Cyclone Wing
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8261,
                        Name = "Forgiven Whimsy",
                        TankBusters = new List<uint>() {
                            15625 //Catechism
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15627 //Sacrament of Penance
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8262,
                        Name = "Forgiven Obscenity",
                        TankBusters = new List<uint>() {
                            15634 //Sforzando
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15636 //Orison Fortissimo
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.Paglthan,
                Name = "Dungeon: Paglthan",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10075,
                        Name = "Amhuluk",
                        TankBusters = new List<uint>() {
                            23630 //Critical Rip
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23629 //Electric Burst
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10076,
                        Name = "Magitek Core",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23710 //Defensive Reaction
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10077,
                        Name = "Lunar Bahamut",
                        TankBusters = new List<uint>() {
                            23384 //Flatten
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23383 //Giga Flare
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheQitanaRavel,
                Name = "Dungeon: The Qitana Ravel",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8231,
                        Name = "Lozatl",
                        TankBusters = new List<uint>() {
                            15497 //Stonefist
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15499 //Scorn
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8232,
                        Name = "Batsquatch",
                        TankBusters = new List<uint>() {
                            15505 //Ripper Fang
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15506, //Soundwave
                            15507, //Subsonics
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8233,
                        Name = "Eros",
                        TankBusters = new List<uint>() {
                            15513 //Rend
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15515 //Glossolalia
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheGrandCosmos,
                Name = "Dungeon: The Grand Cosmos",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9041,
                        Name = "Seeker Of Solitude",
                        TankBusters = new List<uint>(){
                            18281 //Shadowbolt
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18851 //Immortal Anathema
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9044,
                        Name = "Leannan Sith",
                        TankBusters = new List<uint>() {
                            18203 //Storm of Color
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18204 //Ode to Lost Love
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9046,
                        Name = "Lugus",
                        TankBusters = new List<uint>() {
                            18276 //Captive Bolt
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18277 //Culling Blade
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheTwinning,
                Name = "Dungeon: The Twinning",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8162,
                        Name = "Alpha Zaghnal",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15716 //Beastly Roar
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8165,
                        Name = "Mithridates",
                        TankBusters = new List<uint>() {
                            15853 //Thunder Beam
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8167,
                        Name = "The Tycoon",
                        TankBusters = new List<uint>() {
                            15867 //Rail Cannon
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15868 //Discharger
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Shadowbringers: Bozja
            new Encounter {
                ZoneId = ZoneId.TheBozjanSouthernFront,
                Name = "Bozja: The Bozjan Southern Front",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9407,
                        Name = "Red Comet",
                        TankBusters = new List<uint>() {
                            20588 //Choco Slash
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9439,
                        Name = "4th Legion Helldiver",
                        TankBusters = new List<uint>() {
                            20991 //Magitek Missiles
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20988 //MRV Missile
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9436,
                        Name = "Brionac",
                        TankBusters = new List<uint>() {
                            20957 //Electric Anvil
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9442,
                        Name = "Adrammelech",
                        TankBusters = new List<uint>() {
                            20373 //Flare
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20374 //Holy IV
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9452,
                        Name = "Dawon",
                        TankBusters = new List<uint>() {
                            20859 //Scratch
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20858 //Molting Plumage
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9409,
                        Name = "Lyon The Beast King",
                        TankBusters = new List<uint>(){
                            20852 //Twin Agonies
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.DelubrumReginae,
                Name = "Bozja: Delubrum Reginae",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9834,
                        Name = "Trinity Seeker",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23222 //Verdant Tempest
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9843,
                        Name = "Queen's Soldier",
                        TankBusters = new List<uint>() {
                            22537 //Rapid Sever Solider
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22536 //Blood and Bone Soldier
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9849,
                        Name = "Queen's Gunner",
                        TankBusters = new List<uint>() {
                            22545 //Shot in the Dark
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22544 //Queen's Shot
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9840,
                        Name = "Queen's Warrior",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22528 //Blood and Bone Warrior
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9838,
                        Name = "Queen's Knight",
                        TankBusters = new List<uint>() {
                            22523 //Rapid Sever Knight
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22522 //Blood and Bone Knight
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9755,
                        Name = "Bozjan Phantom",
                        TankBusters = new List<uint>() {
                            22537 //Excruciation
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22447 //Malediction of Agony
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9853,
                        Name = "Trinity Avowed",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22902 //Glory of Bozja
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9863,
                        Name = "The Queen",
                        TankBusters = new List<uint>() {
                            22981 //Cleansing Slash
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22984, //Empyrean Iniquity
                            22985 //Gods Save The Queen
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.DelubrumReginaeSavage,
                Name = "Bozja: Delubrum Reginae (Savage)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9834,
                        Name = "Trinity Seeker",
                        TankBusters = null,
                        SharedTankBusters = new List<uint>() {
                            23253 //Baleful Onslaught Buster
                        },
                        Aoes = new List<uint>() {
                            23251 //Verdant Tempest
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9843,
                        Name = "Queen's Soldier",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23062, //Guard Aoes
                            23048, //Guard Aoes
                            23093, //Guard Aoes
                            23075, //Guard Aoes
                            22612, //Blood and Bone
                            22593 //Blood and Bone
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9849,
                        Name = "Queen's Gunner",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23062, //Guard Aoes
                            23048, //Guard Aoes
                            23093, //Guard Aoes
                            23075, //Guard Aoes
                            22612, //Blood and Bone
                            22593 //Blood and Bone
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9840,
                        Name = "Queen's Warrior",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23062, //Guard Aoes
                            23048, //Guard Aoes
                            23093, //Guard Aoes
                            23075, //Guard Aoes
                            22577, //Blood and Bone
                            22561 //Blood and Bone
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9838,
                        Name = "Queen's Knight",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23062, //Guard Aoes
                            23048, //Guard Aoes
                            23093, //Guard Aoes
                            23075, //Guard Aoes
                            22577, //Blood and Bone
                            22561 //Blood and Bone
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9755,
                        Name = "Bozjan Phantom",
                        TankBusters = new List<uint>() {
                            22462 //Excruciation
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22461 //Malediction of Agony
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9853,
                        Name = "Trinity Avowed",
                        TankBusters = null,
                        SharedTankBusters = new List<uint>() {
                            22862 //Wrath of Bozja
                        },
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9863,
                        Name = "The Queen",
                        TankBusters = new List<uint>() {
                            23029 //Cleansing Slash
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.Zadnor,
                Name = "Bozja: Zadnor",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9945,
                        Name = "Blackburn",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23616 //Magitek Rays
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9931,
                        Name = "4th-Make Shemhazai",
                        TankBusters = new List<uint>() {
                            24096 //Devour Soul
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24094 //Blight
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9969,
                        Name = "Hedetet",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24085 //Crystal Needle
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9929,
                        Name = "Clibanarius",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23948 //Call Raze
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10064,
                        Name = "Hanbi",
                        TankBusters = new List<uint>() {
                            23470 //Camisado
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23470 //Dread Wind
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9941,
                        Name = "Hrodvitnir",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23602 //Glaciation
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9930,
                        Name = "4th-Make Belias",
                        TankBusters = new List<uint>() {
                            23962, //Fire IV
                            23961 //Fire
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10095,
                        Name = "Kampe",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23806 //Magnetic Field
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9973,
                        Name = "Alkonost",
                        TankBusters = new List<uint>() {
                            24123 //Bladed Beak
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24124, //Nihility's Song
                            24360 //Nihility's Song
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9693,
                        Name = "4th-Make Hashmal",
                        TankBusters = new List<uint>() {
                            23827 //Rock Cutter
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23828 //Ancient Quake IV
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9925,
                        Name = "Ayida",
                        TankBusters = new List<uint>() {
                            23985 //Serpent's Edge
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            23981 //Roar
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9384,
                        Name = "Sartauvoir The Inferno",
                        TankBusters = new List<uint>() {
                            24208 //Burning Blade
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24189, //Pyrokinesis
                            24199 //Mannatheihon Flame
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10212,
                        Name = "4th Legion Blackburn",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24338, //Magitek Rays
                            24352 //Augur Sanctified Quake III
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10004,
                        Name = "4th-Make Cuchulainn",
                        TankBusters = new List<uint>() {
                            23698 //Might of Malice
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23695 //Putrified Soul
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10006,
                        Name = "Dawon The Younger",
                        TankBusters = new List<uint>() {
                            24020 //Tooth and Talon
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10007,
                        Name = "The Diablo Armament",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            23750, //Aetheric Explosion
                            23731 //Aetheric Boom Raidwide
                        },
                        BigAoes = new List<uint>() {
                            23735 //Void Systems Overload
                        }
                    }
                }
            },
            #endregion

            #region Shadowbringers: Normal Raids
            new Encounter {
                ZoneId = ZoneId.EdensGateResurrection,
                Name = "Normal Raid: Eden's Gate - Resurrection (E1N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8345,
                        Name = "Eden Prime",
                        TankBusters = new List<uint>() {
                            15777 //Spear of Paradise
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15764, //Eden's Gravity
                            15780, //Fragor Maximus
                            15772 //Dimensional Shift
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensGateDescent,
                Name = "Normal Raid: Eden's Gate - Descent (E2N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8382,
                        Name = "Voidwalker",
                        TankBusters = new List<uint>() {
                            15949 //Shadowflame
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15981 //Entropy
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensGateInundation,
                Name = "Normal Raid: Eden's Gate - Inundation (E3N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8486,
                        Name = "Leviathan",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16324, //Tidal Roar
                            16340 //Tsunami
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensGateSepulture,
                Name = "Normal Raid: Eden's Gate - Sepulture (E4N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8350,
                        Name = "Titan",
                        TankBusters = new List<uint>() {
                            16633 //Stonecrusher
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            16631 //Voice of the Land
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensVerseFulmination,
                Name = "Normal Raid: Eden's Verse - Fulmination (E5N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9281,
                        Name = "Ramuh",
                        TankBusters = new List<uint>() {
                            19363 //Crippling Blow
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19352, //Judgement Volts
                            19354 //Judgement Volts
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensVerseFuror,
                Name = "Normal Raid: Eden's Verse - Furor (E6N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9287,
                        Name = "Garuda",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19415 //Superstorm
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9288,
                        Name = "Ifrit",
                        TankBusters = new List<uint>() {
                            19437 //Instant Incineration
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            19441 //Inferno Howl
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9289,
                        Name = "Raktapaksa",
                        TankBusters = new List<uint>() {
                            19437 //Instant Incineration
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            19441 //Inferno Howl
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensVerseIconoclasm,
                Name = "Normal Raid: Eden's Verse - Iconoclasm (E7N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9298,
                        Name = "The Idol of Darkness",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19538 //Empty Wave
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensVerseRefulgence,
                Name = "Normal Raid: Eden's Verse - Refulgence (E8N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9353,
                        Name = "Shiva",
                        TankBusters = new List<uint>() {
                            19930 //Double Slap
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19927, //Absolute Zero
                            19937 //Diamond Frost
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensPromiseUmbra,
                Name = "Normal Raid: Eden's Promise - Umbra (E9N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9764,
                        Name = "Cloud of Darkness",
                        TankBusters = new List<uint>() {
                            21995 //Zero-Form Particle Beam
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            21997, //Ground-Razing Particle Beam
                            20422, //Empty Plane
                            20386, //Obscure Woods
                            20821 //Deluge of Darkness
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensPromiseLitany,
                Name = "Normal Raid: Eden's Promise - Litany (E10N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9769,
                        Name = "Shadowkeeper",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            22245 //Deepshadow Nova
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensPromiseAnamorphosis,
                Name = "Normal Raid: Eden's Promise - Anamorphosis (E11N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9707,
                        Name = "Fatebreaker",
                        TankBusters = new List<uint>() {
                            22094 //Powder Mark
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22096 //Burndished Glory
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensPromiseEternity,
                Name = "Normal Raid: Eden's Promise - Eternity (E12N)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9813,
                        Name = "Eden's Promise",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22642, //Maleficium
                            22628 //Diamond Dust
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Shadowbringers: Savage Raids
            new Encounter {
                ZoneId = ZoneId.EdensGateResurrectionSavage,
                Name = "Normal Raid: Eden's Gate - Resurrection (Savage) (E1S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8345,
                        Name = "Eden Prime",
                        TankBusters = new List<uint>() {
                            15752 //Spear of Paradise
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15728, //Eden's Gravity
                            15755, //Fragor Maximus
                            15743 //Dimensional Shift
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensGateDescentSavage,
                Name = "Normal Raid: Eden's Gate - Descent (Savage) (E2S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8382,
                        Name = "Voidwalker",
                        TankBusters = new List<uint>() {
                            15969, //Shadowflame
                            15970
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15983, //Entropy
                            15985 //Quetus
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensGateInundationSavage,
                Name = "Normal Raid: Eden's Gate - Inundation (Savage) (E3S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8486,
                        Name = "Leviathan",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16348, //Tidal Roar
                            16350 //Tidal Rage
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensGateSepultureSavage,
                Name = "Normal Raid: Eden's Gate - Sepulture (Savage) (E4S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8350,
                        Name = "Titan",
                        TankBusters = new List<uint>() {
                            16662 //Stonecrusher
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            16660 //Voice of the Land
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8349,
                        Name = "Titan Maximum",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16682 //Tumult 
                        },
                        BigAoes = new List<uint>() {
                            16676 //Earthen Fury
                        }
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensVerseFulminationSavage,
                Name = "Normal Raid: Eden's Verse - Fulmination (Savage) (E5S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9281,
                        Name = "Ramuh",
                        TankBusters = new List<uint>() {
                            19402 //Crippling Blow
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19381 //Judgement Volts
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensVerseFurorSavage,
                Name = "Normal Raid: Eden's Verse - Furor (Savage) (E6S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9287,
                        Name = "Garuda",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19447 //Superstorm
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9288,
                        Name = "Ifrit",
                        TankBusters = new List<uint>() {
                            19470 //Instant Incineration
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            19476 //Inferno Howl
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9289,
                        Name = "Raktapaksa",
                        TankBusters = new List<uint>() {
                            19470 //Instant Incineration
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            19476 //Inferno Howl
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensVerseIconoclasmSavage,
                Name = "Normal Raid: Eden's Verse - Iconoclasm (Savage) (E7S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9298,
                        Name = "The Idol of Darkness",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19594, //Empty Wave
                            19595, //Empty Flood
                            19596, //Empty Flood
                            20053, //Empty Flood
                            20054 //Empty Flood
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensVerseRefulgenceSavage,
                Name = "Normal Raid: Eden's Verse - Refulgence (Savage) (E8S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9353,
                        Name = "Shiva",
                        TankBusters = new List<uint>() {
                            19813 //Double Slap
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19916, //Absolute Zero
                            19820, //Diamond Frost
                            19827, //Light Rampart
                            19836 //Wyrm's Lament
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensPromiseUmbraSavage,
                Name = "Normal Raid: Eden's Promise - Umbra (Savage) (E9S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9764,
                        Name = "Cloud of Darkness",
                        TankBusters = new List<uint>() {
                            22051 //Zero-Form Devouring Dark
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22053, //Ground-Razing Particle Beam
                            21999, //Empty Plane
                            21998 //Obscure Woods
                        },
                        BigAoes = new List<uint>() {
                            22001 //Deluge of Darkness
                        }
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensPromiseLitanySavage,
                Name = "Normal Raid: Eden's Promise - Litany (Savage) (E10S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9769,
                        Name = "Shadowkeeper",
                        TankBusters = new List<uint>() {
                            23466 //Umbra Smash
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>(){
                            22334 //Deepshadow Nova
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensPromiseAnamorphosisSavage,
                Name = "Normal Raid: Eden's Promise - Anamorphosis (Savage) (E11S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9707,
                        Name = "Fatebreaker",
                        TankBusters = new List<uint>() {
                            22178 //Powder Mark
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = new List<uint>() {
                            22180 //Burnished Glory
                        }
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.EdensPromiseEternitySavage,
                Name = "Normal Raid: Eden's Promise - Eternity (Savage) (E12S)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9813,
                        Name = "Eden's Promise",
                        TankBusters = new List<uint>() {
                            22697 //Formless Judgement
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            22696, //Maleficium
                            22628 //Diamond Dust
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9832,
                        Name = "Oracle of Darkness",
                        TankBusters = new List<uint>() {
                            22727 //Black Halo
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() { 
                            22768 //Shockwave Pulsar
                        },
                        BigAoes = new List<uint>() {
                            22752, //Basic Relativity
                            22753, //Intermediate Relativity
                            22754, //Advanced Relativity
                            22755 //Terminal Relativity
                        }
                    }
                }
            },
            #endregion

            #region Shadowbringers: Trials
            new Encounter {
                ZoneId = ZoneId.TheCloudDeck,
                Name = "Trial: The Cloud Deck",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9953,
                        Name = "The Diamond Weapon",
                        TankBusters = new List<uint>() {
                            24536 //Auri Doomstead
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24487, //Diamond Rain
                            24535 //Outrage
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.CastrumMarinum,
                Name = "Trial: Castrum Marinum",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9696,
                        Name = "The Emerald Weapon",
                        TankBusters = new List<uint>() {
                            21844 //Emerald Shot
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            21845, //Optimized Ultima
                            21846, //Optimized Ultima
                            23311 //Optimized Ultima
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheDyingGasp,
                Name = "Trial: The Dying Gasp",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8352,
                        Name = "Hades",
                        TankBusters = new List<uint>() {
                            16728 //Ravenous Assault
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16768 //Titanomachy
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheCrownOfTheImmaculate,
                Name = "Trial: The Crown of the Immaculate",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8297,
                        Name = "Innocence",
                        TankBusters = new List<uint>() {
                            16035 //Righteous Bolt
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16026, //Realmrazer
                            16106 //Shadowreaver
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8298,
                        Name = "Innocence",
                        TankBusters = new List<uint>() {
                            16035 //Righteous Bolt
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16026, //Realmrazer
                            16106 //Shadowreaver
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8353,
                        Name = "Innocence",
                        TankBusters = new List<uint>() {
                            16035 //Righteous Bolt
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16026, //Realmrazer
                            16106 //Shadowreaver
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.CinderDrift,
                Name = "Trial: Cinder Drift",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9250,
                        Name = "The Ruby Weapon",
                        TankBusters = new List<uint>() {
                            19143 //Stamp
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19112, //Optimized Ultima
                            19144 //Outrage
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9254,
                        Name = "Raven's Image",
                        TankBusters = new List<uint>() {
                            19135 //Ruby Claw
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheDancingPlague,
                Name = "Trial: The Dancing Plague",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8361,
                        Name = "Titania",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15708 //Bright Sabbath
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9491,
                        Name = "Titania",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15708 //Bright Sabbath
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheSeatOfSacrifice,
                Name = "Trial: The Seat of Sacrifice",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9642,
                        Name = "Warrior of Light",
                        TankBusters = new List<uint>() {
                            20264 //Bitter End
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20265 //Elddragon Dive
                        },
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Shadowbringers: Extreme Trials
            new Encounter {
                ZoneId = ZoneId.TheCloudDeckExtreme,
                Name = "Trial: The Cloud Deck (Extreme)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9953,
                        Name = "The Diamond Weapon",
                        TankBusters = new List<uint>() {
                            24509 //Auri Doomstead
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            24487, //Diamond Rain
                            24508 //Outrage
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.CastrumMarinumExtreme,
                Name = "Trial: Castrum Marinum (Extreme)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9696,
                        Name = "The Emerald Weapon",
                        TankBusters = new List<uint>() {
                            21936 //Emerald Shot
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            21937, //Optimized Ultima
                            23312 //Optimized Ultima
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheMinstrelsBalladHadessElegy,
                Name = "Trial: The Minstrel's Ballad: Hades's Elegey (Extreme)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8352,
                        Name = "Hades",
                        TankBusters = new List<uint>() {
                            18342, //Ravenous Assault
                            18422 //Quadrastrike 2
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18360, //Quake III
                            18419, //Gigantomachy
                            18420, //Quadrastrike 1
                            18422 //Quadrastrike 3
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9186,
                        Name = "Lahabrea's and Igeyorhm's Shades",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18367 //Annihilation
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9182,
                        Name = "Igeyorhm's Shade",
                        TankBusters = new List<uint>() {
                            18371 //Blizzard IV
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9181,
                        Name = "Lahabrea's Shade",
                        TankBusters = new List<uint>() {
                            18370 //Fire IV
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9183,
                        Name = "Ascian Prime's Shade",
                        TankBusters = new List<uint>(){
                            18385 //Height of Chaos
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18384 //Shadow Flare
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheCrownOfTheImmaculateExtreme,
                Name = "Trial: The Crown of the Immaculate (Extreme)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8297,
                        Name = "Innocence",
                        TankBusters = new List<uint>() {
                            16077, //Righteous Bolt
                            16073 //Holy Sword
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16106 //Shadowreaver
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8298,
                        Name = "Innocence",
                        TankBusters = new List<uint>() {
                            16077, //Righteous Bolt
                            16073 //Holy Sword
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16106 //Shadowreaver
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 8353,
                        Name = "Innocence",
                        TankBusters = new List<uint>() {
                            16077, //Righteous Bolt
                            16073 //Holy Sword
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            16106 //Shadowreaver
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.CinderDriftExtreme,
                Name = "Trial: Cinder Drift (Extreme)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9250,
                        Name = "The Ruby Weapon",
                        TankBusters = new List<uint>() {
                            19203, //Stamp
                            19126 //Mark II Magitek Comet
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            19134, //Optimized Ultima
                            19204 //Outrage
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9254,
                        Name = "Raven's Image",
                        TankBusters = new List<uint>() {
                            19199 //Ruby Claw
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheDancingPlagueExtreme,
                Name = "Trial: The Dancing Plague (Extreme)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 8361,
                        Name = "Titania",
                        TankBusters = new List<uint>() {
                            15660, //Fae Light
                            15670 //Hard Swipe
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15691 //Bright Sabbath
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9491,
                        Name = "Titania",
                        TankBusters = new List<uint>() {
                            15660, //Fae Light
                            15670 //Hard Swipe
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            15691 //Bright Sabbath
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheSeatOfSacrificeExtreme,
                Name = "Trial: The Seat of Sacrifice (Extreme)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9642,
                        Name = "Warrior of Light",
                        TankBusters = new List<uint>() {
                            20234 //Bitter End
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            20235 //Elddragon Dive
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9470,
                        Name = "Spectral Warrior",
                        TankBusters = new List<uint>() {
                            20820, //Fatal Cleave or Blade of Shadow
                            20823 //Fatal Cleave or Blade of Shadow
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9468,
                        Name = "Spectral Warrior",
                        TankBusters = new List<uint>() {
                            20820, //Fatal Cleave or Blade of Shadow
                            20823 //Fatal Cleave or Blade of Shadow
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.MemoriaMiseraExtreme,
                Name = "Trial: Memoria Misera (Extreme)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy>() {
                    new Enemy {
                        Id = 9341,
                        Name = "Varis Yae Galvus",
                        TankBusters = new List<uint>() {
                            19696 //Citius
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Shadowbringers: Ultimate Raids

            new Encounter {
                ZoneId = ZoneId.TheEpicOfAlexanderUltimate,
                Name = "Ultimate Raid: The Epic of Alexender (Ultimate)",
                Expansion = FfxivExpansion.Shadowbringers,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 9211,
                        Name = "Living Liquid",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18470 //Cascade
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9218,
                        Name = "Cruise Chaser",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint>() {
                            18882 //Whirlwind
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9216,
                        Name = "Brute Justice",
                        TankBusters = new List<uint>() {
                            18503 //Double Rocket Punch
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 9220,
                        Name = "Alexander Prime",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = new List<uint>() {
                            19075 //Mega Holy
                        }
                    },
                    new Enemy {
                        Id = 9042,
                        Name = "Perfect Alexander",
                        TankBusters = new List<uint>() {
                            18577 //Ordained Punishment
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    }
                }
            },
            #endregion

            #region Endwalker: Dungeons

            new Encounter {
                ZoneId = ZoneId.TheTowerOfZot,
                Name = "Dungeon: The Tower of Zot",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10256,
                        Name = "Minduruva",
                        TankBusters = new List<uint> {
                            25257, //Isitva Siddhi
                            25290 //Manusya Bio
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10257,
                        Name = "Sanduruva",
                        TankBusters = new List<uint> {
                            25257, //Isitva Siddhi
                            25280 //Isitva Siddhi
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10259,
                        Name = "Cinduruva",
                        TankBusters = new List<uint> {
                            25257 //Isitva Siddhi
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25273, //Samsara
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheTowerOfBabil,
                Name = "Dungeon: The Tower of Babil",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10281,
                        Name = "Lugae",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25338 //Thermal Suppression
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10282,
                        Name = "Lugae",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25338 //Thermal Suppression
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10285,
                        Name = "Anima",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25344 //Mega Graviton
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10288,
                        Name = "Anima",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25344 //Mega Graviton
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.Vanaspati,
                Name = "Dungeon: Vanaspati",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10717,
                        Name = "Terminus Snatcher",
                        TankBusters = new List<uint> {
                            25141 //Last Grasp
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25144 //Note of Despair
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 11049,
                        Name = "Terminus Snatcher",
                        TankBusters = new List<uint> {
                            25141 //Last Grasp
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25144 //Note of Despair
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10718,
                        Name = "Terminus Wrecker",
                        TankBusters = new List<uint> {
                            25154 //Total Wreck
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25153 //Meaningless Destruction
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 11052,
                        Name = "Terminus Wrecker",
                        TankBusters = new List<uint> {
                            25154 //Total Wreck
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25153 //Meaningless Destruction
                        },
                        BigAoes = null

                    },
                    new Enemy {
                        Id = 10719,
                        Name = "Svarbhanu",
                        TankBusters = new List<uint> {
                            25171 //Gnashing of Teeth
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25170 //Flames of Decay
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.KtisisHyperboreia,
                Name = "Dungeon: Ktisis Hyperboreia",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10396,
                        Name = "Lyssa",
                        TankBusters = new List<uint> {
                            25182 //Skull Dasher
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25181 //Frigid Stomp
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10398,
                        Name = "Ladon Lord",
                        TankBusters = new List<uint> {
                            25743 //Scratch
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25741 //Intimidation
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10363,
                        Name = "Hermes",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25886 //Trismegistos
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10399,
                        Name = "Hermes",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25886 //Trismegistos
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheAitiascope,
                Name = "Dungeon: The Aitiascope",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10290,
                        Name = "Livia the Undeterred",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25672 //Frustration
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10292,
                        Name = "Rhitahtyn the Unshakable",
                        TankBusters = new List<uint> {
                            25686 //Anvil of Tartarus
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25685 //Tartarean Impact
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10293,
                        Name = "Amon the Undying",
                        TankBusters = new List<uint> {
                            25700 //Dark Forte
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25701 //Entr'acte
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.Smileton,
                Name = "Dungeon: Smileton",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10331,
                        Name = "Face",
                        TankBusters = new List<uint> {
                            26434 //Heart on Fire IV
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26435 //Temper's Flare
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10333,
                        Name = "Frameworker",
                        TankBusters = new List<uint> {
                            26436 //Steel Beam
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26437 //Circular Saw
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10336,
                        Name = "The Big Cheese",
                        TankBusters = new List<uint> {
                            26449 //Piercing Missile
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26450, //Violent Discharge
                            26451,
                            26452
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheDeadEnds,
                Name = "Dungeon: The Dead Ends",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10313,
                        Name = "Caustic Grebuloff",
                        TankBusters = new List<uint> {
                            25920 //Pox Flail
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25916, //Miasmata
                            25921, //Blighted Water
                            25923 //Befoulment
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10315,
                        Name = "Peacekeeper",
                        TankBusters = new List<uint> {
                            28359, //Infantry Deterrent 
                            25935 //Elimination
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25936, //Decimation
                            28351, //Order to Fire
                            25925 //No Future
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10316,
                        Name = "Ra-la",
                        TankBusters = new List<uint> {
                            25949 //Pity
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            25950, //Warm Glow
                            25945, //Benevolence
                            25947 //Still Embrace
                        },
                        BigAoes = null
                    }
                }
            },

            #endregion

            #region Endwalker: Trials

            new Encounter {
                ZoneId = ZoneId.TheDarkInside,
                Name = "Trial: Zodiark",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10456,
                        Name = "Zodiark",
                        TankBusters = new List<uint> {
                            27490 //Ania
                        },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null,
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheMothercrystal,
                Name = "Trial: Hydaelyn",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10453,
                        Name = "Hydaelyn",
                        TankBusters = null,
                        SharedTankBusters = new List<uint> {
                            26070 //Mousa's Scorn
                        },
                        Aoes = new List<uint> {
                            26071, //Heros's Radiance
                            26072, //Magos's Radiance
                            26043, //Exodus
                            26064, //Radiant Halo
                            26037, //Echoes
                            26038, //Echoes
                            26824 //Shining Saber
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheFinalDay,
                Name = "Trial: Endsinger",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10448,
                        Name = "The Endsinger",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26156, //Elegeia
                            26242 //Elegeia Unforgotten
                        },
                        BigAoes = null
                    }
                }
            },

            #endregion

            #region Endwalker: Trials (Extreme)

            new Encounter {
                ZoneId = ZoneId.TheMinstrelsBalladZodiarksFall,
                Name = "Trial: Zodiark (Extreme)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10456,
                        Name = "Zodiark",
                        TankBusters = new List<uint> {
                            26607 //Ania
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26611, //Styx
                            26608 //Phobos
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.TheMinstrelsBalladHydaelynsCall,
                Name = "Trial: Hydaelyn (Extreme)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10453,
                        Name = "Hydaelyn",
                        TankBusters = null,
                        SharedTankBusters = new List<uint> {
                            26048, //Mousa's Scorn 
                            26040 //Dichroic Spectrum
                        },
                        Aoes = new List<uint> {
                            26036, //Radiant Halo
                            26049, //Heros's Radiance
                            26050, //Magos's Radiance
                            27477, //Pummel
                            26021, //Halo
                            27476 //Calculated Combustion
                        },
                        BigAoes = null,
                    }
                }
            },

            #endregion

            #region Endwalker: Normal Raids

            new Encounter {
                ZoneId = ZoneId.AsphodelosTheFirstCircle,
                Name = "Normal Raid: First Circle (P1N)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10345,
                        Name = "Erichthonios",
                        TankBusters = new List<uint> {
                            26099 //Heavy Hand
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26100, //Warder's Wrath
                            26089, //Shining Cells
                            26090 //Slam Shut
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10576,
                        Name = "Erichthonios",
                        TankBusters = new List<uint> {
                            26099 //Heavy Hand
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26100, //Warder's Wrath
                            26089, //Shining Cells
                            26090 //Slam Shut
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AsphodelosTheSecondCircle,
                Name = "Normal Raid: Second Circle (P2N)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10348,
                        Name = "Hippokampos",
                        TankBusters = null,
                        SharedTankBusters = new List<uint> {
                            26638 //Doubled Impact
                        },
                        Aoes = new List<uint> {
                            26639, //Murky Depths
                            26614, //Sewage Deluge
                            26625,
                            26632
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AsphodelosTheThirdCircle,
                Name = "Normal Raid: Third Circle (P3N)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10720,
                        Name = "Phoinix",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26296, //Scorched Exaltation
                            26281 //Dead Rebirth
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AsphodelosTheFourthCircle,
                Name = "Normal Raid: Fourth Circle (P4N)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10742,
                        Name = "Hesperos",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            27217, //Decollation
                            27200 //Bloodrake
                        },
                        BigAoes = null
                    }
                }
            },

            #endregion

            #region Endwalker: Normal Raids (Savage)

            new Encounter {
                ZoneId = ZoneId.AsphodelosTheFirstCircleSavage,
                Name = "Normal Raid: First Circle (Savage) (P1S)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10345,
                        Name = "Erichthonios",
                        TankBusters = new List<uint> {
                            26153 //Heavy Hand
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26154, //Warder's Wrath
                            26134, //Shining Cells
                            26135 //Slam Shut
                        },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10576,
                        Name = "Erichthonios",
                        TankBusters = new List<uint> {
                            26153 //Heavy Hand
                        },
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26154, //Warder's Wrath
                            26134, //Shining Cells
                            26135 //Slam Shut
                        },
                        BigAoes = null
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AsphodelosTheSecondCircleSavage,
                Name = "Normal Raid: Second Circle (Savage) (P2S)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10348,
                        Name = "Hippokampos",
                        TankBusters = null,
                        SharedTankBusters = new List<uint> {
                            26674 //Doubled Impact
                        },
                        Aoes = new List<uint> {
                            26675 //Murky Depths
                        },
                        BigAoes = new List<uint> {
                            26640 //Sewage Deluge
                        }
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AsphodelosTheThirdCircleSavage,
                Name = "Normal Raid: Third Circle (Savage) (P3S)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10720,
                        Name = "Phoinix",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> {
                            26374 //Scorched Exaltation
                        },
                        BigAoes = new List<uint> {
                            26340, //Dead Rebirth
                            26352 //Firestorms of Asphodelos
                        }
                    }
                }
            },
            new Encounter {
                ZoneId = ZoneId.AsphodelosTheFourthCircleSavage,
                Name = "Normal Raid: Fourth Circle (Savage) (P4S)",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10742,
                        Name = "Hesperos",
                        TankBusters = new List<uint> {
                            27144, //Elegant Evisceration
                            27174, //Not sure...
                            27175, //Not sure...
                            27179 //Heart Stake
                        },
                        SharedTankBusters = new List<uint> {
                            28280 //Not sure...
                        },
                        Aoes = new List<uint> {
                            27145, //Decollation
                            27096, //Bloodrake
                            27181 //Searing Stream
                        },
                        BigAoes = new List<uint> {
                            27180 //Ultimate Impulse
                        }
                    }
                }
            }
            #endregion
        };

        internal class Enemy
        {
            internal uint Id { get; set; }
            internal string Name { get; set; }
            internal List<uint> TankBusters { get; set; }
            internal List<uint> SharedTankBusters { get; set; }
            internal List<uint> Aoes { get; set; }
            internal List<uint> BigAoes { get; set; }
        }

        private class Encounter
        {
            internal ushort ZoneId { get; set; }
            internal string Name { get; set; }
            internal FfxivExpansion Expansion { get; set; }
            internal List<Enemy> Enemies { get; set; }
        }

        private static class ZoneId
        {
            public const ushort
                ABloodyReunion = 560,
                AFrostyReception = 1010,
                APathUnveiled = 1015,
                ARelicRebornTheChimera = 368,
                ARelicRebornTheHydra = 369,
                ARequiemForHeroes = 830,
                ASleepDisturbed = 914,
                ASpectacleForTheAges = 533,
                AccrueEnmityFromMultipleTargets = 540,
                AirForceOne = 832,
                AkadaemiaAnyder = 841,
                AlaMhigo = 689,
                AlexanderTheArmOfTheFather = 444,
                AlexanderTheArmOfTheFatherSavage = 451,
                AlexanderTheArmOfTheSon = 522,
                AlexanderTheArmOfTheSonSavage = 531,
                AlexanderTheBreathOfTheCreator = 581,
                AlexanderTheBreathOfTheCreatorSavage = 585,
                AlexanderTheBurdenOfTheFather = 445,
                AlexanderTheBurdenOfTheFatherSavage = 452,
                AlexanderTheBurdenOfTheSon = 523,
                AlexanderTheBurdenOfTheSonSavage = 532,
                AlexanderTheCuffOfTheFather = 443,
                AlexanderTheCuffOfTheFatherSavage = 450,
                AlexanderTheCuffOfTheSon = 521,
                AlexanderTheCuffOfTheSonSavage = 530,
                AlexanderTheEyesOfTheCreator = 580,
                AlexanderTheEyesOfTheCreatorSavage = 584,
                AlexanderTheFistOfTheFather = 442,
                AlexanderTheFistOfTheFatherSavage = 449,
                AlexanderTheFistOfTheSon = 520,
                AlexanderTheFistOfTheSonSavage = 529,
                AlexanderTheHeartOfTheCreator = 582,
                AlexanderTheHeartOfTheCreatorSavage = 586,
                AlexanderTheSoulOfTheCreator = 583,
                AlexanderTheSoulOfTheCreatorSavage = 587,
                AllsWellThatEndsInTheWell = 220,
                AllsWellThatStartsWell = 796,
                AlphascapeV10 = 798,
                AlphascapeV10Savage = 802,
                AlphascapeV20 = 799,
                AlphascapeV20Savage = 803,
                AlphascapeV30 = 800,
                AlphascapeV30Savage = 804,
                AlphascapeV40 = 801,
                AlphascapeV40Savage = 805,
                Amaurot = 838,
                AmdaporKeep = 167,
                AmdaporKeepHard = 189,
                AmhAraeng = 815,
                AnamnesisAnyder = 898,
                AnnoyTheVoid = 222,
                AsTheHeartBids = 894,
                AsTheHeavensBurn = 1012,
                AsphodelosTheFirstCircle = 1002,
                AsphodelosTheFirstCircleSavage = 1003,
                AsphodelosTheFourthCircle = 1008,
                AsphodelosTheFourthCircleSavage = 1009,
                AsphodelosTheSecondCircle = 1004,
                AsphodelosTheSecondCircleSavage = 1005,
                AsphodelosTheThirdCircle = 1006,
                AsphodelosTheThirdCircleSavage = 1007,
                AssistAlliesInDefeatingATarget = 544,
                Astragalos = 729,
                AvoidAreaOfEffectAttacks = 537,
                AzysLla = 402,
                BaelsarsWall = 615,
                BardamsMettle = 623,
                BasicTrainingEnemyParties = 214,
                BasicTrainingEnemyStrongholds = 215,
                BattleInTheBigKeep = 396,
                BattleOnTheBigBridge = 366,
                BloodOnTheDeck = 708,
                BrayfloxsLongstop = 158,
                BrayfloxsLongstopHard = 362,
                CapeWestwind = 332,
                CastrumAbania = 661,
                CastrumFluminis = 778,
                CastrumMarinum = 934,
                CastrumMarinumDrydocks = 967,
                CastrumMarinumExtreme = 935,
                CastrumMeridianum = 217,
                CentralShroud = 148,
                CentralThanalan = 141,
                ChocoboRaceCostaDelSol = 389,
                ChocoboRaceSagoliiRoad = 390,
                ChocoboRaceTranquilPaths = 391,
                ChocoboRaceTutorial = 417,
                CinderDrift = 897,
                CinderDriftExtreme = 912,
                CoerthasCentralHighlands = 155,
                CoerthasWesternHighlands = 397,
                ComingClean = 860,
                ContainmentBayP1T6 = 576,
                ContainmentBayP1T6Extreme = 577,
                ContainmentBayS1T7 = 517,
                ContainmentBayS1T7Extreme = 524,
                ContainmentBayZ1T9 = 637,
                ContainmentBayZ1T9Extreme = 638,
                CopperbellMines = 161,
                CopperbellMinesHard = 349,
                CuriousGorgeMeetsHisMatch = 717,
                CuttersCry = 170,
                DarkAsTheNightSky = 713,
                DeathUntoDawn = 977,
                DefeatAnOccupiedTarget = 545,
                DeltascapeV10 = 691,
                DeltascapeV10Savage = 695,
                DeltascapeV20 = 692,
                DeltascapeV20Savage = 696,
                DeltascapeV30 = 693,
                DeltascapeV30Savage = 697,
                DeltascapeV40 = 694,
                DeltascapeV40Savage = 698,
                DelubrumReginae = 936,
                DelubrumReginaeSavage = 937,
                DohnMheg = 821,
                DomaCastle = 660,
                DragonSound = 714,
                DunScaith = 627,
                DzemaelDarkhold = 171,
                EastShroud = 152,
                EasternLaNoscea = 137,
                EasternThanalan = 145,
                EdensGateDescent = 850,
                EdensGateDescentSavage = 854,
                EdensGateInundation = 851,
                EdensGateInundationSavage = 855,
                EdensGateResurrection = 849,
                EdensGateResurrectionSavage = 853,
                EdensGateSepulture = 852,
                EdensGateSepultureSavage = 856,
                EdensPromiseAnamorphosis = 944,
                EdensPromiseAnamorphosisSavage = 948,
                EdensPromiseEternity = 945,
                EdensPromiseEternitySavage = 949,
                EdensPromiseLitany = 943,
                EdensPromiseLitanySavage = 947,
                EdensPromiseUmbra = 942,
                EdensPromiseUmbraSavage = 946,
                EdensVerseFulmination = 902,
                EdensVerseFulminationSavage = 906,
                EdensVerseFuror = 903,
                EdensVerseFurorSavage = 907,
                EdensVerseIconoclasm = 904,
                EdensVerseIconoclasmSavage = 908,
                EdensVerseRefulgence = 905,
                EdensVerseRefulgenceSavage = 909,
                Elpis = 961,
                Emanation = 719,
                EmanationExtreme = 720,
                EmissaryOfTheDawn = 769,
                Endwalker = 1013,
                EngageMultipleTargets = 541,
                Eulmore = 820,
                EverMarchHeavensward = 1018,
                ExecuteAComboInBattle = 539,
                ExecuteAComboToIncreaseEnmity = 538,
                ExecuteARangedAttackToIncreaseEnmity = 542,
                FadedMemories = 932,
                FinalExercise = 552,
                FitForAQueen = 955,
                FlickingSticksAndTakingNames = 219,
                Foundation = 418,
                FourPlayerMahjongQuickMatchKuitanDisabled = 831,
                Garlemald = 958,
                Halatali = 162,
                HalataliHard = 360,
                HaukkeManor = 166,
                HaukkeManorHard = 350,
                HealAnAlly = 549,
                HealMultipleAllies = 550,
                HeavenOnHighFloors11_20 = 771,
                HeavenOnHighFloors1_10 = 770,
                HeavenOnHighFloors21_30 = 772,
                HeavenOnHighFloors31_40 = 782,
                HeavenOnHighFloors41_50 = 773,
                HeavenOnHighFloors51_60 = 783,
                HeavenOnHighFloors61_70 = 774,
                HeavenOnHighFloors71_80 = 784,
                HeavenOnHighFloors81_90 = 775,
                HeavenOnHighFloors91_100 = 785,
                HellsKier = 810,
                HellsKierExtreme = 811,
                HellsLid = 742,
                HeroOnTheHalfShell = 216,
                HiddenGorge = 791,
                HolminsterSwitch = 837,
                HullbreakerIsle = 361,
                HullbreakerIsleHard = 557,
                Idyllshire = 478,
                IlMheg = 816,
                InFromTheCold = 1011,
                InThalsName = 705,
                InteractWithTheBattlefield = 548,
                InterdimensionalRift = 690,
                ItsProbablyATrap = 665,
                Kholusia = 814,
                KtisisHyperboreia = 974,
                Kugane = 628,
                KuganeCastle = 662,
                KuganeOhashi = 806,
                Labyrinthos = 956,
                LaidToRest = 1017,
                Lakeland = 813,
                LegendOfTheNotSoHiddenTemple = 859,
                LifeEphemeralPathEternal = 1023,
                LimsaLominsaLowerDecks = 129,
                LimsaLominsaUpperDecks = 128,
                LongLiveTheQueen = 298,
                LovmMasterTournament = 506,
                LovmPlayerBattleNonRp = 591,
                LovmPlayerBattleRp = 589,
                LovmTournament = 590,
                LowerLaNoscea = 135,
                MalikahsWell = 836,
                MareLamentorum = 959,
                MatoyasRelict = 933,
                MatsubaMayhem = 710,
                MemoriaMiseraExtreme = 913,
                MessengerOfTheWinds = 834,
                MiddleLaNoscea = 134,
                Mist = 136,
                MorDhona = 156,
                MoreThanAFeeler = 221,
                MtGulg = 822,
                Naadam = 688,
                Neverreap = 420,
                NewGridania = 132,
                NorthShroud = 154,
                NorthernThanalan = 147,
                NyelbertsLament = 876,
                OceanFishing = 900,
                OldGridania = 133,
                OldSharlayan = 962,
                OneLifeForOneWorld = 592,
                OnsalHakairDanshigNaadam = 888,
                OurCompromise = 716,
                OurUnsungHeroes = 722,
                OuterLaNoscea = 180,
                Paglthan = 938,
                PharosSirius = 160,
                PharosSiriusHard = 510,
                PullingPoisonPosies = 191,
                RadzAtHan = 963,
                RaisingTheSword = 706,
                ReturnOfTheBull = 403,
                RhalgrsReach = 635,
                SagesFocus = 1022,
                SaintMociannesArboretum = 511,
                SaintMociannesArboretumHard = 788,
                Sastasha = 157,
                SastashaHard = 387,
                SealRockSeize = 431,
                ShadowAndClaw = 223,
                ShisuiOfTheVioletTides = 616,
                SigmascapeV10 = 748,
                SigmascapeV10Savage = 752,
                SigmascapeV20 = 749,
                SigmascapeV20Savage = 753,
                SigmascapeV30 = 750,
                SigmascapeV30Savage = 754,
                SigmascapeV40 = 751,
                SigmascapeV40Savage = 755,
                Smileton = 976,
                Snowcloak = 371,
                SohmAl = 441,
                SohmAlHard = 617,
                SohrKhai = 555,
                SolemnTrinity = 300,
                SouthShroud = 153,
                SouthernThanalan = 146,
                SpecialEventI = 353,
                SpecialEventIi = 354,
                SpecialEventIii = 509,
                StingingBack = 192,
                SyrcusTower = 372,
                Thavnair = 957,
                TheAery = 435,
                TheAetherochemicalResearchFacility = 438,
                TheAitiascope = 978,
                TheAkhAfahAmphitheatreExtreme = 378,
                TheAkhAfahAmphitheatreHard = 377,
                TheAkhAfahAmphitheatreUnreal = 930,
                TheAntitower = 516,
                TheAquapolis = 558,
                TheAurumVale = 172,
                TheAzimSteppe = 622,
                TheBattleOnBekko = 711,
                TheBindingCoilOfBahamutTurn1 = 241,
                TheBindingCoilOfBahamutTurn2 = 242,
                TheBindingCoilOfBahamutTurn3 = 243,
                TheBindingCoilOfBahamutTurn4 = 244,
                TheBindingCoilOfBahamutTurn5 = 245,
                TheBorderlandRuinsSecure = 376,
                TheBowlOfEmbers = 202,
                TheBowlOfEmbersExtreme = 295,
                TheBowlOfEmbersHard = 292,
                TheBozjaIncident = 911,
                TheBozjanSouthernFront = 920,
                TheBurn = 789,
                TheCalamityRetold = 790,
                TheCarteneauFlatsHeliodrome = 633,
                TheChrysalis = 426,
                TheChurningMists = 400,
                TheCloudDeck = 950,
                TheCloudDeckExtreme = 951,
                TheCopiedFactory = 882,
                TheCrownOfTheImmaculate = 846,
                TheCrownOfTheImmaculateExtreme = 848,
                TheCrystarium = 819,
                TheDancingPlague = 845,
                TheDancingPlagueExtreme = 858,
                TheDarkInside = 992,
                TheDeadEnds = 973,
                TheDiadem = 929,
                TheDiademEasy = 512,
                TheDiademHard = 515,
                TheDiademHuntingGrounds = 625,
                TheDiademHuntingGroundsEasy = 624,
                TheDiademTrialsOfTheFury = 630,
                TheDiademTrialsOfTheMatron = 656,
                TheDomanEnclave = 759,
                TheDragonsNeck = 142,
                TheDravanianForelands = 398,
                TheDravanianHinterlands = 399,
                TheDrownedCityOfSkalla = 731,
                TheDungeonsOfLyheGhiah = 879,
                TheDuskVigil = 434,
                TheDyingGasp = 847,
                TheEpicOfAlexanderUltimate = 887,
                TheExcitatron6000 = 1000,
                TheFaceOfTrueEvil = 709,
                TheFeastCustomMatchCrystalTower = 767,
                TheFeastCustomMatchFeastingGrounds = 619,
                TheFeastCustomMatchLichenweed = 646,
                TheFeastRanked = 765,
                TheFeastTeamRanked = 745,
                TheFeastTraining = 766,
                TheFieldsOfGloryShatter = 554,
                TheFinalCoilOfBahamutTurn1 = 193,
                TheFinalCoilOfBahamutTurn2 = 194,
                TheFinalCoilOfBahamutTurn3 = 195,
                TheFinalCoilOfBahamutTurn4 = 196,
                TheFinalDay = 997,
                TheFinalStepsOfFaith = 559,
                TheForbiddenLandEurekaAnemos = 732,
                TheForbiddenLandEurekaHydatos = 827,
                TheForbiddenLandEurekaPagos = 763,
                TheForbiddenLandEurekaPyros = 795,
                TheFractalContinuum = 430,
                TheFractalContinuumHard = 743,
                TheFringes = 612,
                TheGhimlytDark = 793,
                TheGiftOfMercy = 1019,
                TheGrandCosmos = 884,
                TheGreatGubalLibrary = 416,
                TheGreatGubalLibraryHard = 578,
                TheGreatHunt = 761,
                TheGreatHuntExtreme = 762,
                TheGreatShipVylbrand = 954,
                TheHardenedHeart = 873,
                TheHarvestBegins = 1020,
                TheHauntedManor = 571,
                TheHeartOfTheProblem = 718,
                TheHeroesGauntlet = 916,
                TheHiddenCanalsOfUznair = 725,
                TheHowlingEye = 208,
                TheHowlingEyeExtreme = 297,
                TheHowlingEyeHard = 294,
                TheHuntersLegacy = 875,
                TheJadeStoa = 746,
                TheJadeStoaExtreme = 758,
                TheKeeperOfTheLake = 150,
                TheKillingArt = 1021,
                TheLabyrinthOfTheAncients = 174,
                TheLimitlessBlueExtreme = 447,
                TheLimitlessBlueHard = 436,
                TheLochs = 621,
                TheLostAndTheFound = 874,
                TheLostCanalsOfUznair = 712,
                TheLostCityOfAmdapor = 363,
                TheLostCityOfAmdaporHard = 519,
                TheMinstrelsBalladHadessElegy = 885,
                TheMinstrelsBalladHydaelynsCall = 996,
                TheMinstrelsBalladNidhoggsRage = 566,
                TheMinstrelsBalladShinryusDomain = 730,
                TheMinstrelsBalladThordansReign = 448,
                TheMinstrelsBalladTsukuyomisPain = 779,
                TheMinstrelsBalladUltimasBane = 348,
                TheMinstrelsBalladZodiarksFall = 993,
                TheMothercrystal = 995,
                TheNavel = 206,
                TheNavelExtreme = 296,
                TheNavelHard = 293,
                TheNavelUnreal = 953,
                TheOrbonneMonastery = 826,
                TheOrphansAndTheBrokenBlade = 715,
                ThePalaceOfTheDeadFloors101_110 = 598,
                ThePalaceOfTheDeadFloors111_120 = 599,
                ThePalaceOfTheDeadFloors11_20 = 562,
                ThePalaceOfTheDeadFloors121_130 = 600,
                ThePalaceOfTheDeadFloors131_140 = 601,
                ThePalaceOfTheDeadFloors141_150 = 602,
                ThePalaceOfTheDeadFloors151_160 = 603,
                ThePalaceOfTheDeadFloors161_170 = 604,
                ThePalaceOfTheDeadFloors171_180 = 605,
                ThePalaceOfTheDeadFloors181_190 = 606,
                ThePalaceOfTheDeadFloors191_200 = 607,
                ThePalaceOfTheDeadFloors1_10 = 561,
                ThePalaceOfTheDeadFloors21_30 = 563,
                ThePalaceOfTheDeadFloors31_40 = 564,
                ThePalaceOfTheDeadFloors41_50 = 565,
                ThePalaceOfTheDeadFloors51_60 = 593,
                ThePalaceOfTheDeadFloors61_70 = 594,
                ThePalaceOfTheDeadFloors71_80 = 595,
                ThePalaceOfTheDeadFloors81_90 = 596,
                ThePalaceOfTheDeadFloors91_100 = 597,
                ThePeaks = 620,
                ThePhantomsFeast = 994,
                ThePillars = 419,
                ThePoolOfTribute = 674,
                ThePoolOfTributeExtreme = 677,
                ThePraetorium = 224,
                ThePuppetsBunker = 917,
                TheQitanaRavel = 823,
                TheRaktikaGreatwood = 817,
                TheResonant = 684,
                TheRidoranaLighthouse = 776,
                TheRoyalCityOfRabanastre = 734,
                TheRoyalMenagerie = 679,
                TheRubySea = 613,
                TheSeaOfClouds = 401,
                TheSeatOfSacrifice = 922,
                TheSeatOfSacrificeExtreme = 923,
                TheSecondCoilOfBahamutSavageTurn1 = 380,
                TheSecondCoilOfBahamutSavageTurn2 = 381,
                TheSecondCoilOfBahamutSavageTurn3 = 382,
                TheSecondCoilOfBahamutSavageTurn4 = 383,
                TheSecondCoilOfBahamutTurn1 = 355,
                TheSecondCoilOfBahamutTurn2 = 356,
                TheSecondCoilOfBahamutTurn3 = 357,
                TheSecondCoilOfBahamutTurn4 = 358,
                TheShiftingAltarsOfUznair = 794,
                TheShiftingOubliettesOfLyheGhiah = 924,
                TheSingularityReactor = 437,
                TheSirensongSea = 626,
                TheStepsOfFaith = 143,
                TheStigmaDreamscape = 986,
                TheStoneVigil = 168,
                TheStoneVigilHard = 365,
                TheStrikingTreeExtreme = 375,
                TheStrikingTreeHard = 374,
                TheSunkenTempleOfQarn = 163,
                TheSunkenTempleOfQarnHard = 367,
                TheSwallowsCompass = 768,
                TheTamTaraDeepcroft = 164,
                TheTamTaraDeepcroftHard = 373,
                TheTempest = 818,
                TheTempleOfTheFist = 663,
                TheThousandMawsOfTotoRak = 169,
                TheTowerAtParadigmsBreach = 966,
                TheTowerOfBabil = 969,
                TheTowerOfZot = 952,
                TheTripleTriadBattlehall = 579,
                TheTwinning = 840,
                TheUnendingCoilOfBahamutUltimate = 733,
                TheValentionesCeremony = 741,
                TheVault = 421,
                TheVoidArk = 508,
                TheWanderersPalace = 159,
                TheWanderersPalaceHard = 188,
                TheWeaponsRefrainUltimate = 777,
                TheWeepingCityOfMhach = 556,
                TheWhorleaterExtreme = 359,
                TheWhorleaterHard = 281,
                TheWhorleaterUnreal = 972,
                TheWillOfTheMoon = 797,
                TheWorldOfDarkness = 151,
                TheWreathOfSnakes = 824,
                TheWreathOfSnakesExtreme = 825,
                ThokAstThokExtreme = 446,
                ThokAstThokHard = 432,
                ThornmarchExtreme = 364,
                ThornmarchHard = 207,
                ToCalmerSeas = 1016,
                TripleTriadInvitationalParlor = 941,
                TripleTriadOpenTournament = 940,
                UldahStepsOfNald = 130,
                UldahStepsOfThal = 131,
                UltimaThule = 960,
                UnderTheArmor = 190,
                UpperLaNoscea = 139,
                UrthsFount = 394,
                Vanaspati = 970,
                VowsOfVirtueDeedsOfCruelty = 893,
                WardUp = 299,
                WesternLaNoscea = 138,
                WesternThanalan = 140,
                WhenClansCollide = 723,
                WithHeartAndSteel = 707,
                WolvesDenPier = 250,
                WorthyOfHisBack = 1014,
                Xelphatol = 572,
                Yanxia = 614,
                Zadnor = 975;
        }
    }
}