using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    public interface ILayoutRoot
    {
        DockingManager Manager { get; }

        LayoutAnchorSide TopSide { get; }
        LayoutAnchorSide LeftSide { get; }
        LayoutAnchorSide RightSide { get; }
        LayoutAnchorSide BottomSide { get; }

        LayoutContent ActiveContent { get; set; }

        void CollectGarbage();
    }
}
