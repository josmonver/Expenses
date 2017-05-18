using AutoMapper;
using Expenses.Data;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace Expenses.Api.Expenses
{
    public class TotalExpenseBetweenDatesQueryHandler : IRequestHandler<TotalExpenseBetweenDatesQuery, decimal>
    {
        private readonly IExpensesDbContext _ctx;
        private readonly IMapper _mapper;

        public TotalExpenseBetweenDatesQueryHandler(IExpensesDbContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public decimal Handle(TotalExpenseBetweenDatesQuery message)
        {
            return _ctx.Expenses.AsNoTracking()
                .Where(o => o.Date < message.DateTo && o.Date >= message.DateFrom)
                .Sum(o => (decimal?)o.Amount) ?? 0;
        }
    }
}