using Domain.Enums;

namespace Domain.Entities
{
    // 12. Задачи для инженеров (из задания по календарю)
    public class EngineerTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public TaskStatusEnum Status { get; set; }
        public int Priority { get; set; }
        public int EstimatedHours { get; set; }

        public Guid VendingMachineId { get; set; }
        public Guid? AssignedToId { get; set; }

        public VendingMachine VendingMachine { get; set; }
        public Employee AssignedTo { get; set; }
    }
}
