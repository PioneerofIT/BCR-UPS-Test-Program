using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP.CONTROLLER.VSP;

namespace VSP.CONTROLLER
{

    namespace VSP
    {
        public static class IO_CONFIG
        {
            // 🔧 Resolution Type (예: ADC, DAC)
            public const int RES_12BIT = 12;
            public const int RES_16BIT = 16;

            // 🔹 전체 버퍼 크기
            public const ushort BYTES_CNT = 44; // 총 IO 통신 바이트

            // 🔹 입력 바이트 구성: DI 20 + AI 8
            public const ushort BYTES_IN = 28;

            // 🔹 출력 바이트 구성: DO 12 + AO 8
            public const ushort BYTES_OUT = 20;

            // 🔹 시작 인덱스 설정
            public const ushort AI_START = 20;  // AI는 입력 바이트 중간에서 시작
            public const ushort AO_START = 12;  // AO는 출력 바이트 중간에서 시작

            // 🔹 버퍼 개수 (예: FIFO나 중복 버퍼 등)
            public const int BUF_CNT = 5;

            // ✨ 추가 확장 포인트
            // public const bool USE_CIF = true; // CIF 사용 여부
            // public const int CIF_IO_GROUPS = 2;
        }
    }
    /* ==========================================================================
	TVsIoData Class
    ========================================================================== */
    public class TVsIoData
    {
        public List<byte> ReadIn { get; private set; } = new List<byte>();
        public List<byte> ReadOut { get; private set; } = new List<byte>();
        public List<byte> WriteOut { get; private set; } = new List<byte>();

        public TVsIoData()
        {
            ReadIn = new List<byte>();
            ReadOut = new List<byte>();
            WriteOut = new List<byte>();
        }

        public TVsIoData(int inSize, int outSize)
        {
            SetSize(inSize, outSize);
        }

        public void SetSize(int inBytes, int outBytes)
        {
            ReadIn = new List<byte>(new byte[inBytes]);
            ReadOut = new List<byte>(new byte[outBytes]);
            WriteOut = new List<byte>(new byte[outBytes]);
        }

        public int GetSize(bool isOut)
        {
            return isOut ? ReadOut.Count : ReadIn.Count;
        }
    }
    /* ==========================================================================
        CIoCtrl Class
    ========================================================================== */
    public class CIoCtrl
    {
        protected bool m_bConnected;
        protected TVsIoData m_IoData; // 초기화 제거
        protected string m_strLogHead = "";


        public static byte[] rdMachineData = new byte[IO_CONFIG.BYTES_IN];
        public static byte[] rdCifData = new byte[IO_CONFIG.BYTES_IN];
        public static byte[] wrCifData = new byte[IO_CONFIG.BYTES_OUT];
        public static byte[] wrMachineData = new byte[IO_CONFIG.BYTES_OUT];


        //public static byte[] rdMcDataAxt = new byte[ASING_TYPE.AXT_BYTES_IN];
        //public static byte[] rdAxtData = new byte[ASING_TYPE.AXT_BYTES_IN];
        //public static byte[] wrAxtData = new byte[ASING_TYPE.AXT_BYTES_OUT];
        //public static byte[] wrMcDataAxt = new byte[ASING_TYPE.AXT_BYTES_OUT];
        // 기본 생성자
        public CIoCtrl()
        {
            m_bConnected = false;
            m_IoData = new TVsIoData(); // ✅ 생성 시 직접 초기화
        }

        // 매개변수가 있는 생성자
        public CIoCtrl(TVsIoData IoData)
        {
            m_bConnected = false;
            m_IoData = IoData ?? new TVsIoData(); // ✅ null 방지
        }

        ~CIoCtrl()
        {
            CloseDevice();
        }

        public bool IsConnected()
        {
            return m_bConnected;
        }

        public virtual void OpenDevice() { }
        public virtual void CloseDevice() { }

        public virtual bool IsOn(ushort nIdx, bool IsOut = false) { return false; }
        public virtual void SetBit(ushort nIdx, bool bOn, bool IsOut = true) { }
        public virtual byte GetByte(int nIdx, bool IsOut = false) { return 0; }
        public virtual void SetByteVal(int nIdx, byte byVal, bool IsOut = true) { }
        public virtual ushort GetWord(int nIdx, ushort wRange = 0x00, bool IsOut = false) { return 0; }
        public virtual void SetWordVal(int nIdx, ushort wVal, bool IsOut = true) { }
        public virtual void SetDaConfig(ushort nIdx, double dMax) { }
        public virtual bool WriteAnalogVal(ushort nIdx, double dVal) { return false; }

        public virtual double GetMaxRange(ushort nIdx) { return 0.0; }
        public virtual double GetWriteAnalogVal(ushort nIdx) { return 0.0; }
    }
}

