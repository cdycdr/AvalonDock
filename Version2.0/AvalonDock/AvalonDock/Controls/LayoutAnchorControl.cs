using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using System.Windows.Threading;

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

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            var contentModel = _model;

            if (oldParent != null && contentModel != null && contentModel.Content is DependencyObject)
            {
                var oldParentPaneControl = oldParent.FindVisualAncestor<LayoutAnchorablePaneControl>();
                if (oldParentPaneControl != null)
                {
                    ((ILogicalChildrenContainer)oldParentPaneControl).InternalRemoveLogicalChild(contentModel.Content);
                }
            }

            if (contentModel.Content != null && contentModel.Content is DependencyObject)
            {
                var oldLogicalParentPaneControl = LogicalTreeHelper.GetParent(contentModel.Content as DependencyObject)
                    as ILogicalChildrenContainer;
                if (oldLogicalParentPaneControl != null)
                    oldLogicalParentPaneControl.InternalRemoveLogicalChild(contentModel.Content);
            }

            if (contentModel != null && contentModel.Content != null && contentModel.Root != null && contentModel.Content is DependencyObject)
            {
                ((ILogicalChildrenContainer)contentModel.Root.Manager).InternalAddLogicalChild(contentModel.Content);
            }
        }


        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (!e.Handled)
                _model.Root.Manager.ShowAutoHideWindow(this);    
        }


        DispatcherTimer _openUpTimer = null;

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!e.Handled)
            {
                _openUpTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                _openUpTimer.Interval = TimeSpan.FromMilliseconds(400);
                _openUpTimer.Tick += new EventHandler(_openUpTimer_Tick);
                _openUpTimer.Start();
            }
        }

        void _openUpTimer_Tick(object sender, EventArgs e)
        {
            _openUpTimer.Tick -= new EventHandler(_openUpTimer_Tick);
            _openUpTimer.Stop();
            _openUpTimer = null;
            _model.Root.Manager.ShowAutoHideWindow(this);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (_openUpTimer != null)
            {
                _openUpTimer.Tick -= new EventHandler(_openUpTimer_Tick);
                _openUpTimer.Stop();
                _openUpTimer = null;
            }
            base.OnMouseLeave(e);
        }

    }
}
