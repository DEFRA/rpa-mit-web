using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using AutoMapper;
using Entities;
using EST.MIT.Web.DTOs;
using Microsoft.AspNetCore.Routing.Constraints;

namespace EST.MIT.Web.AutoMapperProfiles;

[ExcludeFromCodeCoverage]
public class InvoiceAPIMapper : Profile
{
    public InvoiceAPIMapper()
    {
        CreateMap<(PaymentRequestDTO, InvoiceLine), InvoiceLineDTO>()
            .ForMember(dest => dest.Value, act => act.MapFrom(src => src.Item2.Value))
            .ForMember(dest => dest.FundCode, act => act.MapFrom(src => "NOTMAPPED")) // need to capture these maybe using the combinations endpoint show as dropdown
            .ForMember(dest => dest.MainAccount, act => act.MapFrom(src => "NOTMAPPED")) // need to capture these maybe using the combinations endpoint show as dropdown
            .ForMember(dest => dest.SchemeCode, act => act.MapFrom(src => src.Item2.SchemeCode))
            .ForMember(dest => dest.MarketingYear, act => act.MapFrom(src => src.Item1.MarketingYear)) // bring to this level from the header
            .ForMember(dest => dest.DeliveryBody, act => act.MapFrom(src => src.Item2.DeliveryBody))
            .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Item2.Description))
            ;
        
        CreateMap<PaymentRequest, PaymentRequestDTO>()
            .ForMember(dest => dest.PaymentRequestId, act => act.MapFrom(src => src.PaymentRequestId))
            .ForMember(dest => dest.SourceSystem, act => act.MapFrom(src => src.SourceSystem))
            .ForMember(dest => dest.Value, act => act.MapFrom(src => src.Value))
            .ForMember(dest => dest.Currency, act => act.MapFrom(src => src.Currency))
            .ForMember(dest => dest.Description, act => act.MapFrom(src => src.InvoiceLines.Any() ? src.InvoiceLines.First().Description : "NOTMAPPED")) // should be at the line level
            .ForMember(dest => dest.OriginalInvoiceNumber, act => act.MapFrom(src => "NOTMAPPED")) // AR fields potentially
            .ForMember(dest => dest.OriginalSettlementDate, act => act.MapFrom(src => DateTime.Now)) // TODO: NOTMAPPED // AR fields potentially
            .ForMember(dest => dest.RecoveryDate, act => act.MapFrom(src => DateTime.Now)) // TODO: NOTMAPPED // AR fields potentially
            .ForMember(dest => dest.InvoiceCorrectionReference, act => act.MapFrom(src => "NOTMAPPED")) // AR fields potentially
            .ForMember(dest => dest.MarketingYear, act => act.MapFrom(src => src.MarketingYear))
            .ForMember(dest => dest.PaymentRequestNumber, act => act.MapFrom(src => src.PaymentRequestNumber))
            .ForMember(dest => dest.AgreementNumber, act => act.MapFrom(src => src.AgreementNumber))
            .ForMember(dest => dest.DueDate, act => act.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.FRN, act => act.MapFrom(src => src.FRN.ToString().Length == 10 ? src.FRN : 0))
            .ForMember(dest => dest.Vendor, act => act.MapFrom(src => src.FRN.ToString().Length == 6 ? src.FRN.ToString() : ""))
            .ForMember(dest => dest.SBI, act => act.MapFrom(src => src.FRN >= 105000000 && src.FRN <= 999999999 ? src.FRN : 0));

        CreateMap<Invoice, PaymentRequestsBatchDTO>()
            .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
            .ForMember(dest => dest.AccountType, act => act.MapFrom(src => src.AccountType))
            .ForMember(dest => dest.Organisation, act => act.MapFrom(src => src.Organisation))
            .ForMember(dest => dest.PaymentType, act => act.MapFrom(src => src.PaymentType))
            .ForMember(dest => dest.SchemeType, act => act.MapFrom(src => src.SchemeType))
            .ForMember(dest => dest.Status, act => act.MapFrom(src => src.Status))
            .ForMember(dest => dest.Reference, act => act.MapFrom(src => src.Reference));
    }
}