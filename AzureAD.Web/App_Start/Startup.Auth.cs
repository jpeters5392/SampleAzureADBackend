using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using Microsoft.Owin.Security.Cookies;
using System.Globalization;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Web;
using AzureAD.Web.Services;
using System.Web.Helpers;
using System.Web.Http;

namespace AzureAD.Web
{
    public partial class Startup
    {
		private static string objectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
		private static string authority = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ida:AADInstance"], ConfigurationManager.AppSettings["ida:Tenant"]);
		private static string graphUrl = ConfigurationManager.AppSettings["ida:GraphUrl"];

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
        {
			// the web access to the site will use cookie authentication
			app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

			app.UseCookieAuthentication(new CookieAuthenticationOptions());

			// we will use OpenIdConnect to authenticate with Azure AD
			app.UseOpenIdConnectAuthentication(
				new OpenIdConnectAuthenticationOptions
				{
					ClientId = ConfigurationManager.AppSettings["ida:ClientID"],
					Authority = authority,
					PostLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"],
					RedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"],
					Notifications = new OpenIdConnectAuthenticationNotifications
					{
						AuthenticationFailed = context =>
						{
							context.HandleResponse();
							context.Response.Redirect("/Error/AuthFailure?message=" + Uri.EscapeUriString(context.Exception.Message));
							return Task.FromResult(0);
						},
						AuthorizationCodeReceived = async context =>
						{
							// We need this to create a token that is stored in the token cache for the current user
							ClientCredential credential = new ClientCredential(ConfigurationManager.AppSettings["ida:ClientID"], ConfigurationManager.AppSettings["ida:AppKey"]);
							string uniqueUserId = context.AuthenticationTicket.Identity.FindFirst(objectIdentifierClaimType).Value;
							Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority, new TokenCacheService(uniqueUserId));

							// we don't do anything with the access token, but we do this so that the access token is added to the cache so we can use it later if needed
							AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
								context.Code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphUrl);
						}
					}
				});

			// WebApi controllers will use bearer token authentication instead of cookie authentication
			app.Map("/api", inner =>
			{
				var config = new HttpConfiguration();

				// suppress the default auth type so that cookies will not work for WebApi
				config.SuppressDefaultHostAuthentication();
				config.Filters.Add(new HostAuthenticationFilter("OAuth2Bearer"));
				inner.UseWindowsAzureActiveDirectoryBearerAuthentication(
				new WindowsAzureActiveDirectoryBearerAuthenticationOptions
				{
					Tenant = ConfigurationManager.AppSettings["ida:Tenant"],
					TokenValidationParameters = new TokenValidationParameters
					{
						ValidateAudience = false,
						AuthenticationType = "OAuth2Bearer"
					},
					AuthenticationType = "OAuth2Bearer"
				});

				config.MapHttpAttributeRoutes();

				config.Routes.MapHttpRoute(
					name: "DefaultApi",
					routeTemplate: "{controller}/{id}",
					defaults: new { id = RouteParameter.Optional }
				);

				inner.UseWebApi(config);
			});
        }
    }
}
