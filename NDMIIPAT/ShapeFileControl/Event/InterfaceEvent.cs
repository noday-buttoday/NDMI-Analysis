using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeFileControl.Event
{
    class CustomEventArgs : EventArgs
    {
        private String destination_;
        private Object instance_;

        public CustomEventArgs(Object instance, String destnation)
        {
            destination_ = destnation;
            instance_ = instance;
        }

        public bool ChkDestination(String name)
        {
            bool rtn = false;

            if (destination_.Equals(name))
                rtn = true;

            return rtn;
        }

        public Object GetSender()
        {
            return instance_;
        }
    }

    class ProgressEventArgs : CustomEventArgs
    {
        private double curr_;
        private double total_;

        public ProgressEventArgs(Object instance, String destination, double curr, double total)
            : base(instance, destination)
        {
            curr_ = curr;
            total_ = total;
        }

        public double GetCurrValue()
        {
            return curr_;
        }

        public double GetTotal()
        {
            return total_;
        }
    }

    class ProgressLabelEventArgs : CustomEventArgs
    {
        private string status_ = "";

        public ProgressLabelEventArgs(Object instance, String destination, String status)
            : base(instance, destination)
        {
            status_ = status;
        }

        public String GetStatus()
        {
            return status_;
        }
    }
}
