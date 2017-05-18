using Expenses.SharedKernel;
using System;

namespace Expenses.Domain
{
    public class Expense : IEntity<Guid>, IObjectTrackingState
    {
        public Guid Id { get; protected set; }
        public string Comment { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        private Expense()
            : this(Guid.NewGuid())
        {

        }

        public Expense(Guid id)
        {
            Id = id;
        }

        public static Expense Create(string comment, decimal amount, DateTime date)
        {
            var expense = new Expense()
            {
                Comment = comment,
                Amount = amount,
                Date = date,
                ObjectState = ObjectState.Added,
            };

            return expense;
        }

        #region IObjectTrackingState
        public ObjectState ObjectState { get; set; }
        #endregion
    }
}