using AutoMapper;
using Domain.Entities;
using DTOs.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MappingProfiles
{
    public class VendingMapProfile : Profile
    {
        public VendingMapProfile()
        {
            // Для списка (максимально легкий)
            CreateMap<VendingMachine, VendingMachineShortDto>()
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : "-"))
                .ForMember(dest => dest.ModemNumber,
                    opt => opt.MapFrom(src => src.Modem != null ? src.Modem.PhoneNumber : "-"));

            // Для создания
            CreateMap<CreateUpdateVendingMachineDto, VendingMachine>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.InventoryDate, opt => opt.Ignore())
                .ForMember(dest => dest.NextVerificationDate, opt => opt.MapFrom(src =>
                    CalculateNextVerificationDate(src.LastVerificationDate, src.VerificationIntervalMonths)))
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Modem, opt => opt.Ignore())
                .ForMember(dest => dest.ProducerCountry, opt => opt.Ignore())
                .ForMember(dest => dest.LastVerificationEmployee, opt => opt.Ignore())
                .ForMember(dest => dest.Sales, opt => opt.Ignore())
                .ForMember(dest => dest.Maintenances, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentTypes, opt => opt.Ignore())
                .ForMember(dest => dest.ProductStocks, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore());

            // Для детального просмотра
            CreateMap<VendingMachine, VendingMachineDetailsDto>()
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : null))
                .ForMember(dest => dest.ModemImei,
                    opt => opt.MapFrom(src => src.Modem != null ? src.Modem.Imei : null))
                .ForMember(dest => dest.ModemProvider,
                    opt => opt.MapFrom(src => src.Modem != null ? src.Modem.Provider : null))
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.ProducerCountry != null ? src.ProducerCountry.Name : null))
                .ForMember(dest => dest.LastVerifiedBy,
                    opt => opt.MapFrom(src => src.LastVerificationEmployee != null ?
                        src.LastVerificationEmployee.FullName : null));
        }

        private DateTime? CalculateNextVerificationDate(DateTime? lastVerificationDate, int? verificationIntervalMonths)
        {
            return lastVerificationDate?.AddMonths(verificationIntervalMonths ?? 0);
        }
    }
}
