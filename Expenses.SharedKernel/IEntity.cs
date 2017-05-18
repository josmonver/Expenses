using System;

namespace Expenses.SharedKernel
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
