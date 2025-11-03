using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCR_Reader_Pro.Service
{
    public class SequenceManager
    {
        private static readonly Lazy<SequenceManager> _instance = new(() => new SequenceManager());
        public static SequenceManager Instance => _instance.Value;
        List<BaseThread> _threads = new();

        private uPmRun _PmThread;
        public SequenceManager()
        {
            _threads.Clear();

        }

        public void CreateThread()
        {
            _PmThread = new uPmRun(0, 200);
            _threads.Add( _PmThread );
        }

        public void AllStopSeq()
        {
            foreach (var thread in _threads)
            {
                thread.Stop();
            }
        }
        public void AllStartSeq()
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }
        }

        public void ThreadEnd()
        {
            foreach (var thread in _threads)
            {
                thread.ThreadEnd();
            }
        }
    }


}
