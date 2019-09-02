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
    public class NumericTextBox : TextBox
    {
        private string oldText;

        public bool isMinMax
        {
            get { return (bool)GetValue(isMinMaxDependencyProperty); }
            set { SetValue(isMinMaxDependencyProperty, value); }
        }

        public int minValue
        {
            get { return (int) GetValue(minValueDependencyProperty); }
            set { SetValue(minValueDependencyProperty, value); }
        }

        public int maxValue
        {
            get { return (int)GetValue(maxValueDependencyProperty); }
            set { SetValue(maxValueDependencyProperty, value); }
        }

        public static readonly DependencyProperty isMinMaxDependencyProperty
            = DependencyProperty.Register("isMinMax", typeof(bool), typeof(NumericTextBox));

        public static readonly DependencyProperty minValueDependencyProperty
            = DependencyProperty.Register("minValue", typeof(int), typeof(NumericTextBox));

        public static readonly DependencyProperty maxValueDependencyProperty
            = DependencyProperty.Register("maxValue", typeof(int), typeof(NumericTextBox));

        public NumericTextBox()
        {
            this.PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
            this.PreviewKeyDown += new KeyEventHandler(NumericTextBox_PreviewKeyDown);
            this.TextChanged += NumericTextBox_TextChanged;

            this.isMinMax = false;
            this.minValue = 0;
            this.maxValue = 0;
            this.oldText = string.Empty;
        }

        void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;;
            int checkVal;
            //눌려진 값의 숫자 여부를 판단한다.
            if (isMinMax)
            {
                if (int.TryParse(tb.Text, out checkVal))
                {
                    if (checkVal >= minValue && checkVal <= maxValue)
                    {
                        oldText = tb.Text;
                    }
                    else
                    {
                        tb.Text = oldText;
                    }
                }
            }
        }

        void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //한글이나 공백등의 키를 걸러낸다
            if (e.Key == Key.Space || e.Key == Key.ImeProcessed)
            {
                e.Handled = true;
            }
        }

        void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int checkVal;
            //눌려진 값의 숫자 여부를 판단한다.
            if (!int.TryParse(e.Text, out checkVal))
            {
                if(!(e.Text == "-"))
                    e.Handled = true;
            }
        }
    }
}
