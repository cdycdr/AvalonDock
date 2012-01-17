using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AvalonDock.Layout
{
    public interface ILayoutOrientableElement : ILayoutPanelElement
    {
        Orientation Orientation { get; set; }
    }
}
