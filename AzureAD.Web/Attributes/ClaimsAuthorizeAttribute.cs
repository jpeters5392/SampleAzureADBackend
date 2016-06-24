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
	public class ClaimsAuthorizeAttribute : AuthorizeAttribute
	{
		private string[] claimValues;
		private string claimName;

		public ClaimsAuthorizeAttribute(string claimName, params string[] values)
		{
			this.claimName = claimName;
			this.claimValues = values;
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