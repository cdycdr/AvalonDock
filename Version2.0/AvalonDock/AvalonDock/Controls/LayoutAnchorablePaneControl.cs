using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    class LayoutAnchorablePaneControl : TabControl, ILayoutControl
    {
        static LayoutAnchorablePaneControl()
        {
        }


        public LayoutAnchorablePaneControl(LayoutAnchorablePane model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            _model = model;
            
            SetBinding(ItemsSourceProperty, new Binding("Model.Children") { Source = this });

            this.LayoutUpdated += new EventHandler(OnLayoutUpdated);
        }

        void OnLayoutUpdated(object sender, EventArgs e)
        {
            var modelWithAtcualSize = _model as ILayoutPositionableElementWithActualSize;
            //if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            modelWithAtcualSize.ActualWidth = ActualWidth;
            //else
            modelWithAtcualSize.ActualHeight = ActualHeight;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new LayoutAnchorableTabItem();
        }

        internal void InternalAddLogicalChild(object value)
        {
            AddLogicalChild(value);
        }

        internal void InternalRemoveLogicalChild(object value)
        {
            RemoveLogicalChild(value);
        }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                return _model.Children.Select(a => a.Content).GetEnumerator();
            }
        }
        //protected override void OnInitialized(EventArgs e)
        //{
        //    base.OnInitialized(e);

        //    var docs = Anchorables;
        //    foreach (LayoutAnchorable content in _model.Children)
        //    {
        //        var contentView = new LayoutAnchorableControl(content);
        //        docs.Add(contentView);
        //        AddLogicalChild(contentView.Model.Content);
        //    }


        //    _model.Children.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ModelChildrenCollectionChanged);
        //    _model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnModelPropertyChanged);
        //}

        //void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{

        //}

        //protected override System.Collections.IEnumerator LogicalChildren
        //{
        //    get
        //    {
        //        return Anchorables.Select(a => a.Model.Content).GetEnumerator();
        //    }
        //}

        //void ModelChildrenCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    var docs = Anchorables;
        //    if (e.NewItems != null)
        //    {
        //        int index = e.NewStartingIndex;
        //        foreach (LayoutAnchorable content in e.NewItems)
        //        {
        //            var contentView = new LayoutAnchorableControl(content);
        //            docs.Insert(index, contentView);
        //            AddLogicalChild(contentView.Model.Content);
        //            index++;
        //        }
        //    }

        //    if (e.OldItems != null)
        //    {
        //        for (int i = e.OldStartingIndex; i < e.OldItems.Count; i++)
        //        {
        //            var docToRemove = docs[e.OldStartingIndex];
        //            docs.RemoveAt(e.OldStartingIndex);
        //            RemoveLogicalChild(docToRemove.Model.Content);
        //        }
        //    }
        //}

        LayoutAnchorablePane _model;

        public ILayoutElement Model
        {
            get { return _model; }
        }


    }
}