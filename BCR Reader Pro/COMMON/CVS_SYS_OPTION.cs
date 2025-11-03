using System.Diagnostics;
using System.IO;
using System.IO;
using System.Reflection;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;
using VSP.COMMON;
using VSP.COMMON;
using VSP.COMMON.BASE_COMPONENT;
using VSP.GUI.COMMON;
using VSP.GUI.SETTING.SYSTEM;

namespace VSP.COMMON
{
    struct TPmOption
    {
        public int RfPort { get; set; }
        public int GasName { get; set; }
    }


    public class TMfcOption
    {
        public bool GasUse { get; set; }
        public int MfcCap { get; set; }
        public int GasName { get; set; }
    }

    public class TStripBcrOption //station 
    {
        public int InspecType { get; set; } = 0;
        public int InspecCommType { get; set; } = 0;
        public int EachLaneInspectReaderCount { get; set; } = 1;

        public int[] SerialComPort { get; set; }
        public int[] SubSerialBcrComPort { get; set; }

        public int[] TcpIpPort { get; set; }
        public int[] TcpIpSubPort { get; set; }
        public string[] TcpIpIP { get; set; }
        public string[] TcpIpSubIP { get; set; }

        public TStripBcrOption(int laneCount)
        {
            SerialComPort = new int[laneCount];
            SubSerialBcrComPort = new int[laneCount];
            TcpIpPort = new int[laneCount];
            TcpIpIP = new string[laneCount];
            TcpIpSubPort = new int[laneCount];
            TcpIpSubIP = new string[laneCount];

            for (int i = 0; i < laneCount; i++)
            {
                SerialComPort[i] = 0;
                SubSerialBcrComPort[i] = 0;
                TcpIpPort[i] = 5000;
                TcpIpIP[i] = "127.0.0.1";
                TcpIpSubPort[i] = 500;
                TcpIpSubIP[i] = "127.0.0.2";
            }
        }
    }
    /// <summary>
    /// 옵션 변경 로그를 기록하는 클래스
    /// </summary>
    public class TOptionChangeLog
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public TOptionChangeLog(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    /// <summary>
    /// FFU(팬 필터 유닛) 관련 옵션 설정 클래스
    /// </summary>
    public class TFfuOption
    {
        public bool UseComm { get; set; }       // 통신 사용 여부
        public int MDCnt { get; set; }          // MD 개수
        public int FfuPort { get; set; }        // FFU 통신 포트
        public int SpeedSet { get; set; }       // 속도 설정 (0~100%)

        public TFfuOption()
        {
            UseComm = false;
            MDCnt = 2;
            FfuPort = 0;
            SpeedSet = 100;
        }

        public TFfuOption(bool useComm, int mdCnt, int ffuPort, int speedSet)
        {
            UseComm = useComm;
            MDCnt = mdCnt;
            FfuPort = ffuPort;
            SpeedSet = speedSet;
        }
    }
    // ============================================================
    // Description : class SystemOptionItem 시스템 설정 항목을 표현하는 데이터 구조.
    // ============================================================
    public class SystemOptionItem
    {
        public string Key { get; set; } = "";
        public string Section { get; set; } = "";
        public string Type { get; set; } = "";
        public string Value { get; set; } = "";
        public string Minimum { get; set; } = "";
        public string Maximum { get; set; } = "";
        public string Unit { get; set; } = "";

        public string Description { get; set; } = "";  // 단일 설명 문자열
        public string Hint { get; set; } = "";         // 단일 힌트 문자열

        public string Category { get; set; } = "";
        public string CategoryIndex { get; set; } = "";
        public List<string> CategoryItems { get; set; } = new();  // ENUM 항목 리스트
    }

    /* ==========================================================================
    Description	: SYS OPTION public static hsjangstatic
    ========================================================================== */
    public static class SysOption
    {
        public static CVS_SYS_OPTION Manager => CVS_SYS_OPTION.Instance;
    }
    // ============================================================
    // Description :  SysOption Class Class (Recipe,User,Log 관리)
    // ============================================================
    public sealed class CVS_SYS_OPTION
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle)
        // ============================================================

        private CVS_SYS_OPTION()
        {
            MakeDefault();


            OverwriteTargetExceptValue();
            LoadValue();


            //Save(optionPath);         
        }

        ~CVS_SYS_OPTION()
        {
            string exePath = AppContext.BaseDirectory;
            string optionPath = Path.Combine(exePath, "data", "option.json");
           // Save(optionPath);
            QueryMsgDlg.ShowMsg("System Option Manager 제거됨!");
        }

        public static CVS_SYS_OPTION Instance => instance ??= new CVS_SYS_OPTION();

        public void Initialize()
        {
            //생성자 내부에서 Initialize를 호출하면 안 돼요,
        }

        public void LoadValue()
        {
            // 파일로 부터 읽어오기
            string OptionPath = Path.Combine(CGlobal.Instance.DataDir, CongfigFileTypeNames.OPTJSONFILE);

            if (!File.Exists(OptionPath))
            {
                string OrgPath = Path.Combine(CGlobal.Instance.ExecuteDir, CongfigFileTypeNames.OPTJSONFILE);

                if (File.Exists(OrgPath))
                {
                    UtilExtern.CopyFile(OrgPath, OptionPath);
                }
                else
                {
                    UtilExtern.ShowInitialMessage($"The file does not exist in the source path either. {CongfigFileTypeNames.OPTJSONFILE} ");
                }

            }

            if (File.Exists(OptionPath))
            {
                LoadFromFile(OptionPath);
            }
            else
            {

            }
            LoadAttributeOptionValFromMap(); // 속성 기반 자동 매핑
            LoadCustomOptionValFromMap();     // 구조체 기반 수동 매핑
            ValidateOptionsRange();

            //var options = GetAllOptionValues();
            //foreach (var kvp in options)
            //{
            //    UtilExtern.ShowInitialMessage($"🔍 {kvp.Key} = {kvp.Value}");
            //}
            CompareOptionValuesWithMap();

    }

        public void SaveValue()
        {
            string OptionPath = Path.Combine(CGlobal.Instance.DataDir, CongfigFileTypeNames.OPTJSONFILE);
            var parser = new VSJsonParser();
            SaveCustomOptionToMap();
            parser.OptionMap = this.OptionMap;

            parser.Save(OptionPath);
        }

        public void OverwriteTargetExceptValue()
        {

            string exePath = AppContext.BaseDirectory;
            string BaseOptionPath = Path.Combine(exePath, "data", CongfigFileTypeNames.OPTSYSTEMTEMPJSONFILE); ;
            string OrgOptionPath = Path.Combine(exePath, "data", CongfigFileTypeNames.OPTJSONFILE); ;
            string OptionPath = Path.Combine(CGlobal.Instance.DataDir, CongfigFileTypeNames.OPTJSONFILE);


            if(File.Exists(OrgOptionPath))
            { 
                if (!File.Exists(OptionPath))
                {
                    UtilExtern.CopyFile(OrgOptionPath, CGlobal.Instance.DataDir);
                }

            }
            else
            {
                UtilExtern.ShowInitialMessage($"The file does not exist in the source path either. {CongfigFileTypeNames.OPTSYSTEMTEMPJSONFILE} ");
                return;
            }

            var sourceParser = new VSJsonParser();
            var sourceMap = sourceParser.LoadOptionItems(BaseOptionPath);

            var targetParser = new VSJsonParser();
            var targetMap = targetParser.LoadOptionItems(OptionPath);

            foreach (var (key, sourceItem) in sourceMap)
            {
                if (targetMap.TryGetValue(key, out var targetItem))
                {
                    // 대상에 존재하는 경우: Value는 유지, 나머지는 덮어쓰기
                    targetItem.Section = sourceItem.Section;
                    targetItem.Type = sourceItem.Type;
                    targetItem.Minimum = sourceItem.Minimum;
                    targetItem.Maximum = sourceItem.Maximum;
                    targetItem.Unit = sourceItem.Unit;
                    targetItem.Description = sourceItem.Description;
                    targetItem.Hint = sourceItem.Hint;
                    targetItem.CategoryItems = sourceItem.CategoryItems;
                    targetItem.Category = sourceItem.Category;
                    targetItem.CategoryIndex = sourceItem.CategoryIndex;
                }
                else
                {
                    // 대상에 없는 경우: 원본 그대로 추가
                    targetMap[key] = sourceItem;
                }
            }

            // 덮어쓴 결과를 target.json에 저장
            targetParser.OptionMap = targetMap;
            targetParser.Save(OptionPath);
        }

        public void CompareOptionValuesWithMap()
        {
            var currentValues = GetAllOptionValues();

            int total = currentValues.Count;
            int matched = 0;
            int mismatched = 0;
            int missingInMap = 0;
            int missingInSystem = 0;

            foreach (var kvp in currentValues)
            {
                string key = kvp.Key;
                object currentValue = kvp.Value;

                if (!OptionMap.TryGetValue(key, out var item))
                {
                    UtilExtern.ShowInitialMessage($"⚠️ {key} → OptionMap에 없음 (시스템 값: {currentValue})");
                    missingInMap++;
                    continue;
                }

                string mapValue = item.Value?.ToString() ?? "null";
                string systemValue = currentValue?.ToString() ?? "null";

                if (mapValue != systemValue)
                {
                    UtilExtern.ShowInitialMessage($"🔄 {key} 값 불일치 → 시스템: {systemValue} / 맵: {mapValue}");
                    mismatched++;
                }
                else
                {
                    //UtilExtern.ShowInitialMessage($"✅ {key} 값 일치 → {systemValue}");
                    matched++;
                }
            }

            // OptionMap에 있지만 시스템에 없는 항목도 체크
            var mappedKeys = currentValues.Keys;
            foreach (var key in OptionMap.Keys)
            {
                if (!mappedKeys.Contains(key))
                {
                    UtilExtern.ShowInitialMessage($"🧩 {key} → 시스템에 매핑된 속성 없음 (맵 값: {OptionMap[key].Value})");
                    missingInSystem++;
                }
            }

            // 📊 통계 출력
            UtilExtern.ShowInitialMessage($"📊 총 시스템 옵션 수: {total}");
            UtilExtern.ShowInitialMessage($"✅ 일치 항목 수: {matched}");
            UtilExtern.ShowInitialMessage($"🔄 불일치 항목 수: {mismatched}");
            UtilExtern.ShowInitialMessage($"⚠️ OptionMap에 없는 항목 수: {missingInMap}");
            UtilExtern.ShowInitialMessage($"🧩 시스템에 없는 OptionMap 항목 수: {missingInSystem}");
        }

        public Dictionary<string, object> GetAllOptionValues()
        {
            var result = new Dictionary<string, object>();

            foreach (var prop in GetType().GetProperties())
            {
                var attr = prop.GetCustomAttribute<OptionAttribute>();
                if (attr == null) continue;

                string key = attr.Key;
                object? value = prop.GetValue(this);

                result[key] = value ?? "null";
            }

            return result;
        }


        // ============================================================
        // Description : [1-2] 인터페이스 구현 (초기화, 로컬라이징 등)
        // ============================================================

        // ============================================================  
        // Description : [2] Properties (속성 및 설정 값)  
        // ============================================================  

        private static CVS_SYS_OPTION instance;
        private static bool isInitialized = false;

      


        public string OptionFile { get; set; } = "default.cfg";
        public string LogHead { get; set; } = "SystemLog";

        public bool UseAutoMgzConv { get; set; } = false;
        public int MotionLibType { get; set; } = 1;
        public bool UseServoInterlockRelease { get; set; } = false;

        public bool UseSeparateRecipeSel { get; set; } = false;

        //	CIM Related =============================

        [Option("MACHINE_NO")]
        public string MachineNo { get; set; } = "VSIP_88D_V8";

        public string MachineName { get; set; } = "";

        [Option("CIM_TYPE")]
        public int CimType { get; set; } = 0;

        //	System Option =============================
        [Option("LOGIN_ONCE")]
        public bool LoginOnce { get; set; } = false;

        [Option("LANG")]
        public int Language { get; set; } = (int)LanguageType.ENGLISH;


        [Option("BUZZER_AUTO_OFF_TIME")]
        public int BuzzOffTime { get; set; } = 3;

        [Option("SERVO_SET_ENABLE")]
        public bool ServoSetEnable { get; set; } = false;

        [Option("USE_PASCAL_UNIT")]
        public bool UsePascalUnit { get; set; } = false;

        [Option("USE_GAS_INJECTION_FIRST")]
        public bool GasInjectFirst { get; set; } = false;

        [Option("RESUME_CLEAN_TIME")]
        public bool UseResumeCleanTm { get; set; } = false;

        [Option("SYSTEM_LANE_CNT")]
        public int SysLaneCnt { get; set; } = 4;

        [Option("LOGIN_IDLE_JUDGE_TIME")]
        public int LoginIdleJudgeTime { get; set; } = 5;

        [Option("DOOR_ALARM_SKIP")]
        public bool SkipDoorAlarm { get; set; } = false;

        [Option("RESET_DOOR_ALARM_SKIP_TIME")]
        public int ResetSkipDoorAlarmTime { get; set; } = 30;

        [Option("GAS_COUNT_OVER_3")]
        public bool GasOverThree { get; set; } = false;

        [Option("UseIonizer")]
        public bool UseIonizer { get; set; } = false;

        [Option("USE_IONIZER_ALARM_LOAD")]
        private bool UseIonizerAlarmLoad { get; set; } = false;

        [Option("USE_IONIZER_ALARM_UNLOAD")]
        private bool UseIonizerAlarmUnload { get; set; } = false;

        public bool GetUseIonizerAlarm(PortType port)
        {
            return port switch
            {
                PortType.Load => UseIonizerAlarmLoad,
                PortType.Unload => UseIonizerAlarmUnload,
                _ => false
            };
        }

        public void SetUseIonizerAlarm(PortType port, bool value)
        {
            switch (port)
            {
                case PortType.Load:
                    UseIonizerAlarmLoad = value;
                    break;
                case PortType.Unload:
                    UseIonizerAlarmUnload = value;
                    break;
            }
        }

        [Option("LANE_2ND_WAIT_TIME_SEC")]
        public int Lane2ndWaitTime { get; set; } = 30;

        [Option("USE_AIR_BLOW_SYSTEM")]
        public bool UseAirBlowSystem { get; set; } = false;

        [Option("UNIT_COUNT_IN_CARRIER")]
        public int UnitCount { get; set; } = 21;

        [Option("USE_SUB_SERVO_ON_ALWAYS")]
        public bool UseSubServoOn { get; set; } = false; // 서브 서보 항상 사용 여부

        [Option("SKIP_STOPPER_DOWN_SENSOR")]
        public bool SkipStopperDownSensor { get; set; } = false; // 스토퍼 다운 센서 생략 여부

        [Option("USE_LOAD_RAIL_MIDDLE_SENSOR")]
        public bool UseLoadRailMiddleSensor { get; set; } = false; // 로딩 레일 중간 센서 사용 여부

        [Option("USE_UNLOAD_RAIL_MIDDLE_SENSOR")]
        public bool UseUnloadRailMiddleSensor { get; set; } = false; // 언로딩 레일 중간 센서 사용 여부

        [Option("STRIP_ENTRY_DIRECTION")]
        public int StripEntryDirection { get; set; } = 0; // 스트립 진입 방향 0 = 정방향 , 1은 역방향

        [Option("INDEX_LOADCONV_CANCRASH_WHEN_CHAMBER_UNLOAD_PROC")]
        public bool UnloadProcCanCrashIndexWithLdConv { get; set; } = false; // 언로딩 시 인덱스와 로딩 컨베이어 충돌 여부

        [Option("INSTALL_LOAD_RAIL_CYLINDER_UP_DOWN")]
        public bool InstalledLoadConvCylUpDown { get; set; } = false; // 로딩 레일 실린더 설치 여부

        [Option("INSTALL_UNLOAD_RAIL_CYLINDER_UP_DOWN")]
        public bool InstalledUnldConvCylUpDown { get; set; } = false; // 언로딩 레일 실린더 설치 여부

        [Option("USE_LOAD_RAIL_CYLINDER_UP_DOWN")]
        public bool UseLoadConvCylUpDown { get; set; } = false; // 로딩 레일 실린더 사용 여부

        [Option("USE_UNLOAD_RAIL_CYLINDER_UP_DOWN")]
        public bool UseUnldConvCylUpDown { get; set; } = false; // 언로딩 레일 실린더 사용 여부

        [Option("INSTALL_LOAD_RAIL_LIFT_UP_DOWN")]
        public bool InstalledLoadLiftUpDown { get; set; } = false; // 로딩 리프트 설치 여부

        [Option("INSTALL_UNLOAD_RAIL_LIFT_UP_DOWN")]
        public bool InstalledUnldLiftUpDown { get; set; } = false; // 언로딩 리프트 설치 여부

        [Option("SMEMA_COUNT")]
        public int SmemaCount { get; set; } = 0; // SMEMA 포트 수

        //	PM Related =============================

        [Option("GEN_PORT_1")]
        private int Pm1GenPort { get; set; } = 0; // SMEMA 포트 수

        [Option("GEN_PORT_2")]
        private int Pm2GenPort { get; set; } = 0; // SMEMA 포트 수

        public int GetRfGenPort(PMType type)
        {
            return type switch
            {
                PMType.PM_BTM => Pm1GenPort,
                //PMType.PM_TOP => Pm2GenPort,
                _ => Pm1GenPort
            };
        }

        public void SetRfGenPort(PMType type, int value)
        {
            switch (type)
            {
                case PMType.PM_BTM:
                    Pm1GenPort = value;
                    break;

                // case PMType.PM_TOP:
                //     Pm2GenPort = value;
                //     break;

                default:
                    Pm1GenPort = value; // fallback 처리
                    break;
            }
        }

        [Option("MAT_PORT_1")]
        private int Pm1MatPort { get; set; } = 1; // Matcher 포트 번호

        [Option("MAT_PORT_2")]
        private int Pm2MatPort { get; set; } = 1; // Matcher 포트 번호

        public int GetMatcherPort(PMType type)
        {
            return type switch
            {
                PMType.PM_BTM => Pm1MatPort,
                //PMType.PM_TOP => Pm2MatPort,
                _ => Pm1MatPort // fallback
            };
        }

        public void SetMatcherPort(PMType type, int value)
        {
            switch (type)
            {
                case PMType.PM_BTM:
                    Pm1MatPort = value;
                    break;

                // case PMType.PM_TOP:
                //     Pm2MatPort = value;
                //     break;

                default:
                    Pm1MatPort = value;
                    break;
            }
        }

        //	Rf Generator =============================
       
        [Option("GENERATOR_TYPE")]
        public int RfGenType { get; set; } = (int)RFGenType.PS_RFGEN;

        [Option("GENERATOR_CAPACITY")]
        public int RfGenCap { get; set; } = 1000;

        [Option("GENERATOR_POWER_LIMIT")]
        public int RfGenPwrLmt { get; set; } = 950;

        [Option("GENERATOR_ON_OFFSET")]
        public int RfOnOffsetPressure { get; set; } = 5;

        [Option("RF_FWD_ERROR_VALUE")]
        public int RfFwdErrVal { get; set; } = 30;

        [Option("RF_REF_ERROR_VALUE")]
        public int RfRefErrVal { get; set; } = 50;

        [Option("RF_ERROR_TIME")]
        public int RfErrTime { get; set; } = 5;

        [Option("RF_STABLE_TIME")]
        public int RfStableTime { get; set; } = 3;

        //	Mass Flow Controller =============================

        public TMfcOption[] MfcOpt { get; set; }

        [Option("GAS_ERROR_VALUE")]
        public int GasErrorValue { get; set; } = 5; // 에러 허용값 (% 또는 단위)

        [Option("GAS_ERROR_TIME")]
        public int GasErrorTime { get; set; } = 5; // 에러 지속 시간

        [Option("GAS_STABLE_TIME")]
        public int GasStableTime { get; set; } = 500; // 가스 안정화 시간

        [Option("GAS_ERROR_UNIT")]
        public int GasErrorUnit { get; set; } = 0; // 0: 퍼센트, 1: SCCM

        //	Vacuum & Pump =============================

        [Option("GUAGE_CAPACITY")]
        public int VacGaugeCapacity { get; set; } = 100;

        [Option("VACUUM_ERROR_TIME")]
        public int VacuumErrorTime { get; set; } = 5; // 단위: Sec

        [Option("VACUUM_GUAGE_FAULT")]
        public float VacuumGaugeFaultThreshold { get; set; } = 0.0f;

        [Option("VENT_TIME")]
        public int VentTime { get; set; } = 1000; // 단위: msec

        [Option("USE_VACUUM_COMPANSATION")]
        public bool UseLowVacCompensation { get; set; } = false;

        [Option("PUMP_TYPE")]
        public int VacuumPumpType { get; set; } = 0;

        [Option("PUMP_OIL_CHANGE_TERM")]
        public int OilChangeTerm { get; set; } = 0; // 단위: Hour

        [Option("PUMP_AUTO_OFF_TIME")]
        public int PumpAutoOffTime { get; set; } = 0; // 단위: Sec

        [Option("VAC_STABLE_TIME")]
        public int VacuumStableTime { get; set; } = 3000; // 단위: msec

        [Option("VAC_REACH_ALARM_SP")]
        public float VacuumReachAlarmSP { get; set; } = 0.0f;

        [Option("RF_OFF_WHEN_CHANGING_STEP")]
        public bool UseRfOffWhenChangingStep { get; set; } = false;

        [Option("VAC_GAUGE_VALVE_PROTECT_TIME")]
        public int VacuumGaugeProtectTime { get; set; } = 4000; // 범위 체크 1000 ~ 5000

        [Option("VAC_PURGE_DELAY_TIME")]
        public int VacuumPurgeDelayTime { get; set; } = 0;

        //	Strip BCR =============================
        public TStripBcrOption[] StripBcrOption { get; set; }

        [Option("BCR. SCAN MAX RETRY CNT")]
        public int BcrMaxRetryCnt { get; set; } = 3;

        //	Description	: UNIMOS  2019-11-12 ======================

        [Option("AUTO_LOG_OFF")]
        public int AutoLogOffTime { get; set; } = 0;

        //	Description	: Handler  TCP/IP 통신사용

        [Option("USE_HANDLER_COMM")]
        public int UseHandlerComm { get; set; } = 0;

        [Option("HANDLER_PASSIVE_MODE")]
        public bool HandlerPassiveMode { get; set; } = false;

        [Option("HANDLER_IP")]
        public String HandlerIp { get; set; } = "";

        [Option("HANDLER_PORT")]
        public int HandlerPort { get; set; } = 0;

        [Option("HANDLER_MGZ_SLOT_CNT")]
        public int HandlerMgzSlotCnt { get; set; } = 0;

        // Decription: Barcode Scan Image Save. 2020-10-15 ========

        [Option("USE_BCRIMG_SAVE_FUNC")]
        public bool UseBcrImgSaveFunc { get; set; } = false;

        [Option("BCRIMGPGM")]
        public String BcrImgSavePgm { get; set; } = "XEQT.exe";

        [Option("BCRIMGCONFIG")]
        public String BcrImgConfigName { get; set; } = "SystemInfo.ini";

        // Decription:  2023-06-29 호스트 LAN 끊김 알람 기능 ========

        [Option("USE_HOST_LAN_CONNECT_CHECK")]
        public String UseHostConnectCheck { get; set; } = "";

        [Option("HOST_IP")]
        public String ConnectCheckHostIp { get; set; } = "";

        // Fan Filter Unit Port -----------------------------------------
        public TFfuOption FfuOpt { get; set; } = new TFfuOption();

        // Decription: 20240310 IDLE-STARVED,IDLE-BLOCKED JUDGE TIME 옵션화(COWELL,LGIT)

        [Option("IDLE_STARVED_JUDGE_TIME")]
        public int IdleStarvedJudgeTime { get; set; } = 2;

        [Option("IDLE_BLOCEKD_JUDGE_TIME")]
        public int IdleBlockedJudgeTime { get; set; } = 2;

        // [CONVEYOR]
        [Option("USE_LOAD_CONV_SPEED_ANALOG_CTRL")]
        public bool UseLoadConvSpeedAnalogCtrl { get; set; } = true;

        [Option("USE_UNLD_CONV_SPEED_ANALOG_CTRL")]
        public bool UseUnldConvSpeedAnalogCtrl { get; set; } = true;

        // [LOAD_PUSHER_OPTION]
        [Option("PUSHER_OVERLOAD_MOTOR_BACK_DISTANCE")]
        public float MtrOvlBackMoveDist
        {
            get => _mtrOvlBackMoveDist;
            set => _mtrOvlBackMoveDist = Math.Clamp(value, 0.0f, 30.0f);
        }
        private float _mtrOvlBackMoveDist = 0.0f;

        // [SENSOR_OPTION]
        [Option("USE_CHAMBER_RAIL_HEIGHT_CHECK")]
        public bool UseChamberRailHeightSensor { get; set; } = false;

        // [GUI_OPTION]
        [Option("USE_MULTI_RECIPE_VIEW")]
        public bool UseMultiRecipeView { get; set; } = false;

        // [LAS]
        [Option("PATH")]
        public string LasUpLoadPath { get; set; } = @"D:\EVMS";

        [Option("UPLOAD_TIME_HHMM")]
        public string LasUpdateTime { get; set; } = "0100";

        [Option("MODEL")]
        public string LasModelName { get; set; } = "MODEL_NAME";


        public Dictionary<string, SystemOptionItem> OptionMap { get; set; } = new();


        private readonly VSJsonParser _parser = new VSJsonParser();

        public bool IsInitialized()
        {
            return isInitialized;
        }
        // ============================================================
        // Description : [2-1] 내부 설정 및 모델 데이터
        // ============================================================
        [AttributeUsage(AttributeTargets.Property)]
        public class OptionAttribute : Attribute
        {
            public string Key { get; }
            public OptionAttribute(string key) => Key = key;
        }

        private string GetDefaultValue(Type type)
        {
            if (type == typeof(string)) return "";
            if (type == typeof(int)) return "0";
            if (type == typeof(bool)) return "false";
            if (type == typeof(double)) return "0.0";
            return "";
        }

        private string GetOptionTypeName(Type type)
        {
            if (type == typeof(string)) return "STRING";
            if (type == typeof(int)) return "INTERGER";
            if (type == typeof(bool)) return "BOOLEAN";
            if (type == typeof(double)) return "DOUBLE";
            return "STRING";
        }

        // ============================================================
        // Description : [3] Internal Logic & UI 이벤트 처리
        // ============================================================      
      
        // ============================================================
        // Description : [3-1] UI 이벤트 핸들러
        // ============================================================

        // ============================================================
        // Description : [3-2] 내부 동작 및 계산 로직
        // ============================================================

        public void LoadFromFile(string optionPath)// 현재는 json
        {
            var parser = new VSJsonParser();
            OptionMap = parser.LoadOptionItems(optionPath);

            Debug.WriteLine($"📦 OptionMap 항목 수: {OptionMap.Count}");
        }

        public void LoadAttributeOptionValFromMap()
        {
            int mappedCount = 0;

            foreach (var prop in GetType().GetProperties())
            {
                var attr = prop.GetCustomAttribute<OptionAttribute>();
                if (attr == null) continue;

                string key = attr.Key;

                if (!OptionMap.TryGetValue(key, out var item))
                {
                    Debug.WriteLine($"⚠️ 옵션 누락: {key} → 디폴트 값 유지");
                    continue;
                }

                try
                {
                    object converted = Convert.ChangeType(item.Value, prop.PropertyType);
                    prop.SetValue(this, converted);
                    mappedCount++;

                    Debug.WriteLine($"✅ {key} → {converted}");
                }
                catch
                {
                    Debug.WriteLine($"⚠️ 변환 실패: {key} = {item.Value}");
                }
            }

        }
        public void LoadCustomOptionValFromMap()
        {
            LoadGasSettingFromOptionMap();
            LoadStripBcrSettingFromOptionMap();
            LoadFfuOptionFromOptionMap();
        }

        public void ValidateOptionsRange()
        {
            foreach (var prop in GetType().GetProperties())
            {
                var attr = prop.GetCustomAttribute<OptionAttribute>();
                if (attr == null) continue;

                string key = attr.Key;
                if (!OptionMap.TryGetValue(key, out var item)) continue;

                var type = prop.PropertyType;
                if (!(type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(decimal)))
                    continue;

                try
                {
                    object value = prop.GetValue(this);
                    var comp = value as IComparable;

                    if (!string.IsNullOrWhiteSpace(item.Minimum))
                    {
                        var min = Convert.ChangeType(item.Minimum, type) as IComparable;
                        if (min != null && comp.CompareTo(min) <= 0)
                            Debug.WriteLine($"⛔ {key} ≤ 최소값({item.Minimum}) → 재검토 필요");
                    }

                    if (!string.IsNullOrWhiteSpace(item.Maximum))
                    {
                        var max = Convert.ChangeType(item.Maximum, type) as IComparable;
                        if (max != null && comp.CompareTo(max) >= 0)
                            Debug.WriteLine($"⛔ {key} ≥ 최대값({item.Maximum}) → 재검토 필요");
                    }
                }
                catch
                {
                    Debug.WriteLine($"⚠️ 범위 확인 실패: {key}");
                }
            }
        }

      

        private void LoadGasSettingFromOptionMap()
        {
            MfcOpt = new TMfcOption[(int)MfcType.Max];
            for (int i = 0; i < MfcOpt.Length; i++)
                MfcOpt[i] = new TMfcOption();

            for (int i = 0; i < (int)MfcType.Max; i++)
            {
                string idx = (i + 1).ToString();
                var opt = MfcOpt[i];

                if (OptionMap.ContainsKey($"USE_GAS_{idx}") && int.TryParse(OptionMap[$"USE_GAS_{idx}"].Value, out var useGas))
                    opt.GasUse = useGas != 0;

                if (OptionMap.ContainsKey($"MFC_CAPACITY_{idx}") && int.TryParse(OptionMap[$"MFC_CAPACITY_{idx}"].Value, out var cap))
                    opt.MfcCap = cap;

                if (OptionMap.ContainsKey($"GAS_NAME_{idx}") && int.TryParse(OptionMap[$"GAS_NAME_{idx}"].Value, out var name))
                    opt.GasName = name;

                MfcOpt[i] = opt;
            }

        }

        private void LoadStripBcrSettingFromOptionMap()
        {
            StripBcrOption = new TStripBcrOption[(int)PortType.Max];

            for (int nPort = (int)PortType.Load; nPort < (int)PortType.Max; nPort++)
            {
                StripBcrOption[nPort] = new TStripBcrOption(SysLaneCnt);
            }

            for (int nPort = (int)PortType.Load; nPort < (int)PortType.Max; nPort++)
            {
                string portName = PortTypeNames.LOAD_PORT_NAME[nPort];
                var opt = StripBcrOption[nPort];

                // 통신 타입
                string key = $"INSPECT_COMM_TYPE_{portName}";
                if (OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var InspectCommType))
                    opt.InspecCommType = InspectCommType;
                else
                    opt.InspecCommType = (int)InsepctCommType.ID_READER_SERIAL;

                Debug.WriteLine($"🔧 [{portName}] 통신 타입: {opt.InspecCommType}");

                // 검사 타입
                key = $"INSPECT_TYPE_{portName}";
                if (OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var inspectType))
                    opt.InspecType = inspectType;
                else
                    opt.InspecType = (nPort == (int)PortType.Load) ? (int)InsepctType.READ_ID : (int)InsepctType.NONE;

                Debug.WriteLine($"🔍 [{portName}] 검사 타입: {opt.InspecType}");

                // 리더기 개수
                key = $"2D_READER_COUNT_{portName}";
                if (OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var readerCnt))
                    opt.EachLaneInspectReaderCount = readerCnt;
                else
                    opt.EachLaneInspectReaderCount = 1;

                Debug.WriteLine($"📦 [{portName}] 리더기 개수: {opt.EachLaneInspectReaderCount}");

                for (int nLane = 0; nLane < SysLaneCnt; nLane++)
                {
                    string laneName = LaneTypeNames.LANE_NAME[nLane];

                    if (nLane == (int)LaneType.LANE_1ST)
                    {
                        key = $"2D_BCR_PORT_{portName}";
                        opt.SerialComPort[nLane] = OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var comPort)
                            ? comPort : (int)COMPortType.COM3 + nLane;

                        key = $"2D_SUB_BCR_PORT_{portName}";
                        opt.SubSerialBcrComPort[nLane] = OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var subPort)
                            ? subPort : (int)COMPortType.COM5 + nLane;

                        key = $"2D_BCR_TCPIP_IP_{portName}";
                        opt.TcpIpIP[nLane] = OptionMap.ContainsKey(key) ? OptionMap[key].Value : "127.0.0.1";

                        key = $"2D_BCR_TCPIP_PORT_{portName}";
                        opt.TcpIpPort[nLane] = OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var tcpPort)
                            ? tcpPort : 5000;
                    }
                    else
                    {
                        key = $"2D_BCR_PORT_{portName}_{laneName}";
                        opt.SerialComPort[nLane] = OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var comPort)
                            ? comPort : (int)COMPortType.COM3 + nLane;

                        key = $"2D_SUB_BCR_PORT_{portName}_{laneName}";
                        opt.SubSerialBcrComPort[nLane] = OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var subPort)
                            ? subPort : (int)COMPortType.COM5 + nLane;

                        key = $"2D_BCR_TCPIP_IP_{portName}_{laneName}";
                        opt.TcpIpIP[nLane] = OptionMap.ContainsKey(key) ? OptionMap[key].Value : "127.0.0.1";

                        key = $"2D_BCR_TCPIP_PORT_{portName}_{laneName}";
                        opt.TcpIpPort[nLane] = OptionMap.ContainsKey(key) && int.TryParse(OptionMap[key].Value, out var tcpPort)
                            ? tcpPort : 5000;
                    }

                    Debug.WriteLine($"🔗 [{portName}][{laneName}] SerialPort={opt.SerialComPort[nLane]}, SubSerial={opt.SubSerialBcrComPort[nLane]}, IP={opt.TcpIpIP[nLane]}, Port={opt.TcpIpPort[nLane]}");
                }

                StripBcrOption[nPort] = opt;
            }

            Debug.WriteLine("✅ ApplyStripBcrSettings 완료");
        }

        private void LoadFfuOptionFromOptionMap()
        {
            if (OptionMap.ContainsKey("USE_FFU_COMM") && int.TryParse(OptionMap["USE_FFU_COMM"].Value, out var useComm))
                FfuOpt.UseComm = useComm != 0;

            if (OptionMap.ContainsKey("FFU_MD_COUNT") && int.TryParse(OptionMap["FFU_MD_COUNT"].Value, out var mdCnt))
                FfuOpt.MDCnt = mdCnt;

            if (OptionMap.ContainsKey("FFU_PORT") && int.TryParse(OptionMap["FFU_PORT"].Value, out var port))
                FfuOpt.FfuPort = port;

            if (OptionMap.ContainsKey("FFU_SPEED_SET") && int.TryParse(OptionMap["FFU_SPEED_SET"].Value, out var speed))
                FfuOpt.SpeedSet = speed;

            //Debug.WriteLine($"🔄 FFU 로딩 완료: UseComm={FfuOpt.UseComm}, MDCnt={FfuOpt.MDCnt}, Port={FfuOpt.FfuPort}, Speed={FfuOpt.SpeedSet}");
        }

 
        public void SaveAttributeOptionToMap()
        {
            var props = this.GetType().GetProperties();

            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<OptionAttribute>();
                if (attr == null)
                    continue;

                string key = attr.Key;

                // 이미 존재하면 저장하지 않음
                if (OptionMap.ContainsKey(key))
                    continue;

                object rawValue = prop.GetValue(this);
                string value = rawValue?.ToString() ?? GetDefaultValue(prop.PropertyType);

                var newItem = new SystemOptionItem
                {
                    Key = key,
                    Value = value,
                    Description = "",
                    Category = "SYSTEM.ETC",
                    CategoryIndex = (OptionMap.Count + 1).ToString(),
                    Type = GetOptionTypeName(prop.PropertyType),
                    Hint = "",
                    Unit = ""
                };

                OptionMap[key] = newItem;
            }
        }
        public void SaveCustomOptionToMap()
        {
            SaveGasSettingToOptionMap();
            SaveStripBcrSettingToOptionMap();
            SaveFfuOptionToOptionMap();
        }

        private void SaveGasSettingToOptionMap()
        {
            for (int i = 0; i < MfcOpt.Length; i++)
            {
                string idx = (i + 1).ToString();
                var opt = MfcOpt[i];

                TryAddOptionItem($"USE_GAS_{idx}", opt.GasUse ? "1" : "0", "BOOLEAN", "UNKNOWN");
                TryAddOptionItem($"MFC_CAPACITY_{idx}", opt.MfcCap.ToString(), "INT", "UNKNOWN");
                TryAddOptionItem($"GAS_NAME_{idx}", opt.GasName.ToString(), "STRING", "UNKNOWN");
            }
        }

        private void SaveStripBcrSettingToOptionMap()
        {
            for (int nPort = (int)PortType.Load; nPort < (int)PortType.Max; nPort++)
            {
                string portName = PortTypeNames.LOAD_PORT_NAME[nPort];
                var opt = StripBcrOption[nPort];

                TryAddOptionItem($"INSPECT_COMM_TYPE_{portName}", opt.InspecCommType.ToString(), "INT", "UNKNOWN");
                TryAddOptionItem($"INSPECT_TYPE_{portName}", opt.InspecType.ToString(), "INT", "UNKNOWN");
                TryAddOptionItem($"2D_READER_COUNT_{portName}", opt.EachLaneInspectReaderCount.ToString(), "INT", "UNKNOWN");

                for (int nLane = 0; nLane < SysLaneCnt; nLane++)
                {
                    string laneName = LaneTypeNames.LANE_NAME[nLane];
                    string suffix = (nLane == (int)LaneType.LANE_1ST) ? "" : $"_{laneName}";

                    TryAddOptionItem($"2D_BCR_PORT_{portName}{suffix}", opt.SerialComPort[nLane].ToString(), "INT", "UNKNOWN");
                    TryAddOptionItem($"2D_SUB_BCR_PORT_{portName}{suffix}", opt.SubSerialBcrComPort[nLane].ToString(), "INT", "UNKNOWN");
                    TryAddOptionItem($"2D_BCR_TCPIP_IP_{portName}{suffix}", opt.TcpIpIP[nLane], "STRING", "UNKNOWN");
                    TryAddOptionItem($"2D_BCR_TCPIP_PORT_{portName}{suffix}", opt.TcpIpPort[nLane].ToString(), "INT", "UNKNOWN");
                }
            }
        }
        private void SaveFfuOptionToOptionMap()
        {
            TryAddOptionItem("USE_FFU_COMM", FfuOpt.UseComm ? "1" : "0", "BOOLEAN", "UNKNOWN");
            TryAddOptionItem("FFU_MD_COUNT", FfuOpt.MDCnt.ToString(), "INT", "UNKNOWN");
            TryAddOptionItem("FFU_PORT", FfuOpt.FfuPort.ToString(), "INT", "UNKNOWN");
            TryAddOptionItem("FFU_SPEED_SET", FfuOpt.SpeedSet.ToString(), "INT", "UNKNOWN");
        }

        private void TryAddOptionItem(string key, string value, string type, string category)
        {
            if (!OptionMap.ContainsKey(key))
            {
                OptionMap[key] = new SystemOptionItem
                {
                    Key = key,
                    Value = value,
                    Type = type,
                    Category = category,
                    CategoryIndex = (OptionMap.Count + 1).ToString()
                };
            }
        }




        // ============================================================
        // Description : [4] External Dependencies (외부 연동 / 저장소 요청)
        // ============================================================
        public bool ValidateSystemOptions()
        {
            return !string.IsNullOrEmpty(OptionFile) && !string.IsNullOrEmpty(LogHead);
        }
        // ============================================================
        // Description : [4-1] 외부 시스템 요청 (DB, API 등)
        // ============================================================
        // ...

        // ============================================================
        // Description : [4-2] 외부에서 호출되는 진입 함수 (Interop 등)
        // ============================================================
        // ...

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        // 현재는 정리되지 않은 부분 (추후 확장 가능)


        // ============================================================  
        // Description : [4] Internal Logic / Validation (데이터 검증)  
        // ============================================================  



        // ============================================================  
        // Description : [5] Unclassified (추후 정리 예정)  
        // ============================================================  

        public void MakeDefault()
        {
            //OptionFile = "default.cfg";
            //LogHead = "SystemLog";
            //MfcOpt[0].GasUse = true;
            //MfcOpt[1].GasUse = true;
            //MfcOpt[0].MfcCap = 50;
            //MfcOpt[1].MfcCap = 100;
        }

    }
}
