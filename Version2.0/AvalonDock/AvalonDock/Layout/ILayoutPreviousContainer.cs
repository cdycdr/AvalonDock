﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    public interface ILayoutPreviousContainer
    {
        ILayoutContainer PreviousContainer { get; }
    }
}
