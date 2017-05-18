using Expenses.Domain;
using System;
using System.Data.Entity;

namespace Expenses.Data
{
    public interface IExpensesDbContext : IDisposable
    {
        DbSet<Expense> Expenses { get; set; }

        int SaveChanges();
        void BeginTransaction();
        void CloseTransaction();
        void CloseTransaction(Exception exception);
    }
}
