using Domain.Enums;

namespace Domain.Entities
{
    // 13. Уведомления
    public class Notification
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public Guid? VendingMachineId { get; set; }
        public Guid? UserId { get; set; }

        public VendingMachine VendingMachine { get; set; }
        public Employee User { get; set; }
    }
}
