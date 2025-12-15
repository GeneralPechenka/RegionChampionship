using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Core.Maintenance
{
    // Maintenance DTO
    public record MaintenanceShortDto(
        Guid Id,
        DateTime MaintenanceDate,
        string? VendingMachineName,  // Из маппинга: src.VendingMachine.Name
        string? EmployeeName,        // Из маппинга: src.Employee.FullName
        MaintenanceTypeEnum Type,
        string Problems);

    public record CreateUpdateMaintenanceDto(
        DateTime MaintenanceDate,
        Guid VendingMachineId,
        MaintenanceTypeEnum Type,
        string? WorkDescription,
        string Problems,
        Guid? EmployeeId);

    public record MaintenanceDetailsDto(
        Guid Id,
        DateTime MaintenanceDate,
        MaintenanceTypeEnum Type,
        string? WorkDescription,
        string Problems,
        Guid VendingMachineId,
        Guid? EmployeeId,
        string? VendingMachineName,      // Из маппинга: src.VendingMachine.Name
        string? EmployeeName,            // Из маппинга: src.Employee.FullName
        string? VendingMachineModel,     // Из маппинга: src.VendingMachine.Model
        string? VendingMachineLocation); // Из маппинга: src.VendingMachine.Location

    // EngineerTask DTO
    public record EngineerTaskShortDto(
        Guid Id,
        string Title,
        DateTime DueDate,
        TaskStatusEnum Status,
        int Priority,
        string? VendingMachineName,  // Из маппинга: src.VendingMachine.Name
        string? AssignedToName,      // Из маппинга: src.AssignedTo.FullName
        int EstimatedHours);

    public record CreateUpdateEngineerTaskDto(
        string Title,
        string? Description,
        DateTime DueDate,
        Guid VendingMachineId,
        int Priority = 5,
        int EstimatedHours = 2,
        Guid? AssignedToId = null);

    public record AssignEngineerTaskDto(
        Guid EmployeeId,
        DateTime? DueDate = null,
        int Priority = 5);

    // Calendar DTO
    public record CalendarTaskDto(
        Guid TaskId,
        string Title,
        TaskStatusEnum Status,
        string? VendingMachineName,
        string ColorCode);

    // Auto-generation DTO
    public record AutoGenerateTasksDto(
        DateTime FromDate,
        DateTime ToDate,
        bool IncludePlannedMaintenance = true);
}
