using System;
using AutoMapper;
using MessageMQCommon.MQ.Messages.OrderMsv;
using Order.Msv.DTOs;
using Order.Msv.Models;

namespace Order.Msv.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrderRequest, TrxOrder>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt ?? DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt ?? DateTime.UtcNow));
                

            CreateMap<CreateOrderDetailRequest, TrxOrdersDetail>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt ?? DateTime.UtcNow))
                .ForMember(dest => dest.OrderId, opt => opt.Ignore());

            CreateMap<TrxOrder, OrderMessage>();
            CreateMap<TrxOrdersDetail, OrderDetailMessage>();
        }
    }
}
