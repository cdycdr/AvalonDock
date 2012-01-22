﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    public interface ILayoutContainer : ILayoutElement
    {
        IEnumerable<ILayoutElement> Children { get; }
        void RemoveChild(ILayoutElement element);
        int ChildrenCount { get; }
    }
}
