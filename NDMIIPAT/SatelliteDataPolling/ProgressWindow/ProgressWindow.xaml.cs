using System.Windows;
using System.Threading;
using System;
using Module.Event;

namespace SatelliteDataPolling.ProgressWindow
{
    /// <summary>
    /// ProgressWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private Thread ProgressThread;
        private delegate void ProgressDelegate(String msg);
        private ProgressDelegate ui_update;

        public ProgressWindow()
        {
            InitializeComponent();

            ProgressThread = new Thread(ProgressThreadCallBack);
            ProgressThread.Start();

            ui_update = new ProgressDelegate(MsgUpdate);
        }

        public void Stop()
        {
            ProgressThread.Abort();
        }

        private void ProgressEvent(Object sender, EventArgs arg)
        {
            CustomEventArgs custom = (arg as CustomEventArgs);
        }

        private void MsgUpdate(String msg)
        {
            this.TextBlock.Text = msg;
        }

        private void ProgressThreadCallBack()
        {
            while (true)
            {
                Pb.Dispatcher.Invoke(
                    new Action(delegate
                    {
                        if (Pb.Value >= 1000)
                        {
                            Pb.Value = 0;
                        }
                        else
                        {
                            Pb.Value += 10;
                        }

                    }), null);
                Thread.Sleep(10);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Stop();
        }
    }
}
