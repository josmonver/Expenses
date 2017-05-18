using AutoMapper;
using Expenses.Domain;

namespace Expenses.Api.Expenses
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Expense, ExpenseResult>();
        }
    }
}