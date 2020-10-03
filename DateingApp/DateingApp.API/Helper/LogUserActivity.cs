using DatingApp.Repository.Repository;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DateingApp.API.Helper
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ActionExecutedContext resultContex = await next();

            int.TryParse(resultContex.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userid);

            if (userid > 0)
            {
                //var repo = resultContex.HttpContext.RequestServices.GetService<typeof(IUserRepository)>;
                var repo = (IUserRepository)resultContex.HttpContext.RequestServices.GetService(typeof(IUserRepository));
                await repo.updateLastActive(userid);
            }

        }
    }
}
