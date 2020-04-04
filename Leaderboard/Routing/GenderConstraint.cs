using System;
using Leaderboard.Areas.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Leaderboard.Routing.Constraints
{
    public class GenderConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var routeVal = values[routeKey]?.ToString();
            return routeVal == "any" || Enum.TryParse<GenderValues>(values[routeKey]?.ToString(), true, out var _);
        }
    }
}
