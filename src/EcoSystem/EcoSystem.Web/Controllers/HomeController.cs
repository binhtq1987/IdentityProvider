using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Newtonsoft.Json;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using Microsoft.AspNetCore.Http.Authentication;

namespace EcoSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Shouts()
        {
            //await RefreshTokenAsync();

            var access_token = await HttpContext.Authentication.GetTokenAsync("access_token");

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", access_token);
                var result = await client.GetAsync("http://localhost:51000/api/values");
                var content = await result.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<string[]>(content);
                ViewData["Shouts"] = values;
                return View();
            }
        }

        private async Task RefreshTokenAsync()
        {
            var authorizationServerInformation = await DiscoveryClient.GetAsync("http://localhost:50000");
            var client = new TokenClient(authorizationServerInformation.TokenEndpoint, "econetwork_code", "secret");
            var refresh_token = await HttpContext.Authentication.GetTokenAsync("refresh_token");
            var tokenResponse = await client.RequestRefreshTokenAsync(refresh_token);
            var id_token = await HttpContext.Authentication.GetTokenAsync("id_token");
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
            var tokens = new[] {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = id_token
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResponse.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResponse.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            };

            var authenticationInformation = await HttpContext.Authentication.GetAuthenticateInfoAsync("Cookies");
            authenticationInformation.Properties.StoreTokens(tokens);
            await HttpContext.Authentication.SignInAsync("Cookies",
                authenticationInformation.Principal,
                authenticationInformation.Properties);
        }

        [Authorize]
        public async Task<IActionResult> About()
        {
            //var access_token = await HttpContext.Authentication.GetTokenAsync("access_token");

            //using (HttpClient client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", access_token);
            //    var result = await client.GetAsync("http://localhost:51000/api/values");

            //    if (result.IsSuccessStatusCode)
            //    {
            //        var content = await result.Content.ReadAsStringAsync();
            //    }
            //}

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Login()
        {
            //HttpContext.Authentication.ChallengeAsync();
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Home/About"
            }, "oidc");
        }

        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            await HttpContext.Authentication.SignOutAsync("oidc");
        }

        public async Task<IActionResult> FrontChannelLogout(string sid)
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentSid = User.FindFirst("sid")?.Value ?? "";
                if (string.Equals(currentSid, sid, StringComparison.Ordinal))
                {
                    await HttpContext.Authentication.SignOutAsync("Cookies");
                }
            }

            return NoContent();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
