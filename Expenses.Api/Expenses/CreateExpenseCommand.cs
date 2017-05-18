using MediatR;
using System;
using System.ComponentModel;

namespace Expenses.Api.Expenses
{
    public class CreateExpenseCommand : IRequest
    {
        public string Comment { get; set; }
        [DisplayName("Importe")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}