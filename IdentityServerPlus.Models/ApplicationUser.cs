using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    public class ApplicationUser
    {
        [ClaimMaping(JwtClaimTypes.Subject)] [Key] public Guid Id { get; set; }

        [ClaimMaping(JwtClaimTypes.Email)]
        public string Email { get; set; }
        public bool EmailVerified { get; set; }

        [ClaimMaping(JwtClaimTypes.PreferredUserName)]
        public string Username { get; set; }

        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }



        public List<ApplicationClaim> Claims { get; set; } = new List<ApplicationClaim>();

        public List<ApplicationProviderInfo> Providers { get; set; } = new List<ApplicationProviderInfo>();

        public DateTimeOffset? LockoutEnd { get; set; }
        public List<ApplicationCodes> Codes { get; set; } = new List<ApplicationCodes>();
        
        public bool TwoFactorEnabled { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        public string ConcurrencyStamp { get; set; }
        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        public bool EmailConfirmed { get; set; }
        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        public bool LockoutEnabled { get; set; }
        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public int AccessFailedCount { get; set; }


    }

    public class ApplicationCodes
    {
        public enum CodeType : byte
        {
            VerificationCode,

            AuthenticatorKey,


            Other = 255
        }

        public CodeType Type { get; set; }

        public string Code { get; set; }
        public DateTimeOffset Expiry { get; set; }
        public DateTime Created { get; set; }
    }

    public class ApplicationClaim
    {

        public ApplicationClaim()
        {
        }

        public ApplicationClaim(Claim claim)
        {
            Type = claim.Type;
            Properties = claim.Properties;
            OriginalIssuer = claim.OriginalIssuer;
            Issuer = claim.Issuer;
            ValueType = claim.ValueType;
            Value = claim.Value;
        }

        public string Type { get; set; }
        
        public IDictionary<string, string> Properties { get; set; }
        
        public string OriginalIssuer { get; set; }
        public string Issuer { get; set; }
        public string ValueType { get; set; }
        public string Value { get; set; }

    }


    public class ApplicationProviderInfo : UserLoginInfo
    {
        public ApplicationProviderInfo(string loginProvider, string providerKey, string providerDisplayName): base(loginProvider, providerKey, providerDisplayName)
        {

        }

        public ApplicationProviderInfo(UserLoginInfo info) : base(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName)
        {
        }

        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime IdTokenExpiry { get; set; }

        public DateTime ProviderLinkedAt { get; set; }

    }

    public class ClaimMapingAttribute : Attribute
    {
        public string Claim { get; set; }
        public ClaimMapingAttribute(string claim)
        {
            this.Claim = claim;
        }
    }

}