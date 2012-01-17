using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public class LayoutDocumentTabItem : TabItem
    {
        public LayoutDocumentTabItem()
        {
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }

        internal LayoutContent GetModel()
        {
            return DataContext as LayoutContent;
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var parentPaneControl = this.FindAncestor<LayoutDocumentPaneControl>();
            if (e.OldValue != null && parentPaneControl != null)
            {
                parentPaneControl.InternalRemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null && parentPaneControl != null)
            {
                parentPaneControl.InternalAddLogicalChild(e.NewValue);
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            var contentModel = GetModel();

            if (oldParent != null && contentModel != null)
            {
                var oldParentPaneControl = oldParent.FindAncestor<LayoutDocumentPaneControl>();
                if (oldParentPaneControl != null)
                {
                    oldParentPaneControl.InternalRemoveLogicalChild(contentModel.Content);
                }
            }

            if (contentModel.Content != null)
            {
                var oldLogicalParentPaneControl = LogicalTreeHelper.GetParent(contentModel.Content as DependencyObject)
                    as LayoutDocumentPaneControl;
                if (oldLogicalParentPaneControl != null)
                    oldLogicalParentPaneControl.InternalRemoveLogicalChild(contentModel.Content);
            }
            if (contentModel.Content != null)
            {
                var oldLogicalParentPaneControl = LogicalTreeHelper.GetParent(contentModel.Content as DependencyObject)
                    as LayoutAnchorablePaneControl;
                if (oldLogicalParentPaneControl != null)
                    oldLogicalParentPaneControl.InternalRemoveLogicalChild(contentModel.Content);
            }

            var parentPaneControl = this.FindAncestor<LayoutDocumentPaneControl>();
            if (contentModel != null && parentPaneControl != null && contentModel.Content != null)
            {
                parentPaneControl.InternalAddLogicalChild(contentModel.Content);
            }
        }

        bool _isMouseDown = false;
        static LayoutDocumentTabItem _draggingItem = null;

        internal static bool IsDraggingItem()
        {
            return _draggingItem != null;
        }

        internal static LayoutDocumentTabItem GetDraggingItem()
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
