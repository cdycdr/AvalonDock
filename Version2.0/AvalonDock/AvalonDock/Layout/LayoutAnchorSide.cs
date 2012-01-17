using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using AvalonDock.Controls;

namespace AvalonDock.Layout
{
    [ContentProperty("Children")]
    public class LayoutAnchorSide : LayoutGroup<LayoutAnchorGroup>
    {
        public LayoutAnchorSide()
        {
        }

        protected override bool GetVisibility()
        {
            return Children.Count > 0;
        }


        protected override void OnParentChanged()
        {
            base.OnParentChanged();

            UpdateSide();
        }

        private void UpdateSide()
        {
            if (Root.LeftSide == this)
                Side = AnchorSide.Left;
            else if (Root.TopSide == this)
                Side = AnchorSide.Top;
            else if (Root.RightSide == this)
                Side = AnchorSide.Right;
            else if (Root.BottomSide == this)
                Side = AnchorSide.Bottom;
        }


        #region Side

        private AnchorSide _side;
        public AnchorSide Side
        {
            get { return _side; }
            private set
            {
                if (_side != value)
                {
                    RaisePropertyChanging("Side");
                    _side = value;
                    RaisePropertyChanged("Side");
                }
            }
        }

        #endregion


        
    }
}
