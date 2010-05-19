using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AvalonDock
{
    /// <summary>
    /// Defines commands that can be applied to a dockable pane
    /// </summary>
    public sealed class DockablePaneCommands
    {
        private static object syncRoot = new object();

        private static RoutedUICommand closeCommand = null;

        /// <summary>
        /// This command closes the <see cref="DockablePane"/> and closes all the contained <see cref="DockableContent"/>s inside it
        /// </summary>
        public static RoutedUICommand Close
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == closeCommand)
                    {
                        closeCommand = new RoutedUICommand("C_lose", "Close", typeof(DockablePaneCommands));
                    }
                }
                return closeCommand;
            }
        }

        private static RoutedUICommand hideCommand = null;

        /// <summary>
        /// This command closes the <see cref="DockablePane"/> and hides all the contained <see cref="DockableContent"/>s inside it
        /// </summary>
        public static RoutedUICommand Hide
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == hideCommand)
                    {
                        hideCommand = new RoutedUICommand("_Hide", "Hide", typeof(DockablePaneCommands));
                    }
                }
                return hideCommand;
            }
        }

        private static RoutedUICommand autoHideCommand = null;

        /// <summary>
        /// This commands auto-hides the pane with all contained <see cref="DockableContent"/>s inside it
        /// </summary>
        public static RoutedUICommand ToggleAutoHide
        {
            get
            {
                lock (syncRoot)
                {
                    if (null == autoHideCommand)
                    {
                        autoHideCommand = new RoutedUICommand("A_utohide", "AutoHide", typeof(DockablePaneCommands));
                    }
                }
                return autoHideCommand;
            }
        }

    }
}
