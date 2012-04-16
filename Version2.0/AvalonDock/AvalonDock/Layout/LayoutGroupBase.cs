using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AvalonDock.Layout
{
    [Serializable]
    public abstract class LayoutGroupBase : LayoutElement
    {
        [field: NonSerialized]
        [field: XmlIgnore]
        public event EventHandler ChildrenCollectionChanged;

        protected virtual void OnChildrenCollectionChanged()
        {
            if (ChildrenCollectionChanged != null)
                ChildrenCollectionChanged(this, EventArgs.Empty);
        }

        protected void NotifyChildrenTreeChanged(ChildrenTreeChange change)
        {
            OnChildrenTreeChanged(change);
            var parentGroup = Parent as LayoutGroupBase;
            if (parentGroup != null)
                parentGroup.NotifyChildrenTreeChanged(ChildrenTreeChange.TreeChanged);
        }

        [field: NonSerialized]
        [field: XmlIgnore]
        public event EventHandler<ChildrenTreeChangedEventArgs> ChildrenTreeChanged;

        protected virtual void OnChildrenTreeChanged(ChildrenTreeChange change)
        {
            if (ChildrenTreeChanged != null)
                ChildrenTreeChanged(this, new ChildrenTreeChangedEventArgs(change));
        }
    }
}
