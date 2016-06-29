using AzureAD.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace AzureAD.Web.Attributes
{
	/// <summary>
	/// This is a little simplistic, see https://blogs.technet.microsoft.com/enterprisemobility/2014/12/18/azure-active-directory-now-with-group-claims-and-application-roles/.
	/// If the user has too many groups, instead of sending the "groups" claim, they will pass "_claim_names" and "_claim_sources" which will define endpoints that when called will return the groups.
	/// So ideally, when validating "groups", we would need to first check for those keys and if they exist then use those.  Otherwise we can use the "groups" claim.
	/// </summary>
	public class ClaimsAuthorizeAttribute : AuthorizeAttribute
	{
		private string[] claimValues;
		private string claimName;

		public ClaimsAuthorizeAttribute(string claimName, params string[] values)
		{
			this.claimName = claimName;

			if (claimName == "groups")
			{
				var groupService = new GroupService();
				IList<string> groups = new List<string>();
				foreach (var value in values)
				{
					groups.Add(groupService.RetrieveGroupId(value));
				}

				this.claimValues = groups.ToArray();
			}
			else
			{
				this.claimValues = values;
			}
		}

		public override bool AllowMultiple
		{
			get
			{
				return base.AllowMultiple;
			}
		}

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var user = ClaimsPrincipal.Current as ClaimsPrincipal;
			if (user != null)
			{
				if (user.Claims.Where(x => x.Type == claimName && claimValues.Contains(x.Value)).Any())
				{
					base.OnAuthorization(actionContext);
					return;
				}
			}

			base.HandleUnauthorizedRequest(actionContext);
		}

		public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
		{
			var user = ClaimsPrincipal.Current as ClaimsPrincipal;
			if (user != null)
			{
				if (user.Claims.Where(x => x.Type == claimName && claimValues.Contains(x.Value)).Any())
				{
					return base.OnAuthorizationAsync(actionContext, cancellationToken);
				}
			}

			base.HandleUnauthorizedRequest(actionContext);
			return Task.FromResult(0);
		}
	}
}