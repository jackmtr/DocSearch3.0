using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DocSearch2._1.CustomHelpers
{
    //Used to simplifly the table header creation as most of the code is generic
    public class CustomTableHeader
    {
        public static IHtmlString PublicTableHeader(string filter, string labelText, string varName) {

            //string abc = CustomLink.AjaxActionLinkWithFontAwesome((Url.Action("Index", "PublicVM", new
            //                {
            //                    publicId = Model.First().Folder_ID,
            //                    filter = "document",
            //                    navBarGroup = ViewData["currentNav"],
            //                    navBarItem = ViewData["currentNavTitle"],
            //                    searchTerm = TempData["SearchTerm"],
            //                    IssueYearMinRange = Model.OrderBy(r => r.IssueDate).First().IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture),
            //                    IssueYearMaxRange = Model.OrderBy(r => r.IssueDate).Last().IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture)
            //                }), "#public_table", "replace-with", "GET", "Type of Document", "fa-sort", "filterLink", "postNavbar", "document"));



            string abc = String.Format("<th> @CustomLink.AjaxActionLinkWithFontAwesome(Url.Action('Index', 'PublicVM', new {{ publicId = Model.First().Folder_ID, filter = '{0}', navBarGroup = ViewData['currentNav'], navBarItem = ViewData['currentNavTitle'], searchTerm = TempData['SearchTerm'], IssueYearMinRange = Model.OrderBy(r => r.IssueDate).First().IssueDate.Value.ToString('yyyy', CultureInfo.InvariantCulture), IssueYearMaxRange = Model.OrderBy(r => r.IssueDate).Last().IssueDate.Value.ToString('yyyy', CultureInfo.InvariantCulture) }}), '#public_table', 'replace-with', 'GET', '{1}', 'fa - sort', 'filterLink', 'postNavbar', '{2}')</th>", filter, labelText, varName);
            IHtmlString ccc = new HtmlString(abc);

            return ccc;


        }

                                                                                                                                                                                                                                                                                                                                                                                                //        <th>
                                                                                                                                                                                                                                                                                                                                                                                                //    <!--1-->
                                                                                                                                                                                                                                                                                                                                                                                                //    @Html.Raw(CustomLink.AjaxActionLinkWithFontAwesome(Url.Action("Index", "PublicVM", new
                                                                                                                                                                                                                                                                                                                                                                                                //{
                                                                                                                                                                                                                                                                                                                                                                                                //    publicId = Model.First().Folder_ID,
                                                                                                                                                                                                                                                                                                                                                                                                //    filter = "document",
                                                                                                                                                                                                                                                                                                                                                                                                //    navBarGroup = ViewData["currentNav"],
                                                                                                                                                                                                                                                                                                                                                                                                //    navBarItem = ViewData["currentNavTitle"],
                                                                                                                                                                                                                                                                                                                                                                                                //    searchTerm = TempData["SearchTerm"],
                                                                                                                                                                                                                                                                                                                                                                                                //    IssueYearMinRange = Model.OrderBy(r => r.IssueDate).First().IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture),
                                                                                                                                                                                                                                                                                                                                                                                                //    IssueYearMaxRange = Model.OrderBy(r => r.IssueDate).Last().IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture)
                                                                                                                                                                                                                                                                                                                                                                                                //}), "#public_table", "replace-with", "GET", "Type of Document", "fa-sort", "filterLink", "postNavbar", "document"))
                                                                                                                                                                                                                                                                                                                                                                                                //</th>

                       //         <th>
                       //     @Html.Raw(CustomLink.AjaxActionLinkWithFontAwesome(Url.Action("Index", "PublicVM", new
                       //{
                       //    publicId = Model.First().Folder_ID,
                       //    filter = "issue",
                       //    navBarGroup = ViewData["currentNav"],
                       //    navBarItem = ViewData["currentNavTitle"],
                       //    searchTerm = TempData["SearchTerm"],
                       //    IssueYearMinRange = Model.OrderBy(r => r.IssueDate).First().IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture),
                       //    IssueYearMaxRange = Model.OrderBy(r => r.IssueDate).Last().IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture)
                       //}), "#public_table", "replace-with", "GET", "Issue Date", "fa-sort", "filterLink", "postNavbar", "issue"))
                       // </th>
    }
}