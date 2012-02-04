using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AvalonDock.Layout
{
    [Serializable]
    public abstract class LayoutElement : ILayoutElement
    {
        internal LayoutElement()
        { }

        [NonSerialized]
        ILayoutContainer _parent;

        [XmlIgnore]
        public ILayoutContainer Parent
        {
            get { return _parent; }
            internal set
            {
                if (_parent != value)
                {
                    RaisePropertyChanging("Parent");
                    _parent = value;
                    OnParentChanged();
                    RaisePropertyChanged("Parent");
                }
            }
        }

        protected virtual void OnParentChanged()
        { }

        [field: NonSerialized]
        [field: XmlIgnore]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        [field: NonSerialized]
        [field: XmlIgnore]
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void RaisePropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
        }


        public ILayoutRoot Root
        {
            get
            {
                var parent = Parent;

                while (parent != null && (!(parent is ILayoutRoot)))
                {
                    parent = parent.Parent;
                }

                return parent as ILayoutRoot;
            }
        }




    }
}
