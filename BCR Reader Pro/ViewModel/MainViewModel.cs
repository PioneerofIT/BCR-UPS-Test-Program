using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BCR_Reader_Pro.Model;
using GalaSoft.MvvmLight.Command;

namespace BCR_Reader_Pro.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public ICommand ConnectCommand { get; }
        public ICommand ReadCommand { get; }
        public MainViewModel()
        {
            ConnectCommand = new RelayCommand( BcrConnectCommand,  CanConnect);
            ReadCommand = new RelayCommand( BcrReadCommand, CanRead);

        }

        private Inspectproc? _runInspect;
        public Inspectproc? RunInspect
        {
            get => _runInspect;
            private set
            {
                if (_runInspect == value) return;
                _runInspect?.Dispose();
                _runInspect = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        // ... Type은 Option ini에서
        private bool _isTcpIpMode;
        public bool IsTcpIpMode
        {
            get { return _isTcpIpMode; }
            set
            {
                if (_isTcpIpMode != value)
                {
                    _isTcpIpMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isCommMode;
        public bool IsCommMode
        {
            get { return _isCommMode; }
            set
            {
                if (_isCommMode != value)
                {
                    _isCommMode = value;
                    // 종속 속성을 호출해줘야 UI 바인딩가능
                    OnPropertyChanged(nameof(IsTxtCommEnable));
                    OnPropertyChanged(nameof(IsTxtIpEnable));
                    OnPropertyChanged(nameof(IsTxtPortEnable));
                }
            }
        }

        public bool IsTxtCommEnable => IsCommMode;
        public bool IsTxtIpEnable => !IsCommMode;
        public bool IsTxtPortEnable => !IsCommMode;

        private string? _txtComm;
        private string? _txtIp;
        private string? _txtPort;
        public string TxtComm
        {
            get => _txtComm;
            set { if (value != _txtComm) { _txtComm = value; OnPropertyChanged(); } }
        }
        public string TxtIp
        {
            get => _txtIp;
            set { if (value != _txtComm) { _txtIp = value; OnPropertyChanged(); } }
        }
        public string TxtPort
        {
            get { return _txtPort; }
            set { if (value != _txtPort) { _txtPort = value; OnPropertyChanged(); } }
        }



        private bool CanConnect() => true;
        private void BcrConnectCommand()
        {

            if (RunInspect != null)
            {
                RunInspect.Dispose();  

            }
            var mode = IsCommMode ? BcrMode.ComPort : BcrMode.TcpIp;
            var ComName = Convert.ToString(_txtComm);

            var Ip = _txtIp;
            var Port = 5000;
            if (!string.IsNullOrWhiteSpace(_txtPort))
            {
                Port = Convert.ToInt16(_txtPort);
            }
            else
            {
                Port = 1;
            }

            var BcrSet = new BcrSettings(mode, ComName, Ip, Port);

            if (IsCommMode)
            {
                RunInspect = new Inspectproc(BcrSet);
            }
            else if (IsTcpIpMode)
            {
                RunInspect = new Inspectproc(BcrSet);
            }
        }

        private bool CanRead() => true;
        private void BcrReadCommand()
        {
            if (RunInspect != null)
            {
                RunInspect.OnTriger();
            }
            else
            {
                MessageBox.Show("Connect 필요");
            }

        }
    }
}
