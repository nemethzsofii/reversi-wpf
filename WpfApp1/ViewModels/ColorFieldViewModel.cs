using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class ColorFieldViewModel : ViewModelBase
    {
        private Int32 _colorNumber; 
        public Int32 Row { get; set; }
        public Int32 Column { get; set; }
        public Int32 ColorNumber
        {
            get { return _colorNumber; }
            set
            {
                if (_colorNumber != value)
                {
                    _colorNumber = value;
                    OnPropertyChanged();
                }
            }
        }
        public DelegateCommand? FieldChangeCommand { get; set; }
    }
}
