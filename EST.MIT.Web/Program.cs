using AutoMapper;
using EST.MIT.Web.Collections;
using EST.MIT.Web.AutoMapperProfiles;
using EST.MIT.Web.Shared;
using EST.MIT.Web.Repositories;
using EST.MIT.Web.Services;
using EST.MIT.Web.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using EST.MIT.Web.APIs;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//TODO: Uncomment the following line to enable authentication
var initialScopes = builder.Configuration["MitWebApi"]?.Split(' ');
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddInMemoryTokenCaches();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
        .AddMicrosoftIdentityConsentHandler();

builder.Services.AddAzureServices(config);
builder.Services.AddAPIServices();
builder.Services.AddRepoServices();

builder.Services.AddScoped<IInvoiceStateContainer, InvoiceStateContainer>();
builder.Services.AddScoped<INavigationStateContainer, NavigationStateContainer>();
builder.Services.AddSingleton<IUploadService, UploadService>();
builder.Services.AddSingleton<IPageServices, PageServices>();
builder.Services.AddSingleton<IFindService, FindService>();
builder.Services.AddSingleton<IApprovalService, ApprovalService>();
builder.Services.AddSingleton<IUploadAPI, UploadAPI>();
builder.Services.AddSingleton<IUploadRepository, UploadRepository>();
builder.Services.AddSingleton<IServiceBusProvider, ServiceBusProvider>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();

//TODO: Uncomment the following line to enable authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new InvoiceAPIMapper());
});
IMapper autoMapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(autoMapper);

builder.Services.AddHttpClient("InvoiceAPI", client =>
{
    //TODO: Confirm the correct base address
    client.BaseAddress = new Uri(config["InvoiceAPIBaseURI"]);
});

builder.Services.AddHttpClient("ApprovalAPI", client =>
{
    //TODO: Confirm the correct base address
    client.BaseAddress = new Uri(config["ApprovalAPIBaseURI"]);
});

builder.Services.AddHttpClient("ReferenceDataAPI", client =>
{
    //TODO: Confirm the correct base address
    client.BaseAddress = new Uri(config["ReferenceDataAPIBaseURI"]);
});

builder.Services.AddHttpClient("InvoiceImporterAPI", client =>
{
    //TODO: Confirm the correct base address
    client.BaseAddress = new Uri(config["InvoiceImporterAPIBaseURI"]);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
