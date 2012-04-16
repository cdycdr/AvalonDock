using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    public enum ChildrenTreeChange
    {
        /// <summary>
        /// Direct insert/remove operation has been perfomed to the group
        /// </summary>
        DirectChildrenChanged,

        /// <summary>
        /// An element below in the hierarchy as been added/removed
        /// </summary>
        TreeChanged
    }

    public class ChildrenTreeChangedEventArgs : EventArgs
    {
        public ChildrenTreeChangedEventArgs(ChildrenTreeChange change)
        {
            Change = change;
        }

        public ChildrenTreeChange Change { get; private set; }
    }
}
