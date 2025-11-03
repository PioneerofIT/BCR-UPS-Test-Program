using VSP.CONTROLLER;
using VSP.CONTROLLER.SimulatorCtrl.Objects;

namespace VSP.CONTROLLER.SimulatorCtrl
{
    public class SimDioCtrl : CIoCtrl
    {
        public static CDI_SIMULATOR[] SIM_DI;
        public static CDO_SIMULATOR[] SIM_DO;

        public static CDI_SIMULATOR[] SIM_DI_M;
        public static CDO_SIMULATOR[] SIM_DO_M;

        private static readonly object csCtrlLock = new object();

        static SimDioCtrl()
        {
            // 정적 생성자에서 초기화
            SIM_DI = new CDI_SIMULATOR[(int)DigitalInput.MAX_DI];
            for (int i = 0; i < SIM_DI.Length; i++)
                SIM_DI[i] = new CDI_SIMULATOR();

            SIM_DO = new CDO_SIMULATOR[(int)DigitalOutput.MAX_DO];
            for (int i = 0; i < SIM_DO.Length; i++)
                SIM_DO[i] = new CDO_SIMULATOR();

            SIM_DI_M = new CDI_SIMULATOR[(int)MotionDigitalInput.MAX_DI_M];
            for (int i = 0; i < SIM_DI_M.Length; i++)
                SIM_DI_M[i] = new CDI_SIMULATOR();

            //SIM_DO_M = new CDO_SIMULATOR[(int)MotionDigitalOutput.MAX_DO_M];
            //for (int i = 0; i < SIM_DO_M.Length; i++)
            //    SIM_DO_M[i] = new CDO_SIMULATOR();
        }
        public override ushort GetWord(int index, ushort range = 0x00, bool isOut = false)
        {
            int startIdx = index * 16;
            int result = 0;

            for (int i = 0; i < 16; i++)
            {
                int idx = startIdx + i;
                if (isOut)
                {
                    if (SIM_DO[idx].IsOn())
                        result |= (1 << i);
                }
                else
                {
                    if (SIM_DI[idx].IsOn())
                        result |= (1 << i);
                }
            }

            return (ushort)result;
        }

        public override bool IsOn(ushort nIdx, bool IsOut = false)
        {
            return IsOut ? SIM_DO[nIdx].IsOn() : SIM_DI[nIdx].IsOn();
        }

        public override void SetBit(ushort nIdx, bool bOn, bool IsOut = true)
        {
            lock (csCtrlLock)
            {
                if (IsOut)
                {
                    SIM_DO[nIdx].SetOn(bOn);
                    foreach (var link in SIM_DO[nIdx].LinkedInputs)
                    {
                        bool action = link.InputOnWhenOutputOn;
                        bool val = bOn ? action : !action;

                        if (link.WireName.Contains("MX"))
                            SIM_DI_M[link.Index].SetOn(val);
                        else
                            SIM_DI[link.Index].SetOn(val);
                    }
                }
                else
                {
                    SIM_DI[nIdx].SetOn(bOn);
                }
            }
        }
    }

    public class SimMotionDioCtrl : CIoCtrl
    {
        private static readonly object csCtrlLock = new object();

        public override ushort GetWord(int index, ushort range = 0x00, bool isOut = false)
        {
            int startIdx = index * 16;
            int result = 0;

            for (int i = 0; i < 16; i++)
            {
                int idx = startIdx + i;
                if (isOut)
                {
                    if (SimDioCtrl.SIM_DO_M[idx].IsOn())
                        result |= (1 << i);
                }
                else
                {
                    if (SimDioCtrl.SIM_DI_M[idx].IsOn())
                        result |= (1 << i);
                }
            }

            return (ushort)result;
        }

        public override bool IsOn(ushort nIdx, bool IsOut = false)
        {
            return IsOut ? SimDioCtrl.SIM_DO_M[nIdx].IsOn() : SimDioCtrl.SIM_DI_M[nIdx].IsOn();
        }

        public override void SetBit(ushort nIdx, bool bOn, bool IsOut = true)
        {
            lock (csCtrlLock)
            {
                if (IsOut)
                {
                    SimDioCtrl.SIM_DO_M[nIdx].SetOn(bOn);
                    foreach (var link in SimDioCtrl.SIM_DO_M[nIdx].LinkedInputs)
                    {
                        bool action = link.InputOnWhenOutputOn;
                        bool val = bOn ? action : !action;

                        if (link.WireName.Contains("MX"))
                            SimDioCtrl.SIM_DI_M[link.Index].SetOn(val);
                        else
                            SimDioCtrl.SIM_DI[link.Index].SetOn(val);
                    }
                }
                else
                {
                    SimDioCtrl.SIM_DI_M[nIdx].SetOn(bOn);
                }
            }
        }
    }
}
