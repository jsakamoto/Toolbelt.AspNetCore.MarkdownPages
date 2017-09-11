using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbelt.AspNetCore.MarkdownPages
{
    public class MarkdownPagesOptions
    {
        public List<string> MetaTags { get; set; } = new List<string> {
            "<meta charset=\"utf-8\" />",
            "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge\" />",
            "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />"
        };

        public List<string> CssLinks { get; set; } = new List<string> {
            "//jsakamoto.github.io/Toolbelt.AspNetCore.MarkdownPages/css/theme/default/style.css"
        };

        public bool EnableAutoLink { get; set; } = true;
    }
}
