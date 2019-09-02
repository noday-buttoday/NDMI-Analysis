using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace ShapeFileControl.UiComponent
{
    public class WaterMarkTextBox : TextBox
    {
        private FontStyle origin_style_;
        private Brush origin_font_color_;
        private String water_mark_text_;
        protected String last_text_ = "";

        public WaterMarkTextBox()
        {
            origin_style_ = base.FontStyle;
            origin_font_color_ = base.Foreground;
        }

        public void SetWaterMarkText(String water_mark)
        {
            water_mark_text_ = water_mark;

            SetWaterMark();
        }

        private void SetWaterMark()
        {
            base.FontStyle = FontStyles.Italic;
            base.Foreground = new SolidColorBrush(Colors.Gray);

            base.Text = water_mark_text_;
        }

        private void BackToOriginSetting()
        {
            base.Foreground = origin_font_color_;
            base.FontStyle = origin_style_;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            BackToOriginSetting();
            base.Text = last_text_;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            last_text_ = base.Text;

            if (base.Text == "")
                SetWaterMark();
        }
    }

    public class FolderSelectionTextBox : WaterMarkTextBox
    {
        public event EventHandler FolderSelected = null;        

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                base.Text = dialog.SelectedPath;
                base.Focus();

                if (FolderSelected != null)
                    FolderSelected(this, null);
            }
            
            base.OnMouseDown(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            last_text_ = base.Text;
        }
    }
}
