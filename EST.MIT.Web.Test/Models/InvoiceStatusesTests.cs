using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EST.MIT.Web.Models;
using Xunit;

namespace EST.MIT.Web.Test.Models;

public class InvoiceStatusesTests
{
    [Fact]
    public void DisplayNameFor_New_ReturnsNew()
    {
        var status = InvoiceStatuses.New;

        var result = InvoiceStatuses.DisplayNameFor(status);

        Assert.Equal("New", result);
    }

    [Fact]
    public void DisplayNameFor_AwaitingApproval_ReturnsAwaitingApproval()
    {
        var status = InvoiceStatuses.AwaitingApproval;

        // 
        var result = InvoiceStatuses.DisplayNameFor(status);

        Assert.Equal("Awaiting Approval", result);
    }

    [Fact]
    public void DisplayNameFor_Approved_ReturnsApproved()
    {
        var status = InvoiceStatuses.Approved;

        var result = InvoiceStatuses.DisplayNameFor(status);

        Assert.Equal("Approved", result);
    }

    [Fact]
    public void DisplayNameFor_Rejected_ReturnsRejected()
    {
        var status = InvoiceStatuses.Rejected;

        var result = InvoiceStatuses.DisplayNameFor(status);

        Assert.Equal("Rejected", result);
    }

    [Fact]
    public void DisplayNameFor_UnknownStatus_ReturnsSameStatus()
    {
        var unknownStatus = "UNKNOWN_STATUS";

        var result = InvoiceStatuses.DisplayNameFor(unknownStatus);

        Assert.Equal(unknownStatus, result);
    }
}