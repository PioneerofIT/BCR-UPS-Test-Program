using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using VSP.GUI.BASE_FRAME;

namespace VSP.COMMON
{
    public sealed class CVS_VIEW_MANAGER
    {
        // ============================================================
        // Description : [1-1] 생성자
        // ============================================================

        private CVS_VIEW_MANAGER()
        {
            _updateInterval = 100;
            _updateVisibleOnly = true;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(_updateInterval)
            };

            _timer.Tick += OnTick;
            _timer.Start(); // ✅ Tick 이벤트 작동시키기 위해 Start 호출s

        }

        public void Initialize()
        {

        }
        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================

        private static CVS_VIEW_MANAGER instance;
        public static CVS_VIEW_MANAGER Instance => instance ??= new CVS_VIEW_MANAGER();

        private  DispatcherTimer _timer;
        private readonly int _updateInterval;
        private readonly bool _updateVisibleOnly;

        private readonly List<IVsFrame> _viewObjects = new(); // ✅ 강한 참조로 변경

        public int RegisteredFrameCount => _viewObjects.Count;
        public bool IsRunning => _timer.IsEnabled;

        // ============================================================
        // Description : [3] Accessors / Computation
        // ============================================================

        public IEnumerable<IVsFrame> GetVisibleFrames()
        {
            return _viewObjects
                .Where(f => f is FrameworkElement fe && fe.IsVisible);
        }

        // ============================================================
        // Description : [4] Internal Logic / Validation
        // ============================================================

        public void InitCtrlsAll()
        {
            foreach (var frame in _viewObjects)
                frame.InitCtrls();
        }

        public void LocalizeAll()
        {
            foreach (var frame in _viewObjects)
                frame.Localize();
        }

        // ============================================================
        // Description : [4-1] UI 이벤트 핸들러
        // ============================================================
        private int _tickCount = 0;
        private void OnTick(object? sender, EventArgs e)
        {

            foreach (var frame in _viewObjects)
            {
                try
                {
                    if (_updateVisibleOnly && frame is FrameworkElement fe && !fe.IsVisible)
                        continue;

                    frame.UpdateStates();

                    if (TraceLoggingEnabled)
                        Console.WriteLine($"[Tick] Updated: {frame.GetType().Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Tick][Error] {frame.GetType().Name}: {ex.Message}");
                }
            }
        }

        // ============================================================
        // Description : [4-2] 내부 동작 로직
        // ============================================================
      
        public void RegisterViewObject(IVsFrame obj)
        {

            if (_viewObjects.Contains(obj)) return;
            _viewObjects.Add(obj);
        }

        public void Unregister(IVsFrame obj)
        {
            _viewObjects.Remove(obj);
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        // ============================================================
        // Description : [5] 기타
        // ============================================================

        public void ForceUpdateOnce()
        {
            OnTick(null, EventArgs.Empty);
        }

        public bool TraceLoggingEnabled { get; set; } = false;
    }
}