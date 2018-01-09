using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Walawren.Authentication.TestClient
{
    class Program
    {
        private static DiscoveryResponse _disco;
        private static string _accessToken;

        public static void Main(string[] args)
        {
            DiscoverEndpoints().GetAwaiter().GetResult();

            Console.WriteLine("Use Client Credentials?");
            var useClientCredentials = Console.ReadKey().KeyChar;
            if(useClientCredentials == 'y')
            {
                ClientCredentials().GetAwaiter().GetResult();
            }
            else
            {
                ResourceOwnerCredentials().GetAwaiter().GetResult();
            }

            Console.WriteLine(_accessToken);
            Console.WriteLine();

            CallAPI().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }

        private static async Task ClientCredentials()
        {
            // request token
            var tokenClient = new TokenClient(_disco.TokenEndpoint, "test-client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("grappnel");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            _accessToken = tokenResponse.AccessToken;
        }

        private static async Task ResourceOwnerCredentials()
        {
            Console.WriteLine("Username:");
            var userName = Console.ReadLine();
            Console.WriteLine("Password:");
            var password = Console.ReadLine();

            var tokenClient = new TokenClient(_disco.TokenEndpoint, "test-client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(userName, password, "grappnel");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            _accessToken = tokenResponse.AccessToken;
        }

        private static async Task DiscoverEndpoints(){
            // discover endpoints from metadata
            _disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (_disco.IsError)
            {
                Console.WriteLine(_disco.Error);
                return;
            }
        }

        private static async Task CallAPI(){
            // call api
            var client = new HttpClient();
            client.SetBearerToken(_accessToken);


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
    }
}
