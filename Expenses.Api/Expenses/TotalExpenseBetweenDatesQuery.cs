using MediatR;
using System;
using System.Collections.Generic;

namespace Expenses.Api.Expenses
{
    public class TotalExpenseBetweenDatesQuery : IRequest<decimal>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}