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

    [TemplatePart(Name = "PART_MenuPin", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_AutoHidePin", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ClosePin", Type = typeof(FrameworkElement))]
    public class AnchorablePaneTitle : ContentControl
    {
        static AnchorablePaneTitle()
        {
            AnchorablePaneTitle.IsHitTestVisibleProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(true));
            AnchorablePaneTitle.DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(typeof(AnchorablePaneTitle)));
        }


        internal AnchorablePaneTitle()
        { }


        Border _menuPinContainer = null;
        Border _menuAutoHideContainer = null;
        Border _menuCloseContainer = null;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _menuPinContainer = this.GetTemplateChild("PART_MenuPin") as Border;
            _menuAutoHideContainer = this.GetTemplateChild("PART_AutoHidePin") as Border;
            _menuCloseContainer = this.GetTemplateChild("PART_ClosePin") as Border;

            if (_menuAutoHideContainer != null)
                _menuAutoHideContainer.MouseLeftButtonUp += (s, e) => OnToggleAutoHide();
        }

        LayoutAnchorable GetModel()
        {
            return DataContext as LayoutAnchorable;
        }

        private void OnToggleAutoHide()
        {
            var anchorableModel = GetModel();
            var manager = anchorableModel.Root.Manager;

            manager.ToggleAutoHide(anchorableModel);
        }

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
