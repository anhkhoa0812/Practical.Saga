using Microsoft.EntityFrameworkCore;
using PaymentApi.Entities;
using PaymentApi.Extensions;
using PaymentApi.GrpcService;
using PaymentApi.Repositories;
using PaymentApi.Service;
using Shared.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContextPool<PaymentDbContext>(dbContextOptionsBuilder =>
{
    dbContextOptionsBuilder.UseNpgsql(
        builder.Configuration.GetConnectionString("postgres"));
});
builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
// builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Services.AddGrpc();
builder.Services.Configure<MomoOptions>(builder.Configuration.GetSection("MomoOptions"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGrpcService<PaymentGrpcService>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
