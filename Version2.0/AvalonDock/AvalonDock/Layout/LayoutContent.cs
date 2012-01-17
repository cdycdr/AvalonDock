using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
    [ContentProperty("Content")]
    [Serializable]
    public abstract class LayoutContent : LayoutElement
    {
        internal LayoutContent()
        { }

        #region Title

        private string _title = null;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    RaisePropertyChanging("Title");
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        #endregion

        #region Content
        [NonSerialized]
        private object _content = null;
        public object Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    RaisePropertyChanging("Content");
                    _content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }

        #endregion

        #region IsSelected

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    bool oldValue = _isSelected;
                    RaisePropertyChanging("IsSelected");
                    _isSelected = value;
                    var parentSelector = (Parent as ILayoutContentSelector);
                    if (parentSelector != null)
                        parentSelector.SelectedContentIndex = _isSelected ? parentSelector.IndexOf(this) : -1;
                    OnIsSelectedChanged(oldValue, value);
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsSelected property.
        /// </summary>
        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
        }

        #endregion

        #region IsActive

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    RaisePropertyChanging("IsActive");
                    bool oldValue = _isActive;
                    _isActive = value;

                    var root = Root;
                    if (root != null && _isActive)
                        root.ActiveContent = this;

                    OnIsActiveChanged(oldValue, value);
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsActive property.
        /// </summary>
        protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
        {
        }

        #endregion


        #region PreviousContainer

        private ILayoutPane _previousContainer = null;
        public ILayoutPane PreviousContainer
        {
            get { return _previousContainer; }
            internal set
            {
                if (_previousContainer != value)
                {
                    _previousContainer = value;
                    RaisePropertyChanged("PreviousContainer");
                }
            }
        }

        #endregion

        #region PreviousContainerIndex

        private int _previousContainerIndex = -1;
        public int PreviousContainerIndex
        {
            get { return _previousContainerIndex; }
            set
            {
                if (_previousContainerIndex != value)
                {
                    _previousContainerIndex = value;
                    RaisePropertyChanged("PreviousContainerIndex");
                }
            }
        }

        #endregion

        protected override void OnParentChanged()
        {
            if (IsSelected && Parent != null && Parent is ILayoutContentSelector)
            {
                var parentSelector = (Parent as ILayoutContentSelector);
                parentSelector.SelectedContentIndex = parentSelector.IndexOf(this);
            }
            base.OnParentChanged();
        }
    }
}
