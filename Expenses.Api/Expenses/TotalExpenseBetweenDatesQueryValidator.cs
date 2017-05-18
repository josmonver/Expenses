using FluentValidation;

namespace Expenses.Api.Expenses
{
    public class TotalExpenseBetweenDatesQueryValidator : AbstractValidator<TotalExpenseBetweenDatesQuery>
    {
        public TotalExpenseBetweenDatesQueryValidator()
        {
            RuleFor(m => m.DateTo).GreaterThanOrEqualTo(m => m.DateFrom);
        }
    }
}