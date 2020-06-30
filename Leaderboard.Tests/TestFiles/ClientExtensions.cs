using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Leaderboard.Tests.Extensions
{
    public static class ClientExtensions
    {
        public static async Task<HttpResponseMessage> AuthenticateAsync(this HttpClient client,
            string userName, string password, bool rememberMe = true)
        {
            var keys = new Dictionary<string, string>
            {
                {"Intput.UserName", userName},
                {"Input.Password", password},
                {"Input.RememberMe", Convert.ToString(rememberMe).ToLower()}
            };

            using var formContent = new FormUrlEncodedContent(keys);

            return await client.PostAsync("/Identity/Account/Login", formContent).ConfigureAwait(false);
        }
    }
}
