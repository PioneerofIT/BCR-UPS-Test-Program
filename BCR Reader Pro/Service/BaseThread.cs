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
        private bool _isRunning;
        private int _intervalMs;
    }
}
