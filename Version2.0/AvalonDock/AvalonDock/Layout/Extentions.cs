using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace AvalonDock.Layout
{
    public static class Extentions
    {
        public static IEnumerable<ILayoutElement> Descendents(this ILayoutElement element)
        {
            var container = element as ILayoutContainer;
            if (container != null)
            {
                foreach (var childElement in container.Children)
                {
                    yield return childElement;
                    foreach (var childChildElement in childElement.Descendents())
                        yield return childChildElement;
                }
            }
        }

        public static T FindParent<T>(this ILayoutElement element) where T : ILayoutContainer
        { 
            var parent = element.Parent;
            while (parent != null &&
                !(parent is T))
                parent = parent.Parent;


            return (T)parent;
        }

        public static bool ContainsChildOfType<T>(this ILayoutContainer element)
        {
            foreach (var childElement in element.Descendents())
                if (childElement is T)
                    return true;

            return false;
        }

        public static bool ContainsChildOfType<T, S>(this ILayoutContainer element)
        {
            foreach (var childElement in element.Descendents())
                if (childElement is T || childElement is S)
                    return true;

            return false;
        }

        public static bool IsOfType<T, S>(this ILayoutContainer element)
        {
            return element is T || element is S;
        }

    }
}
