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
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


//.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))

// options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration(builder.Configuration.GetSection("AzureAd").;

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));


//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
//    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { $"https://test.api.crm.dynamics.com/user_impersonation" })
//    .AddInMemoryTokenCaches();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddAzureServices(config);
builder.Services.AddAPIServices();
builder.Services.AddRepoServices();

builder.Services.AddScoped<IInvoiceStateContainer, InvoiceStateContainer>();
builder.Services.AddScoped<INavigationStateContainer, NavigationStateContainer>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IPageServices, PageServices>();
builder.Services.AddScoped<IFindService, FindService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IUploadAPI, UploadAPI>();
builder.Services.AddScoped<IUploadRepository, UploadRepository>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireRole("ReadOnly").Build();
});

//builder.Services.AddTokenAcquisition()
//    .AddInMemoryTokenCaches();


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
