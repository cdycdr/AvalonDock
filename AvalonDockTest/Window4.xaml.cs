using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AvalonDock;

namespace AvalonDockTest
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        public Window4()
        {
            InitializeComponent();

           // DockingManager mng = new DockingManager();
           //// this.Content = mng;

           // DockableContent ccnt = new DockableContent();

            //System.Windows.Forms.Integration.WindowsFormsHost _PropGridHost = new System.Windows.Forms.Integration.WindowsFormsHost();

            //System.Windows.Forms.PropertyGrid _PropGrid = new System.Windows.Forms.PropertyGrid();

            //_PropGrid.Name = "_PropertyGrid";

            //_PropGridHost.Child = _PropGrid;

            //this.Content = _PropGridHost;

            //ccnt.Content = _PropGridHost;


            //DockablePane pane = new DockablePane();
            //pane.Items.Add(ccnt);
            //ResizingPanel.SetResizeWidth(pane, 200);
            //ResizingPanel.SetResizeHeight(pane, 200);

            //ResizingPanel panel = new ResizingPanel();
            //panel.Children.Add(pane);
            //panel.Children.Add(new Border());

            //mng.Content = pane;

            //FlyoutPaneWindow wnd = new FlyoutPaneWindow();
            //wnd.Content = pane;
            //wnd.Show();

            //Window testWindow = new Window();
            ////testWindow.AllowsTransparency = true;
            //testWindow.Background = Brushes.Transparent;
            //testWindow.WindowStyle = WindowStyle.None;
            //testWindow.BorderThickness = new Thickness(0);
            //testWindow.ShowInTaskbar = false;
            //testWindow.Content = panel;
            //testWindow.Show();


        }

        Dock _dock = Dock.Top;
        public Dock Dock
        {
            get { return _dock; }
            set { _dock = value; }
        }

    }
}
