using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Commerce_WebApi.Filters
{
    public class UseApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var apiKey = configuration.GetValue<string>("ApiKey");

            if (!context.HttpContext.Request.Headers.TryGetValue("key", out var key))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!apiKey.Equals(key))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next(); 
        }   
    }
}
