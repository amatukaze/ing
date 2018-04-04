using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.UWP.Pipe
{
    class Worker : INotifyPropertyChanged
    {
        public int Port { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsListening { get; private set; }
        public bool IsConnected { get; private set; }
    }
}
