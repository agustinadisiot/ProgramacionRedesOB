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

            if (context.Exception is Grpc.Core.RpcException e)
            {
                statusCode = (int)e.Status.StatusCode;
                errorMessage = e.Status.Detail;
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