using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicCore
{
    public class PresentedItem : INotifyPropertyChanged
    {
        public string currentName { get; set; }
        public string newName { get; set; }
        public string path { get; set; }    
        public string error { get; set; }       
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
