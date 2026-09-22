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
            CreateMap<UpdateOrderRequest, TrxOrder>();
            CreateMap<UpdateOrderDetailRequest, TrxOrdersDetail>();
            CreateMap<TrxOrder, UpdateOrderMessage>();
            CreateMap<TrxOrdersDetail, UpdateOrderDetailMessage>();
            CreateMap<TrxOrder, DeleteOrderMessage>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ListDeleteOrderDetails, opt => opt.MapFrom(src => src.TrxOrdersDetails));
            CreateMap<TrxOrdersDetail, DeleteOrderDetails>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<TrxOrder, OrdersDto>()
                .ForMember(dest => dest.OrderDetailsDto, opt=>opt.MapFrom(src=>src.TrxOrdersDetails));
            CreateMap<TrxOrdersDetail, OrderDetailDto>();
        }
    }
}
