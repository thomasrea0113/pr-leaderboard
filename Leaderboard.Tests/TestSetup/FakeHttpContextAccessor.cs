using Microsoft.AspNetCore.Http;

namespace Leaderboard.Tests.TestSetup
{
    public class FakeHttpContextAccess : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }

        public FakeHttpContextAccess(HttpContextAccessor accessor)
        {
            HttpContext = accessor.HttpContext ?? new DefaultHttpContext();
        }
    }
}