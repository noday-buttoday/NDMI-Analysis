using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace ShapeFileControl.Event
{
    class EventControl
    {
        public event EventHandler CheckEvent;
        private static EventControl instance_ = null;
        private Queue<CustomEventArgs> msg_queue;
        private Object this_lock_;
        private bool event_process_start_;
        private Semaphore sync_;

        class QueueMsg
        {
            public object inst { get; set; }
            public List<string> mgs = new List<string>();
        }

        private EventControl()
        {
            msg_queue = new Queue<CustomEventArgs>();
            this_lock_ = new Object();

            event_process_start_ = true;
            Thread event_thread = new Thread(new ThreadStart(EventProcess));
            event_thread.Start();

            sync_ = new Semaphore(1, 1);
        }

        ~EventControl()
        {
            event_process_start_ = false;
        }

        public static EventControl Instance()
        {
            if (instance_ == null)
                instance_ = new EventControl();

            return instance_;
        }

        public void Init()
        {
        }

        public void Stop()
        {
            event_process_start_ = false;
        }

        public void SendEvent(CustomEventArgs msg)
        {
            sync_.WaitOne();
            msg_queue.Enqueue(msg);
            sync_.Release();
        }

        private void EventProcess()
        {
            while (event_process_start_)
            {
                if (msg_queue.Count > 0)
                {
                    sync_.WaitOne();
                    CustomEventArgs msg = msg_queue.Dequeue();
                    sync_.Release();

                    if (CheckEvent != null)
                        CheckEvent(msg.GetSender(), msg);
                }
            }
        }
    }
}
