using System;
using System.Globalization;
using Microsoft.AspNetCore.Routing;

namespace Leaderboard.Extensions
{
    public static class RouteDataExtensions
    {
        public static T ToObject<T>(this RouteData data)
            where T : new()
        {
            var obj = new T();
            foreach (var prop in typeof(T).GetProperties())
            {
                // values in the route dictionary are stored in camelCase
                data.Values.TryGetValue(prop.Name.ToCamelCase(), out var val);
                prop.SetValue(obj, val);
            }
            return obj;
        }
    }
}
