using AzureAD.Web.Models;
using AzureAD.Web.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureAD.Web.Controllers
{
    public class AccountController : Controller
    {
		// GET: SignIn
		public void SignIn()
		{
			//// Send an OpenID Connect sign-in request.
			//if (!Request.IsAuthenticated)
			//{
			HttpContext.GetOwinContext()
				.Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
			//}

		}

		public ActionResult Info()
		{
			if (this.Request.IsAuthenticated)
			{
				var user = ClaimsPrincipal.Current;
				if (user != null)
				{
					var claimsIdentity = user.Identity as ClaimsIdentity;
					if (claimsIdentity != null)
					{
						var role = claimsIdentity.Claims.Where(x => x.Type == "roles").FirstOrDefault();
						var claims = user.Claims;
						IList<UserClaim> userClaims = new List<UserClaim>();
						foreach (var claim in claims)
						{
							var userClaim = new UserClaim(claim);
							userClaims.Add(userClaim);
						}

						return Json(userClaims, JsonRequestBehavior.AllowGet);
					}

					return Content("You do not have claims");
				}
				else
				{
					return Content("You do not have claims");
				}
			}
			else
			{
				return Content("You are not authenticated");
			}
		}

		public ActionResult Groups()
		{
			if (this.Request.IsAuthenticated)
			{
				var user = ClaimsPrincipal.Current;
				if (user != null)
				{
					var claimsIdentity = user.Identity as ClaimsIdentity;
					var groupIds = claimsIdentity.Claims.Where(x => x.Type == "groups");
					IList<string> groupNames = new List<string>();

					var service = new GroupService();
					
					foreach (var groupId in groupIds)
					{
						groupNames.Add(service.RetrieveGroupName(groupId.Value));
					}

						return Json(groupNames, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Content("You do not have claims");
				}
			}
			else
			{
				return Content("You are not authenticated");
			}
		}

		public void SignOut()
		{
			Session.Abandon();

			// Send an OpenID Connect sign-out request.
			HttpContext.GetOwinContext().Authentication.SignOut(OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);

		}
	}
}