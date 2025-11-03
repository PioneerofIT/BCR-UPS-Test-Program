using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP.CONTROLLER.SimulatorCtrl.Objects
{
    public class MotorObject
    {
        public bool InMotion { get; set; }
        public bool HomeInMotion { get; set; }
        public double CurrentPosition { get; set; }
        public double TargetPosition { get; set; }
        public bool ServoOn { get; set; }

        public MotorObject()
        {
            InMotion = false;
            HomeInMotion = false;
            CurrentPosition = 0.0;
            TargetPosition = 0.0;
            ServoOn = false;
        }

        public void GenerateVirtualPos(double velocity, double acceleration)
        {
            if (InMotion || HomeInMotion)
                return;

            InMotion = true;
            var thread = new Thread(() =>
            {
                var mover = new MotorMoveThread(this, velocity, acceleration);
                mover.Run();
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
