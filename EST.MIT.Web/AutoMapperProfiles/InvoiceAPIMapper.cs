using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Entities;
using EST.MIT.Web.DTOs;

namespace EST.MIT.Web.AutoMapperProfiles;

[ExcludeFromCodeCoverage]
public class InvoiceAPIMapper : Profile
{
    public InvoiceAPIMapper()
    {
        CreateMap<InvoiceLine, InvoiceLineDTO>();

        // This is to map to what was the invoice header
        // if you wanted to map from multiple sources, you can do this:
        CreateMap<(Invoice, InvoiceLine), PaymentRequestDTO>();
        
        CreateMap<Invoice, PaymentRequestsBatchDTO>();
    }
}