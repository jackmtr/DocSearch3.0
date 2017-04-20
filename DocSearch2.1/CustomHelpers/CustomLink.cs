using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocSearch2._1.CustomHelpers
{
    public static class CustomLink
    {
        public static string AjaxActionLinkWithFontAwesome(string src, string targetId, string mode, string method, string output, string fontShortcut ) {

            return String.Format("<a href = '{0}' data-ajax-update = '{1}' data-ajax-mode = '{2}' data-ajax-method = '{3}' data-ajax = 'true'> {4} <i class='fa {5}'></i></a>", src, targetId, mode, method, output, fontShortcut);
        }
    }
}