using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using DTOs.Core.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MappingProfiles
{
    public class MaintenanceProfile : Profile
    {
        public MaintenanceProfile()
        {
            // Maintenance маппинги
            CreateMap<Maintenance, MaintenanceShortDto>()
                .ForMember(dest => dest.VendingMachineName,
                    opt => opt.MapFrom(src => src.VendingMachine != null ? src.VendingMachine.Name : null))
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null));

            CreateMap<CreateUpdateMaintenanceDto, Maintenance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VendingMachine, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore());

            CreateMap<Maintenance, MaintenanceDetailsDto>()
                .ForMember(dest => dest.VendingMachineName,
                    opt => opt.MapFrom(src => src.VendingMachine != null ? src.VendingMachine.Name : null))
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
                .ForMember(dest => dest.VendingMachineModel,
                    opt => opt.MapFrom(src => src.VendingMachine != null ? src.VendingMachine.Model : null))
                .ForMember(dest => dest.VendingMachineLocation,
                    opt => opt.MapFrom(src => src.VendingMachine != null ? src.VendingMachine.Location : null));
        }
    }

    public class EngineerTaskProfile : Profile
    {
        public EngineerTaskProfile()
        {
            // EngineerTask маппинги
            CreateMap<EngineerTask, EngineerTaskShortDto>()
                .ForMember(dest => dest.VendingMachineName,
                    opt => opt.MapFrom(src => src.VendingMachine != null ? src.VendingMachine.Name : null))
                .ForMember(dest => dest.AssignedToName,
                    opt => opt.MapFrom(src => src.AssignedTo != null ? src.AssignedTo.FullName : null));

            CreateMap<CreateUpdateEngineerTaskDto, EngineerTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => TaskStatusEnum.New))
                .ForMember(dest => dest.CompletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.VendingMachine, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedTo, opt => opt.Ignore());

            // Для обновления - исключаем поля, которые не должны меняться при обновлении
            CreateMap<CreateUpdateEngineerTaskDto, EngineerTask>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !opts.DestinationMember.Name.Equals(nameof(EngineerTask.Status))));

            // Маппинг для assign операции
            CreateMap<AssignEngineerTaskDto, EngineerTask>()
                .ForMember(dest => dest.AssignedToId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

    // Профиль для календаря
    public class CalendarProfile : Profile
    {
        public CalendarProfile()
        {
            CreateMap<EngineerTask, CalendarTaskDto>()
                .ForMember(dest => dest.VendingMachineName,
                    opt => opt.MapFrom(src => src.VendingMachine != null ? src.VendingMachine.Name : null))
                .ForMember(dest => dest.ColorCode, opt => opt.Ignore()) // Рассчитывается в контроллере
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }

    // Профиль для авто-генерации задач
    public class TaskGenerationProfile : Profile
    {
        public TaskGenerationProfile()
        {
            // Генерация задач из данных ТА
            CreateMap<VendingMachine, EngineerTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => $"Плановое ТО аппарата {src.Name}"))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src =>
                        $"Плановое техническое обслуживание. Следующая поверка: {src.NextVerificationDate:dd.MM.yyyy}"))
                .ForMember(dest => dest.DueDate,
                    opt => opt.MapFrom(src => src.NextVerificationDate ?? DateTime.UtcNow.AddMonths(1)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => TaskStatusEnum.New))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(_ => 5))
                .ForMember(dest => dest.EstimatedHours, opt => opt.MapFrom(_ => 4))
                .ForMember(dest => dest.VendingMachineId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AssignedToId, opt => opt.Ignore())
                .ForMember(dest => dest.VendingMachine, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedTo, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedDate, opt => opt.Ignore());
        }
    }
}
