using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.MVVMTestApp
{
    class PaneViewModel : ViewModelBase
    {
        public PaneViewModel()
        { }


        #region Title

        private string _title = null;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        #endregion

        public virtual Uri IconSource
        {
            get
            {
                return null;
            }
        }
    }
}
