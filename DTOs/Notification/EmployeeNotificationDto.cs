using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Notification
{
    public record EmployeeCreatedNotificationDto(string Fullname);
    public record EmployeeDeletedNotificationDto(string Fullname);
    public record EmployeeUpdatedNotificationDto(string Fullname);
    
}
