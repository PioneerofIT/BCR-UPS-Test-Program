using System;
using System.Collections.Generic;
using System.Windows.Controls;
using VS_MTR;
using VSP.CONTROLLER.AJIN;

namespace VSP.CONTROLLER
{
    // ==========================================================================
    // Description : Servo Motor define
    // ==========================================================================
    public enum ServoMotor
    {
        Sm00_LoadShuttle_Y = 0,      // 로딩 셔틀 Y
        Sm01_UnldShuttle_Y,          // 언로딩 셔틀 Y
        Sm02_Pusher_X,               // 푸셔 X
        Sm03_Pusher_Slave_X,         // 푸셔 X 서브
        MAX_SERVO_AXIS,
        NONE = -1
    }

    public class CMotionManager : IDisposable
    {
        private bool IsInit;
        private CMotorCtrl MotorCtrl;
        private Dictionary<int, CVS_MOTOR> Servo = new Dictionary<int, CVS_MOTOR>();

        // ✅ 모터 객체를 참조 타입으로 변경
        private CVS_MOTOR LoadRail_Y;
        private CVS_MOTOR Pusher_X;
        private CVS_MOTOR UnldRail_Y;

        // ✅ 생성자
        public CMotionManager()
        {
            CreateMotorCtrl();
            CreateMotorObjects();
        }

        // ✅ Dispose 패턴 적용
        public void Dispose()
        {
            CloseDevice();
        }

        // ✅ 장치 관리 함수 변환
        public void OpenDevice() { /* 구현 필요 */ }
        public void CloseDevice() { /* 구현 필요 */ }

        // ✅ 모터 객체 생성 및 삭제
        public void CreateMotorObjects()
        {
            LoadRail_Y = new((int)ServoMotor.Sm00_LoadShuttle_Y, true, (int)ServoType.AC_SERVO, (int)EncoderType.INC);
            Servo.Add((int)ServoMotor.Sm00_LoadShuttle_Y, LoadRail_Y);

            Pusher_X = new((int)ServoMotor.Sm02_Pusher_X, true, (int)ServoType.AC_SERVO, (int)EncoderType.INC);
            Servo.Add((int)ServoMotor.Sm02_Pusher_X, LoadRail_Y);

            UnldRail_Y = new((int)ServoMotor.Sm01_UnldShuttle_Y, true, (int)ServoType.AC_SERVO, (int)EncoderType.INC);
            Servo.Add((int)ServoMotor.Sm01_UnldShuttle_Y, UnldRail_Y);
        }
        public void DeleteMotorObjects() { /* 구현 필요 */ }

        public void CreateMotorCtrl()
        {
            MotorCtrl = new CAxtMotorCtrl(MotorType.CFS2);

            CVS_MOTOR.SetMotorController(MotorCtrl); // 정적 메서드 호출
        }
        public void DeleteMotorCtrl() { MotorCtrl = null; }

        // ✅ 모터 컨트롤러 반환
        public CMotorCtrl GetMotorCtrl() => MotorCtrl;

        // ✅ 모터 개별 제어 함수
        public CVS_MOTOR GetMtr(int nMtrIdx) => Servo.ContainsKey(nMtrIdx) ? Servo[nMtrIdx] : null;
        public void SetMtrSvrOn(int nMtrIdx, bool bOn) { /* 구현 필요 */ }
        public bool IsDriverAlarm(int nMtrIdx) => false;
        public void AlarmClear(int nMtrIdx) { /* 구현 필요 */ }

        // ✅ 모든 모터 제어 함수
        public bool AnyOneIsInMotion() => false;
        public void StopAllTheMotors() { /* 구현 필요 */ }
        public bool IsSvrHomeDoneAll() => false;
        public void SetServoHomeReset(int nMtrIdx) { /* 구현 필요 */ }
        public void SetAllSvrHomeReset() { /* 구현 필요 */ }

        // ✅ 모터 개수 반환 (`std::map.size()` 대체)
        public int GetServoMtrCnt() => Servo.Count;

        // ✅ 모터 오프셋 관련 함수
        public double LoadAbsOffsets(int nMtrIdx) => 0.0;
        public void SaveAbsOffsets(int nMtrIdx, double dbOffset) { /* 구현 필요 */ }

        // ✅ 인터페이스 변환
        public bool IsMotionVerticalAxis(ServoMotor nMtrIdx)
        {
            if(nMtrIdx == ServoMotor.Sm00_LoadShuttle_Y || nMtrIdx == ServoMotor.Sm01_UnldShuttle_Y)
                return true;
            else return false;
        }
 
        public bool IsMotionScanEdgeAxis(int nMtrIdx) => false;
        public bool IsMotionReverseViewAxis(int nMtrIdx) => false;
    }
}