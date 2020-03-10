using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Leaderboard.Routing.Constraints
{
    public class SlugConstraint : IRouteConstraint
    {
        private static readonly Regex _slugRegex = new Regex("^[a-z0-9]+(?:-[a-z0-9]+)*$");

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var routeVal = values[routeKey]?.ToString();
            return routeVal != default ? _slugRegex.IsMatch(values[routeKey]?.ToString()) : false;
        }
    }
}
