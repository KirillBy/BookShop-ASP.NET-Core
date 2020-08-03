using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Middelware
{
    public class AuthorizationLoginMiddleware 
    {
        private readonly RequestDelegate _next;
        public AuthorizationLoginMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public Task Invoke(HttpContext context)
        {
             string authHeader = context.Request.Headers["cookie"];
             if(authHeader != null) 
             {
                Debug.WriteLine(authHeader);
             }
            return _next.Invoke(context);
        }
    }

    public static class AuthorizationLoginExtension
    {
        public static IApplicationBuilder UseAuthorizationLogin(this IApplicationBuilder app)
        {
            app.UseMiddleware(typeof(AuthorizationLoginMiddleware));
            return app;
        }
    }
        
}
