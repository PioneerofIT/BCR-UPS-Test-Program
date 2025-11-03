using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP.COMMON
{
    public class SimSMEMA
    {
        private const int SmemaMax = 2;
        private bool[] BaFromUp = new bool[SmemaMax];
        private bool[] MrFromDown = new bool[SmemaMax];

        public SimSMEMA()
        {
            Array.Fill(BaFromUp, false);
            Array.Fill(MrFromDown, false);
        }

        public void SetBaFromUp(int lane, bool value) => BaFromUp[lane] = value;
        public bool IsBaFromUp(int lane) => BaFromUp[lane];

        public void SetMrFromDown(int lane, bool value) => MrFromDown[lane] = value;
        public bool IsMrFromDown(int lane) => MrFromDown[lane];

        public void ClearSimSignal()
        {
            Array.Fill(BaFromUp, false);
            Array.Fill(MrFromDown, false);
        }
    }

    public class VS_TIMER
    {
        // 🧠 내부 상태
        protected List<long> AccumulateTime = new();   // 누적 시간 기록
        protected Stopwatch Stopwatch = new();         // 현재 타이머
        protected bool IsStartedFlag = false;          // 시작 여부
        protected bool IsPaused = false;               // 일시정지 상태

        // ✅ 시작
        public void Start()
        {
            IsStartedFlag = true;
            IsPaused = false;
            Stopwatch.Restart();
            AccumulateTime.Clear();
        }

        // ✅ 일시정지
        public void Pause()
        {
            if (!IsPaused && Stopwatch.IsRunning)
            {
                AccumulateTime.Add(Stopwatch.ElapsedMilliseconds);
                Stopwatch.Stop();
                IsPaused = true;
            }
        }

        // ✅ 초기화
        public virtual void Reset()
        {
            IsStartedFlag = false;
            IsPaused = false;
            Stopwatch.Reset();
            AccumulateTime.Clear();
        }

        // ✅ 현재 타이머 경과
        public long GetInterval() => Stopwatch.ElapsedMilliseconds;

        // ✅ 누적 경과 시간 (일시정지 포함 전체)
        public long GetElapsed()
        {
            long total = GetAccumulatedTime();
            long current = Stopwatch.IsRunning ? Stopwatch.ElapsedMilliseconds : 0;
            return total + current;
        }

        // ✅ 누적 시간 총합
        protected long GetAccumulatedTime()
        {
            long sum = 0;
            foreach (var ms in AccumulateTime)
                sum += ms;
            return sum;
        }

        // ✅ 누적 리스트 초기화
        protected void ClearPauseQueue() => AccumulateTime.Clear();

        // ✅ 외부 접근용: 타이머 시작 여부
        public bool IsStarted() => IsStartedFlag;
    }

    public class CWaitTimer : VS_TIMER
    {
        private int WaitTime;
        private int ErrNo;

        public void SetTimer(int value) => WaitTime = value;
        public bool IsWaitAlarm() => GetElapsed() >= WaitTime;

        public int GetWaitTime() => WaitTime;
        public int GetErrNo() => ErrNo;
    }

    public class CDelayTimer : VS_TIMER
    {
        private bool isStarted = false;
        private int delayTime = 0;

        public void SetTimer(int milliseconds)
        {
            delayTime = milliseconds;
            isStarted = true;
            Start(); // VSTimer의 타이머 시작 메서드
        }

        public bool IsRemainDelay()
        {
            if (isStarted)
            {
                if (GetElapsed() < delayTime)
                {
                    return true;
                }
                else
                {
                    Reset();
                    return false;
                }
            }

            return false;
        }

        public override void Reset()
        {
            base.Reset();
            delayTime = 0;
            isStarted = false;
        }
    }

}
