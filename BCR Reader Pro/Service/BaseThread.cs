using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCR_Reader_Pro.Service
{
    internal class BaseThread
    {
        private Thread? _thread;
        private ManualResetEvent? _exitEvent;
        private ManualResetEvent? _pauseEvent;
        protected bool _isRunning;
        protected int _step;
        protected int _idx;
        protected int _createDelay;
        

        public BaseThread(int idx, int delay )
        {
            _idx = idx;
            _createDelay = delay;
            Initialize();
            StartThread();
        }

        protected void Initialize()
        {
            _isRunning = false;
            _idx = 0;
        }
        protected void NextStep(int step = -1)
        {
            if(step < 0)
            {
                _step++;
            }
            else
            {
                _step = step;
            }
        }
        public bool StartThread()
        { 
            _exitEvent = new ManualResetEvent(false); 
            _thread = new Thread(VsThreadProc)
            {
                IsBackground = true
            };
            _thread.Start();
            //Console.WriteLine($"Thread {_thread.ManagedThreadId}({_logHead}), Created");
            return true;
        }

        private void VsThreadProc()
        {
            try
            {
                while (true) // ✅ 스레드는 계속 돈다
                {
                    if (_exitEvent!.WaitOne(_createDelay))
                        break;

                    if (!_isRunning)
                        continue; // 🔹 가동 OFF 시에는 아무 일도 안 함

                    RunProc();   // 🔹 _isRunning = true 일 때만 AutoRun 실행
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExpFilter] Thread Exception: {ex.Message}");
            }
        }
        public void ThreadEnd()
        {
            try
            {
                if (!_isRunning) return;

                _isRunning = false;

                // Pause 상태에 있더라도 unblock 해서 종료로 진행하게 함
                _pauseEvent?.Set();
                _exitEvent?.Set();

                if (_thread != null && _thread.IsAlive)
                {
                    if (!_thread.Join(1000))
                    {
                        // 최후 수단: Interrupt (예외 처리 필요)
                        _thread.Interrupt();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StopError] Thread {_idx} stop exception: {ex.Message}");
            }
            finally
            {
                _exitEvent?.Dispose();
                _exitEvent = null;
                _pauseEvent?.Dispose();
                _pauseEvent = null;
                _thread = null;
            }
        }
        public void Stop()
        {
            _isRunning = false;
        }

        public void Start()
        {
            _isRunning = true;
        }


        private int RunProc()
        {
            AutoRun();
            return 0;
        }

        protected virtual int AutoRun()
        {
            return 0;
        }

        protected virtual void DevInit()
        {

        }

        protected virtual void InitRun()
        {

        }




    }
}
