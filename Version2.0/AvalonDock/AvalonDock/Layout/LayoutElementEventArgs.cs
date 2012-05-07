using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    public class LayoutElementEventArgs : EventArgs
    {
        public LayoutElementEventArgs(LayoutElement element)
        {
            Element = element;
        }


        public LayoutElement Element
        {
            get;
            private set;
        }
    }
}
