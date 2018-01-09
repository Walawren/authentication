using System.Collections.Generic;
using IdentityServer4.Models;

namespace Walawren.Authentication
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("grappnel", "Grappnel API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "grappnel-website",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("another secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = {"grappnel"}
                },
                new Client
                {
                    ClientId = "test-client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"grappnel"}
                }
            };
        }
    }
}
