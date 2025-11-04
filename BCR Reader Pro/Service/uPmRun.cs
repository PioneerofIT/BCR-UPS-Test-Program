using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCR_Reader_Pro.Model;

namespace BCR_Reader_Pro.Service
{
    enum PmStep
    {
        PMS_INIT = 0,
        PMS_CLOSE = 10,
        PMS_VACUUM = 20,
        PMS_GAS_ON = 30,
        PMS_VAC_STABLE = 35,
        PMS_RF_ON = 40,
        PMS_AUTO_COOL = 50,
        PMS_ABORT = 60,
        PMS_OPEN = 70,
        PMS_AGING_CHECK = 80,
        PMS_ERROR = 90,
        PMS_END = 100,
        PMS_LEAK_CHECK =300
    }
    enum InitStep : int
    {
        CHECK_CLEAN_DONE ,
        PURGE_ON,
        CHAMBER_OPEN,
        PURGE_OFF,
        CHECK_RF_MODE,
        END
    }
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
            switch(_initStep)
            {
                case (int)InitStep.CHECK_CLEAN_DONE:
                    if(_model.IsLidOpen())
                    {

                    }
                    //else if()
                    //{

                    //}

                        break;

                case (int)InitStep.PURGE_ON:

                break;

                case (int)InitStep.CHAMBER_OPEN:

                break;

                case (int)InitStep.PURGE_OFF:

                break;

                case (int)InitStep.CHECK_RF_MODE:

                break;

                case (int)InitStep.END:

                break;
            }
        }

    }
}
