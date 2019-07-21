using System;
using System.Collections.ObjectModel;
using System.Linq;
using Clio.Utilities.Collections;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models;
using Magitek.Models.Account;
using Magitek.Models.Astrologian;
using Magitek.Models.Bard;
using Magitek.Models.BlackMage;
using Magitek.Models.DarkKnight;
using Magitek.Models.Dragoon;
using Magitek.Models.Machinist;
using Magitek.Models.Monk;
using Magitek.Models.Ninja;
using Magitek.Models.Paladin;
using Magitek.Models.RedMage;
using Magitek.Models.Samurai;
using Magitek.Models.Scholar;
using Magitek.Models.Summoner;
using Magitek.Models.Warrior;
using Magitek.Models.WhiteMage;
using Magitek.Toggles;
using Magitek.Utilities;
using Magitek.Utilities.Managers;
using Magitek.ViewModels;
using Magitek.Views;
using TreeSharp;
using Application = System.Windows.Application;
using BaseSettings = Magitek.Models.Account.BaseSettings;
using Debug = Magitek.ViewModels.Debug;

namespace Magitek
{
    public class Magitek
    {
        public void Initialize()
        {
            Logger.WriteInfo("Initializing ...");

            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);
            var intPtr = patternFinder.Find("Search 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 8D 0D ? ? ? ? E8 ? ? ? ? EB AB Add 3 TraceRelative");
            var languageByte = Core.Memory.Read<byte>(intPtr);

            switch (languageByte)
            {
                case 1:
                    Globals.Language = GameVersion.English;
                    break;
                case 2:
                    Globals.Language = GameVersion.English;
                    break;
                case 3:
                    Globals.Language = GameVersion.English;
                    break;
                case 4:
                    Globals.Language = GameVersion.Chinese;
                    break;
                default:
                    Globals.Language = GameVersion.English;
                    break;
            }

            Logger.WriteInfo($"Current Language: {Globals.Language}");
            RotationManager.Reset();
            ViewModels.BaseSettings.Instance.RoutineSelectedInUi = RotationManager.CurrentRotation.ToString();
            DispelManager.Reset();
            InterruptsAndStunsManager.Reset();
            TankBusterManager.ResetHealers();
            TankBusterManager.ResetTanks();
            TreeRoot.OnStart += OnStart;
            TreeRoot.OnStop += OnStop;
            CurrentZone = WorldManager.ZoneId;
            CurrentJob = Core.Me.CurrentJob;

            Application.Current.Dispatcher.Invoke(delegate
            {
                _form = new SettingsWindow();
                _form.Closed += (sender, args) =>
                {
                    _form = null;
                };
            });

            TogglesManager.LoadTogglesForCurrentJob();
            Logger.WriteInfo("Initialized");
        }

        private void OnStart(BotBase bot)
        {
            Logic.OpenerLogic.InOpener = false;
            Logic.OpenerLogic.OpenerQueue.Clear();
            Logic.SpellQueueLogic.SpellQueue.Clear();

            // Apply the gambits we have
            GambitsViewModel.Instance.ApplyGambits();
            OpenersViewModel.Instance.ApplyOpeners();
        }

        private void OnStop(BotBase bot)
        {
            StopMainOverlay();
            TogglesViewModel.Instance.SaveToggles();
        }

        private ClassJobType CurrentJob { get; set; }
        private ushort CurrentZone { get; set; }

        public void Pulse()
        {
            #region Job switching because events aren't reliable apparently
            if (CurrentJob != Core.Me.CurrentJob)
            {
                // Set our current job
                CurrentJob = Core.Me.CurrentJob;
                Logger.WriteInfo("Job Changed");

                // Run the shit we need to
                Application.Current.Dispatcher.Invoke(delegate
                {
                    GambitsViewModel.Instance.ApplyGambits();
                    OpenersViewModel.Instance.ApplyOpeners();
                    TogglesManager.LoadTogglesForCurrentJob();
                });

                RotationManager.Reset();
                DispelManager.Reset();
                InterruptsAndStunsManager.Reset();
                TankBusterManager.ResetHealers();
                TankBusterManager.ResetTanks();
            }
            #endregion

            #region Zone switching because events aren't reliable apparently

            if (WorldManager.ZoneId != CurrentZone)
            {
                // Set the current zone
                CurrentZone = WorldManager.ZoneId;

                // Run the shit we need to
                GambitsViewModel.Instance.ApplyGambits();
                OpenersViewModel.Instance.ApplyOpeners();
            }

            #endregion

            Tracking.Update();
            Combat.AdjustCombatTime();
            Combat.AdjustDutyTime();

            Debug.Instance.InCombatTime = Combat.CombatTime.Elapsed.Seconds;
            Debug.Instance.OutOfCombatTime = Combat.OutOfCombatTime.Elapsed.Seconds;
            Debug.Instance.InCombatMovingTime = Combat.MovingInCombatTime.Elapsed.Seconds;
            Debug.Instance.NotMovingInCombatTime = Combat.NotMovingInCombatTime.Elapsed.Seconds;
            Debug.Instance.DutyTime = Combat.DutyTime.Elapsed.Seconds;
            Debug.Instance.DutyState = Duty.State();
            Debug.Instance.CastingGambit = Casting.CastingGambit;

            if (BaseSettings.Instance.DebugHealingLists)
            {
                Debug.Instance.CastableWithin10 = new ObservableCollection<GameObject>(Group.CastableAlliesWithin10);
                Debug.Instance.CastableWithin15 = new ObservableCollection<GameObject>(Group.CastableAlliesWithin15);
                Debug.Instance.CastableWithin30 = new ObservableCollection<GameObject>(Group.CastableAlliesWithin30);
            }

            if (Core.Me.InCombat)
            {
                Debug.Instance.InCombatTimeLeft = Combat.CombatTotalTimeLeft;
                Debug.Instance.Enmity = new AsyncObservableCollection<Enmity>(EnmityManager.EnmityList);
            }

            // Reset Zoom Limit based on ZoomHack Setting
            ZoomHack.Toggle();

            if (Core.Me.HasTarget)
            {
                if (BaseSettings.Instance.DebugEnemyInfo)
                {
                    Debug.Instance.IsBoss = XivDataHelper.BossDictionary.ContainsKey(Core.Me.CurrentTarget.NpcId) ? "True" : "False";
                    Debug.Instance.TargetCombatTimeLeft = Core.Me.CurrentTarget.CombatTimeLeft();
                }
            }
        }

        public void Shutdown()
        {
            TreeRoot.OnStart -= OnStart;
            TreeRoot.OnStop -= OnStop;

            #region Save Settings For All Routines
            ScholarSettings.Instance.Save();
            WhiteMageSettings.Instance.Save();
            AstrologianSettings.Instance.Save();
            PaladinSettings.Instance.Save();
            DarkKnightSettings.Instance.Save();
            WarriorSettings.Instance.Save();
            BardSettings.Instance.Save();
            MachinistSettings.Instance.Save();
            DragoonSettings.Instance.Save();
            MonkSettings.Instance.Save();
            NinjaSettings.Instance.Save();
            SamuraiSettings.Instance.Save();
            BlackMageSettings.Instance.Save();
            RedMageSettings.Instance.Save();
            SummonerSettings.Instance.Save();
            #endregion

            Dispelling.Instance.Save();
            InterruptsAndStuns.Instance.Save();
            TankBusters.Instance.Save();
            AuthenticationSettings.Instance.Save();
            TogglesViewModel.Instance.SaveToggles();

            var hotkeys = HotkeyManager.RegisteredHotkeys.Select(r => r.Name).Where(r => r.Contains("Magitek"));

            foreach (var hk in hotkeys)
            {
                HotkeyManager.Unregister(hk);
            }

            //Form?.Close();
        }

        public static void OnButtonPress()
        {
            if (Form.IsVisible)
                return;

            Form.Show();

            if (BaseSettings.Instance.UseOverlay)
                StartMainOverlay();
        }

        private static SettingsWindow _form;

        public static SettingsWindow Form
        {
            get
            {
                if (_form != null) return _form;
                _form = new SettingsWindow();
                _form.Closed += (sender, args) =>
                {
                    _form = null;
                };
                return _form;
            }
        }
        
        private static void StartMainOverlay()
        {
            if (!BaseSettings.Instance.UseOverlay)
                return;

            OverlayManager.StartMainOverlay();
        }

        private static void StopMainOverlay()
        {
            OverlayManager.StopMainOverlay();
        }

        #region Rotations
        public static Composite CombatBehavior { get; set; }
        public static Composite HealBehavior { get; set; }
        public static Composite PullBehavior { get; set; }
        public static Composite PreCombatBuffBehavior { get; set; }
        public static Composite PullBuffBehavior { get; set; }
        public static Composite CombatBuffBehavior { get; set; }
        public static Composite RestBehavior { get; set; }
        #endregion
    }
}
