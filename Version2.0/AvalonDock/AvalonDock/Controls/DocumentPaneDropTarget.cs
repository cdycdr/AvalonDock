﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    internal class DocumentPaneDropTarget : DropTarget<LayoutDocumentPaneControl>
    {
        internal DocumentPaneDropTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect, DropTargetType type)
            : base(paneControl, detectionRect, type)
        {
            _targetPane = paneControl;
        }

        internal DocumentPaneDropTarget(LayoutDocumentPaneControl paneControl, Rect detectionRect, DropTargetType type, int tabIndex)
            : base(paneControl, detectionRect, type)
        {
            _targetPane = paneControl;
            _tabIndex = tabIndex;
        }


        LayoutDocumentPaneControl _targetPane;

        int _tabIndex = -1;

        protected override void Drop(LayoutDocumentFloatingWindow floatingWindow)
        {
            ILayoutPane targetModel = _targetPane.Model as ILayoutPane;
            
            switch (Type)
            {
                case DropTargetType.DocumentPaneDockBottom:
                    #region DropTargetType.DocumentPaneDockBottom
                    {
                        var parentModel = targetModel.Parent as LayoutDocumentPaneGroup;

                        if (parentModel == null)
                        {
                            parentModel = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Vertical};
                            parentModel.Children.Add(targetModel as LayoutDocumentPane);
                            parentModel.Children.Add(new LayoutDocumentPane(floatingWindow.RootDocument));
                        }
                        else
                        {
                            //Debug.Assert(parentModel.Orientation == System.Windows.Controls.Orientation.Vertical);
                            parentModel.Orientation = System.Windows.Controls.Orientation.Vertical;
                            int targetPaneIndex = parentModel.IndexOfChild(targetModel);
                            parentModel.Children.Insert(targetPaneIndex + 1, new LayoutDocumentPane(floatingWindow.RootDocument));
                        }
                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockTop:
                    #region DropTargetType.DocumentPaneDockTop
                    {
                        var parentModel = targetModel.Parent as LayoutDocumentPaneGroup;

                        if (parentModel == null)
                        {
                            parentModel = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Vertical };
                            parentModel.Children.Add(new LayoutDocumentPane(floatingWindow.RootDocument));
                            parentModel.Children.Add(targetModel as LayoutDocumentPane);
                        }
                        else
                        {
                            parentModel.Orientation = System.Windows.Controls.Orientation.Vertical;
                            int targetPaneIndex = parentModel.IndexOfChild(targetModel);
                            parentModel.Children.Insert(targetPaneIndex, new LayoutDocumentPane(floatingWindow.RootDocument));
                        }
                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockLeft:
                    #region DropTargetType.DocumentPaneDockLeft
                    {
                        var parentModel = targetModel.Parent as LayoutDocumentPaneGroup;

                        if (parentModel == null)
                        {
                            parentModel = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                            parentModel.Children.Add(new LayoutDocumentPane(floatingWindow.RootDocument));
                            parentModel.Children.Add(targetModel as LayoutDocumentPane);
                        }
                        else
                        {
                            parentModel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                            int targetPaneIndex = parentModel.IndexOfChild(targetModel);
                            parentModel.Children.Insert(targetPaneIndex, new LayoutDocumentPane(floatingWindow.RootDocument));
                        }
                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockRight:
                    #region DropTargetType.DocumentPaneDockRight
                    {
                        var parentModel = targetModel.Parent as LayoutDocumentPaneGroup;

                        if (parentModel == null)
                        {
                            parentModel = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                            parentModel.Children.Add(targetModel as LayoutDocumentPane);
                            parentModel.Children.Add(new LayoutDocumentPane(floatingWindow.RootDocument));
                        }
                        else
                        {
                            parentModel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                            int targetPaneIndex = parentModel.IndexOfChild(targetModel);
                            parentModel.Children.Insert(targetPaneIndex + 1, new LayoutDocumentPane(floatingWindow.RootDocument));
                        }
                    }
                    break;
                    #endregion

                case DropTargetType.DocumentPaneDockInside:
                    #region DropTargetType.DocumentPaneDockInside
                    {
                        var paneModel = targetModel as LayoutDocumentPane;
                        var sourceModel = floatingWindow.RootDocument;

                        int i = _tabIndex == -1 ? 0 : _tabIndex;
                        paneModel.Children.Insert(i, sourceModel);
                    }
                    break;
                    #endregion


            }
            base.Drop(floatingWindow);
        }

        protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
        {
            ILayoutPane targetModel = _targetPane.Model as ILayoutPane;

            switch (Type)
            {
                case DropTargetType.DocumentPaneDockBottom:
                    #region DropTargetType.DocumentPaneDockBottom
                    {
                        var parentModel = targetModel.Parent as LayoutDocumentPaneGroup;
                        var newLayoutDocumentPane = new LayoutDocumentPane();

                        if (parentModel == null)
                        {
                            parentModel = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Vertical };
                            parentModel.Children.Add(targetModel as LayoutDocumentPane);
                            parentModel.Children.Add(newLayoutDocumentPane);
                        }
                        else
                        {
                            //Debug.Assert(parentModel.Orientation == System.Windows.Controls.Orientation.Vertical);
                            parentModel.Orientation = System.Windows.Controls.Orientation.Vertical;
                            int targetPaneIndex = parentModel.IndexOfChild(targetModel);
                            parentModel.Children.Insert(targetPaneIndex + 1, newLayoutDocumentPane);
                        }

                        foreach (var cntToTransfer in floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray())
                            newLayoutDocumentPane.Children.Add(cntToTransfer);

                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockTop:
                    #region DropTargetType.DocumentPaneDockTop
                    {
                        var parentModel = targetModel.Parent as LayoutDocumentPaneGroup;
                        var newLayoutDocumentPane = new LayoutDocumentPane();

                        if (parentModel == null)
                        {
                            parentModel = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Vertical };
                            parentModel.Children.Add(newLayoutDocumentPane);
                            parentModel.Children.Add(targetModel as LayoutDocumentPane);
                        }
                        else
                        {
                            //Debug.Assert(parentModel.Orientation == System.Windows.Controls.Orientation.Vertical);
                            parentModel.Orientation = System.Windows.Controls.Orientation.Vertical;
                            int targetPaneIndex = parentModel.IndexOfChild(targetModel);
                            parentModel.Children.Insert(targetPaneIndex, newLayoutDocumentPane);
                        }

                        foreach (var cntToTransfer in floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray())
                            newLayoutDocumentPane.Children.Add(cntToTransfer);

                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockLeft:
                    #region DropTargetType.DocumentPaneDockLeft
                    {
                        var parentModel = targetModel.Parent as LayoutDocumentPaneGroup;
                        var newLayoutDocumentPane = new LayoutDocumentPane();

                        if (parentModel == null)
                        {
                            parentModel = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                            parentModel.Children.Add(newLayoutDocumentPane);
                            parentModel.Children.Add(targetModel as LayoutDocumentPane);
                        }
                        else
                        {
                            parentModel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                            int targetPaneIndex = parentModel.IndexOfChild(targetModel);
                            parentModel.Children.Insert(targetPaneIndex, newLayoutDocumentPane);
                        }

                        foreach (var cntToTransfer in floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray())
                            newLayoutDocumentPane.Children.Add(cntToTransfer);

                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockRight:
                    #region DropTargetType.DocumentPaneDockRight
                    {
                        var parentModel = targetModel.Parent as LayoutDocumentPaneGroup;
                        var newLayoutDocumentPane = new LayoutDocumentPane();

                        if (parentModel == null)
                        {
                            parentModel = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                            parentModel.Children.Add(targetModel as LayoutDocumentPane);
                            parentModel.Children.Add(newLayoutDocumentPane);
                        }
                        else
                        {
                            parentModel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                            int targetPaneIndex = parentModel.IndexOfChild(targetModel);
                            parentModel.Children.Insert(targetPaneIndex + 1, newLayoutDocumentPane);
                        }

                        foreach (var cntToTransfer in floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray())
                            newLayoutDocumentPane.Children.Add(cntToTransfer);

                    }
                    break;
                    #endregion
                case DropTargetType.DocumentPaneDockInside:
                    #region DropTargetType.DocumentPaneDockInside
                    {
                        var paneModel = targetModel as LayoutDocumentPane;
                        var layoutAnchorablePaneGroup = floatingWindow.RootPanel as LayoutAnchorablePaneGroup;

                        int i = _tabIndex == -1 ? 0 : _tabIndex;
                        foreach (var anchorableToImport in layoutAnchorablePaneGroup.Descendents().OfType<LayoutAnchorable>().ToArray())
                        {
                            paneModel.Children.Insert(i, anchorableToImport);
                            i++;
                        }
                    }
                    break;
                    #endregion
            }

            base.Drop(floatingWindow);
        }

        public override System.Windows.Media.Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
        {
            switch (Type)
            {
                case DropTargetType.DocumentPaneDockInside:
                    {
                        var targetScreenRect = TargetElement.GetScreenArea();
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);

                        if (_tabIndex == -1)
                        {
                            return new RectangleGeometry(targetScreenRect);
                        }
                        else
                        {
                            var translatedDetectionRect = new Rect(DetectionRects[0].TopLeft, DetectionRects[0].BottomRight);
                            translatedDetectionRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                            
                            var pathFigure = new PathFigure();
                            pathFigure.StartPoint = targetScreenRect.BottomRight;
                            pathFigure.Segments.Add(new LineSegment() { Point = new Point(targetScreenRect.Right, translatedDetectionRect.Bottom) });
                            pathFigure.Segments.Add(new LineSegment() { Point = translatedDetectionRect.BottomRight });
                            pathFigure.Segments.Add(new LineSegment() { Point = translatedDetectionRect.TopRight });
                            pathFigure.Segments.Add(new LineSegment() { Point = translatedDetectionRect.TopLeft });
                            pathFigure.Segments.Add(new LineSegment() { Point = translatedDetectionRect.BottomLeft });
                            pathFigure.Segments.Add(new LineSegment() { Point = new Point(targetScreenRect.Left, translatedDetectionRect.Bottom) });
                            pathFigure.Segments.Add(new LineSegment() { Point = targetScreenRect.BottomLeft });
                            pathFigure.IsClosed = true;
                            pathFigure.IsFilled = true;
                            pathFigure.Freeze();
                            return new PathGeometry(new PathFigure[] { pathFigure });
                        }
                    }
                case DropTargetType.DocumentPaneDockBottom:
                    {
                        var targetScreenRect = TargetElement.GetScreenArea();
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                        targetScreenRect.Offset(0.0, targetScreenRect.Height / 2.0);
                        targetScreenRect.Height /= 2.0;
                        return new RectangleGeometry(targetScreenRect);
                    }
                case DropTargetType.DocumentPaneDockTop:
                    {
                        var targetScreenRect = TargetElement.GetScreenArea();
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                        targetScreenRect.Height /= 2.0;
                        return new RectangleGeometry(targetScreenRect);
                    }
                case DropTargetType.DocumentPaneDockLeft:
                    {
                        var targetScreenRect = TargetElement.GetScreenArea();
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                        targetScreenRect.Width /= 2.0;
                        return new RectangleGeometry(targetScreenRect);
                    }
                case DropTargetType.DocumentPaneDockRight:
                    {
                        var targetScreenRect = TargetElement.GetScreenArea();
                        targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                        targetScreenRect.Offset(targetScreenRect.Width / 2.0, 0.0);
                        targetScreenRect.Width /= 2.0;
                        return new RectangleGeometry(targetScreenRect);
                    }
            }

            return null;
        }

    }
}
