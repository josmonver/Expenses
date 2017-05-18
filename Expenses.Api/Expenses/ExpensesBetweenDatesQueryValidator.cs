using FluentValidation;

namespace Expenses.Api.Expenses
{
    public class ExpensesBetweenDatesQueryValidator : AbstractValidator<ExpensesBetweenDatesQuery>
    {
        public ExpensesBetweenDatesQueryValidator()
        {
            RuleFor(m => m.DateTo).GreaterThanOrEqualTo(m => m.DateFrom);
        }
    }
}