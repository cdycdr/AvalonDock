using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AvalonDock.Controls
{
    public class DropDownButton : ToggleButton
    {
        public DropDownButton()
        {
            SetBinding(IsCheckedProperty, new Binding("DropDownContextMenu.IsOpen") { Source = this, Mode = BindingMode.OneWay });
        }

        #region DropDownContextMenu

        /// <summary>
        /// DropDownContextMenu Dependency Property
        /// </summary>
        public static readonly DependencyProperty DropDownContextMenuProperty =
            DependencyProperty.Register("DropDownContextMenu", typeof(ContextMenu), typeof(DropDownButton),
                new FrameworkPropertyMetadata((ContextMenu)null));

        /// <summary>
        /// Gets or sets the DropDownContextMenu property.  This dependency property 
        /// indicates the context menu to show when the button is clicked.
        /// </summary>
        public ContextMenu DropDownContextMenu
        {
            get { return (ContextMenu)GetValue(DropDownContextMenuProperty); }
            set { SetValue(DropDownContextMenuProperty, value); }
        }

        #endregion

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {

            base.OnMouseLeftButtonDown(e);
        }
        protected override void OnClick()
        {
            if (DropDownContextMenu != null)
            {
                DropDownContextMenu.PlacementTarget = this;
                DropDownContextMenu.Placement = PlacementMode.Bottom;

                DropDownContextMenu.IsOpen = true;
            }
        }
    }
}
