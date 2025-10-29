using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCR_Reader_Pro.Model
{
    public enum BcrMode { ComPort = 0, TcpIp = 1 }
    public sealed record BcrSettings
    (
        BcrMode Mode,
        string? ComName,
        string? Ip,
        int? Port
     );
    public class Inspectproc : INotifyPropertyChanged, IDisposable
    {
        
        private readonly BcrSettings _cfg;
        private readonly int _bcrType;
        private SerialPort? _bcrComm;
        private string _strTerminator = "\r";
        private string _bcrReadingResult;
        private TcpClient? _bcrTcp;
        private NetworkStream? _stream;
        private StringBuilder _rxBuffer = new StringBuilder();

        public event PropertyChangedEventHandler? PropertyChanged;
        private CancellationTokenSource? _cts;
        public event EventHandler<string>? TcpDataReceived;

        public ObservableCollection<string> ReadingLogs { get; } = new();
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public int BcrType { get { return _bcrType; } }
        public SerialPort BcrComm { get { return _bcrComm; } }
        public TcpClient BcrTcp { get { return _bcrTcp; } }
        public string BcrReadingResult
        {
            get { return _bcrReadingResult; }
            set
            {
                if (_bcrReadingResult != value)
                {
                    _bcrReadingResult = value;
                    OnPropertyChanged();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        // UI 스레드에서만 컬렉션에 추가
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ReadingLogs.Add(value);
                        });
                    }


                }

            }
        }
        public string StrTerminator
        {
            get { return _strTerminator; }
            set { _strTerminator = value; }
        }
                                     

        public Inspectproc(BcrSettings cfg)
        {
            _cfg = cfg;
            _bcrType = (int)_cfg.Mode;

            if (_bcrType == 0)
            {
                _bcrComm = new SerialPort();
                OpenReaderComm();
            }
            else if (_bcrType == 1)
            {
                _bcrTcp = new TcpClient();
                OpenReaderComm();

            }

            
        }


        void SetCommPort()
        {
            if (BcrType == 0)
            {
                _bcrComm.PortName = _cfg.ComName;
                _bcrComm.BaudRate = 115200;
                _bcrComm.Parity = Parity.None;
                _bcrComm.DataBits = 8;
                _bcrComm.StopBits = StopBits.One;
                _bcrComm.Encoding = Encoding.ASCII;
                _bcrComm.NewLine = StrTerminator;
                _bcrComm.DataReceived += OnDataReceived;
                _bcrComm.DtrEnable = true;
                _bcrComm.RtsEnable = true;

                _bcrComm.ReadTimeout = 1000;
                _bcrComm.WriteTimeout = 1000;
            }

        }

        public void OpenReaderComm()
        {
            SetCommPort();

            try
            {
                
                if ( BcrType == 0)
                {
                    BcrComm.Open();
                    MessageBox.Show("성공");
                }
                else if (BcrType == 1)
                {
                    // Opt 으로 ip, port받아오기
                    var IpAdrr = _cfg.Ip;
                    var Port = _cfg.Port;
                    if (IpAdrr != null && Port != null)
                    {
                        _bcrTcp.Connect(IpAdrr, (int)Port);
                        _stream = _bcrTcp.GetStream();
                        MessageBox.Show("성공");
                        StartTcpReceiver();
                    }

                }
            }
            catch
            {
                Console.WriteLine("BCR COMPORT OPEN Fail");
            }
            
        }

        public void OnTriger()
        {
            string sTx = "LON";

            sTx += StrTerminator;

            if(IsReadCmdSend(sTx))
            {
                //MessageBox.Show("Triger On");
            }

        }

        public bool IsReadCmdSend(string cmd)
        {

            try
            {
                if (BcrType == 0)
                {
                    BcrComm.WriteLine(cmd);
                }
                else if (BcrType == 1)
                {
                    var bytes = Encoding.ASCII.GetBytes(cmd);
                    _stream.Write(bytes, 0, bytes.Length);
                    _stream.Flush();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("전송 실패: " + e.Message);
                return false;
            }

        }


        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            BcrReadingResult = string.Empty;
            //MessageBox.Show("OnDataReceived");

            try
            {
                string chunk = BcrComm.ReadExisting(); 
                _rxBuffer.Append(chunk);

                CheckReceiveComplete();
                

            }
            catch (TimeoutException)
            {
                Console.WriteLine("TimeOut");
            }


        }
        private void StartTcpReceiver()
        {
            if (_stream == null) return;

            // 기존 루프가 있다면 정리
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            // 백그라운드에서 대기-수신; while은 있지만 ReadAsync가 블로킹하므로 CPU를 점유하지 않음
            _ = Task.Run(() => TcpReadLoopAsync(_cts.Token));
        }

        private async Task TcpReadLoopAsync(CancellationToken ct)
        {
            var vBuffer = new byte[1024];
            var sb = new StringBuilder();

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    int nPacket = await _stream!.ReadAsync(vBuffer, 0, vBuffer.Length, ct).ConfigureAwait(false);
                    if (nPacket == 0) break; // 연결 종료

                    sb.Append(Encoding.ASCII.GetString(vBuffer, 0, nPacket));

                    // 종단문자 기준으로 라인 분리
                    string term = StrTerminator;
                    int idx;
                    while ((idx = sb.ToString().IndexOf(term, StringComparison.Ordinal)) >= 0)
                    {
                        string line = sb.ToString(0, idx);
                        sb.Remove(0, idx + term.Length);

                        // 속성 업데이트 (WPF 로그는 기존 로직이 처리)
                        BcrReadingResult = line;

                        // [이벤트 발행] 구독자가 있으면 알림
                        TcpDataReceived?.Invoke(this, line);
                    }
                }
            }
            catch (OperationCanceledException) { /* 정상 취소 */ }
            catch (ObjectDisposedException) { /* 스트림 종료 시 */ }
            catch (Exception ex)
            {
                Console.WriteLine("TCP 수신 오류: " + ex.Message);
            }
        }

        public void Dispose()
        {
            if (_bcrComm != null)
            {
                _bcrComm.DataReceived -= OnDataReceived;
                if (_bcrComm.IsOpen) _bcrComm.Close();
                _bcrComm.Dispose();
            }
        }
        private void CheckReceiveComplete()
        {
            string term = StrTerminator; // "\r" 또는 "\r\n"
            string current = _rxBuffer.ToString();

            if (current.EndsWith("\r") || current.EndsWith("\n"))
            {
                // 종단문자 제거
                string msg = current.TrimEnd('\r', '\n');
                _rxBuffer.Clear();

                // 로그 찍기
                Console.WriteLine($"ID Read [{msg}]");

                // 정상 판정
                if (!string.IsNullOrWhiteSpace(msg) && msg.Length > 0 || msg.Contains("ERROR"))
                {
                    if (msg.Contains("ERROR"))
                    {
                        // ERROR 응답
                        BcrReadingResult = "ERROR";
                        
                    }
                    else
                    {
                        // 정상 데이터
                        BcrReadingResult = msg;
                        
                    }
                }
                else
                {
                    // 너무 짧거나 비정상
                  
                }

  
     
            }
        }

    }
}
