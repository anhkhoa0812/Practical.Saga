using Carter;
using ProductApi.Extensions;
using ProductApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.ConfigureKafka(builder.Configuration);
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapCarter();
app.UseHttpsRedirection();


app.Run();

