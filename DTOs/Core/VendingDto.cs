using CsvHelper.Configuration.Attributes;
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
    public record VendingMachineShortDto
    (
        Guid Id,
        string Name,
        string Model,
        string Location,
        string SerialNumber,
        string Status,
        string CompanyName,
        string ModemNumber,
        DateTime? NextMaintenanceDate
    );

    // DTO для создания/обновления (только то, что можно менять)
    public record CreateUpdateVendingMachineDto
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
        public string Status { get; set; }
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
        public string Status { get; set; }
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
        public string Status { get; set; }
        public string ConnectionStatus { get; set; } // Из API
        public string LoadingInfo { get; set; } // Из API
        public decimal CashAmount { get; set; } // Из API
        public string Events { get; set; }
        public string EquipmentStatus { get; set; }
        public string AdditionalInfo { get; set; }
    }
    public class VendingMachineExportDto
    {
        [Name("id")]
        public Guid Id { get; set; }

        [Name("serialNumber")]
        public string SerialNumber { get; set; } = string.Empty;

        [Name("inventoryNumber")]
        public string InventoryNumber { get; set; } = string.Empty;

        [Name("name")]
        public string Name { get; set; } = string.Empty;

        [Name("model")]
        public string Model { get; set; } = string.Empty;

        [Name("manufacturer")]
        public string Manufacturer { get; set; } = string.Empty;

        [Name("location")]
        public string Location { get; set; } = string.Empty;

        [Name("address")]
        public string Address { get; set; } = string.Empty;

        [Name("status")]
        [Format("G")] // Для enum
        public string Status { get; set; } = string.Empty;

        [Name("companyName")]
        public string CompanyName { get; set; } = string.Empty;

        [Name("country")]
        public string CountryName { get; set; } = string.Empty;

        [Name("manufactureDate")]
        [Format("yyyy-MM-dd")]
        public DateTime ManufactureDate { get; set; }

        [Name("commissioningDate")]
        [Format("yyyy-MM-dd")]
        public DateTime CommissioningDate { get; set; }

        [Name("lastVerificationDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? LastVerificationDate { get; set; }

        [Name("verificationIntervalMonths")]
        public int? VerificationIntervalMonths { get; set; }

        [Name("nextVerificationDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? NextVerificationDate { get; set; }

        [Name("resourceHours")]
        public int ResourceHours { get; set; }

        [Name("nextMaintenanceDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? NextMaintenanceDate { get; set; }

        [Name("maintenanceDurationHours")]
        public int MaintenanceDurationHours { get; set; }

        [Name("totalRevenue")]
        public decimal TotalRevenue { get; set; }

        [Name("inventoryDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? InventoryDate { get; set; }

        [Name("createdAt")]
        [Format("yyyy-MM-dd HH:mm:ss")]
        public DateTime CreatedAt { get; set; }

        [Name("lastVerificationBy")]
        public string LastVerificationBy { get; set; } = string.Empty;

        [Name("modemImei")]
        public string ModemImei { get; set; } = string.Empty;

        [Name("modemProvider")]
        public string ModemProvider { get; set; } = string.Empty;
    }

    public record BulkStatusUpdateDto
    (
        List<Guid> Ids,
        string? Status,
         string? StatusText
    );
    public record VendingMachineCsvDto
    {
        [Name("serialNumber")]
        public string SerialNumber { get; init; } = string.Empty;

        [Name("inventoryNumber")]
        public string InventoryNumber { get; init; } = string.Empty;

        [Name("model")]
        public string Model { get; init; } = string.Empty;

        [Name("manufacturer")]
        public string Manufacturer { get; init; } = string.Empty;

        [Name("type")]
        public string Type { get; init; } = string.Empty;

        [Name("paymentTypes")]
        public string PaymentTypes { get; init; } = string.Empty;

        [Name("location")]
        public string Location { get; init; } = string.Empty;

        [Name("address")]
        public string Address { get; init; } = string.Empty;

        [Name("country")]
        public string Country { get; init; } = string.Empty;

        [Name("productionDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? ProductionDate { get; init; }

        [Name("commissioningDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? CommissioningDate { get; init; }

        [Name("lastVerificationDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? LastVerificationDate { get; init; }

        [Name("verificationIntervalMonths")]
        public int? VerificationIntervalMonths { get; init; }

        [Name("resourceHours")]
        public int? ResourceHours { get; init; }

        [Name("currentHours")]
        public int? CurrentHours { get; init; }

        [Name("nextMaintenanceDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? NextMaintenanceDate { get; init; }

        [Name("maintenanceDurationHours")]
        public int? MaintenanceDurationHours { get; init; }

        [Name("status")]
        public string Status { get; init; } = string.Empty;

        [Name("totalRevenue")]
        public decimal? TotalRevenue { get; init; }

        [Name("lastInventoryDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? LastInventoryDate { get; init; }

        [Name("lastVerificationBy")]
        public string LastVerificationBy { get; init; } = string.Empty;

        [Name("franchisee")]
        public string Franchisee { get; init; } = string.Empty;

        [Name("name")]
        public string Name { get; init; } = string.Empty;

        [Name("modemImei")]
        public string ModemImei { get; init; } = string.Empty;

        [Name("modemProvider")]
        public string ModemProvider { get; init; } = string.Empty;

        [Name("companyName")]
        public string CompanyName { get; init; } = string.Empty;

        [Name("nextVerificationDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? NextVerificationDate { get; init; }

        [Name("manufactureDate")]
        [Format("yyyy-MM-dd")]
        public DateTime? ManufactureDate { get; init; }
    }
}
