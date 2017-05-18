namespace Expenses.SharedKernel
{
    public interface IObjectTrackingState
    {
        ObjectState ObjectState { get; set; }
    }
}
