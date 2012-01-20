using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public class LayoutAnchorableTabItem : TabItem
    {
        public LayoutAnchorableTabItem()
        {
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }

        internal LayoutContent GetModel()
        {
            return DataContext as LayoutContent;
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var parentPaneControl = this.FindAncestor<LayoutAnchorablePaneControl>();
            var oldModel = e.OldValue as LayoutContent;
            var newModel = e.NewValue as LayoutContent;
            if (oldModel != null && parentPaneControl != null)
            {
                ((ILogicalChildrenContainer)parentPaneControl).InternalRemoveLogicalChild(oldModel);
            }

            if (newModel != null && parentPaneControl != null)
            {
                ((ILogicalChildrenContainer)parentPaneControl).InternalAddLogicalChild(newModel);
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            var contentModel = GetModel();

            if (contentModel == null)
                return;

            if (oldParent != null && contentModel != null)
            {
                var oldParentPaneControl = oldParent.FindAncestor<LayoutAnchorablePaneControl>();
                if (oldParentPaneControl != null)
                {
                    ((ILogicalChildrenContainer) oldParentPaneControl).InternalRemoveLogicalChild(contentModel.Content);
                }
            }

            if (contentModel.Content != null)
            {
                var oldLogicalParentPaneControl = LogicalTreeHelper.GetParent(contentModel.Content as DependencyObject)
                    as ILogicalChildrenContainer;
                if (oldLogicalParentPaneControl != null)
                    oldLogicalParentPaneControl.InternalRemoveLogicalChild(contentModel.Content);
            }

            var parentPaneControl = this.FindAncestor<LayoutAnchorablePaneControl>();
            if (contentModel != null && parentPaneControl != null && contentModel.Content != null)
            {
                ((ILogicalChildrenContainer)parentPaneControl).InternalAddLogicalChild(contentModel.Content);
            }
        }

        bool _isMouseDown = false;
        static LayoutAnchorableTabItem _draggingItem = null;

        internal static bool IsDraggingItem()
        {
            return _draggingItem != null;
        }

        internal static LayoutAnchorableTabItem GetDraggingItem()
        {
            return _draggingItem;
        }
        internal static void ResetDraggingItem()
        {
            _draggingItem = null;
        }



        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _isMouseDown = true;
            _draggingItem = this;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _isMouseDown = false;
                _draggingItem = null;
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMouseDown = false;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                _draggingItem = this;
            }

            _isMouseDown = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (_draggingItem != null &&
                _draggingItem != this &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                Console.WriteLine("Dragging item from {0} to {1}", _draggingItem, this);

                var model = GetModel();
                var container = model.Parent as ILayoutContainer;
                var containerPane = model.Parent as ILayoutPane;
                var childrenList = container.Children.ToList();
                containerPane.MoveChild(childrenList.IndexOf(_draggingItem.GetModel()), childrenList.IndexOf(model));
            }
        }

        public override string ToString()
        {
            return string.Format("TabItem({0})", GetModel().Title);
            //return base.ToString();
        }


    }
}
