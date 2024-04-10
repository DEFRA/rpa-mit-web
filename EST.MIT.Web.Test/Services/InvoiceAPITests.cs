using System.Net;
using System.Text;
using System.Text.Json;
using AutoMapper;
using EST.MIT.Web.AutoMapperProfiles;
using EST.MIT.Web.DTOs;
using EST.MIT.Web.Entities;
using Microsoft.Extensions.Logging;
using EST.MIT.Web.Interfaces;
using EST.MIT.Web.Models;
using EST.MIT.Web.APIs;

namespace EST.MIT.Web.Test.Services;

public class InvoiceAPITests
{
    private Mock<IInvoiceRepository> _mockRepository;
    private readonly IMapper _autoMapper;

    public InvoiceAPITests()
    {
        _mockRepository = new Mock<IInvoiceRepository>();
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new InvoiceAPIMapper());
        });
        _autoMapper = mapperConfig.CreateMapper();

    }

    [Fact]
    public async void FindInvoiceBySearchCriteria()
    {
        _mockRepository.Setup(x => x.GetInvoiceByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new Invoice()), Encoding.UTF8, "application/json")
            });
        _mockRepository.Setup(x => x.GetInvoiceByPaymentRequestIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new Invoice()), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);

        var findByInvoiceIdResponse = await service.FindInvoiceAsync(new SearchCriteria() { InvoiceNumber = Guid.NewGuid().ToString() });
        findByInvoiceIdResponse.Should().NotBeNull();

        var findWithNoCriteriaResponse = await service.FindInvoiceAsync(new SearchCriteria());
        findWithNoCriteriaResponse.Should().BeNull();

        var findWithNullCriteriaResponse = await service.FindInvoiceAsync(null!);
        findWithNullCriteriaResponse.Should().BeNull();

        var findByPaymentRequestIdResponse = await service.FindInvoiceAsync(new SearchCriteria() { PaymentRequestId = "abcd_12345" });
        findByInvoiceIdResponse.Should().NotBeNull();
    }

    [Fact]
    public async void FindInvoiceBySearchCriteria_Deserialze_Fail_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetInvoiceByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(""), Encoding.UTF8, "application/json")
            });
        _mockRepository.Setup(x => x.GetInvoiceByPaymentRequestIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(""), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var findByInvoiceIdResponse = await service.FindInvoiceAsync(new SearchCriteria() { InvoiceNumber = Guid.NewGuid().ToString() });
        findByInvoiceIdResponse.Should().BeNull();

        var findByInvoicePaymentRequestIdResponse = await service.FindInvoiceAsync(new SearchCriteria() { PaymentRequestId = "abcd_12345" });
        findByInvoicePaymentRequestIdResponse.Should().BeNull();
    }

    [Fact]
    public async void FindInvoiceBySearchCriteria_NoContent()
    {
        _mockRepository.Setup(x => x.GetInvoiceByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
        _mockRepository.Setup(x => x.GetInvoiceByPaymentRequestIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);

        var findByInvoiceIdResponse = await service.FindInvoiceAsync(new SearchCriteria() { InvoiceNumber = Guid.NewGuid().ToString() });
        findByInvoiceIdResponse.Should().BeNull();

        var findByPaymentRequestIdResponse = await service.FindInvoiceAsync(new SearchCriteria() { PaymentRequestId = "abcd_12345" });
        findByPaymentRequestIdResponse.Should().BeNull();
    }

    [Fact]
    public async void FindInvoiceBySearchCriteria_NotFound()
    {
        _mockRepository.Setup(x => x.GetInvoiceByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
        _mockRepository.Setup(x => x.GetInvoiceByPaymentRequestIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);

        var findByInvoiceIdResponse = await service.FindInvoiceAsync(new SearchCriteria() { InvoiceNumber = Guid.NewGuid().ToString() });
        findByInvoiceIdResponse.Should().BeNull();

        var findByPaymentRequestIdResponse = await service.FindInvoiceAsync(new SearchCriteria() { PaymentRequestId = "abcd_12345" });
        findByInvoiceIdResponse.Should().BeNull();
    }

    [Fact]
    public async void GetInvoiceAsync_ReturnsOk()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new Invoice()), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().NotBeNull();
    }

    [Fact]
    public async void GetInvoiceAsync_API_Has_No_Results_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().BeNull();
    }

    [Fact]
    public async void GetInvoiceAsync_Deserialze_Fail_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(""), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().BeNull();
    }

    [Fact]
    public async void GetInvoiceAsync_ReturnsNotFound()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().BeNull();

    }

    [Fact]
    public async void GetInvoiceAsync_UnknownResponse_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().BeNull();
    }

    [Fact]
    public async void SaveInvoiceAsync_ReturnsCreated()
    {
        _mockRepository.Setup(x => x.PostInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.SaveInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async void SaveInvoiceAsync_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PostInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.SaveInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void SaveInvoiceAsync_ThrowsException()
    {
        _mockRepository.Setup(x => x.PostInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.SaveInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async void UpdateInvoiceAsync_ReturnsOK()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void UpdateInvoiceAsync_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_ThrowsException()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_PR_ReturnsOK()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice(), new PaymentRequest());

        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void UpdateInvoiceAsync_PR_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice(), new PaymentRequest());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_PR_ThrowsException()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice(), new PaymentRequest());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_Lines_ReturnsOK()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice() { PaymentRequests = new List<PaymentRequest>() { { new PaymentRequest() } } }, new PaymentRequest(), new InvoiceLine());

        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void UpdateInvoiceAsync_AddInvoiceLine_SetsValueCorrectly()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Setup. NB. Deliberately start with incorrect Value for empty PaymentRequest
        var invoice = new Invoice() { AccountType = "AP", Organisation = "RPA", SchemeType = "DA", PaymentType = "EU" };
        var paymentRequest = new PaymentRequest() { PaymentRequestId = "1", Value = 1.23M };
        invoice.PaymentRequests.Add(paymentRequest);
        var invoiceLine1 = new InvoiceLine() { Value = 2.34M };
        var invoiceLine2 = new InvoiceLine() { Value = 1.11M };

        // Add First line (2.34)
        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(invoice, paymentRequest, invoiceLine1);
        var savedInvoice = response.Data;
        savedInvoice.PaymentRequests.First(x => x.PaymentRequestId == "1").Value.Should().Be(2.34M);

        // Add Second Line (1.11)
        response = await service.UpdateInvoiceAsync(invoice, paymentRequest, invoiceLine2);
        savedInvoice = response.Data;
        savedInvoice.PaymentRequests.First(x => x.PaymentRequestId == "1").Value.Should().Be(3.45M);

        // Edit Second Line (1.11 => 2.22)
        invoiceLine2.Value = 2.22M;
        response = await service.UpdateInvoiceAsync(invoice, paymentRequest, invoiceLine2);
        savedInvoice = response.Data;
        savedInvoice.PaymentRequests.First(x => x.PaymentRequestId == "1").Value.Should().Be(4.56M);
    }

    [Fact]
    public async void UpdateInvoiceAsync_PaymentRequest_Values_Set_Correctly()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        Invoice invoice = null!;
        var response = await service.UpdateInvoiceAsync(invoice);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Setup. NB. Deliberately start with incorrect Value for PaymentRequest
        invoice = new Invoice() { AccountType = "AP", Organisation = "RPA", SchemeType = "DA", PaymentType = "EU" };
        var paymentRequest1 = new PaymentRequest() { PaymentRequestId = "1", Value = 1.23M };
        var invoiceLine1 = new InvoiceLine() { Value = 2.34M };
        var invoiceLine2 = new InvoiceLine() { Value = 1.11M };
        paymentRequest1.InvoiceLines.Add(invoiceLine1);
        paymentRequest1.InvoiceLines.Add(invoiceLine2);
        invoice.PaymentRequests.Add(paymentRequest1);
        response = await service.UpdateInvoiceAsync(invoice);
        var savedInvoice = response.Data;
        savedInvoice.PaymentRequests.First(x => x.PaymentRequestId == "1").Value.Should().Be(3.45M);
    }

    [Fact]
    public async void UpdateInvoiceAsync_Line_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice() { PaymentRequests = new List<PaymentRequest>() { { new PaymentRequest() } } }, new PaymentRequest(), new InvoiceLine());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_Line_ThrowsException()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.UpdateInvoiceAsync(new Invoice() { PaymentRequests = new List<PaymentRequest>() { { new PaymentRequest() } } }, new PaymentRequest(), new InvoiceLine());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void DeletePaymentRequestAsync_ReturnsOk()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.DeletePaymentRequestAsync(new Invoice(), "123");

        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void DeletePaymentRequestAsync_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.DeletePaymentRequestAsync(new Invoice(), "123");

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void DeletePaymentRequestAsync_ThrowsException()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<PaymentRequestsBatchDTO>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.DeletePaymentRequestAsync(new Invoice(), "123");

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void GetApprovalsAsync_ReturnsOK()
    {
        var _invoice = new Invoice();
        _invoice.Update(InvoiceStatuses.AwaitingApproval);

        _mockRepository.Setup(x => x.GetAllApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<Invoice>() { _invoice }), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.GetAllApprovalInvoicesAsync();

        response.Should().NotBeNull();
        response.Count().Should().Be(1);
    }

    [Fact]
    public async void GetApprovalsAsync_API_Has_No_Results_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetAllApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.GetAllApprovalInvoicesAsync();

        response.Should().BeNull();
    }

    [Fact]
    public async void GetApprovalsAsync_Deserialze_Fail_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetAllApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(""), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.GetAllApprovalInvoicesAsync();

        response.Should().BeNull();
    }

    [Fact]
    public async void GetApprovalsAsync_Blank_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetAllApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<Invoice>()), Encoding.UTF8, "application/json")
            });

        // var mockReader = new Mock<HttpContent>();
        // mockReader.Setup(x => x.ReadFromJsonAsync<IEnumerable<Invoice>>(It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
        //     .ReturnsAsync(new List<Invoice>());


        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.GetAllApprovalInvoicesAsync();

        response.Should().HaveCount(0);
    }

    [Fact]
    public async void GetApprovalsAsync_ReturnsNotFound()
    {
        _mockRepository.Setup(x => x.GetAllApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.GetAllApprovalInvoicesAsync();

        response.Should().BeNull();
    }

    [Fact]
    public async void GetApprovalsAsync_UnknownResponse_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetAllApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object, _autoMapper);
        var response = await service.GetAllApprovalInvoicesAsync();

        response.Should().BeNull();
    }

    [Fact]
    public async Task GetInvoicesAsync_ReturnsOk()
    {
        var mockInvoices = new List<Invoice>();

        _mockRepository.Setup(x => x.GetInvoicesAsync(string.Empty))
          .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
          {
              Content = new StringContent(JsonSerializer.Serialize(mockInvoices))
          });

        var service = new InvoiceAPI(_mockRepository.Object, Mock.Of<ILogger<InvoiceAPI>>(), _autoMapper);
        var result = await service.GetInvoicesAsync(string.Empty);

        result.Should().Equal(mockInvoices);
    }

    [Fact]
    public async Task GetInvoicesAsync_HandlesException()
    {
        _mockRepository.Setup(x => x.GetInvoicesAsync(string.Empty))
          .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, Mock.Of<ILogger<InvoiceAPI>>(), _autoMapper);
        var result = await service.GetInvoicesAsync(string.Empty);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetInvoicesAsync_HandlesEmptyResponse()
    {
        _mockRepository.Setup(x => x.GetInvoicesAsync(string.Empty))
          .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
          {
              Content = new StringContent("")
          });

        var service = new InvoiceAPI(_mockRepository.Object, Mock.Of<ILogger<InvoiceAPI>>(), _autoMapper);
        var result = await service.GetInvoicesAsync(string.Empty);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetInvoicesAsync_ReturnsOk_WhenDeserializationSucceeds()
    {
        var mockInvoices = new List<Invoice>();

        _mockRepository.Setup(x => x.GetInvoicesAsync(string.Empty))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(mockInvoices))
            });

        var service = new InvoiceAPI(_mockRepository.Object, Mock.Of<ILogger<InvoiceAPI>>(), _autoMapper);
        var result = await service.GetInvoicesAsync(string.Empty);

        result.Should().Equal(mockInvoices);
    }

    [Fact]
    public async Task GetInvoicesAsync_ReturnsNull_WhenDeserializationFails()
    {
        _mockRepository.Setup(x => x.GetInvoicesAsync(string.Empty))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("invalid JSON")
            });

        var service = new InvoiceAPI(_mockRepository.Object, Mock.Of<ILogger<InvoiceAPI>>(), _autoMapper);
        var result = await service.GetInvoicesAsync(string.Empty);

        result.Should().BeNull();
    }
}