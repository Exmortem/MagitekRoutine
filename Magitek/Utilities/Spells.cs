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

        // Tank Limit Breaks
        #region Tank Limit Breaks
        public static readonly SpellData ShieldWall = DataManager.GetSpellData(197);
        public static readonly SpellData Stronghold = DataManager.GetSpellData(198);
        #endregion

        // Melee DPS Limit Breaks
        #region Melee DPS Limit Breaks
        public static readonly SpellData Braver = DataManager.GetSpellData(200);
        public static readonly SpellData Bladedance = DataManager.GetSpellData(201);
        #endregion

        // Ranged DPS Limit Breaks
        #region Ranged DPS Limit Breaks
        public static readonly SpellData BigShot = DataManager.GetSpellData(4238);
        public static readonly SpellData Desperado = DataManager.GetSpellData(4239);
        #endregion

        // Magic DPS Limit Breaks
        #region Magic DPS Limit Breaks
        public static readonly SpellData Skyshard = DataManager.GetSpellData(203);
        public static readonly SpellData Starstorm = DataManager.GetSpellData(204);
        #endregion

        // Healer Limit Breaks
        #region Healer Limit Breaks
        public static readonly SpellData HealingWind = DataManager.GetSpellData(206);
        public static readonly SpellData BreathoftheEarth = DataManager.GetSpellData(207);
        #endregion

        // ACN
        #region ACN
        public static readonly SpellData Ruin = DataManager.GetSpellData(163);
        public static readonly SpellData SummonCarbuncle = DataManager.GetSpellData(25798);
        public static readonly SpellData RadiantAegis = DataManager.GetSpellData(25799);
        public static readonly SpellData Aethercharge = DataManager.GetSpellData(25800);
        public static readonly SpellData SummonRuby = DataManager.GetSpellData(25802);
        public static readonly SpellData Gemshine = DataManager.GetSpellData(25883);
        public static readonly SpellData RubyRuin = DataManager.GetSpellData(25808);
        public static readonly SpellData TopazRuin = DataManager.GetSpellData(25809);
        public static readonly SpellData EmeraldRuin = DataManager.GetSpellData(25810);
        public static readonly SpellData RubyRuinII = DataManager.GetSpellData(25811);
        public static readonly SpellData TopazRuinII = DataManager.GetSpellData(25812);
        public static readonly SpellData EmeraldRuinII = DataManager.GetSpellData(25813);
        public static readonly SpellData Fester = DataManager.GetSpellData(181);
        public static readonly SpellData EnergyDrain = DataManager.GetSpellData(16508);
        public static readonly SpellData Resurrection = DataManager.GetSpellData(173);
        public static readonly SpellData SummonTopaz = DataManager.GetSpellData(25803);
        public static readonly SpellData SummonEmerald = DataManager.GetSpellData(25804);
        public static readonly SpellData Outburst = DataManager.GetSpellData(16511);
        public static readonly SpellData PreciousBrilliance = DataManager.GetSpellData(25884);
        public static readonly SpellData RubyOutburst = DataManager.GetSpellData(25814);
        public static readonly SpellData TopazOutburst = DataManager.GetSpellData(25815);
        public static readonly SpellData EmeraldOutburst = DataManager.GetSpellData(25816);
        public static readonly SpellData Ruin2 = DataManager.GetSpellData(172);
        public static readonly SpellData SmnPhysick = DataManager.GetSpellData(16230);


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
        public static readonly SpellData Astrodyne = DataManager.GetSpellData(25870);
        public static readonly SpellData CrownPlay = DataManager.GetSpellData(25869);
        public static readonly SpellData Exaltation = DataManager.GetSpellData(25873);
        public static readonly SpellData Macrocosmos = DataManager.GetSpellData(25874);
        public static readonly SpellData Microcosmos = DataManager.GetSpellData(25874);
        public static readonly SpellData FallMalefic = DataManager.GetSpellData(25871);
        public static readonly SpellData GravityII = DataManager.GetSpellData(25872);
        public static readonly SpellData AstralStasis = DataManager.GetSpellData(4248);
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
        public static readonly SpellData Blizzard2 = DataManager.GetSpellData(25793);
        public static readonly SpellData HighFireII = DataManager.GetSpellData(25794);
        public static readonly SpellData HighBlizzardII = DataManager.GetSpellData(25795);
        public static readonly SpellData Amplifier = DataManager.GetSpellData(25796);
        public static readonly SpellData Paradox = DataManager.GetSpellData(25797);
        public static readonly SpellData Meteor = DataManager.GetSpellData(205);
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
        public static readonly SpellData Ladonsbite = DataManager.GetSpellData(25783);
        public static readonly SpellData BlastArrow = DataManager.GetSpellData(25784);

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
        public static readonly SpellData RadiantFinale = DataManager.GetSpellData(25785);

        //Songs

        public static readonly SpellData MagesBallad = DataManager.GetSpellData(114);
        public static readonly SpellData ArmysPaeon = DataManager.GetSpellData(116);
        public static readonly SpellData TheWanderersMinuet = DataManager.GetSpellData(3559);

        //Utility/Movement

        public static readonly SpellData RepellingShot = DataManager.GetSpellData(112);
        public static readonly SpellData TheWardensPaean = DataManager.GetSpellData(3561);
        public static readonly SpellData Troubadour = DataManager.GetSpellData(7405);
        public static readonly SpellData NaturesMinne = DataManager.GetSpellData(7408);

        // LB3
        public static readonly SpellData SaggitariusArrow = DataManager.GetSpellData(4244);

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
        public static readonly SpellData FanDanceIV = DataManager.GetSpellData(25791);
        public static readonly SpellData StarfallDance = DataManager.GetSpellData(25792);
        public static readonly SpellData Tillana = DataManager.GetSpellData(25790);
        public static readonly SpellData CrimsonLotus = DataManager.GetSpellData(17106);
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
        public static readonly SpellData HeavensThrust = DataManager.GetSpellData(25771);
        public static readonly SpellData ChaoticSpring = DataManager.GetSpellData(25772);
        public static readonly SpellData WyrmwindThrust = DataManager.GetSpellData(25773);
        public static readonly SpellData DraconianFury = DataManager.GetSpellData(25770);
        public static readonly SpellData DragonsongDive = DataManager.GetSpellData(4242);
        #endregion

        // DRK
        #region DRK
        public static readonly SpellData HardSlash = DataManager.GetSpellData(3617);
        public static readonly SpellData Unleash = DataManager.GetSpellData(3621);
        public static readonly SpellData SyphonStrike = DataManager.GetSpellData(3623);
        public static readonly SpellData Unmend = DataManager.GetSpellData(3624);
        public static readonly SpellData BloodWeapon = DataManager.GetSpellData(3625);
        public static readonly SpellData Grit = DataManager.GetSpellData(3629);
        public static readonly SpellData ReleaseGrit = DataManager.GetSpellData(32067);
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
        public static readonly SpellData Oblation = DataManager.GetSpellData(25754);
        public static readonly SpellData Shadowbringer = DataManager.GetSpellData(25757);
        public static readonly SpellData DarkForce = DataManager.GetSpellData(4241);
        #endregion

        // GNB
        #region GNB
        public static readonly SpellData KeenEdge = DataManager.GetSpellData(16137);
        public static readonly SpellData NoMercy = DataManager.GetSpellData(16138);
        public static readonly SpellData BrutalShell = DataManager.GetSpellData(16139);
        public static readonly SpellData Camouflage = DataManager.GetSpellData(16140);
        public static readonly SpellData DemonSlice = DataManager.GetSpellData(16141);
        public static readonly SpellData RoyalGuard = DataManager.GetSpellData(16142);
        public static readonly SpellData ReleaseRoyalGuard = DataManager.GetSpellData(32068);
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
        public static readonly SpellData HeartOfCorundum = DataManager.GetSpellData(25758);
        public static readonly SpellData DoubleDown = DataManager.GetSpellData(25760);
        public static readonly SpellData Hypervelocity = DataManager.GetSpellData(25759);
        public static readonly SpellData GunmetalSoul = DataManager.GetSpellData(17105);
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
        public static readonly SpellData Scattergun = DataManager.GetSpellData(25786);
        public static readonly SpellData ChainSaw = DataManager.GetSpellData(25788);
        public static readonly SpellData SatelliteBeam = DataManager.GetSpellData(4245);
        public static readonly SpellData Dismantle = DataManager.GetSpellData(2287);

        #endregion

        // MNK
        #region MNK
        public static readonly SpellData ArmOfTheDestroyer = DataManager.GetSpellData(62);
        public static readonly SpellData Bootshine = DataManager.GetSpellData(53);
        public static readonly SpellData TrueStrike = DataManager.GetSpellData(54);
        public static readonly SpellData SnapPunch = DataManager.GetSpellData(56);
        //public static readonly SpellData FistsOfEarth = DataManager.GetSpellData(60);
        public static readonly SpellData TwinSnakes = DataManager.GetSpellData(61);
        public static readonly SpellData Demolish = DataManager.GetSpellData(66);
        public static readonly SpellData Rockbreaker = DataManager.GetSpellData(70);
        //public static readonly SpellData FistsOfWind = DataManager.GetSpellData(73);
        //public static readonly SpellData ShoulderTackle = DataManager.GetSpellData(71);
        //public static readonly SpellData FistsOfFire = DataManager.GetSpellData(63);
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
        public static readonly SpellData TornadoKick = DataManager.GetSpellData(3543);
        public static readonly SpellData MasterfulBlitz = DataManager.GetSpellData(25764);
        public static readonly SpellData ShadowOfTheDestroyer = DataManager.GetSpellData(25767);
        public static readonly SpellData FinalHeaven = DataManager.GetSpellData(202);
        public static readonly SpellData Thunderclap = DataManager.GetSpellData(25762);
        public static readonly SpellData SteelPeak = DataManager.GetSpellData(25761);
        public static readonly SpellData HowlingFist = DataManager.GetSpellData(25763);
        public static readonly SpellData CelestialRevolution = DataManager.GetSpellData(25765);
        public static readonly SpellData RiddleofWind = DataManager.GetSpellData(25766);
        public static readonly SpellData RisingPhoenix = DataManager.GetSpellData(25768);
        public static readonly SpellData PhantomRush = DataManager.GetSpellData(25769);
        public static readonly SpellData FlintStrike = DataManager.GetSpellData(25882);
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
        public static readonly SpellData PhantomKamaitachi = DataManager.GetSpellData(25774);
        public static readonly SpellData ForkedRaiju = DataManager.GetSpellData(25777);
        public static readonly SpellData FleetingRaiju = DataManager.GetSpellData(25778);
        public static readonly SpellData Huraijin = DataManager.GetSpellData(25876);
        public static readonly SpellData LimitBreak = DataManager.GetSpellData(209);
        public static readonly SpellData Chimatsuri = DataManager.GetSpellData(4243);
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
        public static readonly SpellData ReleaseIronWill = DataManager.GetSpellData(32065);
        public static readonly SpellData Clemency = DataManager.GetSpellData(3541);
        public static readonly SpellData Bulwark = DataManager.GetSpellData(22);
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
        public static readonly SpellData Prominence = DataManager.GetSpellData(16457);
        public static readonly SpellData HolyCircle = DataManager.GetSpellData(16458);
        public static readonly SpellData Intervene = DataManager.GetSpellData(16461);
        public static readonly SpellData Atonement = DataManager.GetSpellData(16460);
        public static readonly SpellData Confiteor = DataManager.GetSpellData(16459);
        public static readonly SpellData HolySheltron = DataManager.GetSpellData(25746);
        public static readonly SpellData Expiacion = DataManager.GetSpellData(25747);
        public static readonly SpellData BladeOfFaith = DataManager.GetSpellData(25748);
        public static readonly SpellData BladeOfTruth = DataManager.GetSpellData(25749);
        public static readonly SpellData BladeOfValor = DataManager.GetSpellData(25750);
        public static readonly SpellData LastBastion = DataManager.GetSpellData(199);
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
        public static readonly SpellData EnchantedRiposte = DataManager.GetSpellData(7527);
        public static readonly SpellData EnchantedZwerchhau = DataManager.GetSpellData(7528);
        public static readonly SpellData EnchantedRedoublement = DataManager.GetSpellData(7529);
        public static readonly SpellData EnchantedMoulinet = DataManager.GetSpellData(7530);
        public static readonly SpellData Verthunder2 = DataManager.GetSpellData(16524);
        public static readonly SpellData Veraero2 = DataManager.GetSpellData(16525);
        public static readonly SpellData Impact = DataManager.GetSpellData(16526);
        public static readonly SpellData Engagement = DataManager.GetSpellData(16527);
        public static readonly SpellData Reprise = DataManager.GetSpellData(16528);
        public static readonly SpellData Scorch = DataManager.GetSpellData(16530);
        public static readonly SpellData Resolution = DataManager.GetSpellData(25858);
        public static readonly SpellData VerthunderIII = DataManager.GetSpellData(25855);
        public static readonly SpellData VeraeroIII = DataManager.GetSpellData(25856);
        public static readonly SpellData MagickBarrier = DataManager.GetSpellData(25857);
        public static readonly SpellData VermillionScourge = DataManager.GetSpellData(7862);
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
        //public static readonly SpellData HissatsuKaiten = DataManager.GetSpellData(7492);
        public static readonly SpellData HissatsuYaten = DataManager.GetSpellData(7493);
        public static readonly SpellData HissatsuGyoten = DataManager.GetSpellData(7494);
        public static readonly SpellData Hagakure = DataManager.GetSpellData(7495);
        public static readonly SpellData Ikishoten = DataManager.GetSpellData(16482);
        public static readonly SpellData HissatsuGuren = DataManager.GetSpellData(7496);
        public static readonly SpellData HissatsuSenei = DataManager.GetSpellData(16481);
        public static readonly SpellData TsubameGaeshi = DataManager.GetSpellData(16483);
        public static readonly SpellData Meditate = DataManager.GetSpellData(7497);
        public static readonly SpellData ThirdEye = DataManager.GetSpellData(7498);
        public static readonly SpellData MeikyoShisui = DataManager.GetSpellData(7499);
        //public static readonly SpellData HissatsuSeigan = DataManager.GetSpellData(7501);
        public static readonly SpellData Meditation = DataManager.GetSpellData(3546);
        public static readonly SpellData ShohaII = DataManager.GetSpellData(25779);
        public static readonly SpellData Fuko = DataManager.GetSpellData(25780);
        public static readonly SpellData OgiNamikiri = DataManager.GetSpellData(25781);
        public static readonly SpellData KaeshiNamikiri = DataManager.GetSpellData(25782);
        public static readonly SpellData DoomoftheLiving = DataManager.GetSpellData(7861);
        #endregion

        // SGE
        #region SGE
        public static readonly SpellData Dosis = DataManager.GetSpellData(24283);
        public static readonly SpellData Diagnosis = DataManager.GetSpellData(24284);
        public static readonly SpellData Kardia = DataManager.GetSpellData(24285);
        public static readonly SpellData Prognosis = DataManager.GetSpellData(24286);
        public static readonly SpellData Egeiro = DataManager.GetSpellData(24287);
        public static readonly SpellData Physis = DataManager.GetSpellData(24288);
        public static readonly SpellData PhysisII = DataManager.GetSpellData(24302);
        public static readonly SpellData Phlegma = DataManager.GetSpellData(24289);
        public static readonly SpellData PhlegmaII = DataManager.GetSpellData(24307);
        public static readonly SpellData PhlegmaIII = DataManager.GetSpellData(24313);
        public static readonly SpellData Eukrasia = DataManager.GetSpellData(24290);
        public static readonly SpellData EukrasianDiagnosis = DataManager.GetSpellData(24291);
        public static readonly SpellData EukrasianPrognosis = DataManager.GetSpellData(24292);
        public static readonly SpellData EukrasianDosis = DataManager.GetSpellData(24293);
        public static readonly SpellData Soteria = DataManager.GetSpellData(24294);
        public static readonly SpellData Druochole = DataManager.GetSpellData(24296);
        public static readonly SpellData Dyskrasia = DataManager.GetSpellData(24297);
        public static readonly SpellData Kerachole = DataManager.GetSpellData(24298);
        public static readonly SpellData Ixochole = DataManager.GetSpellData(24299);
        public static readonly SpellData Zoe = DataManager.GetSpellData(24300);
        public static readonly SpellData Pepsis = DataManager.GetSpellData(24301);
        public static readonly SpellData Taurochole = DataManager.GetSpellData(24303);
        public static readonly SpellData Toxikon = DataManager.GetSpellData(24304);
        public static readonly SpellData ToxikonII = DataManager.GetSpellData(24316);
        public static readonly SpellData Haima = DataManager.GetSpellData(24305);
        public static readonly SpellData DosisII = DataManager.GetSpellData(24306);
        public static readonly SpellData EukrasianDosisII = DataManager.GetSpellData(24308);
        public static readonly SpellData Rhizomata = DataManager.GetSpellData(24309);
        public static readonly SpellData Holos = DataManager.GetSpellData(24310);
        public static readonly SpellData Panhaima = DataManager.GetSpellData(24311);
        public static readonly SpellData DosisIII = DataManager.GetSpellData(24312);
        public static readonly SpellData EukrasianDosisIII = DataManager.GetSpellData(24314);
        public static readonly SpellData DyskrasiaII = DataManager.GetSpellData(24315);
        public static readonly SpellData Krasis = DataManager.GetSpellData(24317);
        public static readonly SpellData Pneuma = DataManager.GetSpellData(24318);
        public static readonly SpellData TechneMakre = DataManager.GetSpellData(24859);
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
        public static readonly SpellData SchRuin = DataManager.GetSpellData(17869);
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
        public static readonly SpellData BroilIV = DataManager.GetSpellData(25865);
        public static readonly SpellData ArtOfWarII = DataManager.GetSpellData(25866);
        public static readonly SpellData Protraction = DataManager.GetSpellData(25867);
        public static readonly SpellData Expedient = DataManager.GetSpellData(25868);
        public static readonly SpellData Bio = DataManager.GetSpellData(17864);
        public static readonly SpellData Enkindle = DataManager.GetSpellData(184);
        public static readonly SpellData AngelFeathers = DataManager.GetSpellData(4247);
        #endregion

        // SMN
        #region SMN
        public static readonly SpellData SummonIfrit = DataManager.GetSpellData(25805);
        public static readonly SpellData SummonTitan = DataManager.GetSpellData(25806);
        public static readonly SpellData Painflare = DataManager.GetSpellData(3578);
        public static readonly SpellData SummonGaruda = DataManager.GetSpellData(25807);
        public static readonly SpellData EnergySiphon = DataManager.GetSpellData(16510);
        public static readonly SpellData Ruin3 = DataManager.GetSpellData(3579);
        public static readonly SpellData RubyRuinIII = DataManager.GetSpellData(25817);
        public static readonly SpellData TopazRuinIII = DataManager.GetSpellData(25818);
        public static readonly SpellData EmeraldRuinIII = DataManager.GetSpellData(25819);
        public static readonly SpellData DreadwyrmTrance = DataManager.GetSpellData(3581);
        public static readonly SpellData AstralFlow = DataManager.GetSpellData(25822);
        public static readonly SpellData Ruin4 = DataManager.GetSpellData(7426);
        public static readonly SpellData SearingLight = DataManager.GetSpellData(25801);
        public static readonly SpellData SummonBahamut = DataManager.GetSpellData(7427);
        public static readonly SpellData EnkindleBahamut = DataManager.GetSpellData(7429);
        public static readonly SpellData TriDisaster = DataManager.GetSpellData(25826);
        public static readonly SpellData SummonIfrit2 = DataManager.GetSpellData(25838);
        public static readonly SpellData SummonTitan2 = DataManager.GetSpellData(25839);
        public static readonly SpellData SummonGaruda2 = DataManager.GetSpellData(25840);
        public static readonly SpellData AstralImpulse = DataManager.GetSpellData(25820);
        public static readonly SpellData AstralFlare = DataManager.GetSpellData(25821);
        public static readonly SpellData Deathflare = DataManager.GetSpellData(3582);
        public static readonly SpellData Wyrmwave = DataManager.GetSpellData(7428);
        public static readonly SpellData AkhMorn = DataManager.GetSpellData(3010);
        public static readonly SpellData RubyRite = DataManager.GetSpellData(25823);
        public static readonly SpellData TopazRite = DataManager.GetSpellData(25824);
        public static readonly SpellData EmeraldRite = DataManager.GetSpellData(25825);
        public static readonly SpellData FountainofFire = DataManager.GetSpellData(16514);
        public static readonly SpellData BrandofPurgatory = DataManager.GetSpellData(16515);
        public static readonly SpellData SummonPhoenix = DataManager.GetSpellData(25831);
        public static readonly SpellData EverlastingFlight = DataManager.GetSpellData(16517);
        public static readonly SpellData Rekindle = DataManager.GetSpellData(25830);
        public static readonly SpellData ScarletFlame = DataManager.GetSpellData(16508);
        public static readonly SpellData EnkindlePhoenix = DataManager.GetSpellData(16516);
        public static readonly SpellData Revelation = DataManager.GetSpellData(2951);
        public static readonly SpellData RubyDisaster = DataManager.GetSpellData(25827);
        public static readonly SpellData TopazDisaster = DataManager.GetSpellData(25828);
        public static readonly SpellData EmeraldDisaster = DataManager.GetSpellData(25829);
        public static readonly SpellData RubyCatastrophe = DataManager.GetSpellData(25832);
        public static readonly SpellData TopazCatastrophe = DataManager.GetSpellData(25833);
        public static readonly SpellData EmeraldCatastrophe = DataManager.GetSpellData(25834);
        public static readonly SpellData CrimsonCyclone = DataManager.GetSpellData(25835);
        public static readonly SpellData CrimsonStrike = DataManager.GetSpellData(25885);
        public static readonly SpellData MountainBuster = DataManager.GetSpellData(25836);
        public static readonly SpellData Slipstream = DataManager.GetSpellData(25837);
        public static readonly SpellData Teraflare = DataManager.GetSpellData(4246);
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
        public static readonly SpellData ReleaseDefiance = DataManager.GetSpellData(32066);
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
        public static readonly SpellData ChaoticCyclone = DataManager.GetSpellData(16463);
        public static readonly SpellData Bloodwhetting = DataManager.GetSpellData(25751);
        public static readonly SpellData Orogeny = DataManager.GetSpellData(25752);
        public static readonly SpellData PrimalRend = DataManager.GetSpellData(25753);
        public static readonly SpellData NascentFlash = DataManager.GetSpellData(16464);
        public static readonly SpellData LandWaker = DataManager.GetSpellData(4240);
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
        //public static readonly SpellData FluidAura = DataManager.GetSpellData(134);
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
        public static readonly SpellData GlareIII = DataManager.GetSpellData(25859);
        public static readonly SpellData HolyIII = DataManager.GetSpellData(25860);
        public static readonly SpellData Aquaveil = DataManager.GetSpellData(25861);
        public static readonly SpellData LiturgyOfTheBell = DataManager.GetSpellData(25862);
        public static readonly SpellData PulseofLife = DataManager.GetSpellData(208);
        #endregion

        // BLU
        #region BLU
        public static readonly SpellData Snort = DataManager.GetSpellData(11383);
        public static readonly SpellData FourTonzWeight = DataManager.GetSpellData(11384);
        public static readonly SpellData WaterCannon = DataManager.GetSpellData(11385);
        public static readonly SpellData SongOfTorment = DataManager.GetSpellData(11386);
        public static readonly SpellData HighVoltage = DataManager.GetSpellData(11387);
        public static readonly SpellData BadBreath = DataManager.GetSpellData(11388);
        public static readonly SpellData FlyingFrenzy = DataManager.GetSpellData(11389);
        public static readonly SpellData AquaBreath = DataManager.GetSpellData(11390);
        public static readonly SpellData Plaincracker = DataManager.GetSpellData(11391);
        public static readonly SpellData AcornBomb = DataManager.GetSpellData(11392);
        public static readonly SpellData Bristle = DataManager.GetSpellData(11393);
        public static readonly SpellData MindBlast = DataManager.GetSpellData(11394);
        public static readonly SpellData BloodDrain = DataManager.GetSpellData(11395);
        public static readonly SpellData BombToss = DataManager.GetSpellData(11396);
        public static readonly SpellData ThousandNeedles = DataManager.GetSpellData(11397);
        public static readonly SpellData DrillCannons = DataManager.GetSpellData(11398);
        public static readonly SpellData TheLook = DataManager.GetSpellData(11399);
        public static readonly SpellData SharpKnife = DataManager.GetSpellData(11400);
        public static readonly SpellData Loom = DataManager.GetSpellData(11401);
        public static readonly SpellData FlameThrower = DataManager.GetSpellData(11402);
        public static readonly SpellData Faze = DataManager.GetSpellData(11403);
        public static readonly SpellData Glower = DataManager.GetSpellData(11404);
        public static readonly SpellData Missile = DataManager.GetSpellData(11405);
        public static readonly SpellData WhiteWind = DataManager.GetSpellData(11406);
        public static readonly SpellData FinalSting = DataManager.GetSpellData(11407);
        public static readonly SpellData SelfDestruct = DataManager.GetSpellData(11408);
        public static readonly SpellData Transfusion = DataManager.GetSpellData(11409);
        public static readonly SpellData ToadOil = DataManager.GetSpellData(11410);
        public static readonly SpellData OffGuard = DataManager.GetSpellData(11411);
        public static readonly SpellData StickyTong = DataManager.GetSpellData(11412);
        public static readonly SpellData TailScrew = DataManager.GetSpellData(11413);
        public static readonly SpellData LevelFivePetrify = DataManager.GetSpellData(11414);
        public static readonly SpellData MoonFlute = DataManager.GetSpellData(11415);
        public static readonly SpellData Doom = DataManager.GetSpellData(11416);
        public static readonly SpellData MightyGuard = DataManager.GetSpellData(11417);
        public static readonly SpellData IceSpikes = DataManager.GetSpellData(11418);
        public static readonly SpellData TheRamVoice = DataManager.GetSpellData(11419);
        public static readonly SpellData TheDragonVoice = DataManager.GetSpellData(11420);
        public static readonly SpellData PeculiarLight = DataManager.GetSpellData(11421);
        public static readonly SpellData InkJet = DataManager.GetSpellData(11422);
        public static readonly SpellData FlyingSardine = DataManager.GetSpellData(11423);
        public static readonly SpellData DiamondBack = DataManager.GetSpellData(11424);
        public static readonly SpellData FireAngon = DataManager.GetSpellData(11425);
        public static readonly SpellData FeatherRain = DataManager.GetSpellData(11426);
        public static readonly SpellData Eruption = DataManager.GetSpellData(11427);
        public static readonly SpellData BluMountainBuster = DataManager.GetSpellData(11428);
        public static readonly SpellData ShockStrike = DataManager.GetSpellData(11429);
        public static readonly SpellData GlassDance = DataManager.GetSpellData(11430);
        public static readonly SpellData VeilOfTheWhorl = DataManager.GetSpellData(11431);
        public static readonly SpellData AlpineDraft = DataManager.GetSpellData(18295);
        public static readonly SpellData ProteanWave = DataManager.GetSpellData(18296);
        public static readonly SpellData Northerlies = DataManager.GetSpellData(118297);
        public static readonly SpellData Electrogenesis = DataManager.GetSpellData(18298);
        public static readonly SpellData Kaltstrahl = DataManager.GetSpellData(18299);
        public static readonly SpellData AbyssalTransfixion = DataManager.GetSpellData(18300);
        public static readonly SpellData Chirp = DataManager.GetSpellData(18301);
        public static readonly SpellData EerieSoundwave = DataManager.GetSpellData(18302);
        public static readonly SpellData PomCure = DataManager.GetSpellData(18303);
        public static readonly SpellData GobSkin = DataManager.GetSpellData(18304);
        public static readonly SpellData MagicHammer = DataManager.GetSpellData(18305);
        public static readonly SpellData Avail = DataManager.GetSpellData(18306);
        public static readonly SpellData FrogLegs = DataManager.GetSpellData(18307);
        public static readonly SpellData SonicBoom = DataManager.GetSpellData(18308);
        public static readonly SpellData Whistle = DataManager.GetSpellData(18309);
        public static readonly SpellData WhiteKnightsTour = DataManager.GetSpellData(18310);
        public static readonly SpellData BlackKnightsTour = DataManager.GetSpellData(18311);
        public static readonly SpellData LevelFiveDeath = DataManager.GetSpellData(18312);
        public static readonly SpellData Launcher = DataManager.GetSpellData(18313);
        public static readonly SpellData PerpetualRay = DataManager.GetSpellData(18314);
        public static readonly SpellData Cactguard = DataManager.GetSpellData(18315);
        public static readonly SpellData RevengeBlast = DataManager.GetSpellData(18316);
        public static readonly SpellData AngelWhisper = DataManager.GetSpellData(18317);
        public static readonly SpellData Exuviation = DataManager.GetSpellData(18318);
        public static readonly SpellData Reflux = DataManager.GetSpellData(18319);
        public static readonly SpellData Devour = DataManager.GetSpellData(18320);
        public static readonly SpellData CondensedLibra = DataManager.GetSpellData(18321);
        public static readonly SpellData AetherialMimicry = DataManager.GetSpellData(18322);
        public static readonly SpellData Surpanakha = DataManager.GetSpellData(18323);
        public static readonly SpellData Quasar = DataManager.GetSpellData(18324);
        public static readonly SpellData JKick = DataManager.GetSpellData(18325);
        public static readonly SpellData TripleTrident = DataManager.GetSpellData(23264);
        public static readonly SpellData Tingle = DataManager.GetSpellData(23265);
        public static readonly SpellData TatamiGaeshi = DataManager.GetSpellData(23266);
        public static readonly SpellData ColdFog = DataManager.GetSpellData(23267);
        public static readonly SpellData WhiteDeath = DataManager.GetSpellData(23268);
        public static readonly SpellData SaintlyBeam = DataManager.GetSpellData(23270);
        public static readonly SpellData FeculentFlood = DataManager.GetSpellData(23271);
        public static readonly SpellData AngelsSnack = DataManager.GetSpellData(23272);
        public static readonly SpellData ChelonianGate = DataManager.GetSpellData(23273);
        public static readonly SpellData DivineCataract = DataManager.GetSpellData(23274);
        public static readonly SpellData TheRoseOfDestruction = DataManager.GetSpellData(23275);
        public static readonly SpellData BasicInstinct = DataManager.GetSpellData(23276);
        public static readonly SpellData Ultravibration = DataManager.GetSpellData(23277);
        public static readonly SpellData Blaze = DataManager.GetSpellData(23278);
        public static readonly SpellData MustardBomb = DataManager.GetSpellData(23279);
        public static readonly SpellData DragonForce = DataManager.GetSpellData(23280);
        public static readonly SpellData AetherialSpark = DataManager.GetSpellData(23281);
        public static readonly SpellData HydroPull = DataManager.GetSpellData(23282);
        public static readonly SpellData MaledictionOfWater = DataManager.GetSpellData(23283);
        public static readonly SpellData ChocoMeteor = DataManager.GetSpellData(23284);
        public static readonly SpellData MatraMagic = DataManager.GetSpellData(23285);
        public static readonly SpellData PeripheralSynthesis = DataManager.GetSpellData(23286);
        public static readonly SpellData BothEnds = DataManager.GetSpellData(23287);
        public static readonly SpellData PhantomFlurry = DataManager.GetSpellData(23288);
        public static readonly SpellData PhantomFlurryEnd = DataManager.GetSpellData(23289);
        public static readonly SpellData NightBloom = DataManager.GetSpellData(23290);
        public static readonly SpellData Stotram = DataManager.GetSpellData(23416);
        #endregion

        // RPR
        #region RPR
        public static readonly SpellData Slice = DataManager.GetSpellData(24373); // [24373, Slice]
        public static readonly SpellData WaxingSlice = DataManager.GetSpellData(24374); // [24374, Waxing Slice]
        public static readonly SpellData InfernalSlice = DataManager.GetSpellData(24375); // [24375, Infernal Slice]
        public static readonly SpellData SpinningScythe = DataManager.GetSpellData(24376); // [24376, Spinning Scythe]
        public static readonly SpellData NightmareScythe = DataManager.GetSpellData(24377); // [24377, Nightmare Scythe]
        public static readonly SpellData ShadowOfDeath = DataManager.GetSpellData(24378); // [24378, Shadow of Death]
        public static readonly SpellData WhorlOfDeath = DataManager.GetSpellData(24379); // [24379, Whorl of Death]
        public static readonly SpellData SoulSlice = DataManager.GetSpellData(24380); // [24380, Soul Slice]
        public static readonly SpellData SoulScythe = DataManager.GetSpellData(24381); // [24380, Soul Slice]
        public static readonly SpellData Gibbet = DataManager.GetSpellData(24382); // [24382, Gibbet]
        public static readonly SpellData Gallows = DataManager.GetSpellData(24383); // [24383, Gallows]
        public static readonly SpellData Guillotine = DataManager.GetSpellData(24384); // [24384, Guillotine]
        public static readonly SpellData PlentifulHarvest = DataManager.GetSpellData(24385); // [24385, Plentiful Harvest]
        public static readonly SpellData Harpe = DataManager.GetSpellData(24386); // [24386, Harpe]
        public static readonly SpellData Soulsow = DataManager.GetSpellData(24387); // [24387, Soulsow]
        public static readonly SpellData HarvestMoon = DataManager.GetSpellData(24388); // [24388, Harvest Moon]
        public static readonly SpellData BloodStalk = DataManager.GetSpellData(24389); // [24389, Blood Stalk]
        public static readonly SpellData UnveiledGibbet = DataManager.GetSpellData(24390); // [24390, UnveiledGibbet]
        public static readonly SpellData UnveiledGallows = DataManager.GetSpellData(24391); // [24391, Unveiled Gallows]
        public static readonly SpellData GrimSwathe = DataManager.GetSpellData(24392); // [24392, Grim Swathe]
        public static readonly SpellData Gluttony = DataManager.GetSpellData(24393); // [24393, Gluttony]
        public static readonly SpellData Enshroud = DataManager.GetSpellData(24394); // [24394, Enshroud]
        public static readonly SpellData VoidReaping = DataManager.GetSpellData(24395); // [24395, Void Reaping]
        public static readonly SpellData CrossReaping = DataManager.GetSpellData(24396); // [24396, Cross Reaping]
        public static readonly SpellData GrimReaping = DataManager.GetSpellData(24397); // [24397, Grim Reaping]
        public static readonly SpellData Communio = DataManager.GetSpellData(24398); // [24398, Communio]
        public static readonly SpellData LemuresSlice = DataManager.GetSpellData(24399); // [24399, Lemure's Slice]
        public static readonly SpellData LemuresScythe = DataManager.GetSpellData(24400); // [24400, Lemure's Scythe]
        public static readonly SpellData HellsIngress = DataManager.GetSpellData(24401); // [24401, Hell's Ingress]
        public static readonly SpellData HellsEgress = DataManager.GetSpellData(24402); // [24402, Hell's Egress]
        public static readonly SpellData Regress = DataManager.GetSpellData(24403); // [24403, Regress]
        public static readonly SpellData ArcaneCrest = DataManager.GetSpellData(24404); // [24404, Arcane Crest]
        public static readonly SpellData ArcaneCircle = DataManager.GetSpellData(24405); // [24405, Arcane Circle]
        public static readonly SpellData TheEnd = DataManager.GetSpellData(24858);
        #endregion

        //PVP
        #region PVP
        /* How to find Combo uint value ?
         * 
         * On rebornBuddy Console, execute : Log("GetPvPComboCurrentActionId = {0}", ActionManager.GetPvPComboCurrentActionId(54)); and replace 54 by other value (decreasing or increasing)
         * Answer will give you ID of 1st spell combo. For Dancer, we expect "29416", so we change ComboId Value till we find the right ComboID related to the job
         * 
         */

        public static readonly SpellData Recuperate = DataManager.GetSpellData(29711);
        public static readonly SpellData Purify = DataManager.GetSpellData(29056);
        public static readonly SpellData Guard = DataManager.GetSpellData(29054);

        //WHM
        public static readonly SpellData GlareIIIPvp = DataManager.GetSpellData(29223);
        public static readonly SpellData CureIIPvp = DataManager.GetSpellData(29224);
        public static readonly SpellData CureIIIPvp = DataManager.GetSpellData(29225);
        public static readonly SpellData AfflatusMiseryPvp = DataManager.GetSpellData(29226);
        public static readonly SpellData AquaveilPvp = DataManager.GetSpellData(29227);
        public static readonly SpellData MiracleOfNaturePvp = DataManager.GetSpellData(29228);
        public static readonly SpellData SeraphStrikePvp = DataManager.GetSpellData(29229);
        public static readonly SpellData AfflatusPurgationPvp = DataManager.GetSpellData(29230);

        //SCH
        public static readonly SpellData BroilIVPvp = DataManager.GetSpellData(29231);
        public static readonly SpellData AdloquiumPvp = DataManager.GetSpellData(29232);
        public static readonly SpellData BiolysisPvp = DataManager.GetSpellData(29233);
        public static readonly SpellData DeploymentTacticsPvp = DataManager.GetSpellData(29234);
        public static readonly SpellData MummificationPvp = DataManager.GetSpellData(29235);
        public static readonly SpellData ExpedientPvp = DataManager.GetSpellData(29236);
        public static readonly SpellData ConsolationPvp = DataManager.GetSpellData(29238);
        public static readonly SpellData SeraphicVeil = DataManager.GetSpellData(29240);
        public static readonly SpellData SummonSeraphPvp = DataManager.GetSpellData(29237);

        //AST
        public static readonly SpellData FallMaleficPvp = DataManager.GetSpellData(29242);
        public static readonly SpellData AspectedBeneficPvp = DataManager.GetSpellData(29243);
        public static readonly SpellData GravityIIPvp = DataManager.GetSpellData(29244);
        public static readonly SpellData DoubleCastPvp = DataManager.GetSpellData(29245);
        public static readonly SpellData DoubleFallMaleficPvp = DataManager.GetSpellData(29246);
        public static readonly SpellData DoubleAspectedBeneficPvp = DataManager.GetSpellData(29247);
        public static readonly SpellData DoubleGravityIIPvp = DataManager.GetSpellData(29248);
        public static readonly SpellData DrawPvp = DataManager.GetSpellData(29249);
        public static readonly SpellData BalancePvp = DataManager.GetSpellData(29250);
        public static readonly SpellData MacrocosmosPvp = DataManager.GetSpellData(29253);
        public static readonly SpellData MicrocosmosPvp = DataManager.GetSpellData(29254);
        public static readonly SpellData CelestialRiverPvp = DataManager.GetSpellData(29255);

        //SAGE
        public static readonly SpellData DosisIIIPvp = DataManager.GetSpellData(29256);
        public static readonly SpellData EukrasiaPvp = DataManager.GetSpellData(29258);
        public static readonly SpellData PhlegmaIIIPvp = DataManager.GetSpellData(29259);
        public static readonly SpellData PneumaPvp = DataManager.GetSpellData(29260);
        public static readonly SpellData IcarusPvp = DataManager.GetSpellData(29261);
        public static readonly SpellData ToxikonPvp = DataManager.GetSpellData(29262);
        public static readonly SpellData KardiaPvp = DataManager.GetSpellData(29264);
        public static readonly SpellData MesotesPvp = DataManager.GetSpellData(29266);

        //Bard
        public static readonly SpellData PowerfulShotPvp = DataManager.GetSpellData(29391);
        public static readonly SpellData PitchPerfectPvp = DataManager.GetSpellData(29392);
        public static readonly SpellData ApexArrowPvp = DataManager.GetSpellData(29393);
        public static readonly SpellData BlastArrowPvp = DataManager.GetSpellData(29394);
        public static readonly SpellData SilentNocturnePvp = DataManager.GetSpellData(29395);
        public static readonly SpellData EmpyrealArrowPvp = DataManager.GetSpellData(29396);
        public static readonly SpellData EmpyrealArrowIIPvp = DataManager.GetSpellData(29397);
        public static readonly SpellData EmpyrealArrowIIIPvp = DataManager.GetSpellData(29398);
        public static readonly SpellData TheWardenPaeanPvp = DataManager.GetSpellData(29400);
        public static readonly SpellData FinalFantasiaPvp = DataManager.GetSpellData(29401);

        //MCH
        public static readonly SpellData BlastChargePvp = DataManager.GetSpellData(29402);
        public static readonly SpellData HeatBlastPvp = DataManager.GetSpellData(29403);
        public static readonly SpellData ScattergunPvp = DataManager.GetSpellData(29404);
        public static readonly SpellData DrillPvp = DataManager.GetSpellData(29405);
        public static readonly SpellData BioblasterPvp = DataManager.GetSpellData(29406);
        public static readonly SpellData AirAnchorPvp = DataManager.GetSpellData(29407);
        public static readonly SpellData ChainSawPvp = DataManager.GetSpellData(29408);
        public static readonly SpellData WildfirePvp = DataManager.GetSpellData(29409);
        public static readonly SpellData BishopAutoturretPvp = DataManager.GetSpellData(29412);
        public static readonly SpellData AnalysisPvp = DataManager.GetSpellData(29414);
        public static readonly SpellData MarksmansSpitePvp = DataManager.GetSpellData(29415);

        //DNC
        public static readonly uint FountainPvpCombo = 54;
        public static readonly SpellData CascadePvp = DataManager.GetSpellData(29416);
        public static readonly SpellData CascadePvp2 = DataManager.GetSpellData(25464);
        public static readonly SpellData FountainPvp = DataManager.GetSpellData(29417);
        public static readonly SpellData ReverseCascadePvp = DataManager.GetSpellData(29418);
        public static readonly SpellData FountainFallPvp = DataManager.GetSpellData(29419);
        public static readonly SpellData SaberDancePvp = DataManager.GetSpellData(29420);
        public static readonly SpellData StarfallDancePvp = DataManager.GetSpellData(29421);
        public static readonly SpellData HoningDancePvp = DataManager.GetSpellData(29422);
        public static readonly SpellData FanDancePvp = DataManager.GetSpellData(29428);
        public static readonly SpellData CuringWaltzPvp = DataManager.GetSpellData(29429);
        public static readonly SpellData EnAvantPvp = DataManager.GetSpellData(29430);
        public static readonly SpellData ClosedPositionPvp = DataManager.GetSpellData(29431);
        public static readonly SpellData ContradancePvp = DataManager.GetSpellData(29432);

        //MNK
        public static readonly uint PhantomRushPvpCombo = 55;
        public static readonly SpellData BootshinePvp = DataManager.GetSpellData(29472);
        public static readonly SpellData TrueStrikePvp = DataManager.GetSpellData(29473);
        public static readonly SpellData SnapPunchPvp = DataManager.GetSpellData(29474);
        public static readonly SpellData DragonKickPvp = DataManager.GetSpellData(29475);
        public static readonly SpellData TwinSnakesPvp = DataManager.GetSpellData(29476);
        public static readonly SpellData DemolishPvp = DataManager.GetSpellData(29477);
        public static readonly SpellData PhantomRushPvp = DataManager.GetSpellData(29478);
        public static readonly SpellData SixSidedStarPvp = DataManager.GetSpellData(29479);
        public static readonly SpellData EnlightenmentPvp = DataManager.GetSpellData(29480);
        public static readonly SpellData RisingPhoenixPvp = DataManager.GetSpellData(29481);
        public static readonly SpellData RiddleofEarthPvp = DataManager.GetSpellData(29482);
        public static readonly SpellData EarthReplyPvp = DataManager.GetSpellData(29483);
        public static readonly SpellData ThunderclapPvp = DataManager.GetSpellData(29484);
        public static readonly SpellData MeteodrivePvp = DataManager.GetSpellData(29485);

        //DRG
        public static readonly uint WheelingThrustPvpCombo = 56;
        public static readonly SpellData RaidenThrustPvp = DataManager.GetSpellData(29486);
        public static readonly SpellData FangandClawPvp = DataManager.GetSpellData(29487);
        public static readonly SpellData WheelingThrustPvp = DataManager.GetSpellData(29488);
        public static readonly SpellData HeavensThrustPvp = DataManager.GetSpellData(29489);
        public static readonly SpellData ChaoticSpringPvp = DataManager.GetSpellData(29490);
        public static readonly SpellData GeirskogulPvp = DataManager.GetSpellData(29491);
        public static readonly SpellData NastrondPvp = DataManager.GetSpellData(29492);
        public static readonly SpellData HighJumpPvp = DataManager.GetSpellData(29493);
        public static readonly SpellData ElusiveJumpPvp = DataManager.GetSpellData(29494);
        public static readonly SpellData WyrmwindThrustPvp = DataManager.GetSpellData(29495);
        public static readonly SpellData HorridRoarPvp = DataManager.GetSpellData(29496);
        public static readonly SpellData SkyHighPvp = DataManager.GetSpellData(29497);
        public static readonly SpellData SkyShatterPvp = DataManager.GetSpellData(29498);
        public static readonly SpellData SkyShatter2Pvp = DataManager.GetSpellData(29499);

        //NIN
        public static readonly uint AeolianEdgePvpCombo = 57;
        public static readonly SpellData SpinningEdgePvp = DataManager.GetSpellData(29500);
        public static readonly SpellData GustSlashPvp = DataManager.GetSpellData(29501);
        public static readonly SpellData AeolianEdgePvp = DataManager.GetSpellData(29502);
        public static readonly SpellData AssassinatePvp = DataManager.GetSpellData(29503);
        public static readonly SpellData GokaMekkyakuPvp = DataManager.GetSpellData(29504);
        public static readonly SpellData FumaShurikenPvp = DataManager.GetSpellData(29505);
        public static readonly SpellData HyoshoRanryuPvp = DataManager.GetSpellData(29506);
        public static readonly SpellData ThreeMudraPvp = DataManager.GetSpellData(29507);
        public static readonly SpellData MeisuiPvp = DataManager.GetSpellData(29508);
        public static readonly SpellData MugPvp = DataManager.GetSpellData(29509);
        public static readonly SpellData ForkedRaijuPvp = DataManager.GetSpellData(29510);
        public static readonly SpellData BunshinPvp = DataManager.GetSpellData(29511);
        public static readonly SpellData HutonPvp = DataManager.GetSpellData(29512);
        public static readonly SpellData ShukuchiPvp = DataManager.GetSpellData(29513);
        public static readonly SpellData DotonPvp = DataManager.GetSpellData(29514);
        public static readonly SpellData SeitonTenchuPvp = DataManager.GetSpellData(29515);
        public static readonly SpellData UnsealedSeitonTenchuPvp = DataManager.GetSpellData(29516);
        public static readonly SpellData FleetingRaijuPvp = DataManager.GetSpellData(29517);

        //SAM
        public static readonly uint KashaPvpCombo = 58;
        public static readonly SpellData YukikazePvp = DataManager.GetSpellData(29523);
        public static readonly SpellData GekkoPvp = DataManager.GetSpellData(29524);
        public static readonly SpellData KashaPvp = DataManager.GetSpellData(29525);
        public static readonly SpellData HyosetsuPvp = DataManager.GetSpellData(29526);
        public static readonly SpellData MangetsuPvp = DataManager.GetSpellData(29527);
        public static readonly SpellData OkaPvp = DataManager.GetSpellData(29528);
        public static readonly SpellData MidareSetsugekkaPvp = DataManager.GetSpellData(29529);
        public static readonly SpellData OgiNamikiriPvp = DataManager.GetSpellData(29530);
        public static readonly SpellData KaeshiNamikiriPvp = DataManager.GetSpellData(29531);
        public static readonly SpellData HissatsuSotenPvp = DataManager.GetSpellData(29532);
        public static readonly SpellData HissatsuChitenPvp = DataManager.GetSpellData(29533);
        public static readonly SpellData MineuchiPvp = DataManager.GetSpellData(29535);
        public static readonly SpellData MeikyoShisuiPvp = DataManager.GetSpellData(29536);
        public static readonly SpellData ZantetsukenPvp = DataManager.GetSpellData(29537);

        //RPR
        public static readonly uint InfernalSlicePvpCombo = 59;
        public static readonly SpellData SlicePvp = DataManager.GetSpellData(29538);
        public static readonly SpellData WaxingSlicePvp = DataManager.GetSpellData(29539);
        public static readonly SpellData InfernalSlicePvp = DataManager.GetSpellData(29540);
        public static readonly SpellData VoidReapingPvp = DataManager.GetSpellData(29543);
        public static readonly SpellData CrossReapingePvp = DataManager.GetSpellData(29544);
        public static readonly SpellData HarvestMoonPvp = DataManager.GetSpellData(29545);
        public static readonly SpellData PlentifulHarvestPvp = DataManager.GetSpellData(29546);
        public static readonly SpellData GrimSwathePvp = DataManager.GetSpellData(29547);
        public static readonly SpellData LemureSlicePvp = DataManager.GetSpellData(29548);
        public static readonly SpellData DeathWarrantPvp = DataManager.GetSpellData(29549);
        public static readonly SpellData HellIngressPvp = DataManager.GetSpellData(29550);
        public static readonly SpellData IngressPvp = DataManager.GetSpellData(29551);
        public static readonly SpellData ArcaneCrestPvp = DataManager.GetSpellData(29552);
        public static readonly SpellData TenebraeLemurumPvp = DataManager.GetSpellData(29553);
        public static readonly SpellData CommunioPvp = DataManager.GetSpellData(29554);
        public static readonly SpellData SoulSlicePvp = DataManager.GetSpellData(29566);
        public static readonly SpellData GuillotinePvp = DataManager.GetSpellData(34786);

        //BLM
        public static readonly SpellData FirePvp = DataManager.GetSpellData(29649);
        public static readonly SpellData BlizzardPvp = DataManager.GetSpellData(29653);
        public static readonly SpellData BurstPvp = DataManager.GetSpellData(29657);
        public static readonly SpellData FoulPvp = DataManager.GetSpellData(29371);
        public static readonly SpellData ParadoxPvp = DataManager.GetSpellData(29663);
        public static readonly SpellData NightWingPvp = DataManager.GetSpellData(29659);
        public static readonly SpellData AetherialManipulationPvp = DataManager.GetSpellData(29660);
        public static readonly SpellData SuperFlarePvp = DataManager.GetSpellData(29661);
        public static readonly SpellData SoulResonancePvp = DataManager.GetSpellData(29662);

        //SMN
        public static readonly SpellData RuinIIIPvp = DataManager.GetSpellData(29664);
        public static readonly SpellData CrimsonClylonePvp = DataManager.GetSpellData(29667);
        public static readonly SpellData SlipstreamPvp = DataManager.GetSpellData(29669);
        public static readonly SpellData RadiantAegisPvp = DataManager.GetSpellData(29670);
        public static readonly SpellData MountainBusterPvp = DataManager.GetSpellData(29671);
        public static readonly SpellData FesterPvp = DataManager.GetSpellData(29672);
        public static readonly SpellData EnkindleBahamutPvp = DataManager.GetSpellData(29674);
        public static readonly SpellData EnkindlePhoenixPvp = DataManager.GetSpellData(29679);
        public static readonly SpellData CrimsonStrikePvp = DataManager.GetSpellData(29668);
        public static readonly SpellData SummonBahamutPvp = DataManager.GetSpellData(29673);
        public static readonly SpellData SummonPhoenixPvp = DataManager.GetSpellData(29678);

        //RDM
        public static readonly SpellData VerstonePvp = DataManager.GetSpellData(29683);
        public static readonly SpellData EnchantedRiposteWhitePvp = DataManager.GetSpellData(29689);
        public static readonly SpellData EnchantedZwerchhauWhitePvp = DataManager.GetSpellData(29690);
        public static readonly SpellData EnchantedRedoublementWhitePvp = DataManager.GetSpellData(29691);
        public static readonly SpellData EnchantedRiposteBlackPvp = DataManager.GetSpellData(29692);
        public static readonly SpellData EnchantedZwerchhauBlackPvp = DataManager.GetSpellData(29693);
        public static readonly SpellData EnchantedRedoublementBlackPvp = DataManager.GetSpellData(29694);
        public static readonly SpellData ResolutionBlackPvp = DataManager.GetSpellData(29695);
        public static readonly SpellData ResolutionWhitePvp = DataManager.GetSpellData(29696);
        public static readonly SpellData MagickBarrierPvp = DataManager.GetSpellData(29697);
        public static readonly SpellData FazzlePvp = DataManager.GetSpellData(29698);
        public static readonly SpellData CorpsacorpsPvp = DataManager.GetSpellData(29699);
        public static readonly SpellData DisplacementPvp = DataManager.GetSpellData(29700);
        public static readonly SpellData BlackShiftPvp = DataManager.GetSpellData(29702);
        public static readonly SpellData WhiteShiftPvp = DataManager.GetSpellData(29703);
        public static readonly SpellData VerHolyPvp = DataManager.GetSpellData(29685);
        public static readonly SpellData VerFlarePvp = DataManager.GetSpellData(29688);
        public static readonly SpellData SouthernCrossBlackPvp = DataManager.GetSpellData(29704);
        public static readonly SpellData SouthernCrossWhitePvp = DataManager.GetSpellData(29705);

        //GBN
        public static readonly uint SolidBarrelPvpCombo = 53;
        public static readonly SpellData KeenEdgePvp = DataManager.GetSpellData(29098);
        public static readonly SpellData BrutalShelPvp = DataManager.GetSpellData(29099);
        public static readonly SpellData SolidBarrelPvp = DataManager.GetSpellData(29100);
        public static readonly SpellData BurstStrikePvp = DataManager.GetSpellData(29101);
        public static readonly SpellData GnashingFangPvp = DataManager.GetSpellData(29102);
        public static readonly SpellData SavageClawPvp = DataManager.GetSpellData(29103);
        public static readonly SpellData WickedTalonPvp = DataManager.GetSpellData(29104);
        public static readonly SpellData DoubleDownPvp = DataManager.GetSpellData(29105);
        public static readonly SpellData ContinuationPvp = DataManager.GetSpellData(29106);
        public static readonly SpellData HypervelocityPvp = DataManager.GetSpellData(29107);
        public static readonly SpellData RoughDividePvp = DataManager.GetSpellData(29123);
        public static readonly SpellData DrawandJunctionPvp = DataManager.GetSpellData(29124);
        public static readonly SpellData JunctionedPvp = DataManager.GetSpellData(29125);
        public static readonly SpellData NebulaPvp = DataManager.GetSpellData(29126);
        public static readonly SpellData BlastingZonePvp = DataManager.GetSpellData(29128);
        public static readonly SpellData AuroraPvp = DataManager.GetSpellData(29129);
        public static readonly SpellData RelentlessRushPvp = DataManager.GetSpellData(29130);
        public static readonly SpellData TerminalTriggerPvp = DataManager.GetSpellData(29131);

        //DRK
        public static readonly uint SouleaterPvpCombo = 52;
        public static readonly SpellData HardSlashPvp = DataManager.GetSpellData(29085);
        public static readonly SpellData SyphonStrikePvp = DataManager.GetSpellData(29086);
        public static readonly SpellData SouleaterPvp = DataManager.GetSpellData(29087);
        public static readonly SpellData BloodspillerPvp = DataManager.GetSpellData(29088);
        public static readonly SpellData ShadowbringerPvp = DataManager.GetSpellData(29091);
        public static readonly SpellData PlungePvp = DataManager.GetSpellData(29092);
        public static readonly SpellData BlackestNightPvp = DataManager.GetSpellData(29093);
        public static readonly SpellData SaltedEarthPvp = DataManager.GetSpellData(29094);
        public static readonly SpellData EventidePvp = DataManager.GetSpellData(29097);
        public static readonly SpellData QuietusPvp = DataManager.GetSpellData(29737);

        //WAR
        public static readonly uint StormPathPvpCombo = 51;
        public static readonly SpellData HeavySwingPvp = DataManager.GetSpellData(29074);
        public static readonly SpellData MaimPvp = DataManager.GetSpellData(29075);
        public static readonly SpellData StormPathPvp = DataManager.GetSpellData(29076);
        public static readonly SpellData FellCleavePvp = DataManager.GetSpellData(29078);
        public static readonly SpellData OnslaughtPvp = DataManager.GetSpellData(29079);
        public static readonly SpellData OrogenyPvp = DataManager.GetSpellData(29080);
        public static readonly SpellData BlotaPvp = DataManager.GetSpellData(29081);
        public static readonly SpellData BloodwhettingPvp = DataManager.GetSpellData(29082);
        public static readonly SpellData PrimalScreamPvp = DataManager.GetSpellData(29083);
        public static readonly SpellData PrimalRendPvp = DataManager.GetSpellData(29084);
        public static readonly SpellData ChaoticCyclonePvp = DataManager.GetSpellData(29736);

        //PLD
        public static readonly uint RoyalAuthorityPvpCombo = 50;
        public static readonly SpellData FastBladePvp = DataManager.GetSpellData(29058);
        public static readonly SpellData RiotBladePvp = DataManager.GetSpellData(29059);
        public static readonly SpellData RoyalAuthorityPvp = DataManager.GetSpellData(29060);
        public static readonly SpellData AtonementPvp = DataManager.GetSpellData(29061);
        public static readonly SpellData ShieldBashPvp = DataManager.GetSpellData(29064);
        public static readonly SpellData IntervenePvp = DataManager.GetSpellData(29065);
        public static readonly SpellData GuardianPvp = DataManager.GetSpellData(29066);
        public static readonly SpellData HolySheltronPvp = DataManager.GetSpellData(29067);
        public static readonly SpellData PhalanxPvp = DataManager.GetSpellData(29069);
        public static readonly SpellData ConfiteorPvp = DataManager.GetSpellData(29070);
        public static readonly SpellData BladeofFaithPvp = DataManager.GetSpellData(29071);
        public static readonly SpellData BladeofTruthPvp = DataManager.GetSpellData(29072);
        public static readonly SpellData BladeofValorPvp = DataManager.GetSpellData(29073);

        #endregion

    }

}