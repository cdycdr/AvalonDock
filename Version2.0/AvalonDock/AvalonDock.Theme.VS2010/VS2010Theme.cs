using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock.Themes
{
    public class VS2010Theme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri(
                "/AvalonDock.Themes.VS2010;component/Theme.xaml", 
                UriKind.Relative);  
        }
    }
}
