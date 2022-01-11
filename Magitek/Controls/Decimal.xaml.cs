using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Magitek.Controls
{
    public partial class Decimal : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(Decimal), new PropertyMetadata(0f));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(float), typeof(Decimal), new PropertyMetadata(100f));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(float), typeof(Decimal), new PropertyMetadata(0f));
        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(float), typeof(Decimal), new PropertyMetadata(.05f));
        public static readonly DependencyProperty LargeIncrementProperty = DependencyProperty.Register("LargeIncrement", typeof(float), typeof(Decimal), new PropertyMetadata(.1f));
        public static readonly DependencyProperty TextBlockValueProperty = DependencyProperty.Register("TextBlockValue", typeof(string), typeof(Decimal), new UIPropertyMetadata(null));

        public float Value
        {
            get
            {
                return (float)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public string TextBlockValue
        {
            get { return (string)GetValue(TextBlockValueProperty); }
            set { SetValue(TextBlockValueProperty, value); }
        }


        public float MaxValue
        {
            get
            {
                return (float)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public float MinValue
        {
            get
            {
                return (float)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public float Increment
        {
            get
            {
                return (float)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
            }
        }

        public float LargeIncrement
        {
            get
            {
                return (float)GetValue(LargeIncrementProperty);
            }
            set
            {
                SetValue(LargeIncrementProperty, value);
            }
        }

        private float _previousValue;
        private static readonly int DelayRate = SystemParameters.KeyboardDelay;
        private static readonly int RepeatSpeed = Math.Max(1, SystemParameters.KeyboardSpeed);
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private bool _isIncrementing;

        public Decimal()
        {
            InitializeComponent();

            TextBox.PreviewTextInput += TextBoxPreviewTextInput;
            TextBox.GotFocus += TextBoxGotFocus;
            TextBox.LostFocus += TextBoxLostFocus;
            TextBox.PreviewKeyDown += TextBoxPreviewKeyDown;

            ButtonIncrement.PreviewMouseLeftButtonDown += buttonIncrement_PreviewMouseLeftButtonDown;
            ButtonIncrement.PreviewMouseLeftButtonUp += buttonIncrement_PreviewMouseLeftButtonUp;
            ButtonDecrement.PreviewMouseLeftButtonDown += buttonDecrement_PreviewMouseLeftButtonDown;
            ButtonDecrement.PreviewMouseLeftButtonUp += buttonDecrement_PreviewMouseLeftButtonUp;

            _timer.Tick += _timer_Tick;
        }

        #region Textbox

        private static void TextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true;
            }
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            _previousValue = Value;
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            float newValue;
            if (float.TryParse(TextBox.Text, out newValue))
            {
                if (newValue > MaxValue)
                {
                    newValue = MaxValue;
                }
                else if (newValue < MinValue)
                {
                    newValue = MinValue;
                }
            }
            else
            {
                newValue = _previousValue;
            }
            TextBox.Text = newValue.ToString("0.00");
        }

        private void TextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    IncrementValue();
                    break;
                case Key.Down:
                    DecrementValue();
                    break;
                case Key.PageUp:
                    Value = Math.Min(Value + LargeIncrement, MaxValue);
                    break;
                case Key.PageDown:
                    Value = Math.Max(Value - LargeIncrement, MinValue);
                    break;
            }
        }
        #endregion

        #region Increment/Decrement
        private void IncrementValue()
        {
            Value = Math.Min(Value + Increment, MaxValue);
        }

        private void DecrementValue()
        {
            Value = Math.Max(Value - Increment, MinValue);
        }

        private static bool IsNumericInput(string text)
        {
            return text.All(c => char.IsDigit(c) || c == '.');
        }

        private void buttonIncrement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonIncrement.CaptureMouse();
            _timer.Interval =
                TimeSpan.FromMilliseconds(DelayRate * 250);
            _timer.Start();

            _isIncrementing = true;
        }

        private void buttonIncrement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            ButtonIncrement.ReleaseMouseCapture();
            IncrementValue();
        }

        private void buttonDecrement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonDecrement.CaptureMouse();
            _timer.Interval =
                TimeSpan.FromMilliseconds(DelayRate * 250);
            _timer.Start();

            _isIncrementing = false;
        }

        private void buttonDecrement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            ButtonDecrement.ReleaseMouseCapture();
            DecrementValue();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_isIncrementing)
            {
                IncrementValue();
            }
            else
            {
                DecrementValue();
            }
            _timer.Interval =
                TimeSpan.FromMilliseconds(1000.0 / RepeatSpeed);
        }
        #endregion
    }
}
