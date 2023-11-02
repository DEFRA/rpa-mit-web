using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using EST.MIT.Web.Entities;
using EST.MIT.Web.DTOs;

namespace EST.MIT.Web.AutoMapperProfiles;

[ExcludeFromCodeCoverage]
public class InvoiceAPIMapper : Profile
{
    public InvoiceAPIMapper()
    {
        CreateMap<InvoiceLine, InvoiceLineDTO>()
            .ForMember(dest => dest.Value, act => act.MapFrom(src => src.Value))
            .ForMember(dest => dest.FundCode, act => act.MapFrom(src => src.FundCode))
            .ForMember(dest => dest.MainAccount, act => act.MapFrom(src => src.MainAccount))
            .ForMember(dest => dest.SchemeCode, act => act.MapFrom(src => src.SchemeCode))
            .ForMember(dest => dest.MarketingYear, act => act.MapFrom(src => src.MarketingYear))
            .ForMember(dest => dest.DeliveryBody, act => act.MapFrom(src => src.DeliveryBody))
            .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Description))
            ;

        CreateMap<PaymentRequest, PaymentRequestDTO>()
            .ForMember(dest => dest.PaymentRequestId, act => act.MapFrom(src => src.PaymentRequestId))
            .ForMember(dest => dest.SourceSystem, act => act.MapFrom(src => src.SourceSystem))
            .ForMember(dest => dest.Value, act => act.MapFrom(src => src.Value))
            .ForMember(dest => dest.Currency, act => act.MapFrom(src => src.Currency))
            .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Description))
            .ForMember(dest => dest.OriginalInvoiceNumber, act => act.MapFrom(src => src.OriginalInvoiceNumber))
            .ForMember(dest => dest.OriginalSettlementDate, act => act.MapFrom(src => src.OriginalSettlementDate))
            .ForMember(dest => dest.RecoveryDate, act => act.MapFrom(src => src.RecoveryDate))
            .ForMember(dest => dest.InvoiceCorrectionReference, act => act.MapFrom(src => src.InvoiceCorrectionReference))
            .ForMember(dest => dest.MarketingYear, act => act.MapFrom(src => src.MarketingYear))
            .ForMember(dest => dest.PaymentRequestNumber, act => act.MapFrom(src => src.PaymentRequestNumber))
            .ForMember(dest => dest.AgreementNumber, act => act.MapFrom(src => src.AgreementNumber))
            .ForMember(dest => dest.DueDate, act => act.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.FRN, act => act.MapFrom(src => !string.IsNullOrWhiteSpace(src.FRN) ? Convert.ToInt64(src.FRN) : 0))
            .ForMember(dest => dest.Vendor, act => act.MapFrom(src => src.Vendor))
            .ForMember(dest => dest.SBI, act => act.MapFrom(src => !string.IsNullOrWhiteSpace(src.SBI) ? Convert.ToInt64(src.SBI) : 0))
            ;

        CreateMap<Invoice, PaymentRequestsBatchDTO>()
            .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
            .ForMember(dest => dest.AccountType, act => act.MapFrom(src => src.AccountType))
            .ForMember(dest => dest.Organisation, act => act.MapFrom(src => src.Organisation))
            .ForMember(dest => dest.PaymentType, act => act.MapFrom(src => src.PaymentType))
            .ForMember(dest => dest.SchemeType, act => act.MapFrom(src => src.SchemeType))
            .ForMember(dest => dest.Status, act => act.MapFrom(src => src.Status))
            .ForMember(dest => dest.Reference, act => act.MapFrom(src => src.Reference))
            .ForMember(dest => dest.Created, act => act.MapFrom(src => src.Created.LocalDateTime))
            .ForMember(dest => dest.Updated, act => act.MapFrom(src => src.Updated.LocalDateTime))
            ;
    }
}