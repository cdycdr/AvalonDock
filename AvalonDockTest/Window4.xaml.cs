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
using System.IO;

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

        }

        private void dockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            string xmlLayout =
                "<DockingManager>" +
                  "<ResizingPanel Orientation=\"Horizontal\">" +
                  "  <DockablePane ResizeWidth=\"0.2125\" Anchor=\"Left\">" +
                  "    <DockableContent Name=\"MyUserControl1\" AutoHide=\"false\" />" +
                  "  </DockablePane>" +
                  "  <DockablePane Anchor=\"Left\">" +
                  "    <DockableContent Name=\"MyUserControl2\" AutoHide=\"false\" />" +
                  "  </DockablePane>" +
                  "</ResizingPanel>" +
                  "<Hidden />" +
                  "<Windows />" +
                "</DockingManager>";

            StringReader sr = new StringReader(xmlLayout);
            dockingManager.RestoreLayout(sr);
        }

    }
}
