using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    public interface ILayoutGroup
    {
        int IndexOfChild(ILayoutElement element);
        void InsertChildAt(int index, ILayoutElement element);
        void RemoveChildAt(int index);
        void RemoveChild(ILayoutElement element);
        int ChildrenCount { get;  }
        void ReplaceChildAt(int index, ILayoutElement element);
    }
}
