

namespace PQS.Test.DummyOData.Utils
{
    using Microsoft.AspNetCore.Http;
    using Serilog;
    using System;
    using System.Linq;
    public static class LogEnricher
    {
        /// <summary>
        /// Enriches the HTTP request log with additional data via the Diagnostic Context
        /// </summary>
        /// <param name="diagnosticContext">The Serilog diagnostic context</param>
        /// <param name="httpContext">The current HTTP Context</param>
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var authenticated = httpContext.User?.Identity.IsAuthenticated;

            if (authenticated.Value)
            {
                diagnosticContext.Set("User", httpContext.User.Identity.Name);
            }

            var test = httpContext.Request.Headers.Where(h => h.Key.StartsWith("X-"));

            foreach (var head in test)
            {
                diagnosticContext.Set(head.Key, head.Value);
            }

            // proxy header
            var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"];
            if (!String.IsNullOrEmpty(forwardedHeader))
            {
                diagnosticContext.Set("RemoteIpAddress", forwardedHeader.LastOrDefault());
            }
            else
                diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress.ToString());

            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["UserAgent"].FirstOrDefault());
        }


    }
}
