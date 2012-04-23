using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls;

namespace AvalonDock.Layout
{
    [Serializable]
    public class LayoutAnchorable : LayoutContent
    {
        #region IsVisible
        [XmlIgnore]
        public bool IsVisible
        {
            get
            {
                return Parent != null && !(Parent is LayoutRoot);
            }
            set
            {
                if (value)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        [XmlIgnore]
        public bool IsHidden
        {
            get
            {
                return (Parent is LayoutRoot);
            }
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            UpdateParentVisibility();
            RaisePropertyChanged("IsVisible");
            RaisePropertyChanged("IsHidden");
            RaisePropertyChanged("IsAutoHidden");
            base.OnParentChanged(oldValue, newValue);
        }

        void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutElementWithVisibility;
            if (parentPane != null)
                parentPane.ComputeVisibility();
        }

        #endregion

        #region AutoHideWidth

        private double _autohideWidth = 0.0;
        public double AutoHideWidth
        {
            get { return _autohideWidth; }
            set
            {
                if (_autohideWidth != value)
                {
                    RaisePropertyChanging("AutoHideWidth");
                    _autohideWidth = value;
                    RaisePropertyChanged("AutoHideWidth");
                }
            }
        }

        #endregion

        #region AutoHideMinWidth

        private double _autohideMinWidth = 25.0;
        public double AutoHideMinWidth
        {
            get { return _autohideMinWidth; }
            set
            {
                if (_autohideMinWidth != value)
                {
                    RaisePropertyChanging("AutoHideMinWidth");
                    _autohideMinWidth = value;
                    RaisePropertyChanged("AutoHideMinWidth");
                }
            }
        }

        #endregion

        #region AutoHideHeight

        private double _autohideHeight = 0.0;
        public double AutoHideHeight
        {
            get { return _autohideHeight; }
            set
            {
                if (_autohideHeight != value)
                {
                    RaisePropertyChanging("AutoHideHeight");
                    _autohideHeight = value;
                    RaisePropertyChanged("AutoHideHeight");
                }
            }
        }

        #endregion

        #region AutoHideMinHeight

        private double _autohideMinHeight = 0.0;
        public double AutoHideMinHeight
        {
            get { return _autohideMinHeight; }
            set
            {
                if (_autohideMinHeight != value)
                {
                    RaisePropertyChanging("AutoHideMinHeight");
                    _autohideMinHeight = value;
                    RaisePropertyChanged("AutoHideMinHeight");
                }
            }
        }

        #endregion
        
        /// <summary>
        /// Hide this contents
        /// </summary>
        /// <remarks>Add this content to <see cref="ILayoutRoot.Hidden"/> collection of parent root.</remarks>
        public void Hide()
        {
            if (!IsVisible)
            {
                IsSelected = true;
                IsActive = true;
                return;
            }
            RaisePropertyChanging("IsHidden");
            RaisePropertyChanging("IsVisible");
            if (Parent is ILayoutPane)
            {
                PreviousContainer = Parent as ILayoutPane;
                PreviousContainerIndex = (Parent as ILayoutContentSelector).SelectedContentIndex;
            }
            Root.Hidden.Add(this);
            RaisePropertyChanged("IsVisible");
            RaisePropertyChanged("IsHidden");
        }

        /// <summary>
        /// Show the content
        /// </summary>
        /// <remarks>Try to show the content where it was previously hidden.</remarks>
        public void Show()
        {
            if (IsVisible)
                return;

            RaisePropertyChanging("IsHidden");
            RaisePropertyChanging("IsVisible");

            bool added = false;
            var root = Root;
            if (root != null && root.Manager != null)
            {
                if (root.Manager.LayoutUpdateStrategy != null)
                    added = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(this, PreviousContainer);
            }

            if (!added && PreviousContainer != null)
            {
                var previousContainerAsLayoutGroup = PreviousContainer as ILayoutGroup;
                previousContainerAsLayoutGroup.InsertChildAt(PreviousContainerIndex, this);
                IsSelected = true;
                IsActive = true;
            }

            if (!added && root != null && root.Manager != null)
            {
                if (root.Manager.LayoutUpdateStrategy != null)
                    root.Manager.LayoutUpdateStrategy.InsertAnchorable(this, PreviousContainer);
            }


            RaisePropertyChanged("IsVisible");
            RaisePropertyChanged("IsHidden");
        }

        public void AddToLayout(DockingManager manager, AnchorableShowStrategy strategy)
        {
            if (IsVisible ||
                IsHidden)
                throw new InvalidOperationException();


            bool most = (strategy & AnchorableShowStrategy.Most) == AnchorableShowStrategy.Most;
            bool left = (strategy & AnchorableShowStrategy.Left) == AnchorableShowStrategy.Left;
            bool right = (strategy & AnchorableShowStrategy.Right) == AnchorableShowStrategy.Right;
            bool top = (strategy & AnchorableShowStrategy.Top) == AnchorableShowStrategy.Top;
            bool bottom = (strategy & AnchorableShowStrategy.Bottom) == AnchorableShowStrategy.Bottom;
            if (most)
            {
                if (manager.Layout.RootPanel == null)
                    manager.Layout.RootPanel = new LayoutPanel() { Orientation = (left || right ? Orientation.Horizontal : Orientation.Vertical) };

                if (left || right)
                {
                    if (manager.Layout.RootPanel.Orientation == Orientation.Vertical &&
                        manager.Layout.RootPanel.ChildrenCount > 1)
                    {
                        manager.Layout.RootPanel = new LayoutPanel(manager.Layout.RootPanel);
                    }

                    manager.Layout.RootPanel.Orientation = Orientation.Horizontal;

                    if (left)
                        manager.Layout.RootPanel.Children.Insert(0, new LayoutAnchorablePane(this));
                    else
                        manager.Layout.RootPanel.Children.Add(new LayoutAnchorablePane(this));
                }
                else
                {
                    if (manager.Layout.RootPanel.Orientation == Orientation.Horizontal &&
                        manager.Layout.RootPanel.ChildrenCount > 1)
                    {
                        manager.Layout.RootPanel = new LayoutPanel(manager.Layout.RootPanel);
                    }

                    manager.Layout.RootPanel.Orientation = Orientation.Vertical;

                    if (top)
                        manager.Layout.RootPanel.Children.Insert(0, new LayoutAnchorablePane(this));
                    else
                        manager.Layout.RootPanel.Children.Add(new LayoutAnchorablePane(this));
                }
                
            }
        }


        /// <summary>
        /// Get a value indicating if the anchorable is anchored to a border in an autohide status
        /// </summary>
        public bool IsAutoHidden
        {
            get { return Parent is LayoutAnchorGroup; }
        }


    }
}
