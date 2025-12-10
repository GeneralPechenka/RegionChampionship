namespace ApiGateway
{
    public class SignHttpsRequest(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Headers["ApiGateway"] = "Signed";
            Console.WriteLine(context.Request.Path);
            await next(context);
        }
    }
}