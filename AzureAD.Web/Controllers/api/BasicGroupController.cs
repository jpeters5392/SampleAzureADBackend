using AzureAD.Web.Attributes;
using AzureAD.Web.Services;
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
	[ClaimsAuthorize("groups", "Basic Users")]
	public class BasicGroupController : ApiController
	{
		// GET api/<controller>
		public Dictionary<string, string> Get()
		{
			var groupIds = ClaimsPrincipal.Current.Claims.Where(x => x.Type == "groups").Select(x => x.Value).ToList();
			var groupService = new GroupService();
			var groups = new Dictionary<string, string>();
			foreach (var groupId in groupIds)
			{
				var groupName = groupService.RetrieveGroupName(groupId);
				groups.Add(groupId, groupName);
			}
			return groups;
		}
	}
}