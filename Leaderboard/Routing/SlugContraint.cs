using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using static Leaderboard.Utilities.SlugUtilities;

namespace Leaderboard.Routing.Constraints
{
    public class SlugConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var routeVal = values[routeKey]?.ToString();
            return routeVal != default && SlugRegex.IsMatch(values[routeKey]?.ToString());
        }
    }
}
