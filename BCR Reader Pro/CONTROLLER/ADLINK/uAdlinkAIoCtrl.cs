using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using VSP.CONTROLLER;
//using VSP.CONTROLLER.ADLINK.Dask;

namespace VSP.CONTROLLER.ADLINK
{
    public class CAdlinkIoCtrl : CIoCtrl
    {
        private bool _connected = false;
        private short _pci9112 = -1;

        public CAdlinkIoCtrl()
        {
            m_strLogHead = "ADLINK";
            OpenDevice();
        }

        ~CAdlinkIoCtrl()
        {
            CloseDevice();
        }

        public override void OpenDevice()
        {
            // 장치 오픈 및 초기화 로직
            // 예시: AdlinkLib.Initialize()
            _pci9112 = DASK.Register_Card(DASK.PCI_9112, 0);  // 초기화 함수 예시
            if (_pci9112 < 0)
            {
                //UtilExtern.ShowInitialMessage("Analog Input Initialization Fail ");
            }
            else
            {
                DASK.AI_9112_Config(DASK.PCI_9112, DASK.TRIG_INT_PACER);
                //LOG_PRINTF(m_strLogHead, L"PCI-9112 Register_Card, %d", Pci9112);
                m_bConnected = true;
            }

            _connected = (_pci9112 >= 0);
        }

        public override void CloseDevice()
        {
            // 장치 종료
            // 예시: CAiCtrl.CloseADLINK();
        }

        public bool IsConnected() => _connected;

        public override bool IsOn(ushort idx, bool isOut = false)
        {
            return false;
        }

        public override void SetBit(ushort idx, bool bOn, bool isOut = true)
        {

        }

        public override byte GetByte(int idx, bool isOut = false)
        {
            return 0x00;
        }

        public override void SetByteVal(int idx, byte val, bool isOut = true)
        {

        }

        public override ushort GetWord(int idx, ushort range = 0x00, bool isOut = false)
        {
            ushort Val = 0x00;

            DASK.AI_ReadChannel(DASK.PCI_9112, (ushort)idx, range, out Val);
            Val = (ushort)((Val >> 4) + 1); // ← 형변환 추가해서 확실하게 처리!

            return Val;
        }

        public override void SetWordVal(int idx, ushort val, bool isOut = true)
        {
            //if (InRange(nIdx, 0, MAX_AI - 1))
            //{
            //    AO_9112_Config(Pci9112, nIdx, -5);
            //}
        }

        public void SetDaConfig(int idx, double max)
        {
            //if (UtilExtern.InRange(idx, 0, (int)AnalogInput.MAX_AI - 1))
            {
                DASK.AO_9112_Config((ushort)_pci9112, (ushort)idx, -5);
            }
        }

        public bool WriteAnalogVal(int idx, double val)
        {
            short sRet = DASK.ErrorInvalidIoChannel;

            if (m_bConnected)
                sRet = DASK.AO_VWriteChannel((ushort)_pci9112, (ushort)idx, val);

            return (sRet == DASK.NoError);
        }

        public double GetMaxRange(int idx)
        {
            return 0.0;
        }

        public double GetWriteAnalogVal(int idx)
        {
            return 0.0;
        }
    }
}
