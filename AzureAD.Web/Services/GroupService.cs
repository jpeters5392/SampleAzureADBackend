using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Helpers;
using System.Net;

namespace AzureAD.Web.Services
{
	public class GroupService
	{
		private static IDictionary<string, string> groups = new Dictionary<string, string>()
		{
			{ "a558043c-f499-4a94-89b3-1cbae0a5fc6b", "Elevated Permissions" },
			{ "0daa5167-17c3-44ed-b62a-da910337e5dc", "Basic Users" }
		};

		public string RetrieveGroupName(string groupId)
		{
			return groups[groupId];
		}
	}
}