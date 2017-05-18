using System;

namespace Expenses.SharedKernel
{
    public interface IAudit
    {
        DateTime DateCreated { get; set; }
        DateTime? DateModified { get; set; }
        string UserCreated { get; set; }
        string UserModified { get; set; }
    }
}
