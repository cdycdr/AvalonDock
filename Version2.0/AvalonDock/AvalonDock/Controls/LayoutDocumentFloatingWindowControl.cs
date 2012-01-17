using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public class LayoutDocumentFloatingWindowControl : LayoutFloatingWindowControl
    {

        internal LayoutDocumentFloatingWindowControl(LayoutDocumentFloatingWindow model)
            :base(model)
        {
            _model = model;
        }

        LayoutDocumentFloatingWindow _model;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var manager = _model.Root.Manager;

            Content = manager.GetUIElementForModel(_model.RootDocument);
            SetBinding(BackgroundProperty, new Binding("DataContext.Background") { Source = Content });
        }


        
    }
}
