﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AvalonDock.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutPanel : LayoutPositionableGroup<ILayoutPanelElement>, ILayoutPanelElement, ILayoutOrientableGroup
    {
        public LayoutPanel()
        {
            
        }

        public LayoutPanel(ILayoutPanelElement firstChild)
        {
            Children.Add(firstChild);
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
            return Children.Any(c => c.IsVisible);
        }

    }
}