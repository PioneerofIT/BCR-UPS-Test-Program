using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BCR_Reader_Pro.Service;
using GalaSoft.MvvmLight.Command;

namespace BCR_Reader_Pro.ViewModel
{
    internal class AutoRunViewModel
    {
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }



        private bool _canStatcondition = false;
        public bool CanStartCondition
        {
            get { return _canStatcondition; }
            set { _canStatcondition = value; }
        }

        public AutoRunViewModel()
        {
            StartCommand = new RelayCommand(StartSequnce);
            StopCommand = new RelayCommand(StopSequence);
        }

        private void StartSequnce()
        {

            SequenceManager.Instance.AllStartSeq(); 
            // 실제 시퀀스 시작 로직
            LogManager.Instance.Log($"Start Sequence");

        }

        private void StopSequence()
        {

            SequenceManager.Instance.AllStopSeq();
            // 실제 시퀀스 시작 로직
            LogManager.Instance.Log($"Stop Sequence");

        }

    }
}
