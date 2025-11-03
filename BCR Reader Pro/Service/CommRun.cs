using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCR_Reader_Pro.Service
{
    internal class CommRun : BaseThread
    {
        public CommRun(int idx, int delay) : base(idx, delay)
        {

        }

        protected override int AutoRun()
        {
            return 0;
        }

    }
}
