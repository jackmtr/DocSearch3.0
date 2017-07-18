using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.Filters
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext); //gets the authorization info of the person making the request

            if (!isAuthorized) {

                return false; //checks if the user is loged into windows
            }

            if (WindowAuth.WindowLoginName != httpContext.User.Identity.Name) {

                return false;  //checks if the user making the request is the same as when starting application
            }


            if (httpContext.User.IsInRole("IT-ops"))
            {
                return true;
            }
            else
            {
                return false;
            }


            //if (httpContext.User.IsInRole("IT-ops1")) //this group doesnt exist, simulates being denied
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //can do checks for what info to give user when not authorized arrises

            filterContext.Result = new ViewResult {
                ViewName = "~/Views/Shared/Errors.cshtml"
            };
        }
    }
}