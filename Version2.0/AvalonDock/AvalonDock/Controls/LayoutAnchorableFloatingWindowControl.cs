using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public class LayoutAnchorableFloatingWindowControl : LayoutFloatingWindowControl, ILayoutControl, IOverlayWindowHost
    {
        static LayoutAnchorableFloatingWindowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorableFloatingWindowControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorableFloatingWindowControl)));
        } 


        internal LayoutAnchorableFloatingWindowControl(LayoutAnchorableFloatingWindow model)
            :base(model)
        {
            _model = model;
        }

        LayoutAnchorableFloatingWindow _model;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var manager = _model.Root.Manager;

            Content = manager.GetUIElementForModel(_model.RootPanel);
            SetBinding(BackgroundProperty, new Binding("DataContext.Background") { Source = Content });
        }


        bool IOverlayWindowHost.HitTest(Point dragPoint)
        {
            Rect detectionRect = new Rect(new Point(Left, Top), new Size(Width, Height));
            return detectionRect.Contains(dragPoint);
        }

        OverlayWindow _overlayWindow = null;
        void CreateOverlayWindow()
        {
            if (_overlayWindow == null)
                _overlayWindow = new OverlayWindow(this);
            Rect rectWindow = new Rect(new Point(Left, Top), new Size(Width, Height));
            _overlayWindow.Left = rectWindow.Left;
            _overlayWindow.Top = rectWindow.Top;
            _overlayWindow.Width = rectWindow.Width;
            _overlayWindow.Height = rectWindow.Height;
        }

        IOverlayWindow IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
        {
            CreateOverlayWindow();
            _overlayWindow.Owner = draggingWindow;
            _overlayWindow.Show();

            return _overlayWindow;
        }

        void IOverlayWindowHost.HideOverlayWindow()
        {
            _dropAreas = null;
            _overlayWindow.Owner = null;
            _overlayWindow.Hide();
        }

        List<IDropArea> _dropAreas = null;
        IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
        {
            if (_dropAreas != null)
                return _dropAreas;

            _dropAreas = new List<IDropArea>();

            if (draggingWindow.Model is LayoutDocumentFloatingWindow)
                return _dropAreas;

            var rootVisual = (Content as FloatingWindowContentHost).RootVisual;

            foreach (var areaHost in rootVisual.FindVisualChildren<LayoutAnchorablePaneControl>())
            {
                _dropAreas.Add(new DropArea<LayoutAnchorablePaneControl>(
                    areaHost,
                    DropAreaType.AnchorablePane));
            }
            foreach (var areaHost in rootVisual.FindVisualChildren<LayoutDocumentPaneControl>())
            {
                _dropAreas.Add(new DropArea<LayoutDocumentPaneControl>(
                    areaHost,
                    DropAreaType.DocumentPane));
            }

            return _dropAreas;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!CloseInitiatedByUser)
            {
                var root = Model.Root;
                root.Manager.RemoveFloatingWindow(this);
                root.CollectGarbage();
            }


            base.OnClosed(e);
        }
    }
}
