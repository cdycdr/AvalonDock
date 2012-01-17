using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
    [ContentProperty("RootDocument")]
    public class LayoutDocumentFloatingWindow : LayoutFloatingWindow
    {
        public LayoutDocumentFloatingWindow()
        {

        }

        #region RootDocument

        private LayoutDocument _rootDocument = null;
        public LayoutDocument RootDocument
        {
            get { return _rootDocument; }
            set
            {
                if (_rootDocument != value)
                {
                    RaisePropertyChanging("RootDocument");
                    _rootDocument = value;
                    if (_rootDocument != null)
                        _rootDocument.Parent = this;
                    RaisePropertyChanged("RootDocument");
                }
            }
        }

        #endregion

        public override IEnumerable<ILayoutElement> Children
        {
            get { yield return RootDocument; }
        }
    }

}
