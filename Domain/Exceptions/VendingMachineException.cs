namespace Domain.Exceptions
{
    // 1. VendingMachine
    public class VendingMachineException(string? message) : Exception(message)
    {
    }
}
