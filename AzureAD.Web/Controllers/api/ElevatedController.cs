using AzureAD.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace AzureAD.Web.Controllers.api
{
	[HostAuthentication("OAuth2Bearer")]
	[ClaimsAuthorize("roles", "Elevated")]
	public class ElevatedController : ApiController
	{
		// GET api/<controller>
		public IEnumerable<string> Get()
		{
			return ClaimsPrincipal.Current.Claims.Where(x => x.Type == "roles").Select(x => x.Value);
		}
	}
}