using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
    [ContentProperty("Children")]
    public class LayoutAnchorGroup : LayoutGroup<LayoutAnchorable>
    {
        public LayoutAnchorGroup()
        {
        }

        protected override bool GetVisibility()
        {
            return Children.Count > 0;
        }
        
    }
}
