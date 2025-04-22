using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using OrderApi.Commands.Handlers;
using OrderApi.Entities;
using OrderApi.Extensions;
using OrderApi.Repositories;
using Shared.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<OrderDbContext>(dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(
        builder.Configuration.GetConnectionString("postgres"));
});
builder.Services.Configure<GrpcSettings>(builder.Configuration.GetSection("GrpcSettings"));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddMediatR(clf => clf.RegisterServicesFromAssemblyContaining<CreateOrderCommandHandler>());
builder.Services.AddCustomKafka(builder.Configuration);
builder.Services.ConfigureGrpcServices(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

