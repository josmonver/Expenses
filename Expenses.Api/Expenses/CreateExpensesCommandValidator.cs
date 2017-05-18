using FluentValidation;

namespace Expenses.Api.Expenses
{
    public class CreateExpensesCommandValidator : AbstractValidator<CreateExpenseCommand>
    {
        public CreateExpensesCommandValidator()
        {
            RuleFor(m => m.Comment).NotEmpty();
            RuleFor(m => m.Amount).NotEqual(0);
        }
    }
}