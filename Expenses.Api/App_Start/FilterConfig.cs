using Expenses.Api.Filters;
using System.Web.Http.Filters;

namespace Expenses.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(HttpFilterCollection filters)
        {
            filters.Add(new ValidatorActionFilter());
            filters.Add(new TransactionFilter());
        }
    }
}