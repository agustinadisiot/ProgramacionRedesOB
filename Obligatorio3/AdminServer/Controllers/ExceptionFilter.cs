using Common.Protocol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Server;
using System;
using System.IO;
using System.Security.Authentication;

namespace AdminServer
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            int statusCode;

            string errorMessage = context.Exception.Message;

            if (context.Exception is NameAlreadyExistsException)
            {
                statusCode = 404;
            }
            else if (context.Exception is TitleAlreadyExistsException)
            {
                statusCode = 404;
            }
            else if (context.Exception is AuthenticationException)
            {
                statusCode = 401;
                errorMessage = "Incorrect password or username";
            }
            else if (context.Exception is DirectoryNotFoundException)
            {
                statusCode = 500;
            }
            else if (context.Exception is ServerError e)
            {
                statusCode = 500;
                errorMessage = e.Message;
            }
            else
            {
                statusCode = 500;
            }

            MessageReply message = new MessageReply() { Message = errorMessage};
            context.Result = new ObjectResult(message)
            {
                StatusCode = statusCode,
            };
        }
    }
}