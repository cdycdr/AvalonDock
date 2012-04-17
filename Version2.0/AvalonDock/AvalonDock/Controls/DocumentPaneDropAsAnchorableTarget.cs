using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    internal class DocumentPaneDropAsAnchorableTarget : DropTarget<LayoutDocumentPaneControl>
    {
        internal DocumentPaneDropAsAnchorableTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect, DropTargetType type)
            : base(paneControl, detectionRect, type)
        {
            _targetPane = paneControl;
        }

        internal DocumentPaneDropAsAnchorableTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect, DropTargetType type, int tabIndex)
            : base(paneControl, detectionRect, type)
        {
            _targetPane = paneControl;
            _tabIndex = tabIndex;
        }


        LayoutDocumentPaneControl _targetPane;

        int _tabIndex = -1;

        protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
        {
            ILayoutDocumentPane targetModel = _targetPane.Model as ILayoutDocumentPane;

            switch (Type)
            {
                case DropTargetType.DocumentPaneDockAsAnchorableBottom:
                    #region DropTargetType.DocumentPaneDockAsAnchorableBottom
                    {
                        LayoutPanel parentGroupPanel = null;
                        var parentGroup = targetModel.Parent as LayoutDocumentPaneGroup;
                        if (parentGroup != null)
                            parentGroupPanel = parentGroup.Parent as LayoutPanel;
                        else
                            parentGroupPanel = targetModel.Parent as LayoutPanel;

                        if (parentGroupPanel != null &&
                            parentGroupPanel.Orientation == System.Windows.Controls.Orientation.Vertical)
                        {
                            parentGroupPanel.Children.Insert(
                                parentGroupPanel.IndexOfChild(parentGroup != null ? parentGroup : targetModel) + 1,
                                floatingWindow.RootPanel);
                        }
                        else if (parentGroupPanel != null)
                        {
                            var newParentPanel = new LayoutPanel() { Orientation = System.Windows.Controls.Orientation.Vertical };
                            parentGroupPanel.ReplaceChild(parentGroup != null ? parentGroup : targetModel, newParentPanel);
                            newParentPanel.Children.Add(parentGroup != null ? parentGroup : targetModel);
                            newParentPanel.Children.Add(floatingWindow.RootPanel);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockAsAnchorableTop:
                    #region DropTargetType.DocumentPaneDockAsAnchorableTop
                    {
                        LayoutPanel parentGroupPanel = null;
                        var parentGroup = targetModel.Parent as LayoutDocumentPaneGroup;
                        if (parentGroup != null)
                            parentGroupPanel = parentGroup.Parent as LayoutPanel;
                        else
                            parentGroupPanel = targetModel.Parent as LayoutPanel;

                        if (parentGroupPanel != null &&
                            parentGroupPanel.Orientation == System.Windows.Controls.Orientation.Vertical)
                        {
                            parentGroupPanel.Children.Insert(
                                parentGroupPanel.IndexOfChild(parentGroup != null ? parentGroup : targetModel),
                                floatingWindow.RootPanel);
                        }
                        else if (parentGroupPanel != null)
                        {
                            var newParentPanel = new LayoutPanel() { Orientation = System.Windows.Controls.Orientation.Vertical };
                            parentGroupPanel.ReplaceChild(parentGroup != null ? parentGroup : targetModel, newParentPanel);
                            newParentPanel.Children.Add(parentGroup != null ? parentGroup : targetModel);
                            newParentPanel.Children.Insert(0, floatingWindow.RootPanel);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockAsAnchorableLeft:
                    #region DropTargetType.DocumentPaneDockAsAnchorableLeft
                    {
                        LayoutPanel parentGroupPanel = null;
                        var parentGroup = targetModel.Parent as LayoutDocumentPaneGroup;
                        if (parentGroup != null)
                            parentGroupPanel = parentGroup.Parent as LayoutPanel;
                        else
                            parentGroupPanel = targetModel.Parent as LayoutPanel;

                        if (parentGroupPanel != null &&
                            parentGroupPanel.Orientation == System.Windows.Controls.Orientation.Horizontal)
                        {
                            parentGroupPanel.Children.Insert(
                                parentGroupPanel.IndexOfChild(parentGroup != null ? parentGroup : targetModel),
                                floatingWindow.RootPanel);
                        }
                        else if (parentGroupPanel != null)
                        {
                            var newParentPanel = new LayoutPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                            parentGroupPanel.ReplaceChild(parentGroup != null ? parentGroup : targetModel, newParentPanel);
                            newParentPanel.Children.Add(parentGroup != null ? parentGroup : targetModel);
                            newParentPanel.Children.Insert(0, floatingWindow.RootPanel);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockAsAnchorableRight:
                    #region DropTargetType.DocumentPaneDockAsAnchorableRight
                    {
                        LayoutPanel parentGroupPanel = null;
                        var parentGroup = targetModel.Parent as LayoutDocumentPaneGroup;
                        if (parentGroup != null)
                            parentGroupPanel = parentGroup.Parent as LayoutPanel;
                        else
                            parentGroupPanel = targetModel.Parent as LayoutPanel;

                        if (parentGroupPanel != null &&
                            parentGroupPanel.Orientation == System.Windows.Controls.Orientation.Horizontal)
                        {
                            parentGroupPanel.Children.Insert(
                                parentGroupPanel.IndexOfChild(parentGroup != null ? parentGroup : targetModel) + 1,
                                floatingWindow.RootPanel);
                        }
                        else if (parentGroupPanel != null)
                        {
                            var newParentPanel = new LayoutPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                            parentGroupPanel.ReplaceChild(parentGroup != null ? parentGroup : targetModel, newParentPanel);
                            newParentPanel.Children.Add(parentGroup != null ? parentGroup : targetModel);
                            newParentPanel.Children.Add(floatingWindow.RootPanel);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    break;
                    #endregion
            }

            base.Drop(floatingWindow);
        }

        public override System.Windows.Media.Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
        {
            Rect targetScreenRect;
            ILayoutDocumentPane targetModel = _targetPane.Model as ILayoutDocumentPane;
            var manager = targetModel.Root.Manager;

            if (targetModel.Parent is LayoutDocumentPaneGroup)
            {
                var parentGroup = targetModel.Parent as LayoutDocumentPaneGroup;
                var documentPaneGroupControl = manager.FindLogicalChildren<LayoutDocumentPaneGroupControl>().First(d => d.Model == parentGroup);
                targetScreenRect = documentPaneGroupControl.GetScreenArea();
            }
            else
            {
                var documentPaneControl = manager.FindLogicalChildren<LayoutDocumentPaneControl>().First(d => d.Model == targetModel);
                targetScreenRect = documentPaneControl.GetScreenArea();
            }

            switch (Type)
            {
                case DropTargetType.DocumentPaneDockAsAnchorableBottom:
                    {
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                        targetScreenRect.Offset(0.0, targetScreenRect.Height - targetScreenRect.Height / 3.0);
                        targetScreenRect.Height /= 3.0;
                        return new RectangleGeometry(targetScreenRect);
                    }
                case DropTargetType.DocumentPaneDockAsAnchorableTop:
                    {
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                        targetScreenRect.Height /= 3.0;
                        return new RectangleGeometry(targetScreenRect);
                    }
                case DropTargetType.DocumentPaneDockAsAnchorableRight:
                    {
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                        targetScreenRect.Offset(targetScreenRect.Width - targetScreenRect.Width / 3.0, 0.0);
                        targetScreenRect.Width /= 3.0;
                        return new RectangleGeometry(targetScreenRect);
                    }
                case DropTargetType.DocumentPaneDockAsAnchorableLeft:
                    {
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                        targetScreenRect.Width /= 3.0;
                        return new RectangleGeometry(targetScreenRect);
                    }
            }

            return null;
        }

    }
}
