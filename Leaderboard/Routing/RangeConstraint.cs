using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Leaderboard.Routing.Constraints
{
    public class RangeConstraint : IRouteConstraint
    {
        private const string _decimalRegex = "([0-9]+(\\.[0-9]+)?|any)";
        private static readonly Regex _rangeRegex = new Regex($"^({_decimalRegex}-{_decimalRegex}|any)$");
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var routeVal = values[routeKey]?.ToString();
            return routeVal != default ? _rangeRegex.IsMatch(values[routeKey]?.ToString()) : false;
        }
    }
}
