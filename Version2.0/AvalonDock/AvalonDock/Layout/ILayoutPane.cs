using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    public interface ILayoutPane : ILayoutContainer
    {
        void MoveChild(int oldIndex, int newIndex);

        void RemoveChildAt(int childIndex);

        void ComputeVisibility();
    }
}
