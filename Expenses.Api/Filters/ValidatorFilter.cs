using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Expenses.Api.Filters
{
    public class ValidatorActionFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string httpVerb = actionContext.Request.Method.Method.ToUpper();
            if (httpVerb == "POST" || httpVerb == "PUT")
            {
                var modelState = actionContext.ModelState;
                if (!modelState.IsValid)
                {
                    actionContext.Response = actionContext.Request
                         .CreateErrorResponse(HttpStatusCode.BadRequest, modelState);
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {

        }
    }
}

