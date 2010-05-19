using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.Integration;
using System.IO;

namespace AvalonDock.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreatePropertyGrid();

            //var dockableContent = new DockableContent() { Title = "Test", Content = new TextBox() };
            //dockableContent.ShowAsFloatingWindow(DockManager, true);

            //var dockableContent2 = new DockableContent() { Title = "Test2", Content = new TextBox() };
            //dockableContent.ContainerPane.Items.Add(dockableContent2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DockManager.SaveLayout(@"layout.xml");
        }


        void CreatePropertyGrid()
        {
            var pg = new System.Windows.Forms.PropertyGrid() { SelectedObject = DockManager };
            WindowsFormsHost wfh = new WindowsFormsHost() { Child = pg };
            DockingManagerPropertiesHost.Content = wfh;
        }

        private void OnShowDockableContent(object sender, RoutedEventArgs e)
        {
            var selectedContent = ((MenuItem)e.OriginalSource).DataContext as DockableContent;

            if (selectedContent.State != DockableContentState.Docked)
            {
                //show content as docked content
                selectedContent.Show(DockManager, AnchorStyle.Right);
            }

            selectedContent.Activate();
        }

        private void OnCreateNewDockableContent(object sender, RoutedEventArgs e)
        {
            string title = "NewContent";
            DockableContent[] cnts = this.DockManager.DockableContents.ToArray();
            int i = 1;
            while (cnts.FirstOrDefault(c => c.Title == title) != null)
            {
                title = string.Format("NewContent{0}", i);
                i++;
            }

            var newContent = new SampleDockableContent() { Name = title, Title = title };

            if (((MenuItem)sender).Header.ToString() == "Create New Floating")
            {
                newContent.ShowAsFloatingWindow(DockManager, true);
                newContent.Activate();
            }
            else
            {
                newContent.Show(DockManager, AnchorStyle.Right);
                newContent.Activate();
            }
        }

        private void ExportLayoutToDocument(object sender, RoutedEventArgs e)
        {
            DocumentContent doc = new DocumentContent()
            {
                Title = string.Format("Layout_{0}", DateTime.Now.ToString()),
                Content = new TextBox()
                {
                    AcceptsReturn = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Text = DockManager.LayoutToString()
                }
            };


            doc.Show(DockManager);
            doc.Activate();
        }

        private void ImportLayoutFromDocument(object sender, RoutedEventArgs e)
        {
            if (DockManager.ActiveDocument != null &&
                DockManager.ActiveDocument.Content is TextBox)
            {
                string xml_layout = (DockManager.ActiveDocument.Content as TextBox).Text;
                DockManager.LayoutFromString(xml_layout);
            }
        }

        private void OnCreateNewDocumentContent(object sender, RoutedEventArgs e)
        {
            string title = "MainWindow.xml.cs";
            DocumentContent[] cnts = this.DockManager.Documents.ToArray();
            int i = 1;
            while (cnts.FirstOrDefault(c => c.Title == title) != null)
            {
                title = string.Format("NewDocument{0}", i);
                i++;
            }

            var newContent = new DocumentContent() { Title = title, Content = new TextBox() };
            newContent.Show(DockManager);
            newContent.Activate();            
        }

        private void DockManager_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("layout.xml"))
                DockManager.RestoreLayout(@"layout.xml");
        }

        private void ResetContent(object sender, RoutedEventArgs e)
        {
            DockManager.Content = null;
        }

        private void OnShowDocumentContent(object sender, RoutedEventArgs e)
        {
            var selectedDocument = ((MenuItem)e.OriginalSource).DataContext as DocumentContent;

            selectedDocument.Activate();
        }

        #region Themes

        private void SetDefaultTheme(object sender, RoutedEventArgs e)
        {
            ThemeFactory.ResetTheme();
        }

        private void ChangeCustomTheme(object sender, RoutedEventArgs e)
        {
            string uri = (string)((MenuItem)sender).Tag;
            ThemeFactory.ChangeTheme(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void ChangeStandardTheme(object sender, RoutedEventArgs e)
        {
            string name = (string)((MenuItem)sender).Tag;
            ThemeFactory.ChangeTheme(name);
        }

        #endregion




    }
}
