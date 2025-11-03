using System;

using System;
using VS_MTR;
using VSP.CONTROLLER;

namespace VSP.CONTROLLER
{
    public struct VS_HOMING_PARAM
    {
        public uint OrgSensorType;  // DWORD → uint
        public uint SensorLevel;    // DWORD → uint
        public bool UseZPhase;

        public double HomeVel1;
        public double HomeVel2;
        public double HomeAcc;
        public double HomeOffset;
        public double HomeOrgOffset;
        public double OffsetVel;
        public double OffsetAcc;

        // 생성자
        public VS_HOMING_PARAM()
        {
            OrgSensorType = 0;
            SensorLevel = 0;
            UseZPhase = false;

            HomeVel1 = 0.0;
            HomeVel2 = 0.0;
            HomeAcc = 0.0;
            HomeOffset = 0.0;
            HomeOrgOffset = 0.0;
            OffsetVel = 0.0;
            OffsetAcc = 0.0;
        }

        // 초기화 함수
        public void Reset()
        {
            OrgSensorType = 0;
            SensorLevel = 0;
            UseZPhase = false;

            HomeVel1 = 0.0;
            HomeVel2 = 0.0;
            HomeAcc = 0.0;
            HomeOffset = 0.0;
            HomeOrgOffset = 0.0;
            OffsetVel = 0.0;
            OffsetAcc = 0.0;
        }
    }

    public struct VS_HOMING_VAL
    {
        public int OrgStepNo;
        public bool DidHoming; // 작업 도중 홈 검색했는가?
        public bool OrgAbort;
        public bool HomeDoneOk;
        public bool HomeProcEnd;
        public bool Homing;
        public bool HomeFail;

        // 기본 생성자 (모든 필드를 초기화)
        public VS_HOMING_VAL()
        {
            OrgStepNo = 0;
            DidHoming = false;
            OrgAbort = false;
            HomeDoneOk = false;
            HomeProcEnd = false;
            Homing = false;
            HomeFail = false;
        }

        // 모든 값을 기본 상태로 리셋
        public void Reset()
        {
            OrgStepNo = 0;
            DidHoming = false;
            OrgAbort = false;
            HomeDoneOk = false;
            HomeProcEnd = false;
            Homing = false;
            HomeFail = false;
        }
    }

    public struct VS_MOVE_UNIT
    {
        public double Resolution { get; }
        public double GearRate { get; }
        public double LeadPitch { get; }

        // 매개변수 있는 생성자 (C#에서는 반드시 모든 필드를 초기화해야 함)
        public VS_MOVE_UNIT(double resolution, double gearRate, double leadPitch)
        {
            Resolution = resolution;
            GearRate = gearRate;
            LeadPitch = leadPitch;
        }
    }

    public class TMotionUnit
    {
        public double Position { get; set; }
        public double Velocity { get; set; }
        public double Acceleration { get; set; }
        public bool Jog { get; set; }

        // 기본 생성자
        public TMotionUnit() { }

        // 매개변수 생성자
        public TMotionUnit(double pos, double vel, double acc, bool jog = false)
        {
            Position = pos;
            Velocity = vel;
            Acceleration = acc;
            Jog = jog;
        }

        // 복사 메서드 (operator= 대체)
        public void CopyFrom(TMotionUnit arg)
        {
            Position = arg.Position;
            Velocity = arg.Velocity;
            Acceleration = arg.Acceleration;
            Jog = arg.Jog;
        }
    }
    // ==========================================================================
    // Description : CVS_MOTOR Class
    // ==========================================================================

    public class CVS_MOTOR
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle)
        // ============================================================

        // ============================================================
        // Description : [1-1] 생성자
        // ============================================================
        public CVS_MOTOR(int axis, bool encReverse, int servoType, EncoderType encType)
        {
            Axis = axis;
            IsEncReverse = encReverse;
            ServoType = servoType;
            EncType = encType;
        }

        // ============================================================
        // Description : [1-2] 인터페이스 구현 (초기화, 로컬라이징 등)
        // ============================================================
        // ...

        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        
        private string LogHead = "SERVO_NONE";
        // static 연결된 컨트롤러
        private static CMotorCtrl? MotorCtrl = null;
        public static void SetMotorController(CMotorCtrl controller) => MotorCtrl = controller;

        //모터 기본 타입
        public int Axis { get; protected set; }                 //축
        public bool IsEncReverse { get; protected set; }        // 엔코더 방향 반전 여부
        public int ServoType { get; protected set; }            // 서보 타입
        public EncoderType EncType { get; protected set; }              //  -1: No Encoder, Absolute Encoder 0: incremental, 1: absolute

        // Gear rate -----------------------------------------
        public double Resolution { get; protected set; }   // 펄스 해상도
        public double GearRate { get; protected set; }   // 기어비
        public double LeadPitch { get; protected set; }   // 리드 피치 (mm/rev)

        //Initial setting -----------------------------------------
        public double MovePulsePerUnit { get; protected set; }   // 이동 단위당 펄스 수
        public double StartStopSpeed { get; protected set; }   // 시작/정지 속도
        public double MaxSpeed { get; protected set; }   // 최대 속도

        //Servo I/O -----------------------------------------
        private bool PLmtOn;                            //+ limit
        private bool NLmtOn;                            //- limit
        private uint ServoOnLevel;

        // Alarm 관련 ----------------------------------------------
        public bool AlarmReset { get; protected set; }   // 알람 리셋 여부
        public byte AlarmResetLevel { get; protected set; }   // 알람 리셋 레벨
        public uint AlarmMSec { get; protected set; }   // 알람 발생 시간 (ms)

        private bool ReqHome;
        private VS_HOMING_PARAM HomeParam;
        private VS_HOMING_VAL HomingVal;

        // ✅ 폭 관련 변수
        private bool IsWidth;
        private double OrgWidth;

        // ✅ 오버로드 체크
        private bool CheckOverload;

        // Description : Position -----------------------------------------
        public void MatchPos()
        {
            var motorCtrl = CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl();
            double pos = motorCtrl.GetActPos(Axis);
            motorCtrl.SetCmdPos(Axis, pos);
        }

        public void SetActPos(double pos)
        {
            var motorCtrl = CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl();

            motorCtrl.SetActPos(Axis, pos);
        }

        public void SetCmdPos(double pos)
        {
            var motorCtrl = CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl();
            motorCtrl.SetCmdPos(Axis, pos);
        }

        public double GetCurPos()
        {
            double dbRet = (EncType == EncoderType.NONE) ? GetCmdPos() : GetActPos();

            return dbRet;
        }

        // ============================================================
        // Description : [2-1] 내부 설정 및 모델 데이터
        // ============================================================

        // Description	: Unit Conversion  -----------------------------------------
        public double GetMmToPulse(double pos)
        {
            if (LeadPitch == 0.0)
                return 0.0;

            return ((Resolution * pos) / LeadPitch) * GearRate;
        }
        public double GetPulseToMm(double pos)
        {
            if (Resolution == 0.0 || GearRate == 0.0)
                return 0.0;

            return (pos * LeadPitch) / (Resolution * GearRate);
        }

        // ✅ 이동 관련 함수
        public double GetMovePulsePerUnit() => MovePulsePerUnit;
        public double GetStartStopSpeed() => StartStopSpeed;
        public double GetMaxSpeed() => MaxSpeed;

        // ✅ 위치 관련 함수
        public double GetActPos() { return 0; /* 구현 필요 */ }
        public double GetCmdPos() { return 0; /* 구현 필요 */ }
        //public double GetCurPos() => (EncType == EncoderType.NONE ? GetCmdPos() : GetActPos());

        // ✅ I/O 관련 함수 변환
        public void ServoOn(bool status) { /* 서보 모터 ON/OFF 구현 필요 */ }
        public bool IsServoOn() => (ServoType == (int)VS_MTR.ServoType.EZI_SERVO) ? !CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl().IsServoEnabled(Axis) : CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl().IsServoEnabled(Axis);

        public bool IsHomeDoneOk() => HomingVal.HomeDoneOk;

        public bool IsAlarmOn() => CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl().IsAlarmOn(Axis);

        public bool IsPELM() => CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl().IsPELM(Axis);
        public bool IsORG() => CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl().IsORG(Axis);
        public bool IsNELM() => CVSDeviceCtrlManager.Instance.MotionManager.GetMotorCtrl().IsNELM(Axis);   

        public void AlarmResetMethod() { /* 알람 리셋 구현 필요 */ }

        // ✅ 동작 관련 함수 변환
        public bool StartMotor(double pos, double vel, double acc) { return false; }
        public bool StartMove(double pos, double vel, double acc) { return false; }

        // ✅ 폭 관련 함수
        public void SetOrgWidth(double val) => OrgWidth = val;
        public double GetOrgWidth() => OrgWidth;


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
        // 순수 계산 및 상태 제어 포함
        // ...

        // ============================================================
        // Description : [4] External Dependencies (외부 연동 / 저장소 요청)
        // ============================================================

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

    }
}