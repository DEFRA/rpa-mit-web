using System.Diagnostics;
using EST.MIT.Web.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EST.MIT.Web.Test
{
    public class ErrorModelTests
    {
        private readonly Mock<ILogger<ErrorModel>> _loggerMock;
        private readonly ErrorModel _errorModel;

        public ErrorModelTests()
        {
            _loggerMock = new Mock<ILogger<ErrorModel>>();
            _errorModel = new ErrorModel(_loggerMock.Object);
        }


        [Fact]
        public void ShowRequestId_Should_Return_False_When_RequestId_Is_Null_Or_Empty()
        {
            _errorModel.RequestId = null;
            var showRequestId = _errorModel.ShowRequestId;

            Assert.False(showRequestId);
            _errorModel.RequestId = string.Empty;

            showRequestId = _errorModel.ShowRequestId;
            Assert.False(showRequestId);
        }

        [Fact]
        public void ShowRequestId_Should_Return_True_When_RequestId_Is_Not_Null_Or_Empty()
        {
            _errorModel.RequestId = "test";
            var showRequestId = _errorModel.ShowRequestId;
            Assert.True(showRequestId);
        }

        [Fact]
        public void OnGet_Should_SetRequestId()
        {
            var sut = new ErrorModel(_loggerMock.Object);
            var activity = new Activity("testActivityId");
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "testTraceId";
            activity.AddTag("RequestId", "testActivityId");
            Activity.Current = activity;
            sut.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            sut.OnGet();

            Assert.Equal("testTraceId", sut.RequestId);
        }

    }
}