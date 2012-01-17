using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
    [ContentProperty("RootPanel")]
    public class LayoutAnchorableFloatingWindow : LayoutFloatingWindow
    {
        public LayoutAnchorableFloatingWindow()
        { 
        
        }

        #region RootPanel

        private LayoutAnchorablePaneGroup _rootPanel = null;
        public LayoutAnchorablePaneGroup RootPanel
        {
            get { return _rootPanel; }
            set
            {
                if (_rootPanel != value)
                {
                    RaisePropertyChanging("RootPanel");
                    _rootPanel = value;
                    if (_rootPanel != null)
                        _rootPanel.Parent = this;
                    RaisePropertyChanged("RootPanel");
                }
            }
        }

        #endregion

        public override IEnumerable<ILayoutElement> Children
        {
            get { yield return RootPanel; }
        }
    }
}
