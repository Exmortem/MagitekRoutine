using System.Windows;
using Buddy.Overlay;
using Buddy.Overlay.Controls;
using ff14bot;
using Magitek.Models.Account;
using Magitek.Views.UserControls;

namespace Magitek.Utilities
{
    internal static class OverlayManager
    {
        private static readonly MainOverlayUiComponent MainSettingsOverlay = new MainOverlayUiComponent();

        public static void StartMainOverlay()
        {
            if (!Core.OverlayManager.IsActive)
                return;

            Core.OverlayManager.AddUIComponent(MainSettingsOverlay);
        }

        public static void StopMainOverlay()
        {
            if (!Core.OverlayManager.IsActive)
                return;

            Core.OverlayManager.RemoveUIComponent(MainSettingsOverlay);
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
                    Application.Current.Dispatcher.Invoke(Magitek.OnButtonPress);
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
}
