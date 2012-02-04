using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AvalonDock.Controls
{
    interface IFocusControlOwner
    {
        IntPtr LastFocusedWindowHandle { get; }
        IInputElement LastFocusedElement { get; }
    }
}
