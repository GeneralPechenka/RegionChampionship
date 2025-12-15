using AutoMapper;
using Domain.Entities;
using DTOs.Core.PayType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MappingProfiles
{
    public class PayTypeMapProfile : Profile
    {
        public PayTypeMapProfile()
        {
            CreateMap<PayType, PayTypeShortDto>();

            CreateMap<CreateUpdatePayTypeDto, PayType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VendingMachines, opt => opt.Ignore());

            CreateMap<PayType, PayTypeDetailsDto>()
                .ForMember(dest => dest.VendingMachinesCount,
                    opt => opt.MapFrom(src => src.VendingMachines != null ? src.VendingMachines.Count : 0));
        }
    }
}
