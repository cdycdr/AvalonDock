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

            this.Activated += new EventHandler(LayoutDocumentFloatingWindowControl_Activated);
        }


        LayoutDocumentFloatingWindow _model;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var manager = _model.Root.Manager;

            Content = manager.GetUIElementForModel(_model.RootDocument);
            SetBinding(BackgroundProperty, new Binding("DataContext.Background") { Source = Content });
        }

        void LayoutDocumentFloatingWindowControl_Activated(object sender, EventArgs e)
        {
            
        }


        protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32Helper.WM_NCLBUTTONDOWN: //Left button down on title -> start dragging over docking manager
                    if (wParam.ToInt32() == Win32Helper.HT_CAPTION)
                    {
                        FocusElementManager.SetFocusOnLastElement(_model.RootDocument);
                    }
                    break;
            }
               
            return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}
