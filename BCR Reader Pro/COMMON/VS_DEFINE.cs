using System;
namespace VSP.COMMON
{
    public static class VSMessage
    {
        public const int PROC_MSG = 1000;
        public const int JOB_MSG = 1001;
        public const int ALARM_MSG = 1002;
        public const int COMM_MSG = 1003;
        public const int RESET_MSG = 1004;
        public const int ATK_PLS = 1005;
        public const int MGZSCAN_MSG = 1006;
        public const int CIM_MSG = 1007;
        public const int MENU_MSG = 1008;
        public const int UPDATE_SLOT = 1009;
        public const int UPDATE_RCP_LIST = 1010;
        public const int UPDATE_RCP_SEL = 1011;
        public const int UPDATE_RCP_CHANGE = 1012;
        public const int BCR_READ = 1013;
        public const int STATION_CHANGE = 1014;
        public const int UPDATE_DEVICE_ID = 1015;
        public const int PBS_MSG = 1016;
        public const int LOGIN_MSG = 1017;
        public const int FORM_RELOAD = 1018;
        public const int SHOW_KEYBOARD = 1019;
        public const int GEM_SEND_EVENT = 1020;
        public const int GEM_SEND_ALARM = 1021;
        public const int MATERIALINFO_DLG_MSG = 1023;
        public const int LOT_STATE_CHANGE = 1024;
        public const int TEST_MSG = 1025;
        public const int CHAMBER_PM_MSG = 1026;
        public const int CIM_SUNWODA_DLG_MSG = 1027;
        public const int STOP_REPORT_MSG = 1028;
        public const int COWELL_CIM_MODE_MSG = 1029;
        public const int DOORINTERLOCK_DLG_MSG = 1045;
        public const int MODE_CHANGE_DLG_MSG = 1046;
        public const int DRY_RUN_STATE = 1051;
    }

    public class PacketType
    {
        public List<byte> Data { get; set; }

        public PacketType()
        {
            Data = new List<byte>();
        }
    }
    // ============================================================  
    // Description : OPTION
    // ============================================================ 
    public static class CongfigFileTypeNames
    {
        public const string SYSINIFILE = "GLOBAL.INI";
        public const string OPTINIFILE = "OPTION.INI";
        public const string OPTJSONFILE = "OPTION.JSON";
        public const string OPTSYSTEMTEMPJSONFILE = "SystemOptionTemplate.JSON";
        public const string RSVINIFILE = "RESERVE.INI";
        public const string LEGENDINIFILE = "LEGEND.INI";
        public const string TOWERINIFILE = "TOWER.INI";
        public const string LANEINIFILE = "LANE.INI";
        public const string PASSLEVELINIFILE = "PASSLEVEL.INI";
        public const string VSPDB = "VSP.DB";
        public const string VSPMDB = "vsp.MDB";
        public const string ERRDB = "ErrDefDb.DB";

    }

    // ============================================================  
    // Description : COMMUNICATION
    // ============================================================ 
    public static class CommunicationConstants
    {
        public const char SOH = '\x01';
        public const char STX = '\x02';
        public const char BEL = '\x07';
        public const char HT = '\x09';
        public const char CR = '\x0D'; // ♪
        public const char LF = '\x0A';

        public const int COMM_TIMEOUT = 3000;
        public const int VS_PACK_LEN_MAX = 256;
    }

    public enum COMPortType
    {
        COM1 = 0, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, COM10,
        COM11, COM12, COM13, COM14, COM15, COM16, COM17, COM18, COM19, COM20, COM_PORT_MAX
    }

    public static class COMPortTypeNames 
    {
        public static readonly string[] NAME =
        {
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10",
            "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19", "COM20"
        }; 

    }
    //}
    // ============================================================  
    // Description : LANGUAGE
    // ============================================================ 
    public enum LanguageType
    {
        KOREAN = 0, ENGLISH, CHINA_SIM, CHINA_TRAD, VIETNAM, Max
    }
    public static class LanguageNameType
    {
        public static readonly string[] Name = { "KOREAN", "ENGLISH", "CHINA_SIM", "CHINA_TRAD", "VIETNAM"};
    }
    // ============================================================  
    // Description : RECIPE
    // ============================================================ 
    public enum RecipeType 
    { 
        CleanParam, MotionParam, RcpParam, Max 
    }
    public enum RecipeSelectionType
    { 
        Local, Remote, Aging, Sleep 
    }
    public enum CleanStepType 
    { 
        STEP1 = 0, STEP2, STEP3, STEP4, STEP5, Max 
    }

    // ============================================================  
    // Description : GENERATOR
    // ============================================================ 
    public enum RFGenType
    {
        PS_RFGEN = 0, YS_RFGEN, PTS_RFGEN, AIO_RFGEN, Max
    }

    public enum GenMode
    {
        LOC_MODE = 0,
        REM_MODE,
        ANA_MODE
    }
    // ============================================================  
    // Description : PUMP
    // ============================================================  
    public enum OilPumpType
    {
        OIL_SENSOR_PUMP = 0, OIL_ONLY_PUMP, DRY_SIGNAL_PUMP, DRY_ONLY_PUMP, Max
    }
    // ============================================================  
    // Description : MFC
    // ============================================================  
    public enum MfcType { MFC_1 = 0, MFC_2, MFC_3, MFC_4, Max }
    
    public enum GasType { GAS_TYPE_Ar = 0, GAS_TYPE_O2, GAS_TYPE_H2, GAS_TYPE_N2, GAS_TYPE_ArH2, GAS_TYPE_ArO2, GAS_TYPE_CDA, Max }

    public static class GasNameType
    {
        public static readonly string[] MFC_GAS_NAME = { "Ar", "O2", "H2", "N2", "Ar+H2", "Ar+O2", "CDA" };
    }
    // ============================================================  
    // Description : CIM
    // ============================================================  
    public enum CIMType : int
    {
        CIM_NONE = 0, CIM_GEM, CIM_LGIT, CIM_SUNWODA, CIM_COWELL, CIM_NVT, Max
    }

    public static class CIMConstants
    {
        public static readonly string[] CIM_NAME = { "NONE", "GEM", "GEM_LGIT", "CIM_SUNWODA", "CIM_COWELL", "CIM_NVT" };
    }

    // ============================================================  
    // Description : 모델별 하드웨어 Section OPTION
    // ============================================================  
    public enum DoorPosition
    {
        Front, Rear, Max
    }

    public static class DoorNames
    {
        public static readonly string[] Name = { "DOOR_FRONT", "DOOR_REAR" };
    }

    public enum DoorType
    {
        None, Magnetic, Lock, Max
    }

    public static class DoorTypeNames
    {
        public static readonly string[] Name = { "DOOR_NONE", "DOOR_MAGNETIC", "DOOR_LOCK" };
    }

    public enum PM
    {
        PM_1,  // PM_1: BOTTOM  
               // PM_2 (주석 처리)  
        Max
    }

    public enum PMType
    {
        PM_BTM,  // PM_1: BOTTOM  
                 // PM_TOP (주석 처리)  
        Max
    }

    public static class PMInfo
    {
        public static readonly string[] PM_NAME = { "PM_BTM" /*, "PM_TOP"*/ };
    }

    public enum StationType
    {
        LoadShuttle, LoadStation, PmStation, UnldStation, UnldShuttle, Max
    }

    public static class StationNames
    {
        public static readonly string[] Name = { "LOAD_SHUTTLE", "LOAD_RAIL", "CHAMBER", "UNLOAD_RAIL", "UNLOAD_SHUTTLE" };
    }

    public enum LayerType
    {
        LAYER_BTM = 0, LAYER_TOP, Max
    }
    public static class LayerConstants
    {
        public static readonly string[] LAYER_NAME = { "BTM" };
    }

    public enum PortType
    {
        Load = 0, Unload, Max
    }

    public static class PortTypeNames
    {
        public static readonly string[] LOAD_PORT_NAME = { "LOADING", "UNLOADING" };
    }

    public enum LaneType
    {
        LANE_1ST = 0, LANE_2ND, LANE_3RD, LANE_4TH, LANE_5TH, LANE_6TH, Max
    }

    public static class LaneTypeNames
    {
        public static readonly string[] LANE_NAME = { "1LANE", "2LANE", "3LANE", "4LANE", "5LANE", "6LANE" };
    }

    public enum  InsepctCommType
    {
        ID_READER_SERIAL = 0, ID_READER_TCPIP, Max
    }

    public enum InsepctType
    {
        NONE, READ_ID ,DIRECTION, MAX
    }
    // ============================================================  
    // Description : Strip ID
    // ============================================================  

    public struct TSerialCommSet
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int CommPort { get; set; }

        public string SubIp { get; set; }
        public int SubPort { get; set; }
        public int SubCommPort { get; set; }

        public TSerialCommSet(string ip = "127.0.0.1", int port = 5000, int commPort = 0,
                              string subIp = "127.0.0.1", int subPort = 5000, int subCommPort = 0)
        {
            Ip = ip;
            Port = port;
            CommPort = commPort;
            SubIp = subIp;
            SubPort = subPort;
            SubCommPort = subCommPort;
        }

        public void CopyFrom(TSerialCommSet other)
        {
            Ip = other.Ip;
            Port = other.Port;
            CommPort = other.CommPort;
            SubIp = other.SubIp;
            SubPort = other.SubPort;
            SubCommPort = other.SubCommPort;
        }

        public override bool Equals(object obj)
        {
            if (obj is TSerialCommSet other)
                return Ip == other.Ip && Port == other.Port && CommPort == other.CommPort &&
                       SubIp == other.SubIp && SubPort == other.SubPort && SubCommPort == other.SubCommPort;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Ip, Port, CommPort, SubIp, SubPort, SubCommPort);
        }

        public static bool operator ==(TSerialCommSet left, TSerialCommSet right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TSerialCommSet left, TSerialCommSet right)
        {
            return !left.Equals(right);
        }
    }

    public struct TTcpIpSetInfo
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int RetryTerm { get; set; }

        public TTcpIpSetInfo(string ip = "127.0.0.1", int port = 5000, int retryTerm = 0)
        {
            Ip = ip;
            Port = port;
            RetryTerm = retryTerm;
        }

        public void CopyFrom(TTcpIpSetInfo other)
        {
            Ip = other.Ip;
            Port = other.Port;
            RetryTerm = other.RetryTerm;
        }

        public override bool Equals(object obj)
        {
            if (obj is TTcpIpSetInfo other)
                return Ip == other.Ip && Port == other.Port && RetryTerm == other.RetryTerm;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Ip, Port, RetryTerm);
        }

        public static bool operator ==(TTcpIpSetInfo left, TTcpIpSetInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TTcpIpSetInfo left, TTcpIpSetInfo right)
        {
            return !left.Equals(right);
        }
    }

    public struct TSerialCommInfo
    {
        public int Port { get; set; }
        public int RetryTerm { get; set; }

        public TSerialCommInfo(int port = 3, int retryTerm = 0)
        {
            Port = port;
            RetryTerm = retryTerm;
        }

        public void CopyFrom(TSerialCommInfo other)
        {
            Port = other.Port;
            RetryTerm = other.RetryTerm;
        }

        public override bool Equals(object obj)
        {
            if (obj is TSerialCommInfo other)
                return Port == other.Port && RetryTerm == other.RetryTerm;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Port, RetryTerm);
        }

        public static bool operator ==(TSerialCommInfo left, TSerialCommInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TSerialCommInfo left, TSerialCommInfo right)
        {
            return !left.Equals(right);
        }
    }


    // ============================================================  
    // Description : STATISTICS  
    // ============================================================  
    public enum LOG_KIND
    {
        LOG_ALARM = 0,
        LOG_RUNTIME,
        LOG_MAINT,
        LOG_PRODUCT,
        LOG_CLEANING,
        SHIFT_PRODUCT,
        LOG_GUI,
        LOG_SEQ,
        MAX_LOG_KIND
    }

    public static class LogConstants
    {
        public static readonly string[] LOG_KIND_STR =
        {
        "LOG_ALARM",
        "LOG_RUNTIME",
        "LOG_MAINT",
        "LOG_PRODUCT",
        "LOG_CLEANING",
        "SHIFT_PRODUCT",
        "LOG_GUI",
        "LOG_SEQ"
    };

        public const int MAX_LOG_CNT = 500;
    }
    // ============================================================  
    // Description : 미분류
    // ============================================================  

    public enum PMRunMode
    {
        NORMAL_RUN = 0, MANUAL_RUN, AGING_RUN, SLEEP_RUN, LEAK_CHECK, PM_RUN_MAX
    }

    public static class PMRunConstants
    {
        public static readonly string[] PM_RUN_MODESTR = { "NORMAL", "MANUAL", "AGING", "SLEEP", "LEAK CHECK" };
    }

    public enum DeviceState
    {
        EMPTY = 0, BEFORE, PROC_START, WORKING, DONE, PROC_COMP, Max
    }

    public static class DeviceStateConstants
    {
        public static readonly string[] DEVICE_STATE_STR = { "EMPTY", "BEFORE", "PROC_START", "WORKING", "DONE", "PROC_COMP" };
    }

    //namespace VSP.COMMON.RECIPE_PARAM
    public static class ServoConstants
    {
        //VS_MOTION_PARAM
        public const int MAX_SERVO_POS = 12;
        public const int SERVO_MAX_LIMIT_POS = 10;
        public const int SERVO_MIN_LIMIT_POS = 11;
        //VS_LANE_OTHER_PARAM
        public const int MGZ_SLOT_MAX = 40;
        public const int MAX_CNT_TMR = 10;

        public static readonly string[] SLOT_KIND_NAMES =
        {
            "SLOT_NORM",
            "SLOT_ODD",
            "SLOT_EVEN"
        };

        public static readonly string[] STRIP_LOAD_TYPE_NAMES =
        {
            "WAIT AT GRIP. POS",
            "GRIP AFTER PUSH"
        };

        public static readonly string[] BOAT_TYPE_NAMES =
        {
            "SINGLE BOAT MATRIX",
            "DUAL BOAT MATRIX"
        };



    }
    //VS_LANE_OTHER_PARAM
    public enum RecipeTimeout
    {
        LOAD_EMPTY_TIMEOUT = 0,
        BA_FROM_UP_TIMEOUT,
        MR_FROM_DOWN_TIMEOUT,
        DEVICE_ARRIVAL_TIMEOUT,
        RECIPE_TC_MAX
    }

    public enum ConveyorVelocity
    {
        LOAD_SHUTTLE_CONV_VELOCITY = 0,
        LOAD_STATION_CONV_VELOCITY,
        UNLD_STATION_CONV_VELOCITY,
        UNLD_SHUTTLE_CONV_VELOCITY,
        CONV_VELOCITY_MAX
    }
    public enum DeviceType
    {
        DEVICE_SMALL = 0,  // WAFER_SIZE_6
        DEVICE_LARGE       // WAFER_SIZE_8
    }
    public enum RecipeItem
    {
        USE_ID_READ_LOAD = 0,
        USE_ID_READ_UNLOAD,
        PLASMA_PROCESS_SKIP,
        RECIPE_ITEM_MAX
    }

    public enum SlotKind
    {
        SLOT_NORM = 0,
        SLOT_ODD,
        SLOT_EVEN,
        SLOT_JOB_MAX
    }

    // 미사용 주석 처리된 부분
    // public enum UnloadType
    // {
    //     BTM_FIRST = 0,
    //     TOP_FIRST,
    //     SLOT_MATCHING,
    //     UNLD_TYPE_MAX
    // }
    // public static readonly string[] UNLD_TYPE_NAMES =
    // {
    //     "BTM -> TOP",
    //     "TOP -> BTM",
    //     "SLOT MATCHING"
    // };

    public enum StripLoadType
    {
        WAIT_AT_GRIP_POS = 0,
        GRIP_AFTER_PUSH,
        STRIP_LOAD_TYPE_MAX
    }

    public enum BoatType
    {
        SINGLE_BOAT_MATRIX = 0,
        DUAL_BOAT_MATRIX,
        BOAT_TYPE_MAX
    }
    
    // ============================================================  
    // Description : LOGIN  
    // ============================================================  
    public enum LOGIN_LEVEL
    {
        OPERATOR,
        MANAGER,
        SUPERVISOR,
        LOG_OUT,
        SYS_MAKER,
        Max
    }

    public static class LoginConstants
    {
        public static readonly string[] LOGIN_TEXT =
        {
        "Operator",
        "Manager",
        "Supervisor",
        "Log Out",
        "Sys Maker"
         };

    }
    public enum CARD_TYPE : int//0 : CAM5 , 1: CFS, 2 : CFS2, 3 : SMC_2V04
    {
        CAM5 = 0,
        CFS = 1,
        CFS2 = 2,
        SMC_2V04 = 3
    }
}

