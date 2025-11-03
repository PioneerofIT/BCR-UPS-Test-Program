using System;
using VS_MTR;

namespace VS_MTR
{
    public static class Constants
    {
        public const double TURN_TABLE_PITCH = 360.0; // C++의 const 유지
    }

    public enum MotorType : int { CAM5, CFS, CFS2, SMC_2V04 }

    public enum AlarmType : int
    {
        NO_ALARM,
        NOT_HOME,
        AMP_ALARM,
        P_LMT_ON,
        N_LMT_ON,
        EMG_ON
    }

    public enum CommandType : int
    {
        COM_SVRON = 0,
        COM_ALMCLR = 1,
        COM_OUT2,
        COM_OUT3
    }

    public enum SignalType : int
    {
        COM_ORG = 0,
        COM_ZPHASE,
        COM_IN2,
        COM_IN3,
        P_LIMIT,
        N_LIMIT
    }

    public enum EdgeType : byte { DOWN_EDGE, UP_EDGE }
    public enum LevelType { LOW = 0, HIGH }
    public enum EncoderType { NONE = -1, INC = 0, ABS = 1 }
    public enum ServoType { AC_SERVO = 0, EZI_SERVO }

    public class MOTPOS
    {
        public int CurrentPos;
        public int NextPos;
        public double CmdPos;
        public double ActPos;

        // 기본 생성자
        public MOTPOS()
        {
            CurrentPos = 0;
            NextPos = 0;
            CmdPos = 0.0;
            ActPos = 0.0;
        }
    }
}

namespace VSP.CONTROLLER
{
    public class CMotorCtrl
    {
        // ✅ 상수 값 → readonly 필드로 변경
        public readonly MotorType CardType;
        public string LogHead;

        private uint inposlevel;
        private uint alarmlevel;
        private uint NLimitLevel;
        private uint PLimitLevel;
       


        private bool MonEmgEnable; // 2019-09-11 Door Lock 이후에 Emergency 감지

        // ✅ `static` 변수 유지
        public static bool MotorCtrlInit;

        public uint Inposlevel { get => inposlevel; set => inposlevel = value; }
        public uint Alarmlevel { get => alarmlevel; set => alarmlevel = value; }
        public uint Nlimitlevel { get => NLimitLevel; set => NLimitLevel = value; }
        public uint PlimitLevel { get => PLimitLevel; set => PLimitLevel = value; }

        // ✅ 생성자
        public CMotorCtrl(MotorType cardType)
        {
            CardType = cardType;
            MonEmgEnable = false;
        }

        // ✅ `Dispose()` 패턴 활용 가능
        public void Dispose()
        {
            CloseDevice();
        }

        // ✅ 장치 관리 함수 변환
        public virtual bool OpenDevice() => false;
        public virtual void CloseDevice() { }

        // ✅ 초기 설정 관련 메서드
        public virtual bool SetDecelPulseMode(int axis, uint decelStart, uint pulseOut, uint detectSignal) => false;
        public virtual bool SetDriveMode(int axis, uint encMethod, uint stopMode) => false;
        public virtual bool SetInSignals(int axis, uint encMethod, uint inpos, uint alarm, uint nslmt, uint pslmt, uint nlmt, uint plmt, bool encReverse) => false;

        public virtual bool SetServoOnLevel(int axis, uint level) => false;
        public virtual bool SetAlarmOnLevel(int axis, uint level) => false;
        public virtual bool SetAlarmResetLevel(int axis, uint level) => false;
        public virtual bool SetInPosLevel(int axis, uint level) => false;
        public virtual bool SetEmgInputLevel(int axis, uint level) => false;

        public virtual bool SetMovePulsePerUnit(int axis, double pulse) => false;
        public virtual bool SetStartStopSpeed(int axis, double speed) => false;
        public virtual bool SetMaxSpeed(int axis, double max) => false;

        // ✅ I/O 관련 메서드
        public virtual void OutOnOff(int axis, byte bitNo, byte status) { }
        public virtual void ServoEnable(int axis, bool status) { }
        public virtual bool IsServoEnabled(int axis) => false;


        public virtual void AlarmReset(int axis) { }

        public virtual bool IsReady(int axis) => false;
        public virtual bool IsEmergencyOn(int axis) => false;
        public virtual bool IsAlarmOn(int axis) => false;
        public virtual bool IsInpos(int axis) => false;
        public virtual void SetInposWait(int axis, byte wait) { }

        public virtual bool IsPELM(int axis) => false;
        public virtual bool IsNELM(int axis) => false;
        public virtual bool IsORG(int axis) => false;
        public virtual bool IsEdgeOn(int axis, uint inPort) => false;

        public virtual void ToggleIO(int axis, byte outputBit) { }
        public virtual uint GetMechSignal(int axis) => 0x00;

        public bool GetMonEmgEnable() => MonEmgEnable;
        public void SetMonEmgEnable(bool val) => MonEmgEnable = val;

        // ✅ 모션 관련 메서드
        public virtual bool InMotion(int axis) => true;
        public virtual uint GetEndStatus(int axis) => 0x00;

        public virtual bool SetHomeSensorLevel(int axis, byte level) => false;
        public virtual bool SignalSearch1(int axis, double vel, double accel, byte detectSignal, byte edge) => false;
        public virtual bool SignalSearch2(int axis, double vel, byte detectSignal, byte edge) => false;
        public virtual bool StartMotor(int axis, double pos, double vel, double acc) => false;
        public virtual bool StartMove(int axis, double pos, double vel, double acc) => false;
        public virtual bool StartRMove(int axis, double distance, double vel, double acc) => false;
        public virtual bool MoveMotor(int axis, double pos, double vel, double acc) => false;
        public virtual bool JogMove(int axis, double vel, double acc) => false;
        public virtual bool VMove(int axis, double vel, double acc) => false;
        public virtual void SetStop(int axis) { }
        public virtual void SetEStop(int axis) { }
        public virtual bool StartRepeat(int axis, double distance, double vel, double acc, uint wait) => false;
        public virtual void StopRepeat(int axis) { }

        // ✅ 위치 관련 메서드
        public virtual void SetActPos(int axis, double pos) { }
        public virtual void SetCmdPos(int axis, double pos) { }
        public virtual double GetActPos(int axis) => -10.0;
        public virtual double GetCmdPos(int axis) => -10.0;

        // ✅ Scan Edge 관련 메서드
        public virtual void InitScanQ(int axis) { }
        public virtual void SetScriptScan(int axis, int servoType) { }
        public virtual bool GetScriptScanResult(int axis, out uint result)
        {
            result = 0x00;
            return false;
        }
        public byte GetSearchSignalValue(byte signal, byte edge)
        {
            byte byRet = (byte)DETECT_DESTINATION_SIGNAL.PElmPositiveEdge;

            switch (signal)
            {
                case (byte)SignalType.P_LIMIT:
                    byRet = (edge == (byte)EdgeType.UP_EDGE) ? (byte)DETECT_DESTINATION_SIGNAL.PElmPositiveEdge : (byte)DETECT_DESTINATION_SIGNAL.PElmNegativeEdge;
                    break;
                case (byte)SignalType.N_LIMIT:
                    byRet = (edge == (byte)EdgeType.UP_EDGE) ? (byte)DETECT_DESTINATION_SIGNAL.NElmPositiveEdge : (byte)DETECT_DESTINATION_SIGNAL.NElmNegativeEdge;
                    break;
                case (byte)SignalType.COM_ORG:
                    byRet = (edge == (byte)EdgeType.UP_EDGE) ? (byte)DETECT_DESTINATION_SIGNAL.In0UpEdge : (byte)DETECT_DESTINATION_SIGNAL.In0DownEdge;
                    break;
                case (byte)SignalType.COM_ZPHASE:
                    byRet = (edge == (byte)EdgeType.UP_EDGE) ? (byte)DETECT_DESTINATION_SIGNAL.In1UpEdge : (byte)DETECT_DESTINATION_SIGNAL.In1DownEdge;
                    break;
                case (byte)SignalType.COM_IN2:
                    byRet = (edge == (byte)EdgeType.UP_EDGE) ? (byte)DETECT_DESTINATION_SIGNAL.In2UpEdge : (byte)DETECT_DESTINATION_SIGNAL.In2DownEdge;
                    break;
                case (byte)SignalType.COM_IN3:
                    byRet = (edge == (byte)EdgeType.UP_EDGE) ? (byte)DETECT_DESTINATION_SIGNAL.In3UpEdge : (byte)DETECT_DESTINATION_SIGNAL.In3DownEdge;
                    break;
                default:
                    break;
            }

            return byRet;
            
        }
    }
}
