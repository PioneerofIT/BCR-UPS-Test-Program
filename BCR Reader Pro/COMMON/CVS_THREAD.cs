using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using VSP.COMMON;
using VSP.CONTROLLER;
using static VSP.COMMON.CVS_THREAD;

namespace VSP.COMMON
{

    public class CVS_THREAD
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle)
        // ============================================================

        // ============================================================
        // Description : [1-1] 생성자
        // ============================================================
        public CVS_THREAD(int nIdx, int dwDelay = 100)
        {
            Index = nIdx;
            Delay = dwDelay;
            m_hExit = new ManualResetEvent(false);
        }

        // ============================================================
        // Description : [1-2] 인터페이스 구현 (초기화, 로컬라이징 등)
        // ============================================================
        public void Initialize()
        {
            Step = 0;
            ReturnType = SeqReturnType.BUSY;
            RunType = RunModeType.AUTO;

            InitStatus = SeqInitStatusType.NOT_INIT;
            InitStep = 0;

            IsRun = false;
            IsDryRun = false;
            IsReset = false;

            IsComplete = false;
            IsReady = false;

            IsManualOn = false;
            IsOverloadDetected = false;
            UseStepLog = false;
        }

        protected virtual void DevInit() { }
        protected virtual void AllOutOff() { }
        protected virtual void StepStop() { }

        protected virtual void Always() => CheckRunMode();
        protected virtual void CheckRunMode() { }
        protected /*virtual*/ void CheckActuator() { }

        protected virtual void ResetSeq() { }
        protected virtual void ResetData() { }


        protected virtual SeqReturnType InitRun() { return 0; }
        protected virtual SeqReturnType AutoRun() { return 0; }
        protected bool IsPass => (!IsRun) && (ReturnType == 0);//(Return==0),ReturnType이 BUSY → 즉, 아직 아무런 결과도 없고, 기본 초기 상태라는 의미(모터 구동위에는 꼭 적어서막아둘것)
        public bool IsPassed() { return IsPass; }

        protected void NextStep(int step = -1)
        {
            int prevStep = Step;

            if (step < 0)
            {
                if (IsInitializing && !IsInitOk)
                    InitStep++;
                else
                    Step++;
            }
            else
            {
                if (IsInitializing && !IsInitOk)
                    InitStep = step;
                else
                    Step = step;
            }

            if (Step != prevStep && (!IsInitializing || IsInitOk))
            {
                StepHistory.Enqueue(Step);
                if (StepHistory.Count > MaxStepHistory)
                    StepHistory.Dequeue();

            }


            if (UseStepLog && (!IsInitializing || IsInitOk))
            {
                //UtilExtern.ShowInitialMessage($"{LogHead} Next [{prevStep} → {Step}] Ready:{IsReady} Complete:{IsComplete} ");
            }
        }
        protected virtual bool IsRunningCondition() => true;


        public virtual void StartInitial() 
        {
            InitStatus = SeqInitStatusType.INITIALIZING;
            InitStep = 0;
        }
        public virtual void CancelInitial()
        {
            CVS_THREAD.CancelInit = true;
            InitStatus = SeqInitStatusType.NOT_INIT;
            
            WaitTimer.Reset();

            foreach (var motor in ServoMotorList)
            {
                //if (motor.IsHoming())
                //    motor.SetOrgAbort(true);
            }
        }
        public  void Stop()
        {
            if (IsRun)
            {
                //Global->SetAutoRun(false);
            }

            IsManualOn = false;
            RunType = RunModeType.AUTO;
        }

        public void StepRun()
        {
            if (IsRun)
            { 
                //Global->SetAutoRun(false);
            }

            Run(RunModeType.STEP);
        }
        public void CycleRun()
        {

        }
        public void Run(RunModeType Step)
        {

        }

        public void Run(RunModeType Mode, int Step)
        {

        }
        public void ResetProcStep()
        {
            Step = 0;
            ResetSeq();
            ResetData();
            //m_WaitTimer.Reset();}

        }
        public void SetEmergency()
        {
            //for (auto it = begin(m_vSvrMtr); it != end(m_vSvrMtr); ++it)
            ////	for(size_t i = 0; i < m_vSvrMtr.size(); i++)
            //{
            //    (*it)->SetEStop();
            //    (*it)->ResetHomeDoneOk();
            //    //        m_vSvrMtr.at(i)->SetEStop();
            //    //		m_vSvrMtr.at(i)->ResetHomeDoneOk();
            //}
        }
        public void SendError(int nErrCode)
        {

         //   Stop();
	        //if (!Global->GetErrFlag())
	        //{
		       // Global->SetErrFlag(true);

         //       PostMessage(Application->MainFormHandle, VSP::VS_ALARM_MSG, nErrCode, ALARM_SET);
         //   }
        }
        public void SetDelayTime(int nDelay)
        {
            if (!DelayTimer.IsStarted())
            {
                DelayTimer.SetTimer(nDelay);
                DelayTimer.Start();
            }
        }
        ////---------------------------------------------------------------------------
        void SetWaitTimer(int nVal)
        {
           // if (!Global->GetDryRun() && nVal > 0)
           // {
                DelayTimer.Reset();
                DelayTimer.SetTimer(nVal);
           // }
        }
        //}




        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        protected readonly int Index;
        protected readonly int Delay;

        private DateTime st;
        private int m_nTempStep;

        private Thread m_hThread;
        private int m_dwThreadId;
        private ManualResetEvent m_hExit;


        protected List<CVS_MOTOR> m_vSvrMtr = new();
        protected CWaitTimer WaitTimer = new();
        protected CDelayTimer DelayTimer = new();

        public string LogHead { get; set; } = "[THREAD]";
        public bool UseStepLog { get; set; } = false;

        public static bool CancelInit = false;

        public int StepNo => (IsInitializing && !IsInitOk) ? InitStep : Step;

        public int Step { get; protected set; } = 0;
        public int InitStep { get; protected set; } = 0;

        public bool IsRun { get; protected set; } = false;
        public bool IsDryRun { get; /*protected*/ set; } = false;

        public bool IsReset { get; protected set; } = false;
        public bool IsManualOn { get; protected set; } = false;
        public void ResetManualOn() {IsManualOn = false; }

        public bool IsComplete { get; protected set; } = false;
        public bool IsReady { get; protected set; } = false;

        public bool IsOverloadDetected { get; protected set; } = false;

        private readonly Queue<int> StepHistory = new();    //new item
        private const int MaxStepHistory = 5; //new item

        public List<int> GetLastStepList(int count = 5)
        {
            // StepHistory는 Queue<int>
            int available = Math.Min(count, StepHistory.Count);
            return StepHistory.Reverse().Take(available).ToList();
        }


        // 시퀀스 초기화 상태
        public enum SeqInitStatusType
        {
            NOT_INIT,
            INITIALIZING,
            TIMEOUT,
            FAIL,
            DONE
        }
        public SeqInitStatusType InitStatus { get; protected set; } = SeqInitStatusType.NOT_INIT;

        public bool IsInitOk => InitStatus == SeqInitStatusType.DONE;
        public bool IsInitializing => InitStatus == SeqInitStatusType.INITIALIZING;
        public bool IsInitialiTimeOut  => InitStatus == SeqInitStatusType.TIMEOUT;


        public enum RunModeType
        {
            AUTO,
            STEP,       // 단일 단계 실행
            CYCLE,      // 반복 단위 실행
       
        }
        public RunModeType RunType { get; protected set; } = RunModeType.AUTO;


        public enum SeqReturnType
        {
            BUSY = 0,     // 아직 동작 중
            SUCCESS,      // 정상 완료
            PAUSE,        // 일시 정지됨
            FAIL          // 실패 처리
        }
        public SeqReturnType ReturnType { get; protected set; } = SeqReturnType.BUSY;


        private List<CVS_MOTOR> ServoMotorList = new();


        // ============================================================
        // Description : [2-1] 내부 설정 및 모델 데이터
        // ============================================================


        // ============================================================
        // Description : [3] Internal Logic & UI 이벤트 처리
        // ============================================================
        // ============================================================
        // Description : [3-1] UI 이벤트 핸들러
        // ============================================================
        // (UI 연동 없음)

        // ============================================================
        // Description : [3-2] 내부 동작 및 계산 로직
        // ============================================================
        private ManualResetEvent exitEvent;
        private Thread thread;
        private int threadId;
        // 스레드 시작
        public bool StartThread()
        {
            exitEvent = new ManualResetEvent(false);

            try
            {
                thread = new Thread(ThreadProc);
                thread.IsBackground = true;
                thread.Start(this);
                threadId = thread.ManagedThreadId;

                Console.WriteLine($"[{LogHead}] Thread {threadId}, Created");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{LogHead}] Thread creation failed: {ex.Message}");
                return false;
            }
        }

        // 스레드 루프
        private void ThreadProc(object? obj)
        {
            if (obj is not CVS_THREAD threadInstance)
                return;

            try
            {
                while (!threadInstance.exitEvent.WaitOne(threadInstance.Delay))
                {
                    threadInstance.RunProc();
                }

                Debug.WriteLine($"[{LogHead}] Thread {threadId} ({LogHead}), Exit Execute");
            }
            catch (Exception ex)
            {
                ExpFilter(ex);
            }

            Debug.WriteLine($"[{LogHead}] Loop End Thread {threadId} ({LogHead} Step:{Step})");
        }

        // 예외 필터 (C++의 SEH는 C#에서 일반 예외 처리로 대체)
        private static void ExpFilter(Exception ex)
        {
            Debug.WriteLine($"[CVS_THREAD] Exception caught: {ex.Message}");
            // 필요 시 로그 저장 또는 재처리
        }

        // 스레드 종료
        public virtual void QuitThread()
        {
            exitEvent.Set();
            thread?.Join();
            Debug.WriteLine($"[{LogHead}] Thread {threadId} Quit");
        }

        // 실제 동작
        public SeqReturnType RunProc()
        {
            //LogHead;
            Always();

            if (DelayTimer.IsRemainDelay())
                return SeqReturnType.BUSY;

            if (InitStatus == SeqInitStatusType.INITIALIZING && InitStatus != SeqInitStatusType.DONE)
            {
                ReturnType = InitRun();
            }
            else
            {
                if (IsRunningCondition())
                    ReturnType = AutoRun();
                else
                    ReturnType = SeqReturnType.BUSY;
            }

            switch (RunType)
            {
                case RunModeType.AUTO:
                    break;

                case RunModeType.STEP:
                    if (ReturnType != SeqReturnType.BUSY)
                    {
                        Stop();
                        return SeqReturnType.BUSY;
                    }
                    break;

                case RunModeType.CYCLE:
                    if (ReturnType >= SeqReturnType.BUSY)
                    {
                        Stop();
                        return SeqReturnType.BUSY;
                    }
                    break;
            }

            return ReturnType;
        }


        // ============================================================
        // Description : [4] External Dependencies (외부 연동 / 저장소 요청)
        // ============================================================

        // ============================================================
        // Description : [4-1] 외부 시스템 요청 (DB, API 등)
        // ============================================================
        // (현재 없음)

        // ============================================================
        // Description : [4-2] 외부에서 호출되는 진입 함수 (Interop 등)
        // ============================================================
        // (현재 없음)


        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================

        // 추후 확장 시 필요한 멤버들 배치 가능
        public virtual void FindStep() {  }

    }

}