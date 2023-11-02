using System.Net;
using System.Text;
using System.Text.Json;
using EST.MIT.Web.Entities;
using EST.MIT.Web.Services;
using Microsoft.Extensions.Logging;
using EST.MIT.Web.Repositories;

namespace EST.MIT.Web.Test.Services;

public class InvoiceAPITests
{
    private Mock<IInvoiceRepository> _mockRepository;

    public InvoiceAPITests()
    {
        _mockRepository = new Mock<IInvoiceRepository>();
    }

    [Fact]
    public async void GetInvoiceAsync_ReturnsOk()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new Invoice()), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().NotBeNull();
    }

    [Fact]
    public async void GetInvoiceAsync_API_Has_No_Results_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
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

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().BeNull();
    }

    [Fact]
    public async void GetInvoiceAsync_ReturnsNotFound()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().BeNull();

    }

    [Fact]
    public async void GetInvoiceAsync_UnknownResponse_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetInvoiceAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.FindInvoiceAsync("123", "ABC");

        response.Should().BeNull();
    }

    [Fact]
    public async void SaveInvoiceAsync_ReturnsCreated()
    {
        _mockRepository.Setup(x => x.PostInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.SaveInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async void SaveInvoiceAsync_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PostInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.SaveInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void SaveInvoiceAsync_ThrowsException()
    {
        _mockRepository.Setup(x => x.PostInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.SaveInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async void UpdateInvoiceAsync_ReturnsOK()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void UpdateInvoiceAsync_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_ThrowsException()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_PR_ReturnsOK()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice(), new PaymentRequest());

        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void UpdateInvoiceAsync_PR_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice(), new PaymentRequest());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_PR_ThrowsException()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice(), new PaymentRequest());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_Lines_ReturnsOK()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice() { PaymentRequests = new List<PaymentRequest>() { { new PaymentRequest() } } }, new PaymentRequest(), new InvoiceLine());

        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void UpdateInvoiceAsync_Line_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice() { PaymentRequests = new List<PaymentRequest>() { { new PaymentRequest() } } }, new PaymentRequest(), new InvoiceLine());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void UpdateInvoiceAsync_Line_ThrowsException()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.UpdateInvoiceAsync(new Invoice() { PaymentRequests = new List<PaymentRequest>() { { new PaymentRequest() } } }, new PaymentRequest(), new InvoiceLine());

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void DeletePaymentRequestAsync_ReturnsOk()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.DeletePaymentRequestAsync(new Invoice(), "123");

        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void DeletePaymentRequestAsync_ReturnsBadRequest()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.DeletePaymentRequestAsync(new Invoice(), "123");

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void DeletePaymentRequestAsync_ThrowsException()
    {
        _mockRepository.Setup(x => x.PutInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.DeletePaymentRequestAsync(new Invoice(), "123");

        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async void GetApprovalsAsync_ReturnsOK()
    {
        var _invoice = new Invoice();
        _invoice.Update("approval");

        _mockRepository.Setup(x => x.GetApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<Invoice>() { _invoice }), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.GetApprovalsAsync();

        response.Should().NotBeNull();
        response.Count().Should().Be(1);
    }

    [Fact]
    public async void GetApprovalsAsync_API_Has_No_Results_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.GetApprovalsAsync();

        response.Should().BeNull();
    }

    [Fact]
    public async void GetApprovalsAsync_Deserialze_Fail_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(""), Encoding.UTF8, "application/json")
            });

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.GetApprovalsAsync();

        response.Should().BeNull();
    }

    [Fact]
    public async void GetApprovalsAsync_Blank_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<Invoice>()), Encoding.UTF8, "application/json")
            });

        // var mockReader = new Mock<HttpContent>();
        // mockReader.Setup(x => x.ReadFromJsonAsync<IEnumerable<Invoice>>(It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
        //     .ReturnsAsync(new List<Invoice>());


        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.GetApprovalsAsync();

        response.Should().HaveCount(0);
    }

    [Fact]
    public async void GetApprovalsAsync_ReturnsNotFound()
    {
        _mockRepository.Setup(x => x.GetApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.GetApprovalsAsync();

        response.Should().BeNull();
    }

    [Fact]
    public async void GetApprovalsAsync_UnknownResponse_ReturnsNull()
    {
        _mockRepository.Setup(x => x.GetApprovalsAsync())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var service = new InvoiceAPI(_mockRepository.Object, new Mock<ILogger<InvoiceAPI>>().Object);
        var response = await service.GetApprovalsAsync();

        response.Should().BeNull();
    }
}