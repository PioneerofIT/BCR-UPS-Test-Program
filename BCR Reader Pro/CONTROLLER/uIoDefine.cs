using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP.CONTROLLER
{
    /* ==========================================================================
    Description : DI define
    ========================================================================== */
    public enum DigitalInput
    {
        X000 = 0, X001, X002, X003, X004, X005, X006, X007, X008, X009, X00A, X00B, 

        MAX_DI
    }
    public static class Din
    {
        public static CDI[] DI = new CDI[(int)DigitalInput.MAX_DI];

        public static void Initialize(CIoCtrl controller)
        {
            CDI.SetIoController(controller);
            for (int i = 0; i < DI.Length; i++)
                DI[i] = new CDI((ushort)i);
        }

        public static CDI I_PumpOvld_Btm => DI[(int)DigitalInput.X000];
        public static CDI I_MainAirPres => DI[(int)DigitalInput.X001];
        public static CDI I_ECoolOn_Btm => DI[(int)DigitalInput.X002];
        public static CDI I_Gas1Pres => DI[(int)DigitalInput.X003];
        public static CDI I_Gas2Pres => DI[(int)DigitalInput.X004];
        public static CDI I_Gas3Pres => DI[(int)DigitalInput.X005];
        public static CDI I_N2Pres => DI[(int)DigitalInput.X006];
        public static CDI I_VacValveOpen_Btm => DI[(int)DigitalInput.X007];
        public static CDI I_ChamberOpen_Btm => DI[(int)DigitalInput.X008];

        public static CDI I_ChamberClose_Btm => DI[(int)DigitalInput.X009];
        public static CDI I_ChamberInStripDetect_Btm => DI[(int)DigitalInput.X00A];
        public static CDI I_ChamberOutStripDetect_Btm => DI[(int)DigitalInput.X00B];
  



    }
    /* ==========================================================================
     Description : DO define
     ========================================================================== */
    public enum DigitalOutput
    {
        Y000 = 0, Y001, Y002, Y003, Y004, Y005, Y006, Y007, Y008, Y009, Y00A, Y00B, 

        MAX_DO
    }
    public static class Dout
    {
        public static CDO[] DO = new CDO[(int)DigitalOutput.MAX_DO];

        public static void Initialize(CIoCtrl controller)
        {
            CDO.SetIoController(controller);
            for (int i = 0; i < DO.Length; i++)
                DO[i] = new CDO((ushort)i);
        }

        public static CDO O_RfGenPower_Btm => DO[(int)DigitalOutput.Y000];
        public static CDO O_VacPumpPwr_Btm => DO[(int)DigitalOutput.Y001];
        public static CDO O_ECool_Btm => DO[(int)DigitalOutput.Y002];
        public static CDO O_Gas1Open_Btm => DO[(int)DigitalOutput.Y003];
        public static CDO O_Gas2Open_Btm => DO[(int)DigitalOutput.Y004];
        public static CDO O_Gas3Open_Btm => DO[(int)DigitalOutput.Y005];
        public static CDO O_N2Purge_Btm => DO[(int)DigitalOutput.Y006];
        public static CDO O_VacValveOpen_Btm => DO[(int)DigitalOutput.Y007];
        public static CDO O_AirPurge_Btm => DO[(int)DigitalOutput.Y008];
        public static CDO O_GaugeValOpen_Btm => DO[(int)DigitalOutput.Y009];
        public static CDO O_ChamberOpen_Btm => DO[(int)DigitalOutput.Y00A];
        public static CDO O_ChamberClose_Btm => DO[(int)DigitalOutput.Y00B];



    }

    /* ==========================================================================
     Description : Analog Input Define
     ========================================================================== */
    public enum AnalogInput : int
    {
        AX00 = 0, AX01, AX02, AX03, AX04, AX05, AX06, AX07,
        MAX_AI
    }
    public static class Ain
    {
        public static CAnalogIn[] AI = new CAnalogIn[(int)AnalogInput.MAX_AI];

        //public static void Initialize(CIoCtrl controller)
        //{
        //    CAnalogIn.SetIoController(controller);
        //    for (int i = 0; i < AI.Length; i++)
        //        AI[i] = new CAnalogIn((ushort)i);
        //}

        // 🔖 시그널 별칭
        public static CAnalogIn AI_Mfc1_Pm1 => AI[(int)AnalogInput.AX00];
        public static CAnalogIn AI_Mfc2_Pm1 => AI[(int)AnalogInput.AX01];
        public static CAnalogIn AI_Mfc3_Pm1 => AI[(int)AnalogInput.AX02];
        public static CAnalogIn AI_Vac_Pm1 => AI[(int)AnalogInput.AX03];
        public static CAnalogIn AI_AX04 => AI[(int)AnalogInput.AX04];
        public static CAnalogIn AI_AX05 => AI[(int)AnalogInput.AX05];
        public static CAnalogIn AI_AX06 => AI[(int)AnalogInput.AX06];
        public static CAnalogIn AI_AX07 => AI[(int)AnalogInput.AX07];
    }
    /* ==========================================================================
     Description : Analog OutPut Define
     ========================================================================== */
    public enum AnalogOutput : int
    {
        AY00 = 0, AY01, AY02, AY03, AY04,
        MAX_AO
    }

    public static class Aout
    {
        public static CAnalogOut[] AO = new CAnalogOut[(int)AnalogOutput.MAX_AO];

        //public static void Initialize(CIoCtrl controller)
        //{
        //    CAnalogOut.SetIoController(controller);
        //    for (int i = 0; i < AO.Length; i++)
        //        AO[i] = new CAnalogOut((ushort)i);
        //}

        // 🔖 시그널 별칭
        public static CAnalogOut AO_Mfc1_Pm1 => AO[(int)AnalogOutput.AY00];
        public static CAnalogOut AO_Mfc2_Pm1 => AO[(int)AnalogOutput.AY01];
        public static CAnalogOut AO_LoadConv_Spd => AO[(int)AnalogOutput.AY02];
        public static CAnalogOut AO_UnldConv_Spd => AO[(int)AnalogOutput.AY03];
        public static CAnalogOut AO_Mfc3_Pm1 => AO[(int)AnalogOutput.AY04];
    }
    /* ==========================================================================
     Description : Motion IN define
     ========================================================================== */

    public enum MotionDigitalInput
    {
        MX000 = 0, MX001, MX002, MX003, MX004, MX005, MX006, MX007,
        MX008, MX009, MX00A, MX00B, MX00C, MX00D, MX00E, MX00F,
        MX010, MX011, MX012, MX013, MX014, MX015, MX016, MX017,
        MX018, MX019, MX01A, MX01B, MX01C, MX01D, MX01E, MX01F,
        MAX_DI_M
    }
    public static class MotionIn
    {
        public static CDI_M[] DI_MTR = new CDI_M[(int)MotionDigitalInput.MAX_DI_M];

        //public static void Initialize(CIoCtrl controller)
        //{
        //    CDI_M.SetIoController(controller);
        //    for (int i = 0; i < DI_MTR.Length; i++)
        //        DI_MTR[i] = new CDI_M(i);
        //}

        // 🔖 별칭 매핑
        public static CDI_M I_Axis00_Org => DI_MTR[(int)MotionDigitalInput.MX000];
        public static CDI_M I_Axis00_PhaseZ => DI_MTR[(int)MotionDigitalInput.MX001];
        public static CDI_M I_Axis00_MX2 => DI_MTR[(int)MotionDigitalInput.MX002];
        public static CDI_M I_LdCV1_BoatMiddle => DI_MTR[(int)MotionDigitalInput.MX003];
        public static CDI_M I_LdCV2_BoatMiddle => DI_MTR[(int)MotionDigitalInput.MX004];
        public static CDI_M I_Axis00_MX05 => DI_MTR[(int)MotionDigitalInput.MX005];
        public static CDI_M I_Axis00_MX6 => DI_MTR[(int)MotionDigitalInput.MX006];
        public static CDI_M I_Axis00_MX7 => DI_MTR[(int)MotionDigitalInput.MX007];
        public static CDI_M I_Axis01_Org => DI_MTR[(int)MotionDigitalInput.MX008];
        public static CDI_M I_Axis01_PhaseZ => DI_MTR[(int)MotionDigitalInput.MX009];
        public static CDI_M I_Axis01_MX2 => DI_MTR[(int)MotionDigitalInput.MX00A];
        public static CDI_M I_LoadIonizerAlarm => DI_MTR[(int)MotionDigitalInput.MX00B];
        public static CDI_M I_UnldIonizerAlarm => DI_MTR[(int)MotionDigitalInput.MX00C];
        public static CDI_M I_UnldCV1_BoatMiddle1 => DI_MTR[(int)MotionDigitalInput.MX00D];
        public static CDI_M I_Axis01_MX6 => DI_MTR[(int)MotionDigitalInput.MX00E];
        public static CDI_M I_Axis01_MX7 => DI_MTR[(int)MotionDigitalInput.MX00F];
        public static CDI_M I_Axis02_Org => DI_MTR[(int)MotionDigitalInput.MX010];
        public static CDI_M I_Axis02_PhaseZ => DI_MTR[(int)MotionDigitalInput.MX011];
        public static CDI_M I_Axis02_MX2 => DI_MTR[(int)MotionDigitalInput.MX012];
        public static CDI_M I_UnldCV1_BoatMiddle2 => DI_MTR[(int)MotionDigitalInput.MX013];
        public static CDI_M I_UnldCV2_BoatMiddle1 => DI_MTR[(int)MotionDigitalInput.MX014];
        public static CDI_M I_UnldCV2_BoatMiddle2 => DI_MTR[(int)MotionDigitalInput.MX015];
        public static CDI_M I_Axis02_MX6 => DI_MTR[(int)MotionDigitalInput.MX016];
        public static CDI_M I_Axis02_MX7 => DI_MTR[(int)MotionDigitalInput.MX017];
        public static CDI_M I_Axis03_Org => DI_MTR[(int)MotionDigitalInput.MX018];
        public static CDI_M I_Axis03_PhaseZ => DI_MTR[(int)MotionDigitalInput.MX019];
        public static CDI_M I_Axis03_MX2 => DI_MTR[(int)MotionDigitalInput.MX01A];
        public static CDI_M I_Axis03_MX3 => DI_MTR[(int)MotionDigitalInput.MX01B];
        public static CDI_M I_Axis03_MX4 => DI_MTR[(int)MotionDigitalInput.MX01C];
        public static CDI_M I_Axis03_MX5 => DI_MTR[(int)MotionDigitalInput.MX01D];
        public static CDI_M I_Axis03_MX6 => DI_MTR[(int)MotionDigitalInput.MX01E];
        public static CDI_M I_Axis03_MX7 => DI_MTR[(int)MotionDigitalInput.MX01F];
    }

    public class CVS_IO_OBJ_MANAGER
    {
        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================

        private readonly string _logHead = "IO_Mng";

        public List<CDI> DI = new();          // Digital Input
        public List<CDO> DO = new();          // Digital Output
        public List<CAnalogIn> AI = new();    // Analog Input
        public List<CAnalogOut> AO = new();   // Analog Output
        public List<CDI_M> DI_MTR = new();    // Motion DI

        // 별칭

        // ============================================================
        // Description : [1] 객체 생명 주기
        // ============================================================

        public CVS_IO_OBJ_MANAGER()
        {
            CreateObjects();
            MakeAlias();
            MakeMotionIoAlias();
        }

        ~CVS_IO_OBJ_MANAGER()
        {
            DeleteObjects();
        }

        // ============================================================
        // Description : [3] 객체 생성 및 삭제
        // ============================================================
        private void CreateObjects()
        {
            // ① Digital Input (DI)
            for (int i = 0; i < (int)DigitalInput.MAX_DI; i++)
            {
                var di = new CDI((ushort)i);
                DI.Add(di);               // Manager 내부 리스트
                Din.DI[i] = di;           // 별칭 클래스 배열 채움
            }
            //UtilExtern.ShowInitialMessage($"{_logHead} DI Size : {DI.Count}");
            Debug.WriteLine($"{_logHead} DI Size : {DI.Count}");

            // ② Digital Output (DO)
            for (int i = 0; i < (int)DigitalOutput.MAX_DO; i++)
            {
                var doObj = new CDO((ushort)i);
                DO.Add(doObj);
                Dout.DO[i] = doObj;
            }
            //UtilExtern.ShowInitialMessage($"{_logHead} DO Size : {DO.Count}");
            Debug.WriteLine($"{_logHead} DO Size : {DO.Count}");

            for (int i = 0; i < (int)AnalogInput.MAX_AI; i++)
            {
                var ai = new CAnalogIn(i);
                AI.Add(ai);
                Ain.AI[i] = ai;
            }
            //UtilExtern.ShowInitialMessage($"{_logHead} AI Size : {AI.Count}");
            Debug.WriteLine($"{_logHead} AI Size : {AI.Count}");

            for (int i = 0; i < (int)AnalogOutput.MAX_AO; i++)
            {
                var ao = new CAnalogOut(i);
                AO.Add(ao);
                Aout.AO[i] = ao;
            }
            //UtilExtern.ShowInitialMessage($"{_logHead} AO Size : {AO.Count}");
            Debug.WriteLine($"{_logHead} AO Size : {AO.Count}");

            for (int i = 0; i < (int)MotionDigitalInput.MAX_DI_M; i++)
            {
                var mDi = new CDI_M(i);
                DI_MTR.Add(mDi);
                MotionIn.DI_MTR[i] = mDi;
            }
            //UtilExtern.ShowInitialMessage($"{_logHead} Motion DI Size : {DI_MTR.Count}");
            Debug.WriteLine($"{_logHead} Motion DI Size : {DI_MTR.Count}");
        }

        private void DeleteObjects()
        {
            DI.Clear();
            DO.Clear();
            AI.Clear();
            AO.Clear();
            DI_MTR.Clear();
        }

        // ============================================================
        // Description : [4] 별칭 매핑
        // ============================================================

        private void MakeAlias()
        {
            // INPUT =======================
            //Din.I_EmergencyFront = Din.DI[(int)DigitalInput.X000];
        //    I_EmergencyFront = DI[(int)DigitalInput.X000]; // 명시적 매핑


            //// OUTPUT =======================
            //O_TowerRed = DO[(int)DigitalOutput.Y000];
            // ... 추가 매핑
            // 예시:
            // O_UnldRailStripLiftUp_4   = DO[(int)DigitalOutput.Y05E];
            // O_UnldRailStripLiftDown_4 = DO[(int)DigitalOutput.Y05F];

            // ANALOG =======================
            //AI_Mfc1_Pm1 = AI[(int)AnalogInput.AX00];
            //AO_Mfc3_Pm1 = AO[(int)AnalogOutput.AY04];
        }

        private void MakeMotionIoAlias()
        {
            // MOTION DIGITAL INPUT =======================
            //I_Axis00_Org = DI_MTR[(int)MotionDigitalInput.MX000];
            // ... 추가 매핑
            // 예시:
            // I_Axis03_MX6 = DI_MTR[(int)MotionDigitalInput.MX01E];
            // I_Axis03_MX7 = DI_MTR[(int)MotionDigitalInput.MX01F];
        }
    }
}
