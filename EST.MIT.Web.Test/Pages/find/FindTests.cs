// using Microsoft.AspNetCore.Components;
// using Microsoft.Extensions.DependencyInjection;
// using EST.MIT.Web.Entities;
// using EST.MIT.Web.Pages.find.Find;
// using EST.MIT.Web.Services;
// using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

// namespace EST.MIT.Web.Tests.Pages;

// public class FindTests : TestContext
// {
//     private readonly Mock<IPageServices> _mockPageService;
//     private readonly Mock<IFindService> _mockFindService;
//     private readonly Invoice _invoice;
//     public FindTests()
//     {
//         _invoice = new Invoice()
//         {
//             Id = Guid.NewGuid(),
//             SchemeType = "BPS",
//         };

//         _mockPageService = new Mock<IPageServices>();
//         _mockFindService = new Mock<IFindService>();
//         Services.AddSingleton<IPageServices>(_mockPageService.Object);
//         Services.AddSingleton<IFindService>(_mockFindService.Object);
//     }

//     [Fact]
//     public void Search_Navigates_To_Summary()
//     {
//         _mockFindService.Setup(x => x.FetchInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_invoice);

//         var component = RenderComponent<Find>();

//         component.FindAll("input#invoicenumber")[0].Change(_invoice.Id.ToString());
//         component.FindAll("input#scheme")[0].Change("BPS");

//         var button = component.FindAll("button.govuk-button")[0];
//         button.Click();

//         var navigationManager = Services.GetService<NavigationManager>();
//         navigationManager?.Uri.Should().Be($"http://localhost/invoice/summary/BPS/{_invoice.Id.ToString()}");
//     }

//     [Fact]
//     public void Search_Fields_Are_Required()
//     {
//         bool IsErrored = false;
//         Dictionary<string, string> errors = new();

//         var component = RenderComponent<Find>();

//         var button = component.FindAll("button.govuk-button")[0];
//         button.Click();

//         _mockPageService.Verify(x => x.Validation(component.Instance._searchCriteria, out IsErrored, out errors), Times.Once);
//     }

//     [Fact]
//     public void Invoices_Not_Found()
//     {
//         _mockFindService.Setup(x => x.FetchInvoiceAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<Invoice>(null));

//         var component = RenderComponent<Find>();

//         component.FindAll("input#invoicenumber")[0].Change("Goat");
//         component.FindAll("input#scheme")[0].Change("BPS");

//         var button = component.FindAll("button.govuk-button")[0];
//         button.Click();

//         component.Instance.invoice.Should().BeNull();

//     }
// }

