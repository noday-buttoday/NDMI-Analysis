using Module.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatelliteDataPolling.Event
{
    class EventProcess
    {
        private MainWindow main;

        public EventProcess(MainWindow main_)
        {
            main = main_;
        }

        public void MakeEventProcessor()
        {
            EventControl.Instance().AddEvent(EventControl.EVENT_TYPE.TYPE_SNOW_GUI_UPDATE, new EventHandler(SnowGuiEventProcess));
            EventControl.Instance().AddEvent(EventControl.EVENT_TYPE.TYPE_DROUGHT_GUI_UPDATE, new EventHandler(DroughtGuiEventProcess));
            EventControl.Instance().AddEvent(EventControl.EVENT_TYPE.TYPE_MODULE_FINISH, new EventHandler(ModuleFinishEventProcess));
        }
        private void DroughtGuiEventProcess(Object sender_, EventArgs args_)
        {
            DroughtUiEventArg arg = (args_ as DroughtUiEventArg);

            main.Dispatcher.Invoke(new Action(() =>
            {
                MainWindowViewModel vm = main.DataContext as MainWindowViewModel;
                vm.UpdateList(arg.event_file, arg.evnet_msg);
            }));
        }

        private void SnowGuiEventProcess(Object sender_, EventArgs args_)
        {
            SnowUiEventArg arg = (args_ as SnowUiEventArg);

            main.Dispatcher.Invoke(new Action(() =>
            {
                MainWindowViewModel vm = main.DataContext as MainWindowViewModel;
                vm.UpdateList(arg.event_file, arg.evnet_msg);
            }));
        }

        private void ModuleFinishEventProcess(Object sender_, EventArgs args_)
        {
            main.Dispatcher.Invoke(new Action(() =>
            {
                MainWindowViewModel vm = main.DataContext as MainWindowViewModel;
                vm.ModuleFinished();
            }));
        }
    }
}
