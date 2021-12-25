using Buddy.Overlay;
using Buddy.Overlay.Controls;
using ff14bot;
using Magitek.Models.Account;
using Magitek.Views.UserControls;

namespace Magitek.Utilities.Overlays
{
    internal class CombatMessageUiComponent : OverlayUIComponent
    {
        public CombatMessageUiComponent(bool isHitTestable) : base(isHitTestable) { }

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
                    /* Default (or invalid) values - take a reasonable guess where it should go */
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
