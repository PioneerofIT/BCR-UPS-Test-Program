using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using BCR_Reader_Pro.Model;
using BCR_Reader_Pro.Service;
using GalaSoft.MvvmLight.Command;
using VSP.CONTROLLER;

namespace BCR_Reader_Pro.ViewModel
{
    internal class IOTestViewModel
    {
        public ObservableCollection<IOItemModel> Inputs { get; } = new();
        public ObservableCollection<IOItemModel> Outputs { get; } = new();

        private readonly DispatcherTimer _ioUpdateTimer;

        public ICommand OutPutCommand { get; }


        public IOTestViewModel()
        {
            AssingIO();
            OutPutCommand = new RelayCommand<int>(OutOnOff);

            _ioUpdateTimer = new DispatcherTimer();
            _ioUpdateTimer.Interval = TimeSpan.FromMilliseconds(200);
            _ioUpdateTimer.Tick += UpdateInputStates;
            _ioUpdateTimer.Start();

        }

        private void AssingIO()
        {
            Inputs.Clear();
            Outputs.Clear();

            int inPutSize = (int)DigitalInput.MAX_DI;
            int outPutSize = (int)DigitalOutput.MAX_DO;

            for (int i = 0; i < inPutSize; i++)
            {
                Inputs.Add(new IOItemModel { Index = i, Name = $"X{i:000}", State = false });
            
            }

            for (int i = 0; i < outPutSize; i++)
            {
                Outputs.Add(new IOItemModel { Index = i, Name = $"Y{i:000}", State = false });

            }

        }

        private void OutOnOff(int index)
        {
            CDO outBit = new CDO((ushort)index);
            bool newState = !Outputs[index].State;

            if (newState)
            {
                outBit.On();
                LogManager.Instance.Log($"Y{index:000}" + "Sol On");
            }

            else
            {
                outBit.Off();
                LogManager.Instance.Log($"Y{index:000}" + "Sol Off");
            }
                

            Outputs[index].State = newState;
        }

        private void UpdateInputStates(object? sender, EventArgs e)
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                var input = new CDI((ushort)i);
                bool newState = input.IsOn();
                if (Inputs[i].State != newState)
                {
                    Inputs[i].State = newState;
                    
                    if(newState)
                        LogManager.Instance.Log($"X{i:000}" + "On");
                    else
                        LogManager.Instance.Log($"X{i:000}" + "Off");

                }
                    
            }
        }

    }
}
