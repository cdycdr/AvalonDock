using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{
    public abstract class LayoutItem : FrameworkElement
    {
        internal LayoutItem(LayoutContent model)
        {
            Model = model;
        }


        public LayoutContent Model
        {
            get;
            private set;
        }

        #region Title

        /// <summary>
        /// Title Dependency Property
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LayoutItem),
                new FrameworkPropertyMetadata((string)null,
                    new PropertyChangedCallback(OnTitleChanged)));

        /// <summary>
        /// Gets or sets the Title property.  This dependency property 
        /// indicates the title of the element.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Title property.
        /// </summary>
        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnTitleChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Title property.
        /// </summary>
        protected virtual void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            Model.Title = (string)e.NewValue;
        }

        #endregion


    }
}
