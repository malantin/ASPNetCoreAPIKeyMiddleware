using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Builder.CustomMiddleware
{
    class APIKeyMiddleware
    {
        // Set or retrieve you api key name that is expected in the request header
        private string APIKeyHeaderName { get { return "x-company-apikey"; } }
        // Set or retrieve the api key you expect. This could a share or individual key for each user
        private string APIKey { get { return "<yourkey>"; } }

        private readonly RequestDelegate _next;

        public APIKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.Keys.Contains(APIKeyHeaderName))
            {
                // The header has not been set correctly. Return 400: Bad request.
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Error 400: Bad request. Please set {APIKeyHeaderName} header and the correct API key.");
                return;
            }

            if (!context.Request.Headers[APIKeyHeaderName].Equals(APIKey))
            {
                // The api key was incorrect. Return 401: Unauthoried.
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Error 401: Unauthorized. Incorrect or no API key.");
                return;
            }

            // Call the next delegate/middleware in the pipeline
            await this._next(context);
        }
    }

    public static class APIKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseAPIKey(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<APIKeyMiddleware>();
        }
    }
}
