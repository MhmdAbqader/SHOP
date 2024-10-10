using System.Text.RegularExpressions;

namespace SHOP.CustomizationNeeds
{
    public class CharRouteConstraint : IRouteConstraint
    {
        public bool Match(
            HttpContext httpContext,
            IRouter route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out var value) && value != null)
            {
                // Check if the value is a single character
                string stringValue = Convert.ToString(value);
                return stringValue.Length == 1 && Regex.IsMatch(stringValue, @"^[a-zA-Z]$");
            }
            return false;
        }
    }
}
