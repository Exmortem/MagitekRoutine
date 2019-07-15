using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace Magitek.Controls
{
    /// <summary>
    /// Interaction logic for HotkeyToggles.xaml
    /// </summary>
    public partial class HotkeyToggles : UserControl
    {
        public static readonly DependencyProperty ToggleHotkeyTextProperty = DependencyProperty.Register("ToggleHotkeyText", typeof(string), typeof(HotkeyToggles), new UIPropertyMetadata("TOGGLE HOTKEY:"));
        public static readonly DependencyProperty ToggleKeySettingProperty = DependencyProperty.Register("ToggleKeySetting", typeof(Keys), typeof(HotkeyToggles), new PropertyMetadata(Keys.None));
        public static readonly DependencyProperty ToggleModKeySettingProperty = DependencyProperty.Register("ToggleModKeySetting", typeof(ModifierKeys), typeof(HotkeyToggles), new PropertyMetadata(ModifierKeys.None));


        public HotkeyToggles()
        {
            InitializeComponent();
            PreviewKeyDown += OnPreviewKeyDown;
            LostFocus += OnLostFocus;
        }

        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            // Re - Registering Code
        }

        public string ToggleHotkeyText
        {
            get => (string)GetValue(ToggleHotkeyTextProperty);
            set => SetValue(ToggleHotkeyTextProperty, value);
        }

        public string HkText => ToggleModKeySetting + " + " + ToggleKeySetting;

        public Keys ToggleKeySetting
        {
            get => (Keys)GetValue(ToggleKeySettingProperty);
            set => SetValue(ToggleKeySettingProperty, value);
        }

        public ModifierKeys ToggleModKeySetting
        {
            get => (ModifierKeys)GetValue(ToggleModKeySettingProperty);
            set => SetValue(ToggleModKeySettingProperty, value);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            var key = (e.Key == Key.System ? e.SystemKey : e.Key);

            switch (key)
            {
                case Key.Escape:
                    TxtHk.Text = "None + None";
                    ToggleModKeySetting = ModifierKeys.None;
                    ToggleKeySetting = Keys.None;
                    return;
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LWin:
                case Key.RWin:
                    return;
            }

            // Ignore modifier keys.

            // Build the shortcut key name.
            var shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl + ");
                ToggleModKeySetting = ModifierKeys.Control;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift + ");
                ToggleModKeySetting = ModifierKeys.Shift;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt + ");
                ToggleModKeySetting = ModifierKeys.Alt;
            }

            if (Keyboard.Modifiers == 0)
            {
                shortcutText.Append("None + ");
                ToggleModKeySetting = ModifierKeys.None;
            }

            shortcutText.Append(key);

            var newKey = (Keys)KeyInterop.VirtualKeyFromKey(key);
            ToggleKeySetting = newKey;
            // Update the text box.
            TxtHk.Text = shortcutText.ToString();
        }
    }
}
