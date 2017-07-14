using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.CustomHelpers
{
    //public static class MyCustomHelper //currently not using
    //{
    //    public static MvcHtmlString SubmitButton(this HtmlHelper helper, string buttonText)
    //    {
    //        string str = "<input type=\"submit\" value=\"" + buttonText + "\" />";
    //        return new MvcHtmlString(str);
    //    }

    //    public static string ExtensionMethod(this HtmlHelper htmlHelper)
    //    {
    //        var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
    //        var url = urlHelper.Action("Home", "Index");

    //        return url;
    //    }
    //    public static string AbsoluteAction(this UrlHelper url, string actionName, string controllerName, object routeValues = null) {
    //        string scheme = url.RequestContext.HttpContext.Request.Url.Scheme;

    //        return url.Action(actionName, controllerName, routeValues, scheme);
    //    }

    //}



    public static class CustomLink
    {
        //Custom html helper that takes in typical 'Link' attributes and allows for font awesome icons to be used in the link span
        //This overload may not be used anymore
        //public static IHtmlString AjaxActionLinkWithFontAwesome(string src, string targetId, string mode, string method, string output, string fontShortcut, string linkClass, string ajaxComplete, string id ) {

        //    string formattedString =  String.Format("<a href = '{0}' class = '{6}' data-ajax-update = '{1}' data-ajax-mode = '{2}' data-ajax-method = '{3}' data-ajax = 'true' data-ajax-success = '{7}' data-ajax-complete = 'rememeberSort($(this))'> {4} <i  id='{8}' class='fa {5}'></i></a>", src, targetId, mode, method, output, fontShortcut, linkClass, ajaxComplete, id);

        //    IHtmlString nonEncodedString = new HtmlString(formattedString);

        //    return nonEncodedString;
        //}

        public static IHtmlString AjaxActionLinkWithFontAwesome(this UrlHelper url,Int32 folderId, string thisfilter, string thisNavBarGroup, string thisNavBarItem, string thisSearchTerm, string thisIssueYearMinRange, string thisIssueYearMaxRange, string targetId, string mode, string method, string output, string fontShortcut, bool ascending, string linkClass, string ajaxComplete, string id)
        {
            string scheme = url.RequestContext.HttpContext.Request.Url.Scheme;

            string src = url.Action("Index", "PublicVM", new { publicId = folderId, filter = thisfilter, navBarGroup = thisNavBarGroup, navBarItem = thisNavBarItem, searchTerm = thisSearchTerm, IssueYearMinRange = thisIssueYearMinRange, thisIssueYearMaxRange = thisIssueYearMaxRange }, scheme);

            string formattedString = String.Format("<a href = '{0}' class = '{6}' data-ajax-update = '{1}' data-ajax-mode = '{2}' data-ajax-method = '{3}' data-ajax = 'true' data-ajax-success = '{7}' data-ajax-complete = 'rememeberSort($(this), {9})'> {4} <i  id='{8}' class='fa {5}'></i></a>", src, targetId, mode, method, output, fontShortcut, linkClass, ajaxComplete, id, ascending.ToString().ToLower());

            IHtmlString nonEncodedString = new HtmlString(formattedString);

            return nonEncodedString;
        }
    }
}