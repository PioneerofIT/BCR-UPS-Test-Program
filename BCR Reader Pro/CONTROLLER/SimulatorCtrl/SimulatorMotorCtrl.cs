using VSP.CONTROLLER;
using VSP.CONTROLLER.SimulatorCtrl.Objects;

namespace VSP.CONTROLLER.SimulatorCtrl
{
    public class CSimulatorMotorCtrl : CMotorCtrl
    {        
        private MotorObject[] MotorParam;

        public CSimulatorMotorCtrl() : base(0)
        {
            MotorParam = new MotorObject[(int)ServoMotor.MAX_SERVO_AXIS];
            for (int i = 0; i < (int)ServoMotor.MAX_SERVO_AXIS; i++)
            {
                MotorParam[i] = new MotorObject();
            }
        }

        // I/O 관련
        public override void ServoEnable(int nAxis, bool bStatus)
        {
            MotorParam[nAxis].ServoOn = bStatus;
        }

        public override bool IsServoEnabled(int nAxis)
        {
            return MotorParam[nAxis].ServoOn;
        }

        public override bool IsInpos(int nAxis)
        {
            return true;
        }

        // 모션 관련
        public override bool InMotion(int nAxis)
        {
            return MotorParam[nAxis].InMotion;
        }

        //public override bool HomeInMotion(int nAxis)
        //{
        //    return MotorParam[nAxis].HomeInMotion;
        //}

        public override bool SignalSearch1(int nAxis, double dVel, double dAccel, byte detect_signal, byte byEdge)
        {
            return true;
        }

        public override bool SignalSearch2(int nAxis, double dVel, byte detect_signal, byte byEdge)
        {
            return true;
        }

        public override bool StartMotor(int nAxis, double dPos, double dVel, double dAcc)
        {
            MotorParam[nAxis].TargetPosition = dPos;
            MotorParam[nAxis].GenerateVirtualPos(dVel, dAcc);
            return true;
        }

        public override bool StartMove(int nAxis, double dPos, double dVel, double dAcc)
        {
            MotorParam[nAxis].TargetPosition = dPos;
            MotorParam[nAxis].GenerateVirtualPos(dVel, dAcc);
            return true;
        }

        public override bool StartRMove(int nAxis, double dDistance, double dVel, double dAcc)
        {
            MotorParam[nAxis].CurrentPosition += dDistance;
            return true;
        }

        // 위치 설정/조회
        public override void SetActPos(int nAxis, double pos)
        {
            // 구현 생략됨 (시뮬레이터에서는 필요시 구현)
        }

        public override void SetCmdPos(int nAxis, double pos)
        {
            // 구현 생략됨
        }

        public override double GetActPos(int nAxis)
        {
            return MotorParam[nAxis].CurrentPosition;
        }

        public override double GetCmdPos(int nAxis)
        {
            return MotorParam[nAxis].CurrentPosition;
        }
    }
}
