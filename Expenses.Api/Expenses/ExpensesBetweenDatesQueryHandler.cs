using AutoMapper;
using Expenses.Data;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace Expenses.Api.Expenses
{
    public class ExpensesBetweenDatesQueryHandler : IRequestHandler<ExpensesBetweenDatesQuery, List<ExpenseResult>>
    {
        private readonly IExpensesDbContext _ctx;
        private readonly IMapper _mapper;

        public ExpensesBetweenDatesQueryHandler(IExpensesDbContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public List<ExpenseResult> Handle(ExpensesBetweenDatesQuery message)
        {
            return _ctx.Expenses.AsNoTracking().Where(o => o.Date < message.DateTo && o.Date >= message.DateFrom)
                .ProjectToList<ExpenseResult>(_mapper.ConfigurationProvider);
        }
    }
}