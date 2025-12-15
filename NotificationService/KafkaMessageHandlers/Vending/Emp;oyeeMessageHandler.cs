using DTOs.Core.Employeee;
using DTOs.Notification;
using KafkaConsuming.Interfaces;

namespace NotificationService.KafkaMessageHandlers.Vending
{
    public class EmployeeCreatedMessageHandler : IMessageHandler<EmployeeCreatedNotificationDto>
    {
        public Task HandleAsync(EmployeeCreatedNotificationDto message, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
    public class EmployeeDeletedMessageHandler : IMessageHandler<EmployeeDeletedNotificationDto>
    {
        public Task HandleAsync(EmployeeDeletedNotificationDto message, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
