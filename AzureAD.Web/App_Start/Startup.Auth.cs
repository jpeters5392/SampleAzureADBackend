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
			app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

			app.UseCookieAuthentication(new CookieAuthenticationOptions());

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

							// we don't do anything with this here, but we need to do this so that the access token is added to the cache so we can use it later
							AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
								context.Code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphUrl);
						}
					}
				});

			app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = ConfigurationManager.AppSettings["ida:Tenant"],
                    TokenValidationParameters = new TokenValidationParameters {
                         ValidAudience = ConfigurationManager.AppSettings["ida:Audience"]
                    },
					AuthenticationType = "OAuth2Bearer"
                });
        }
    }
}
