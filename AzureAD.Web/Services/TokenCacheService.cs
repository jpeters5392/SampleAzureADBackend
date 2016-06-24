using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureAD.Web.Services
{
	public class TokenCacheService : TokenCache
	{
		private static ConcurrentDictionary<string, byte[]> userCaches = new ConcurrentDictionary<string, byte[]>();
		private string currentUserId;

		public TokenCacheService(string userId) : base()
		{
			this.currentUserId = userId;

			this.AfterAccess = AfterAccessNotification;
			this.BeforeAccess = BeforeAccessNotification;

			// initialize the current state of the cache
			this.Deserialize(userCaches.ContainsKey(currentUserId) ? userCaches[currentUserId] : null);
		}

		// Update the in-memory copy from our local cache
		void BeforeAccessNotification(TokenCacheNotificationArgs args)
		{
			this.Deserialize(userCaches.ContainsKey(currentUserId) ? userCaches[currentUserId] : null);
		}

		void AfterAccessNotification(TokenCacheNotificationArgs args)
		{
			// ADAL updated the state of the cache, so serialize it
			if (this.HasStateChanged)
			{
				// update our cached copy
				userCaches[currentUserId] = this.Serialize();
				this.HasStateChanged = false;
			}
		}
	}
}