//using Clio.Utilities.Collections;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
//using Magitek.Models;
//using Magitek.Models.Account;
using Magitek.Models.Astrologian;
using Magitek.Models.Bard;
using Magitek.Models.BlackMage;
using Magitek.Models.BlueMage;
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
using Magitek.Utilities.CombatMessages;
using Magitek.Utilities.Managers;
using Magitek.Utilities.Overlays;
using Magitek.ViewModels;
using Magitek.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Magitek.Models.Reaper;
using TreeSharp;
using Application = System.Windows.Application;
using BaseSettings = Magitek.Models.Account.BaseSettings;
using Debug = Magitek.ViewModels.Debug;

namespace Magitek
{
    public class Magitek
    {
        private DateTime _pulseLimiter, _saveFormTime;
        public void Initialize()
        {
            Logger.WriteInfo("Initializing ...");
            ViewModels.BaseSettings.Instance.RoutineSelectedInUi = RotationManager.CurrentRotation.ToString();
            DispelManager.Reset();
            InterruptsAndStunsManager.Reset();
            //TankBusterManager.ResetHealers();
            //TankBusterManager.ResetTanks();
            TreeRoot.OnStart += OnStart;
            TreeRoot.OnStop += OnStop;
            CurrentZone = WorldManager.ZoneId;
            CurrentJob = Core.Me.CurrentJob;

            HookBehaviors();

            Application.Current.Dispatcher.Invoke(delegate
            {
                _form = new SettingsWindow();
                _form.Closed += (sender, args) =>
                {
                    _form = null;
                };
            });

            TogglesManager.LoadTogglesForCurrentJob();
            CombatMessageManager.RegisterMessageStrategiesForClass(Core.Me.CurrentJob);
            Logger.WriteInfo("Initialized");
        }

        public void OnStart(BotBase bot)
        {
            // Reset Zoom Limit based on ZoomHack Setting
            ZoomHack.Toggle();

            Logic.OpenerLogic.InOpener = false;
            Logic.OpenerLogic.OpenerQueue.Clear();
            Logic.SpellQueueLogic.SpellQueue.Clear();

            // Apply the gambits we have
            GambitsViewModel.Instance.ApplyGambits();
            OpenersViewModel.Instance.ApplyOpeners();
            OverlayManager.StartMainOverlay();
            OverlayManager.StartCombatMessageOverlay();
            CombatMessageManager.RegisterMessageStrategiesForClass(Core.Me.CurrentJob);
            HookBehaviors();
        }

        public void OnStop(BotBase bot)
        {
            OverlayManager.StopMainOverlay();
            OverlayManager.StopCombatMessageOverlay();
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

                HookBehaviors();
                DispelManager.Reset();
                InterruptsAndStunsManager.Reset();
                CombatMessageManager.RegisterMessageStrategiesForClass(Core.Me.CurrentJob);
                //TankBusterManager.ResetHealers();
                //TankBusterManager.ResetTanks();
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

            Debug.Instance.InCombatTime = (long)Combat.CombatTime.Elapsed.TotalSeconds;
            Debug.Instance.OutOfCombatTime = (int)Combat.OutOfCombatTime.Elapsed.TotalSeconds;
            Debug.Instance.InCombatMovingTime = (int)Combat.MovingInCombatTime.Elapsed.TotalSeconds;
            Debug.Instance.NotMovingInCombatTime = (int)Combat.NotMovingInCombatTime.Elapsed.TotalSeconds;
            Debug.Instance.DutyTime = (long)Combat.DutyTime.Elapsed.TotalSeconds;
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
                //Debug.Instance.Enmity = new AsyncObservableCollection<Enmity>(EnmityManager.EnmityList);
            }

            if (Core.Me.HasTarget)
            {
                if (BaseSettings.Instance.DebugEnemyInfo)
                {
                    Debug.Instance.IsBoss = XivDataHelper.BossDictionary.ContainsKey(Core.Me.CurrentTarget.NpcId) ? "True" : "False";
                    Debug.Instance.TargetCombatTimeLeft = Core.Me.CurrentTarget.CombatTimeLeft();
                }
            }

            if (DateTime.Now < _pulseLimiter) return;
            _pulseLimiter = DateTime.Now.AddSeconds(1);

            if (DateTime.Now > _saveFormTime)
            {

                Dispelling.Instance.Save();
                InterruptsAndStuns.Instance.Save();
                //TankBusters.Instance.Save();
                TogglesViewModel.Instance.SaveToggles();

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
                ReaperSettings.Instance.Save();
                BlueMageSettings.Instance.Save();
                BlackMageSettings.Instance.Save();
                RedMageSettings.Instance.Save();
                SummonerSettings.Instance.Save();
                #endregion

                _saveFormTime = DateTime.Now.AddSeconds(60);
            }

            CombatMessageManager.UpdateDisplayedMessage();
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
            BlueMageSettings.Instance.Save();
            BlackMageSettings.Instance.Save();
            RedMageSettings.Instance.Save();
            SummonerSettings.Instance.Save();
            #endregion

            Dispelling.Instance.Save();
            InterruptsAndStuns.Instance.Save();
            //TankBusters.Instance.Save();
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

            OverlayManager.StartMainOverlay();
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

        #region Behavior Composites

        public void HookBehaviors()
        {
            Logger.Write("Hooking behaviors");
            TreeHooks.Instance.ReplaceHook("Rest", RestBehavior);
            TreeHooks.Instance.ReplaceHook("PreCombatBuff", PreCombatBuffBehavior);
            TreeHooks.Instance.ReplaceHook("Pull", PullBehavior);
            TreeHooks.Instance.ReplaceHook("Heal", HealBehavior);
            TreeHooks.Instance.ReplaceHook("CombatBuff", CombatBuffBehavior);
            TreeHooks.Instance.ReplaceHook("Combat", CombatBehavior);
        }

        public Composite RestBehavior
        {
            get
            {
                return new Decorator(new PrioritySelector(new Decorator(r => WorldManager.InPvP, new ActionRunCoroutine(ctx => RotationManager.Rotation.PvP())),
                    new ActionRunCoroutine(ctx => RotationManager.Rotation.Rest())));
            }
        }

        public Composite PreCombatBuffBehavior
        {
            get
            {
                return new Decorator(new PrioritySelector(new Decorator(r => WorldManager.InPvP, new ActionRunCoroutine(ctx => RotationManager.Rotation.PvP())),
                    new ActionRunCoroutine(ctx => RotationManager.Rotation.PreCombatBuff())));
            }
        }

        public Composite PullBehavior
        {
            get
            {
                return new Decorator(new PrioritySelector(new Decorator(r => WorldManager.InPvP, new ActionRunCoroutine(ctx => RotationManager.Rotation.PvP())),
                    new ActionRunCoroutine(ctx => RotationManager.Rotation.Pull())));
            }
        }

        public Composite HealBehavior
        {
            get
            {
                return new Decorator(new PrioritySelector(new Decorator(r => WorldManager.InPvP, new ActionRunCoroutine(ctx => RotationManager.Rotation.PvP())),
                    new ActionRunCoroutine(ctx => RotationManager.Rotation.Heal())));
            }
        }

        public Composite CombatBuffBehavior
        {
            get
            {
                return new Decorator(new PrioritySelector(new Decorator(r => WorldManager.InPvP, new ActionRunCoroutine(ctx => RotationManager.Rotation.PvP())),
                    new ActionRunCoroutine(ctx => RotationManager.Rotation.CombatBuff())));
            }
        }

        public Composite CombatBehavior
        {
            get
            {
                return new Decorator(new PrioritySelector(new Decorator(r => WorldManager.InPvP, new ActionRunCoroutine(ctx => RotationManager.Rotation.PvP())),
                        new ActionRunCoroutine(ctx => RotationManager.Rotation.Combat())));
            }
        }

        #endregion Behavior Composites
    }
}
