namespace OrderFlow.Api.Middleware;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}