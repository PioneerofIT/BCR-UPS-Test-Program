using System.Diagnostics;
using VSP.COMMON;
using VSP.CONTROLLER;

namespace VSP.CONTROLLER
{
    public class CVS_READ_AI_THREAD : CVS_THREAD
    {
        private List<CAnalogIn> aiList = new();
        private int currentIndex = 0;

        public CVS_READ_AI_THREAD(int index, int delay)
            : base(index, delay)
        {
            LogHead = "READ_AI";
            DevInit();
        }

        ~CVS_READ_AI_THREAD()
        {
            aiList.Clear();
        }

        protected override void DevInit()
        {
            aiList.Clear();

            for (int i = 0; i < (int)AnalogInput.MAX_AI; i++)
            {
                var ai = Ain.AI[i]; // AI[i]에 해당하는 접근 방식
                if (ai != null)
                    aiList.Add(ai);
            }

            currentIndex = 0;
        }

        protected override SeqReturnType AutoRun()
        {
            if (aiList.Count == 0)
                return 0;

            if (currentIndex >= aiList.Count)
                currentIndex = 0;

            var ai = aiList[currentIndex];
            ai?.GetReadData();

            // 예시: 로그 출력 또는 값 활용
            double analogVal = ai?.GetAnalogVal() ?? 0.0;
            int digitalVal = ai?.GetDigital() ?? 0;

            //Debug.WriteLine($"[READ_AI] AI[{currentIndex}] → Analog: {analogVal:F2}, Digital: {digitalVal}");

            switch (Step)
            {
                case 0:
                    NextStep(1);
                    SetDelayTime(3000);
                    break;
                case 1:
                    NextStep(0);
                    SetDelayTime(3000);
                    break;

                default:
                    break;
            }
            currentIndex++;
            return SeqReturnType.BUSY;
        }

        protected override bool IsRunningCondition()
        {
            return true;
        }

        //public  void RunProc()
        //{
        //    AutoRun();
        //}
    }
}