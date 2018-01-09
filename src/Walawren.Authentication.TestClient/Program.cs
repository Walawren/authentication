using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Walawren.Authentication.TestClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            ClientCredentials().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        private static async Task ClientCredentials()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "test-client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("grappnel");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);


            var url = "http://localhost:5001/api/identity";
            Console.WriteLine(url);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }

        private static void ResourceOwnerCredentials()
        {
            
        }
    }
}
