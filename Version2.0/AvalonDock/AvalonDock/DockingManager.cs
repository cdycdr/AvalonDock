using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Interop;
using System.Diagnostics;

using AvalonDock.Layout;
using AvalonDock.Controls;
using System.Windows.Input;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Threading;
using AvalonDock.Commands;

namespace AvalonDock
{
    [ContentProperty("Layout")]
    [TemplatePart(Name="PART_AutoHideArea")]
    public class DockingManager : Control, IOverlayWindowHost, ILogicalChildrenContainer
    {
        static DockingManager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(typeof(DockingManager)));
            FocusableProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(true));
            HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
            Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.None;

        }


        public DockingManager()
        {
            Layout = new LayoutRoot();
            this.Loaded += new RoutedEventHandler(DockingManager_Loaded);
            this.Unloaded += new RoutedEventHandler(DockingManager_Unloaded);
        }

        #region Layout

        /// <summary>
        /// Layout Dependency Property
        /// </summary>
        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(LayoutRoot), typeof(DockingManager),
                new FrameworkPropertyMetadata((LayoutRoot)null,
                    new PropertyChangedCallback(OnLayoutChanged),
                    new CoerceValueCallback(CoerceLayoutValue)));

        /// <summary>
        /// Gets or sets the Layout property.  This dependency property 
        /// indicates layout tree.
        /// </summary>
        public LayoutRoot Layout
        {
            get { return (LayoutRoot)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Layout property.
        /// </summary>
        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutChanged(e.OldValue as LayoutRoot, e.NewValue as LayoutRoot);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the <see cref="DockingManager.Layout"/> property.
        /// </summary>
        protected virtual void OnLayoutChanged(LayoutRoot oldLayout, LayoutRoot newLayout)
        {
            if (oldLayout != null)
            {
                oldLayout.PropertyChanged -= new PropertyChangedEventHandler(OnLayoutRootPropertyChanged);
            }

            DetachDocumentsSource(oldLayout, DocumentsSource);

            if (oldLayout != null &&
                oldLayout.Manager == this)
                oldLayout.Manager = null;

            ClearLogicalChildrenList();

            Layout.Manager = this;

            if (IsInitialized)
            {
                LayoutRootPanel = GetUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                LeftSidePanel = GetUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
                TopSidePanel = GetUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
                RightSidePanel = GetUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
                BottomSidePanel = GetUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;

                foreach (var fwc in _fwList)
                    fwc.InternalClose();
                _fwList.Clear();

                foreach (var fw in Layout.FloatingWindows)
                    _fwList.Add(GetUIElementForModel(fw) as LayoutAnchorableFloatingWindowControl);

                foreach (var fw in _fwList)
                    fw.Owner = Window.GetWindow(this);
            }

            AttachDocumentsSource(newLayout, DocumentsSource);

            if (newLayout != null)
            {
                newLayout.PropertyChanged += new PropertyChangedEventHandler(OnLayoutRootPropertyChanged);
            }

            if (LayoutChanged != null)
                LayoutChanged(this, EventArgs.Empty);
        }

        void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RootPanel")
            {
                if (IsInitialized)
                {
                    var layoutRootPanel = GetUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                    LayoutRootPanel = layoutRootPanel;
                }
            }
            else if (e.PropertyName == "ActiveContent")
            {
                if (Layout.ActiveContent != null)
                {
                    //set focus on active element only after a layout pass is completed
                    //it's possible that it is not yet visible in the visual tree
                    Dispatcher.BeginInvoke(new Action(() =>
                        {
                            FocusElementManager.SetFocusOnLastElement(Layout.ActiveContent);
                        }), DispatcherPriority.Loaded);
                }
            }
        }

        /// <summary>
        /// Event fired when <see cref="DockingManager.Layout"/> property changes
        /// </summary>
        public event EventHandler LayoutChanged;

        /// <summary>
        /// Coerces the <see cref="DockingManager.Layout"/> value.
        /// </summary>
        private static object CoerceLayoutValue(DependencyObject d, object value)
        {
            if (value == null)
                return new LayoutRoot() { RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup()) };

            return value;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetupAutoHideArea();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (!DesignerProperties.GetIsInDesignMode(this) && Layout.Manager == this)
            {
                LayoutRootPanel = GetUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                LeftSidePanel = GetUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
                TopSidePanel = GetUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
                RightSidePanel = GetUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
                BottomSidePanel = GetUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;
                foreach (var fw in Layout.FloatingWindows)
                    _fwList.Add(GetUIElementForModel(fw) as LayoutAnchorableFloatingWindowControl);
            }

        }

        void DockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (var fw in _fwList)
                    fw.Owner = Window.GetWindow(this);

                CreateOverlayWindow();
                FocusElementManager.SetupFocusManagement(this);
            } 
        }

        void DockingManager_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (var fw in _fwList.ToArray())
                {
                    fw.Owner = null;
                    fw.InternalClose();
                }

                DestroyOverlayWindow();
                FocusElementManager.FinalizeFocusManagement(this);
            }
        }

        internal UIElement GetUIElementForModel(ILayoutElement model)
        {
            if (model is LayoutPanel)
                return new LayoutPanelControl(model as LayoutPanel);
            if (model is LayoutAnchorablePaneGroup)
                return new LayoutAnchorablePaneGroupControl(model as LayoutAnchorablePaneGroup);
            if (model is LayoutDocumentPaneGroup)
                return new LayoutDocumentPaneGroupControl(model as LayoutDocumentPaneGroup);

            if (model is LayoutAnchorSide)
            {
                var templateModelView = new LayoutAnchorSideControl(model as LayoutAnchorSide);
                templateModelView.SetBinding(LayoutAnchorSideControl.TemplateProperty, new Binding("AnchorSideTemplate") { Source = this });
                return templateModelView;
            }
            if (model is LayoutAnchorGroup)
            {
                var templateModelView = new LayoutAnchorGroupControl(model as LayoutAnchorGroup);
                templateModelView.SetBinding(LayoutAnchorGroupControl.TemplateProperty, new Binding("AnchorGroupTemplate") { Source = this });
                return templateModelView;
            }

            if (model is LayoutDocumentPane)
            {
                var templateModelView = new LayoutDocumentPaneControl(model as LayoutDocumentPane);
                templateModelView.SetBinding(LayoutDocumentPaneControl.StyleProperty, new Binding("DocumentPaneControlStyle") { Source = this });
                return templateModelView;
            }
            if (model is LayoutAnchorablePane)
            {
                var templateModelView = new LayoutAnchorablePaneControl(model as LayoutAnchorablePane);
                templateModelView.SetBinding(LayoutAnchorablePaneControl.StyleProperty, new Binding("AnchorablePaneControlStyle") { Source = this });
                return templateModelView;
            }

            if (model is LayoutAnchorableFloatingWindow)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return null;
                var modelFW = model as LayoutAnchorableFloatingWindow;
                var newFW = new LayoutAnchorableFloatingWindowControl(modelFW);

                var paneForExtentions = modelFW.RootPanel.Children.OfType<LayoutAnchorablePane>().FirstOrDefault();
                if (paneForExtentions != null)
                {
                    newFW.Left = paneForExtentions.FloatingLeft;
                    newFW.Top = paneForExtentions.FloatingTop;
                    newFW.Width = paneForExtentions.FloatingWidth;
                    newFW.Height = paneForExtentions.FloatingHeight;
                }

                newFW.ShowInTaskbar = false;
                newFW.Show();
                return newFW;
            }


            if (model is LayoutDocument)
            {
                var templateModelView = new LayoutDocumentControl() { Model = model as LayoutDocument };
                return templateModelView;
            }

            return null;
        }

        #region DocumentPaneTemplate

        /// <summary>
        /// DocumentPaneTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty DocumentPaneTemplateProperty =
            DependencyProperty.Register("DocumentPaneTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((ControlTemplate)null,
                    new PropertyChangedCallback(OnDocumentPaneTemplateChanged)));

        /// <summary>
        /// Gets or sets the DocumentPaneDataTemplate property.  This dependency property 
        /// indicates .
        /// </summary>
        public ControlTemplate DocumentPaneTemplate
        {
            get { return (ControlTemplate)GetValue(DocumentPaneTemplateProperty); }
            set { SetValue(DocumentPaneTemplateProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DocumentPaneTemplate property.
        /// </summary>
        private static void OnDocumentPaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneTemplateChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DocumentPaneTemplate property.
        /// </summary>
        protected virtual void OnDocumentPaneTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region AnchorablePaneTemplate

        /// <summary>
        /// AnchorablePaneTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnchorablePaneTemplateProperty =
            DependencyProperty.Register("AnchorablePaneTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((ControlTemplate)null,
                    new PropertyChangedCallback(OnAnchorablePaneTemplateChanged)));

        /// <summary>
        /// Gets or sets the AnchorablePaneTemplate property.  This dependency property 
        /// indicates ....
        /// </summary>
        public ControlTemplate AnchorablePaneTemplate
        {
            get { return (ControlTemplate)GetValue(AnchorablePaneTemplateProperty); }
            set { SetValue(AnchorablePaneTemplateProperty, value); }
        }

        /// <summary>
        /// Handles changes to the AnchorablePaneDataTemplate property.
        /// </summary>
        private static void OnAnchorablePaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorablePaneTemplateChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the AnchorablePaneDataTemplate property.
        /// </summary>
        protected virtual void OnAnchorablePaneTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region AnchorSideTemplate

        /// <summary>
        /// AnchorSideTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnchorSideTemplateProperty =
            DependencyProperty.Register("AnchorSideTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((ControlTemplate)null));

        /// <summary>
        /// Gets or sets the AnchorSideTemplate property.  This dependency property 
        /// indicates ....
        /// </summary>
        public ControlTemplate AnchorSideTemplate
        {
            get { return (ControlTemplate)GetValue(AnchorSideTemplateProperty); }
            set { SetValue(AnchorSideTemplateProperty, value); }
        }

        #endregion

        #region AnchorGroupTemplate

        /// <summary>
        /// AnchorGroupTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnchorGroupTemplateProperty =
            DependencyProperty.Register("AnchorGroupTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((ControlTemplate)null));

        /// <summary>
        /// Gets or sets the AnchorGroupTemplate property.  This dependency property 
        /// indicates the template used to render the AnchorGroup control.
        /// </summary>
        public ControlTemplate AnchorGroupTemplate
        {
            get { return (ControlTemplate)GetValue(AnchorGroupTemplateProperty); }
            set { SetValue(AnchorGroupTemplateProperty, value); }
        }

        #endregion

        #region AnchorTemplate

        /// <summary>
        /// AnchorTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnchorTemplateProperty =
            DependencyProperty.Register("AnchorTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((ControlTemplate)null));

        /// <summary>
        /// Gets or sets the AnchorTemplate property.  This dependency property 
        /// indicates ....
        /// </summary>
        public ControlTemplate AnchorTemplate
        {
            get { return (ControlTemplate)GetValue(AnchorTemplateProperty); }
            set { SetValue(AnchorTemplateProperty, value); }
        }

        #endregion

        #region DocumentPaneControlStyle

        /// <summary>
        /// DocumentPaneControlStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty DocumentPaneControlStyleProperty =
            DependencyProperty.Register("DocumentPaneControlStyle", typeof(Style), typeof(DockingManager),
                new FrameworkPropertyMetadata((Style)null,
                    new PropertyChangedCallback(OnDocumentPaneControlStyleChanged)));

        /// <summary>
        /// Gets or sets the DocumentPaneControlStyle property.  This dependency property 
        /// indicates ....
        /// </summary>
        public Style DocumentPaneControlStyle
        {
            get { return (Style)GetValue(DocumentPaneControlStyleProperty); }
            set { SetValue(DocumentPaneControlStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DocumentPaneControlStyle property.
        /// </summary>
        private static void OnDocumentPaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneControlStyleChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DocumentPaneControlStyle property.
        /// </summary>
        protected virtual void OnDocumentPaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region AnchorablePaneControlStyle

        /// <summary>
        /// AnchorablePaneControlStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnchorablePaneControlStyleProperty =
            DependencyProperty.Register("AnchorablePaneControlStyle", typeof(Style), typeof(DockingManager),
                new FrameworkPropertyMetadata((Style)null,
                    new PropertyChangedCallback(OnAnchorablePaneControlStyleChanged)));

        /// <summary>
        /// Gets or sets the AnchorablePaneControlStyle property.  This dependency property 
        /// indicates the style to apply to AnchorablePaneControl.
        /// </summary>
        public Style AnchorablePaneControlStyle
        {
            get { return (Style)GetValue(AnchorablePaneControlStyleProperty); }
            set { SetValue(AnchorablePaneControlStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the AnchorablePaneControlStyle property.
        /// </summary>
        private static void OnAnchorablePaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorablePaneControlStyleChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the AnchorablePaneControlStyle property.
        /// </summary>
        protected virtual void OnAnchorablePaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region DocumentHeaderTemplate

        /// <summary>
        /// DocumentHeaderTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty DocumentHeaderTemplateProperty =
            DependencyProperty.Register("DocumentHeaderTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplate)null));

        /// <summary>
        /// Gets or sets the DocumentHeaderTemplate property.  This dependency property 
        /// indicates data template to use when creating document headers.
        /// </summary>
        public DataTemplate DocumentHeaderTemplate
        {
            get { return (DataTemplate)GetValue(DocumentHeaderTemplateProperty); }
            set { SetValue(DocumentHeaderTemplateProperty, value); }
        }

        #endregion

        #region DocumentHeaderTemplateSelector

        /// <summary>
        /// DocumentHeaderTemplateSelector Dependency Property
        /// </summary>
        public static readonly DependencyProperty DocumentHeaderTemplateSelectorProperty =
            DependencyProperty.Register("DocumentHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplateSelector)null));

        /// <summary>
        /// Gets or sets the DocumentHeaderTemplateSelector property.  This dependency property 
        /// indicates data template selector to use when selecting data template for documents header.
        /// </summary>
        public DataTemplateSelector DocumentHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DocumentHeaderTemplateSelectorProperty); }
            set { SetValue(DocumentHeaderTemplateSelectorProperty, value); }
        }

        #endregion

        #region AnchorableHeaderTemplate

        /// <summary>
        /// AnchorableHeaderTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnchorableHeaderTemplateProperty =
            DependencyProperty.Register("AnchorableHeaderTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplate)null));

        /// <summary>
        /// Gets or sets the AnchorableHeaderTemplate property.  This dependency property 
        /// indicates data template to use when creating anchorable headers.
        /// </summary>
        public DataTemplate AnchorableHeaderTemplate
        {
            get { return (DataTemplate)GetValue(AnchorableHeaderTemplateProperty); }
            set { SetValue(AnchorableHeaderTemplateProperty, value); }
        }

        #endregion

        #region AnchorableHeaderTemplateSelector

        /// <summary>
        /// AnchorableHeaderTemplateSelector Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnchorableHeaderTemplateSelectorProperty =
            DependencyProperty.Register("AnchorableHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplateSelector)null));

        /// <summary>
        /// Gets or sets the AnchorableHeaderTemplateSelector property.  This dependency property 
        /// indicates data template selector to use when creating data template for anchorable headers.
        /// </summary>
        public DataTemplateSelector AnchorableHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(AnchorableHeaderTemplateSelectorProperty); }
            set { SetValue(AnchorableHeaderTemplateSelectorProperty, value); }
        }

        #endregion

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus is Grid)
                Debug.WriteLine(string.Format("DockingManager.OnGotKeyboardFocus({0})", e.NewFocus));
            base.OnGotKeyboardFocus(e);
        }

        protected override void OnPreviewGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Debug.WriteLine(string.Format("DockingManager.OnPreviewGotKeyboardFocus({0})", e.NewFocus));
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Debug.WriteLine(string.Format("DockingManager.OnPreviewLostKeyboardFocus({0})", e.OldFocus));
            base.OnPreviewLostKeyboardFocus(e);
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine(string.Format("DockingManager.OnMouseLeftButtonDown([{0}])", e.GetPosition(this)));
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            //Debug.WriteLine(string.Format("DockingManager.OnMouseMove([{0}])", e.GetPosition(this)));
            base.OnMouseMove(e);
        }

        #region LayoutRootPanel

        /// <summary>
        /// LayoutRootPanel Dependency Property
        /// </summary>
        public static readonly DependencyProperty LayoutRootPanelProperty =
            DependencyProperty.Register("LayoutRootPanel", typeof(LayoutPanelControl), typeof(DockingManager),
                new FrameworkPropertyMetadata((LayoutPanelControl)null,
                    new PropertyChangedCallback(OnLayoutRootPanelChanged)));

        /// <summary>
        /// Gets or sets the LayoutRootPanel property.  This dependency property 
        /// indicates ....
        /// </summary>
        public LayoutPanelControl LayoutRootPanel
        {
            get { return (LayoutPanelControl)GetValue(LayoutRootPanelProperty); }
            set { SetValue(LayoutRootPanelProperty, value); }
        }

        /// <summary>
        /// Handles changes to the LayoutRootPanel property.
        /// </summary>
        private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutRootPanelChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the LayoutRootPanel property.
        /// </summary>
        protected virtual void OnLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((ILogicalChildrenContainer)this).InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                ((ILogicalChildrenContainer)this).InternalAddLogicalChild(e.NewValue);
        }

        #endregion

        #region RightSidePanel

        /// <summary>
        /// RightSidePanel Dependency Property
        /// </summary>
        public static readonly DependencyProperty RightSidePanelProperty =
            DependencyProperty.Register("RightSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata((LayoutAnchorSideControl)null,
                    new PropertyChangedCallback(OnRightSidePanelChanged)));

        /// <summary>
        /// Gets or sets the RightSidePanel property.  This dependency property 
        /// indicates right side anchor panel.
        /// </summary>
        public LayoutAnchorSideControl RightSidePanel
        {
            get { return (LayoutAnchorSideControl)GetValue(RightSidePanelProperty); }
            set { SetValue(RightSidePanelProperty, value); }
        }

        /// <summary>
        /// Handles changes to the RightSidePanel property.
        /// </summary>
        private static void OnRightSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnRightSidePanelChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the RightSidePanel property.
        /// </summary>
        protected virtual void OnRightSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((ILogicalChildrenContainer)this).InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                ((ILogicalChildrenContainer)this).InternalAddLogicalChild(e.NewValue);
        }

        #endregion

        #region LeftSidePanel

        /// <summary>
        /// LeftSidePanel Dependency Property
        /// </summary>
        public static readonly DependencyProperty LeftSidePanelProperty =
            DependencyProperty.Register("LeftSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata((LayoutAnchorSideControl)null,
                    new PropertyChangedCallback(OnLeftSidePanelChanged)));

        /// <summary>
        /// Gets or sets the LeftSidePanel property.  This dependency property 
        /// indicates the left side panel control.
        /// </summary>
        public LayoutAnchorSideControl LeftSidePanel
        {
            get { return (LayoutAnchorSideControl)GetValue(LeftSidePanelProperty); }
            set { SetValue(LeftSidePanelProperty, value); }
        }

        /// <summary>
        /// Handles changes to the LeftSidePanel property.
        /// </summary>
        private static void OnLeftSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLeftSidePanelChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the LeftSidePanel property.
        /// </summary>
        protected virtual void OnLeftSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((ILogicalChildrenContainer)this).InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                ((ILogicalChildrenContainer)this).InternalAddLogicalChild(e.NewValue);
        }

        #endregion

        #region TopSidePanel

        /// <summary>
        /// TopSidePanel Dependency Property
        /// </summary>
        public static readonly DependencyProperty TopSidePanelProperty =
            DependencyProperty.Register("TopSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata((LayoutAnchorSideControl)null,
                    new PropertyChangedCallback(OnTopSidePanelChanged)));

        /// <summary>
        /// Gets or sets the TopSidePanel property.  This dependency property 
        /// indicates top side control panel.
        /// </summary>
        public LayoutAnchorSideControl TopSidePanel
        {
            get { return (LayoutAnchorSideControl)GetValue(TopSidePanelProperty); }
            set { SetValue(TopSidePanelProperty, value); }
        }

        /// <summary>
        /// Handles changes to the TopSidePanel property.
        /// </summary>
        private static void OnTopSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnTopSidePanelChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the TopSidePanel property.
        /// </summary>
        protected virtual void OnTopSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((ILogicalChildrenContainer)this).InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                ((ILogicalChildrenContainer)this).InternalAddLogicalChild(e.NewValue);
        }

        #endregion

        #region BottomSidePanel

        /// <summary>
        /// BottomSidePanel Dependency Property
        /// </summary>
        public static readonly DependencyProperty BottomSidePanelProperty =
            DependencyProperty.Register("BottomSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata((LayoutAnchorSideControl)null,
                    new PropertyChangedCallback(OnBottomSidePanelChanged)));

        /// <summary>
        /// Gets or sets the BottomSidePanel property.  This dependency property 
        /// indicates bottom side panel control.
        /// </summary>
        public LayoutAnchorSideControl BottomSidePanel
        {
            get { return (LayoutAnchorSideControl)GetValue(BottomSidePanelProperty); }
            set { SetValue(BottomSidePanelProperty, value); }
        }

        /// <summary>
        /// Handles changes to the BottomSidePanel property.
        /// </summary>
        private static void OnBottomSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnBottomSidePanelChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the BottomSidePanel property.
        /// </summary>
        protected virtual void OnBottomSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((ILogicalChildrenContainer)this).InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                ((ILogicalChildrenContainer)this).InternalAddLogicalChild(e.NewValue);
        }

        #endregion




        #region LogicalChildren

        List<object> _logicalChildren = new List<object>();

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                return _logicalChildren.GetEnumerator();
            }
        }


        void ILogicalChildrenContainer.InternalAddLogicalChild(object element)
        {
            if (_logicalChildren.Contains(element))
                throw new InvalidOperationException();
            _logicalChildren.Add(element);
            AddLogicalChild(element);
        }

        void ILogicalChildrenContainer.InternalRemoveLogicalChild(object element)
        {
            if (!_logicalChildren.Contains(element))
                throw new InvalidOperationException();
            _logicalChildren.Remove(element);
            RemoveLogicalChild(element);
        }

        void ClearLogicalChildrenList()
        {
            foreach (var child in _logicalChildren.ToArray())
            {
                _logicalChildren.Remove(child);
                RemoveLogicalChild(child);
            }
        }

        #endregion  
    
        #region AutoHide window
        internal void ShowAutoHideWindow(LayoutAnchorControl anchor)
        {
            if (_autohideArea == null)
                return;

            if (AutoHideWindow != null && AutoHideWindow.Model == anchor.Model)
                return;

            HideAutoHideWindow();

            SetAutoHideWindow(new LayoutAutoHideWindow(anchor));
        }

        internal void HideAutoHideWindow()
        {
            if (AutoHideWindow != null)
            {
                AutoHideWindow.Dispose();
                SetAutoHideWindow(null);
            }
        }

        FrameworkElement _autohideArea;
        internal FrameworkElement GetAutoHideAreaElement()
        {
            return _autohideArea;
        }

        void SetupAutoHideArea()
        {
            _autohideArea = GetTemplateChild("PART_AutoHideArea") as FrameworkElement;
        }

        #region AutoHideWindow

        /// <summary>
        /// AutoHideWindow Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey AutoHideWindowPropertyKey
            = DependencyProperty.RegisterReadOnly("AutoHideWindow", typeof(LayoutAutoHideWindow), typeof(DockingManager),
                new FrameworkPropertyMetadata((LayoutAutoHideWindow)null,
                    new PropertyChangedCallback(OnAutoHideWindowChanged)));

        public static readonly DependencyProperty AutoHideWindowProperty
            = AutoHideWindowPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the AutoHideWindow property.  This dependency property 
        /// indicates ....
        /// </summary>
        public LayoutAutoHideWindow AutoHideWindow
        {
            get { return (LayoutAutoHideWindow)GetValue(AutoHideWindowProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the AutoHideWindow property.  
        /// This dependency property indicates ....
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetAutoHideWindow(LayoutAutoHideWindow value)
        {
            SetValue(AutoHideWindowPropertyKey, value);
        }

        /// <summary>
        /// Handles changes to the AutoHideWindow property.
        /// </summary>
        private static void OnAutoHideWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAutoHideWindowChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the AutoHideWindow property.
        /// </summary>
        protected virtual void OnAutoHideWindowChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((ILogicalChildrenContainer)this).InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                ((ILogicalChildrenContainer)this).InternalAddLogicalChild(e.NewValue);
        }

        #endregion



        #endregion

        #region Floating Windows
        List<LayoutFloatingWindowControl> _fwList = new List<LayoutFloatingWindowControl>();

        internal void StartDraggingFloatingWindowForContent(LayoutContent contentModel)
        {
            var parentPane = contentModel.Parent as ILayoutPane;
            var parentPaneAsPositionableElement = contentModel.Parent as ILayoutPositionableElement;
            var parentPaneAsWithActualSize = contentModel.Parent as ILayoutPositionableElementWithActualSize;
            var contentModelParentChildrenIndex = parentPane.Children.ToList().IndexOf(contentModel);

            if (contentModel.FindParent<LayoutFloatingWindow>() == null)
            {
                contentModel.PreviousContainer = parentPane;
                contentModel.PreviousContainerIndex = contentModelParentChildrenIndex;
            }

            parentPane.RemoveChildAt(contentModelParentChildrenIndex);

            double fwWidth = parentPaneAsPositionableElement.FloatingWidth;
            double fwHeight = parentPaneAsPositionableElement.FloatingHeight;

            if (fwWidth == 0.0)
                fwWidth = parentPaneAsWithActualSize.ActualWidth;
            if (fwHeight == 0.0)
                fwHeight = parentPaneAsWithActualSize.ActualHeight;

            LayoutFloatingWindow fw;
            LayoutFloatingWindowControl fwc;
            if (contentModel is LayoutAnchorable)
            {
                var anchorableContent = contentModel as LayoutAnchorable;
                fw = new LayoutAnchorableFloatingWindow()
                {
                    RootPanel = new LayoutAnchorablePaneGroup(
                        new LayoutAnchorablePane(anchorableContent)
                        {
                            DockWidth = parentPaneAsPositionableElement.DockWidth,
                            DockHeight = parentPaneAsPositionableElement.DockHeight,
                            DockMinHeight = parentPaneAsPositionableElement.DockMinHeight,
                            DockMinWidth = parentPaneAsPositionableElement.DockMinWidth
                        })
                };

                fwc = new LayoutAnchorableFloatingWindowControl(
                    fw as LayoutAnchorableFloatingWindow)
                    {
                        Width = fwWidth,
                        Height = fwHeight
                    };
            }
            else
            {
                var anchorableDocument = contentModel as LayoutDocument;
                fw = new LayoutDocumentFloatingWindow()
                {
                    RootDocument = anchorableDocument
                };

                fwc = new LayoutDocumentFloatingWindowControl(
                    fw as LayoutDocumentFloatingWindow)
                {
                    Width = fwWidth,
                    Height = fwHeight
                };
            }
            
            Layout.FloatingWindows.Add(fw);

            fwc.Owner = Window.GetWindow(this);

            _fwList.Add(fwc);
            
            Layout.CollectGarbage();

            fwc.AttachDrag();
            fwc.Show();
        }

        internal void StartDraggingFloatingWindowForPane(LayoutAnchorablePane paneModel)
        {
            var paneAsPositionableElement = paneModel as ILayoutPositionableElement;
            var paneAsWithActualSize = paneModel as ILayoutPositionableElementWithActualSize;

            double fwWidth = paneAsPositionableElement.FloatingWidth;
            double fwHeight = paneAsPositionableElement.FloatingHeight;

            if (fwWidth == 0.0)
                fwWidth = paneAsWithActualSize.ActualWidth;
            if (fwHeight == 0.0)
                fwHeight = paneAsWithActualSize.ActualHeight;

            var destPane = new LayoutAnchorablePane()
            {
                DockWidth = paneAsPositionableElement.DockWidth,
                DockHeight = paneAsPositionableElement.DockHeight,
                DockMinHeight = paneAsPositionableElement.DockMinHeight,
                DockMinWidth = paneAsPositionableElement.DockMinWidth
            };

            bool savePreviousContainer = paneModel.FindParent<LayoutFloatingWindow>() == null;

            while (paneModel.Children.Count > 0)
            {
                var contentModel = paneModel.Children[paneModel.Children.Count - 1] as LayoutAnchorable;

                if (savePreviousContainer)
                {
                    contentModel.PreviousContainer = paneModel;
                    contentModel.PreviousContainerIndex = paneModel.Children.Count - 1;
                }

                paneModel.RemoveChildAt(paneModel.Children.Count - 1);
                destPane.Children.Insert(0, contentModel);
            }


            LayoutFloatingWindow fw;
            LayoutFloatingWindowControl fwc;
            fw = new LayoutAnchorableFloatingWindow()
            {
                RootPanel = new LayoutAnchorablePaneGroup(
                    destPane)
            };

            fwc = new LayoutAnchorableFloatingWindowControl(
                fw as LayoutAnchorableFloatingWindow)
            {
                Width = fwWidth,
                Height = fwHeight
            };
            

            Layout.FloatingWindows.Add(fw);

            fwc.Owner = Window.GetWindow(this);

            _fwList.Add(fwc);

            Layout.CollectGarbage();

            fwc.AttachDrag();
            fwc.Show();

        }


        internal IEnumerable<LayoutFloatingWindowControl> GetFloatingWindowsByZOrder()
        {
            var parentWindow = Window.GetWindow(this);

            if (parentWindow == null)
                yield break;

            IntPtr windowParentHanlde = new WindowInteropHelper(parentWindow).Handle;
            
            IntPtr currentHandle = Win32Helper.GetWindow(windowParentHanlde, (uint)Win32Helper.GetWindow_Cmd.GW_HWNDFIRST);
            while (currentHandle != IntPtr.Zero)
            {
                LayoutFloatingWindowControl ctrl = _fwList.FirstOrDefault(fw => new WindowInteropHelper(fw).Handle == currentHandle);
                if (ctrl != null && ctrl.Model.Root.Manager == this)
                    yield return ctrl;

                currentHandle = Win32Helper.GetWindow(currentHandle, (uint)Win32Helper.GetWindow_Cmd.GW_HWNDNEXT);
            }
        }

        internal void RemoveFloatingWindow(LayoutFloatingWindowControl floatingWindow)
        {
            _fwList.Remove(floatingWindow);
        }
        #endregion

        #region OverlayWindow

        bool IOverlayWindowHost.HitTest(Point dragPoint)
        {
            Rect detectionRect = new Rect(this.PointToScreenDPI(new Point()), this.TransformActualSizeToAncestor());
            return detectionRect.Contains(dragPoint);
        }

        OverlayWindow _overlayWindow = null;
        void CreateOverlayWindow()
        {
            if (_overlayWindow == null)
                _overlayWindow = new OverlayWindow(this);
            Rect rectWindow = new Rect(this.PointToScreenDPI(new Point()), this.TransformActualSizeToAncestor());
            _overlayWindow.Left = rectWindow.Left;
            _overlayWindow.Top = rectWindow.Top;
            _overlayWindow.Width = rectWindow.Width;
            _overlayWindow.Height = rectWindow.Height;
        }

        void DestroyOverlayWindow()
        {
            if (_overlayWindow != null)
            {
                _overlayWindow.Close();
                _overlayWindow = null;
            }
        }

        IOverlayWindow IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
        {
            Debug.WriteLine("ShowOverlayWindow");
            CreateOverlayWindow();
            _overlayWindow.Owner = draggingWindow;
            _overlayWindow.Show();
            return _overlayWindow;
        }

        void IOverlayWindowHost.HideOverlayWindow()
        {
            Debug.WriteLine("HideOverlayWindow");
            _areas = null;
            _overlayWindow.Owner = null;
            _overlayWindow.Hide();
        }

        List<IDropArea> _areas = null;

        IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
        {
            if (_areas != null)
                return _areas;

            bool isDraggingDocuments = draggingWindow.Model is LayoutDocumentFloatingWindow;

            _areas = new List<IDropArea>();

            if (!isDraggingDocuments)
            {
                _areas.Add(new DropArea<DockingManager>(
                    this,
                    DropAreaType.DockingManager));

                foreach (var areaHost in this.FindVisualChildren<LayoutAnchorablePaneControl>())
                {
                    _areas.Add(new DropArea<LayoutAnchorablePaneControl>(
                        areaHost,
                        DropAreaType.AnchorablePane));
                }
            }

            foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneControl>())
            {
                _areas.Add(new DropArea<LayoutDocumentPaneControl>(
                    areaHost,
                    DropAreaType.DocumentPane));
            }

            foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneGroupControl>())
            {
                var documentGroupModel = areaHost.Model as LayoutDocumentPaneGroup;
                if (documentGroupModel.Children.Where(c => c.IsVisible).Count() == 0)
                {
                    _areas.Add(new DropArea<LayoutDocumentPaneGroupControl>(
                        areaHost,
                        DropAreaType.DocumentPaneGroup));
                }
            }

            return _areas;
        }


        #endregion

        #region AutoHide
        public void ToggleAutoHide(LayoutAnchorable anchorableModel)
        {
            #region Anchorable is already auto hidden
            if (anchorableModel.Parent is LayoutAnchorGroup)
            {
                var parentGroup = anchorableModel.Parent as LayoutAnchorGroup;
                var parentSide = parentGroup.Parent as LayoutAnchorSide;
                var previousContainer = parentGroup.PreviousContainer;

                if (previousContainer == null)
                {
                    AnchorSide side = (parentGroup.Parent as LayoutAnchorSide).Side;
                    switch (side)
                    {
                        case AnchorSide.Right:
                            if (parentGroup.Root.RootPanel.Orientation == Orientation.Horizontal)
                            {
                                previousContainer = new LayoutAnchorablePane();
                                parentGroup.Root.RootPanel.Children.Add(previousContainer);
                            }
                            else
                            {
                                previousContainer = new LayoutAnchorablePane();
                                LayoutPanel panel = new LayoutPanel() { Orientation = Orientation.Horizontal };
                                LayoutRoot root = parentGroup.Root as LayoutRoot;
                                LayoutPanel oldRootPanel = parentGroup.Root.RootPanel as LayoutPanel;
                                root.RootPanel = panel;
                                panel.Children.Add(oldRootPanel);
                                panel.Children.Add(previousContainer);
                            }
                            break;
                        case AnchorSide.Left:
                            if (parentGroup.Root.RootPanel.Orientation == Orientation.Horizontal)
                            {
                                previousContainer = new LayoutAnchorablePane();
                                parentGroup.Root.RootPanel.Children.Insert(0, previousContainer);
                            }
                            else
                            {
                                previousContainer = new LayoutAnchorablePane();
                                LayoutPanel panel = new LayoutPanel() { Orientation = Orientation.Horizontal };
                                LayoutRoot root = parentGroup.Root as LayoutRoot;
                                LayoutPanel oldRootPanel = parentGroup.Root.RootPanel as LayoutPanel;
                                root.RootPanel = panel;
                                panel.Children.Add(previousContainer);
                                panel.Children.Add(oldRootPanel);
                            }
                            break;
                        case AnchorSide.Top:
                            if (parentGroup.Root.RootPanel.Orientation == Orientation.Vertical)
                            {
                                previousContainer = new LayoutAnchorablePane();
                                parentGroup.Root.RootPanel.Children.Insert(0, previousContainer);
                            }
                            else
                            {
                                previousContainer = new LayoutAnchorablePane();
                                LayoutPanel panel = new LayoutPanel() { Orientation = Orientation.Vertical };
                                LayoutRoot root = parentGroup.Root as LayoutRoot;
                                LayoutPanel oldRootPanel = parentGroup.Root.RootPanel as LayoutPanel;
                                root.RootPanel = panel;
                                panel.Children.Add(previousContainer);
                                panel.Children.Add(oldRootPanel);
                            }
                            break;
                        case AnchorSide.Bottom:
                            if (parentGroup.Root.RootPanel.Orientation == Orientation.Vertical)
                            {
                                previousContainer = new LayoutAnchorablePane();
                                parentGroup.Root.RootPanel.Children.Add(previousContainer);
                            }
                            else
                            {
                                previousContainer = new LayoutAnchorablePane();
                                LayoutPanel panel = new LayoutPanel() { Orientation = Orientation.Vertical };
                                LayoutRoot root = parentGroup.Root as LayoutRoot;
                                LayoutPanel oldRootPanel = parentGroup.Root.RootPanel as LayoutPanel;
                                root.RootPanel = panel;
                                panel.Children.Add(oldRootPanel);
                                panel.Children.Add(previousContainer);
                            }
                            break;
                    }
                }


                foreach (var anchorableToToggle in parentGroup.Children.ToArray())
                    previousContainer.Children.Add(anchorableToToggle);

                parentSide.Children.Remove(parentGroup);

                HideAutoHideWindow();
            }
            #endregion
            #region Anchorable is docked
            else if (anchorableModel.Parent is LayoutAnchorablePane)
            {
                var parentPane = anchorableModel.Parent as LayoutAnchorablePane;

                var newAnchorGroup = new LayoutAnchorGroup() { PreviousContainer = parentPane };

                foreach (var anchorableToImport in parentPane.Children.ToArray())
                    newAnchorGroup.Children.Add(anchorableToImport);

                //detect anchor side for the pane
                var anchorSide = parentPane.GetSide();

                switch (anchorSide)
                {
                    case AnchorSide.Right:
                        Layout.RightSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Left:
                        Layout.LeftSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Top:
                        Layout.TopSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Bottom:
                        Layout.BottomSide.Children.Add(newAnchorGroup);
                        break;
                }
            }
            #endregion
        }

        #endregion

        #region LayoutDocument & LayoutAnchorable Templates

        #region DocumentTemplate

        /// <summary>
        /// DocumentTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty DocumentTemplateProperty =
            DependencyProperty.Register("DocumentTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplate)null,
                    new PropertyChangedCallback(OnDocumentTemplateChanged)));

        /// <summary>
        /// Gets or sets the DocumentTemplate property.  This dependency property 
        /// indicates data template to used when creating document contents.
        /// </summary>
        public DataTemplate DocumentTemplate
        {
            get { return (DataTemplate)GetValue(DocumentTemplateProperty); }
            set { SetValue(DocumentTemplateProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DocumentTemplate property.
        /// </summary>
        private static void OnDocumentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentTemplateChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DocumentTemplate property.
        /// </summary>
        protected virtual void OnDocumentTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region DocumentTemplateSelector

        /// <summary>
        /// DocumentTemplateSelector Dependency Property
        /// </summary>
        public static readonly DependencyProperty DocumentTemplateSelectorProperty =
            DependencyProperty.Register("DocumentTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplateSelector)null,
                    new PropertyChangedCallback(OnDocumentTemplateSelectorChanged)));

        /// <summary>
        /// Gets or sets the DocumentTemplateSelector property.  This dependency property 
        /// indicates the data template selector to use when creating data templates for documents.
        /// </summary>
        public DataTemplateSelector DocumentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DocumentTemplateSelectorProperty); }
            set { SetValue(DocumentTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DocumentTemplateSelector property.
        /// </summary>
        private static void OnDocumentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentTemplateSelectorChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DocumentTemplateSelector property.
        /// </summary>
        protected virtual void OnDocumentTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #endregion

        #region DocumentsSource

        /// <summary>
        /// DocumentsSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty DocumentsSourceProperty =
            DependencyProperty.Register("DocumentsSource", typeof(IEnumerable), typeof(DockingManager),
                new FrameworkPropertyMetadata((IEnumerable)null,
                    new PropertyChangedCallback(OnDocumentsSourceChanged)));

        /// <summary>
        /// Gets or sets the DocumentsSource property.  This dependency property 
        /// indicates the source collection of documents.
        /// </summary>
        public IEnumerable DocumentsSource
        {
            get { return (IEnumerable)GetValue(DocumentsSourceProperty); }
            set { SetValue(DocumentsSourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DocumentsSource property.
        /// </summary>
        private static void OnDocumentsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentsSourceChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DocumentsSource property.
        /// </summary>
        protected virtual void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            DetachDocumentsSource(Layout, e.OldValue as IEnumerable);
            AttachDocumentsSource(Layout, e.NewValue as IEnumerable);
        }


        void AttachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
        {
            if (documentsSource == null)
                return;

            if (layout == null)
                return;

            if (layout.Descendents().OfType<LayoutDocument>().Any())
                throw new InvalidOperationException("Unable to set the DocumentsSource property if LayoutDocument objects are already present in the model");

            var documents = documentsSource as IEnumerable;
            LayoutDocumentPane documentPane = null;
            if (layout.LastFocusedDocument != null)
            {
                documentPane = layout.LastFocusedDocument.Parent as LayoutDocumentPane;
            }

            if (documentPane == null)
            {
                documentPane = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            }

            if (documentPane != null)
            {
                foreach (var documentToImport in (documentsSource as IEnumerable))
                {
                    documentPane.Children.Add(new LayoutDocument() { Content = documentToImport });
                }
            }

            var documentsSourceAsNotifier = documentsSource as INotifyCollectionChanged;
            if (documentsSourceAsNotifier != null)
                documentsSourceAsNotifier.CollectionChanged += new NotifyCollectionChangedEventHandler(documentsSourceElementsChanged);
        }

        void documentsSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Layout == null)
                return;

            //handle remove
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    var documentsToRemove = Layout.Descendents().OfType<LayoutDocument>().Where(d => e.OldItems.Contains(d.Content)).ToArray();
                    foreach (var documentToRemove in documentsToRemove)
                    {
                        (documentToRemove.Parent as LayoutDocumentPane).Children.Remove(
                            documentToRemove);
                    }
                }
            }

            //handle add
            if (e.NewItems != null &&
                (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                if (e.NewItems != null)
                {
                    LayoutDocumentPane documentPane = null;
                    if (Layout.LastFocusedDocument != null)
                    {
                        documentPane = Layout.LastFocusedDocument.Parent as LayoutDocumentPane;
                    }

                    if (documentPane == null)
                    {
                        documentPane = Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                    }

                    if (documentPane != null)
                    {
                        foreach (var documentToImport in e.NewItems)
                        {
                            documentPane.Children.Add(new LayoutDocument() { Content = documentToImport });
                        }
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //NOTE: I'm going to clear every document present in layout but
                //some documents may have been added directly to layout, for now I clear them too
                var documentsToRemove = Layout.Descendents().OfType<LayoutDocument>().ToArray();
                foreach (var documentToRemove in documentsToRemove)
                {
                    (documentToRemove.Parent as LayoutDocumentPane).Children.Remove(
                        documentToRemove);
                }                
            }
        }

        void DetachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
        {
            if (documentsSource == null)
                return;

            if (layout == null)
                return;

            var documentsToRemove = layout.Descendents().OfType<LayoutDocument>()
                .Where(d => documentsSource.Contains(d.Content)).ToArray();

            foreach (var documentToRemove in documentsToRemove)
            {
                (documentToRemove.Parent as LayoutDocumentPane).Children.Remove(
                    documentToRemove);
            }

            var documentsSourceAsNotifier = documentsSource as INotifyCollectionChanged;
            if (documentsSourceAsNotifier != null)
                documentsSourceAsNotifier.CollectionChanged -= new NotifyCollectionChangedEventHandler(documentsSourceElementsChanged);
        }

        #region DocumentCloseCommand

        static ICommand _defaultDocumentCloseCommand = new RelayCommand((p)=>ExecuteDocumentCloseCommand(p), (p) => CanExecuteDocumentCloseCommand(p));

        /// <summary>
        /// DocumentCloseCommand Dependency Property
        /// </summary>
        public static readonly DependencyProperty DocumentCloseCommandProperty =
            DependencyProperty.Register("DocumentCloseCommand", typeof(ICommand), typeof(DockingManager),
                new FrameworkPropertyMetadata(_defaultDocumentCloseCommand,
                    new PropertyChangedCallback(OnDocumentCloseCommandChanged),
                    new CoerceValueCallback(CoerceDocumentCloseCommandValue)));

        /// <summary>
        /// Gets or sets the DocumentCloseCommand property.  This dependency property 
        /// indicates the command to execute when user click the document close button.
        /// </summary>
        public ICommand DocumentCloseCommand
        {
            get { return (ICommand)GetValue(DocumentCloseCommandProperty); }
            set { SetValue(DocumentCloseCommandProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DocumentCloseCommand property.
        /// </summary>
        private static void OnDocumentCloseCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentCloseCommandChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DocumentCloseCommand property.
        /// </summary>
        protected virtual void OnDocumentCloseCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces the DocumentCloseCommand value.
        /// </summary>
        private static object CoerceDocumentCloseCommandValue(DependencyObject d, object value)
        {
            if (value == null)
                return _defaultDocumentCloseCommand;

            return value;
        }

        private static bool CanExecuteDocumentCloseCommand(object parameter)
        {
            return true;
        }

        private static void ExecuteDocumentCloseCommand(object parameter)
        {
            var document = parameter as LayoutDocument;
            if (document == null)
                return;

            var dockingManager = document.Root.Manager;
            dockingManager._ExecuteDocumentCloseCommand(document);
        }

        void _ExecuteDocumentCloseCommand(LayoutDocument document)
        {
            if (DocumentClosing != null)
            {
                var evargs = new DocumentClosingEventArgs(document);
                DocumentClosing(this, evargs);
                if (evargs.Cancel)
                    return;
            }

            document.Close();

            if (DocumentClose != null)
            { 
                var evargs = new DocumentCloseEventArgs(document);
                DocumentClose(this, evargs);
            }
        }

        /// <summary>
        /// Event fired when a document is about to be closed
        /// </summary>
        /// <remarks>Subscribers have the opportuniy to cancel the operation.</remarks>
        public event EventHandler<CancelEventArgs> DocumentClosing;

        /// <summary>
        /// Event fired after a document is closed
        /// </summary>
        public event EventHandler DocumentClose;



        #endregion





        #endregion


    }
}
