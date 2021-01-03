using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EcoSystem.OAuth.Configuration
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[] {
                new ApiResource("econetwork", "Eco Network")
                {
                    UserClaims = { "email" }
                }
            };
        }

        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[] {
                new Client{
                    ClientId = "econetwork",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "econetwork" }
                },
                new Client{
                    ClientId = "econetwork_implicit",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = new []
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "econetwork"
                    },
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://firstwebapp.local.ccn/signin-oidc" },
                    PostLogoutRedirectUris = new [] { "http://firstwebapp.local.ccn/signout-callback-oidc" }
                },
                new Client{
                    ClientId = "econetwork_code",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes = new []
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "econetwork"
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://firstwebapp.local.ccn/signin-oidc" },
                    PostLogoutRedirectUris = new [] { "http://firstwebapp.local.ccn/signout-callback-oidc" },
                    LogoutUri = "http://firstwebapp.local.ccn/signout-oidc",
                    //LogoutUri = "http://firstwebapp.local.ccn/Home/FrontChannelLogout",
                    RequireConsent = false
                },
                new Client{
                    ClientId = "econetwork_secondweb_code",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes = new []
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "econetwork"
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://secondwebapp.local.ccn/signin-oidc" },
                    PostLogoutRedirectUris = new [] { "http://secondwebapp.local.ccn/signout-callback-oidc" },
                    LogoutUri = "http://secondwebapp.local.ccn/signout-oidc",
                    //LogoutUri = "http://secondwebapp.local.ccn/Home/FrontChannelLogout",
                    RequireConsent = false
                },
                new Client{
                    ClientId = "econetwork_spa",
                    //ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    RequireClientSecret = false,
                    //RequirePkce = true,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = new []
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "econetwork"
                    },
                    //AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://localhost:4200/signin-callback" },
                    PostLogoutRedirectUris = new [] { "http://localhost:4200/signout-callback" },
                    RequireConsent = false
                }
            };
        }

        public static IEnumerable<TestUser> Users()
        {
            return new[]
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "binh_tq@ccn.com.sg",
                    Password = "12345",
                    Claims = new [] { new Claim("email", "binh_tq@ccn.com.sg") }
                }
            };
        }
    }
}
