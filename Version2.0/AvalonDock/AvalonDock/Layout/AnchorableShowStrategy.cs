using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    public enum AnchorableShowStrategy : uint
    {
        Most  = 0x0001,
        Left  = 0x0002,
        Right = 0x0004,
        Top   = 0x0010,
        Bottom= 0x0020,
    }
}
