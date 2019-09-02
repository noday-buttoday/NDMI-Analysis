using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using ShapeFileControl.Event;

namespace ShapeFileControl.UiComponent
{
    class EventProgressBar : ProgressBar
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

    class OperationProgressBar : EventProgressBar
    {
        protected override void EventLogic(CustomEventArgs e)
        {
            ProgressEventArgs msg = (e as ProgressEventArgs);
            double curr = msg.GetCurrValue();
            double total = msg.GetTotal();

            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Maximum = total;
                this.Value = curr;
            }));
        }
    }
}
