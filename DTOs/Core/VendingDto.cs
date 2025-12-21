using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Core
{
    // Минимальный DTO для списка автоматов (таблица/плитка)
    public class VendingMachineShortDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Location { get; set; }
        public string SerialNumber { get; set; }
        public MachineStatusEnum Status { get; set; }
        public string CompanyName { get; set; }
        public string ModemNumber { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }

    // DTO для создания/обновления (только то, что можно менять)
    public class CreateUpdateVendingMachineDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        public string Address { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string SerialNumber { get; set; }

        [Required]
        public string InventoryNumber { get; set; }

        public string Manufacturer { get; set; }

        [Required]
        public DateTime ManufactureDate { get; set; }

        [Required]
        public DateTime CommissioningDate { get; set; }

        public DateTime? LastVerificationDate { get; set; }
        public int? VerificationIntervalMonths { get; set; }
        public int ResourceHours { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public int MaintenanceDurationHours { get; set; }
        public MachineStatusEnum Status { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? ModemId { get; set; }
        public Guid? ProducerCountryId { get; set; }
        public Guid? LastVerificationEmployeeId { get; set; }
    }

    // Детальный DTO (только когда нужны все данные)
    public class VendingMachineDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Model { get; set; }
        public decimal TotalRevenue { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime CommissioningDate { get; set; }
        public DateTime? LastVerificationDate { get; set; }
        public int? VerificationIntervalMonths { get; set; }
        public DateTime? NextVerificationDate { get; set; }
        public int ResourceHours { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public int MaintenanceDurationHours { get; set; }
        public MachineStatusEnum Status { get; set; }
        public DateTime? InventoryDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Только необходимые внешние данные
        public string CompanyName { get; set; }
        public string ModemImei { get; set; }
        public string ModemProvider { get; set; }
        public string CountryName { get; set; }
        public string LastVerifiedBy { get; set; }
    }

    // DTO для монитора ТА (специфичные поля)
    public class VendingMachineMonitorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public MachineStatusEnum Status { get; set; }
        public string ConnectionStatus { get; set; } // Из API
        public string LoadingInfo { get; set; } // Из API
        public decimal CashAmount { get; set; } // Из API
        public string Events { get; set; }
        public string EquipmentStatus { get; set; }
        public string AdditionalInfo { get; set; }
    }
    public class VendingMachineExportDto
    {
        public string Id { get; set; } = null!;
        public string SerialNumber { get; set; } = null!;
        public string? InventoryNumber { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? Type { get; set; }
        public string? PaymentTypes { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? CommissioningDate { get; set; }
        public DateTime? LastVerificationDate { get; set; }
        public int? VerificationIntervalMonths { get; set; }
        public DateTime? NextVerificationDate { get; set; }
        public int? ResourceHours { get; set; }
        public int? CurrentHours { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public int? MaintenanceDurationHours { get; set; }
        public string? Status { get; set; }
        public string? StatusText { get; set; }
        public decimal? TotalRevenue { get; set; }
        public DateTime? LastInventoryDate { get; set; }
        public string? LastVerificationBy { get; set; }
        public string? Franchisee { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public record BulkStatusUpdateDto
    (
        List<Guid> Ids,
        string? Status,
         string? StatusText
    );
    public record VendingMachineCsvDto
    (
        string? SerialNumber,
        string? InventoryNumber,
        string? Model,
        string? Manufacturer,
        string? Type,
        string? PaymentTypes,
        string? Location,
        string? Address,
        string? Country,
        DateTime? ManufactureDate,
        DateTime? CommissioningDate,
        DateTime? LastVerificationDate,
        int? VerificationIntervalMonths,
        int? ResourceHours,
        int? CurrentHours,
        DateTime? NextMaintenanceDate,
        int? MaintenanceDurationHours,
        string? Status,
        string? StatusText,
        decimal? TotalRevenue,
        DateTime? LastInventoryDate,
        string? LastVerificationBy,
        string? Franchisee
    );
}
