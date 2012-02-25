using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Layout
{
    [Serializable]
    public class LayoutDocument : LayoutContent
    {
        public bool IsVisible
        {
            get { return true; }
        }

        internal void Close()
        {
            var parentAsDocuPane = Parent as ILayoutDocumentPane;
            parentAsDocuPane.RemoveChild(this);
        }
    }
}
