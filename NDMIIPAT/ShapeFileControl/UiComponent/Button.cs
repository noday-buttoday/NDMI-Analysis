using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using ShapeFileControl.Event;


namespace ShapeFileControl.UiComponent
{
    class ToggleButton : Button
    {
        protected bool flag_;

        public void SetState(bool flag)
        {
            flag_ = flag;
            Update();
        }

        public bool GetState()
        {
            return flag_;
        }

        public void ChangeState()
        {
            bool flag = (flag_ == true) ? false : true;
            flag_ = flag;
            Update();
        }

        protected virtual void Update()
        { }
    }

    class StartStopButton : ToggleButton
    {
        private String on_text_;
        private String off_text_;

        public void SetText(bool flag, String text)
        {
            if (flag)
                on_text_ = text;
            else
                off_text_ = text;
        }

        protected override void Update()
        {
            base.Update();

            Action<String> ChangeText = text =>
                {
                    base.Content = text;
                };

            String content = (flag_ == true) ? on_text_ : off_text_;

            this.Dispatcher.Invoke(ChangeText, content);
        }

        protected override void OnClick()
        {
            base.OnClick();

            ChangeState();
        }
        
    }
}
