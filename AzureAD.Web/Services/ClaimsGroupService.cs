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
	public class ClaimsGroupService
	{
		private static string clientId = ConfigurationManager.AppSettings["ida:ClientID"];
		private static string authority = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ida:AADInstance"], ConfigurationManager.AppSettings["ida:Tenant"]);
		private static string appKey = ConfigurationManager.AppSettings["ida:AppKey"];
		private static string graphUrl = ConfigurationManager.AppSettings["ida:GraphUrl"];
		private static string graphVersion = ConfigurationManager.AppSettings["ida:GraphApiVersion"];
		private static string objectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
		private static string groupsUrl = "https://graph.windows.net/myorganization/users/{0}/memberOf?api-version={1}";

		public async Task<IList<string>> RetrieveAADGroups(string uniqueUserId)
		{
			IList<string> groupObjectIds = new List<string>();

			// Acquire the Access Token
			ClientCredential credential = new ClientCredential(clientId, appKey);
			AuthenticationContext authContext = new AuthenticationContext(authority, new TokenCacheService(uniqueUserId));

			// if we had not acquired a token on the initial sign-in then this would fail since there would not be a cached token
			AuthenticationResult result = await authContext.AcquireTokenSilentAsync(graphUrl, credential,
				new UserIdentifier(uniqueUserId, UserIdentifierType.UniqueId));

			string requestUrl = string.Format(groupsUrl, uniqueUserId, graphVersion);

			// Prepare and Make the POST request
			HttpClient client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
			StringContent content = new StringContent("{\"securityEnabledOnly\": \"false\"}");
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			//request.Content = content;
			HttpResponseMessage response = await client.SendAsync(request);

			// Endpoint returns JSON with an array of Group ObjectIDs
			if (response.IsSuccessStatusCode)
			{
				string responseContent = await response.Content.ReadAsStringAsync();
				var groupsResult = (Json.Decode(responseContent)).value;

				foreach (var groupObjectID in groupsResult)
					groupObjectIds.Add((string)groupObjectID.displayName);
			}
			else
			{
				throw new WebException();
			}

			return groupObjectIds;
		}
	}
}