using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using MyUniversity.UserManager.Models.CustomExceptions;
using System;
using System.Threading.Tasks;

namespace MyUniversity.UserManager.Interceptors
{
    public class ExceptionInterceptor : Interceptor
    {
        private readonly ILogger<ExceptionInterceptor> _logger;

        public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (BadArgumentException ex)
            {
                _logger.LogWarning(ex, $"Error thrown by {context.Method}.");
                throw;
            }
            catch (AccessForbiddenException ex)
            {
                _logger.LogWarning(ex, $"Error thrown by {context.Method}.");
                throw;
            }
            catch (ItemNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Error thrown by {context.Method}.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error thrown by {context.Method}.");
                throw;
            }
        }
    }
}
