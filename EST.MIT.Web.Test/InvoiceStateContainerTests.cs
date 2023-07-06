using Entities;
using EST.MIT.Web.Shared;

namespace EST.MIT.Web.Tests
{
    public class InvoiceStateContainerTests
    {
        [Fact]
        public void SetValue_Should_Set_Value_And_Invoke_OnStateChange()
        {
            var invoice = new Invoice();
            var stateContainer = new InvoiceStateContainer();
            var onStateChangeMock = new Mock<Action>();
            stateContainer.OnStateChange += onStateChangeMock.Object;
            stateContainer.SetValue(invoice);

            Assert.Equal(invoice, stateContainer.Value);
            onStateChangeMock.Verify(x => x.Invoke(), Times.Once);
        }
    }
}
