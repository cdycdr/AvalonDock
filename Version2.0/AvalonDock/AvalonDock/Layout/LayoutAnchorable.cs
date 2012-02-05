using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AvalonDock.Layout
{
    [Serializable]
    public class LayoutAnchorable : LayoutContent
    {
        #region IsVisible

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    RaisePropertyChanging("IsVisible");
                    _isVisible = value;
                    UpdateParentVisibility();
                    RaisePropertyChanged("IsVisible");
                }
            }
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            UpdateParentVisibility(); 
            base.OnParentChanged(oldValue, newValue);
        }

        void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutAnchorablePane;
            if (parentPane != null)
                parentPane.ComputeVisibility();
        }

        #endregion

        #region AutoHideWidth

        private double _autohideWidth = 0.0;
        public double AutoHideWidth
        {
            get { return _autohideWidth; }
            set
            {
                if (_autohideWidth != value)
                {
                    RaisePropertyChanging("AutoHideWidth");
                    _autohideWidth = value;
                    RaisePropertyChanged("AutoHideWidth");
                }
            }
        }

        #endregion

        #region AutoHideMinWidth

        private double _autohideMinWidth = 25.0;
        public double AutoHideMinWidth
        {
            get { return _autohideMinWidth; }
            set
            {
                if (_autohideMinWidth != value)
                {
                    RaisePropertyChanging("AutoHideMinWidth");
                    _autohideMinWidth = value;
                    RaisePropertyChanged("AutoHideMinWidth");
                }
            }
        }

        #endregion

        #region AutoHideHeight

        private double _autohideHeight = 0.0;
        public double AutoHideHeight
        {
            get { return _autohideHeight; }
            set
            {
                if (_autohideHeight != value)
                {
                    RaisePropertyChanging("AutoHideHeight");
                    _autohideHeight = value;
                    RaisePropertyChanged("AutoHideHeight");
                }
            }
        }

        #endregion

        #region AutoHideMinHeight

        private double _autohideMinHeight = 0.0;
        public double AutoHideMinHeight
        {
            get { return _autohideMinHeight; }
            set
            {
                if (_autohideMinHeight != value)
                {
                    RaisePropertyChanging("AutoHideMinHeight");
                    _autohideMinHeight = value;
                    RaisePropertyChanged("AutoHideMinHeight");
                }
            }
        }

        #endregion
       
    }
}
