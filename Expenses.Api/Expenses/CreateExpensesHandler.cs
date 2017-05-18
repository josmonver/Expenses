using Expenses.Data;
using Expenses.Domain;
using MediatR;

namespace Expenses.Api.Expenses
{
    public class CreateExpensesHandler : IRequestHandler<CreateExpenseCommand>
    {
        private readonly IExpensesDbContext _ctx;

        public CreateExpensesHandler(IExpensesDbContext ctx)
        {
            _ctx = ctx;
        }

        public void Handle(CreateExpenseCommand message)
        {
            var expense = Expense.Create(message.Comment, message.Amount, message.Date);
            _ctx.Expenses.Add(expense);
        }
    }
}