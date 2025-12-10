using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{

    // 6. Employee
    public class EmployeeException(string? message) : Exception(message)
    {
    }

    // 9. ProductStock
    public class ProductStockException(string? message) : Exception(message)
    {
    }

    // 10. Sale
    public class SaleException(string? message) : Exception(message)
    {
    }

    // 11. Maintenance
    public class MaintenanceException(string? message) : Exception(message)
    {
    }

    // 12. EngineerTask
    public class EngineerTaskException(string? message) : Exception(message)
    {
    }

    // 13. Notification
    public class NotificationException(string? message) : Exception(message)
    {
    }

    // 14. EventLog
    public class EventLogException(string? message) : Exception(message)
    {
    }


    // 17. Role (если есть отдельная сущность Role)
    public class RoleException(string? message) : Exception(message)
    {
    }

    // 18. Import (для импорта данных)
    public class ImportException(string? message) : Exception(message)
    {
    }

    // 19. Export (для экспорта данных)
    public class ExportException(string? message) : Exception(message)
    {
    }

}
