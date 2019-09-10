using ff14bot.Managers;
using ff14bot.Objects;

namespace Magitek.Utilities
{
    internal static class Spells
    {
        // DPS Role
        #region DPS Role
        public static readonly SpellData SecondWind = DataManager.GetSpellData(7541);
        public static readonly SpellData Bloodbath = DataManager.GetSpellData(7542);
        public static readonly SpellData TrueNorth = DataManager.GetSpellData(7546);
        public static readonly SpellData ArmsLength = DataManager.GetSpellData(7548);
        public static readonly SpellData Feint = DataManager.GetSpellData(7549);
        public static readonly SpellData HeadGraze = DataManager.GetSpellData(7551);
        public static readonly SpellData FootGraze = DataManager.GetSpellData(7553);
        public static readonly SpellData LegGraze = DataManager.GetSpellData(7554);
        public static readonly SpellData Peloton = DataManager.GetSpellData(7557);
        public static readonly SpellData LegSweep = DataManager.GetSpellData(7863);
        #endregion

        // Magic Role
        #region Magic Role
        public static readonly SpellData Surecast = DataManager.GetSpellData(7559);
        public static readonly SpellData Addle = DataManager.GetSpellData(7560);
        public static readonly SpellData Swiftcast = DataManager.GetSpellData(7561);
        public static readonly SpellData LucidDreaming = DataManager.GetSpellData(7562);
        public static readonly SpellData Esuna = DataManager.GetSpellData(7568);
        public static readonly SpellData Rescue = DataManager.GetSpellData(7571);
        public static readonly SpellData Repose = DataManager.GetSpellData(16560);
        #endregion

        // Tank Role
        #region Tank Role
        public static readonly SpellData Rampart = DataManager.GetSpellData(7531);
        public static readonly SpellData Provoke = DataManager.GetSpellData(7533);
        public static readonly SpellData Reprisal = DataManager.GetSpellData(7535);
        public static readonly SpellData Shirk = DataManager.GetSpellData(7537);
        public static readonly SpellData Interject = DataManager.GetSpellData(7538);
        public static readonly SpellData LowBlow = DataManager.GetSpellData(7540);
        #endregion

        // ACN
        #region ACN
        public static readonly SpellData Ruin = DataManager.GetSpellData(17869);
        public static readonly SpellData Ruin2 = DataManager.GetSpellData(17870);
        public static readonly SpellData Summon = DataManager.GetSpellData(165);
        public static readonly SpellData Summon2 = DataManager.GetSpellData(170);
        public static readonly SpellData Miasma = DataManager.GetSpellData(168);
        public static readonly SpellData Resurrection = DataManager.GetSpellData(173);
        public static readonly SpellData Bio = DataManager.GetSpellData(17864);
        public static readonly SpellData Bio2 = DataManager.GetSpellData(17865);
        public static readonly SpellData Summon3 = DataManager.GetSpellData(180);
        public static readonly SpellData Enkindle = DataManager.GetSpellData(184);
        #endregion

        // AST
        #region AST
        public static readonly SpellData Draw = DataManager.GetSpellData(3590);
        public static readonly SpellData Redraw = DataManager.GetSpellData(3593);
        public static readonly SpellData Benefic = DataManager.GetSpellData(3594);
        public static readonly SpellData AspectedBenefic = DataManager.GetSpellData(3595);
        public static readonly SpellData Malefic = DataManager.GetSpellData(3596);
        public static readonly SpellData Malefic2 = DataManager.GetSpellData(3598);
        public static readonly SpellData Combust = DataManager.GetSpellData(3599);
        public static readonly SpellData Helios = DataManager.GetSpellData(3600);
        public static readonly SpellData AspectedHelios = DataManager.GetSpellData(3601);
        public static readonly SpellData Ascend = DataManager.GetSpellData(3603);
        public static readonly SpellData DiurnalSect = DataManager.GetSpellData(3604);
        public static readonly SpellData NocturnalSect = DataManager.GetSpellData(3605);
        public static readonly SpellData Lightspeed = DataManager.GetSpellData(3606);
        public static readonly SpellData Combust2 = DataManager.GetSpellData(3608);
        public static readonly SpellData Benefic2 = DataManager.GetSpellData(3610);
        public static readonly SpellData Synastry = DataManager.GetSpellData(3612);
        public static readonly SpellData CollectiveUnconscious = DataManager.GetSpellData(3613);
        public static readonly SpellData EssentialDignity = DataManager.GetSpellData(3614);
        public static readonly SpellData Gravity = DataManager.GetSpellData(3615);
        public static readonly SpellData Balance = DataManager.GetSpellData(4401);
        public static readonly SpellData Arrow = DataManager.GetSpellData(4402);
        public static readonly SpellData Spear = DataManager.GetSpellData(4403);
        public static readonly SpellData Bole = DataManager.GetSpellData(4404);
        public static readonly SpellData Ewer = DataManager.GetSpellData(4405);
        public static readonly SpellData Spire = DataManager.GetSpellData(4406);
        public static readonly SpellData EarthlyStar = DataManager.GetSpellData(7439);
        public static readonly SpellData Malefic3 = DataManager.GetSpellData(7442);
        public static readonly SpellData MinorArcana = DataManager.GetSpellData(7443);
        public static readonly SpellData LordofCrowns = DataManager.GetSpellData(7444);
        public static readonly SpellData LadyofCrowns = DataManager.GetSpellData(7445);
        public static readonly SpellData SleeveDraw = DataManager.GetSpellData(7448);
        public static readonly SpellData StellarDetonation = DataManager.GetSpellData(8324);
        public static readonly SpellData Undraw = DataManager.GetSpellData(9629);
        public static readonly SpellData Divination = DataManager.GetSpellData(16552);
        public static readonly SpellData CelestialOpposition = DataManager.GetSpellData(16553);
        public static readonly SpellData Combust3 = DataManager.GetSpellData(16554);
        public static readonly SpellData Malefic4 = DataManager.GetSpellData(16555);
        public static readonly SpellData CelestialIntersection = DataManager.GetSpellData(16556);
        public static readonly SpellData Horoscope = DataManager.GetSpellData(16557);
        public static readonly SpellData NeutralSect = DataManager.GetSpellData(16559);
        public static readonly SpellData Play = DataManager.GetSpellData(17055);
        #endregion

        // BLM
        #region BLM
        public static readonly SpellData Fire = DataManager.GetSpellData(141);
        public static readonly SpellData Blizzard = DataManager.GetSpellData(142);
        public static readonly SpellData Thunder = DataManager.GetSpellData(144);
        public static readonly SpellData Fire2 = DataManager.GetSpellData(147);
        public static readonly SpellData Transpose = DataManager.GetSpellData(149);
        public static readonly SpellData Fire3 = DataManager.GetSpellData(152);
        public static readonly SpellData Thunder3 = DataManager.GetSpellData(153);
        public static readonly SpellData Blizzard3 = DataManager.GetSpellData(154);
        public static readonly SpellData Scathe = DataManager.GetSpellData(156);
        public static readonly SpellData ManaFont = DataManager.GetSpellData(158);
        public static readonly SpellData Flare = DataManager.GetSpellData(162);
        public static readonly SpellData Freeze = DataManager.GetSpellData(159);
        public static readonly SpellData LeyLines = DataManager.GetSpellData(3573);
        public static readonly SpellData Sharpcast = DataManager.GetSpellData(3574);
        public static readonly SpellData Enochian = DataManager.GetSpellData(3575);
        public static readonly SpellData Blizzard4 = DataManager.GetSpellData(3576);
        public static readonly SpellData Fire4 = DataManager.GetSpellData(3577);
        public static readonly SpellData Thunder2 = DataManager.GetSpellData(7447);
        public static readonly SpellData Thunder4 = DataManager.GetSpellData(7420);
        public static readonly SpellData Triplecast = DataManager.GetSpellData(7421);
        public static readonly SpellData Foul = DataManager.GetSpellData(7422);
        public static readonly SpellData Despair = DataManager.GetSpellData(16505);
        public static readonly SpellData UmbralSoul = DataManager.GetSpellData(16506);
        public static readonly SpellData Xenoglossy = DataManager.GetSpellData(16507);
        #endregion

        // BRD
        #region BRD
        //SingleTarget

        public static readonly SpellData HeavyShot = DataManager.GetSpellData(97);
        public static readonly SpellData StraightShot = DataManager.GetSpellData(98);
        public static readonly SpellData Bloodletter = DataManager.GetSpellData(110);
        public static readonly SpellData PitchPerfect = DataManager.GetSpellData(7404);
        public static readonly SpellData EmpyrealArrow = DataManager.GetSpellData(3558);
        public static readonly SpellData Sidewinder = DataManager.GetSpellData(3562);
        public static readonly SpellData RefulgentArrow = DataManager.GetSpellData(7409);
        public static readonly SpellData BurstShot = DataManager.GetSpellData(16495);

        //AoE

        public static readonly SpellData QuickNock = DataManager.GetSpellData(106);
        public static readonly SpellData RainofDeath = DataManager.GetSpellData(117);
        public static readonly SpellData Shadowbite = DataManager.GetSpellData(16494);
        public static readonly SpellData ApexArrow = DataManager.GetSpellData(16496);

        //Dot

        public static readonly SpellData VenomousBite = DataManager.GetSpellData(100);
        public static readonly SpellData Windbite = DataManager.GetSpellData(113);
        public static readonly SpellData IronJaws = DataManager.GetSpellData(3560);         //Not a DoT but will refresh both
        public static readonly SpellData CausticBite = DataManager.GetSpellData(7406);
        public static readonly SpellData Stormbite = DataManager.GetSpellData(7407);

        //Cooldowns - unsure about naming this :/

        public static readonly SpellData RagingStrikes = DataManager.GetSpellData(101);
        public static readonly SpellData Barrage = DataManager.GetSpellData(107);
        public static readonly SpellData BattleVoice = DataManager.GetSpellData(118);

        //Songs

        public static readonly SpellData MagesBallad = DataManager.GetSpellData(114);
        public static readonly SpellData ArmysPaeon = DataManager.GetSpellData(116);
        public static readonly SpellData TheWanderersMinuet = DataManager.GetSpellData(3559);

        //Utility/Movement

        public static readonly SpellData RepellingShot = DataManager.GetSpellData(112);
        public static readonly SpellData TheWardensPaean = DataManager.GetSpellData(3561);
        public static readonly SpellData Troubadour = DataManager.GetSpellData(7405);
        public static readonly SpellData NaturesMinne = DataManager.GetSpellData(7408);

        #endregion

        // DNC
        #region DNC
        public static readonly SpellData Cascade = DataManager.GetSpellData(15989);
        public static readonly SpellData Fountain = DataManager.GetSpellData(15990);
        public static readonly SpellData ReverseCascade = DataManager.GetSpellData(15991);
        public static readonly SpellData Fountainfall = DataManager.GetSpellData(15992);
        public static readonly SpellData Windmill = DataManager.GetSpellData(15993);
        public static readonly SpellData Bladeshower = DataManager.GetSpellData(15994);
        public static readonly SpellData RisingWindmill = DataManager.GetSpellData(15995);
        public static readonly SpellData Bloodshower = DataManager.GetSpellData(15996);
        public static readonly SpellData StandardStep = DataManager.GetSpellData(15997);
        public static readonly SpellData Emboite = DataManager.GetSpellData(15999);
        public static readonly SpellData Entrechat = DataManager.GetSpellData(16000);
        public static readonly SpellData Jete = DataManager.GetSpellData(16001);
        public static readonly SpellData Pirouette = DataManager.GetSpellData(16002);
        public static readonly SpellData StandardFinish = DataManager.GetSpellData(16003);
        public static readonly SpellData SaberDance = DataManager.GetSpellData(16005);
        public static readonly SpellData ClosedPosition = DataManager.GetSpellData(16006);
        public static readonly SpellData FanDance = DataManager.GetSpellData(16007);
        public static readonly SpellData FanDance2 = DataManager.GetSpellData(16008);
        public static readonly SpellData FanDance3 = DataManager.GetSpellData(16009);
        public static readonly SpellData EnAvant = DataManager.GetSpellData(16010);
        public static readonly SpellData Devilment = DataManager.GetSpellData(16011);
        public static readonly SpellData ShieldSamba = DataManager.GetSpellData(16012);
        public static readonly SpellData Flourish = DataManager.GetSpellData(16013);
        public static readonly SpellData Improvisation = DataManager.GetSpellData(16014);
        public static readonly SpellData CuringWaltz = DataManager.GetSpellData(16015);
        public static readonly SpellData SingleStandardFinish = DataManager.GetSpellData(16191);
        public static readonly SpellData DoubleStandardFinish = DataManager.GetSpellData(16192);
        public static readonly SpellData Ending = DataManager.GetSpellData(18073);
        public static readonly SpellData TechnicalStep = DataManager.GetSpellData(15998);
        public static readonly SpellData SingleTechnicalFinish = DataManager.GetSpellData(16193);
        public static readonly SpellData DoubleTechnicalFinish = DataManager.GetSpellData(16194);
        public static readonly SpellData TripleTechnicalFinish = DataManager.GetSpellData(16195);
        public static readonly SpellData QuadrupleTechnicalFinish = DataManager.GetSpellData(16196);
        #endregion

        // DRG
        #region DRG
        public static readonly SpellData TrueThrust = DataManager.GetSpellData(75);
        public static readonly SpellData VorpalThrust = DataManager.GetSpellData(78);
        public static readonly SpellData LifeSurge = DataManager.GetSpellData(83);
        public static readonly SpellData DoomSpike = DataManager.GetSpellData(86);
        public static readonly SpellData Disembowel = DataManager.GetSpellData(87);
        public static readonly SpellData ChaosThrust = DataManager.GetSpellData(88);
        public static readonly SpellData Jump = DataManager.GetSpellData(92);
        public static readonly SpellData SpineshatterDive = DataManager.GetSpellData(95);
        public static readonly SpellData DragonfireDive = DataManager.GetSpellData(96);
        public static readonly SpellData BloodoftheDragon = DataManager.GetSpellData(3553);
        public static readonly SpellData FangAndClaw = DataManager.GetSpellData(3554);
        public static readonly SpellData Geirskogul = DataManager.GetSpellData(3555);
        public static readonly SpellData WheelingThrust = DataManager.GetSpellData(3556);
        public static readonly SpellData BattleLitany = DataManager.GetSpellData(3557);
        public static readonly SpellData MirageDive = DataManager.GetSpellData(7399);
        public static readonly SpellData Nastrond = DataManager.GetSpellData(7400);
        public static readonly SpellData LanceCharge = DataManager.GetSpellData(85);
        public static readonly SpellData FullThrust = DataManager.GetSpellData(84);
        public static readonly SpellData SonicThrust = DataManager.GetSpellData(7397);
        public static readonly SpellData DragonSight = DataManager.GetSpellData(7398);
        public static readonly SpellData CoerthanTorment = DataManager.GetSpellData(16477);
        public static readonly SpellData HighJump = DataManager.GetSpellData(16478);
        public static readonly SpellData RaidenThrust = DataManager.GetSpellData(16479);
        public static readonly SpellData Stardiver = DataManager.GetSpellData(16480);
        #endregion

        // DRK
        #region DRK
        public static readonly SpellData HardSlash = DataManager.GetSpellData(3617);
        public static readonly SpellData Unleash = DataManager.GetSpellData(3621);
        public static readonly SpellData SyphonStrike = DataManager.GetSpellData(3623);
        public static readonly SpellData Unmend = DataManager.GetSpellData(3624);
        public static readonly SpellData BloodWeapon = DataManager.GetSpellData(3625);
        public static readonly SpellData Grit = DataManager.GetSpellData(3629);
        public static readonly SpellData Souleater = DataManager.GetSpellData(3632);
        public static readonly SpellData DarkMind = DataManager.GetSpellData(3634);
        public static readonly SpellData ShadowWall = DataManager.GetSpellData(3636);
        public static readonly SpellData LivingDead = DataManager.GetSpellData(3638);
        public static readonly SpellData SaltedEarth = DataManager.GetSpellData(3639);
        public static readonly SpellData Plunge = DataManager.GetSpellData(3640);
        public static readonly SpellData AbyssalDrain = DataManager.GetSpellData(3641);
        public static readonly SpellData CarveandSpit = DataManager.GetSpellData(3643);
        public static readonly SpellData Delirium = DataManager.GetSpellData(7390);
        public static readonly SpellData Quietus = DataManager.GetSpellData(7391);
        public static readonly SpellData Bloodspiller = DataManager.GetSpellData(7392);
        public static readonly SpellData TheBlackestNight = DataManager.GetSpellData(7393);
        public static readonly SpellData FloodofDarkness = DataManager.GetSpellData(16466);
        public static readonly SpellData EdgeofDarkness = DataManager.GetSpellData(16467);
        public static readonly SpellData StalwartSoul = DataManager.GetSpellData(16468);
        public static readonly SpellData FloodofShadow = DataManager.GetSpellData(16469);
        public static readonly SpellData EdgeofShadow = DataManager.GetSpellData(16470);
        public static readonly SpellData DarkMissionary = DataManager.GetSpellData(16471);
        public static readonly SpellData LivingShadow = DataManager.GetSpellData(16472);
        #endregion

        // GNB
        #region GNB
        public static readonly SpellData KeenEdge = DataManager.GetSpellData(16137);
        public static readonly SpellData NoMercy = DataManager.GetSpellData(16138);
        public static readonly SpellData BrutalShell = DataManager.GetSpellData(16139);
        public static readonly SpellData Camouflage = DataManager.GetSpellData(16140);
        public static readonly SpellData DemonSlice = DataManager.GetSpellData(16141);
        public static readonly SpellData RoyalGuard = DataManager.GetSpellData(16142);
        public static readonly SpellData LightningShot = DataManager.GetSpellData(16143);
        public static readonly SpellData DangerZone = DataManager.GetSpellData(16144);
        public static readonly SpellData SolidBarrel = DataManager.GetSpellData(16145);
        public static readonly SpellData GnashingFang = DataManager.GetSpellData(16146);
        public static readonly SpellData SavageClaw = DataManager.GetSpellData(16147);
        public static readonly SpellData Nebula = DataManager.GetSpellData(16148);
        public static readonly SpellData DemonSlaughter = DataManager.GetSpellData(16149);
        public static readonly SpellData WickedTalon = DataManager.GetSpellData(16150);
        public static readonly SpellData Aurora = DataManager.GetSpellData(16151);
        public static readonly SpellData Superbolide = DataManager.GetSpellData(16152);
        public static readonly SpellData SonicBreak = DataManager.GetSpellData(16153);
        public static readonly SpellData RoughDivide = DataManager.GetSpellData(16154);
        public static readonly SpellData Continuation = DataManager.GetSpellData(16155);
        public static readonly SpellData JugularRip = DataManager.GetSpellData(16156);
        public static readonly SpellData AbdomenTear = DataManager.GetSpellData(16157);
        public static readonly SpellData EyeGouge = DataManager.GetSpellData(16158);
        public static readonly SpellData BowShock = DataManager.GetSpellData(16159);
        public static readonly SpellData HeartofLight = DataManager.GetSpellData(16160);
        public static readonly SpellData HeartofStone = DataManager.GetSpellData(16161);
        public static readonly SpellData BurstStrike = DataManager.GetSpellData(16162);
        public static readonly SpellData FatedCircle = DataManager.GetSpellData(16163);
        public static readonly SpellData Bloodfest = DataManager.GetSpellData(16164);
        public static readonly SpellData BlastingZone = DataManager.GetSpellData(16165);
        #endregion

        // MCH
        #region MCH
        public static readonly SpellData RookAutoturret = DataManager.GetSpellData(2864);
        public static readonly SpellData SplitShot = DataManager.GetSpellData(2866);
        public static readonly SpellData SlugShot = DataManager.GetSpellData(2868);
        public static readonly SpellData SpreadShot = DataManager.GetSpellData(2870);
        public static readonly SpellData HotShot = DataManager.GetSpellData(2872);
        public static readonly SpellData CleanShot = DataManager.GetSpellData(2873);
        public static readonly SpellData GaussRound = DataManager.GetSpellData(2874);
        public static readonly SpellData Reassemble = DataManager.GetSpellData(2876);
        public static readonly SpellData Wildfire = DataManager.GetSpellData(2878);
        public static readonly SpellData Ricochet = DataManager.GetSpellData(2890);
        public static readonly SpellData HeatBlast = DataManager.GetSpellData(7410);
        public static readonly SpellData HeatedSplitShot = DataManager.GetSpellData(7411);
        public static readonly SpellData HeatedSlugShot = DataManager.GetSpellData(7412);
        public static readonly SpellData HeatedCleanShot = DataManager.GetSpellData(7413);
        public static readonly SpellData BarrelStabilizer = DataManager.GetSpellData(7414);
        public static readonly SpellData RookOverdrive = DataManager.GetSpellData(7415);
        public static readonly SpellData Flamethrower = DataManager.GetSpellData(7418);
        public static readonly SpellData AutoCrossbow = DataManager.GetSpellData(16497);
        public static readonly SpellData Drill = DataManager.GetSpellData(16498);
        public static readonly SpellData Bioblaster = DataManager.GetSpellData(16499);
        public static readonly SpellData AirAnchor = DataManager.GetSpellData(16500);
        public static readonly SpellData AutomationQueen = DataManager.GetSpellData(16501);
        public static readonly SpellData QueenOverdrive = DataManager.GetSpellData(16502);
        public static readonly SpellData Detonator = DataManager.GetSpellData(16766);
        public static readonly SpellData Tactician = DataManager.GetSpellData(16889);
        public static readonly SpellData Hypercharge = DataManager.GetSpellData(17209);
        #endregion

        // MNK
        #region MNK
        public static readonly SpellData ArmOfTheDestroyer = DataManager.GetSpellData(62);
        public static readonly SpellData Bootshine = DataManager.GetSpellData(53);
        public static readonly SpellData TrueStrike = DataManager.GetSpellData(54);
        public static readonly SpellData SnapPunch = DataManager.GetSpellData(56);
        public static readonly SpellData FistsOfEarth = DataManager.GetSpellData(60);
        public static readonly SpellData TwinSnakes = DataManager.GetSpellData(61);
        public static readonly SpellData Demolish = DataManager.GetSpellData(66);
        public static readonly SpellData Rockbreaker = DataManager.GetSpellData(70);
        public static readonly SpellData FistsOfWind = DataManager.GetSpellData(73);
        public static readonly SpellData ShoulderTackle = DataManager.GetSpellData(71);
        public static readonly SpellData FistsOfFire = DataManager.GetSpellData(63);
        public static readonly SpellData Mantra = DataManager.GetSpellData(65);
        public static readonly SpellData PerfectBalance = DataManager.GetSpellData(69);
        public static readonly SpellData DragonKick = DataManager.GetSpellData(74);
        public static readonly SpellData TheForbiddenChakra = DataManager.GetSpellData(3547);
        public static readonly SpellData ElixirField = DataManager.GetSpellData(3545);
        public static readonly SpellData RiddleofEarth = DataManager.GetSpellData(7394);
        public static readonly SpellData RiddleofFire = DataManager.GetSpellData(7395);
        public static readonly SpellData Brotherhood = DataManager.GetSpellData(7396);
        public static readonly SpellData FormShift = DataManager.GetSpellData(4262);
        public static readonly SpellData FourPointFury = DataManager.GetSpellData(16473);
        public static readonly SpellData Enlightenment = DataManager.GetSpellData(16474);
        #endregion

        // NIN
        #region NIN
        public static readonly SpellData SpinningEdge = DataManager.GetSpellData(2240);
        public static readonly SpellData ShadeShift = DataManager.GetSpellData(2241);
        public static readonly SpellData GustSlash = DataManager.GetSpellData(2242);
        public static readonly SpellData Hide = DataManager.GetSpellData(2245);
        public static readonly SpellData Assassinate = DataManager.GetSpellData(2246);
        public static readonly SpellData ThrowingDagger = DataManager.GetSpellData(2247);
        public static readonly SpellData Mug = DataManager.GetSpellData(2248);
        public static readonly SpellData DeathBlossom = DataManager.GetSpellData(2254);
        public static readonly SpellData AeolianEdge = DataManager.GetSpellData(2255);
        public static readonly SpellData ShadowFang = DataManager.GetSpellData(2257);
        public static readonly SpellData TrickAttack = DataManager.GetSpellData(2258);
        public static readonly SpellData Ten = DataManager.GetSpellData(2259);
        public static readonly SpellData Ninjutsu = DataManager.GetSpellData(2260);
        public static readonly SpellData Chi = DataManager.GetSpellData(2261);
        public static readonly SpellData Shukuchi = DataManager.GetSpellData(2262);
        public static readonly SpellData Jin = DataManager.GetSpellData(2263);
        public static readonly SpellData Kassatsu = DataManager.GetSpellData(2264);
        public static readonly SpellData FumaShuriken = DataManager.GetSpellData(2265);
        public static readonly SpellData Katon = DataManager.GetSpellData(2266);
        public static readonly SpellData Raiton = DataManager.GetSpellData(2267);
        public static readonly SpellData Hyoton = DataManager.GetSpellData(2268);
        public static readonly SpellData Huton = DataManager.GetSpellData(2269);
        public static readonly SpellData Doton = DataManager.GetSpellData(2270);
        public static readonly SpellData Suiton = DataManager.GetSpellData(2271);
        public static readonly SpellData RabbitMedium = DataManager.GetSpellData(2272);
        public static readonly SpellData ArmorCrush = DataManager.GetSpellData(3563);
        public static readonly SpellData DreamWithinaDream = DataManager.GetSpellData(3566);
        public static readonly SpellData HellfrogMedium = DataManager.GetSpellData(7401);
        public static readonly SpellData Bhavacakra = DataManager.GetSpellData(7402);
        public static readonly SpellData TenChiJin = DataManager.GetSpellData(7403);
        public static readonly SpellData HakkeMujinsatsu = DataManager.GetSpellData(16488);
        public static readonly SpellData Meisui = DataManager.GetSpellData(16489);
        public static readonly SpellData GokaMekkyaku = DataManager.GetSpellData(16491);
        public static readonly SpellData HyoshoRanryu = DataManager.GetSpellData(16492);
        public static readonly SpellData Bunshin = DataManager.GetSpellData(16493);
        #endregion

        // PLD
        #region PLD
        public static readonly SpellData Sentinel = DataManager.GetSpellData(17);
        public static readonly SpellData FightorFlight = DataManager.GetSpellData(20);
        public static readonly SpellData Cover = DataManager.GetSpellData(27);
        public static readonly SpellData HallowedGround = DataManager.GetSpellData(30);
        public static readonly SpellData DivineVeil = DataManager.GetSpellData(3540);
        public static readonly SpellData Sheltron = DataManager.GetSpellData(3542);
        public static readonly SpellData CircleofScorn = DataManager.GetSpellData(23);
        public static readonly SpellData SpiritsWithin = DataManager.GetSpellData(29);
        public static readonly SpellData IronWill = DataManager.GetSpellData(28);
        public static readonly SpellData Clemency = DataManager.GetSpellData(3541);
        public static readonly SpellData FastBlade = DataManager.GetSpellData(9);
        public static readonly SpellData RiotBlade = DataManager.GetSpellData(15);
        public static readonly SpellData ShieldBash = DataManager.GetSpellData(16);
        public static readonly SpellData RageofHalone = DataManager.GetSpellData(21);
        public static readonly SpellData ShieldLob = DataManager.GetSpellData(24);
        public static readonly SpellData GoringBlade = DataManager.GetSpellData(3538);
        public static readonly SpellData RoyalAuthority = DataManager.GetSpellData(3539);
        public static readonly SpellData TotalEclipse = DataManager.GetSpellData(7381);
        public static readonly SpellData Intervention = DataManager.GetSpellData(7382);
        public static readonly SpellData HolySpirit = DataManager.GetSpellData(7384);
        public static readonly SpellData Requiescat = DataManager.GetSpellData(7383);
        public static readonly SpellData Prominance = DataManager.GetSpellData(16457);
        public static readonly SpellData HolyCircle = DataManager.GetSpellData(16458);
        public static readonly SpellData Intervene = DataManager.GetSpellData(16461);
        public static readonly SpellData Atonement = DataManager.GetSpellData(16460);
        public static readonly SpellData Confiteor = DataManager.GetSpellData(16459);
        #endregion

        // RDM
        #region RDM
        public static readonly SpellData Jolt = DataManager.GetSpellData(7503);
        public static readonly SpellData Riposte = DataManager.GetSpellData(7504);
        public static readonly SpellData Verthunder = DataManager.GetSpellData(7505);
        public static readonly SpellData CorpsACorps = DataManager.GetSpellData(7506);
        public static readonly SpellData Veraero = DataManager.GetSpellData(7507);
        public static readonly SpellData Scatter = DataManager.GetSpellData(7509);
        public static readonly SpellData Verfire = DataManager.GetSpellData(7510);
        public static readonly SpellData Verstone = DataManager.GetSpellData(7511);
        public static readonly SpellData Zwerchhau = DataManager.GetSpellData(7512);
        public static readonly SpellData Moulinet = DataManager.GetSpellData(7513);
        public static readonly SpellData Vercure = DataManager.GetSpellData(7514);
        public static readonly SpellData Displacement = DataManager.GetSpellData(7515);
        public static readonly SpellData Redoublement = DataManager.GetSpellData(7516);
        public static readonly SpellData Fleche = DataManager.GetSpellData(7517);
        public static readonly SpellData Acceleration = DataManager.GetSpellData(7518);
        public static readonly SpellData ContreSixte = DataManager.GetSpellData(7519);
        public static readonly SpellData Embolden = DataManager.GetSpellData(7520);
        public static readonly SpellData Manafication = DataManager.GetSpellData(7521);
        public static readonly SpellData Verraise = DataManager.GetSpellData(7523);
        public static readonly SpellData Jolt2 = DataManager.GetSpellData(7524);
        public static readonly SpellData Verflare = DataManager.GetSpellData(7525);
        public static readonly SpellData Verholy = DataManager.GetSpellData(7526);
        public static readonly SpellData Verthunder2 = DataManager.GetSpellData(16524);
        public static readonly SpellData Veraero2 = DataManager.GetSpellData(16525);
        public static readonly SpellData Impact = DataManager.GetSpellData(16526);
        public static readonly SpellData Engagement = DataManager.GetSpellData(16527);
        public static readonly SpellData Reprise = DataManager.GetSpellData(16529);
        public static readonly SpellData Scorch = DataManager.GetSpellData(16530);
        #endregion

        // SAM
        #region SAM
        public static readonly SpellData Hakaze = DataManager.GetSpellData(7477);
        public static readonly SpellData Shoha = DataManager.GetSpellData(16487);
        public static readonly SpellData Jinpu = DataManager.GetSpellData(7478);
        public static readonly SpellData Shifu = DataManager.GetSpellData(7479);
        public static readonly SpellData Yukikaze = DataManager.GetSpellData(7480);
        public static readonly SpellData Gekko = DataManager.GetSpellData(7481);
        public static readonly SpellData Kasha = DataManager.GetSpellData(7482);
        public static readonly SpellData Fuga = DataManager.GetSpellData(7483);
        public static readonly SpellData Mangetsu = DataManager.GetSpellData(7484);
        public static readonly SpellData Oka = DataManager.GetSpellData(7485);
        public static readonly SpellData Enpi = DataManager.GetSpellData(7486);
        public static readonly SpellData MidareSetsugekka = DataManager.GetSpellData(7487);
        public static readonly SpellData KaeshiSetsugekka = DataManager.GetSpellData(16486);
        public static readonly SpellData TenkaGoken = DataManager.GetSpellData(7488);
        public static readonly SpellData KaeshiGoken = DataManager.GetSpellData(16485);
        public static readonly SpellData Higanbana = DataManager.GetSpellData(7489);
        public static readonly SpellData KaeshiHiganbana = DataManager.GetSpellData(16484);
        public static readonly SpellData HissatsuShinten = DataManager.GetSpellData(7490);
        public static readonly SpellData HissatsuKyuten = DataManager.GetSpellData(7491);
        public static readonly SpellData HissatsuKaiten = DataManager.GetSpellData(7494);
        public static readonly SpellData Ikishoten = DataManager.GetSpellData(16482);
        public static readonly SpellData HissatsuGuren = DataManager.GetSpellData(7496);
        public static readonly SpellData HissatsuSenei = DataManager.GetSpellData(16481);
        public static readonly SpellData Meditate = DataManager.GetSpellData(7497);
        public static readonly SpellData ThirdEye = DataManager.GetSpellData(7498);
        public static readonly SpellData MeikyoShisui = DataManager.GetSpellData(7499);
        public static readonly SpellData HissatsuSeigan = DataManager.GetSpellData(7501);
        public static readonly SpellData Meditation = DataManager.GetSpellData(3546);
        #endregion

        // SCH
        #region SCH
        public static readonly SpellData Aetherflow = DataManager.GetSpellData(166);
        public static readonly SpellData EnergyDrain2 = DataManager.GetSpellData(167);
        public static readonly SpellData Adloquium = DataManager.GetSpellData(185);
        public static readonly SpellData Succor = DataManager.GetSpellData(186);
        public static readonly SpellData SacredSoil = DataManager.GetSpellData(188);
        public static readonly SpellData Lustrate = DataManager.GetSpellData(189);
        public static readonly SpellData Physick = DataManager.GetSpellData(190);
        public static readonly SpellData Indomitability = DataManager.GetSpellData(3583);
        public static readonly SpellData Broil = DataManager.GetSpellData(3584);
        public static readonly SpellData DeploymentTactics = DataManager.GetSpellData(3585);
        public static readonly SpellData EmergencyTactics = DataManager.GetSpellData(3586);
        public static readonly SpellData Dissipation = DataManager.GetSpellData(3587);
        public static readonly SpellData Excogitation = DataManager.GetSpellData(7434);
        public static readonly SpellData Broil2 = DataManager.GetSpellData(7435);
        public static readonly SpellData ChainStrategem = DataManager.GetSpellData(7436);
        public static readonly SpellData Aetherpact = DataManager.GetSpellData(7437);
        public static readonly SpellData DissolveUnion = DataManager.GetSpellData(7869);
        public static readonly SpellData WhisperingDawn = DataManager.GetSpellData(16537);
        public static readonly SpellData FeyIllumination = DataManager.GetSpellData(16538);
        public static readonly SpellData ArtOfWar = DataManager.GetSpellData(16539);
        public static readonly SpellData Biolysis = DataManager.GetSpellData(16540);
        public static readonly SpellData Broil3 = DataManager.GetSpellData(16541);
        public static readonly SpellData Recitation = DataManager.GetSpellData(16542);
        public static readonly SpellData FeyBlessing = DataManager.GetSpellData(16543);
        public static readonly SpellData SummonSeraph = DataManager.GetSpellData(16545);
        public static readonly SpellData Consolation = DataManager.GetSpellData(16546);
        public static readonly SpellData SummonEos = DataManager.GetSpellData(17215);
        public static readonly SpellData SummonSelene = DataManager.GetSpellData(17216);
        #endregion

        // SMN
        #region SMN
        public static readonly SpellData SmnRuin = DataManager.GetSpellData(163);
        public static readonly SpellData SmnBio = DataManager.GetSpellData(164);
        public static readonly SpellData SmnRuin2 = DataManager.GetSpellData(172);
        public static readonly SpellData Bane = DataManager.GetSpellData(174);
        public static readonly SpellData Fester = DataManager.GetSpellData(181);
        public static readonly SpellData Painflare = DataManager.GetSpellData(3578);
        public static readonly SpellData Ruin3 = DataManager.GetSpellData(3579);
        public static readonly SpellData TriDisaster = DataManager.GetSpellData(3580);
        public static readonly SpellData Trance = DataManager.GetSpellData(3581);
        public static readonly SpellData Deathflare = DataManager.GetSpellData(3582);
        public static readonly SpellData SmnAetherpact = DataManager.GetSpellData(7423);
        public static readonly SpellData Bio3 = DataManager.GetSpellData(7424);
        public static readonly SpellData Miasma3 = DataManager.GetSpellData(7425);
        public static readonly SpellData Ruin4 = DataManager.GetSpellData(7426);
        public static readonly SpellData SummonBahamut = DataManager.GetSpellData(7427);
        public static readonly SpellData EnkindleBahamut = DataManager.GetSpellData(7429);
        public static readonly SpellData EnergyDrain = DataManager.GetSpellData(16508);
        public static readonly SpellData EgiAssault = DataManager.GetSpellData(16509);
        public static readonly SpellData EnergySiphon = DataManager.GetSpellData(16510);
        public static readonly SpellData Outburst = DataManager.GetSpellData(16511);
        public static readonly SpellData EgiAssault2 = DataManager.GetSpellData(16512);
        public static readonly SpellData FountainofFire = DataManager.GetSpellData(16514);
        public static readonly SpellData BrandofPurgatory = DataManager.GetSpellData(16515);
        public static readonly SpellData EnkindlePhoenix = DataManager.GetSpellData(16516);
        #endregion

        // WAR
        #region WAR
        public static readonly SpellData HeavySwing = DataManager.GetSpellData(31);
        public static readonly SpellData Maim = DataManager.GetSpellData(37);
        public static readonly SpellData Berserk = DataManager.GetSpellData(38);
        public static readonly SpellData ThrillofBattle = DataManager.GetSpellData(40);
        public static readonly SpellData Overpower = DataManager.GetSpellData(41);
        public static readonly SpellData StormsPath = DataManager.GetSpellData(42);
        public static readonly SpellData Holmgang = DataManager.GetSpellData(43);
        public static readonly SpellData Vengeance = DataManager.GetSpellData(44);
        public static readonly SpellData StormsEye = DataManager.GetSpellData(45);
        public static readonly SpellData Tomahawk = DataManager.GetSpellData(46);
        public static readonly SpellData Defiance = DataManager.GetSpellData(48);
        public static readonly SpellData InnerBeast = DataManager.GetSpellData(49);
        public static readonly SpellData SteelCyclone = DataManager.GetSpellData(51);
        public static readonly SpellData Infuriate = DataManager.GetSpellData(52);
        public static readonly SpellData FellCleave = DataManager.GetSpellData(3549);
        public static readonly SpellData Decimate = DataManager.GetSpellData(3550);
        public static readonly SpellData RawIntuition = DataManager.GetSpellData(3551);
        public static readonly SpellData Equilibrium = DataManager.GetSpellData(3552);
        public static readonly SpellData Onslaught = DataManager.GetSpellData(7386);
        public static readonly SpellData Upheaval = DataManager.GetSpellData(7387);
        public static readonly SpellData ShakeItOff = DataManager.GetSpellData(7388);
        public static readonly SpellData InnerRelease = DataManager.GetSpellData(7389);
        public static readonly SpellData MythrilTempest = DataManager.GetSpellData(16462);
        public static readonly SpellData InnerChaos = DataManager.GetSpellData(16465);
        public static readonly SpellData ChaoticCyclone = DataManager.GetSpellData(16462);
        #endregion

        // WHM
        #region WHM
        public static readonly SpellData Stone = DataManager.GetSpellData(119);
        public static readonly SpellData Cure = DataManager.GetSpellData(120);
        public static readonly SpellData Aero = DataManager.GetSpellData(121);
        public static readonly SpellData Medica = DataManager.GetSpellData(124);
        public static readonly SpellData Raise = DataManager.GetSpellData(125);
        public static readonly SpellData Stone2 = DataManager.GetSpellData(127);
        public static readonly SpellData Cure3 = DataManager.GetSpellData(131);
        public static readonly SpellData Aero2 = DataManager.GetSpellData(132);
        public static readonly SpellData Medica2 = DataManager.GetSpellData(133);
        public static readonly SpellData FluidAura = DataManager.GetSpellData(134);
        public static readonly SpellData Cure2 = DataManager.GetSpellData(135);
        public static readonly SpellData PresenceofMind = DataManager.GetSpellData(136);
        public static readonly SpellData Regen = DataManager.GetSpellData(137);
        public static readonly SpellData Holy = DataManager.GetSpellData(139);
        public static readonly SpellData Benediction = DataManager.GetSpellData(140);
        public static readonly SpellData Stone3 = DataManager.GetSpellData(3568);
        public static readonly SpellData Asylum = DataManager.GetSpellData(3569);
        public static readonly SpellData Tetragrammaton = DataManager.GetSpellData(3570);
        public static readonly SpellData Assize = DataManager.GetSpellData(3571);
        public static readonly SpellData ThinAir = DataManager.GetSpellData(7430);
        public static readonly SpellData Stone4 = DataManager.GetSpellData(7431);
        public static readonly SpellData DivineBenison = DataManager.GetSpellData(7432);
        public static readonly SpellData PlenaryIndulgence = DataManager.GetSpellData(7433);
        public static readonly SpellData AfflatusSolace = DataManager.GetSpellData(16531);
        public static readonly SpellData Dia = DataManager.GetSpellData(16532);
        public static readonly SpellData Glare = DataManager.GetSpellData(16533);
        public static readonly SpellData AfflatusRapture = DataManager.GetSpellData(16534);
        public static readonly SpellData AfflatusMisery = DataManager.GetSpellData(16535);
        public static readonly SpellData Temperance = DataManager.GetSpellData(16536);
        #endregion

        #region PVP
        public static readonly SpellData Concentrate = DataManager.GetSpellData(1582);
        public static readonly SpellData Muse = DataManager.GetSpellData(1583);
        public static readonly SpellData Safeguard = DataManager.GetSpellData(1585);
        public static readonly SpellData Enliven = DataManager.GetSpellData(1580);
        public static readonly SpellData Recuperate = DataManager.GetSpellData(1590);
        public static readonly SpellData Testudo = DataManager.GetSpellData(1558);
        public static readonly SpellData GlorySlash = DataManager.GetSpellData(1559);
        public static readonly SpellData FullSwing = DataManager.GetSpellData(1562);
        public static readonly SpellData PushBack = DataManager.GetSpellData(1597);
        public static readonly SpellData EmpyreanRain = DataManager.GetSpellData(3362);

        public static readonly SpellData PvpMalefic3 = DataManager.GetSpellData(8912);
        public static readonly SpellData PvpEssentialDignity = DataManager.GetSpellData(8916);
        public static readonly SpellData PvpLightspeed = DataManager.GetSpellData(8917);
        public static readonly SpellData PvpSynastry = DataManager.GetSpellData(8918);
        public static readonly SpellData PvpBenefic = DataManager.GetSpellData(8913);
        public static readonly SpellData PvpBenefic2 = DataManager.GetSpellData(8914);
        public static readonly SpellData Deorbit = DataManager.GetSpellData(9466);
        public static readonly SpellData PvpDisable = DataManager.GetSpellData(9623);
        public static readonly SpellData PvpDraw = DataManager.GetSpellData(10026);
        public static readonly SpellData PvpPlayDrawn = DataManager.GetSpellData(10026);

        public static readonly SpellData StraightShotPvp = DataManager.GetSpellData(8835);
        public static readonly SpellData EmpyrealArrowPvp = DataManager.GetSpellData(8838);
        public static readonly SpellData RepellingShotPvp = DataManager.GetSpellData(8839);
        public static readonly SpellData BloodletterPvp = DataManager.GetSpellData(9624);
        public static readonly SpellData SidewinderPvp = DataManager.GetSpellData(8841);
        public static readonly SpellData BarragePvp = DataManager.GetSpellData(9625);
        public static readonly SpellData PitchPerfectPvp = DataManager.GetSpellData(8842);
        public static readonly SpellData TheWanderersMinuetPvp = DataManager.GetSpellData(8843);
        public static readonly SpellData ArmysPaeonPvp = DataManager.GetSpellData(8844);
        public static readonly SpellData TroubadourPvp = DataManager.GetSpellData(10023);

        //WHM
        public static readonly SpellData Purify = DataManager.GetSpellData(1584);
        public static readonly SpellData Stone3Pvp = DataManager.GetSpellData(8894);
        public static readonly SpellData CurePvp = DataManager.GetSpellData(8895);
        public static readonly SpellData Cure2Pvp = DataManager.GetSpellData(8896);
        public static readonly SpellData RegenPvp = DataManager.GetSpellData(8898);
        public static readonly SpellData DivineBenisonPvp = DataManager.GetSpellData(9621);
        public static readonly SpellData AssizePvp = DataManager.GetSpellData(9620);
        public static readonly SpellData FluidAuraPvp = DataManager.GetSpellData(8900);
        #endregion
    }
}