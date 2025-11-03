using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace VSP.CONTROLLER
{
    // ============================================================
    // Description :CDI Class
    // ============================================================
    public class CDI
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle)
        // ============================================================

        // ============================================================
        // Description : [1-1] 생성자
        // ============================================================
        public CDI(ushort index)
        {
            _index = index;
            _isOn = false;
            ResetTimer();
        }

        // ============================================================
        // Description : [1-2] 인터페이스 구현 (초기화, 로컬라이징 등)
        // ============================================================
        public static void SetIoController(CIoCtrl controller)
        {
            _ioCtrl = controller;
        }
        // TODO: 향후 다중 컨트롤러 대응 시 CDI 구조 리팩토링 필요
        // 변경 방향 예시:
        // public CDI(CIoCtrl ctrl, int index) { _ioCtrl = ctrl; ... }
        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        // ============================================================
        // Description : [2-1] 내부 설정 및 모델 데이터
        // ============================================================
        private static CIoCtrl _ioCtrl;
        private readonly ushort _index;
        public ushort Index => _index;
        private bool _isOn;

        private DateTime _onStart;
        private DateTime _offStart;

        private TimeSpan _onTime;
        private TimeSpan _offTime;

        // ============================================================
        // Description : [3] Internal Logic & UI 이벤트 처리
        // ============================================================
        // ============================================================
        // Description : [3-1] UI 이벤트 핸들러
        // ============================================================
        // ============================================================
        // Description : [3-2] 내부 동작 및 계산 로직
        // ============================================================

        public void On()
        {
            if (_ioCtrl != null)
                _ioCtrl.SetBit(_index, true, false);
            else
                _isOn = true;

            _offStart = DateTime.MinValue;
            _offTime = TimeSpan.Zero;
        }

        public void Off()
        {
            if (_ioCtrl != null)
                _ioCtrl.SetBit(_index, false, false);
            else
                _isOn = false;

            _onStart = DateTime.MinValue;
            _onTime = TimeSpan.Zero;
        }
        
        public virtual bool IsOn()
        {
            if (_ioCtrl != null)
                _isOn = _ioCtrl.IsOn(_index);

            if (_isOn)
            {
                _offStart = DateTime.MinValue;
                _offTime = TimeSpan.Zero;
            }
            else
            {
                _onStart = DateTime.MinValue;
                _onTime = TimeSpan.Zero;
            }

            return _isOn;
        }

        public virtual bool IsOff() => !IsOn();

        public bool IsTmOn(TimeSpan requiredOnTime)
        {
            if (_onStart == DateTime.MinValue)
                _onStart = DateTime.Now;

            if (IsOn())
            {
                if (_onStart > DateTime.Now)
                {
                    _onStart = DateTime.Now;
                    return false;
                }

                _onTime = DateTime.Now - _onStart;

                if (_onTime.TotalMilliseconds >= 100000)
                    _onTime = requiredOnTime;
            }

            return _onTime >= requiredOnTime;
        }

        public bool IsTmOff(TimeSpan requiredOffTime)
        {
            if (_offStart == DateTime.MinValue)
                _offStart = DateTime.Now;

            if (IsOff())
            {
                if (_offStart > DateTime.Now)
                {
                    _offStart = DateTime.Now;
                    return false;
                }

                _offTime = DateTime.Now - _offStart;

                if (_offTime.TotalMilliseconds >= 100000)
                    _offTime = requiredOffTime;
            }

            return _offTime >= requiredOffTime;
        }

        public void ResetTimer()
        {
            _onStart = DateTime.MinValue;
            _offStart = DateTime.MinValue;
            _onTime = TimeSpan.Zero;
            _offTime = TimeSpan.Zero;
        }

        // ============================================================
        // Description : [4] External Dependencies (외부 연동 / 저장소 요청)
        // ============================================================
        // ============================================================
        // Description : [4-1] 외부 시스템 요청 (DB, API 등)
        // ============================================================
        // ============================================================
        // Description : [4-2] 외부에서 호출되는 진입 함수 (Interop 등)
        // ============================================================
        public DateTime GetOnStart() => _onStart;
        public DateTime GetOffStart() => _offStart;
        public TimeSpan GetOnTime() => _onTime;
        public TimeSpan GetOffTime() => _offTime;
        public int GetIdx() => _index;

    }

    // ============================================================
    // Description :CDI Class
    // ============================================================
    public class CDO
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle)
        // ============================================================

        // [1-1] 생성자
        private static CIoCtrl _ioCtrl;             // static 컨트롤러
        private readonly ushort _index;
        private bool _isOn;

        public static void SetIoController(CIoCtrl controller)
        {
            _ioCtrl = controller;
        }

        public CDO(ushort index)
        {
            _index = index;
            _isOn = false;
            ResetTimer();
        }

        // ============================================================
        // Description : [1-2] 인터페이스 구현 (초기화 등)
        // ============================================================
        // <reserved>

        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================

        private bool _toggleOn;
        private DateTime _onStart;
        private DateTime _offStart;
        private TimeSpan _onTime;
        private TimeSpan _offTime;

        // ============================================================
        // Description : [3] Internal Logic & UI 이벤트 처리
        // ============================================================

        // [3-2] 내부 동작 및 계산 로직
        public virtual bool IsOn()
        {
            if (_ioCtrl != null)
                _isOn = _ioCtrl.IsOn(_index, true);

            if (_isOn)
            {
                _offStart = DateTime.MinValue;
                _offTime = TimeSpan.Zero;
            }
            else
            {
                _onStart = DateTime.MinValue;
                _onTime = TimeSpan.Zero;
            }

            return _isOn;
        }

        public virtual bool IsOff()
        {
            return !IsOn();
        }

        public virtual void On()
        {
            if (_ioCtrl != null)
                _ioCtrl.SetBit(_index, true);
            else
                _isOn = true;

            _offStart = DateTime.MinValue;
            _offTime = TimeSpan.Zero;
        }

        public virtual void Off()
        {
            if (_ioCtrl != null)
                _ioCtrl.SetBit(_index, false);
            else
                _isOn = false;

            _onStart = DateTime.MinValue;
            _onTime = TimeSpan.Zero;
        }

        public bool IsTmOn(TimeSpan requiredOnTime)
        {
            if (_onStart == DateTime.MinValue)
                _onStart = DateTime.Now;

            if (IsOn())
            {
                if (_onStart > DateTime.Now)
                {
                    _onStart = DateTime.Now;
                    return false;
                }

                _onTime = DateTime.Now - _onStart;

                if (_onTime.TotalMilliseconds >= 100000)
                    _onTime = requiredOnTime;
            }

            return _onTime >= requiredOnTime;
        }

        public bool IsTmOff(TimeSpan requiredOffTime)
        {
            if (_offStart == DateTime.MinValue)
                _offStart = DateTime.Now;

            if (IsOff())
            {
                if (_offStart > DateTime.Now)
                {
                    _offStart = DateTime.Now;
                    return false;
                }

                _offTime = DateTime.Now - _offStart;

                if (_offTime.TotalMilliseconds >= 100000)
                    _offTime = requiredOffTime;
            }

            return _offTime >= requiredOffTime;
        }

        public void ResetTimer()
        {
            _onStart = DateTime.MinValue;
            _offStart = DateTime.MinValue;
            _onTime = TimeSpan.Zero;
            _offTime = TimeSpan.Zero;
        }

        // ============================================================
        // Description : [4] External Dependencies
        // ============================================================

        public TimeSpan GetOnTime() => _onTime;
        public TimeSpan GetOffTime() => _offTime;
        public int GetIdx() => _index;

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        public bool IsToggleOn() => _toggleOn;
        public void SetToggleOn(bool value) => _toggleOn = value;
    }

    // ============================================================
    // Description : CAnalogIn Class
    // ============================================================

    public class CAnalogIn
    {
        const int BUF_CNT = 5;
        // 🔧 공유 컨트롤러
        private static CIoCtrl? _ioCtrl = null;
        public static void SetIoController(CIoCtrl controller) => _ioCtrl = controller;

        // 🧠 내부 상태
        private readonly int _index;
        private int _readData = 0;

        private long _resolution = 0;
        private ushort _devRange = 0;
        private double _voltRange = 0.0;
        private double _calibration = 1.0;

        private readonly List<int> _digitalSamples = new();

        //private readonly CWaitTimer _alarmTimer = new();

        public CAnalogIn(int index)
        {
            _index = index;
        }

        // 📏 보정 인자 설정
        public void SetFactor(ushort resBits, ushort devRange, double voltRange, double cal)
        {
            _devRange = devRange;
            _voltRange = voltRange;
            _calibration = cal;
            _resolution = (devRange == 0) ? 10 : (long)Math.Pow(2.0, resBits);
        }

        // 📡 센서 데이터 읽기 및 중간값 평균 처리
        public void GetReadData()
        {
            if (_ioCtrl == null) return;

            int value = _ioCtrl.GetWord(_index, _devRange, false);

            if (_digitalSamples.Count < BUF_CNT)
            {
                _digitalSamples.Add(value);
            }
            else
            {
                var sorted = _digitalSamples.OrderBy(x => x).ToList();
                int mid = BUF_CNT / 2;
                _readData = (sorted[mid - 1] + sorted[mid] + sorted[mid + 1]) / 3;
                _digitalSamples.Clear();
            }
        }

        // ⚙️ 보정 적용된 아날로그 값 반환
        public double GetAnalogVal()
        {
            double cal = (_devRange == 0) ? 1.0 : _calibration;
            return (_resolution > 0) ? (cal * _readData / _resolution) : 0.0;
        }

        // 🔢 원시 디지털값
        public int GetDigital() => _readData;

        // ⏱ 알람 타이머 관련
        //public bool IsAlarmStarted() => _alarmTimer.IsStarted();
        //public void SetWaitTimer(int timeMs) => _alarmTimer.SetTimer(timeMs);
        //public void ResetWaitTimer() => _alarmTimer.Reset();
        //public bool IsAlarmOn() => _alarmTimer.IsWaitAlarm();
    }

    // ============================================================
    // Description : CAnalogOut Class
    // ============================================================
    public class CAnalogOut
    {
        // 🔧 static 연결된 컨트롤러
        private static CIoCtrl? _ioCtrl = null;
        public static void SetIoController(CIoCtrl controller) => _ioCtrl = controller;

        // 🧠 내부 상태
        private readonly int _index;

        private long _resolution = 0;
        private ushort _devRange = 0;
        private double _voltRange = 0.0;
        private double _calibration = 1.0;

        private int _writeDigital = 0;
        private double _writeAnalog = 0.0;

        // 🔧 생성자
        public CAnalogOut(int index)
        {
            _index = index;
        }

        // 📏 보정값 및 설정 적용
        public void SetFactor(ushort resBits, ushort devRange, double voltRange, double cal)
        {
            _resolution = (devRange == 0) ? 10 : (long)Math.Pow(2.0, resBits);
            _devRange = devRange;
            _voltRange = voltRange;
            _calibration = cal;

            _ioCtrl?.SetDaConfig((ushort)_index, _voltRange);
        }

        // 🔢 디지털 출력 설정
        public void SetWriteData(int value)
        {
            _writeDigital = value;
            _ioCtrl?.SetWordVal(_index, (ushort)_writeDigital, true);
        }

        // ⚙️ 아날로그 출력 설정
        public void SetAnalogVal(double value)
        {
            if (_calibration <= 0.0) return;

            _writeAnalog = (value / _calibration) * _voltRange;

            _ioCtrl?.WriteAnalogVal((ushort)_index, _writeAnalog);

            // ※ 필요 시, 보정 계산 방식을 여기에 확장 가능
            // _writeAnalog = (value * _resolution * _devRange) / (_calibration * _voltRange);
        }

        // 📈 아날로그 최대 출력값
        public double GetMaxRange()
        {
            return _ioCtrl?.GetMaxRange((ushort)_index) ?? 0.0;
        }

        // 📦 현재 설정된 아날로그 출력값
        public double GetWriteAnalogVal()
        {
            return _ioCtrl?.GetWriteAnalogVal((ushort)_index) ?? 0.0;
        }
    }

    // ============================================================
    // Description : CMotion Input Class
    // ============================================================

    public class CDI_M : CDI
    {
        private static CIoCtrl? _ioCtrl = null;
        public static void SetIoController(CIoCtrl controller) => _ioCtrl = controller;

        // 내부 상태 변수
        private bool _isOn = false;

        private uint _dwOnStart = 0;
        private uint _dwOnTime = 0;
        private uint _dwOffStart = 0;
        private uint _dwOffTime = 0;

        public CDI_M(int index) : base((ushort)index) { }

        void On()
        {
            if (_ioCtrl != null)
                _ioCtrl.SetBit(Index, true, false);
            else
                _isOn = true;

            _dwOffStart = 0;
            _dwOffTime = 0;
        }

        void Off()
        {
            if (_ioCtrl != null)
                _ioCtrl.SetBit(Index, false, false);
            else
                _isOn = false;

            _dwOnStart = 0;
            _dwOnTime = 0;
        }

        public override bool IsOn()
        {
            if (_ioCtrl != null)
                _isOn = _ioCtrl.IsOn((ushort)Index);

            if (_isOn)
            {
                _dwOffStart = 0;
                _dwOffTime = 0;
            }
            else
            {
                _dwOnStart = 0;
                _dwOnTime = 0;
            }

            return _isOn;
        }

        public override bool IsOff()
        {
            return !IsOn();
        }

        // 필요 시 타이머 값 참조용 속성 추가 가능:
        public uint OffStart => _dwOffStart;
        public uint OffTime => _dwOffTime;
        public uint OnStart => _dwOnStart;
        public uint OnTime => _dwOnTime;
    }
}