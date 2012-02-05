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
                    new PropertyChangedCallback(OnLayoutChanged)));

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
        /// Provides derived classes an opportunity to handle changes to the Layout property.
        /// </summary>
        protected virtual void OnLayoutChanged(LayoutRoot oldLayout, LayoutRoot newLayout)
        {
            if (oldLayout != null)
            {
                oldLayout.PropertyChanged -= new PropertyChangedEventHandler(OnLayoutRootPropertyChanged);
            }

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
        }

        public event EventHandler LayoutChanged;


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
                var templateModelView = new LayoutAnchorSideControl(model as LayoutAnchorSide) { Template = AnchorSideTemplate };
                return templateModelView;
            }
            if (model is LayoutAnchorGroup)
            {
                var templateModelView = new LayoutAnchorGroupControl(model as LayoutAnchorGroup) { Template = AnchorGroupTemplate };
                return templateModelView;
            }

            if (model is LayoutDocumentPane)
            {
                var templateModelView = new LayoutDocumentPaneControl(model as LayoutDocumentPane) { Style = DocumentPaneControlStyle };

                return templateModelView;
            }
            if (model is LayoutAnchorablePane)
            {
                var templateModelView = new LayoutAnchorablePaneControl(model as LayoutAnchorablePane) { Style = AnchorablePaneControlStyle };

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
                var templateModelView = new LayoutDocumentControl() { Model = model as LayoutDocument};

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

            while (paneModel.Children.Count > 0)
            {
                var contentModel = paneModel.Children[paneModel.Children.Count - 1] as LayoutAnchorable;

                contentModel.PreviousContainer = paneModel;
                contentModel.PreviousContainerIndex = paneModel.Children.Count - 1;

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
            Layout.FloatingWindows.Remove(floatingWindow.Model as LayoutFloatingWindow);
            _fwList.Remove(floatingWindow);
            Layout.CollectGarbage();
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
                    if (areaHost is LayoutAnchorablePaneControl)
                    {
                        _areas.Add(new DropArea<LayoutAnchorablePaneControl>(
                            areaHost,
                            DropAreaType.AnchorablePane));
                    }
                }
            }

            foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneControl>())
            {
                if (areaHost is LayoutDocumentPaneControl)
                {
                    _areas.Add(new DropArea<LayoutDocumentPaneControl>(
                        areaHost,
                        DropAreaType.DocumentPane));
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


    }
}
