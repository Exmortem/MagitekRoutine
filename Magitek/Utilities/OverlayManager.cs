using Buddy.Overlay;
using Buddy.Overlay.Controls;
using ff14bot;
using Magitek.Models.Account;
using Magitek.Views.UserControls;
using System.Windows;

namespace Magitek.Utilities
{
    internal static class OverlayManager
    {
        private static MainOverlayUiComponent MainSettingsOverlay;
        private static CombatMessageUiComponent CombatMessageOverlay;

        public static void StartMainOverlay()
        {
            if (!Core.OverlayManager.IsActive)
                return;

            if (!BaseSettings.Instance.UseOverlay)
                return;

            if (MainSettingsOverlay == null)
            {
                MainSettingsOverlay = new MainOverlayUiComponent();
            }

            Core.OverlayManager.AddUIComponent(MainSettingsOverlay);
        }

        public static void StopMainOverlay()
        {
            if (!Core.OverlayManager.IsActive)
                return;

            if (MainSettingsOverlay != null)
            {
                Core.OverlayManager.RemoveUIComponent(MainSettingsOverlay);
            }
            MainSettingsOverlay = null;
        }

        public static void RestartMainOverlay()
        {
            StopMainOverlay();
            StartMainOverlay();
        }

        public static void StartCombatMessageOverlay()
        {
            if (!Core.OverlayManager.IsActive)
                return;

            if (!BaseSettings.Instance.UseCombatMessageOverlay)
                return;

            if (CombatMessageOverlay == null)
            {
                CombatMessageOverlay = new CombatMessageUiComponent(BaseSettings.Instance.CombatMessageOverlayAdjustable);
            }

            Core.OverlayManager.AddUIComponent(CombatMessageOverlay);
        }

        public static void StopCombatMessageOverlay()
        {
            if (!Core.OverlayManager.IsActive)
                return;

            if (CombatMessageOverlay != null)
            {
                Core.OverlayManager.RemoveUIComponent(CombatMessageOverlay);
            }
            CombatMessageOverlay = null;
        }

        public static void RestartCombatMessageOverlay()
        {
            StopCombatMessageOverlay();
            StartCombatMessageOverlay();
        }
    }

    internal class MainOverlayUiComponent : OverlayUIComponent
    {
        public MainOverlayUiComponent() : base(true) { }

        private OverlayControl _control;

        public override OverlayControl Control
        {
            get
            {
                if (_control != null)
                    return _control;

                var overlayUc = new MainSettingsOverlay();

                overlayUc.BtnOpenSettings.Click += (sender, args) =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        if (!Magitek.Form.IsVisible)
                            Magitek.Form.Show();

                        Magitek.Form.Activate();
                    });
                };

                _control = new OverlayControl()
                {
                    Name = "MagitekOverlay",
                    Content = overlayUc,
                    Width = overlayUc.Width + 5,
                    X = BaseSettings.Instance.OverlayPosX,
                    Y = BaseSettings.Instance.OverlayPosY,
                    AllowMoving = true
                };

                _control.MouseLeave += (sender, args) =>
                {
                    BaseSettings.Instance.OverlayPosX = _control.X;
                    BaseSettings.Instance.OverlayPosY = _control.Y;
                    BaseSettings.Instance.Save();
                };

                _control.MouseLeftButtonDown += (sender, args) =>
                {
                    _control.DragMove();
                };

                return _control;
            }
        }
    }

    internal class CombatMessageUiComponent : OverlayUIComponent
    {
        public CombatMessageUiComponent(bool isHitTestable) : base(true) { }

        private OverlayControl _control;

        public override OverlayControl Control
        {
            get
            {
                if (_control != null)
                    return _control;

                var overlayUc = new CombatMessageOverlay();

                double width = BaseSettings.Instance.CombatMessageOverlayWidth;
                double height = BaseSettings.Instance.CombatMessageOverlayHeight;
                double posX = BaseSettings.Instance.CombatMessageOverlayPosX;
                double posY = BaseSettings.Instance.CombatMessageOverlayPosY;
                if (width < 0 || height < 0 || posX < 0 || posY < 0)
                {
                    /* Invalid (or default) values - take a reasonable guess where it should go */
                    width = Core.OverlayManager.UnscaledOverlayWidth / 2;
                    height = Core.OverlayManager.UnscaledOverlayHeight / 20;
                    posX = Core.OverlayManager.UnscaledOverlayWidth / 8;
                    posY = Core.OverlayManager.UnscaledOverlayHeight / 8;
                }

                _control = new OverlayControl()
                {
                    Name = "MagitekCombatTextOverlay",
                    Content = overlayUc,
                    Width = width,
                    Height = height,
                    X = posX,
                    Y = posY,
                    AllowMoving = IsHitTestable,
                    AllowResizing = IsHitTestable
                };

                _control.MouseLeave += (sender, args) =>
                {
                    if (IsHitTestable)
                    {
                        BaseSettings.Instance.CombatMessageOverlayWidth = _control.Width;
                        BaseSettings.Instance.CombatMessageOverlayHeight = _control.Height;
                        BaseSettings.Instance.CombatMessageOverlayPosX = _control.X;
                        BaseSettings.Instance.CombatMessageOverlayPosY = _control.Y;
                        BaseSettings.Instance.Save();
                    }
                };

                _control.MouseLeftButtonDown += (sender, args) =>
                {
                    _control.DragMove();
                };

                return _control;
            }
        }
    }
}
