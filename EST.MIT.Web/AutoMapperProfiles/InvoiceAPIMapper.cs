using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using EST.MIT.Web.Entities;
using EST.MIT.Web.DTOs;
using System;

namespace EST.MIT.Web.AutoMapperProfiles;

[ExcludeFromCodeCoverage]
public class InvoiceAPIMapper : Profile
{
    public InvoiceAPIMapper()
    {
        CreateMap<InvoiceLine, InvoiceLineDTO>()
            .ForMember(dest => dest.MarketingYear, act => act.MapFrom(src => !string.IsNullOrWhiteSpace(src.MarketingYear) ? Convert.ToInt32(src.MarketingYear) : 0));

        CreateMap<InvoiceLineDTO, InvoiceLine>()
            .ForMember(dest => dest.Id, act => act.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.MarketingYear, act => act.MapFrom(src => src.MarketingYear <= 0 ? string.Empty : src.MarketingYear.ToString()));

        CreateMap<PaymentRequest, PaymentRequestDTO>()
            .ForMember(dest => dest.FRN, act => act.MapFrom(src => !string.IsNullOrWhiteSpace(src.FRN) ? Convert.ToInt64(src.FRN) : 0))
            .ForMember(dest => dest.SBI, act => act.MapFrom(src => !string.IsNullOrWhiteSpace(src.SBI) ? Convert.ToInt64(src.SBI) : 0))
            .ForMember(dest => dest.MarketingYear, act => act.MapFrom(src => !string.IsNullOrWhiteSpace(src.MarketingYear) ? Convert.ToInt32(src.MarketingYear) : 0));

        CreateMap<PaymentRequestDTO, PaymentRequest>()
            .ForMember(dest => dest.FRN, act => act.MapFrom(src => src.FRN <= 0 ? string.Empty : src.FRN.ToString()))
            .ForMember(dest => dest.SBI, act => act.MapFrom(src => src.SBI <= 0 ? string.Empty : src.SBI.ToString()))
            .ForMember(dest => dest.MarketingYear, act => act.MapFrom(src => src.MarketingYear <= 0 ? string.Empty : src.MarketingYear.ToString()));

        CreateMap<Invoice, PaymentRequestsBatchDTO>()
            .ForMember(dest => dest.Created, act => act.MapFrom(src => src.Created.LocalDateTime))
            .ForMember(dest => dest.Updated, act => act.MapFrom(src => src.Updated.LocalDateTime));

        CreateMap<PaymentRequestsBatchDTO, Invoice>()
            .ForMember(dest => dest.Created, act => act.MapFrom(src => new DateTimeOffset(src.Created, TimeZoneInfo.Local.GetUtcOffset(src.Created))))
            .ForMember(dest => dest.Updated, act => act.MapFrom(src => src.Updated.HasValue == false ? default(DateTimeOffset) : new DateTimeOffset(src.Updated.Value, TimeZoneInfo.Local.GetUtcOffset(src.Updated.Value))));

    }
}