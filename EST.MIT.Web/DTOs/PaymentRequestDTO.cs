using Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EST.MIT.Web.DTOs;

[ExcludeFromCodeCoverage]
public class PaymentRequestDTO
{
    [JsonProperty("paymentRequestId")]
    public string PaymentRequestId { get; init; } = default!;

    [JsonProperty("sourceSystem")]
    public string SourceSystem { get; init; } = default!;

    [JsonProperty("frn")]
    public long FRN { get; init; } = default!;

    [JsonProperty("value")]
    public decimal Value { get; init; }

    [JsonProperty("currency")]
    public string Currency { get; init; } = default!;

    [JsonProperty("description")]
    public string Description { get; init; } = default!;

    [JsonProperty("originalInvoiceNumber")]
    public string OriginalInvoiceNumber { get; init; } = default!;

    [JsonProperty("originalSettlementDate")]
    public DateTime OriginalSettlementDate { get; init; } = default!;

    [JsonProperty("recoveryDate")]
    public DateTime RecoveryDate { get; init; } = default!;

    [JsonProperty("invoiceCorrectionReference")]
    public string InvoiceCorrectionReference { get; init; } = default!;

    [JsonProperty("invoiceLines")]
    public List<InvoiceLineDTO> InvoiceLines { get; init; } = default!;

    [JsonProperty("marketingYear")]
    [Range(2021, 2099, ErrorMessage = "Marketing Year must be between 2021 and 2099 ")]
    public int MarketingYear { get; init; }

    [JsonProperty("paymentRequestNumber")]
    public int PaymentRequestNumber { get; init; }

    [JsonProperty("agreementNumber")]
    public string AgreementNumber { get; init; } = default!;

    [JsonProperty("dueDate")]
    public string DueDate { get; init; } = default!;

    [JsonProperty("appendixReferences")]
    public AppendixReferencesDTO AppendixReferences { get; init; } = default!;

    [JsonProperty("sbi")]
    public int SBI { get; init; } = default!;

    [JsonProperty("vendor")]
    public string Vendor { get; init; } = default!;
}