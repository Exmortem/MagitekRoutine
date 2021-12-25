using Buddy.Overlay;
using Buddy.Overlay.Controls;
using Magitek.Models.Account;
using Magitek.Views.UserControls;
using System.Windows;

namespace Magitek.Utilities.Overlays
{
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
}
