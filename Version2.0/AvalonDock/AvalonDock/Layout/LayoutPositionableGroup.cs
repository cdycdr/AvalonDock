using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AvalonDock.Layout
{
    [Serializable]
    public abstract class LayoutPositionableGroup<T> : LayoutGroup<T>, ILayoutPositionableElement, ILayoutPositionableElementWithActualSize where T : class, ILayoutElement
    {
        public LayoutPositionableGroup()
        { }

        GridLength _dockWidth = new GridLength(1.0, GridUnitType.Star);
        public GridLength DockWidth
        {
            get
            {
                return _dockWidth;
            }
            set
            {
                if (DockWidth != value)
                {
                    RaisePropertyChanging("DockWidth");
                    _dockWidth = value;
                    RaisePropertyChanged("DockWidth");
                }
            }
        }

        GridLength _dockHeight = new GridLength(1.0, GridUnitType.Star);
        public GridLength DockHeight
        {
            get
            {
                return _dockHeight;
            }
            set
            {
                if (DockHeight != value)
                {
                    RaisePropertyChanging("DockHeight");
                    _dockHeight = value;
                    RaisePropertyChanged("DockHeight");
                }
            }
        }


        #region DockMinWidth

        private double _dockMinWidth = 25.0;
        public double DockMinWidth
        {
            get { return _dockMinWidth; }
            set
            {
                if (_dockMinWidth != value)
                {
                    MathHelper.AssertIsPositiveOrZero(value);
                    RaisePropertyChanging("DockMinWidth");
                    _dockMinWidth = value;
                    RaisePropertyChanged("DockMinWidth");
                }
            }
        }

        #endregion

        #region DockMinHeight

        private double _dockMinHeight = 25.0;
        public double DockMinHeight
        {
            get { return _dockMinHeight; }
            set
            {
                if (_dockMinHeight != value)
                {
                    MathHelper.AssertIsPositiveOrZero(value);
                    RaisePropertyChanging("DockMinHeight");
                    _dockMinHeight = value;
                    RaisePropertyChanged("DockMinHeight");
                }
            }
        }

        #endregion

        #region FloatingWidth

        private double _floatingWidth = 0.0;
        public double FloatingWidth
        {
            get { return _floatingWidth; }
            set
            {
                if (_floatingWidth != value)
                {
                    RaisePropertyChanging("FloatingWidth");
                    _floatingWidth = value;
                    RaisePropertyChanged("FloatingWidth");
                }
            }
        }

        #endregion

        #region FloatingHeight

        private double _floatingHeight = 0.0;
        public double FloatingHeight
        {
            get { return _floatingHeight; }
            set
            {
                if (_floatingHeight != value)
                {
                    RaisePropertyChanging("FloatingHeight");
                    _floatingHeight = value;
                    RaisePropertyChanged("FloatingHeight");
                }
            }
        }

        #endregion

        #region FloatingLeft

        private double _floatingLeft = 0.0;
        public double FloatingLeft
        {
            get { return _floatingLeft; }
            set
            {
                if (_floatingLeft != value)
                {
                    RaisePropertyChanging("FloatingLeft");
                    _floatingLeft = value;
                    RaisePropertyChanged("FloatingLeft");
                }
            }
        }

        #endregion

        #region FloatingTop

        private double _floatingTop = 0.0;
        public double FloatingTop
        {
            get { return _floatingTop; }
            set
            {
                if (_floatingTop != value)
                {
                    RaisePropertyChanging("FloatingTop");
                    _floatingTop = value;
                    RaisePropertyChanged("FloatingTop");
                }
            }
        }

        #endregion

        [NonSerialized]
        double _actualWidth;
        double ILayoutPositionableElementWithActualSize.ActualWidth
        {
            get
            {
                return _actualWidth;
            }
            set
            {
                _actualWidth = value;
            }
        }
        
        [NonSerialized]
        double _actualHeight;
        double ILayoutPositionableElementWithActualSize.ActualHeight
        {
            get
            {
                return _actualHeight;
            }
            set
            {
                _actualHeight = value;
            }
        }
    }
}
