using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public class LayoutAnchorControl : Control, ILayoutControl
    {
        static LayoutAnchorControl()
        {
            Control.IsHitTestVisibleProperty.AddOwner(typeof(LayoutAnchorControl), new FrameworkPropertyMetadata(true)); 
        }


        internal LayoutAnchorControl(LayoutAnchorable model)
        {
            _model = model;
        }

        LayoutAnchorable _model;

        public ILayoutElement Model
        {
            get { return _model; }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (!e.Handled)
                _model.Root.Manager.OnShowAutoHideWindow(this);    
        }

    }
}
