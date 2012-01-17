using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public class AnchorablePaneTitle : ContentControl
    {
        static AnchorablePaneTitle()
        {
            IsHitTestVisibleProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(true));
        }


        internal AnchorablePaneTitle()
        { }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _isMouseDown = false;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                var paneModel = this.FindAncestor<LayoutAnchorablePaneControl>().Model as LayoutAnchorablePane;
                var manager = paneModel.Root.Manager;

                manager.StartDraggingFloatingWindowForPane(paneModel);                
            }

            _isMouseDown = false;
        }

        bool _isMouseDown = false;
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            base.OnMouseLeftButtonUp(e);
        }
    }
}
