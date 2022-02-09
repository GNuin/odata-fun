namespace PQS.Test.DummyOData
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;

    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _envirioment;
        private readonly ILogger<HttpGlobalExceptionFilter> _log;

        public HttpGlobalExceptionFilter(IHostEnvironment env, ILogger<HttpGlobalExceptionFilter> log)
        {
            this._envirioment = env;
            this._log = log;
        }

        public void OnException(ExceptionContext context)
        {
            _log.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            if (context.Exception is FluentValidation.ValidationException)
            {
                // maneja los errores de validacion
                var pobjValidationException = context.Exception as FluentValidation.ValidationException;

                var problemDetails = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation exception",
                    Detail = pobjValidationException.Message
                };

                // agrupa los errores por propiedad y los agrega al detalle
                foreach (var errorGropup in pobjValidationException.Errors.GroupBy(e => e.PropertyName))
                {
                    problemDetails.Errors.Add(errorGropup.Key, errorGropup.Select(e => e.ErrorMessage).ToArray());
                }

                context.Result = new BadRequestObjectResult(problemDetails);
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                _log.LogError($"Validation exception in {context.HttpContext.Request.Path}", context.Exception);
            }

            /*
             else if (context.Exception is DbUpdateException)
             {
                 var pobjEntityException = context.Exception as DbUpdateException;

                 // maneja excepciones PQS 
                 var problemDetails = new ProblemDetails()
                 {
                     Instance = context.HttpContext.Request.Path,
                     Status = StatusCodes.Status500InternalServerError,
                     Title = "Error updating the database",
                     Detail = Tools.LastExceptionMessage(pobjEntityException)
                 };

                 if (_envirioment.IsDevelopment())
                 {
                     problemDetails.Extensions.Add("errors", pobjEntityException.InnerException);
                 }
                 context.Result = new InternalServerErrorObjectResult(problemDetails);
                 context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                 _log.LogError($"DB Exception in {context.HttpContext.Request.Path}", context.Exception);
             }*/

            else
            {
                var json = new JsonErrorResponse
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Unhandleld exception",
                    Detail = "Ocurrio un error"
                };

                //if (_envirioment.IsDevelopment())
                //{
                var list = new List<string>();
                list.Add(context.Exception.Message);
                list.Add(context.Exception.StackTrace);

                json.Detail += ":\r\n" + context.Exception.Message;
                json.Errors = list.ToArray();
                //}

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                _log.LogError(context.Exception, $"Unhandleld exception in {context.HttpContext.Request.Path}");
            }

            context.ExceptionHandled = true;
        }

        private class JsonErrorResponse : ProblemDetails
        {
            public string[] Errors { get; set; }
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
