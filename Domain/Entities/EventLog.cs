namespace Domain.Entities
{
    // 14. Журнал событий (для push-уведомлений)
    public class EventLog
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public DateTime OccurredAt { get; set; }
        
        public Guid? VendingMachineId { get; set; }

        public VendingMachine VendingMachine { get; set; }
    }
}
