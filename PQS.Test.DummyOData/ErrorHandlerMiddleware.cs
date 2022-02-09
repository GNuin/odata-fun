namespace PQS.Test.DummyOData
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _envirioment;
        private readonly ILogger<HttpGlobalExceptionFilter> _log;
        public ErrorHandlerMiddleware(RequestDelegate next, IHostEnvironment env, ILogger<HttpGlobalExceptionFilter> log)
        {
            this._next = next;
            this._envirioment = env;
            this._log = log;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                

                switch (error)
                {
                    case FluentValidation.ValidationException e:
                        // maneja los errores de validacion
                        var problemDetails = new ValidationProblemDetails()
                        {

                            Instance = context.Request.Path,
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Errores de validación",
                            Detail = e.Message,
                            
                        };
                        // agrupa los errores por propiedad y los agrega al detalle
                        foreach (var errorGropup in e.Errors.GroupBy(e => e.PropertyName))
                        {
                            problemDetails.Errors.Add(errorGropup.Key, errorGropup.Select(e => e.ErrorMessage).ToArray());
                        }


                        context.Response.StatusCode = (int)StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(problemDetails);
                        

                        break;
                    default:

                        var traceId = string.IsNullOrEmpty(context.TraceIdentifier) ? Guid.NewGuid().ToString() : context.TraceIdentifier;

                        _log.LogError(error, $"Unhandleld exception in {context.Request.Path}");

                        // unhandled error
                        var errorResponse = new JsonErrorResponse
                        {
                            Instance = context.Request.Path,
                            Status = StatusCodes.Status500InternalServerError,
                            Title = "Ha ocurrido un error al procesar el HttpRequest",
                            Detail = error.Message,
                            TraceId = traceId
                        };

                        var list = new List<string>
                        {
                            error.Message
                        };

                        if (_envirioment.IsDevelopment())
                        {
                            list.Add(error.StackTrace);
                        }
                        errorResponse.Errors = list.ToArray();

                        context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
                        
                        await context.Response.WriteAsJsonAsync(errorResponse);
                        break;
                }
            }
        }
        private class JsonErrorResponse : ProblemDetails
        {
            public string[] Errors { get; set; }
            public string TraceId { get; set; }
        }

        public class InternalServerErrorObjectResult : ObjectResult
        {
            public InternalServerErrorObjectResult(object error)
                : base(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }

}
