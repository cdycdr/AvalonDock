using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AvalonDock.Controls
{
    public class ContextMenuEx : ContextMenu
    {
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            //return base.GetContainerForItemOverride();
            return new MenuItemEx();
        }
    }
}
