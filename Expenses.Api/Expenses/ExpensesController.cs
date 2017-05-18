using MediatR;
using System;
using System.Web.Http;

namespace Expenses.Api.Expenses
{
    public class ExpensesController : ApiController
    {
        private readonly IMediator _mediator;

        public ExpensesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IHttpActionResult Get(DateTime dateFrom, DateTime dateTo)
        {
            var data = _mediator.Send(new ExpensesBetweenDatesQuery { DateFrom = dateFrom, DateTo = dateTo });
            return Ok(data.Result);
        }

        public IHttpActionResult GetTotal(DateTime dateFrom, DateTime dateTo)
        {
            var data = _mediator.Send(new TotalExpenseBetweenDatesQuery { DateFrom = dateFrom, DateTo = dateTo });
            return Ok(data.Result);
        }

        public IHttpActionResult Post(CreateExpenseCommand command)
        {
            _mediator.Send(command);
            return Ok();
        }
    }
}
