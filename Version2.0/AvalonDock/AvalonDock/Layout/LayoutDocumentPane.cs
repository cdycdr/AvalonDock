using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
    [ContentProperty("Children")]
    public class LayoutDocumentPane : LayoutPositionableGroup<LayoutContent>, ILayoutDocumentPane, ILayoutPositionableElement, ILayoutContentSelector
    {
        public LayoutDocumentPane()
        {
        }
        public LayoutDocumentPane(LayoutContent firstChild)
        {
            Children.Add(firstChild);
        }

        protected override bool GetVisibility()
        {
            if (Parent is LayoutDocumentPaneGroup)
                return ChildrenCount > 0;
            
            return true;
        }

        #region SelectedContentIndex

        private int _selectedIndex = -1;
        public int SelectedContentIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value < 0 ||
                    value >= Children.Count)
                    value = -1;

                if (_selectedIndex != value)
                {
                    RaisePropertyChanged("SelectedContentIndex");
                    if (_selectedIndex >= 0 &&
                        _selectedIndex < Children.Count)
                        Children[_selectedIndex].IsSelected = false;

                    _selectedIndex = value;

                    if (_selectedIndex >= 0 &&
                        _selectedIndex < Children.Count)
                        Children[_selectedIndex].IsSelected = true;

                    RaisePropertyChanged("SelectedContentIndex");
                }
            }
        }
        public LayoutContent SelectedContent
        {
            get { return Children[_selectedIndex]; }
        }
        #endregion

        public int IndexOf(LayoutContent content)
        {
            return Children.IndexOf(content);
        }

        protected override void OnIsVisibleChanged()
        {
            UpdateParentVisibility();
            base.OnIsVisibleChanged();
        }

        void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutDocumentPane;
            if (parentPane != null)
                parentPane.ComputeVisibility();
        }
    }
}
