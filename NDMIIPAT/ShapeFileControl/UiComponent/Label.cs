using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

using ShapeFileControl.Event;

namespace ShapeFileControl.UiComponent
{
    class EventLabel : Label
    {
        private String name_;

        public void AddEventListener()
        {
            name_ = base.Name;
            EventControl.Instance().CheckEvent += new EventHandler(EventProcessor);
        }

        private void EventProcessor(Object sender, EventArgs e)
        {
            CustomEventArgs custom_event = (e as CustomEventArgs);

            if (custom_event.ChkDestination(name_))
                EventLogic(custom_event);
        }

        protected virtual void EventLogic(CustomEventArgs e) { }
    }

    class ProgressLabel : EventLabel
    {
        protected override void EventLogic(CustomEventArgs e)
        {
            ProgressLabelEventArgs msg = (e as ProgressLabelEventArgs);

            this.Dispatcher.Invoke(new Action(() =>
                {
                    this.Content = msg.GetStatus();
                }));
        }
    }
}
