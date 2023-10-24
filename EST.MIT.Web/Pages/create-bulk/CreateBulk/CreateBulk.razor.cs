﻿using Entities;
using EST.MIT.Web.Shared;
using Microsoft.AspNetCore.Components;

namespace EST.MIT.Web.Pages.create_bulk.CreateBulk
{
    public partial class CreateBulk
    {
        [Inject] private NavigationManager _nav { get; set; }
        [Inject] private IInvoiceStateContainer _invoiceStateContainer { get; set; }

        private void Start()
        {
            _invoiceStateContainer.SetValue(new Invoice());
            _nav.NavigateTo("/create-bulk/account");
        }

        private void Cancel()
        {
            _invoiceStateContainer.SetValue(null);
            _nav.NavigateTo("/");
        }
    }
}