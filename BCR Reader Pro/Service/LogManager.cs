using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCR_Reader_Pro.Service
{
    public class LogManager
    {
        private static readonly Lazy<LogManager> _instance = new(() => new LogManager());
        public static LogManager Instance => _instance.Value;

        public ObservableCollection<string> LogMessages { get; } = new();

        public void Log(string message)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string fullMsg = $"[{time}] {message}";

            // UI 로그 리스트 추가
            App.Current.Dispatcher.Invoke(() => LogMessages.Insert(0, fullMsg));

           
        }
    }
}
