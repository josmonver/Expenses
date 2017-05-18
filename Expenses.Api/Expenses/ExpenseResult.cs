using System;

namespace Expenses.Api.Expenses
{
    public class ExpenseResult
    {
        public Guid Id { get; protected set; }
        public string Comment { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}