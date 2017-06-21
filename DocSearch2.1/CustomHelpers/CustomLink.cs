using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocSearch2._1.CustomHelpers
{
    public static class CustomLink
    {
        //Custom html helper that takes in typical 'Link' attributes and allows for font awesome icons to be used in the link span
        public static string AjaxActionLinkWithFontAwesome(string src, string targetId, string mode, string method, string output, string fontShortcut, string linkClass, string ajaxComplete, string id ) {

            return String.Format("<a href = '{0}' class = '{6}' data-ajax-update = '{1}' data-ajax-mode = '{2}' data-ajax-method = '{3}' data-ajax = 'true' data-ajax-success = '{7}' data-ajax-complete = 'abc($(this))'> {4} <i  id='{8}' class='fa {5}'></i></a>", src, targetId, mode, method, output, fontShortcut, linkClass, ajaxComplete, id);
        }
    }
}