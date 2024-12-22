using IAMS.API.Data;
using IAMS.Model;
using Microsoft.EntityFrameworkCore;
using IAMS.API.Endpoints;
using IAMS.API.Repositories.Contract;
using IAMS.API.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<IAMSDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    //options.UseInMemoryDatabase("IAMSDataBase");
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  //  app.UseSwagger();
   // app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/security/getMessage",() => "Hello World!").RequireAuthorization();

app.RegisterCountryEndpoints();
app.RegisterStateEndpoints();
app.RegisterAddressEndpoints();

app.Run();
