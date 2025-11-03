using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BCR_Reader_Pro.Service;

namespace BCR_Reader_Pro.Model
{
    internal class IOItemModel : INotifyPropertyChanged
    {
        private bool _state;

        public int Index { get; set; }        
        public string Name { get; set; } = "";
        public bool State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();

                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
           
    }
}
