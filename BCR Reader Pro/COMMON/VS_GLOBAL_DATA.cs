using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP.COMMON.BASE_COMPONENT;
using VSP.COMMON;

namespace VSP.COMMON
{
    /* ==========================================================================
    Description	: Global public static hsjangstatic
    ========================================================================== */
    public static class GlobalExtern
    {

        // ============================================================  
        // Description : [3] Global Methods  
        // ============================================================  

        // 로그 문자열 가져오기  
        public static string GetLogString(string sVal)
        {
            // 구현 필요  
            return $"Log: {sVal}";
        }

        // 이벤트 로그 추가  
        public static void AddEventLog(string sLogMsg, LOG_KIND nKind, int nCntOrErrNo)
        {
            // 구현 필요  
            Console.WriteLine($"Event Log: {sLogMsg}, Kind: {nKind}, Count/Error: {nCntOrErrNo}");
        }

        // 로그 출력 상태 확인  
        public static bool IsLogOutState()
        {
            // 구현 필요  
            return false;
        }

        // 로그인 레벨 확인  
        public static bool IsCorrectLoginLevel(LOGIN_LEVEL needLevel)
        {
            // 구현 필요  
            return true;
        }

        // 날짜 변환  
        public static bool GetDate(string sDate, out DateTime dtDate)
        {
            return DateTime.TryParse(sDate, out dtDate);
        }
    }

    /* ==========================================================================
    Description	: ComponentRunTime Class
    ========================================================================== */
    public class ComponentRunTime
    {
        // ============================================================
        // Description : [1] Properties
        // ============================================================
        private long hour;
        private int minute;
        private int second;
        private bool minChanged;
        private int lifeTime;

        // ============================================================
        // Description : [2] Interface - Common Methods (생성자 / Clear / CopyFrom)
        // ============================================================
        public ComponentRunTime()
        {
            hour = 0;
            minute = 0;
            second = 0;
            minChanged = false;
            lifeTime = 0;
        }

        public void Clear()
        {
            hour = 0;
            minute = 0;
            second = 0;
            minChanged = false;
        }

        public void CopyFrom(ComponentRunTime arg)
        {
            if (arg == null) return;

            hour = arg.hour;
            minute = arg.minute;
            second = arg.second;
            minChanged = arg.minChanged;
            lifeTime = arg.lifeTime;
        }

        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================
        public int GetCompLifeTime()
        {
            return lifeTime;
        }

        public void SetCompLifeTime(int value)
        {
            lifeTime = value;
        }

        public bool IsMinChanged()
        {
            return minChanged;
        }

        public void SetMinChanged(bool value)
        {
            minChanged = value;
        }

        public void IncreaseSecond()
        {
            second++;
            if (second >= 60)
            {
                second = 0;
                minute++;
                minChanged = true;
            }
        }

        public void ResetTime()
        {
            hour = 0;
            minute = 0;
            second = 0;
            minChanged = false;
        }

        public bool IsRunTimeExceedLifeTime()
        {
            return hour * 3600 + minute * 60 + second > lifeTime;
        }

        // ============================================================
        // Description : [4] Internal Logic / Validation (데이터 검증)
        // ============================================================
        public void LoadCompRunTime(string value)
        {
            // 구현 필요
        }

        public string GetCompRunTime()
        {
            return $"{hour:D2}:{minute:D2}:{second:D2}";
        }

        public string GetRunTimeFmtStr(bool isShort)
        {
            return isShort ? $"{minute:D2}:{second:D2}" : $"{hour:D2}:{minute:D2}:{second:D2}";
        }

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        public void MakeDefault()
        {
            hour = 0;
            minute = 0;
            second = 0;
            minChanged = false;
            lifeTime = 0;
        }
    }


    public sealed class CGlobal
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle)
        // ============================================================

        // ============================================================
        // Description : [1-1] 생성자
        // ============================================================

        private CGlobal()
        {
            InitRuntimeComponents();    //동적 아이템 초기화
            SetSysPath();
            GlobalFile = DataDir + CongfigFileTypeNames.SYSINIFILE;
            InitValues();
        }

        ~CGlobal()
        {

        }

        private void InitRuntimeComponents()
        {
            int max = (int)PM.Max;

            PumpRunTime = new ComponentRunTime[max];
            RfGenRunTime = new ComponentRunTime[max];

            for (int i = 0; i < max; i++)
            {
                PumpRunTime[i] = new ComponentRunTime();
                RfGenRunTime[i] = new ComponentRunTime();
            }

        }

        private void InitValues()
        {
            GlobalFile = string.Empty;
            recipe = new CvsRecipe();
            recipeMode = 0;
            totalProd = 0;
            todayProd = 0;
            shift = 0;
            shiftStartTime = DateTime.Now;
            autoRun = false;
            dryRun = false;
            emgOn = false;
            errFlag = false;
            seqMode = 0;
            commLogOn = false;
            lastErr = string.Empty;
            ionizerOffReq = false;
            lampOffReq = false;
            startAgingTime = 0;
        }

        public void MakeDefault()
        {
         //
            totalProd = 0;
            todayProd = 0;
            shift = 0;
            shiftStartTime = DateTime.Now;
        }

        public void Initialize()
        {
            // 생성자 내부에서 Initialize를 호출하면 안 돼요
        }

        // ============================================================
        // Description : [1-2] 인터페이스 구현 (초기화, 로컬라이징 등)
        // ============================================================

        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        public static CGlobal Instance => instance ??= new CGlobal();
        private static CGlobal instance;

        public string GlobalFile { get; private set; }
        public CvsRecipe recipe { get; private set; }
        public int recipeMode { get; set; }

        public long totalProd { get; set; }
        public long todayProd { get; set; }
        public int shift { get; set; }
        public DateTime shiftStartTime { get; set; }

        public bool autoRun { get; set; }
        public bool dryRun { get; set; }
        public bool emgOn { get; set; }
        public bool errFlag { get; set; }
        public int seqMode { get; set; }
        public bool commLogOn { get; set; }
        public string lastErr { get; set; }
        public bool ionizerOffReq { get; set; }
        public bool lampOffReq { get; set; }

        public uint startAgingTime { get; set; }

        public bool ValidateSystemOptions()
        {
            return !string.IsNullOrEmpty(GlobalFile);
        }


        // 📁 경로 속성 선언 (읽기 전용)
        public string ExecuteDir { get; private set; }
        public string DataDir { get; private set; }
        public string LogDir { get; private set; }
        public string ReportDir { get; private set; }
        public string CimDir { get; private set; }
        public string ResDir { get; private set; }
        public string BcrImgSavePgmDir { get; private set; }
        public string BcrImgSavePath { get; private set; }


        // [RECIPE]
        public string RecipeName { get; set; } = "DefaultRecipe";
        // public string AgingRecipeName { get; set; } = "DefaultAgingRecipe"; // 필요 시 추가

        // [TIMER_COUNT]
        public int TotalProd { get; set; } = 0;
        public int TodayProd { get; set; } = 0;
        public int Shift { get; set; } = 0;

        public int Old1Hour { get; set; } = -1;
        public int OldShiftCase { get; set; } = 1;
        public int ShiftStartHour { get; set; } = 7;
        public int ShiftEndHour { get; set; } = 19;
        public int ProdCnt1Hour { get; set; } = 0;
        public int ProdCnt12Hour { get; set; } = 0;

        // [LOGGING]
        public int LoggingLevel { get; set; } = 0;


        //[PUMP,RF GEM 가동시간 체크]
        public ComponentRunTime[] PumpRunTime { get; private set; }
        public ComponentRunTime[] RfGenRunTime { get; private set; }

        // ============================================================
        // Description : [2-1] 내부 설정 및 모델 데이터
        // ============================================================


        // ============================================================
        // Description : [3] Internal Logic & UI 이벤트 처리
        // ============================================================

        // ============================================================
        // Description : [3-1] UI 이벤트 핸들러
        // ============================================================
        // ...

        // ============================================================
        // Description : [3-2] 내부 동작 및 계산 로직
        // ============================================================

        private string GetProjectName()
        {
            // 실행 어셈블리 이름을 프로젝트명으로 사용
            return System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "DefaultProject";
        }

        public void SetSysPath()
        {
            ExecuteDir = AppContext.BaseDirectory;
            string exeName = Path.GetFileName(ExecuteDir.TrimEnd(Path.DirectorySeparatorChar));
            string projectName = GetProjectName(); // 사용자 정의 함수로 프로젝트명 가져오기

            string basePath;

            // D 드라이브 존재 여부 및 CD-ROM이 아닌지 확인
            bool dDriveExists = DriveInfo.GetDrives().Any(d => d.Name.StartsWith("D:\\"));
            bool dDriveIsFixed = new DriveInfo("D").DriveType == DriveType.Fixed;

            if (dDriveExists && dDriveIsFixed)
            {
                basePath = Path.Combine(@"D:\", projectName);
            }
            else
            {
                basePath = ExecuteDir;
            }

            DataDir = Path.Combine(basePath, "DATA");
            LogDir = Path.Combine(basePath, "LOG");
            ReportDir = Path.Combine(basePath, "REPORT");
            CimDir = Path.Combine(basePath, "CIM");
            ResDir = Path.Combine(ExecuteDir, "RES");

            BcrImgSavePgmDir = Path.Combine(ExecuteDir, "BcrImgSavePgm");
            BcrImgSavePath = Path.Combine(ExecuteDir, "BcrImage");

            CreateFolderIfNotExists(DataDir, "DATA");
            CreateFolderIfNotExists(LogDir, "LOG");
            CreateFolderIfNotExists(ReportDir, "REPORT");
            CreateFolderIfNotExists(CimDir, "CIM");
            CreateFolderIfNotExists(ReportDir, "RES");

        }

        private void CreateFolderIfNotExists(string path, string folderName)
        {

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    //folderName은 디버깅용
                    string name = folderName ?? Path.GetFileName(path);
                    string message = $"{name} folder creation failed: {path}";

                    UtilExtern.ShowInitialMessage(message);
                    throw new Exception(message, ex);

                }
            }
        }


        // ============================================================
        // Description : [4] External Dependencies (외부 연동 / 저장소 요청)
        // ============================================================

        public void LoadValue()
        {
            try
            {
                var ini = new VSIniFile(GlobalFile);

                string section;

                // [RECIPE]
                section = "RECIPE";
                RecipeName = ini.ReadString(section, "RECIPE", RecipeName);
                // AgingRecipeName = ini.ReadString(section, "AGING_RECIPE", AgingRecipeName); // 필요 시 추가

                // [TIMER_COUNT]
                section = "TIMER_COUNT";
                TotalProd = ini.ReadInteger(section, "TOTAL_PRODUCT", TotalProd);
                TodayProd = ini.ReadInteger(section, "TODAY_PRODUCT", TodayProd);

                Shift = ini.ReadInteger(section, "SHIFT", Shift);
                for (int i = 0; i < (int)PM.Max; i++)
                {
                    string pumpKey = $"PUMP_TIME_PM_{i + 1}";
                    string rfKey = $"RF_GEN_TIME_PM_{i + 1}";

                    PumpRunTime[i].LoadCompRunTime(ini.ReadString(section, pumpKey, ""));
                    RfGenRunTime[i].LoadCompRunTime(ini.ReadString(section, rfKey, ""));
                }

                Old1Hour = ini.ReadInteger(section, "CURR_1HOUR", -1);
                OldShiftCase = ini.ReadInteger(section, "SHIFT_CASE", 1);
                ShiftStartHour = ini.ReadInteger(section, "SHIFT_START_HOUR", 7);
                ShiftEndHour = ini.ReadInteger(section, "SHIFT_END_HOUR", 19);
                ProdCnt1Hour = ini.ReadInteger(section, "1HR_PROD_COUNT", 0);
                ProdCnt12Hour = ini.ReadInteger(section, "12HR_PROD_COUNT", 0);

                // [LOGGING]
                section = "LOGGING";
                LoggingLevel = ini.ReadInteger(section, "LOGGING_LEVEL", LoggingLevel);
            }
            catch (Exception ex)
            {
                UtilExtern.ShowInitialMessage($"⚠️ Golbal.INI 파일 로딩 실패: {ex.Message}");
            }
        }

        public void SaveValue()
        {
            Console.WriteLine("Saving system values...");
        }

        // [4-1] 외부 시스템 요청 (DB, API 등)
        // (현재 없음)

        // [4-2] 외부에서 호출되는 진입 함수 (Interop 등)
        // (현재 없음)
    }

}
