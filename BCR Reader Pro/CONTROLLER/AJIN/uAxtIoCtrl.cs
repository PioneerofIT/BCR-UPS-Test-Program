using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP.CONTROLLER;
using VSP.CONTROLLER.VSP;


namespace VSP.CONTROLLER.AJIN
{

    /* ==========================================================================
    CAxtDioCtrl
    ========================================================================== */
    public class CAxtDioCtrl : CIoCtrl
    {
        protected struct TModuleInfo
        {
            public short ModuleNo;
            public short ByteCnt;
        }

        protected List<TModuleInfo> m_vDiModule = new List<TModuleInfo>();
        protected List<TModuleInfo> m_vDoModule = new List<TModuleInfo>();

        public CAxtDioCtrl() //: base(IoData) // IoData는 CIoCtrl 생성자에 넘길 데이터 (필요 시 수정)
        {
            m_strLogHead = "AXT_IO";  // C++의 L"AXT_IO" → C#은 그냥 문자열
            m_bConnected = false;       // C++의 초기화 리스트 값 반영
            OpenDevice();             // 장치 오픈 로직 실행
        }

        ~CAxtDioCtrl()
        {
            CloseDevice();
        }

        public override void OpenDevice()
        {
            if(CAxtDIO.InitializeDIO() == -1 )
            {
                Debug.WriteLine("Axt OpenDevice Fail\n");

                return;
            }
            else
            {
                ushort ModuleCnt = CAxtDIO.DIOget_module_count();
                TModuleInfo ModuleInfo = new TModuleInfo();
                
                for (short i = 0; i < ModuleCnt; i++)
                {
                    var ModuleId = CAxtDIO.DIOget_module_id(i);
                    ModuleInfo.ModuleNo = (short)i ;

                    if(ModuleId == (ushort) AXT_FUNC_MODULE.AXT_SIO_DO32P || ModuleId == (ushort)AXT_FUNC_MODULE.AXT_SIO_DO32T)
                    {
                        ModuleInfo.ByteCnt = 4;
                        m_vDoModule.Add(ModuleInfo);
                    }
                    else if(ModuleId == (ushort)AXT_FUNC_MODULE.AXT_SIO_DB32P || ModuleId == (ushort)AXT_FUNC_MODULE.AXT_SIO_DB32T)
                    {
                        ModuleInfo.ByteCnt = 2;
                        m_vDoModule.Add(ModuleInfo);
                        m_vDiModule.Add(ModuleInfo);

                    }
                    else if(ModuleId == (ushort)AXT_FUNC_MODULE.AXT_SIO_DI32) 
                    {
                        ModuleInfo.ByteCnt = 4;
                        m_vDiModule.Add(ModuleInfo);
                    }
                    ModuleInfo = default;


                }
            }
            m_bConnected = true;

        }
        public override void CloseDevice() { }

        public void ReadDiModule()
        {
            var BufIndex = 0;
            for (var i = 0; i < m_vDiModule.Capacity; i++)
            {
                for(ushort Offset = 0;  Offset < m_vDiModule[i].ByteCnt; Offset++, BufIndex++)
                {
                    if(m_bConnected)
                    {
                        rdCifData[BufIndex] = CAxtDIO.DIOread_inport_byte(m_vDiModule[i].ModuleNo, Offset);
                    }
                    else
                    {
                        rdMachineData[BufIndex] = rdCifData[BufIndex];
                    }
                }
            }
            int ElementCount = m_vDiModule.Count / sizeof(ushort);
            Array.Copy(rdMachineData, 0, rdCifData, 0, ElementCount);
        }
        public void ReadDoModule()
        {
            var BufIndex = 0;
            for (var i = 0; i < m_vDoModule.Capacity; i++)
            {
                for (ushort Offset = 0; Offset < m_vDoModule[i].ByteCnt; Offset++, BufIndex++)
                {
                    if (m_bConnected)
                    {
                        wrCifData[BufIndex] = CAxtDIO.DIOread_outport_byte(m_vDoModule[i].ModuleNo, Offset);
                    }
                    else
                    {
                        wrCifData[BufIndex] = wrMachineData[BufIndex];
                    }
                }
            }
        }
        public void WriteDoModule()
        {
            int BufIndex = 0;

            for (int i = 0; i < m_vDoModule.Count; i++)
            {
                var module = m_vDoModule[i];

                for (ushort offset = 0; offset < module.ByteCnt; offset++, BufIndex++)
                {
                    byte targetValue = wrMachineData[BufIndex]; // 실제 출력값

                    // 현재 출력 상태와 비교 (옵션)
                    if (CAxtDIO.DIOread_outport_byte(module.ModuleNo, offset) != targetValue)
                    {
                        // DO 출력 전송
                        CAxtDIO.DIOwrite_outport_byte(module.ModuleNo, offset, targetValue);

                        // Optional: 디버그 로그 출력
//#if DEBUG
//                        string log = $"[DO] Buf[{BufIndex}] = {targetValue} → ModNo:{module.ModuleNo}, Offset:{offset}";
//                        System.Diagnostics.Debug.WriteLine(log);
//#endif
                    }
                }
            }
        }

        public override bool IsOn(ushort nIdx, bool IsOut = false)
        {
            bool bitOn = (IsOut ? CAxtDIO.DIOread_outport(nIdx) : CAxtDIO.DIOread_inport(nIdx)) == 1;

            return bitOn;
        }

        public override void SetBit(ushort nIdx, bool bOn, bool IsOut = true)
        {
            if (IsOut)
            {
                CAxtDIO.DIOwrite_outport(nIdx, bOn);
            }

        }

        public override byte GetByte(int nIdx, bool IsOut = false)
        {
            if (IsOut)
            {
                ReadDoModule();
                return wrCifData[nIdx];
            }
            else
            {
                ReadDiModule();
                return rdCifData[nIdx];
            }
        }

        public override void SetByteVal(int nIdx, byte byVal, bool IsOut = true)
        {
            bool ValidIndexRange = IsOut && (nIdx >= 0 && nIdx <= IO_CONFIG.AO_START-1);

            if (ValidIndexRange)
            {
                wrMachineData[nIdx] = byVal;
                WriteDoModule();
            }
            else
            {
                //QueryMsgDlg.ShowMsg("SetByteVal ValidIndexRange is false");
            }

        }

        public override ushort GetWord(int nIdx, ushort wRange = 0x00, bool isOut = false)
        {
            ushort uRet = 0x00;
            int wordIdx = nIdx * 2;

            if (isOut)
            {
                ReadDoModule();

                if (wordIdx + 1 < wrCifData.Length)
                    uRet = (ushort)((wrCifData[wordIdx]) | (wrCifData[wordIdx + 1] << 8));


                Debug.WriteLine(
                    $"[OUT] [{wrCifData[wordIdx]:X2}, {wrCifData[wordIdx + 1]:X2}] → {uRet}");

            }
            else
            {
                ReadDiModule();

                if (wordIdx + 1 < rdCifData.Length)
                    uRet = (ushort)((rdCifData[wordIdx]) | (rdCifData[wordIdx + 1] << 8));


                Debug.WriteLine(
                    $"[IN ] [{rdCifData[wordIdx]:X2}, {rdCifData[wordIdx + 1]:X2}] → {uRet}");

            }

            return uRet;
        }

        public override void SetWordVal(int nIdx, ushort wVal, bool isOut = true)
        {
            // Index 범위 유효성 검사: AO 시작 이전까지만 허용
            bool isValidRange = isOut && (nIdx >= 0 && nIdx <= IO_CONFIG.AO_START - 2);

            if (isValidRange)
            {
                int wordIdx = nIdx * 2;

                // C++처럼 WORD → 2 BYTE 분할
                wrMachineData[wordIdx] = (byte)(wVal & 0x00FF);        // 하위 바이트
                wrMachineData[wordIdx + 1] = (byte)((wVal >> 8) & 0x00FF); // 상위 바이트

                // 실제 출력 모듈에 적용
                WriteDoModule();
            }
            else
            {
                //QueryMsgDlg.ShowMsg($"SetWordVal Index Range Over: {nIdx}\n");
            }
        }
    }

    /* ==========================================================================
    CAxtAioCtrl
    ========================================================================== */
    public class CAxtAioCtrl : CIoCtrl
    {
        private bool _connected = false;
        private int _channelCount = 4;

        public CAxtAioCtrl()
        {
            m_strLogHead = "AXT_AIO";
            OpenDevice();
        }

        public override void OpenDevice()
        {
            if (CAxtAIO.InitializeAIO())
            {
                _channelCount = CAxtAIO.AIOget_channel_number_dac(); // ← AIOget_channel_number_dac()
                _connected = true;
            }
            else
            {
                //UtilExtern.ShowInitialMessage("InitializeAIO() Fail");
                //QueryMsgDlg.ShowMsg("InitializeAIO() Fail");
            }
        }

        public override void CloseDevice()
        {
            // 연결 종료 로직 필요 시 여기에 작성
            // CAxtAIO.CloseAll(); 등 호출 가능
        }

        public bool IsConnected() => _connected;

        public void SetDaConfig(int idx, double max)
        {
            if (InRange(idx, 0, _channelCount - 1))
                CAxtAIO.AIOset_range_dac((short)idx, 0.0, max); // ← AIOset_range_dac
        }

        public bool WriteAnalogVal(int idx, double value)
        {
            if (InRange(idx, 0, _channelCount - 1))
                return CAxtAIO.AIOwrite_dac((short)idx, value); // ← AIOwrite_dac

            return false;
        }

        public double GetMaxRange(int idx)
        {
            double min = 0.0, max = 0.0;
            CAxtAIO.AIOget_range_dac((short)idx, ref min, ref max); // ← AIOget_range_dac
            return max;
        }

        public double GetWriteAnalogVal(int idx)
        {
            return CAxtAIO.AIOread_dac((short)idx); // ← AIOread_dac
        }

        private bool InRange(int val, int min, int max)
        {
            return val >= min && val <= max;
        }
    }
    /* ==========================================================================
    CAxtMotionDioCtrl
    ========================================================================== */
    public class CAxtMotionDioCtrl : CIoCtrl
    {
        private bool _connected = false;
        private List<short> _diModules = new();
        private List<short> _doModules = new();

        public CAxtMotionDioCtrl()
        {
            m_strLogHead = "AXT_MOTION_IO";
            OpenDevice();
        }

        public override void OpenDevice()
        {
            InitBuffer();

            if (CAxtMotorCtrl.GetAxtMotionLibInit())
            {
                int totalAxis = CAxtMotorCtrl.GetTotalAxisCount();
                for (int i = 0; i < totalAxis; i++)
                {
                    _diModules.Add((short)i);
                    _doModules.Add((short)i);
                }

                Debug.WriteLine($"{m_strLogHead} MOTION Axis Count = {totalAxis}");
                _connected = true;
            }
            else
            {
                _connected = false;
            }
        }

        public override void CloseDevice()
        {
            // 연결 해제 로직 필요 시 구현
        }

        private void InitBuffer()
        {
            _diModules.Clear();
            _doModules.Clear();
        }

        public void ReadDiModule()
        {
            // 필요 시 모션 입력 버퍼 전체 읽기 구현
        }

        public void ReadDoModule()
        {
            // 필요 시 모션 출력 버퍼 전체 읽기 구현
        }

        public void WriteDoModule()
        {
            // 필요 시 전체 DO 상태를 모듈에 반영하는 로직 구현
        }

        public override bool IsOn(ushort idx, bool isOut = false)
        {
            int axisNo = idx / 8;
            int offset = idx % 8;

            return isOut
                ? CAxtMotorCtrl.GetOutputBit(axisNo, offset)
                : CAxtMotorCtrl.GetInputBit(axisNo, offset);
        }

        public override void SetBit(ushort idx, bool bOn, bool isOut = true)
        {
            int axisNo = idx / 8;
            int offset = idx % 8;

            if (isOut)
            {
                if (bOn)
                    CAxtMotorCtrl.SetOutputBit(axisNo, offset);
                else
                    CAxtMotorCtrl.ResetOutputBit(axisNo, offset);
            }
        }

        public override byte GetByte(int idx, bool isOut = false)
        {
            int axisNo = idx / 8;

            return isOut
                ? CAxtMotorCtrl.GetOutputByte(axisNo)
                : CAxtMotorCtrl.GetInputByte(axisNo);
        }

        public override void SetByteVal(int idx, byte val, bool isOut = true)
        {
            // 필요한 경우 구현 (예: axis별 바이트 단위 출력 설정)
            // 아직 로직이 비어 있으므로 향후 제어 방식에 맞게 작성 필요
        }

        public override ushort GetWord(int idx, ushort range = 0x00, bool isOut = false)
        {
            // 워드 단위 입력/출력 처리 필요 시 구현
            return 0;
        }

        public override void SetWordVal(int idx, ushort val, bool isOut = true)
        {
            // 워드 단위 출력 처리 필요 시 구현
        }

        public bool IsConnected() => _connected;
    }

}
