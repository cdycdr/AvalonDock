using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public class LayoutDocumentPaneControl : TabControl, ILayoutControl
    {
        static LayoutDocumentPaneControl()
        {
            FocusableProperty.OverrideMetadata(typeof(LayoutDocumentPaneControl), new FrameworkPropertyMetadata(false));
        }


        internal LayoutDocumentPaneControl(LayoutDocumentPane model)
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
            return new LayoutDocumentTabItem();
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

        //    CreateViews();

        //    _model.Children.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ModelChildrenCollectionChanged);
        //    _model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnModelPropertyChanged);
        //}

        //private void CreateViews()
        //{
        //    var docs = Documents;
        //    foreach (LayoutDocumentControl content in docs)
        //        RemoveLogicalChild(content.Model.Content);
        //    docs.Clear();

        //    foreach (LayoutContent content in _model.Children)
        //    {
        //        var contentView = new LayoutDocumentControl(content);
        //        docs.Add(contentView);
        //        AddLogicalChild(contentView.Model.Content);
        //    }

        //}

        //void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    //if (e.PropertyName == "SelectedContentIndex")
        //    //{
        //    //    SelectedDocumentIndex = _model.SelectedContentIndex;
        //    //}
        //}

        //protected override System.Collections.IEnumerator LogicalChildren
        //{
        //    get
        //    {
        //        return Documents.Select(d => d.Model.Content).GetEnumerator();
        //    }
        //}
        

        //void ModelChildrenCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    var docs = Documents;
        //    if (e.NewItems != null)
        //    {
        //        int index = e.NewStartingIndex;
        //        foreach (LayoutContent content in e.NewItems)
        //        {
        //            docs.Insert(index, new LayoutDocumentControl(content));
        //            AddLogicalChild(content.Content);
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

        //    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
        //    {
        //        CreateViews();
        //    }
        //}

        LayoutDocumentPane _model;

        public ILayoutElement Model
        {
            get { return _model; }
        }



        //public ObservableCollection<LayoutDocumentControl> _documents = new ObservableCollection<LayoutDocumentControl>();
        //public ObservableCollection<LayoutDocumentControl> Documents
        //{
        //    get { return _documents; }
        //}


    }
}
