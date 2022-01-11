using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace webScrapWPF;
public static class Utilities
{
    // public static string regPage         = "<article class=\"project-card\"(.*\n)*?.*?</article>";
    // public static string regexIdentifier = "&@||@";
    // public static string url             = "https://www.modrinth.com/mods?q=&o=&@||@&f=categories%3Afabric";

    // ERRORS  

    public static class Colors
    {
        public static SolidColorBrush darkThemeTextColor = new (Color.FromRgb(182, 183, 186));
    }

    public enum ErrorCodes
    {
        noError             = 0,
        emptyPageError      = 1,
        unclasifiedError    = 2,
        
    }
}

