using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

using FileCheck.Event;

namespace SatelliteDataPolling.UiComponent
{
    class FileStateListView : ListView
    {
        private String name_;
        private UInt32 index_;

        public void AddEventListener()
        {
            index_ = 0;
            name_ = base.Name;
            EventControl.Instance().CheckEvent += new EventHandler(EventProcessor);
        }

        private void EventProcessor(Object sender, EventArgs e)
        {
            CustomEventArgs custom_event = (e as CustomEventArgs);

            if (custom_event.ChkDestination(name_))
                EventLogic(custom_event);
        }

        private void EventLogic(CustomEventArgs e) 
        {

        }
    }
}
