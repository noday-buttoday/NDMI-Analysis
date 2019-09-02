using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileChecker;

namespace FileChecker.Builder
{
    public abstract class PollingBuilder
    {
        protected FileControl polling_;

        public PollingBuilder() { }
        public FileControl GetPolling() { return polling_; }
        public void CreateAuto() { polling_ = new FileControl(); }
        public void CreateAuto(FileControl instance) { polling_ = instance; }

        public abstract void AddChecker(string path);
    }

    public class SnowInputBuilder : PollingBuilder
    {
        public override void AddChecker(string path)
        {
            polling_.AddModule(path, "MOD03", ".hdf", false, true);
            polling_.AddModule(path, "MOD021", ".hdf", false, true);
        }
    }

    public class SnowOutputBuilder : PollingBuilder
    {
        public override void AddChecker(string path)
        {
            polling_.AddModule(path, "ndsi", ".hdr", true, false);
            polling_.AddModule(path, "ndsi", ".img", true, false);
        }
    }
}
