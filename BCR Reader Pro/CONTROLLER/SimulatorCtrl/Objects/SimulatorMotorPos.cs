using VSP.CONTROLLER.SimulatorCtrl.Objects;

namespace VSP.CONTROLLER.SimulatorCtrl.Objects
{
    public class MotorMoveThread
    {
        private readonly MotorObject _motor;
        private readonly double _targetVel;
        private readonly double _acc;
        private const int DelayMs = 10;
        private double _stepSize;
        private volatile bool _terminated = false;

        public MotorMoveThread(MotorObject motor, double velocity, double acceleration)
        {
            _motor = motor;
            _targetVel = velocity;
            _acc = acceleration;
        }

        public void Terminate()
        {
            _terminated = true;
        }

        public void Run()
        {
            double distance = Math.Abs(_motor.TargetPosition - _motor.CurrentPosition);
            if (distance <= 0.0)
            {
                _motor.InMotion = false;
                _motor.HomeInMotion = false;
                return;
            }

            double t_acc = _targetVel / _acc;
            double s_acc = 0.5 * _acc * t_acc * t_acc;

            double totalTime;
            if (2 * s_acc >= distance)
            {
                // Triangular velocity profile
                totalTime = 2 * Math.Sqrt(distance / _acc);
            }
            else
            {
                // Trapezoidal velocity profile
                double s_const = distance - 2 * s_acc;
                double t_const = s_const / _targetVel;
                totalTime = 2 * t_acc + t_const;
            }

            int steps = (int)(totalTime * 1000 / DelayMs);
            if (steps <= 0) steps = 1;

            _stepSize = distance / steps;

            int dir = (_motor.TargetPosition > _motor.CurrentPosition) ? 1 : -1;

            for (int i = 0; i < steps && !_terminated; i++)
            {
                _motor.CurrentPosition += dir * _stepSize;
                Thread.Sleep(DelayMs);
            }

            _motor.CurrentPosition = _motor.TargetPosition;
            _motor.InMotion = false;
            _motor.HomeInMotion = false;
        }
    }
}
