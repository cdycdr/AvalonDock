using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace AvalonDock.Layout
{
    [ContentProperty("Content")]
    [Serializable]
    public abstract class LayoutContent : LayoutElement, IXmlSerializable
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
        [XmlIgnore]
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

        [field: NonSerialized]
        private ILayoutPane _previousContainer = null;

        [XmlIgnore]
        public ILayoutPane PreviousContainer
        {
            get { return _previousContainer; }
            internal set
            {
                if (_previousContainer != value)
                {
                    _previousContainer = value;
                    RaisePropertyChanged("PreviousContainer");

                    var paneSerializable = _previousContainer as ILayoutPaneSerializable;
                    if (paneSerializable != null &&
                        paneSerializable.Id == null)
                        paneSerializable.Id = Guid.NewGuid().ToString();
                }
            }
        }

        #endregion

        #region PreviousContainerIndex
        [field: NonSerialized]
        private int _previousContainerIndex = -1;
        [XmlIgnore]
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

            var root = Root;
            if (root != null && _isActive)
                root.ActiveContent = this;

            base.OnParentChanged();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Title"))
                Title = reader.Value;
            if (reader.MoveToAttribute("IsSelected"))
                IsSelected = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("IsActive"))
                IsActive = bool.Parse(reader.Value);

            reader.Read();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(Title))
                writer.WriteAttributeString("Title", Title);
            
            if (IsSelected)
                writer.WriteAttributeString("IsSelected", IsSelected.ToString());
            
            if (IsActive)
                writer.WriteAttributeString("IsActive", IsActive.ToString());
            
            if (_previousContainer != null)
            {
                var paneSerializable = _previousContainer as ILayoutPaneSerializable;
                writer.WriteAttributeString("PreviousContainer", paneSerializable.Id);
                writer.WriteAttributeString("PreviousContainerIndex", _previousContainerIndex.ToString());
            }

        }
    }
}
