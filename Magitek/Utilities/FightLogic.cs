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

                            Debug.Instance.FightLogicData += "\n";
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

            #region Endwalker: Dungeons

            new Encounter {
                ZoneId = ZoneId.TheTowerOfZot,
                Name = "Dungeon: The Tower of Zot",
                Expansion = FfxivExpansion.Endwalker,
                Enemies = new List<Enemy> {
                    new Enemy {
                        Id = 10256,
                        Name = "Minduruva",
                        TankBusters = new List<uint> { 25257, 25290 },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10257,
                        Name = "Sanduruva",
                        TankBusters = new List<uint> { 25257, 25280 },
                        SharedTankBusters = null,
                        Aoes = null,
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10259,
                        Name = "Cinduruva",
                        TankBusters = new List<uint> { 25257 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25273, },
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
                        Aoes = new List<uint> { 25338 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10282,
                        Name = "Lugae",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25338 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10285,
                        Name = "Anima",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25344 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10288,
                        Name = "Anima",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25344 },
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
                        TankBusters = new List<uint> { 25141 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25144 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 11049,
                        Name = "Terminus Snatcher",
                        TankBusters = new List<uint> { 25141 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25144 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10718,
                        Name = "Terminus Wrecker",
                        TankBusters = new List<uint> { 25154 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25153 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 11052,
                        Name = "Terminus Wrecker",
                        TankBusters = new List<uint> { 25154 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25153 },
                        BigAoes = null

                    },
                    new Enemy {
                        Id = 10719,
                        Name = "Svarbhanu",
                        TankBusters = new List<uint> { 25171 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25170 },
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
                        TankBusters = new List<uint> { 25182 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25181 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10398,
                        Name = "Ladon Lord",
                        TankBusters = new List<uint> { 25743 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25741 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10363,
                        Name = "Hermes",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25886 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10399,
                        Name = "Hermes",
                        TankBusters = null,
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25886 },
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
                        Aoes = new List<uint> { 25672 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10292,
                        Name = "Rhitahtyn the Unshakable",
                        TankBusters = new List<uint> { 25686 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25685 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10293,
                        Name = "Amon the Undying",
                        TankBusters = new List<uint> { 25700 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25701 },
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
                        TankBusters = new List<uint> { 26434 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 26435 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10333,
                        Name = "Frameworker",
                        TankBusters = new List<uint> { 26436 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 26437 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10336,
                        Name = "The Big Cheese",
                        TankBusters = new List<uint> { 26449 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 26450, 26451, 26452 },
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
                        TankBusters = new List<uint> { 25920 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25916, 25921, 25923 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10315,
                        Name = "Peacekeeper",
                        TankBusters = new List<uint> { 28359, 25935 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25936, 28351, 25925 },
                        BigAoes = null
                    },
                    new Enemy {
                        Id = 10316,
                        Name = "Ra-la",
                        TankBusters = new List<uint> { 25949 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 25950, 25945, 25947 },
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
                        TankBusters = new List<uint> { 27490 },
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
                        SharedTankBusters = new List<uint> { 26070 },
                        Aoes = new List<uint> { 26071, 26072, 26043, 26064, 26037, 26038, 26824 },
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
                        Aoes = new List<uint> { 26156, 26242 },
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
                        TankBusters = new List<uint> { 26607 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 26611, 26608 },
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
                        SharedTankBusters = new List<uint> { 26048, 26040 },
                        Aoes = new List<uint> { 26036, 26049, 26050, 27477, 26021, 27476 },
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
                        Id = 10576,
                        Name = "Erichthonios",
                        TankBusters = new List<uint> { 26099 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 26100, 26089, 26090 },
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
                        SharedTankBusters = new List<uint> { 26638 },
                        Aoes = new List<uint> { 26639, 26614, 26625, 26632 },
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
                        Aoes = new List<uint> { 26296, 26281 },
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
                        Aoes = new List<uint> { 27217, 27200 },
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
                        Id = 10576,
                        Name = "Erichthonios",
                        TankBusters = new List<uint> { 26153 },
                        SharedTankBusters = null,
                        Aoes = new List<uint> { 26154, 26134, 26135 },
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
                        SharedTankBusters = new List<uint> { 26674 },
                        Aoes = new List<uint> { 26675 },
                        BigAoes = new List<uint> { 26640 }
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
                        Aoes = new List<uint> { 26374 },
                        BigAoes = new List<uint> { 26340, 26352 }
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
                        TankBusters = new List<uint> { 27144, 27174, 27175, 27179 },
                        SharedTankBusters = new List<uint> { 28280 },
                        Aoes = new List<uint> { 27145, 27096, 27181 },
                        BigAoes = new List<uint> { 27180 }
                    }
                }
            }

            #endregion
        };
        public class Ref<T>
        {
            public Ref() { }
            public Ref(T value) { Value = value; }
            private T Value { get; set; }

            override public string ToString()
            {
                T value = Value;
                return value == null ? "" : value.ToString();
            }

            public static implicit operator T(Ref<T> r) { return r.Value; }
            public static implicit operator Ref<T>(T value) { return new Ref<T>(value); }
        }

        internal class Enemy
        {
            internal uint Id { get; set; }
            internal string Name { get; set; }
            internal List<uint> TankBusters { get; set; }
            internal List<uint> SharedTankBusters { get; set; }
            internal List<uint> Aoes { get; set; }
            internal List<uint> BigAoes { get; set; }
        }

        internal class Encounter
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