//using Clio.Utilities.Collections;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using Magitek.Extensions;
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
using Magitek.Models.Reaper;
using Magitek.Models.RedMage;
using Magitek.Models.Sage;
using Magitek.Models.Samurai;
using Magitek.Models.Scholar;
using Magitek.Models.Summoner;
using Magitek.Models.Warrior;
using Magitek.Models.WhiteMage;
using Magitek.Toggles;
using Magitek.Utilities;
using Magitek.Utilities.CombatMessages;
using Magitek.Utilities.GamelogManager;
using Magitek.Utilities.Managers;
using Magitek.Utilities.Overlays;
using Magitek.ViewModels;
using Magitek.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TreeSharp;
using Application = System.Windows.Application;
using BaseSettings = Magitek.Models.Account.BaseSettings;
using Debug = Magitek.ViewModels.Debug;
using Regexp = System.Text.RegularExpressions;

namespace Magitek
{
    public class Magitek : CombatRoutine
    {
        private static SettingsWindow _form;
        private DateTime _pulseLimiter, _saveFormTime;
        private ClassJobType CurrentJob { get; set; }
        private ushort CurrentZone { get; set; }

        public override void Initialize()
        {
            Logger.WriteInfo("Initializing ...");
            ViewModels.BaseSettings.Instance.RoutineSelectedInUi = RotationManager.CurrentRotation.ToString();
            ViewModels.BaseSettings.Instance.SettingsFirstInitialization = true;
            DispelManager.Reset();
            InterruptsAndStunsManager.Reset();
            TreeRoot.OnStart += OnStart;
            TreeRoot.OnStop += OnStop;
            CurrentZone = WorldManager.ZoneId;
            CurrentJob = Core.Me.CurrentJob;
            GameEvents.OnClassChanged += GameEventsOnOnClassChanged;
            GameEvents.OnLevelUp += GameEventsOnOnLevelUp;

            HookBehaviors();

            Application.Current.Dispatcher.Invoke(delegate
            {
                _form = new SettingsWindow();
                _form.Closed += (_, _) =>
                {
                    _form = null;
                };
            });

            TogglesManager.LoadTogglesForCurrentJob();
            CombatMessageManager.RegisterMessageStrategiesForClass(Core.Me.CurrentJob);
            Logger.WriteInfo("Initialized");
        }

        private void GameEventsOnOnLevelUp(object sender, EventArgs e)
        {
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
        }

        private void GameEventsOnOnClassChanged(object sender, EventArgs e)
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
            }
            #endregion
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

            GamelogManager.MessageRecevied += GamelogManagerCountdownRecevied;
        }

        public void OnStop(BotBase bot)
        {
            OverlayManager.StopMainOverlay();
            OverlayManager.StopCombatMessageOverlay();
            TogglesViewModel.Instance.SaveToggles();
            GamelogManagerCountdown.StopCooldown();
        }


        public override string Name { get; }
        public override float PullRange { get; } = 25;

        public override ClassJobType[] Class
        {
            get
            {
                switch (Core.Me.CurrentJob)
                {
                    case ClassJobType.Arcanist:
                    case ClassJobType.Scholar:
                    case ClassJobType.Summoner:
                    case ClassJobType.Archer:
                    case ClassJobType.Bard:
                    case ClassJobType.Thaumaturge:
                    case ClassJobType.BlackMage:
                    case ClassJobType.Conjurer:
                    case ClassJobType.WhiteMage:
                    case ClassJobType.Lancer:
                    case ClassJobType.Dragoon:
                    case ClassJobType.Gladiator:
                    case ClassJobType.Paladin:
                    case ClassJobType.Pugilist:
                    case ClassJobType.Monk:
                    case ClassJobType.Marauder:
                    case ClassJobType.Warrior:
                    case ClassJobType.Rogue:
                    case ClassJobType.Ninja:
                    case ClassJobType.Astrologian:
                    case ClassJobType.Machinist:
                    case ClassJobType.DarkKnight:
                    case ClassJobType.RedMage:
                    case ClassJobType.Samurai:
                    case ClassJobType.Dancer:
                    case ClassJobType.Gunbreaker:
                    case ClassJobType.BlueMage:
                    case ClassJobType.Reaper:
                    case ClassJobType.Sage:
                        return new[] { Core.Me.CurrentJob };
                    default:
                        return new[] { ClassJobType.Adventurer };
                }
            }
        }

        public override void Pulse()
        {
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

            var time = DateTime.Now;
            if (time < _pulseLimiter) return;
            _pulseLimiter = time.AddSeconds(1);

            if (time > _saveFormTime)
            {
                Dispelling.Instance.Save();
                InterruptsAndStuns.Instance.Save();
                BaseSettings.Instance.Save();
                TogglesViewModel.Instance.SaveToggles();
                _saveFormTime = time.AddSeconds(60);
            }

            CombatMessageManager.UpdateDisplayedMessage();
        }

        public override void ShutDown()
        {
            TreeRoot.OnStart -= OnStart;
            TreeRoot.OnStop -= OnStop;

            Dispelling.Instance.Save();
            BaseSettings.Instance.Save();
            InterruptsAndStuns.Instance.Save();
            TogglesViewModel.Instance.SaveToggles();

            var hotkeys = HotkeyManager.RegisteredHotkeys.Select(r => r.Name).Where(r => r.Contains("Magitek"));

            foreach (var hk in hotkeys)
            {
                HotkeyManager.Unregister(hk);
            }
        }

        private void GamelogManagerCountdownRecevied(object sender, ChatEventArgs e)
        {
            if ((int)e.ChatLogEntry.MessageType == 313 || (int)e.ChatLogEntry.MessageType == 185 || MessageType.SystemMessages.Equals(e.ChatLogEntry.MessageType))
            {
                //Start countdown
                var StartCountdownRegex = new Regexp.Regex(@"(Battle commencing in|Début du combat dans|Noch) ([\d]+) (seconds|secondes|Sekunden bis Kampfbeginn!)!(.*)", Regexp.RegexOptions.Compiled);
                var matchStart = StartCountdownRegex.Match(e.ChatLogEntry.FullLine);
                if (matchStart.Success)
                {
                    var groups = matchStart.Groups;
                    var time = groups[2].ToString() != "" ? int.Parse(groups[2].ToString()) : -1;
                    Logger.WriteInfo($@"Fight starting in {time} seconds");
                    GamelogManagerCountdown.RegisterAndStartCountdown(time);
                }

                //Abort countdown
                var AbortCountdownRegex = new Regexp.Regex(@"(.*)(Countdown canceled by|Le compte à rebours a été interrompu|hat den Countdown abgebrochen)(.*)", Regexp.RegexOptions.Compiled);
                var matchAbort = AbortCountdownRegex.Match(e.ChatLogEntry.FullLine);
                if (matchAbort.Success)
                {
                    Logger.WriteInfo($@"Countdown aborted!");
                    GamelogManagerCountdown.StopCooldown();
                }
            }
        }

        public override void OnButtonPress()
        {
            if (Form.IsVisible)
                return;

            Form.Show();

            OverlayManager.StartMainOverlay();
        }

        public static SettingsWindow Form
        {
            get
            {
                if (_form != null) return _form;
                _form = new SettingsWindow();
                _form.Closed += (_, _) =>
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

        public override Composite RestBehavior =>
            new Decorator(new PrioritySelector(new Decorator(_ => WorldManager.InPvP, new ActionRunCoroutine(_ => RotationManager.Rotation.PvP())),
                new ActionRunCoroutine(_ => RotationManager.Rotation.Rest())));

        public override Composite PreCombatBuffBehavior =>
            new Decorator(new PrioritySelector(new Decorator(_ => WorldManager.InPvP, new ActionRunCoroutine(_ => RotationManager.Rotation.PvP())),
                new ActionRunCoroutine(_ => RotationManager.Rotation.PreCombatBuff())));

        public override Composite PullBehavior =>
            new Decorator(new PrioritySelector(new Decorator(_ => WorldManager.InPvP, new ActionRunCoroutine(_ => RotationManager.Rotation.PvP())),
                new ActionRunCoroutine(_ => RotationManager.Rotation.Pull())));

        public override Composite HealBehavior =>
            new Decorator(new PrioritySelector(new Decorator(_ => WorldManager.InPvP, new ActionRunCoroutine(_ => RotationManager.Rotation.PvP())),
                new ActionRunCoroutine(_ => RotationManager.Rotation.Heal())));

        public override Composite CombatBuffBehavior =>
            new Decorator(new PrioritySelector(new Decorator(_ => WorldManager.InPvP, new ActionRunCoroutine(_ => RotationManager.Rotation.PvP())),
                new ActionRunCoroutine(_ => RotationManager.Rotation.CombatBuff())));

        public override Composite CombatBehavior =>
            new Decorator(new PrioritySelector(new Decorator(_ => WorldManager.InPvP, new ActionRunCoroutine(_ => RotationManager.Rotation.PvP())),
                new ActionRunCoroutine(_ => RotationManager.Rotation.Combat())));

        #endregion Behavior Composites
    }
}
