using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Web;

namespace AzureAD.Web.Models
{
	public class UserClaim
	{
		public UserClaim()
		{

		}

		public UserClaim(Claim claim)
		{
			this.Issuer = claim.Issuer;
			this.OriginalIssuer = claim.OriginalIssuer;
			this.Properties = claim.Properties;
			this.Subject = claim.Subject.Name;
			this.Type = claim.Type;
			this.Value = claim.Value;
			this.ValueType = claim.ValueType;
		}

		public string Issuer { get; set; }
		public string OriginalIssuer { get; set; }
		public IDictionary<string, string> Properties { get; set; }
		public string Subject { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }
		public string ValueType { get; set; }
	}
}