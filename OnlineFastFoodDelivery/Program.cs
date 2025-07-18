using Microsoft.EntityFrameworkCore;
using OnlineFastFoodDelivery;
using Stripe;
using Models;
using BLL.Interfaces;
using BLL.Implementation;
using Microsoft.SemanticKernel;
using Mediator.Query;
using Mediator.Command;
using Mediator.Handler;
using System.Reflection;
using Mediator.Query.Home;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();//registering the context accessor 




string azureOpenAIKey = string.Empty;

//registering gemini and kernel
builder.Services.AddSingleton(provider =>
{
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(
    deploymentName: "gpt-4",
   endpoint: "https://aksha-mcugjuq9-eastus2.cognitiveservices.azure.com/",
   apiKey: azureOpenAIKey
    );

    var kernel = builder.Build();
    return kernel;
});



//registering mediatR

builder.Services.AddMediatR(cfng =>
{
    var mediatorAssembly = Assembly.Load("Mediator");
    //cfng.RegisterServicesFromAssembly(Assembly.GetAssembly
    //    (Mediator));
    cfng.RegisterServicesFromAssemblies(mediatorAssembly);
});



//email Configuration
var emailConfig = builder.Configuration.GetSection("MailSettings").Get<MailSettings>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<HomePageDAO, HomePageDao>();
builder.Services.AddScoped<UserLogin, UserLoginDao>();
builder.Services.AddScoped<SendMailDAO, SendMailDao>();
builder.Services.AddScoped<OrderStatusDAO,OrderStatusDao>();
builder.Services.AddScoped<CartDAO, CartDao>();
builder.Services.AddScoped<CheckoutDAO, CheckoutDao>();


//DBCONtext
var provider = builder.Services.BuildServiceProvider();
var config=provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<Online_Food_ApplicationContext>(item => item.UseSqlServer(config.GetConnectionString("con")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
