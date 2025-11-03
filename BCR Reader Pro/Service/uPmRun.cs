using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCR_Reader_Pro.Model;

namespace BCR_Reader_Pro.Service
{
    internal class uPmRun : BaseThread
    {
        private uPmModel _model;

        public uPmRun(int idx, int delay) : base(idx, delay)
        {
            _model = new uPmModel();
        }

        protected override int AutoRun()
        {
            return 0;
        }

        protected override void DevInit()
        {
            _model.RfGenPwrOn(true);

        }

        protected override void InitRun()
        {
          

        }

    }
}
