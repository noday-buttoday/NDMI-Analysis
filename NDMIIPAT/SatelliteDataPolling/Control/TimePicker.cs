using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SatelliteDataPolling.Control
{
    [TemplatePart(Name = TimePicker.ElementHourTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = TimePicker.ElementMinuteTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = TimePicker.ElementSecondTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = TimePicker.ElementIncrementButton, Type = typeof(Button))]
    [TemplatePart(Name = TimePicker.ElementDecrementButton, Type = typeof(Button))]
    public class TimePicker : System.Windows.Controls.Control
    {
        #region Constants

        private const string ElementHourTextBox = "PART_HourTextBox";
        private const string ElementMinuteTextBox = "PART_MinuteTextBox";
        private const string ElementSecondTextBox = "PART_SecondTextBox";
        private const string ElementIncrementButton = "PART_IncrementButton";
        private const string ElementDecrementButton = "PART_DecrementButton";

        #endregion Constants

        #region Data
        private static readonly TimeSpan MinValidTime = new TimeSpan(0, 0, 0);
        private static readonly TimeSpan MaxValidTime = new TimeSpan(23, 59, 59);

        private TextBox hourTextBox;
        private TextBox minuteTextBox;
        private TextBox secondTextBox;
        private Button incrementButton;
        private Button decrementButton;
        private TextBox selectedTextBox;

        #endregion Data

        #region Ctor

        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public TimePicker()
        {
            SelectedTime = DateTime.Now.TimeOfDay;
        }

        #endregion Ctor

        #region Public Properties

        #region ReadOnlyMode

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TimePicker));

        #endregion MinTime

        #region SelectedTime

        public TimeSpan SelectedTime
        {
            get { return (TimeSpan)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
            "SelectedTime",
            typeof(TimeSpan),
            typeof(TimePicker),
            new FrameworkPropertyMetadata(TimePicker.MinValidTime, new PropertyChangedCallback(TimePicker.OnSelectedTimeChanged), new CoerceValueCallback(TimePicker.CoerceSelectedTime)));

        #endregion SelectedTime

        #region MinTime

        public TimeSpan MinTime
        {
            get { return (TimeSpan)GetValue(MinTimeProperty); }
            set { SetValue(MinTimeProperty, value); }
        }

        public static readonly DependencyProperty MinTimeProperty =
            DependencyProperty.Register(
            "MinTime",
            typeof(TimeSpan),
            typeof(TimePicker),
            new FrameworkPropertyMetadata(TimePicker.MinValidTime, new PropertyChangedCallback(TimePicker.OnMinTimeChanged)),
            new ValidateValueCallback(TimePicker.IsValidTime));

        #endregion MinTime

        #region MaxTime

        public TimeSpan MaxTime
        {
            get { return (TimeSpan)GetValue(MaxTimeProperty); }
            set { SetValue(MaxTimeProperty, value); }
        }

        public static readonly DependencyProperty MaxTimeProperty =
            DependencyProperty.Register(
            "MaxTime",
            typeof(TimeSpan),
            typeof(TimePicker),
            new FrameworkPropertyMetadata(TimePicker.MaxValidTime, new PropertyChangedCallback(TimePicker.OnMaxTimeChanged), new CoerceValueCallback(TimePicker.CoerceMaxTime)),
            new ValidateValueCallback(TimePicker.IsValidTime));

        #endregion MaxTime

        #region SelectedTimeChangedEvent

        public event RoutedPropertyChangedEventHandler<TimeSpan> SelectedTimeChanged
        {
            add { base.AddHandler(SelectedTimeChangedEvent, value); }
            remove { base.RemoveHandler(SelectedTimeChangedEvent, value); }
        }

        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent(
            "SelectedTimeChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<TimeSpan>),
            typeof(TimePicker));

        #endregion SelectedTimeChangedEvent

        #endregion Public Properties

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            hourTextBox = GetTemplateChild(ElementHourTextBox) as TextBox;
            if (hourTextBox != null)
            {
                hourTextBox.IsReadOnly = IsReadOnly;
                hourTextBox.GotFocus += SelectTextBox;

                #region 키 이벤트 설정
                hourTextBox.PreviewKeyDown += delegate(object sender, KeyEventArgs e)
                {
                    //숫자, 백스페이스, 좌우 화살표만 입력되게
                    if (!(e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 || e.Key == Key.NumPad6
                        || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Delete))
                    {
                        e.Handled = true;
                    }
                };
                hourTextBox.PreviewTextInput += delegate(object sender, TextCompositionEventArgs e)
                {
                    int value = int.Parse(hourTextBox.Text + e.Text);
                    int minHours = MinValidTime.Hours;
                    int maxHours = MaxValidTime.Hours;

                    if (!(minHours <= value && maxHours >= value))
                    {
                        e.Handled = true;
                    }
                };
                #endregion
            }

            minuteTextBox = GetTemplateChild(ElementMinuteTextBox) as TextBox;
            if (minuteTextBox != null)
            {
                minuteTextBox.IsReadOnly = IsReadOnly;
                minuteTextBox.GotFocus += SelectTextBox;

                #region 키 이벤트 설정
                minuteTextBox.PreviewKeyDown += delegate(object sender, KeyEventArgs e)
                {
                    //숫자, 백스페이스, 좌우 화살표만 입력되게
                    if (!(e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 || e.Key == Key.NumPad6
                        || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Delete))
                    {
                        e.Handled = true;
                    }
                };
                minuteTextBox.PreviewTextInput += delegate(object sender, TextCompositionEventArgs e)
                {
                    int value = int.Parse(minuteTextBox.Text + e.Text);
                    int minHours = MinValidTime.Minutes;
                    int maxHours = MaxValidTime.Minutes;

                    if (!(minHours <= value && maxHours >= value))
                    {
                        e.Handled = true;
                    }
                };
                #endregion
            }

            secondTextBox = GetTemplateChild(ElementSecondTextBox) as TextBox;
            if (secondTextBox != null)
            {
                secondTextBox.IsReadOnly = IsReadOnly;
                secondTextBox.GotFocus += SelectTextBox;

                #region 키 이벤트 설정
                secondTextBox.PreviewKeyDown += delegate(object sender, KeyEventArgs e)
                {
                    //숫자, 백스페이스, 좌우 화살표만 입력되게
                    if (!(e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 || e.Key == Key.NumPad6
                        || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Delete))
                    {
                        e.Handled = true;
                    }
                };
                secondTextBox.PreviewTextInput += delegate(object sender, TextCompositionEventArgs e)
                {
                    int value = int.Parse(secondTextBox.Text + e.Text);
                    int minHours = MinValidTime.Seconds;
                    int maxHours = MaxValidTime.Seconds;

                    if (!(minHours <= value && maxHours >= value))
                    {
                        e.Handled = true;
                    }
                };
                #endregion
            }

            incrementButton = GetTemplateChild(ElementIncrementButton) as Button;
            if (incrementButton != null) 
            {
                incrementButton.Click += IncrementTime;
            }

            decrementButton = GetTemplateChild(ElementDecrementButton) as Button;
            if (decrementButton != null)
            {
                decrementButton.Click += DecrementTime;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static object CoerceSelectedTime(DependencyObject d, object value)
        {
            TimePicker timePicker = (TimePicker)d;
            TimeSpan minimum = timePicker.MinTime;
            if ((TimeSpan)value < minimum)
            {
                return minimum;
            }
            TimeSpan maximum = timePicker.MaxTime;
            if ((TimeSpan)value > maximum)
            {
                return maximum;
            }
            return value;
        }

        private static object CoerceMaxTime(DependencyObject d, object value)
        {
            TimePicker timePicker = (TimePicker)d;
            TimeSpan minimum = timePicker.MinTime;
            if ((TimeSpan)value < minimum)
            {
                return minimum;
            }
            return value;
        }

        private static bool IsValidTime(object value)
        {
            TimeSpan time = (TimeSpan)value;
            return MinValidTime <= time && time <= MaxValidTime;
        }

        protected virtual void OnSelectedTimeChanged(TimeSpan oldSelectedTime, TimeSpan newSelectedTime)
        {
            RoutedPropertyChangedEventArgs<TimeSpan> e = new RoutedPropertyChangedEventArgs<TimeSpan>(oldSelectedTime, newSelectedTime);
            e.RoutedEvent = SelectedTimeChangedEvent;
            base.RaiseEvent(e);
        }

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker element = (TimePicker)d;
            element.OnSelectedTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual void OnMinTimeChanged(TimeSpan oldMinTime, TimeSpan newMinTime)
        {
        }

        private static void OnMinTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker element = (TimePicker)d;
            element.CoerceValue(MaxTimeProperty);
            element.CoerceValue(SelectedTimeProperty);
            element.OnMinTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual void OnMaxTimeChanged(TimeSpan oldMaxTime, TimeSpan newMaxTime)
        {
        }

        private static void OnMaxTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimePicker element = (TimePicker)d;
            element.CoerceValue(SelectedTimeProperty);
            element.OnMaxTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        private void SelectTextBox(object sender, RoutedEventArgs e)
        {
            selectedTextBox = sender as TextBox;
        }

        private void IncrementTime(object sender, RoutedEventArgs e)
        {
            IncrementDecrementTime(1);
        }

        private void DecrementTime(object sender, RoutedEventArgs e)
        {
            IncrementDecrementTime(-1);
        }

        private void IncrementDecrementTime(int step)
        {
            if (selectedTextBox == null)
            {
                selectedTextBox = hourTextBox;
            }

            TimeSpan time;

            if (selectedTextBox == hourTextBox)
            {
                time = SelectedTime.Add(new TimeSpan(step, 0, 0));
            }
            else if (selectedTextBox == minuteTextBox)
            {
                time = SelectedTime.Add(new TimeSpan(0, step, 0));
            }
            else
            {
                time = SelectedTime.Add(new TimeSpan(0, 0, step));
            }

            SelectedTime = time;
        }

        #endregion
    }
}
