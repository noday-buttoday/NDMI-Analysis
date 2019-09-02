using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileChecker.Builder
{
    public class PollingMaker
    {
        private PollingBuilder builder_;

        public void SetBuilder(PollingBuilder ab) { builder_ = ab; }
        public FileControl GetAutomation() { return builder_.GetPolling(); }

        public void Make(string path)
        {
            builder_.CreateAuto();
            builder_.AddChecker(path);
        }

        public void Make(string path, FileControl instance)
        {
            builder_.CreateAuto(instance);
            builder_.AddChecker(path);
        }
    }
}
