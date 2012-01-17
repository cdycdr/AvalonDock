using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
    [ContentProperty("Children")]
    public class LayoutDocumentPaneGroup : LayoutPositionableGroup<ILayoutDocumentPane>, ILayoutDocumentPane, ILayoutOrientableElement
    {
        public LayoutDocumentPaneGroup()
        {
        }

        #region Orientation

        private Orientation _orientation;
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation != value)
                {
                    RaisePropertyChanging("Orientation");
                    _orientation = value;
                    RaisePropertyChanged("Orientation");
                }
            }
        }

        #endregion

        protected override bool GetVisibility()
        {
            return true;// Children.Any(c => c.IsVisible);
        }
    }
}
