using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.MVVMTestApp
{
    class ToolViewModel : ViewModelBase
    {
        public ToolViewModel(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }


        #region IsVisible

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }

        #endregion


    }
}
