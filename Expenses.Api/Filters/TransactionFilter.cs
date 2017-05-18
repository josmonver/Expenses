using Expenses.Data;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Expenses.Api.Filters
{
    public class TransactionFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string httpVerb = actionContext.Request.Method.Method.ToUpper();
            if (httpVerb != "GET")
            {
                IExpensesDbContext context =
                (IExpensesDbContext)actionContext.Request.GetDependencyScope().GetService(typeof(IExpensesDbContext));
                context.BeginTransaction();
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            string httpVerb = actionContext.Request.Method.Method.ToUpper();
            if (httpVerb != "GET")
            {
                IExpensesDbContext context =
                (IExpensesDbContext)actionContext.Request.GetDependencyScope().GetService(typeof(IExpensesDbContext));
                context.CloseTransaction(actionContext.Exception);
            }
        }
    }
}