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
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public class LayoutAnchorableControl : Control
    {
        static LayoutAnchorableControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorableControl)));
            FocusableProperty.OverrideMetadata(typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(false));
        }

        public LayoutAnchorableControl()
        {
        }


        #region Model

        /// <summary>
        /// Model Dependency Property
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutAnchorable), typeof(LayoutAnchorableControl),
                new FrameworkPropertyMetadata((LayoutAnchorable)null,
                    new PropertyChangedCallback(OnModelChanged)));

        /// <summary>
        /// Gets or sets the Model property.  This dependency property 
        /// indicates the model attached to this view.
        /// </summary>
        public LayoutAnchorable Model
        {
            get { return (LayoutAnchorable)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Model property.
        /// </summary>
        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableControl)d).OnModelChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Model property.
        /// </summary>
        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateLogicalParent();
        }

        void UpdateLogicalParent()
        {
            var parentPaneControl = this.FindVisualAncestor<LayoutAnchorablePaneControl>();

            if (Model != null &&
                Model.Content != null &&
                Model.Content is DependencyObject)
            {
                var oldLogicalParentPaneControl = LogicalTreeHelper.GetParent(Model.Content as DependencyObject)
                    as ILogicalChildrenContainer;

                if (oldLogicalParentPaneControl == parentPaneControl)
                    return;

                if (oldLogicalParentPaneControl != null)
                    oldLogicalParentPaneControl.InternalRemoveLogicalChild(Model.Content);
            }

            if (Model != null &&
                parentPaneControl != null &&
                Model.Content != null &&
                Model.Content is DependencyObject)
            {
                ((ILogicalChildrenContainer)parentPaneControl).InternalAddLogicalChild(Model.Content);
                BindingHelper.RebindInactiveBindings(Model.Content as DependencyObject);
            }

        }

        #endregion

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Model.IsActive = true;

            base.OnGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
        }


    }
}
