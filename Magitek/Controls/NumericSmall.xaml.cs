using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Magitek.Controls
{
    public partial class NumericSmall : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericSmall), new PropertyMetadata(0));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(NumericSmall), new PropertyMetadata(100));
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(NumericSmall), new PropertyMetadata(0));
        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(int), typeof(NumericSmall), new PropertyMetadata(1));
        public static readonly DependencyProperty LargeIncrementProperty = DependencyProperty.Register("LargeIncrement", typeof(int), typeof(NumericSmall), new PropertyMetadata(5));
        public static readonly DependencyProperty FirstTextProperty = DependencyProperty.Register("FirstText", typeof(string), typeof(NumericSmall), new UIPropertyMetadata(null));
        public static readonly DependencyProperty SecondTextProperty = DependencyProperty.Register("SecondText", typeof(string), typeof(NumericSmall), new UIPropertyMetadata(null));

        public string FirstText
        {
            get
            {
                return (string)GetValue(FirstTextProperty);
            }
            set
            {
                SetValue(FirstTextProperty, value);
            }
        }

        public string SecondText
        {
            get
            {
                return (string)GetValue(SecondTextProperty);
            }
            set
            {
                SetValue(SecondTextProperty, value);
            }
        }

        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public int MaxValue
        {
            get
            {
                return (int)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public int MinValue
        {
            get
            {
                return (int)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public int Increment
        {
            get
            {
                return (int)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
            }
        }

        public int LargeIncrement
        {
            get
            {
                return (int)GetValue(LargeIncrementProperty);
            }
            set
            {
                SetValue(LargeIncrementProperty, value);
            }
        }

        private int _previousValue;
        private static readonly int DelayRate = SystemParameters.KeyboardDelay;
        private static readonly int RepeatSpeed = Math.Max(1, SystemParameters.KeyboardSpeed);
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private bool _isIncrementing;

        public NumericSmall()
        {
            InitializeComponent();

            NumericTextBox.PreviewTextInput += NumericTextBoxPreviewNumericTextInput;
            NumericTextBox.GotFocus += NumericTextBoxGotFocus;
            NumericTextBox.LostFocus += NumericTextBoxLostFocus;
            NumericTextBox.PreviewKeyDown += NumericTextBoxPreviewKeyDown;

            ButtonIncrement.PreviewMouseLeftButtonDown += buttonIncrement_PreviewMouseLeftButtonDown;
            ButtonIncrement.PreviewMouseLeftButtonUp += buttonIncrement_PreviewMouseLeftButtonUp;
            ButtonDecrement.PreviewMouseLeftButtonDown += buttonDecrement_PreviewMouseLeftButtonDown;
            ButtonDecrement.PreviewMouseLeftButtonUp += buttonDecrement_PreviewMouseLeftButtonUp;

            _timer.Tick += _timer_Tick;
        }

        #region Textbox

        private static void NumericTextBoxPreviewNumericTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true;
            }
        }

        private void NumericTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            _previousValue = Value;
        }

        private void NumericTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            int newValue;

            if (int.TryParse(NumericTextBox.Text, out newValue))
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

            NumericTextBox.Text = newValue.ToString();
        }

        private void NumericTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
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
            return text.All(char.IsDigit);
        }

        private void buttonIncrement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonIncrement.CaptureMouse();
            _timer.Interval = TimeSpan.FromMilliseconds(DelayRate * 250);
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
            _timer.Interval = TimeSpan.FromMilliseconds(DelayRate * 250);
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

            _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / RepeatSpeed);
        }
        #endregion
    }
}
