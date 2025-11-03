using System.Timers;

namespace VSP.COMMON
{
    public class AlarmManager
    {
        private bool _hideAlarm;
        private List<int> _alarmIds;
        private System.Timers.Timer _tmShowErr;

        public AlarmManager()
        {
            _hideAlarm = false;
            _alarmIds = new List<int>();

            // 타이머 초기화 및 이벤트 핸들러 설정
            _tmShowErr = new System.Timers.Timer(30); // 30ms 간격 설정
            _tmShowErr.Elapsed += ShowErrTimer; // 이벤트 핸들러 연결
            _tmShowErr.AutoReset = false; // 한 번만 실행되도록 설정
            _tmShowErr.Enabled = false; // 기본적으로 비활성화
        }

        ~AlarmManager()
        {
            _tmShowErr.Dispose();
        }

        public void SetAlarm(int alarmId)
        {
            _alarmIds.Add(alarmId);

            if (!_hideAlarm)
            {
                _tmShowErr.Enabled = true;
            }

            // 이벤트 관련 로직 (임의의 상태 변경)
            //var eventInfo = new CeidEventInfo();
            //eventInfo.NewMcState = SysOption.CimType == CimType.LGIT ? State.LGIT_PAUSE : State.STATE_DOWN;
            //CimManager.ReserveEventReport(EventType.PROCSTATE_CHANGE, eventInfo);
            //CimManager.SendErrorSet(alarmId);
        }

        public void ResetAlarm(int alarmId)
        {
            //RunSwLamp.ResetErrFlags();
            //_alarmIds.Remove(alarmId);

            //if (alarmId == -1)
            //{
            //    _hideAlarm = false;
            //    _alarmIds.Clear();
            //    return;
            //}

            //if (SystemSeq.IsInManualFunction())
            //{
            //    SystemSeq.ResetManualFunction();
            //}

            //CimManager.SendErrorReset(alarmId);

            //if (_alarmIds.Count == 0)
            //{
            //    var eventInfo = new CeidEventInfo();
            //    eventInfo.NewMcState = SysOption.CimType == CimType.LGIT ? State.LGIT_IDLE : State.STATE_IDLE;
            //    CimManager.ReserveEventReport(EventType.PROCSTATE_CHANGE, eventInfo);
 //           }
        }

        public void SetHideAlarm(bool value) => _hideAlarm = value;

        private void ShowErrTimer(object sender, ElapsedEventArgs e)
        {
            if (_tmShowErr.Enabled)
            {
                _tmShowErr.Enabled = false;

                if (!_hideAlarm && GetAlarmNo() >= 0)
                {
                    //ShowAlarmDlg(GetAlarmNo());
                }
            }
        }

        public int GetAlarmNo() => _alarmIds.Count > 0 ? _alarmIds[0] : -1;

        public void ResetAlarmWindow()
        {
            //PostMessage(hAlarmDlg, VSP.VS_PBS_MSG, ResetType.RESET_SW, 0);
        }
    }

    // 싱글톤 인스턴스 (필요한 경우)
    public static class GlobalAlarmManager
    {
        public static AlarmManager Instance = new AlarmManager();
    }
}
