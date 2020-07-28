using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Auth
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        public string[] ExpectMethod;

        private static TokenValidationParameters GetValidationParameters()
        {
            var key = Encoding.ASCII.GetBytes("3a97556f-c07f-4f5a-8bc6-28711d4922e9");

            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            string authHeader = filterContext.HttpContext.Request.Headers["Authorization"];

            if (authHeader == null)
            {
                if (ExpectMethod != null)
                {
                    if (!ExpectMethod.Contains(filterContext.RouteData.Values["action"]))
                    {
                        filterContext.Result = new UnauthorizedResult();
                    }
                }
                else
                {
                    filterContext.Result = new UnauthorizedResult();
                }

                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(authHeader, validationParameters, out validatedToken);

            if (principal.Identity.IsAuthenticated)
            {
                if (ExpectMethod != null)
                {
                    if (ExpectMethod.Contains(filterContext.RouteData.Values["action"]))
                    {
                        filterContext.Result = new UnauthorizedResult();
                    }
                }
            }
            else
            {
                filterContext.Result = new UnauthorizedResult();
            }
        }
    }
}
