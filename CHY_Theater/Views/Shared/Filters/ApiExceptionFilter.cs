using CHY_Theater.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CHY_Theater.Views.Shared.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var error = new ErrorViewModel();

            if (context.Exception is UnauthorizedAccessException)
            {
                context.Result = new ViewResult
                {
                    ViewName = "UnauthorizedAccess",
                    ViewData = new ViewDataDictionary<ErrorViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = new ErrorViewModel
                        {
                            Message = "Access Denied",
                            Details = context.Exception.Message
                        }
                    }
                };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (context.Exception is HttpRequestException)
            {
                context.Result = new ViewResult
                {
                    ViewName = "ApiError",
                    ViewData = new ViewDataDictionary<ErrorViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = new ErrorViewModel
                        {
                            Message = "API Access Error",
                            Details = context.Exception.Message
                        }
                    }
                };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            context.ExceptionHandled = true;
        }
    }
}
